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
using System.ComponentModel.Composition;
using System.Timers;
using System.Windows.Media;
using CShell.Framework;
using CShell.Framework.Services;
using CShell.Modules.Repl.Controls;
using CShell.Modules.Repl.Views;
using Caliburn.Micro;
using Execute = CShell.Framework.Services.Execute;

namespace CShell.Modules.Repl.ViewModels
{
    [Export(typeof(ReplViewModel))]
    [Export(typeof(IRepl))]
    [Export(typeof(ITool))]
    public class ReplViewModel : Tool, IRepl, IHandle<WorkspaceOpeningEventArgs>, IHandle<WorkspaceClosedEventArgs>
    {
        private readonly Timer timer;
        private IRepl internalRepl;
        private IReplView replView;

        [Import] private IShell shell;

        [ImportingConstructor]
        public ReplViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);

            DisplayName = "C# Interactive";

            timer = new Timer(100);
            timer.AutoReset = true;
            timer.Elapsed += TimerOnElapsed;
        }
        
        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/CShell;component/Resources/Icons/Output.png"); }
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Bottom; }
        }

        public override Uri Uri
        {
            get { return new Uri("tool://cshell/repl"); }
        }

        protected override void OnViewLoaded(object view)
        {
            replView = (IReplView) view;
            internalRepl = replView.GetRepl();

            timer.Start();
        }

        protected override void OnDeactivate(bool close)
        {
            if(close)
                timer.Dispose();
        }

        private DateTime lastTimeNoEvaluations;
        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if(!internalRepl.IsEvaluating)
            {
                lastTimeNoEvaluations = DateTime.Now;
                if(shell.StatusBar.Message != "Ready")
                {
                    shell.StatusBar.UpdateMessage();
                    shell.StatusBar.UpdateProgress(false);
                }
            }
            else
            {
                var evaluatingTime = DateTime.Now - lastTimeNoEvaluations;
                if(evaluatingTime.TotalSeconds > 0.5)
                {
                    shell.StatusBar.UpdateMessage("Running...");
                    shell.StatusBar.UpdateProgress(true);
                }
            }
        }

        public void Handle(WorkspaceOpeningEventArgs message)
        {
            replView.ScriptingEngine = message.Workspace.ScriptingEngine;
        }

        public void Handle(WorkspaceClosedEventArgs message)
        {
            replView.ScriptingEngine = null;
        }

        #region IRepl wrapper implementaion
        public void Clear()
        {
            Execute.OnUIThread(()=>internalRepl.Clear());
        }

        public bool IsEvaluating
        {
            get { return internalRepl.IsEvaluating; }
        }

        public IEnumerable<string> SuppressedWarnings
        {
            get { return internalRepl.SuppressedWarnings; }
        }

        public void SuppressWarning(string warningCode)
        {
            internalRepl.SuppressWarning(warningCode);
        }

        public void ShowWarning(string warningCode)
        {
            internalRepl.ShowWarning(warningCode);
        }

        public bool ShowConsoleOutput
        {
            get { return internalRepl.ShowConsoleOutput; }
            set { Execute.OnUIThread(()=>internalRepl.ShowConsoleOutput = value); }
        }

        public string Font
        {
            get { return internalRepl.Font; }
            set { Execute.OnUIThread(()=>internalRepl.Font = value); }
        }

        public double FontSize
        {
            get { return internalRepl.FontSize; }
            set { Execute.OnUIThread(()=>internalRepl.FontSize = value); }
        }

        public System.Windows.Media.Color BackgroundColor
        {
            get { return internalRepl.BackgroundColor; }
            set { Execute.OnUIThread(()=>internalRepl.BackgroundColor = value); }
        }

        public System.Windows.Media.Color OutputColor
        {
            get { return internalRepl.OutputColor; }
            set { Execute.OnUIThread(()=>internalRepl.OutputColor = value); }
        }

        public System.Windows.Media.Color WarningColor
        {
            get { return internalRepl.WarningColor; }
            set { Execute.OnUIThread(()=>internalRepl.WarningColor = value); }
        }

        public System.Windows.Media.Color ErrorColor
        {
            get { return internalRepl.ErrorColor; }
            set { Execute.OnUIThread(()=>internalRepl.ErrorColor = value); }
        }

        public System.Windows.Media.Color ReplColor
        {
            get { return internalRepl.ReplColor; }
            set { Execute.OnUIThread(()=>internalRepl.ReplColor = value); }
        }
        #endregion

    }//end class
}
