namespace SharpDevelopRemoteControl.AddIn
{
    public interface ICommandListener
    {
        string ListenUri { get; }
        void Start();
        void Stop();
    }
}