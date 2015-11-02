using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Media;
using ScriptCs.Contracts;

namespace CShell.Framework.Services
{
    public interface IReplOutput : IOutput
    {
        void Initialize(IReplScriptExecutor replExecutor);

        void EvaluateStarted(string input, string sourceFile);
        void EvaluateCompleted(ScriptResult result);

        bool IsEvaluating { get; }

        bool ShowConsoleOutput { get; set; }
        Color ResultColor { get; set; }
        Color WarningColor { get; set; }
        Color ErrorColor { get; set; }
        void ResetColor();

        //TODO: move to IReplScripExecutor?
        IEnumerable<string> SuppressedWarnings { get; }
        void SuppressWarning(string warningCode);
        void ShowWarning(string warningCode);
    }
}
