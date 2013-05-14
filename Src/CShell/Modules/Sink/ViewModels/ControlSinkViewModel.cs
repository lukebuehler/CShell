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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace CShell.Modules.Sink.ViewModels
{
    public class ContolWpfViewModel
    {
        public Control Control { get;private set; }

        public ContolWpfViewModel(Control control, string desciption)
        {
            Control = control;
        }
    }

    public class ControlSinkViewModel : Framework.Sink
    {
        public ControlSinkViewModel(Uri uri)
        {
            Uri = uri;
            DisplayName = GetTitle(uri, "Control");
        }
        private object controlViewModel;
        public object ControlViewModel
        {
            get { return controlViewModel; }
            set { controlViewModel = value; NotifyOfPropertyChange(()=>ControlViewModel); }
        }

        public override void Dump(object o, string description)
        {
            var control = o as Control;
            //TODO: support actions that create controls, so that we can execute it on the UI thread
            // this might be simple using contravariance
            // see: http://stackoverflow.com/questions/4669858/simple-examples-of-co-and-contravariance
            if(o is Action<Control>)
            { }

            if(o is Control)
            {
                ControlViewModel = new ContolWpfViewModel((Control) o, description);
            }
        }

        public override void Clear()
        {
            ControlViewModel = null;
        }
    }
}
