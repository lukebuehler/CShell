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
