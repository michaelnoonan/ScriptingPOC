using System;
using System.IO;
using System.Runtime.Serialization;
using MyCoolApp.Model;
using MyCoolApp.Projects;

namespace MyCoolApp.Persistence
{
    public class ProjectLoader
    {
        public Project LoadProject(string projectFilePathToLoad)
        {
            if (File.Exists(projectFilePathToLoad) == false)
                throw new InvalidOperationException("The project file doesn't exist: " + projectFilePathToLoad);
            var project = new Project(projectFilePathToLoad);
            using (var sr = File.OpenRead(project.ProjectFilePath))
            {
                var dcs = new DataContractSerializer(typeof(ProjectData));
                var projectData = (ProjectData)dcs.ReadObject(sr);
                foreach (var a in projectData.PlannedActivities)
                {
                    project.PlannedActivities.Add(new PlannedActivityViewModel(a.PlannedFor, a.Description));
                }
            }
            return project;
        }
    }
}