using System.Reflection;
using Caliburn.Micro;

namespace CShell.Framework.Services
{
	public static class ExtensionMethods
	{
		public static string GetExecutingAssemblyName()
		{
			return Assembly.GetExecutingAssembly().GetAssemblyName();
		}
	}
}