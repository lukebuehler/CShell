// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CShell.Completion.Images;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CShell.Completion.DataItems
{
    class EntityCompletionData : CompletionData, IEntityCompletionData
    {
        readonly IEntity entity;
        static readonly CSharpAmbience csharpAmbience = new CSharpAmbience();

        public IEntity Entity
        {
            get { return entity; }
        }

        public EntityCompletionData(IEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            this.entity = entity;
            IAmbience ambience = new CSharpAmbience();
            ambience.ConversionFlags = entity is ITypeDefinition ? ConversionFlags.ShowTypeParameterList : ConversionFlags.None;
            DisplayText = entity.Name;
            this.CompletionText = ambience.ConvertEntity(entity);
            ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
            if (entity is ITypeDefinition)
            {
                // Show fully qualified Type name
                ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedTypeNames;
            }
            this.Image = CompletionImage.GetImage(entity);
        }

        #region Description & Documentation
        private string description;
        public override string Description
        {
            get
            {
                if (description == null)
                {
                    description = GetText(Entity);
                    if (HasOverloads)
                    {
                        description += " (+" + OverloadedData.Count() + " overloads)";
                    }
                    description += Environment.NewLine + XmlDocumentationToText(Entity.Documentation);
                }
                return description;
            }
            set
            {
                description = value;
            }
        }

        /// <summary>
        /// Converts a member to text.
        /// Returns the declaration of the member as C# or VB code, e.g.
        /// "public void MemberName(string parameter)"
        /// </summary>
        static string GetText(IEntity entity)
        {
            IAmbience ambience = csharpAmbience;
            ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
            if (entity is ITypeDefinition)
            {
                // Show fully qualified Type name
                ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedTypeNames;
            }
            if(entity is IMethod)
            {
                //if the method is an extension method we wanna see the whole method for the description
                //the original method (not reduced) can be obtained by calling ReducedFrom
                var reducedFromMethod = ((IMethod)entity).ReducedFrom;
                if(reducedFromMethod != null)
                    entity = reducedFromMethod;
            }
            return ambience.ConvertEntity(entity);
        }

        public static string XmlDocumentationToText(string xmlDoc)
        {
            //.Diagnostics.Debug.WriteLine(xmlDoc);
            StringBuilder b = new StringBuilder();
            try
            {
                using (XmlTextReader reader = new XmlTextReader(new StringReader("<root>" + xmlDoc + "</root>")))
                {
                    reader.XmlResolver = null;
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Text:
                                b.Append(reader.Value);
                                break;
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "filterpriority":
                                        reader.Skip();
                                        break;
                                    case "returns":
                                        b.AppendLine();
                                        b.Append("Returns: ");
                                        break;
                                    case "param":
                                        b.AppendLine();
                                        b.Append(reader.GetAttribute("name") + ": ");
                                        break;
                                    case "remarks":
                                        b.AppendLine();
                                        b.Append("Remarks: ");
                                        break;
                                    case "see":
                                        if (reader.IsEmptyElement)
                                        {
                                            b.Append(reader.GetAttribute("cref"));
                                        }
                                        else
                                        {
                                            reader.MoveToContent();
                                            if (reader.HasValue)
                                            {
                                                b.Append(reader.Value);
                                            }
                                            else
                                            {
                                                b.Append(reader.GetAttribute("cref"));
                                            }
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                }
                return b.ToString();
            }
            catch (XmlException)
            {
                return xmlDoc;
            }
        }

        #endregion
    } //end class EntityCompletionData
}
