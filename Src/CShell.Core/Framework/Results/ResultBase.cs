using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace CShell.Framework.Results
{
    public abstract class ResultBase : IResult
    {
        public abstract void Execute(CoroutineExecutionContext context);

        public event EventHandler<ResultCompletionEventArgs> Completed;
        protected virtual void OnCompleted(Exception exception)
        {
            if (Completed != null)
                Completed(this, new ResultCompletionEventArgs { Error = exception });
        }

        protected virtual void OnCompleted(Exception exception, bool wasCancelled)
        {
            if (Completed != null)
                Completed(this, new ResultCompletionEventArgs { Error = exception, WasCancelled = wasCancelled });
        }
    }
}
