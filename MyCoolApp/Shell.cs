using System;
using System.IO;
using System.Windows.Forms;
using MyCoolApp.Development;

namespace MyCoolApp
{
    public partial class Shell : Form
    {
        public Shell()
        {
            // Add some event subscriptions to make sure we can keep in sync with system events
            // This would normally be done with EventAggregator/Broker
            RemoteControlManager.Instance.ConnectionStateChanged += HandleRemoteControlConnectionStateChange;

            InitializeComponent();

            EvilHorribleSyncMenuItems();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            RemoteControlManager.Instance.ConnectionStateChanged -= HandleRemoteControlConnectionStateChange;
        }

        private void HandleRemoteControlConnectionStateChange(object sender, EventArgs e)
        {
            Invoke(new Action(EvilHorribleSyncMenuItems));
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.FileName = "MyProject1.proj";
            var result = sfd.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                File.CreateText(sfd.FileName).Close();
                Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(sfd.FileName), "Scripting"));
                ProjectManager.Instance.LoadProject(sfd.FileName);
            }

            EvilHorribleSyncMenuItems();
        }

        private void openProjectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.ShowDialog(this);
            if (ofd.FileName != null && File.Exists(ofd.FileName))
            {
                ProjectManager.Instance.LoadProject(ofd.FileName);
            }

            EvilHorribleSyncMenuItems();
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectManager.Instance.LoadScriptingProject();

            EvilHorribleSyncMenuItems();
        }

        private void startSharpDevelopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoteControlManager.Instance.StartDevelopmentEnvironment();

            EvilHorribleSyncMenuItems();
        }

        private void EvilHorribleSyncMenuItems()
        {
            startSharpDevelopToolStripMenuItem.Enabled = RemoteControlManager.Instance.IsConnectionEstablished == false;
            openProjectToolStripMenuItem.Enabled = ProjectManager.Instance.HasScriptingSolution;
            runScriptToolStripMenuItem.Enabled = ProjectManager.Instance.HasScriptingSolution;
        }
    }
}
