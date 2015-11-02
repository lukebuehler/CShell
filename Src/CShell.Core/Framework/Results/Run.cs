using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CShell.Framework.Results
{
    public static class Run
    {
        public static RunCodeResult Code(string code, string sourceFile)
        {
            return new RunCodeResult(code, sourceFile);
        }

        public static RunCodeResult Code(string code)
        {
            return new RunCodeResult(code);
        }

        public static RunCodeResult File(string file)
        {
            var fileText =System.IO.File.ReadAllText(file);
            return new RunCodeResult(fileText, file);
        }
    }
}
