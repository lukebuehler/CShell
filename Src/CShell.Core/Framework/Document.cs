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
using System.Windows.Input;

namespace CShell.Framework
{
	public abstract class Document : LayoutItemBase, IDocument
	{
		private ICommand _closeCommand;
		public ICommand CloseCommand
		{
			get
			{
			    return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(), p => true));
                
                //documents can be closed all the time, if dirty, a dialog will be presented to ask if close is allowed
                //{
                //    bool canClose = false;
                //    CanClose(r => canClose = r);
                //    return canClose;
                //});
			}
		}

        public virtual Uri Uri { get; set; }
        public virtual bool IsDirty { get; set; }

        public virtual void Save()
        { }
        public virtual void SaveAs(string newFile)
        { }
    }
}