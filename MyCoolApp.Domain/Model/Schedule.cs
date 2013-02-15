using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyCoolApp.Domain.Model
{
    public class Schedule
    {
        public Schedule()
        {
            PlannedActivities = new ObservableCollection<PlannedActivityViewModel>();
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