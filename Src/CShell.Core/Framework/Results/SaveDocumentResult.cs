using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace CShell.Framework.Results
{
    public class SaveDocumentResult : ResultBase
    {
        private IDocument document;
        private string newFile;

        public SaveDocumentResult(IDocument document)
        {
            if (document == null) throw new ArgumentNullException("document");
            this.document = document;
        }

        public SaveDocumentResult(IDocument document, string newFile)
            :this(document)
        {
            this.newFile = newFile;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            Exception ex = null;
            try
            {
                if (!String.IsNullOrEmpty(newFile))
                {
                    document.SaveAs(newFile);
                }
                else if (document.IsDirty)
                {
                    document.Save();
                }
            }
            catch(Exception e)
            {
                ex = e;
            }
            OnCompleted(ex);
        }
    }
}
