namespace MyCoolApp.Model
{
    public class PlannedActivityViewModel : BindableBase
    {
        public PlannedActivityViewModel()
        {
            
        }

        public PlannedActivityViewModel(string description)
        {
            _description = description;
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
    }
}