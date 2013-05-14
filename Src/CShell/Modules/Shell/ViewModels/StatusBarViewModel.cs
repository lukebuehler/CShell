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
using System.ComponentModel.Composition;
using CShell.Framework.Services;
using Caliburn.Micro;
using Execute = CShell.Framework.Services.Execute;

namespace CShell.Modules.Shell.ViewModels
{
	[Export(typeof(IStatusBar))]
	public class StatusBarViewModel : PropertyChangedBase, IStatusBar
	{
        private readonly object syncRoot = new object();
	    private const string DefaultMessage = "Ready";

		private string message;
		public string Message
		{
            get { lock(syncRoot) return message; }
		}

        public void UpdateMessage()
        {
            UpdateMessage(DefaultMessage);
        }

        public void UpdateMessage(string message)
        {
            lock (syncRoot)
            {
                this.message = message;
            }
            Execute.OnUIThread(()=>NotifyOfPropertyChange(() => Message));
        }

	    private int progress = 0;
        public int Progress
        {
            get { lock (syncRoot) return progress; }
        }

	    private bool showingProgress;
        public bool ShowingProgress
        {
            get { lock (syncRoot) return showingProgress; }
        }

        public bool IndeterminateProgress
        {
            get { lock (syncRoot) return progress <= 0; }
        }

        public void UpdateProgress(bool running)
        {
            lock (syncRoot)
            {
                showingProgress = running;
                if (!running)
                    progress = 0;
            }
            NotifyOfPropertyChange(() => Progress);
            NotifyOfPropertyChange(() => ShowingProgress);
            NotifyOfPropertyChange(() => IndeterminateProgress);
        }

        public void UpdateProgress(int progress)
        {
            var prog = progress;
            if (progress < 0)
                prog = 0;
            else if (progress > 100)
                prog = 100;

            lock (syncRoot)
            {
                showingProgress = true;
                this.progress = prog;
            }
            NotifyOfPropertyChange(() => Progress);
            NotifyOfPropertyChange(() => ShowingProgress);
            NotifyOfPropertyChange(() => IndeterminateProgress);
        }
    }
}