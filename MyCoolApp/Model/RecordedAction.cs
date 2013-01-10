namespace MyCoolApp.Model
{
    public class RecordedAction
    {
        public RecordedAction(string description)
        {
            _description = description;
        }

        public bool IsDirty { get; private set; }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    IsDirty = true;
                }
            }
        }
    }
}