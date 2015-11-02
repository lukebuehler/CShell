using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace CShell.Framework.Results
{
    public class ShowDialogResult : ResultBase
    {
        private readonly Type dialogViewModelType;
        private object dialogViewModel;

        public IDictionary<string, object> Settings { get; set; } 

        public ShowDialogResult(object dialogViewModel)
        {
            this.dialogViewModel = dialogViewModel;
        }

        public ShowDialogResult(Type dialogViewModelType)
        {
            this.dialogViewModelType = dialogViewModelType;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            var windowManager = IoC.Get<IWindowManager>();
            if(dialogViewModel == null)
            {
                dialogViewModel = IoC.GetInstance(dialogViewModelType, null);
            }
            var result = windowManager.ShowDialog(dialogViewModel, settings:Settings);
            OnCompleted(null, result.HasValue && !result.Value);
        }
    }
}
