﻿using System.IO;
using System.Runtime.Serialization;
using MyCoolApp.Model;

namespace MyCoolApp.Persistence
{
    public class ProjectSaver
    {
        public void SaveProject(Project projectToSave)
        {
            var projectData = new ProjectData();

            foreach (var a in projectToSave.PlannedActivities)
            {
                projectData.PlannedActivities.Add(new PlannedActivity(a.PlannedFor, a.Description));
            }

            using (var s = File.OpenWrite(projectToSave.ProjectFilePath))
            {
                var dcs = new DataContractSerializer(typeof (ProjectData));
                dcs.WriteObject(s, projectData);
            }
        }
    }
}