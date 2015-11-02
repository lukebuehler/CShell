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