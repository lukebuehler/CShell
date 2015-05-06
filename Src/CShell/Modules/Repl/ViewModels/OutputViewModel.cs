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
using System.Linq;
using System.ComponentModel.Composition;
using System.Windows.Media;
using CShell.Framework;
using CShell.Framework.Services;
using CShell.Modules.Repl.Views;
using Caliburn.Micro;
using Execute = CShell.Framework.Services.Execute;

namespace CShell.Modules.Repl.ViewModels
{
    [Export]
    [Export(typeof(IOutput))]
    [Export(typeof(ITool))]
    public class OutputViewModel : Tool, IOutput
	{
		private IOutputView _view;

        [ImportingConstructor]
        public OutputViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
            BufferLength = 200;
            Font = "Consolas";
            FontSize = 12;
            BackgroundColor = Color.FromArgb(255, 40, 40, 40);
            TextColor = Color.FromArgb(255, 242, 242, 242);
            DisplayName = "Output";
        }

		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Bottom; }
		}

        public override Uri Uri
        {
            get { return new Uri("tool://cshell/output"); }
        }

        public override Uri IconSource
        {
            get { return new Uri("pack://application:,,,/CShell;component/Resources/Icons/Output.png"); }
        }

		private string _text = string.Empty;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
                if (_text != null)
                {
                    var lines = _text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    //only remove old lines every 10 updates 
                    if (lines.Length > BufferLength && (bufferLength <= 10 || lines.Length % 10 == 0))
                        _text = String.Join(Environment.NewLine, lines.Skip(lines.Length - bufferLength));
                }
			    NotifyOfPropertyChange(() => Text);

				if (_view != null)
					Execute.OnUIThread(() => _view.ScrollToEnd());
			}
		}

	    private int bufferLength;
	    public int BufferLength
	    {
	        get { return bufferLength; }
	        set
	        {
                if (value <= 0)
                    throw new ArgumentException("BufferLength has to be 1 or greater");
                bufferLength = value;
	        }
	    }

	    public void Clear()
		{
			Text = string.Empty;
		}

        public void Write(string text)
        {
            Text += text;
        }

        public void WriteLine()
        {
            Text += Environment.NewLine;
        }

        public void WriteLine(string text)
        {
            Text += text + Environment.NewLine;
        }

        public void Write(string format, params object[] arg)
        {
            Text += String.Format(format, arg);
        }

        public void WriteLine(string format, params object[] arg)
        {
            Text += String.Format(format, arg) + Environment.NewLine;
        }

		protected override void OnViewLoaded(object view)
		{
			_view = (IOutputView) view;
			_view.ScrollToEnd();
		}

        #region Appearance from IOutput
        private string font;
        public string Font
        {
            get { return font; }
            set { font = value; NotifyOfPropertyChange(() => Font); }
        }

        private double fontSize;
        public double FontSize
        {
            get { return fontSize; }
            set { fontSize = value; NotifyOfPropertyChange(() => FontSize); }
        }

        private Color textColor;
        public Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                NotifyOfPropertyChange(() => TextColor);
                NotifyOfPropertyChange(() => TextBrush);
            }
        }
        public Brush TextBrush
        {
            get { return new SolidColorBrush(TextColor); }
        }

        private Color backgroundColor;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                NotifyOfPropertyChange(() => BackgroundColor);
                NotifyOfPropertyChange(() => BackgroundBrush);

            }
        }
        public Brush BackgroundBrush
        {
            get { return new SolidColorBrush(BackgroundColor); }
        }
        #endregion

        
    }
}