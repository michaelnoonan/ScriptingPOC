using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace SharpDevelopRemoteControl.AddIn.Scripting
{
    public class RunCurrentScriptCommand : AbstractCommand
    {
        public override void Run()
        {
            // TODO: This should be done asynchronously to stop blocking the UI thread!
            var activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
            if (activeViewContent != null && activeViewContent.PrimaryFileName != null)
            {
                activeViewContent.PrimaryFile.SaveToDisk();
                WorkbenchSingleton.StatusBar.SetMessage("Executing script in Host Application...");
                var result = HostApplicationAdapter.Instance.ExecuteFileAsScript(activeViewContent.PrimaryFileName);
                LoggingService.Info(result);
                WorkbenchSingleton.StatusBar.SetMessage(result.ResultDescription + " in " + result.ElapsedTime);
            }
        }
    }
}