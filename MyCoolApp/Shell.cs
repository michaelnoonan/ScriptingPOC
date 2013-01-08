using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using MyCoolApp.DevelopmentEnvironment;

namespace MyCoolApp
{
    public partial class Shell : Form
    {
        public Shell()
        {
            InitializeComponent();
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
            ProjectManager.Instance.LoadScriptingProject();
        }

        private void startSharpDevelopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DevelopmentEnvironmentHub.Instance.StartDevelopmentEnvironment();
        }
    }
}
