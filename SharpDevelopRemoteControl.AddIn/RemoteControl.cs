using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;
using SharpDevelopRemoteControl.AddIn.Scripting;
using SharpDevelopRemoteControl.Contracts;

namespace SharpDevelopRemoteControl.AddIn
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RemoteControl : IRemoteControl
    {
        public void LogMessage(string message)
        {
            LoggingService.InfoFormatted("Message Recieved: {0}", message);
        }

        public void LoadScriptingProject(string projectFilePath)
        {
            LoggingService.InfoFormatted("Load project command received: '{0}'", projectFilePath);
            LoggingService.InfoFormatted("Current project is '{0}'", ProjectService.CurrentProject != null ? ProjectService.CurrentProject.FileName : "{null}");

            IProject project;
            if (ProjectService.OpenSolution == null ||
                (ProjectService.CurrentProject != null && ProjectService.CurrentProject.FileName != projectFilePath))
            {
                ProjectService.LoadSolutionOrProject(projectFilePath);
                project = ProjectService.OpenSolution.Projects.Single(p => p.FileName == projectFilePath);
                ProjectService.CurrentProject = project;
            }
            else
            {
                project = ProjectService.CurrentProject;
                LoggingService.Info("The project is already loaded.");
            }

            EnsureScriptingDependenciesAreReferenced(project);

            BuildEngine.BuildInGui(
                project,
                new BuildOptions(BuildTarget.Rebuild,
                                 buildResults => HostApplicationAdapter.Instance.AnnounceScriptingProjectLoaded(
                                     new LoadScriptingProjectResult(projectFilePath, buildResults.Result == BuildResultCode.Success))));
        }

        private void EnsureScriptingDependenciesAreReferenced(IProject project)
        {
            var libFolder = Path.Combine(Path.GetDirectoryName(project.FileName), "Lib");

            EnsureReference(project, Path.Combine(libFolder, "MyCoolApp.Scripting.dll"));
            EnsureReference(project, Path.Combine(libFolder, "MyCoolApp.Domain.dll"));
            project.Save();
        }

        private void EnsureReference(IProject project, string assemblyFile)
        {
            LoggingService.InfoFormatted("Checking for {0}", assemblyFile);
            var matchingReferences = project.Items.OfType<ReferenceProjectItem>().Where(r => r.FileName == assemblyFile).ToArray();
            if (matchingReferences.Any() == false)
            {
                LoggingService.InfoFormatted("Adding reference to {0}", assemblyFile);
                var assemblyReference = new ReferenceProjectItem(project);
                assemblyReference.Include = Path.GetFileNameWithoutExtension(assemblyFile);
                assemblyReference.HintPath = FileUtility.GetRelativePath(project.Directory, assemblyFile);
                ProjectService.AddProjectItem(project, assemblyReference);
            }
            LoggingService.InfoFormatted("Already have reference to {0}", assemblyFile);
        }

        public void ShutDown()
        {
            LoggingService.Info("Shutdown command received...");
            if (BuildEngine.IsGuiBuildRunning)
            {
                LoggingService.Info("Time to kill!");
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                WorkbenchSingleton.MainWindow.Close();
            }
        }

        public void StartDebuggingScript(string className)
        {
            LoggingService.Info("Debug Script: " + className);
            var classToDebug = ParserService.AllProjectContents.Select(p => p.GetClass(className, 0)).FirstOrDefault(c => c != null);
            LoggingService.Info("Class to debug: " + (classToDebug != null ? classToDebug.FullyQualifiedName : "NOT FOUND!!!!"));
            var methodToDebug = classToDebug.Methods.FirstOrDefault(m => m.Name == "Main");
            NavigationService.NavigateTo(methodToDebug);
            EnsureBreakpointAtCurrentLocation();
            new DebugCurrentScriptCommand().Run();
        }

        private static void EnsureBreakpointAtCurrentLocation()
        {
            var activeView = (AvalonEditViewContent) WorkbenchSingleton.Workbench.ActiveViewContent;
            var filename = activeView.PrimaryFileName;
            var methodPosition = activeView.CodeEditor.ActiveTextEditorAdapter.Caret.Position;
            var matchingBreakpoint =
                BookmarkManager.GetBookmarks(filename)
                               .OfType<BreakpointBookmark>()
                               .FirstOrDefault(bp => bp.LineNumber == methodPosition.Line);
            if (matchingBreakpoint == null)
            {
                new ToggleBreakpointCommand().Run();
            }
        }
    }
}