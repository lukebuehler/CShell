#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2012  Arnova Asset Management Ltd., Lukas Buhler
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
using System.IO;
using System.Linq;
using System.Text;
using CShell.Framework.Results;
using CShell.Modules.Workspace.ViewModels;

namespace CShell.Modules.Workspace.Results
{
    public class AddFolderResult : ResultBase
    {
        private readonly string folder;

        public AddFolderResult(string folder)
        {
            this.folder = folder;
        }

        public override void Execute(Caliburn.Micro.CoroutineExecutionContext context)
        {
            ////add the whole folder
            //if(Directory.Exists(folder))
            //{
            //    references.AddFilesAsGroup(folder);
            //}

            ////ceate a new group that doesnt correspond with a particular folder, add it and make it editable
            //else if(parent != null)
            //{
            //    var groupName = DefaultGroupName;
            //    var counter = 1;
            //    while (references.Groups.Contains(groupName))
            //    {
            //        groupName = DefaultGroupName + counter;
            //        counter++;
            //    }
            //    references.Groups.Add(new FileReferences(groupName));
            //    //find the new groupVM and make it editable
            //    var newGroupVM = parent.Children.FirstOrDefault(item => item is FileReferencesViewModel && item.DisplayName == groupName);
            //    if (newGroupVM != null)
            //        newGroupVM.IsInEditMode = true;
            //}
           
        }
    }
}
