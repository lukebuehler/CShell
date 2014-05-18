#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using CShell.Util;

namespace CShell
{
    public sealed partial class Workspace : INotifyPropertyChanged
    {
        private readonly ILog log = LogManager.GetLog(typeof (Workspace));
        private readonly ScriptingEngine engine;
        private static readonly string startupDirectory = Environment.CurrentDirectory;

        internal Workspace(string cshellFile)
        {
            CShellFile = Path.GetFullPath(cshellFile);
            CShellFileName = Path.GetFileNameWithoutExtension(cshellFile);
            this.engine = new ScriptingEngine();

            //assemblies = new AssemblyReferences(engine);
            //files = new FileReferences("Files");
            RootFolder = Path.GetDirectoryName(cshellFile);
            RootFolder = Path.GetFullPath(RootFolder);

            //make sure we add the CShell assembly
            var cshellCoreAssembly = Assembly.GetExecutingAssembly();
            //assemblies.Add(new AssemblyReference(cshellCoreAssembly) { Removable = false });
            engine.Evaluate("using CShell; using CShell.Sinks;");
        }

        /// <summary>
        /// Gets CShell file name without extension.
        /// </summary>
        public string CShellFileName { get; private set; }
        public string CShellFile { get; private set; }

        public ScriptingEngine ScriptingEngine
        {
            get { return engine; }
        }

        //conatains refs to dlls
        //private readonly AssemblyReferences assemblies;
        //public AssemblyReferences Assemblies { get { return assemblies; } }

        private string rootFolder;

        public string RootFolder
        {
            get { return rootFolder; }
            set
            {
                if(!Directory.Exists(value))
                    throw new ArgumentException("The provided root folder in 'value' doesn't exist.");
                //rootFolder = PathHelper.ToRelativePath(Environment.CurrentDirectory, value);
                rootFolder = value;
                OnPropertyChanged("RootFolder");
            }
        }

        private string filter = Constants.WorkspaceFilter;
        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                OnPropertyChanged("Filter");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }


        internal Task Open()
        {
            //set the workspace direcotry as the working path
            Environment.CurrentDirectory = Path.GetDirectoryName(CShellFile) ?? startupDirectory;

            var cshellFileCode = File.ReadAllText(CShellFile);
            //compile the cshell file
            return engine.EvaluateAsync(cshellFileCode, CShellFile)
                .ContinueWith(t =>
                {
                    var result = t.Result;
                    //the errors will have been picked up by the REPL so we dont need to handle it here
                    if (!result.HasErrors)
                    {
                        var runCode = "";
                        runCode += "var cshellFile = new CShellFile();" + Environment.NewLine;
                        runCode += "try{cshellFile.OnOpened(CShell.Shell.Workspace);}";
                        runCode += "catch(Exception ex)";
                        runCode += "{throw new WorkspaceException(\"There was an error when calling the CShell file 'OnOpened' method. It is recommended to fix this issue and try opening the workspace again.\", ex);}";
                        return engine.EvaluateAsync(runCode, CShellFile).Result;
                    }
                    return null;
                });
        }

        internal Task Close()
        {
            var cshellFileCode = File.ReadAllText(CShellFile);
            //compile the cshell file
            return engine.EvaluateAsync(cshellFileCode, CShellFile)
                .ContinueWith(t =>
                {
                    var result = t.Result;
                    //the errors will have been picked up by the REPL so we dont need to handle it here
                    if (!result.HasErrors)
                    {
                        var runCode = "";
                        runCode += "var cshellFile = new CShellFile();" + Environment.NewLine;
                        runCode += "try{cshellFile.OnClosing(CShell.Shell.Workspace);}";
                        runCode += "catch(Exception ex)";
                        runCode += "{throw new WorkspaceException(\"There was an error when calling the CShell file 'OnClosing' method. It is recommended to fix this issue and try closing the workspace again.\", ex);}";
                        return engine.EvaluateAsync(runCode, CShellFile).Result;
                    }
                    return null;
                })
                .ContinueWith(t =>
                {
                    //release resources
                    //assemblies.Dispose();
                    //set the workspace direcotry back to the bin
                    Environment.CurrentDirectory = startupDirectory;
                });
        }

        #region Helpers
        internal static void CreateWorkspaceFile(string cshellFile)
        {
            //load the contents of the empty file
            var emptyFileText = "";
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            var templateFile = Path.Combine(exeDir, Constants.CShellTemplatesPath, "Empty.cshell");
            if (File.Exists(templateFile))
            {
                emptyFileText = System.IO.File.ReadAllText(templateFile);
            }
            else
            {
                emptyFileText += "public class CShellFile : ICShellFile" + Environment.NewLine;
                emptyFileText += "{" + Environment.NewLine;
                emptyFileText += "    public void OnOpened(Workspace workspace){}" + Environment.NewLine;
                emptyFileText += "    public void OnClosing(Workspace workspace){}" + Environment.NewLine;
                emptyFileText += "}";
            }
            File.WriteAllText(cshellFile, emptyFileText);
        }


        public static void CreateEmptyFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            var templateFile = Path.Combine(exeDir, Constants.CShellTemplatesPath, "Empty" + extension);
            var emptyFileText = "";
            if (File.Exists(templateFile))
                emptyFileText = File.ReadAllText(templateFile);
            File.WriteAllText(filePath, emptyFileText);
        }
        #endregion

    }//end class
}
