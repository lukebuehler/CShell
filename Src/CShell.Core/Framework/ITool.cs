using System;
using System.Windows.Input;
using CShell.Framework.Services;
using Caliburn.Micro;

namespace CShell.Framework
{
	public interface ITool : IScreen
	{
        Uri Uri { get; }
		ICommand CloseCommand { get; }
		PaneLocation PreferredLocation { get; }
		bool IsVisible { get; set; }
	}
}