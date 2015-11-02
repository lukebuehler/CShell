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
