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
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml;
using CShell.Framework.Menus;
using Caliburn.Micro;

namespace CShell.Framework.Services
{
    public interface IShell : IConductor
	{
		string Title { get; set; }
		ImageSource Icon { get; set; }
        IMenu MainMenu { get; }
        IToolBar ToolBar { get; }
		IStatusBar StatusBar { get; }

		IScreen ActiveItem { get; }
        IEnumerable<IDocument> Documents { get; }

        IEnumerable<ITool> Tools { get; }
        void ShowTool(ITool model);

		void OpenDocument(IDocument model);
		void CloseDocument(IDocument document);
		void ActivateDocument(IDocument document);

        void Opened(string[] args);
		void Close();

        void SaveLayout(XmlWriter xmlWriter);
        void LoadLayout(XmlReader xmlReader);
	}
}