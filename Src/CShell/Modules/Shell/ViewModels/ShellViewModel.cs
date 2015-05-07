#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler
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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using CShell.Framework;
using CShell.Framework.Menus;
using CShell.Framework.Results;
using CShell.Framework.Services;
using CShell.Modules.Shell.Views;
using CShell.Properties;
using Caliburn.Micro;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Action = System.Action;
using Execute = CShell.Framework.Services.Execute;

namespace CShell.Modules.Shell.ViewModels
{
	[Export(typeof(IShell))]
	public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell
	{
	    private bool closing = false;
	    private IShellView shellView;
        private readonly ILog log = LogManager.GetLog(typeof(IShell));

        public ShellViewModel()
        {
            _tools = new BindableCollection<ITool>();
        }

        protected override void OnViewLoaded(object view)
        {
            this.Title = "CShell";
            this.StatusBar.UpdateMessage("Ready");
            this.StatusBar.UpdateProgress(false);
            //this.Icon = _resourceManager.GetBitmap("Icon.ico", Assembly.GetExecutingAssembly().GetAssemblyName());
            shellView = view as IShellView;
            base.OnViewLoaded(view);
        }

        #region Properties
        private string _title = "[Default Title]";
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				NotifyOfPropertyChange(() => Title);
			}
		}

		private ImageSource _icon;
		public ImageSource Icon
		{
			get { return _icon; }
			set
			{
				_icon = value;
				NotifyOfPropertyChange(() => Icon);
			}
		}

		[Import]
		private IMenu _mainMenu;
		public IMenu MainMenu
		{
			get { return _mainMenu; }
		}

        [Import]
        private IToolBar _toolBar;
        public IToolBar ToolBar
        {
            get { return _toolBar; }
        }

		[Import]
		private IStatusBar _statusBar;
		public IStatusBar StatusBar
		{
			get { return _statusBar; }
		}

		private readonly BindableCollection<ITool> _tools;
        public IObservableCollection<ITool> ToolsObservable
        {
            get { return _tools; }
        }
		public IEnumerable<ITool> Tools
		{
			get { return _tools; }
		}

        public IEnumerable<IDocument> Documents
        {
            get { return Items.Where(item => item is IDocument).Cast<IDocument>(); }
        }
        #endregion

        #region Documents & Tools
        public void ShowTool(ITool model)
		{
			if (ToolsObservable.Contains(model))
				model.IsVisible = true;
			else
                ToolsObservable.Add(model);
		}

		public void OpenDocument(IDocument model)
		{
			ActivateItem(model);
		}

		public void ActivateDocument(IDocument document)
		{
			ActivateItem(document);
		}

        public void CloseDocument(IDocument document)
        {
            DeactivateItem(document, true);
        }
        #endregion

        #region Open & Close the App
        public void Opened(string[] args)
        {
            //if a workspace was specified in the arguments open it now
            if (args != null && args.Length > 0 && !String.IsNullOrEmpty(args[0]))
            {
                var workspaceDir = args[0];
                new ChangeWorkspaceResult(workspaceDir).BeginExecute(null);
            }
            else if (Settings.Default.OpenLastWorkspace && Directory.Exists(Settings.Default.LastWorkspace))
            {
                var workspaceDir = Settings.Default.LastWorkspace;
                new ChangeWorkspaceResult(workspaceDir).BeginExecute(null);
            }
            else if (Settings.Default.IsFirstStartup)
            {
                //open the default workspace if this is the first startup
                Settings.Default.IsFirstStartup = false;
                new ChangeWorkspaceResult(Constants.DefaultWorkspacePath).BeginExecute(null);
            }
        }

	    public void Close()
		{
			Application.Current.MainWindow.Close();
		}

        protected override void OnDeactivate(bool close)
        {
            //for some reason, this sometimes crashes when the app is closed, we can actually disregard it, but for now it's looged to see when it happens
            try
            {
                base.OnDeactivate(close);
            }
            catch(Exception ex)
            {
                LogManager.GetLog(typeof(ShellViewModel)).Error(ex);
            }
        }

        public IEnumerable<IResult> Closing(CancelEventArgs e)
        {
            //this is a bit tricky,
            // for the async close process to have enough time BEFORE the app closes we need to first cancel the close 
            // and then continue once the closing of the workspace is complete, and all of this in an async matter.
            // TODO: check if there are any unsaved documents and handle that properly first.
            if (!closing)
            {
                e.Cancel = true;
                closing = true;
                yield return new ChangeWorkspaceResult(null);
                yield return new CloseShellResult();
            }
        }
        #endregion

        #region Save & Load Layout
        public void SaveLayout(XmlWriter xmlWriter)
        {
            if(shellView != null && shellView.DockingManager != null)
            {
                Execute.OnUIThreadEx(() =>
                {
                    var layoutSerializer = new XmlLayoutSerializer(shellView.DockingManager);
                    layoutSerializer.Serialize(xmlWriter);
                });
            }
        }

        public void LoadLayout(XmlReader xmlReader)
        {
            if (shellView != null && shellView.DockingManager != null)
            {
                Execute.OnUIThreadEx(() =>
                {
                    var layoutSerializer = new XmlLayoutSerializer(shellView.DockingManager);
                    //Here I've implemented the LayoutSerializationCallback just to show
                    // a way to feed layout desarialization with content loaded at runtime
                    //Actually I could in this case let AvalonDock to attach the contents
                    //from current layout using the content ids
                    //LayoutSerializationCallback should anyway be handled to attach contents
                    //not currently loaded
                    layoutSerializer.LayoutSerializationCallback += (s, e) =>
                    {
                        var contentId = e.Model.ContentId;
                        //if no content id is available or the content has already been found we dont bother loading it
                        if(string.IsNullOrEmpty(contentId) || e.Content != null)
                            return;

                        //restore the documents and sinks
                        var uri = new Uri(contentId);
                        if(uri.Scheme == "file")
                            e.Content = CShell.Shell.GetDoc(uri, true);
                        if (uri.Scheme == "sink")
                            e.Content = CShell.Shell.GetSink(uri, true); //do not open sink
                        //make sure that the document is part of the shells items
                        var doc = e.Content as IDocument;
                        if (doc != null)
                            EnsureItem(doc);

                        //restore the tools
                        if (uri.Scheme == "tool")
                            e.Content = CShell.Shell.GetTool(uri, true);
                        var tool = e.Content as ITool;
                        if(tool != null && !_tools.Contains(tool))
                            _tools.Add(tool);
                            
                    };
                    try
                    {
                        layoutSerializer.Deserialize(xmlReader);
                    }
                    catch (Exception ex)
                    {
                        //no big deal if the layout cannot be restored, there can be a few reasons, we just log the error and move on
                        // one reason is if there is no layout element in the xml
                        log.Error(ex);
                    }
                });
            }
        }//end method

        public CShell.Workspace.WindowLocation GetWindowLocation()
        {
            return shellView.GetWindowLocation();
        }

        public void RestoreWindowLocation(CShell.Workspace.WindowLocation windowLocation)
        {
            shellView.RestoreWindowLocation(windowLocation);
        }
        #endregion



       
    }//end class
}