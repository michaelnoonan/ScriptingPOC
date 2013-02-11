using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace MyCoolApp.Model
{
    public class Project : IDirty
    {
        public Project(string projectFilePath)
        {
            PlannedActivities = new ObservableCollection<PlannedActivityViewModel>();
            ProjectFilePath = Path.GetFullPath(projectFilePath);
            Name = Path.GetFileNameWithoutExtension(ProjectFilePath);
            ProjectFolder = Path.GetDirectoryName(ProjectFilePath);
            ScriptingFolder = Path.Combine(ProjectFolder, "Scripting");
            ScriptingProjectFilePath = Path.Combine(ScriptingFolder, Name + ".sln");
        }

        public string Name { get; set; }
        public string ProjectFolder { get; private set; }
        public string ProjectFilePath { get; private set; }
        public string ScriptingFolder { get; private set; }
        public string ScriptingProjectFilePath { get; private set; }
        public ObservableCollection<PlannedActivityViewModel> PlannedActivities { get; private set; }
        public bool IsDirty { get { return PlannedActivities.Any(x => x.IsDirty); } }

        public void AddPlannedActivity(DateTime plannedFor, string description)
        {
            PlannedActivities.Add(new PlannedActivityViewModel(plannedFor, description));
        }

        public void MarkAsClean()
        {
            foreach (var a in PlannedActivities)
            {
                a.MarkAsClean();
            }
        }
    }
}