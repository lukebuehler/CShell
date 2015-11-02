using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Caliburn.Micro;

namespace CShell.Framework.Results
{
    /// <summary>
    /// Helps execution coroutines easier.
    /// </summary>
    public static class ResultExtensions
    {
        public static void BeginExecute(this IEnumerable<IResult> results, CoroutineExecutionContext executionContext = null)
        {
            Coroutine.BeginExecute(results.GetEnumerator(), executionContext);
        }

        public static void BeginExecute(this IResult result, CoroutineExecutionContext executionContext = null)
        {
            IEnumerable<IResult> results = new[] {result};
            results.BeginExecute(executionContext);
        }

        public static void ExecuteSynchronized(this IResult result, CoroutineExecutionContext executionContext = null)
        {
            IoC.BuildUp(result);
            var reset = new ManualResetEvent(false);
            result.Completed+= (s,e)=>reset.Set();
            result.Execute(executionContext);
            reset.WaitOne();
        }
    }
}
