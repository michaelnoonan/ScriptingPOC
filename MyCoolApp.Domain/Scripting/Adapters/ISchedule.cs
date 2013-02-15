using System;
using System.Collections.Generic;
using MyCoolApp.Domain.Model;

namespace MyCoolApp.Domain.Scripting.Adapters
{
    public interface ISchedule
    {
        IEnumerable<PlannedActivityViewModel> PlannedActivities { get; }
        void AddPlannedActivity(DateTime plannedFor, string description);
    }
}