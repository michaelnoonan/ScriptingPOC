using System;
using System.IO;
using MyCoolApp.Events;
using MyCoolApp.Persistence;

namespace MyCoolApp.Projects
{
    public class ProjectManager : IProjectManager
    {
        public static readonly ProjectManager Instance = new ProjectManager(new ProjectLoader(), new ProjectSaver());
        
        private readonly ProjectLoader _projectLoader;
        private readonly ProjectSaver _projectSaver;

        public ProjectManager(ProjectLoader projectLoader, ProjectSaver projectSaver)
        {
            _projectLoader = projectLoader;
            _projectSaver = projectSaver;
        }

        public Project Project { get; private set; }
        public bool IsProjectLoaded { get { return Project != null; } }
        
        public bool HasScriptingProject
        {
            get
            {
                return IsProjectLoaded &&
                    string.IsNullOrWhiteSpace(Project.ScriptingProjectFilePath) == false &&
                       File.Exists(Project.ScriptingProjectFilePath);
            }
        }

        public void CreateNewProject(string projectPath)
        {
            if (IsProjectLoaded)
            {
                UnloadProject();
            }

            var projectFileName = Path.GetFileName(projectPath) + ".proj";
            var projectFilePath = Path.Combine(projectPath, projectFileName);

            if (Directory.Exists(projectPath) == false)
            {
                Directory.CreateDirectory(projectPath);
            }

            if (File.Exists(projectFilePath))
                throw new InvalidOperationException("A project already exists at " + projectFilePath);

            Project = new Project(projectFilePath);
            Project.AddPlannedActivity(DateTime.Now, "Create my first activity.");
            SaveProject();
            Program.GlobalEventAggregator.Publish(new ProjectLoaded(Project));
        }

        public void LoadProject(string projectFilePath)
        {
            if (IsProjectLoaded)
            {
                UnloadProject();
            }

            Project = _projectLoader.LoadProject(projectFilePath);
            Program.GlobalEventAggregator.Publish(new ProjectLoaded(Project));
        }

        public void SaveProject()
        {
            if (IsProjectLoaded == false)
                throw new InvalidOperationException("There is no project loaded to save!");

            _projectSaver.SaveProject(Project);
            Project.MarkAsClean();
        }

        public void UnloadProject()
        {
            var projectBeingUnloaded = Project;
            Project = null;
            Program.GlobalEventAggregator.Publish(new ProjectUnloaded(projectBeingUnloaded));
        }
    }
}