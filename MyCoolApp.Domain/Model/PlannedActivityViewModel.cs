using System;

namespace MyCoolApp.Domain.Model
{
    public class PlannedActivityViewModel : BindableBase
    {
        public PlannedActivityViewModel()
        {
            
        }

        public PlannedActivityViewModel(DateTime plannedFor, string description)
        {
            _plannedFor = plannedFor;
            _description = description;
        }

        private DateTime _plannedFor;
        public DateTime PlannedFor
        {
            get { return _plannedFor; }
            set { SetProperty(ref _plannedFor, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
    }
}