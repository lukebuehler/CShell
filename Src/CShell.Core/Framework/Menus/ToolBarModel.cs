using System.Collections.Generic;
using Caliburn.Micro;

namespace CShell.Framework.Menus
{
    public class ToolBarModel : BindableCollection<MenuItemBase>, IToolBar
	{
		public void Add(params MenuItemBase[] items)
		{
			items.Apply(Add);
		}
	}
}