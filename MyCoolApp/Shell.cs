using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using MyCoolApp.Development;
using MyCoolApp.Events;
using MyCoolApp.Events.DevelopmentEnvironment;
using MyCoolApp.Events.Diagnostics;
using MyCoolApp.Events.Scripting;
using MyCoolApp.Projects;
using MyCoolApp.Scripting;

namespace MyCoolApp
{
    public partial class Shell :
        Form,
        IHandle<ProjectLoaded>,
        IHandle<ProjectClosed>,
        IHandle<DevelopmentEnvironmentConnected>,
        IHandle<DevelopmentEnvironmentDisconnected>,
        IHandle<ScriptExecutionCompleted>,
        IHandle<LogInfoEvent>,
        IHandle<LogErrorEvent>
        
    {
        private const string DefaultApplicationTitle = "Host Application";

        public IProjectManager ProjectManager { get; set; }
        public SharpDevelopAdapter SharpDevelopAdapter { get; set; }
        public Logger Logger { get; set; }

        public Shell()
        {
            ProjectManager = Projects.ProjectManager.Instance;
            SharpDevelopAdapter = SharpDevelopAdapter.Instance;
            Logger = Logger.Instance;

            InitializeComponent();
            Text = DefaultApplicationTitle;
            EvaluateCommands();
            Program.GlobalEventAggregator.Subscribe(this);
        }

        private void NewProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog {RootFolder = Environment.SpecialFolder.MyDocuments};
            var result = fbd.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                ProjectManager.CreateNewProject(fbd.SelectedPath);
            }
        }

        private void OpenProjectToolStripMenuItem1Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.ShowDialog(this);
            if (ofd.FileName != null && File.Exists(ofd.FileName))
            {
                ProjectManager.LoadProject(ofd.FileName);
            }
        }

        private void OpenProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            SharpDevelopAdapter.LoadScriptingProject(ProjectManager.Project);
        }

        private void SaveProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            ProjectManager.SaveProject();
        }

        private void CloseProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            ProjectManager.CloseProject();
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

        public new void Handle(DevelopmentEnvironmentConnected message)
        {
            Invoke(new Action(() =>
                                  {
                                      Logger.Info("Development environment remote control on {0}", message.ListenUri);
                                      statusConnectedToIDE.Visible = true;
                                      statusNotConnectedToIDE.Visible = false;
                                      EvaluateCommands();
                                  }));
        }

        public new void Handle(DevelopmentEnvironmentDisconnected message)
        {
            if (IsDisposed) return;
            Invoke(new Action(() =>
                                  {
                                      Logger.Info("Development environment shut down.");
                                      statusConnectedToIDE.Visible = false;
                                      statusNotConnectedToIDE.Visible = true;
                                      EvaluateCommands();
                                  }));
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

        public new void Handle(LogInfoEvent message)
        {
            Invoke(new Action(
                       () =>
                           {
                               outputWindow.AppendText(message.Message + Environment.NewLine);
                               outputWindow.Select(outputWindow.TextLength, 0);
                               outputWindow.ScrollToCaret();
                           }));
        }

        public new void Handle(LogErrorEvent message)
        {
            Invoke(new Action(
                       () =>
                           {
                               var before = outputWindow.TextLength;
                               outputWindow.AppendText(message.Message + Environment.NewLine);
                               outputWindow.Select(before, outputWindow.TextLength);
                               outputWindow.SelectionColor = Color.Red;
                               outputWindow.Select(outputWindow.TextLength, 0);
                               outputWindow.ScrollToCaret();
                           }));
        }

        private void EvaluateCommands()
        {
            closeProjectToolStripMenuItem.Enabled = ProjectManager.IsProjectLoaded;
            recalculateToolStripMenuItem.Enabled = ProjectManager.IsProjectLoaded;
            scriptingOpenProjectToolStripMenuItem.Enabled = ProjectManager.IsProjectLoaded;
            runScriptToolStripMenuItem.Enabled = ProjectManager.HasScriptingSolution;
            debugScriptToolStripMenuItem.Enabled = ProjectManager.HasScriptingSolution;
            toggleOutputWindowButton.Checked = outputWindow.Visible;
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

        private void ToggleOutputWindowButtonClick(object sender, EventArgs e)
        {
            ToggleOutputWindow();
        }

        private void ToggleOutputWindow()
        {
            outputWindow.Visible = !outputWindow.Visible;
            toggleOutputWindowButton.Checked = outputWindow.Visible;
        }
    }
}
