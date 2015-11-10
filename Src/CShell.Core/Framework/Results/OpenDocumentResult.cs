using System;
using System.ComponentModel.Composition;
using System.Linq;
using CShell.Framework.Services;
using Caliburn.Micro;

namespace CShell.Framework.Results
{
	public class OpenDocumentResult : OpenResultBase<IDocument>
	{
		private IDocument document;
        private readonly Type documentType;
		private readonly Uri uri;

		[Import]
		private IShell shell;

		public OpenDocumentResult(IDocument document)
		{
            this.document = document;
		}

		public OpenDocumentResult(string path)
		{
		    if (path == null) throw new ArgumentNullException("path");
		    uri = new Uri(System.IO.Path.GetFullPath(path));
		}

	    public OpenDocumentResult(Uri uri)
	    {
	        this.uri = uri;
	    }

        public OpenDocumentResult(Type documentType)
		{
            this.documentType = documentType;
		}

	    public IDocument Document
	    {
	        get { return document; }
	    }

	    public override void Execute(CoroutineExecutionContext context)
		{
	        if (document == null)
	        {
	            document = (uri == null
	                ? (IDocument) IoC.GetInstance(documentType, null)
	                : Shell.GetDoc(uri));
	        }

	        if (document == null)
			{
				OnCompleted(null);
				return;
			}

			if (_setData != null)
                _setData(document);

			if (_onConfigure != null)
                _onConfigure(document);

            document.Deactivated += (s, e) =>
			{
				if (_onShutDown != null)
                    _onShutDown(document);
			};

            shell.OpenDocument(document);

            OnCompleted(null);
        }

       
	}
}