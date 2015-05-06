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
using Mono.CSharp;
using CSharpParser = ICSharpCode.NRefactory.CSharp.CSharpParser;
using CShell.Util;

namespace CShell.Completion
{
    public class CSharpCompletion : ICompletion
    {
        private readonly bool isRepl;
        private IProjectContent projectContent;

        private CSharpCompletion(IProjectContent projectContent, bool isRepl)
        {
            this.projectContent = projectContent;
            this.isRepl = isRepl;
        }

        public CSharpCompletion(bool isRepl)
        {
            this.isRepl = isRepl;
            projectContent = new CSharpProjectContent();
            var assemblies = new Assembly[]
            {
                    typeof(object).Assembly, // mscorlib
//                    typeof(Uri).Assembly, // System.dll
//                    typeof(Enumerable).Assembly, // System.Core.dll
//					typeof(System.Xml.XmlDocument).Assembly, // System.Xml.dll
//					typeof(System.Drawing.Bitmap).Assembly, // System.Drawing.dll
//					typeof(Form).Assembly, // System.Windows.Forms.dll
//					typeof(ICSharpCode.NRefactory.TypeSystem.IProjectContent).Assembly,
                };

            Stopwatch total = Stopwatch.StartNew();
            AddReferences(assemblies);
            Debug.WriteLine("Init project content, loading base assemblies: " + total.Elapsed);
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

        public void AddReferences(params Assembly[] references)
        {
            if (references == null || references.Length == 0)
                return;
            AddReferences(references.Select(a=>a.Location).ToArray());
        }

        public void RemoveReferences(params Assembly[] references)
        {
            if (references == null || references.Length == 0)
                return;
            RemoveReferences(references.Select(a => a.Location).ToArray());
        }

        public void AddReferences(params string[] references)
        {
            if(references == null || references.Length == 0)
                return;

            var unresolvedAssemblies = GetUnresolvedAssemblies(references);
            projectContent = projectContent.AddAssemblyReferences((IEnumerable<IUnresolvedAssembly>)unresolvedAssemblies);
        }

        public void RemoveReferences(params string[] references)
        {
            if (references == null || references.Length == 0)
                return;

            var unresolvedAssemblies = GetUnresolvedAssemblies(references);
            projectContent = projectContent.RemoveAssemblyReferences((IEnumerable<IUnresolvedAssembly>)unresolvedAssemblies);
        }


        public ICompletion Clone(bool isRepl = false)
        {
            return new CSharpCompletion(projectContent, isRepl);
        }


        private static IUnresolvedAssembly[] GetUnresolvedAssemblies(string[] references)
        {
            IUnresolvedAssembly[] unresolvedAssemblies = null;
            if (references.Length == 1)
            {
                unresolvedAssemblies = new[] { GetUnresolvedAssembly(references[0]) };
            }
            else
            {
                unresolvedAssemblies = references
                    .AsParallel()
                    .AsOrdered()
                    .Select(GetUnresolvedAssembly)
                    .ToArray();
            }
            return unresolvedAssemblies;
        }

        private static IUnresolvedAssembly GetUnresolvedAssembly(string reference)
        {
            var fullPath = reference;
            //look in the bin folder
            if (!File.Exists(fullPath))
                fullPath = Path.Combine(Environment.CurrentDirectory, Constants.BinFolder, reference);
            if (!File.Exists(fullPath))
                fullPath = Path.Combine(Environment.CurrentDirectory, Constants.BinFolder, reference+".dll");
            if (!File.Exists(fullPath))
                fullPath = Path.Combine(Environment.CurrentDirectory, Constants.BinFolder, reference+".exe");
            //try to resolve as relaive path
            if (!File.Exists(fullPath))
                fullPath = PathHelper.ToAbsolutePath(Environment.CurrentDirectory, reference);
            //exe path
            if (!File.Exists(fullPath))
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                var exePath = Path.GetDirectoryName(path);
                if (exePath != null)
                {
                    fullPath = Path.Combine(exePath, reference + ".dll");
                    if(!File.Exists(fullPath))
                        fullPath = Path.Combine(exePath, reference + ".exe");
                }
            }
            //try to find in GAC
            if (!File.Exists(fullPath))
            {
                try
                {
                    var assemblyName = new AssemblyName(reference);
                    fullPath = GlobalAssemblyCache.FindAssemblyInNetGac(assemblyName);
                }
                catch { }
            }
            if (!File.Exists(fullPath))
            {
                var assemblyName = GlobalAssemblyCache.FindBestMatchingAssemblyName(reference);
                if (assemblyName != null)
                    fullPath = GlobalAssemblyCache.FindAssemblyInNetGac(assemblyName);
            }

            if (File.Exists(fullPath))
            {
                var loader = new CecilLoader();
                loader.DocumentationProvider = GetXmlDocumentation(fullPath);
                var unresolvedAssembly = loader.LoadAssemblyFile(fullPath);
                return unresolvedAssembly;
            }
            throw new FileNotFoundException("Reference could not be found: "+reference);
        }

        private static XmlDocumentationProvider GetXmlDocumentation(string dllPath)
        {
            if (string.IsNullOrEmpty(dllPath))
                return null;

            var xmlFileName = Path.GetFileNameWithoutExtension(dllPath) + ".xml";
            var localPath = Path.Combine(Path.GetDirectoryName(dllPath), xmlFileName);
            if (File.Exists(localPath))
                return new XmlDocumentationProvider(localPath);

            //if it's a .NET framework assembly it's in one of following folders
            //TODO: this path reference seems brittle
            var netPath = Path.Combine(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0", xmlFileName);
            if (File.Exists(netPath))
                return new XmlDocumentationProvider(netPath);

            return null;
        }
    }
}
