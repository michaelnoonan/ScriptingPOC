using System.IO;
using MyCoolApp.Domain.Model;

namespace MyCoolApp.Domain.Projects
{
    public class Project
    {
        public Project(string projectFilePath)
        {
            ProjectFilePath = Path.GetFullPath(projectFilePath);
            Name = Path.GetFileNameWithoutExtension(ProjectFilePath);
            ProjectFolder = Path.GetDirectoryName(ProjectFilePath);
            Schedule = new Schedule();
        }

        public Schedule Schedule { get; private set; }

        public string Name { get; set; }
        public string ProjectFolder { get; private set; }
        public string ProjectFilePath { get; private set; }

        public string ScriptingFolder
        {
            get { return Path.Combine(ProjectFolder, "Scripting"); }
        }

        public string ScriptingDependenciesFolder
        {
            get { return Path.Combine(ScriptingFolder, "Lib"); }
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
    }
}