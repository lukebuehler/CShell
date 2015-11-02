using System.Collections.Generic;
using Caliburn.Micro;

namespace CShell.Framework.Menus
{
	public interface IMenu : IObservableCollection<MenuItemBase>
	{
		IEnumerable<MenuItemBase> All { get; }
	}
}