using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CShell.Framework.Services;
using Xceed.Wpf.AvalonDock.Layout;

namespace CShell.Framework.Controls
{
	public class LayoutInitializer : ILayoutUpdateStrategy
	{
        //http://avalondock.codeplex.com/wikipage?title=AvalonDock%202.0%20Getting%20Start%20Guide&referringTitle=Documentation
		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
            if (anchorableToShow.Content is ITool)
            {
                var preferredLocation = ((ITool) anchorableToShow.Content).PreferredLocation;
                string paneName = GetPaneName(preferredLocation);
                var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == paneName);
                if (toolsPane == null)
                {
                    switch (preferredLocation)
                    {
                        case PaneLocation.Left:
                        {
                            //TODO: this should use two steps: first, try to add to existing "LayoutAnchorablePane" if not create layoutAnchorGroup like below
                            var layoutAnchorSide = layout.Descendents().OfType<LayoutAnchorSide>().First(side => side.Side == AnchorSide.Left);
                            var layoutAnchorGroup = new LayoutAnchorGroup();
                            layoutAnchorGroup.InsertChildAt(0, anchorableToShow);
                            layoutAnchorSide.InsertChildAt(0, layoutAnchorGroup);
                            anchorableToShow.AutoHideWidth = 200;

                            //var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == Orientation.Horizontal);
                            //toolsPane = new LayoutAnchorablePane { DockWidth = new GridLength(200, GridUnitType.Pixel) };
                        }
                            break;
                        case PaneLocation.Right:
                        {
                            var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == Orientation.Horizontal);
                            toolsPane = new LayoutAnchorablePane { DockWidth = new GridLength(200, GridUnitType.Pixel) };
                            parent.Children.Add(toolsPane);
                        }
                            break;
                        case PaneLocation.Bottom:
                        {
                            var ds = layout.Descendents().ToList();
                            var items = layout.Descendents().OfType<LayoutPanel>().ToList();
                            var items2 = layout.Descendents().OfType<LayoutAnchorGroup>().ToList();
                            //var parent = items2.First();
                            var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == Orientation.Vertical);
                            toolsPane = new LayoutAnchorablePane { DockHeight = new GridLength(300, GridUnitType.Pixel) };
                            parent.Children.Add(toolsPane);
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                if(toolsPane != null)
                    toolsPane.Children.Add(anchorableToShow);
                return true;
            }

			return false;
		}



		private static string GetPaneName(PaneLocation location)
		{
			switch (location)
			{
				case PaneLocation.Left:
					return "LeftPane";
				case PaneLocation.Right:
					return "RightPane";
				case PaneLocation.Bottom:
					return "BottomPane";
				default:
					throw new ArgumentOutOfRangeException("location");
			}
		}

		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
			
		}

	    public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
	    {
	        return false;
	    }

	    public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
	    {
	    }
	}
}