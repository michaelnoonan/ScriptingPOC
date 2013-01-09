using System;
using System.IO;
using System.Windows.Forms;
using Caliburn.Micro;
using MyCoolApp.Development;
using MyCoolApp.Events;
using MyCoolApp.Events.DevelopmentEnvironment;

namespace MyCoolApp
{
    public partial class Shell :
        Form,
        IHandle<ProjectLoaded>,
        IHandle<ProjectClosed>,
        IHandle<RemoteControlStarted>,
        IHandle<RemoteControlShutDown>
        
    {
        private const string DefaultApplicationTitle = "Host Application";

        public Shell()
        {
            InitializeComponent();
            Text = DefaultApplicationTitle;
            EvaluateCommands();
            Program.GlobalEventAggregator.Subscribe(this);
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
        }

        private void openProjectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.ShowDialog(this);
            if (ofd.FileName != null && File.Exists(ofd.FileName))
            {
                ProjectManager.Instance.LoadProject(ofd.FileName);
            }
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharpDevelopAdapter.Instance.LoadScriptingProject(ProjectManager.Instance.ProjectScriptingSolutionFilePath);
        }

        private void startSharpDevelopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusLabel.Text = "Starting development environment...";
            SharpDevelopAdapter.Instance.StartDevelopmentEnvironment();
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectManager.Instance.IsProjectLoaded)
            {
                ProjectManager.Instance.CloseProject();
            }
        }

        public new void Handle(ProjectLoaded message)
        {
            Text = string.Format("{0} - {1}", ProjectManager.Instance.ProjectName,
                                 ProjectManager.Instance.ProjectFileFullPath);
            StatusLabel.Text = string.Format("Project opened: {0}", message.ProjectFileFullPath);
            Invoke(new Action(EvaluateCommands));
        }

        public new void Handle(ProjectClosed message)
        {
            Text = DefaultApplicationTitle;
            StatusLabel.Text = string.Format("Project closed: {0}", message.ProjectFilePath);
            Invoke(new Action(EvaluateCommands));
        }

        public new void Handle(RemoteControlStarted message)
        {
            StatusLabel.Text = string.Format("Development environment remote control on {0}", message.ListenUri);
            Invoke(new Action(EvaluateCommands));
        }

        public new void Handle(RemoteControlShutDown message)
        {
            if (IsDisposed) return;
            StatusLabel.Text = string.Format("Development environment shut down.");
            Invoke(new Action(EvaluateCommands));
        }

        private void EvaluateCommands()
        {
            closeProjectToolStripMenuItem.Enabled = ProjectManager.Instance.IsProjectLoaded;
            scriptingOpenProjectToolStripMenuItem.Enabled = ProjectManager.Instance.HasScriptingSolution;
            startSharpDevelopToolStripMenuItem.Enabled = !SharpDevelopAdapter.Instance.IsConnectionEstablished;
            runScriptToolStripMenuItem.Enabled = ProjectManager.Instance.HasScriptingSolution;
        }
    }
}
