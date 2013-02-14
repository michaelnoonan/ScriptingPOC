using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Caliburn.Micro;
using MyCoolApp.Development;
using MyCoolApp.Diagnostics;
using MyCoolApp.Events;
using MyCoolApp.Events.DevelopmentEnvironment;
using MyCoolApp.Events.Diagnostics;
using MyCoolApp.Events.Scripting;
using MyCoolApp.Projects;
using MyCoolApp.Properties;
using MyCoolApp.Scripting;

namespace MyCoolApp
{
    public partial class Shell :
        Form,
        IHandle<ProjectLoaded>,
        IHandle<ProjectUnloaded>,
        IHandle<DevelopmentEnvironmentConnected>,
        IHandle<DevelopmentEnvironmentDisconnected>,
        IHandle<ScriptingAssemblyLoaded>,
        IHandle<LogInfoEvent>,
        IHandle<LogErrorEvent>
        
    {
        private const string DefaultApplicationTitle = "My Cool Planner";

        public IProjectManager ProjectManager { get; set; }
        public IScriptingService ScriptingService { get; set; }
        public ILogger Logger { get; set; }

        public Shell()
        {
            InitializeComponent();
            Program.GlobalEventAggregator.Subscribe(this);

            ProjectManager = Projects.ProjectManager.Instance;
            ScriptingService = Scripting.ScriptingService.Instance;
            Logger = Diagnostics.Logger.Instance;

            SetTitle(DefaultApplicationTitle);
            EvaluateCommands();
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

        private void SaveProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            ProjectManager.SaveProject();
        }

        private void CloseProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            ProjectManager.UnloadProject();
        }

        private void OpenScriptingProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            ScriptingService.LoadScriptingProject();
        }

        private void SetTitle(string title)
        {
            Text = string.Format("{0} - {1} ({2})",
                                 title, DefaultApplicationTitle, (Environment.Is64BitProcess ? "(x64)" : "(x86)"));
        }

        public new void Handle(ProjectLoaded message)
        {
            plannedActivitiesBindingSource.DataSource = message.LoadedProject.PlannedActivities;
            SetTitle(string.Format("{0} - {1}", message.LoadedProject.Name,
                                   message.LoadedProject.ProjectFilePath));
            StatusLabel.Text = string.Format("Project opened: {0}", message.LoadedProject.ProjectFilePath);
            Invoke(new Action(EvaluateCommands));
        }

        public new void Handle(ProjectUnloaded message)
        {
            plannedActivitiesBindingSource.Clear();
            SetTitle(DefaultApplicationTitle);
            StatusLabel.Text = string.Format("Project closed: {0}", message.UnloadedProject.ProjectFilePath);
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
                                      Logger.Info("Development environment disconnected.");
                                      statusConnectedToIDE.Visible = false;
                                      statusNotConnectedToIDE.Visible = true;
                                      EvaluateCommands();
                                  }));
        }

        public new void Handle(ScriptingAssemblyLoaded message)
        {
            if (IsDisposed) return;

            Invoke(new Action(() => LoadNewScriptingOptions(message)));
        }

        private void LoadNewScriptingOptions(ScriptingAssemblyLoaded message)
        {
            runScriptToolStripMenuItem.DropDownItems.Clear();
            var runScriptButtons =
                message.ScriptNames
                       .Select(x => new ToolStripMenuItem(x, Resources.script_go, ExecuteScript))
                       .ToArray();
            foreach (var button in runScriptButtons)
            {
                runScriptToolStripMenuItem.DropDownItems.Add(button);
            }
        }

        private async void ExecuteScript(object sender, EventArgs e)
        {
            var scriptName = ((ToolStripMenuItem) sender).Text;
            Logger.Info("Execute " + scriptName);
            var result = await ScriptingService.ExecuteScriptAsync(scriptName);
            if (result.Successful)
            {
                StatusLabel.Text = "Execution complete in " + result.ElapsedTime.ToString();
            }
            else
            {
                StatusLabel.Text = string.Format("Script execution failed after {0}: {1}",
                                                 result.ElapsedTime, result.FailureReason);
            }
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
            saveProjectToolStripMenuItem.Enabled = ProjectManager.IsProjectLoaded;
            recalculateToolStripMenuItem.Enabled = ProjectManager.IsProjectLoaded;
            scriptingOpenProjectToolStripMenuItem.Enabled = ProjectManager.IsProjectLoaded;
            runScriptToolStripMenuItem.Enabled = ProjectManager.HasScriptingProject;
            if (ProjectManager.HasScriptingProject == false) runScriptToolStripMenuItem.DropDownItems.Clear();
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
            //    ExecuteScriptAsync(ScriptTextBox.Text);
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
