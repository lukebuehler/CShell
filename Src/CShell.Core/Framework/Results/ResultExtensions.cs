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
