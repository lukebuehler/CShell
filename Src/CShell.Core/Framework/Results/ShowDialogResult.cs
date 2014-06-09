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
using Caliburn.Micro;

namespace CShell.Framework.Results
{
    public class ShowDialogResult : ResultBase
    {
        private readonly Type dialogViewModelType;
        private object dialogViewModel;

        public IDictionary<string, object> Settings { get; set; } 

        public ShowDialogResult(object dialogViewModel)
        {
            this.dialogViewModel = dialogViewModel;
        }

        public ShowDialogResult(Type dialogViewModelType)
        {
            this.dialogViewModelType = dialogViewModelType;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            var windowManager = IoC.Get<IWindowManager>();
            if(dialogViewModel == null)
            {
                dialogViewModel = IoC.GetInstance(dialogViewModelType, null);
            }
            var result = windowManager.ShowDialog(dialogViewModel, settings:Settings);
            OnCompleted(null, result.HasValue && !result.Value);
        }
    }
}
