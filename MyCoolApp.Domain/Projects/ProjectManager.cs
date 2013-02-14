using System;
using System.IO;
using Caliburn.Micro;
using MyCoolApp.Domain.Events;
using MyCoolApp.Domain.Events.Projects;
using MyCoolApp.Domain.Persistence;

namespace MyCoolApp.Domain.Projects
{
    public class ProjectManager : IProjectManager
    {
        public static readonly ProjectManager Instance =
            new ProjectManager(
                new ProjectLoader(),
                new ProjectSaver(),
                GlobalEventAggregator.Instance);
        
        private readonly ProjectLoader _projectLoader;
        private readonly ProjectSaver _projectSaver;
        private readonly IEventAggregator _globalEventAggregator;

        public ProjectManager(
            ProjectLoader projectLoader,
            ProjectSaver projectSaver,
            IEventAggregator globalEventAggregator)
        {
            _projectLoader = projectLoader;
            _projectSaver = projectSaver;
            _globalEventAggregator = globalEventAggregator;
        }

        public Project Project { get; private set; }
        public bool IsProjectLoaded { get { return Project != null; } }
        public bool HasScriptingProject { get { return IsProjectLoaded && Project.HasScriptingProject; } }

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
            _globalEventAggregator.Publish(new ProjectLoaded(Project));
        }

        public void LoadProject(string projectFilePath)
        {
            if (IsProjectLoaded)
            {
                UnloadProject();
            }

            Project = _projectLoader.LoadProject(projectFilePath);
            _globalEventAggregator.Publish(new ProjectLoaded(Project));
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
            _globalEventAggregator.Publish(new ProjectUnloaded(projectBeingUnloaded));
        }
    }
}