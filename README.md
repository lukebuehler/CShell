CShell Readme
=============

**Website and binaries:** http://cshell.net

CShell is an interactive C# scripting environment. It allows you to use C# without any fluff right in a console like environment called a read-eval-print-loop (REPL). Your code is directly evaluated and executed in a shell window, no separate executable has to be compiled and then run in a different process. More elaborate code can be written in a C# script and then evaluated as one file, only one line, or a selection. Then, your results can be easily dumped to HTML (like LINQPad) or viewed as a data grid or plotted to a chart.

CShell is perfect to explore data and quickly drill deeper into information since you can write one line of code, evaluate it, check out the results, and then write some more code, evaluate it again, and so forth.

If you just want to test one or two lines of C# code without creating a new console project in Visual Studio just fire up CShell, type the code you wanna test and you immediately can see if it works the way you want.

![Screenshot](https://raw.githubusercontent.com/lukebuehler/CShell/master/Doc/Screenshot1.png)

System Requirements
------------
CShell needs the .NET Framework 4.5 installed. Supported Windows versions: Windows 7, 8, 8.1 and Vista SP2. Windows XP is not supported.

How to Build
------------
To build Visual Studio 2013 or higher is required. Open the solution and build the project or configure the build script in the /Build folder to point to the right Visual Studio 'vcvarsall' script.

For the first build some NuGet packages have to be downloaded make sure you have NuGet installed and package restore enabled (Tools>Options>Package Manager>Package Restore).

License
-------
CShell is released under the [Apache 2.0 license](http://www.apache.org/licenses/LICENSE-2.0).



