namespace CShell.Completion
{
    /// <summary>
    /// This interface allows to provide more information for scripts such as using statements, etc.
    /// </summary>
    public interface ICSharpScriptProvider
    {
        string GetUsing();
        string GetVars();
    }
}
