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
