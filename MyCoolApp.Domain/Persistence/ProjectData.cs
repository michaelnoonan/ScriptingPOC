using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyCoolApp.Domain.Persistence
{
    [DataContract]
    public class ProjectData
    {
        public ProjectData()
        {
            PlannedActivities = new List<PlannedActivity>();
        }

        [DataMember]
        public List<PlannedActivity> PlannedActivities { get; set; }
    }
}