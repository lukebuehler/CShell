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
using System.Diagnostics;
using System.IO;
using Mono.CSharp;

namespace CShell.Code
{
    public class ScriptingInteractiveBase// : InteractiveBase
    {
        public static TextWriter Output = Console.Out;
        public static TextWriter Error = Console.Error;
        //public static string Prompt = "csharp> ";
        //public static string ContinuationPrompt = "      > ";
        public static Evaluator Evaluator;
        public static bool ClearRequested;

        public static string help
        {
            get
            {
                var h = "";
                h += "Evaluator static methods:" + Environment.NewLine;
                h += "  Describe (object);       - Describes the object's type" + Environment.NewLine;
                h += "  ShowUsing ();            - Show active using declarations" + Environment.NewLine;
                h += "  ShowVars ();             - Shows defined local variables" + Environment.NewLine;
                h += "  Time (() => { });        - Times the specified code" + Environment.NewLine;
                h += "  print (obj);             - Shorthand for Console.WriteLine" + Environment.NewLine;
                h += "  clear;                   - Clear all the text in the REPL" + Environment.NewLine;
                h += "  help;                    - This help text";
                return h;
            }
        }

        public static object clear
        {
            get
            {
                ClearRequested = true;
                return null;
            }
        }

        public static void ShowVars()
        {
            Output.Write(Evaluator.GetVars());
        }

        public static void ShowUsing()
        {
            Output.Write(Evaluator.GetUsing());
        }

        public static TimeSpan Time(Action a)
        {
            Stopwatch sp = Stopwatch.StartNew();
            a();
            return sp.Elapsed;
        }

        public static void print(object obj)
        {
            Output.WriteLine(obj);
        }

        public static void print(string format, params object[] args)
        {
            Output.WriteLine(format, args);
        }

        public static string Describe(object x)
        {
            if (x == null)
                return "<null>";
            System.Type t = x as System.Type ?? x.GetType();
            StringWriter stringWriter = new StringWriter();
            new Outline(t, (TextWriter)stringWriter, true, false, false).OutlineType();
            return stringWriter.ToString();
        }
    }
}
