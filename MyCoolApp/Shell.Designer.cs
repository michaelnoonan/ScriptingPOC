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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Shell));
            this.MenuBar = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recalculateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptingOpenProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolBar = new System.Windows.Forms.ToolStrip();
            this.toggleOutputWindowButton = new System.Windows.Forms.ToolStripButton();
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.statusNotConnectedToIDE = new System.Windows.Forms.ToolStripDropDownButton();
            this.statusConnectedToIDE = new System.Windows.Forms.ToolStripSplitButton();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.plannedActivitiesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.plannedActivitiesGridView = new System.Windows.Forms.DataGridView();
            this.outputWindow = new System.Windows.Forms.RichTextBox();
            this.plannedForDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isDirtyDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MenuBar.SuspendLayout();
            this.ToolBar.SuspendLayout();
            this.StatusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plannedActivitiesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plannedActivitiesGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // MenuBar
            // 
            this.MenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.scheduleToolStripMenuItem,
            this.scriptingToolStripMenuItem});
            this.MenuBar.Location = new System.Drawing.Point(0, 0);
            this.MenuBar.Name = "MenuBar";
            this.MenuBar.Size = new System.Drawing.Size(863, 24);
            this.MenuBar.TabIndex = 0;
            this.MenuBar.Text = "MenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.closeProjectToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Image = global::MyCoolApp.Properties.Resources.folder_add;
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.newProjectToolStripMenuItem.Text = "New Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.NewProjectToolStripMenuItemClick);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Image = global::MyCoolApp.Properties.Resources.folder;
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.openProjectToolStripMenuItem.Text = "Open Project";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.OpenProjectToolStripMenuItem1Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Image = global::MyCoolApp.Properties.Resources.disk;
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.saveProjectToolStripMenuItem.Text = "Save Project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.SaveProjectToolStripMenuItemClick);
            // 
            // closeProjectToolStripMenuItem
            // 
            this.closeProjectToolStripMenuItem.Image = global::MyCoolApp.Properties.Resources.cross;
            this.closeProjectToolStripMenuItem.Name = "closeProjectToolStripMenuItem";
            this.closeProjectToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.closeProjectToolStripMenuItem.Text = "Close Project";
            this.closeProjectToolStripMenuItem.Click += new System.EventHandler(this.CloseProjectToolStripMenuItemClick);
            // 
            // scheduleToolStripMenuItem
            // 
            this.scheduleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recalculateToolStripMenuItem});
            this.scheduleToolStripMenuItem.Name = "scheduleToolStripMenuItem";
            this.scheduleToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.scheduleToolStripMenuItem.Text = "Schedule";
            // 
            // recalculateToolStripMenuItem
            // 
            this.recalculateToolStripMenuItem.Image = global::MyCoolApp.Properties.Resources.arrow_refresh;
            this.recalculateToolStripMenuItem.Name = "recalculateToolStripMenuItem";
            this.recalculateToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.recalculateToolStripMenuItem.Text = "Recalculate Now!";
            // 
            // scriptingToolStripMenuItem
            // 
            this.scriptingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scriptingOpenProjectToolStripMenuItem,
            this.runScriptToolStripMenuItem,
            this.debugScriptToolStripMenuItem});
            this.scriptingToolStripMenuItem.Name = "scriptingToolStripMenuItem";
            this.scriptingToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.scriptingToolStripMenuItem.Text = "Scripting";
            // 
            // scriptingOpenProjectToolStripMenuItem
            // 
            this.scriptingOpenProjectToolStripMenuItem.Image = global::MyCoolApp.Properties.Resources.script;
            this.scriptingOpenProjectToolStripMenuItem.Name = "scriptingOpenProjectToolStripMenuItem";
            this.scriptingOpenProjectToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.scriptingOpenProjectToolStripMenuItem.Text = "Open Scripting Project";
            this.scriptingOpenProjectToolStripMenuItem.Click += new System.EventHandler(this.OpenProjectToolStripMenuItemClick);
            // 
            // runScriptToolStripMenuItem
            // 
            this.runScriptToolStripMenuItem.Image = global::MyCoolApp.Properties.Resources.script_go;
            this.runScriptToolStripMenuItem.Name = "runScriptToolStripMenuItem";
            this.runScriptToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.runScriptToolStripMenuItem.Text = "Run Script";
            // 
            // debugScriptToolStripMenuItem
            // 
            this.debugScriptToolStripMenuItem.Image = global::MyCoolApp.Properties.Resources.script_error;
            this.debugScriptToolStripMenuItem.Name = "debugScriptToolStripMenuItem";
            this.debugScriptToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.debugScriptToolStripMenuItem.Text = "Debug Script";
            // 
            // ToolBar
            // 
            this.ToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleOutputWindowButton});
            this.ToolBar.Location = new System.Drawing.Point(0, 24);
            this.ToolBar.Name = "ToolBar";
            this.ToolBar.Size = new System.Drawing.Size(863, 25);
            this.ToolBar.TabIndex = 1;
            this.ToolBar.Text = "toolStrip1";
            // 
            // toggleOutputWindowButton
            // 
            this.toggleOutputWindowButton.CheckOnClick = true;
            this.toggleOutputWindowButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleOutputWindowButton.Image = ((System.Drawing.Image)(resources.GetObject("toggleOutputWindowButton.Image")));
            this.toggleOutputWindowButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleOutputWindowButton.Name = "toggleOutputWindowButton";
            this.toggleOutputWindowButton.Size = new System.Drawing.Size(23, 22);
            this.toggleOutputWindowButton.Text = "Show/Hide output window";
            this.toggleOutputWindowButton.ToolTipText = "Show/Hide output window (F12)";
            this.toggleOutputWindowButton.Click += new System.EventHandler(this.ToggleOutputWindowButtonClick);
            // 
            // StatusBar
            // 
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusNotConnectedToIDE,
            this.statusConnectedToIDE,
            this.StatusLabel,
            this.toolStripProgressBar1});
            this.StatusBar.Location = new System.Drawing.Point(0, 611);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(863, 22);
            this.StatusBar.TabIndex = 2;
            this.StatusBar.Text = "statusStrip1";
            // 
            // statusNotConnectedToIDE
            // 
            this.statusNotConnectedToIDE.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.statusNotConnectedToIDE.Image = global::MyCoolApp.Properties.Resources.disconnect;
            this.statusNotConnectedToIDE.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.statusNotConnectedToIDE.Name = "statusNotConnectedToIDE";
            this.statusNotConnectedToIDE.Size = new System.Drawing.Size(149, 20);
            this.statusNotConnectedToIDE.Text = "Not connected to IDE";
            // 
            // statusConnectedToIDE
            // 
            this.statusConnectedToIDE.Image = global::MyCoolApp.Properties.Resources.connect;
            this.statusConnectedToIDE.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.statusConnectedToIDE.Name = "statusConnectedToIDE";
            this.statusConnectedToIDE.Size = new System.Drawing.Size(131, 20);
            this.statusConnectedToIDE.Text = "Connected to IDE";
            this.statusConnectedToIDE.Visible = false;
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(48, 17);
            this.StatusLabel.Text = "Ready...";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // plannedActivitiesBindingSource
            // 
            this.plannedActivitiesBindingSource.AllowNew = true;
            this.plannedActivitiesBindingSource.DataSource = typeof(MyCoolApp.Model.PlannedActivityViewModel);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 49);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.plannedActivitiesGridView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.outputWindow);
            this.splitContainer1.Size = new System.Drawing.Size(863, 562);
            this.splitContainer1.SplitterDistance = 436;
            this.splitContainer1.TabIndex = 9;
            // 
            // plannedActivitiesGridView
            // 
            this.plannedActivitiesGridView.AutoGenerateColumns = false;
            this.plannedActivitiesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.plannedActivitiesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.plannedForDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.isDirtyDataGridViewCheckBoxColumn});
            this.plannedActivitiesGridView.DataSource = this.plannedActivitiesBindingSource;
            this.plannedActivitiesGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plannedActivitiesGridView.Location = new System.Drawing.Point(0, 0);
            this.plannedActivitiesGridView.Name = "plannedActivitiesGridView";
            this.plannedActivitiesGridView.Size = new System.Drawing.Size(863, 436);
            this.plannedActivitiesGridView.TabIndex = 8;
            // 
            // outputWindow
            // 
            this.outputWindow.BackColor = System.Drawing.Color.Black;
            this.outputWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputWindow.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputWindow.ForeColor = System.Drawing.Color.Lime;
            this.outputWindow.Location = new System.Drawing.Point(0, 0);
            this.outputWindow.Name = "outputWindow";
            this.outputWindow.ReadOnly = true;
            this.outputWindow.Size = new System.Drawing.Size(863, 122);
            this.outputWindow.TabIndex = 7;
            this.outputWindow.Text = "";
            this.outputWindow.WordWrap = false;
            // 
            // plannedForDataGridViewTextBoxColumn
            // 
            this.plannedForDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.plannedForDataGridViewTextBoxColumn.DataPropertyName = "PlannedFor";
            this.plannedForDataGridViewTextBoxColumn.HeaderText = "Planned For";
            this.plannedForDataGridViewTextBoxColumn.Name = "plannedForDataGridViewTextBoxColumn";
            this.plannedForDataGridViewTextBoxColumn.Width = 89;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            // 
            // isDirtyDataGridViewCheckBoxColumn
            // 
            this.isDirtyDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.isDirtyDataGridViewCheckBoxColumn.DataPropertyName = "IsDirty";
            this.isDirtyDataGridViewCheckBoxColumn.HeaderText = "Is Dirty";
            this.isDirtyDataGridViewCheckBoxColumn.Name = "isDirtyDataGridViewCheckBoxColumn";
            this.isDirtyDataGridViewCheckBoxColumn.ReadOnly = true;
            this.isDirtyDataGridViewCheckBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.isDirtyDataGridViewCheckBoxColumn.Width = 45;
            // 
            // Shell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 633);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.ToolBar);
            this.Controls.Add(this.MenuBar);
            this.Controls.Add(this.StatusBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.MenuBar;
            this.Name = "Shell";
            this.Text = "My Cool Planner";
            this.MenuBar.ResumeLayout(false);
            this.MenuBar.PerformLayout();
            this.ToolBar.ResumeLayout(false);
            this.ToolBar.PerformLayout();
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plannedActivitiesBindingSource)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.plannedActivitiesGridView)).EndInit();
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
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ToolStripButton toggleOutputWindowButton;
        private System.Windows.Forms.ToolStripMenuItem scheduleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recalculateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripDropDownButton statusNotConnectedToIDE;
        private System.Windows.Forms.ToolStripSplitButton statusConnectedToIDE;
        private System.Windows.Forms.BindingSource plannedActivitiesBindingSource;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView plannedActivitiesGridView;
        private System.Windows.Forms.RichTextBox outputWindow;
        private System.Windows.Forms.DataGridViewTextBoxColumn plannedForDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isDirtyDataGridViewCheckBoxColumn;

    }
}

