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
    public class WorkspaceActivator : IWorkspaceActivator, IHandle<WorkspaceOpenedEventArgs> , IHandle<WorkspaceClosingEventArgs>
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

        public void Handle(WorkspaceClosingEventArgs message)
        {
            Deactivate(true);
        }
    }
}
