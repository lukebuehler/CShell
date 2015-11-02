using System.Windows;
using System.Windows.Controls;
using CShell.Framework.Menus;

namespace CShell.Framework.Controls
{
	public class ToolBar : System.Windows.Controls.ToolBar
	{
		private object _currentItem;

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			_currentItem = item;
			return base.IsItemItsOwnContainerOverride(item);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			if(_currentItem is MenuItemSeparator)
                return new Separator { Style = (Style)FindResource(SeparatorStyleKey) };
		    return base.GetContainerForItemOverride();
		}
	}
}