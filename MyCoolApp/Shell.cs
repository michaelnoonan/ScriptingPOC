﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using MyCoolApp.Domain;
using MyCoolApp.Domain.Development;
using MyCoolApp.Domain.Diagnostics;
using MyCoolApp.Domain.Events;
using MyCoolApp.Domain.Events.DevelopmentEnvironment;
using MyCoolApp.Domain.Events.Diagnostics;
using MyCoolApp.Domain.Events.Projects;
using MyCoolApp.Domain.Events.Scripting;
using MyCoolApp.Domain.Projects;
using MyCoolApp.Domain.Scripting;
using MyCoolApp.Properties;
using Timer = System.Threading.Timer;

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

        public ISharpDevelopIntegrationService SharpDevelopIntegrationService { get; set; }
        public IProjectManager ProjectManager { get; set; }
        public IScriptingService ScriptingService { get; set; }
        public IEventAggregator GlobalEventAggregator { get; set; }
        public ILogger Logger { get; set; }

        public Shell()
        {
            Closing += ShellClosing;
            InitializeComponent();
            GlobalEventAggregator = Domain.GlobalEventAggregator.Instance;
            SharpDevelopIntegrationService = Domain.Development.SharpDevelopIntegrationService.Instance;
            ProjectManager = Domain.Projects.ProjectManager.Instance;
            ScriptingService = Domain.Scripting.ScriptingService.Instance;
            Logger = Domain.Diagnostics.Logger.Instance;

            GlobalEventAggregator.Subscribe(this);

            SetTitle(DefaultApplicationTitle);
            EvaluateCommands();
        }

        private async void ShellClosing(object sender, CancelEventArgs e)
        {
            GlobalEventAggregator.Unsubscribe(this);
            GlobalEventAggregator.Publish(new ApplicationShuttingDown());

            if (SharpDevelopIntegrationService.IsSharpDevelopRunning)
            {
                e.Cancel = true;
                var busy = new Busy();
                busy.SetMessage("Waiting for SharpDevelop to shut down...");
                busy.Show(this);
                var startedWaiting = DateTime.Now;
                var timeout = startedWaiting + TimeSpan.FromSeconds(20);
                while (SharpDevelopIntegrationService.IsSharpDevelopRunning && DateTime.Now < timeout)
                {
                    await Sleeper.SleepAsync(1000);
                    if (DateTime.Now - startedWaiting > TimeSpan.FromSeconds(5))
                    {
                        busy.SetMessage("Still waiting for SharpDevelop to shut down... maybe SharpDevelop has a question for you?");
                    }
                }
                busy.NotBusyAnymore();
                Close();
            }
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

        private async void OpenScriptingProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            await ScriptingService.LoadScriptingProjectAsync();
        }

        private void SetTitle(string title)
        {
            Text = string.Format("{0} - {1} ({2})",
                                 title, DefaultApplicationTitle, (Environment.Is64BitProcess ? "x64" : "x86"));
        }

        public new void Handle(ProjectLoaded message)
        {
            if (IsDisposed) return;

            plannedActivitiesBindingSource.DataSource = message.LoadedProject.Schedule.PlannedActivities;
            SetTitle(string.Format("{0} - {1}", message.LoadedProject.Name,
                                   message.LoadedProject.ProjectFilePath));
            StatusLabel.Text = string.Format("Project opened: {0}", message.LoadedProject.ProjectFilePath);
            Invoke(new Action(EvaluateCommands));
        }

        public new void Handle(ProjectUnloaded message)
        {
            if (IsDisposed) return;

            plannedActivitiesBindingSource.Clear();
            SetTitle(DefaultApplicationTitle);
            StatusLabel.Text = string.Format("Project closed: {0}", message.UnloadedProject.ProjectFilePath);
            Invoke(new Action(EvaluateCommands));
        }

        public new void Handle(DevelopmentEnvironmentConnected message)
        {
            if (IsDisposed) return;

            Invoke(new Action(() =>
                                  {
                                      if (IsDisposed) return;
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
                                      if (IsDisposed) return;
                                      Logger.Info("Development environment disconnected.");
                                      statusConnectedToIDE.Visible = false;
                                      statusNotConnectedToIDE.Visible = true;
                                      EvaluateCommands();
                                  }));
        }

        public new void Handle(ScriptingAssemblyLoaded message)
        {
            if (IsDisposed) return;

            Invoke(new Action(() =>
                                  {
                                      if (IsDisposed) return;
                                      LoadNewScriptingOptions(message);
                                  }));
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

            debugScriptToolStripMenuItem.DropDownItems.Clear();
            var debugScriptButtons =
                message.ScriptNames
                       .Select(x => new ToolStripMenuItem(x, Resources.script_go, DebugScript))
                       .ToArray();
            foreach (var button in debugScriptButtons)
            {
                debugScriptToolStripMenuItem.DropDownItems.Add(button);
            }
        }

        private async void ExecuteScript(object sender, EventArgs e)
        {
            var scriptName = ((ToolStripMenuItem) sender).Text;
            Logger.Info("Execute " + scriptName);
            var cts = new CancellationTokenSource();
            var busy = new Busy("Executing script " + scriptName, cts);
            busy.Show(this);
            var result = await ScriptingService.ExecuteScriptAsync(scriptName, cts.Token);
            busy.NotBusyAnymore();
            if (result.IsCancelled)
            {
                StatusLabel.Text = "Execution cancelled after " + result.ElapsedTime.ToString();
            }
            else if (result.IsSuccessful)
            {
                StatusLabel.Text = "Execution complete in " + result.ElapsedTime.ToString();
            }
            else
            {
                StatusLabel.Text = string.Format("Script execution failed after {0}: {1}",
                                                 result.ElapsedTime, result.FailureReason);
            }
        }

        private async void DebugScript(object sender, EventArgs e)
        {
            var scriptName = ((ToolStripMenuItem) sender).Text;
            Logger.Info("Debug " + scriptName);
            await ScriptingService.DebugScriptAsync(scriptName);
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
            if (IsDisposed) return;

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
            if (ProjectManager.HasScriptingProject == false)
            {
                runScriptToolStripMenuItem.DropDownItems.Clear();
                debugScriptToolStripMenuItem.DropDownItems.Clear();
            }
            toggleOutputWindowButton.Checked = outputWindow.Visible;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F12)
            {
                ToggleOutputWindow();
                return false;
            }
            
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
