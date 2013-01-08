using ICSharpCode.Core;

namespace SharpDevelopRemoteControl
{
    public class AutoStartCommand : AbstractCommand
    {
        public override void Run()
        {
            RemoteControlServiceHost.Instance.Start();
        }
    }
}