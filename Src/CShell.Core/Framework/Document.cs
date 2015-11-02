using System;
using System.Windows.Input;

namespace CShell.Framework
{
	public abstract class Document : LayoutItemBase, IDocument
	{
		private ICommand _closeCommand;
		public ICommand CloseCommand
		{
			get
			{
			    return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(), p => true));
                
                //documents can be closed all the time, if dirty, a dialog will be presented to ask if close is allowed
                //{
                //    bool canClose = false;
                //    CanClose(r => canClose = r);
                //    return canClose;
                //});
			}
		}

        public virtual Uri Uri { get; set; }
        public virtual bool IsDirty { get; set; }

        public virtual void Save()
        { }
        public virtual void SaveAs(string newFile)
        { }
    }
}