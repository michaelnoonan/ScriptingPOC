﻿using MyCoolApp.Model;

namespace MyCoolApp.Events
{
    public class ProjectLoaded
    {
        public Project LoadedProject { get; private set; }

        public ProjectLoaded(Project loadedProject)
        {
            LoadedProject = loadedProject;
        }
    }
}