namespace CShell.Hosting
{
    public class FileSystem : ScriptCs.FileSystem
    {
        public override string BinFolder
        {
            get { return Constants.BinFolder; }
        }

        public override string DllCacheFolder
        {
            get { return Constants.DllCacheFolder; }
        }

        public override string PackagesFile
        {
            get { return Constants.PackagesFile; }
        }

        public override string PackagesFolder
        {
            get { return Constants.PackagesFolder; }
        }

        public override string NugetFile
        {
            get { return Constants.NugetFile; }
        }
    }
}
