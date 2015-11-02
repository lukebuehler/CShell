using System;

namespace CShell.Framework.Services
{
	public interface ISinkProvider
	{
		bool Handles(Uri uri);
        ISink Create(Uri uri);
	}
}