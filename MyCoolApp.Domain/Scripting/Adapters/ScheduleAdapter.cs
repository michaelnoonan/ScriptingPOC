using System;
using System.Collections.Generic;
using MyCoolApp.Domain.Model;

namespace MyCoolApp.Domain.Scripting.Adapters
{
    public class ScheduleAdapter : ISchedule
    {
        private readonly Schedule _schedule;

        public ScheduleAdapter(Schedule schedule)
        {
            _schedule = schedule;
        }

        public IEnumerable<PlannedActivityViewModel> PlannedActivities { get { return _schedule.PlannedActivities; } }

        public void AddPlannedActivity(DateTime plannedFor, string description)
        {
            _schedule.AddPlannedActivity(plannedFor, description);
        }
    }
}