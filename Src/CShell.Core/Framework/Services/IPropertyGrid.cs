namespace CShell.Framework.Services
{
	public interface IPropertyGrid : ITool
	{
		object SelectedObject { get; set; }
	}
}