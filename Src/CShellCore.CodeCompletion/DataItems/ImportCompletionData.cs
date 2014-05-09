// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;

namespace CShellCore.CodeCompletion.DataItems
{
    /// <summary>
    /// Completion item that introduces a using declaration.
    /// </summary>
    class ImportCompletionData : EntityCompletionData
    {
        string insertUsing;
        string insertionText;

        public ImportCompletionData(ITypeDefinition typeDef, CSharpTypeResolveContext contextAtCaret, bool useFullName)
            : base(typeDef)
        {
            this.Description = "using " + typeDef.Namespace + ";";
            if (useFullName)
            {
                var astBuilder = new TypeSystemAstBuilder(new CSharpResolver(contextAtCaret));
                insertionText = astBuilder.ConvertType(typeDef).GetText();
            }
            else
            {
                insertionText = typeDef.Name;
                insertUsing = typeDef.Namespace;
            }
        }
    } //end class ImportCompletionData
}
