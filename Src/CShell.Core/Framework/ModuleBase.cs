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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using CShell.Framework.Menus;
using CShell.Framework.Services;

namespace CShell.Framework
{
    public class ModuleConfiguration : IModuleConfiguration
    {
        public ModuleConfiguration(CompositionBatch compositionBatch)
        {
            CompositionBatch = compositionBatch;
            References = new List<string>();
            Namespaces = new List<string>();
            
        }
        public System.Collections.Generic.IList<string> References { get; private set; }
        public System.Collections.Generic.IList<string> Namespaces { get; private set; }
        public CompositionBatch CompositionBatch { get; private set; }
    }

	public abstract class ModuleBase : IModule
	{
		[Import]
		private IShell _shell;

		protected IShell Shell
		{
			get { return _shell; }
		}

		protected IMenu MainMenu
		{
			get { return _shell.MainMenu; }
		}

        protected IToolBar ToolBar
        {
            get { return _shell.ToolBar; }
        }

        public int Order { get; set; }

        public abstract void Configure(IModuleConfiguration configuration);
        public abstract void Start();

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {}

	}
}