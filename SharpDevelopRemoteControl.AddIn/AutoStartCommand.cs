using ICSharpCode.Core;

namespace SharpDevelopRemoteControl.AddIn
{
    public class AutoStartCommand : AbstractCommand
    {
        public override void Run()
        {
            CommandListener.Instance.Start();
            HostApplicationAdapter.Instance.Start();
        }
    }
}