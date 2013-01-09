using ICSharpCode.Core;

namespace SharpDevelopRemoteControl
{
    public class AutoStartCommand : AbstractCommand
    {
        public override void Run()
        {
            CommandListener.Instance.Start();
            EventPublisher.Instance.Start();
        }
    }
}