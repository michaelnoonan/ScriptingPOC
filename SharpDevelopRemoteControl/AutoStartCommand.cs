using ICSharpCode.Core;

namespace SharpDevelopRemoteControl
{
    public class AutoStartCommand : AbstractCommand
    {
        public override void Run()
        {
            CommandReceiver.Instance.Start();
            EventPublisher.Instance.Start();
        }
    }
}