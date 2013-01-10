using System;
using System.IO;
using MyCoolApp.Events;
using MyCoolApp.Model;

namespace MyCoolApp
{
    public class ProjectManager
    {
        public static readonly ProjectManager Instance = new ProjectManager();

        public Project Project { get; private set; }

        public bool IsProjectLoaded { get { return Project != null; } }
        
        public bool HasScriptingSolution
        {
            get
            {
                return IsProjectLoaded &&
                    string.IsNullOrWhiteSpace(Project.ScriptingProjectFilePath) == false &&
                       File.Exists(Project.ScriptingProjectFilePath);
            }
        }

        public void LoadProject(string projectFilePath)
        {
            if (IsProjectLoaded)
            {
                CloseProject();
            }

            Project = new Project();
            Project.LoadProjectFromFile(projectFilePath);
            Program.GlobalEventAggregator.Publish(new ProjectLoaded(Project));
        }

        public void CloseProject()
        {
            var projectBeingClosed = Project;
            Project = null;
            Program.GlobalEventAggregator.Publish(new ProjectClosed(projectBeingClosed));
        }
    }
}