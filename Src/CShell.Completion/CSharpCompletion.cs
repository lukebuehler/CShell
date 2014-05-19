using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CShell.Completion.DataItems;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;

namespace CShell.Completion
{
    public class CSharpCompletion
    {
        private IProjectContent projectContent;
        
        public CSharpCompletion()
        {
            projectContent = new CSharpProjectContent();
            var assemblies = new List<Assembly>
            {
                    typeof(object).Assembly, // mscorlib
//                    typeof(Uri).Assembly, // System.dll
//                    typeof(Enumerable).Assembly, // System.Core.dll
//					typeof(System.Xml.XmlDocument).Assembly, // System.Xml.dll
//					typeof(System.Drawing.Bitmap).Assembly, // System.Drawing.dll
//					typeof(Form).Assembly, // System.Windows.Forms.dll
//					typeof(ICSharpCode.NRefactory.TypeSystem.IProjectContent).Assembly,
                };

            var unresolvedAssemblies = new IUnresolvedAssembly[assemblies.Count];
            Stopwatch total = Stopwatch.StartNew();
            Parallel.For(
                0, assemblies.Count,
                delegate(int i)
                {
                    var loader = new CecilLoader();
                    var path = assemblies[i].Location;
                    loader.DocumentationProvider = GetXmlDocumentation(assemblies[i].Location);
                    unresolvedAssemblies[i] = loader.LoadAssemblyFile(assemblies[i].Location);
                });
            Debug.WriteLine("Init project content, loading base assemblies: " + total.Elapsed);
            projectContent = projectContent.AddAssemblyReferences((IEnumerable<IUnresolvedAssembly>)unresolvedAssemblies);
        }

        private XmlDocumentationProvider GetXmlDocumentation(string dllPath)
        {
            if(string.IsNullOrEmpty(dllPath))
                return null;

            var xmlFileName = Path.GetFileNameWithoutExtension(dllPath) + ".xml";
            var localPath = Path.Combine(Path.GetDirectoryName(dllPath), xmlFileName);
            if(File.Exists(localPath))
                return new XmlDocumentationProvider(localPath);

            //if it's a .NET framework assembly it's in one of following folders
            var netPath = Path.Combine(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0", xmlFileName);
            if (File.Exists(netPath))
                return new XmlDocumentationProvider(netPath);

            return null;
        }

        public string[] GetAssemblies()
        {
            return projectContent.AssemblyReferences.OfType<IUnresolvedAssembly>().Select(aref => aref.Location).ToArray();
        }

        public void AddAssembly(string file)
        {
            if (String.IsNullOrEmpty(file))
                return;

            var loader = new CecilLoader();
            loader.DocumentationProvider = GetXmlDocumentation(file);
            var unresolvedAssembly = loader.LoadAssemblyFile(file);
            projectContent = projectContent.AddAssemblyReferences(unresolvedAssembly);
        }

        public void RemoveAssembly(string file)
        {
            if (String.IsNullOrEmpty(file))
                return;

            var loader = new CecilLoader();
            var unresolvedAssembly = loader.LoadAssemblyFile(file);
            projectContent = projectContent.RemoveAssemblyReferences(unresolvedAssembly);
        }

        public void ProcessInput(string input, string sourceFile)
        {
            if (string.IsNullOrEmpty(sourceFile))
                return;
            //see if it contains the word class, enum or struct
            //todo: this is buggy because if two classes are evaluated seperately, the original file will overwrite it
            // if the file is a script we should try to extract the class name and use it as the file name. sciptname + class
            // we can probably use the AST for that.
            if (input.Contains("class ") || input.Contains("enum ") || input.Contains("struct "))
            {
                var syntaxTree = new CSharpParser().Parse(input, sourceFile);
                syntaxTree.Freeze();
                var unresolvedFile = syntaxTree.ToTypeSystem();
                projectContent = projectContent.AddOrUpdateFiles(unresolvedFile);
            }
        }

        public CodeCompletionResult GetCompletions(IDocument document, int offset, bool controlSpace = false, string[] namespaces = null)
        {
            var result = new CodeCompletionResult();

            if (String.IsNullOrEmpty(document.FileName))
                return result;

            var completionContext = new CSharpCompletionContext(document, offset, projectContent, namespaces);

            var completionFactory = new CSharpCompletionDataFactory(completionContext.TypeResolveContextAtCaret, completionContext);
            var cce = new CSharpCompletionEngine(
                completionContext.Document,
                completionContext.CompletionContextProvider,
                completionFactory,
                completionContext.ProjectContent,
                completionContext.TypeResolveContextAtCaret
                );

            cce.EolMarker = Environment.NewLine;
            cce.FormattingPolicy = FormattingOptionsFactory.CreateSharpDevelop();


            var completionChar = completionContext.Document.GetCharAt(completionContext.Offset - 1);
            int startPos, triggerWordLength;
            IEnumerable<ICSharpCode.NRefactory.Completion.ICompletionData> completionData;
            if (controlSpace)
            {
                if (!cce.TryGetCompletionWord(completionContext.Offset, out startPos, out triggerWordLength))
                {
                    startPos = completionContext.Offset;
                    triggerWordLength = 0;
                }
                completionData = cce.GetCompletionData(startPos, true);
                //this outputs tons of available entities
                //if (triggerWordLength == 0)
                //    completionData = completionData.Concat(cce.GetImportCompletionData(startPos));
            }
            else
            {
                startPos = completionContext.Offset;

                if (char.IsLetterOrDigit(completionChar) || completionChar == '_')
                {
                    if (startPos > 1 && char.IsLetterOrDigit(completionContext.Document.GetCharAt(startPos - 2)))
                        return result;
                    completionData = cce.GetCompletionData(startPos, false);
                    startPos--;
                    triggerWordLength = 1;
                }
                else
                {
                    completionData = cce.GetCompletionData(startPos, false);
                    triggerWordLength = 0;
                }
            }

            result.TriggerWordLength = triggerWordLength;
            result.TriggerWord = completionContext.Document.GetText(completionContext.Offset - triggerWordLength, triggerWordLength);
            Debug.Print("Trigger word: '{0}'", result.TriggerWord);

            //cast to AvalonEdit completion data and add to results
            foreach (var completion in completionData)
            {
                var cshellCompletionData = completion as CompletionData;
                if (cshellCompletionData != null)
                {
                    cshellCompletionData.TriggerWord = result.TriggerWord;
                    cshellCompletionData.TriggerWordLength = result.TriggerWordLength;
                    result.CompletionData.Add(cshellCompletionData);
                }
            }

            //method completions
            if (!controlSpace)
            {
                // Method Insight
                var pce = new CSharpParameterCompletionEngine(
                    completionContext.Document,
                    completionContext.CompletionContextProvider,
                    completionFactory,
                    completionContext.ProjectContent,
                    completionContext.TypeResolveContextAtCaret
                );

                var parameterDataProvider = pce.GetParameterDataProvider(completionContext.Offset, completionChar);
                result.OverloadProvider = parameterDataProvider as IOverloadProvider;
            }

            return result;
        }
    }
}
