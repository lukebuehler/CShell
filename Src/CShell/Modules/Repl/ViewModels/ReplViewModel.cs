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
using ScriptCs.Contracts;
using Execute = CShell.Framework.Services.Execute;

namespace CShell.Modules.Repl.ViewModels
{
    [Export(typeof(ReplViewModel))]
    [Export(typeof(IReplOutput))]
    [Export(typeof(ITool))]
    public class ReplViewModel : Tool, IReplOutput
    {
        private readonly Timer timer;
        private IReplOutput internalReplOutput;
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
            internalReplOutput = replView.GetReplOutput();

            timer.Start();
            base.OnViewLoaded(view);
        }

        protected override void OnDeactivate(bool close)
        {
            if(close)
                timer.Dispose();
        }

        private DateTime lastTimeNoEvaluations;
        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if(!internalReplOutput.IsEvaluating)
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


        #region IRepl wrapper implementaion
        public void Initialize(IReplScriptExecutor replExecutor)
        {
            Execute.OnUIThread(() => internalReplOutput.Initialize(replExecutor));
        }

        public void EvaluateStarted(string input, string sourceFile)
        {
            Execute.OnUIThread(() => internalReplOutput.EvaluateStarted(input, sourceFile));
        }

        public void EvaluateCompleted(ScriptResult result)
        {
            Execute.OnUIThread(() => internalReplOutput.EvaluateCompleted(result));
        }

        public void Clear()
        {
            Execute.OnUIThread(()=>internalReplOutput.Clear());
        }

        public bool IsEvaluating
        {
            get { return internalReplOutput.IsEvaluating; }
        }

        public void Write(string value)
        {
            Execute.OnUIThread(() => internalReplOutput.Write(value));
        }

        public void WriteLine()
        {
            Execute.OnUIThread(() => internalReplOutput.WriteLine());
        }

        public void WriteLine(string value)
        {
            Execute.OnUIThread(() => internalReplOutput.WriteLine(value));
        }

        public void Write(string format, params object[] arg)
        {
            Execute.OnUIThread(() => internalReplOutput.Write(format, arg));
        }

        public void WriteLine(string format, params object[] arg)
        {
            Execute.OnUIThread(() => internalReplOutput.WriteLine(format, arg));
        }

        public int BufferLength
        {
            get { return internalReplOutput.BufferLength; }
            set { Execute.OnUIThread(() => internalReplOutput.BufferLength = value); }
        }

        public IEnumerable<string> SuppressedWarnings
        {
            get { return internalReplOutput.SuppressedWarnings; }
        }

        public void SuppressWarning(string warningCode)
        {
            internalReplOutput.SuppressWarning(warningCode);
        }

        public void ShowWarning(string warningCode)
        {
            internalReplOutput.ShowWarning(warningCode);
        }

        public void ResetColor()
        {
            Execute.OnUIThread(() => internalReplOutput.ResetColor());
        }

        public bool ShowConsoleOutput
        {
            get { return internalReplOutput.ShowConsoleOutput; }
            set { Execute.OnUIThread(()=>internalReplOutput.ShowConsoleOutput = value); }
        }

        public string Font
        {
            get { return internalReplOutput.Font; }
            set { Execute.OnUIThread(()=>internalReplOutput.Font = value); }
        }

        public double FontSize
        {
            get { return internalReplOutput.FontSize; }
            set { Execute.OnUIThread(()=>internalReplOutput.FontSize = value); }
        }

        public System.Windows.Media.Color BackgroundColor
        {
            get { return internalReplOutput.BackgroundColor; }
            set { Execute.OnUIThread(()=>internalReplOutput.BackgroundColor = value); }
        }

        public System.Windows.Media.Color ResultColor
        {
            get { return internalReplOutput.ResultColor; }
            set { Execute.OnUIThread(()=>internalReplOutput.ResultColor = value); }
        }

        public System.Windows.Media.Color WarningColor
        {
            get { return internalReplOutput.WarningColor; }
            set { Execute.OnUIThread(()=>internalReplOutput.WarningColor = value); }
        }

        public System.Windows.Media.Color ErrorColor
        {
            get { return internalReplOutput.ErrorColor; }
            set { Execute.OnUIThread(()=>internalReplOutput.ErrorColor = value); }
        }

        public System.Windows.Media.Color TextColor
        {
            get { return internalReplOutput.TextColor; }
            set { Execute.OnUIThread(()=>internalReplOutput.TextColor = value); }
        }
        #endregion
       
    }//end class
}
