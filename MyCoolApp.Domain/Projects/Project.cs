using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MyCoolApp.Domain.Model;

namespace MyCoolApp.Domain.Projects
{
    public class Project
    {
        public Project(string projectFilePath)
        {
            PlannedActivities = new ObservableCollection<PlannedActivityViewModel>();
            ProjectFilePath = Path.GetFullPath(projectFilePath);
            Name = Path.GetFileNameWithoutExtension(ProjectFilePath);
            ProjectFolder = Path.GetDirectoryName(ProjectFilePath);
        }

        public string Name { get; set; }
        public string ProjectFolder { get; private set; }
        public string ProjectFilePath { get; private set; }

        public string ScriptingFolder
        {
            get { return Path.Combine(ProjectFolder, "Scripting"); }
        }

        public string ScriptingProjectFilePath
        {
            get { return Path.Combine(ScriptingFolder, Name + ".vbproj"); }
        }

        public bool HasScriptingProject
        {
            get {
                return string.IsNullOrWhiteSpace(ScriptingProjectFilePath) == false &&
                       File.Exists(ScriptingProjectFilePath);
            }
        }

        public string ScriptingAssemblyFilePath
        {
            get { return Path.Combine(ScriptingFolder, @"bin\" + Name + ".dll"); }
        }

        public string ScriptingSymbolsFilePath
        {
            get { return Path.Combine(ScriptingFolder, @"bin\" + Name + ".pdb"); }
        }

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