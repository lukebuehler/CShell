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

// This file is based on code from the SharpDevelop project:
//   Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \Doc\sharpdevelop-copyright.txt)
//   This code is distributed under the GNU LGPL (for details please see \Doc\COPYING.LESSER.txt)
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CShell.Completion.DataItems;
using CShell.Completion.Images;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;

namespace CShell.Completion
{
    sealed class CSharpCompletionDataFactory : ICompletionDataFactory, IParameterCompletionDataFactory
    {
        readonly CSharpTypeResolveContext contextAtCaret;
        private readonly CSharpCompletionContext context;

        public CSharpCompletionDataFactory(CSharpTypeResolveContext contextAtCaret, CSharpCompletionContext context)
        {
            Debug.Assert(contextAtCaret != null);
            this.contextAtCaret = contextAtCaret;
            this.context = context;
        }

        #region ICompletionDataFactory implementation
        ICompletionData ICompletionDataFactory.CreateEntityCompletionData(IEntity entity)
        {
            return new EntityCompletionData(entity);
        }

        ICompletionData ICompletionDataFactory.CreateEntityCompletionData(IEntity entity, string text)
        {
            return new EntityCompletionData(entity)
            {
                CompletionText = text,
                DisplayText = text
            };
        }

        ICompletionData ICompletionDataFactory.CreateTypeCompletionData(IType type, bool showFullName, bool isInAttributeContext, bool addForTypeCreation)
        {
            var typeDef = type.GetDefinition();
            if (typeDef != null)
                return new EntityCompletionData(typeDef);
            else
            {
                string name = showFullName ? type.FullName : type.Name;
                if (isInAttributeContext && name.EndsWith("Attribute") && name.Length > "Attribute".Length)
                {
                    name = name.Substring(0, name.Length - "Attribute".Length);
                }
                return new CompletionData(name);
            }
        }

        ICompletionData ICompletionDataFactory.CreateMemberCompletionData(IType type, IEntity member)
        {
            return new CompletionData(type.Name + "." + member.Name);
        }

        ICompletionData ICompletionDataFactory.CreateLiteralCompletionData(string title, string description, string insertText)
        {
            return new CompletionData(title)
            {
                Description = description,
                CompletionText = insertText ?? title,
                Image = CompletionImage.Literal.BaseImage,
                Priority = 2
            };
        }

        ICompletionData ICompletionDataFactory.CreateNamespaceCompletionData(INamespace name)
        {
            return new CompletionData(name.Name)
            {
                Image = CompletionImage.NamespaceImage,
            };
        }

        ICompletionData ICompletionDataFactory.CreateVariableCompletionData(IVariable variable)
        {
            return new VariableCompletionData(variable);
        }

        ICompletionData ICompletionDataFactory.CreateVariableCompletionData(ITypeParameter parameter)
        {
            return new CompletionData(parameter.Name);
        }

        ICompletionData ICompletionDataFactory.CreateEventCreationCompletionData(string varName, IType delegateType, IEvent evt, string parameterDefinition, IUnresolvedMember currentMember, IUnresolvedTypeDefinition currentType)
        {
            return new CompletionData("TODO: event creation");
        }

        ICompletionData ICompletionDataFactory.CreateNewOverrideCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IMember m)
        {
            return new OverrideCompletionData(declarationBegin, m, contextAtCaret);
        }

        ICompletionData ICompletionDataFactory.CreateNewPartialCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IUnresolvedMember m)
        {
            return new CompletionData("TODO: partial completion");
        }

        IEnumerable<ICompletionData> ICompletionDataFactory.CreateCodeTemplateCompletionData()
        {
            yield break;
        }

        IEnumerable<ICompletionData> ICompletionDataFactory.CreatePreProcessorDefinesCompletionData()
        {
            yield return new CompletionData("DEBUG");
            yield return new CompletionData("TEST");
        }

        ICompletionData ICompletionDataFactory.CreateImportCompletionData(IType type, bool useFullName, bool addForTypeCreation)
        {
            ITypeDefinition typeDef = type.GetDefinition();
            if (typeDef != null)
                return new ImportCompletionData(typeDef, contextAtCaret, useFullName);
            else
                throw new InvalidOperationException("Should never happen");
        }

        ICompletionData ICompletionDataFactory.CreateFormatItemCompletionData(string format, string description, object example)
        {
            throw new NotImplementedException();
        }

        ICompletionData ICompletionDataFactory.CreateXmlDocCompletionData(string tag, string description = null, string tagInsertionText = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IParameterCompletionDataFactory implementation
        IParameterDataProvider CreateMethodDataProvider(int startOffset, IEnumerable<IParameterizedMember> methods)
        {
            return new CSharpOverloadProvider(context, startOffset, from m in methods where m != null select new CSharpInsightItem(m));
        }

        IParameterDataProvider IParameterCompletionDataFactory.CreateConstructorProvider(int startOffset, IType type)
        {
            return CreateMethodDataProvider(startOffset, type.GetConstructors());
        }

        IParameterDataProvider IParameterCompletionDataFactory.CreateConstructorProvider(int startOffset, IType type, AstNode thisInitializer)
        {
            return CreateMethodDataProvider(startOffset, type.GetConstructors());
        }

        IParameterDataProvider IParameterCompletionDataFactory.CreateMethodDataProvider(int startOffset, IEnumerable<IMethod> methods)
        {
            return CreateMethodDataProvider(startOffset, methods);
        }

        IParameterDataProvider IParameterCompletionDataFactory.CreateDelegateDataProvider(int startOffset, IType type)
        {
            return CreateMethodDataProvider(startOffset, new[] { type.GetDelegateInvokeMethod() });
        }

        public IParameterDataProvider CreateIndexerParameterDataProvider(int startOffset, IType type, IEnumerable<IProperty> accessibleIndexers, AstNode resolvedNode)
        {
            throw new NotImplementedException();
            //return CreateMethodDataProvider(startOffset, accessibleIndexers);
        }

        IParameterDataProvider IParameterCompletionDataFactory.CreateTypeParameterDataProvider(int startOffset, IEnumerable<IType> types)
        {
            return null;
        }

        public IParameterDataProvider CreateTypeParameterDataProvider(int startOffset, IEnumerable<IMethod> methods)
        {
            return CreateMethodDataProvider(startOffset, methods);
        }
        #endregion

    }
}
