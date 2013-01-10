namespace MyCoolApp
{
    partial class Shell
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Shell));
            this.MenuBar = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startSharpDevelopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptingOpenProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolBar = new System.Windows.Forms.ToolStrip();
            this.toggleConsoleButton = new System.Windows.Forms.ToolStripButton();
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ExecuteButton = new System.Windows.Forms.Button();
            this.RecordedActionsListView = new System.Windows.Forms.CheckedListBox();
            this.ScriptTextBox = new System.Windows.Forms.RichTextBox();
            this.OutputTextBox = new System.Windows.Forms.RichTextBox();
            this.MenuBar.SuspendLayout();
            this.ToolBar.SuspendLayout();
            this.StatusBar.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuBar
            // 
            this.MenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.scriptingToolStripMenuItem});
            this.MenuBar.Location = new System.Drawing.Point(0, 0);
            this.MenuBar.Name = "MenuBar";
            this.MenuBar.Size = new System.Drawing.Size(618, 24);
            this.MenuBar.TabIndex = 0;
            this.MenuBar.Text = "MenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.closeProjectToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.newProjectToolStripMenuItem.Text = "New Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.openProjectToolStripMenuItem.Text = "Open Project";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem1_Click);
            // 
            // closeProjectToolStripMenuItem
            // 
            this.closeProjectToolStripMenuItem.Name = "closeProjectToolStripMenuItem";
            this.closeProjectToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.closeProjectToolStripMenuItem.Text = "Close Project";
            this.closeProjectToolStripMenuItem.Click += new System.EventHandler(this.closeProjectToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // scriptingToolStripMenuItem
            // 
            this.scriptingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startSharpDevelopToolStripMenuItem,
            this.scriptingOpenProjectToolStripMenuItem,
            this.runScriptToolStripMenuItem});
            this.scriptingToolStripMenuItem.Name = "scriptingToolStripMenuItem";
            this.scriptingToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.scriptingToolStripMenuItem.Text = "Scripting";
            // 
            // startSharpDevelopToolStripMenuItem
            // 
            this.startSharpDevelopToolStripMenuItem.Name = "startSharpDevelopToolStripMenuItem";
            this.startSharpDevelopToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.startSharpDevelopToolStripMenuItem.Text = "Start SharpDevelop";
            this.startSharpDevelopToolStripMenuItem.Click += new System.EventHandler(this.startSharpDevelopToolStripMenuItem_Click);
            // 
            // scriptingOpenProjectToolStripMenuItem
            // 
            this.scriptingOpenProjectToolStripMenuItem.Name = "scriptingOpenProjectToolStripMenuItem";
            this.scriptingOpenProjectToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.scriptingOpenProjectToolStripMenuItem.Text = "Open Project";
            this.scriptingOpenProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // runScriptToolStripMenuItem
            // 
            this.runScriptToolStripMenuItem.Name = "runScriptToolStripMenuItem";
            this.runScriptToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.runScriptToolStripMenuItem.Text = "Run Script";
            this.runScriptToolStripMenuItem.Click += new System.EventHandler(this.runScriptToolStripMenuItem_Click);
            // 
            // ToolBar
            // 
            this.ToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleConsoleButton});
            this.ToolBar.Location = new System.Drawing.Point(0, 24);
            this.ToolBar.Name = "ToolBar";
            this.ToolBar.Size = new System.Drawing.Size(618, 25);
            this.ToolBar.TabIndex = 1;
            this.ToolBar.Text = "toolStrip1";
            // 
            // toggleConsoleButton
            // 
            this.toggleConsoleButton.CheckOnClick = true;
            this.toggleConsoleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleConsoleButton.Image = ((System.Drawing.Image)(resources.GetObject("toggleConsoleButton.Image")));
            this.toggleConsoleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleConsoleButton.Name = "toggleConsoleButton";
            this.toggleConsoleButton.Size = new System.Drawing.Size(23, 22);
            this.toggleConsoleButton.Text = "Show/Hide scripting console";
            this.toggleConsoleButton.ToolTipText = "Show/Hide scripting console (F12)";
            this.toggleConsoleButton.CheckedChanged += new System.EventHandler(this.toggleConsoleButton_CheckedChanged);
            // 
            // StatusBar
            // 
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.StatusBar.Location = new System.Drawing.Point(0, 410);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(618, 22);
            this.StatusBar.TabIndex = 2;
            this.StatusBar.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(48, 17);
            this.StatusLabel.Text = "Ready...";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.ExecuteButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 285);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(618, 29);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // ExecuteButton
            // 
            this.ExecuteButton.Location = new System.Drawing.Point(3, 3);
            this.ExecuteButton.Name = "ExecuteButton";
            this.ExecuteButton.Size = new System.Drawing.Size(109, 23);
            this.ExecuteButton.TabIndex = 0;
            this.ExecuteButton.Text = "Execute (F5)";
            this.ExecuteButton.UseVisualStyleBackColor = true;
            this.ExecuteButton.Click += new System.EventHandler(this.ExecuteButton_Click);
            // 
            // RecordedActionsListView
            // 
            this.RecordedActionsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecordedActionsListView.FormattingEnabled = true;
            this.RecordedActionsListView.Location = new System.Drawing.Point(0, 49);
            this.RecordedActionsListView.Name = "RecordedActionsListView";
            this.RecordedActionsListView.Size = new System.Drawing.Size(618, 140);
            this.RecordedActionsListView.TabIndex = 5;
            // 
            // ScriptTextBox
            // 
            this.ScriptTextBox.AcceptsTab = true;
            this.ScriptTextBox.BackColor = System.Drawing.Color.Black;
            this.ScriptTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ScriptTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScriptTextBox.ForeColor = System.Drawing.Color.Lime;
            this.ScriptTextBox.Location = new System.Drawing.Point(0, 189);
            this.ScriptTextBox.Name = "ScriptTextBox";
            this.ScriptTextBox.Size = new System.Drawing.Size(618, 96);
            this.ScriptTextBox.TabIndex = 6;
            this.ScriptTextBox.Text = "RecordedActions.First(x => x.Description.Contains(\"second\")).Description";
            // 
            // OutputTextBox
            // 
            this.OutputTextBox.AcceptsTab = true;
            this.OutputTextBox.BackColor = System.Drawing.Color.Black;
            this.OutputTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.OutputTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputTextBox.ForeColor = System.Drawing.Color.Lime;
            this.OutputTextBox.Location = new System.Drawing.Point(0, 314);
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.Size = new System.Drawing.Size(618, 96);
            this.OutputTextBox.TabIndex = 7;
            this.OutputTextBox.Text = "";
            // 
            // Shell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 432);
            this.Controls.Add(this.RecordedActionsListView);
            this.Controls.Add(this.ScriptTextBox);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.ToolBar);
            this.Controls.Add(this.MenuBar);
            this.Controls.Add(this.OutputTextBox);
            this.Controls.Add(this.StatusBar);
            this.KeyPreview = true;
            this.MainMenuStrip = this.MenuBar;
            this.Name = "Shell";
            this.Text = "MyCoolApp";
            this.MenuBar.ResumeLayout(false);
            this.MenuBar.PerformLayout();
            this.ToolBar.ResumeLayout(false);
            this.ToolBar.PerformLayout();
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuBar;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptingOpenProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStrip ToolBar;
        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startSharpDevelopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button ExecuteButton;
        private System.Windows.Forms.CheckedListBox RecordedActionsListView;
        private System.Windows.Forms.RichTextBox ScriptTextBox;
        private System.Windows.Forms.ToolStripButton toggleConsoleButton;
        private System.Windows.Forms.RichTextBox OutputTextBox;

    }
}

