﻿using System;
using System.Collections.Generic;
using MyCoolApp.Model;
using MyCoolApp.Projects;

namespace MyCoolApp.Scripting
{
    /// <summary>
    /// This is the class that will be exposed by default to scripts run in the ScriptingEngine.
    /// </summary>
    public class ScriptingHostModel
    {
        private readonly Project _project;

        public ScriptingHostModel(Project project)
        {
            _project = project;
        }

        public IEnumerable<PlannedActivityViewModel> PlannedActivities { get { return _project.PlannedActivities; } }

        public void AddPlannedActivity(DateTime plannedFor, string description)
        {
            _project.PlannedActivities.Add(new PlannedActivityViewModel(plannedFor, description));
        }
    }
}