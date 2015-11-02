using System.Collections.Generic;
using Caliburn.Micro;

namespace CShell.Framework.Menus
{
	public interface IToolBar : IObservableCollection<MenuItemBase>
	{
	    void Add(params MenuItemBase[] menuItems);
	}
}