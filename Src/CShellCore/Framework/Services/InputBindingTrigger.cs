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
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CShell.Framework.Services
{
	public class InputBindingTrigger : TriggerBase<FrameworkElement>, ICommand
	{
		public static readonly DependencyProperty InputBindingProperty =
			DependencyProperty.Register("InputBinding", typeof(InputBinding), 
			typeof(InputBindingTrigger), new UIPropertyMetadata(null));

		public InputBinding InputBinding
		{
			get { return (InputBinding)GetValue(InputBindingProperty); }
			set { SetValue(InputBindingProperty, value); }
		}

		protected override void OnAttached()
		{
			if (InputBinding != null)
			{
				InputBinding.Command = this;
				AssociatedObject.InputBindings.Add(InputBinding);
			}
			base.OnAttached();
		}

		#region ICommand Members
		public bool CanExecute(object parameter)
		{
			// action is anyway blocked by Caliburn at the invoke level
			return true;
		}
		public event EventHandler CanExecuteChanged = delegate { };

		public void Execute(object parameter)
		{
			InvokeActions(parameter);
		}

		#endregion
	}
}