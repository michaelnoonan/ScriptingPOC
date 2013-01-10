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
            RecordedActions = new List<RecordedAction>();
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
            RecordedActions.AddRange(lines.Select(l => new RecordedAction(l)));
        }

        public bool IsLoaded { get; private set; }
        public string Name { get; set; }
        public string ProjectFolder { get; private set; }
        public string ProjectFilePath { get; private set; }
        public string ScriptingFolder { get; private set; }
        public string ScriptingProjectFilePath { get; private set; }
        public List<RecordedAction> RecordedActions { get; private set; }

        public void AddRecordedAction(string description)
        {
            RecordedActions.Add(new RecordedAction(description));
        }
    }
}