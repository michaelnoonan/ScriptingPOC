using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Caliburn.Micro;
using MyCoolApp.Development;
using MyCoolApp.Events;
using MyCoolApp.Events.DevelopmentEnvironment;
using MyCoolApp.Events.Diagnostics;
using MyCoolApp.Events.Scripting;
using MyCoolApp.Scripting;

namespace MyCoolApp
{
    public partial class Shell :
        Form,
        IHandle<ProjectLoaded>,
        IHandle<ProjectClosed>,
        IHandle<RemoteControlStarted>,
        IHandle<RemoteControlShutDown>,
        IHandle<ScriptExecutionCompleted>,
        IHandle<LogInfoEvent>,
        IHandle<LogErrorEvent>
        
    {
        private const string DefaultApplicationTitle = "Host Application";

        public Shell()
        {
            InitializeComponent();
            Text = DefaultApplicationTitle;
            EvaluateCommands();
            Program.GlobalEventAggregator.Subscribe(this);
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyDocuments;
            var result = fbd.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                ProjectManager.Instance.CreateNewProject(fbd.SelectedPath);
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

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectManager.Instance.SaveProject();
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectManager.Instance.CloseProject();
        }

        public new void Handle(ProjectLoaded message)
        {
            plannedActivitiesBindingSource.DataSource = message.LoadedProject.PlannedActivities;
            Text = string.Format("{0} - {1}", message.LoadedProject.Name,
                                 message.LoadedProject.ProjectFilePath);
            StatusLabel.Text = string.Format("Project opened: {0}", message.LoadedProject.ProjectFilePath);
            Invoke(new Action(EvaluateCommands));
        }

        public new void Handle(ProjectClosed message)
        {
            plannedActivitiesBindingSource.Clear();
            Text = DefaultApplicationTitle;
            StatusLabel.Text = string.Format("Project closed: {0}", message.ClosedProject.ProjectFilePath);
            Invoke(new Action(EvaluateCommands));
        }

        public new void Handle(RemoteControlStarted message)
        {
            Invoke(new Action(() =>
                                  {
                                      StatusLabel.Text = string.Format("Development environment remote control on {0}", message.ListenUri);
                                      statusConnectedToIDE.Visible = true;
                                      statusNotConnectedToIDE.Visible = false;
                                      EvaluateCommands();
                                  }));
        }

        public new void Handle(RemoteControlShutDown message)
        {
            if (IsDisposed) return;
            Invoke(new Action(() =>
                                  {
                                      StatusLabel.Text = string.Format("Development environment shut down.");
                                      statusConnectedToIDE.Visible = false;
                                      statusNotConnectedToIDE.Visible = true;
                                      EvaluateCommands();
                                  }));
        }

        private void ExecuteScript(string scriptText)
        {
            if (ProjectManager.Instance.IsProjectLoaded == false)
                return;

            StatusLabel.Text = "Executing script...";
            var executor = new ScriptExecutor(ProjectManager.Instance.Project);
            executor.ExecuteScriptAsync(scriptText);
        }

        public new void Handle(ScriptExecutionCompleted message)
        {
            if (IsDisposed) return;

            Invoke(new Action(
                       () =>
                       {
                           StatusLabel.Text = "Execution complete in " + message.Result.ElapsedTime.ToString();
                       }));
        }

        private void EvaluateCommands()
        {
            closeProjectToolStripMenuItem.Enabled = ProjectManager.Instance.IsProjectLoaded;
            recalculateToolStripMenuItem.Enabled = ProjectManager.Instance.IsProjectLoaded;
            scriptingOpenProjectToolStripMenuItem.Enabled = ProjectManager.Instance.HasScriptingSolution;
            runScriptToolStripMenuItem.Enabled = ProjectManager.Instance.HasScriptingSolution;
            debugScriptToolStripMenuItem.Enabled = ProjectManager.Instance.HasScriptingSolution;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F12)
            {
                ToggleOutputWindow();
                return false;
            }
            
            //if (keyData == Keys.F5)
            //{
            //    ExecuteScript(ScriptTextBox.Text);
            //    return false;
            //}

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void toggleOutputWindowButton_Click(object sender, EventArgs e)
        {
            ToggleOutputWindow();
        }

        private void ToggleOutputWindow()
        {
            toggleOutputWindowButton.Checked = !toggleOutputWindowButton.Checked;
            outputWindow.Visible = toggleOutputWindowButton.Checked;
        }

        public void Handle(LogInfoEvent message)
        {
            outputWindow.AppendText(message.Message + Environment.NewLine);
        }

        public void Handle(LogErrorEvent message)
        {
            var before = outputWindow.TextLength;
            outputWindow.AppendText(message.Message + Environment.NewLine);
            outputWindow.Select(before, outputWindow.TextLength);
            outputWindow.SelectionColor = Color.Red;
            outputWindow.Select(outputWindow.TextLength, 0);
            outputWindow.ScrollToCaret();
        }
    }
}
