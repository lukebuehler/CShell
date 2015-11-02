namespace CShell.Framework.Services
{
	public interface IStatusBar
	{
		string Message { get; }

        void UpdateMessage();
        void UpdateMessage(string message);

        int Progress { get; }
        bool ShowingProgress { get; }

	    void UpdateProgress(bool running);
	    void UpdateProgress(int progress);
	}
}