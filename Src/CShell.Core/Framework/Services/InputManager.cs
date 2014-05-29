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
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Caliburn.Micro;

namespace CShell.Framework.Services
{
	[Export(typeof(IInputManager))]
	public class InputManager : IInputManager
	{
		public void SetShortcut(DependencyObject view, InputGesture gesture, object handler)
		{
			var inputBindingTrigger = new InputBindingTrigger();
			inputBindingTrigger.InputBinding = new InputBinding(new RoutedCommand(), gesture);

			//var target = ViewLocator.LocateForModel(handler, null, null);
			Interaction.GetTriggers(view).Add(inputBindingTrigger);

			inputBindingTrigger.Actions.Add(new TestTriggerAction(handler));
		}

		public void SetShortcut(InputGesture gesture, object handler)
		{
			SetShortcut(Application.Current.MainWindow, gesture, handler);
		}

		private class TestTriggerAction : TriggerAction<FrameworkElement>
		{
			private readonly object _handler;

			public TestTriggerAction(object handler)
			{
				_handler = handler;
			}

			protected override void Invoke(object parameter)
			{
				Action.Invoke(_handler, "Execute");
			}
		}
	}
}