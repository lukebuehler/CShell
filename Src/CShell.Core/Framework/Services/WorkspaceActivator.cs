using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace CShell.Framework.Services
{
    /// <summary>
    /// Helper interface to activate and deactivate UI item depending if a workspace is open.
    /// </summary>
    public interface IWorkspaceActivator : IActivate, IDeactivate
    { }
    
    [Export(typeof(IWorkspaceActivator))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class WorkspaceActivator : IWorkspaceActivator, IHandle<WorkspaceOpenedEventArgs>
    {
        [ImportingConstructor]
        public WorkspaceActivator(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
        }

        public void Activate()
        {
            isActive = true;
            if(Activated != null)
                Activated(this, new ActivationEventArgs());
        }

        public event EventHandler<ActivationEventArgs> Activated;

        private bool isActive = false;
        public bool IsActive
        {
            get { return isActive; }
        }

        public event EventHandler<DeactivationEventArgs> AttemptingDeactivation;

        public void Deactivate(bool close)
        {
            isActive = false;

            if (AttemptingDeactivation != null)
                AttemptingDeactivation(this, new DeactivationEventArgs(){WasClosed = false});

            if (Deactivated != null)
                Deactivated(this, new DeactivationEventArgs() { WasClosed = true });
        }

        public event EventHandler<DeactivationEventArgs> Deactivated;



        public void Handle(WorkspaceOpenedEventArgs message)
        {
            Activate();
        }
    }
}
