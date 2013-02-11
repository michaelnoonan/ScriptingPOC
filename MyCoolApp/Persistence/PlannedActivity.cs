using System;
using System.Runtime.Serialization;

namespace MyCoolApp.Persistence
{
    [DataContract]
    public class PlannedActivity
    {
        protected PlannedActivity()
        {
        }

        public PlannedActivity(DateTime plannedFor, string description)
        {
            PlannedFor = plannedFor;
            Description = description;
        }

        [DataMember]
        public DateTime PlannedFor { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}