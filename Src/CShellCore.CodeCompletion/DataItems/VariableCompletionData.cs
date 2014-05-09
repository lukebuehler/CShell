// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using CShellCore.CodeCompletion.Images;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CShellCore.CodeCompletion.DataItems
{
    internal class VariableCompletionData : CompletionData, IVariableCompletionData
    {
        public VariableCompletionData(IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException("variable");
            Variable = variable;

            IAmbience ambience = new CSharpAmbience();
            DisplayText = variable.Name;
            Description = ambience.ConvertVariable(variable);
            CompletionText = Variable.Name;
            this.Image = CompletionImage.Field.BaseImage;
        }

        public IVariable Variable { get; private set; }
    } //end class VariableCompletionData

}
