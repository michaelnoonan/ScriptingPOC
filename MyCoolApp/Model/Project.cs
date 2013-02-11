using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyCoolApp.Model
{
    public class Project
    {
        public Project()
        {
            PlannedActivities = new List<PlannedActivityViewModel>();
        }

        public void LoadProjectFromFile(string projectFilePathToLoad)
        {
            if (File.Exists(projectFilePathToLoad) == false)
                throw new InvalidOperationException("The project file doesn't exist: " + projectFilePathToLoad);

            ProjectFilePath = Path.GetFullPath(projectFilePathToLoad);
            Name = Path.GetFileNameWithoutExtension(ProjectFilePath);
            ProjectFolder = Path.GetDirectoryName(ProjectFilePath);
            ScriptingFolder = Path.Combine(ProjectFolder, "Scripting");
            ScriptingProjectFilePath = Path.Combine(ScriptingFolder, Name + ".sln");
            LoadDataFromFile(ProjectFilePath);
            IsLoaded = true;
        }

        private void LoadDataFromFile(string projectFilePath)
        {
            var lines = File.ReadLines(projectFilePath);
            PlannedActivities.AddRange(lines.Select(l => new PlannedActivityViewModel(l)));
        }

        public bool IsLoaded { get; private set; }
        public string Name { get; set; }
        public string ProjectFolder { get; private set; }
        public string ProjectFilePath { get; private set; }
        public string ScriptingFolder { get; private set; }
        public string ScriptingProjectFilePath { get; private set; }
        public List<PlannedActivityViewModel> PlannedActivities { get; private set; }

        public void AddPlannedActivity(string description)
        {
            PlannedActivities.Add(new PlannedActivityViewModel(description));
        }
    }
}