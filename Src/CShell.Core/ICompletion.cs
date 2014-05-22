using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.NRefactory.Editor;

namespace CShell
{
    public class CodeCompletionResult
    {
        public List<ICompletionData> CompletionData = new List<ICompletionData>();
        public IOverloadProvider OverloadProvider;
        public ICompletionData SuggestedCompletionDataItem;
        public string TriggerWord;
        public int TriggerWordLength;
    }

    public interface ICompletion
    {
        void AddReferences(params Assembly[] references);
        void RemoveReferences(params Assembly[] references);
        void AddReferences(params string[] references);
        void RemoveReferences(params string[] references);

        CodeCompletionResult GetCompletions(IDocument document, int offset, bool controlSpace = false, bool saveDeclarations = false, string[] namespaces = null);
        ICompletion Clone();
    }
}