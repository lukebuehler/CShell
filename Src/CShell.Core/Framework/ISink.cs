using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CShell.Framework.Services;
using Caliburn.Micro;

namespace CShell.Framework
{
    public interface ISink : IDocument
    {
        PaneLocation PreferredLocation { get; }
        bool IsVisible { get; set; }

        void Dump(object o);
        void Clear();
    }
}
