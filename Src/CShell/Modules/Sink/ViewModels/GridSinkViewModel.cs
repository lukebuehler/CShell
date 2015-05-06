#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace CShell.Modules.Sink.ViewModels
{
    public class GridSinkViewModel : Framework.Sink
    {
        public GridSinkViewModel(Uri uri)
        {
            Uri = uri;

            DisplayName = GetTitle(uri, "Grid");
            data = new List<object>();
        }

        public override void Dump(object o, string description)
        {
            var enumerable = o as IEnumerable;
            if (enumerable != null)
            {
                var d = enumerable.Cast<object>().ToList();
                //get the type to see if it's only a primitive list or if it contains objects
                var first = d.FirstOrDefault();
                //if it's a simple type, we'll try to present it better
                if(first != null && IsSimpleType(first.GetType()))
                {
                    Data = d.Select(v => new {Value = v}).Cast<object>().ToList();
                }
                else
                {
                    Data = d;
                }
            }
        }

        public override void Clear()
        {
            Data = new List<object>();
        }

        private List<object> data;
        public List<object> Data
        {
            get { return data; }
            set
            {
                data = value;

                if (lastItemType == null || lastItemType != ItemType)
                    InitializeItemProperties();

                NotifyOfPropertyChange(() => Data);
                NotifyOfPropertyChange(() => DataCount);
            }
        }

        public int DataCount
        {
            get { return Data.Count; }
        }

        #region Type Properties
        private Type lastItemType;
        public Type ItemType
        {
            get
            {
                var firstItem = Data.FirstOrDefault();
                if (firstItem != null)
                    return firstItem.GetType();
                return null;
            }
        }

        public Collection<PropertyInfo> Properties { get; private set; }
        public Collection<PropertyInfo> SelectedProperties { get; private set; }

        private void InitializeItemProperties()
        {
            lastItemType = ItemType;
            Properties = new Collection<PropertyInfo>(GetProperties(ItemType));

            SelectedProperties = new Collection<PropertyInfo>(Properties
                .Where(p=>
                       {
                           var browsable = p.GetAttributes<BrowsableAttribute>(true);
                           return !browsable.Any() || browsable.First().Browsable;
                       }).ToList());

            NotifyOfPropertyChange(() => ItemType);
            NotifyOfPropertyChange(() => SelectedProperties);
            NotifyOfPropertyChange(() => Properties);
        }
        #endregion

        #region Helpers
        public static IList<PropertyInfo> GetProperties(Type itemType)
        {
            var itemProperties = new List<PropertyInfo>();
            var properties = itemType.GetProperties();
            itemProperties.AddRange(properties);
            return itemProperties;
        }

        public static IList<PropertyInfo> GetValueProperties(Type itemType)
        {
            return GetProperties(itemType)
                .Where(p => IsValueType(p.PropertyType)).ToList();
        }

        private static bool IsValueType(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        }

        private static bool IsSimpleType(Type type)
        {
            return IsValueType(type) ||
                   type == typeof (DateTime) ||
                   type == typeof (TimeSpan) ||
                   type == typeof (string);
        }
        #endregion

    }
}
