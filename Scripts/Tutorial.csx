// CShell Tutorial
// ===================
//
// CShell is an interactive C# environment, unlike most IDEs or other development setups which compile their 
// code into an executable and then run it, CShell uses a read-evaluate-print-loop (REPL) to execute all code which 
// perserves the state from one execution to the next.
// 
// This tutorial will guide you through the basic possibilities that this application offers. If you know what a REPL 
// loop is already and just want to go to the nitty gritty stuff scroll down to the "CShell Specific" section.

// ===================
// Getting Started
// ===================

// To evaluate an expression in the REPL shell enter following code in the window bellow an press enter: "7*7"
// As you can see the expression is evaluated and the result is printed.

// The console output is forwarded to the REPL as well. Try entering: "Console.WriteLine("Hello CShell");"

// When you press the up arrow you can iterate through the command history.

// Not all code has to be typed in the REPL window. Code in any file can be sent to the REPL and be evaluated.
// To do this just put the curor to the line you want to evaluate and press Alt+Enter. Try it here:
Math.Pow(2, 4);

// Also, more than one line can be executed at once. Select the the two line below and hit Alt+Enter.
var x = 7;
x + x;

// The state is preserved now just enter "x" in the REPL and hit enter: The variable "x" containing the value 7 is still there.

// Methods can be declared too, but only in classes. Select the class and send it to the REPL:
// (It's commented out so as not to mess up the code completion, execute it without the comments)
/*
static class MyMath
{
	public static long Fibonacci(long n)
    {
    	//anything more than 48 will result in a stack overflow.
    	if (n >= 48) throw new ArgumentException("Enter a number less than 48");
        if (n == 0) return 0;
        if (n == 1) return 1;
        return Fibonacci(n - 1) + Fibonacci(n - 2);
    }
}
*/
// Now the method can be called like so:
MyMath.Fibonacci(12);

// If you want only the method it can also be declared with a lamda:
Action<int> multiplyAndPrint = n => Console.WriteLine(n * 2);
//and then called like this:
multiplyAndPrint(7);

// The above fibonacci method would go something like this:
Func<int, int> fib = null;
fib = n => n > 1 ? fib(n - 1) + fib(n - 2) : n;
// Then call it
fib(12);

// By now you might already have seen it, but if an error occurs in the execution the result is red and compiler warnings are yellow.
throw new Exception("hahaha");


// ===================
// CShell Specific
// ===================
// At the root of the CShell environment is the .cshell file. If you double click it, you'll see that it contains C# code and not XML or 
// some other data definitions. It's structured according to a simple convention, it needs to implement a class called "CShellFile" and 
// the interface ICShellFile which containt two methods: "OnOpened" and "OnClosing". The whole workspace environment can be configured
// and saved in those two methods. That the file itself contains normal C# code is an important point to understand about CShell, all
// APIs like saving things, restoring the layouts and workspace files, modifying the UI are available to you at anytime, when opening a
// workspace or later via the REPL.

// Your workspace will probably not change very much as compared to, for example, VisualStudio solutions. It's recommened to configure your
// environment once via the .cshell file and use it for whatever work you wanna do. Of course if a different project requires you to load
// many different assemblies you might want to create a different workspace for that.
// When CShell is opened the last .cshell file will be loaded automatically unless you provide a different .cshell file in the arguments.

// All the APIs live in the "CShell" namespace. To try this just type "CShell" in the REPL and see what's available. One of the most 
// important static classes is called "Shell", through it many UI APIs can be accessed.
// for example to update the status when a long running operation is executed we can do the following:
for(int i=0;i<100;i++)
{
	System.Threading.Thread.Sleep(20);
	//this will update the status bar
	CShell.Shell.UpdateProgress(i);
}

// Note: The code is NOT executed in a seperate AppDomain that cannot contaminate the UI shell. The CShell UI, the loaded assemblies and all 
// evaluated code live in the same AppDomain and can call each other. Of course this could lead to quite some trouble but it creates tremendous
// flexibility, because remember, this is a scripting envrionment.


// Dumping to Sinks
// ------------------------------
// To explore the data and outputs generated in CShell it's helpful to dump the data to a sink.
// The simplest and default sink is the Xhtml sink. To dump to any sink use the "Shell.Dump" overloads.
Shell.Dump(new []{1,2,3,4});

// When dumping data you can select a sink by specifying a sink URI which are always in following format:
//   "sink://<Provider>/<SinkName>/<SinkTitle>"
// All CShell sinks are using the provider name "cshell" so they have follwing format:
//   "sink://cshell/<SinkName>/<SinkTitle>"

// There are a few sinks suppored out of the box:
//  * xhtml: Uses a LinqPad like XHtml dumper.
    new []{1,2,3,4}.Dump(new Uri("sink://cshell/xhtml/MyXhtmlSink"));
//  * grid: Displays the data in a data grid.
    new []{1,2,3,4}.Dump(new Uri("sink://cshell/grid/MyGridSink"));
//  * plot: Plots the data points of an IEnumerable.
	new []{1,2,3,4}.Dump(new Uri("sink://cshell/plot/MyPlotSink"));

// Since it's quite annoying having to type the whole URI everytime you wanna dump some data, there are usually shortcut methods for a sink.
// For example to plot there are many helper methods:
   new []{5,4,3,2,4,5}.Plot(); // -> same as .Dump(new Uri("sink://cshell/grid"))
   Plotting.PlotFunction(Math.Sin, "SinFunction"); // -> not possible by just dumping
   Plotting.GetPlotSink(null).Clear(); // gets the default sink, called "Plot" and clears it

// Some other shortcuts are:
   new []{1,2,3,4}.DumpXhtml(); // -> same as .Dump(new Uri("sink://cshell/xhtml"))
   new []{1,2,3,4}.DumpGrid(); // -> same as .Dump(new Uri("sink://cshell/grid"))


// Scripting the UI and Workspace
// ------------------------------
// There are many built in helper methods in CShell, here we'll explore some of them. Call them in a script, class or directly in the .cshell file
// to configure the UI and workspace to your liking.

// Evaluate a string as code
Shell.Evaluate("1+1");
// Evaluate the contents of a file. It finds the first occurence of the file in the whole workspace and executes it.
Shell.TryEvaluateFile("SomeFileInYourWorkspace.cs");
// Note: I like to create a file called "Usings.cs" where I add all my "using" declarations used in the .csx scripts. Then I evaluate it the .chell 
// file's "OnOpened" event after loading the workspace like so: Shell.TryEvaluateFile("Usings.cs");

// Modify the REPL, you can access the IRepl interface via "Shell.Repl"
Shell.Repl.Clear();


// ===================
// Under the Hood
// ===================
// To evaluate the code we use the Mono.CSharp compiler as a service called "Evaluator". It basically compiles your code in memory and executes it 
// right away.