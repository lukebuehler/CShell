using System;
using System.Linq;
using Caliburn.Micro;

namespace CShell.Framework.Services
{
	public interface IDocumentProvider
	{
		bool Handles(Uri uri);
        IDocument Create(Uri uri);
	}
}