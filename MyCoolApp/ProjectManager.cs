using System;
using System.IO;

namespace MyCoolApp
{
    public class ProjectManager
    {
        public static readonly ProjectManager Instance = new ProjectManager();

        public string ProjectFolder { get { return IsProjectLoaded ? Path.GetDirectoryName(ProjectFileFullPath) : null; } }
        public string ProjectName { get { return IsProjectLoaded ? Path.GetFileNameWithoutExtension(ProjectFileFullPath) : null; } }
        public string ProjectFileFullPath { get; private set; }

        public string ProjectScriptSolutionFolder { get { return IsProjectLoaded ? Path.Combine(ProjectFolder, "Scripting") : null; } }
        public string ProjectScriptingSolutionFilePath { get { return IsProjectLoaded ? Path.Combine(ProjectScriptSolutionFolder, ProjectName + ".sln") : null; } }

        public bool IsProjectLoaded { get { return ProjectFileFullPath != null; } }
        
        public bool HasScriptingSolution
        {
            get
            {
                return string.IsNullOrWhiteSpace(ProjectScriptingSolutionFilePath) == false &&
                       File.Exists(ProjectScriptingSolutionFilePath);
            }
        }

        public void LoadProject(string projectFilePath)
        {
            if (string.IsNullOrWhiteSpace(projectFilePath) == false &&
                File.Exists(projectFilePath))
            {
                // Simulate loading a project
                ProjectFileFullPath = projectFilePath;
                OnProjectLoaded(projectFilePath);
            }
        }

        public event EventHandler<ProjectLoadedEventArgs> ProjectLoaded;

        protected virtual void OnProjectLoaded(string projectFileFullPath)
        {
            var handler = ProjectLoaded;
            if (handler != null) handler(this, new ProjectLoadedEventArgs(projectFileFullPath));
        }
    }

    public class ProjectLoadedEventArgs : EventArgs
    {
        public string ProjectFileFullPath { get; private set; }

        public ProjectLoadedEventArgs(string projectFileFullPath)
        {
            ProjectFileFullPath = projectFileFullPath;
        }
    }
}