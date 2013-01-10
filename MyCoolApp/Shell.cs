using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using MyCoolApp.Development;
using MyCoolApp.Events;
using MyCoolApp.Events.DevelopmentEnvironment;
using MyCoolApp.Model;
using MyCoolApp.Scripting;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

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
            ToggleScriptingControls();
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
            SharpDevelopAdapter.Instance.LoadScriptingProject(ProjectManager.Instance.Project.ScriptingProjectFilePath);
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
            RefreshRecordedActionsList(message.LoadedProject.RecordedActions);
            Text = string.Format("{0} - {1}", message.LoadedProject.Name,
                                 message.LoadedProject.ProjectFilePath);
            StatusLabel.Text = string.Format("Project opened: {0}", message.LoadedProject.ProjectFilePath);
            Invoke(new Action(EvaluateCommands));
        }

        private void RefreshRecordedActionsList(IEnumerable<RecordedAction> recordedActions)
        {
            RecordedActionsListView.Items.Clear();
            RecordedActionsListView.DisplayMember = "Description";
            RecordedActionsListView.Items.AddRange(recordedActions.ToArray());
        }

        public new void Handle(ProjectClosed message)
        {
            RecordedActionsListView.Items.Clear();
            Text = DefaultApplicationTitle;
            StatusLabel.Text = string.Format("Project closed: {0}", message.ClosedProject.ProjectFilePath);
            Invoke(new Action(EvaluateCommands));
        }

        public new void Handle(RemoteControlStarted message)
        {
            Invoke(new Action(() =>
                                  {
                                      StatusLabel.Text = string.Format("Development environment remote control on {0}", message.ListenUri);
                                      EvaluateCommands();
                                  }));
        }

        public new void Handle(RemoteControlShutDown message)
        {
            if (IsDisposed) return;
            Invoke(new Action(() =>
                                  {
                                      StatusLabel.Text = string.Format("Development environment shut down.");
                                      EvaluateCommands();
                                  }));
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            ExecuteScript();
        }

        private void ExecuteScript()
        {
            if (ProjectManager.Instance.IsProjectLoaded == false)
                return;

            StatusLabel.Text = "Executing script...";
            var executor = new ScriptExecutor(ProjectManager.Instance.Project);
            var task = executor.ExecuteScriptAsync(ScriptTextBox.Text);
            task.ContinueWith(HandleResultOfExecution);
        }

        private void HandleResultOfExecution(Task<string> task)
        {
            if (IsDisposed) return;

            Invoke(new Action(
                       () =>
                           {
                               StatusLabel.Text = "Execution complete!";
                               if (task.Status == TaskStatus.Faulted)
                               {
                                   OutputTextBox.Text = string.Join(Environment.NewLine,
                                                                    task.Exception.InnerExceptions.Select(x => x.Message));
                               }
                               else
                               {
                                   OutputTextBox.Text = task.Result;
                               }
                               RefreshRecordedActionsList(ProjectManager.Instance.Project.RecordedActions);
                           }));
        }

        private void toggleConsoleButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleScriptingControls();
        }

        private void EvaluateCommands()
        {
            closeProjectToolStripMenuItem.Enabled = ProjectManager.Instance.IsProjectLoaded;
            scriptingOpenProjectToolStripMenuItem.Enabled = ProjectManager.Instance.HasScriptingSolution;
            startSharpDevelopToolStripMenuItem.Enabled = !SharpDevelopAdapter.Instance.IsConnectionEstablished;
            runScriptToolStripMenuItem.Enabled = ProjectManager.Instance.HasScriptingSolution;
            ExecuteButton.Enabled = ProjectManager.Instance.IsProjectLoaded;
            ScriptTextBox.Enabled = ProjectManager.Instance.IsProjectLoaded;
            OutputTextBox.Enabled = ProjectManager.Instance.IsProjectLoaded;
        }

        private void ToggleScriptingControls()
        {
            ScriptTextBox.Visible = toggleConsoleButton.Checked;
            ExecuteButton.Visible = toggleConsoleButton.Checked;
            OutputTextBox.Visible = toggleConsoleButton.Checked;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F12)
            {
                toggleConsoleButton.Checked = !toggleConsoleButton.Checked;
                return false;
            }
            
            if (keyData == Keys.F5)
            {
                ExecuteScript();
                return false;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
