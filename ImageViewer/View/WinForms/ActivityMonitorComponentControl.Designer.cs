using ClearCanvas.Desktop.View.WinForms;
namespace ClearCanvas.ImageViewer.View.WinForms
{
	partial class ActivityMonitorComponentControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivityMonitorComponentControl));
			this._workItemsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._overviewPanel = new System.Windows.Forms.Panel();
			this._dashboardTable = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this._totalStudies = new System.Windows.Forms.Label();
			this._failures = new System.Windows.Forms.Label();
			this._reindexLink = new System.Windows.Forms.LinkLabel();
			this._openFileStoreLink = new System.Windows.Forms.LinkLabel();
			this._localServerConfigLink = new System.Windows.Forms.LinkLabel();
			this._diskSpacePanel = new System.Windows.Forms.Panel();
			this._diskSpaceMeter = new ClearCanvas.Desktop.View.WinForms.Meter();
			this._diskSpaceWarningIcon = new System.Windows.Forms.PictureBox();
			this._diskSpaceWarningMessage = new System.Windows.Forms.Label();
			this._diskSpace = new System.Windows.Forms.Label();
			this._logFileLink = new System.Windows.Forms.LinkLabel();
			this._studyRulesLink = new System.Windows.Forms.LinkLabel();
			this._statusLight = new ClearCanvas.Desktop.View.WinForms.IndicatorLight();
			this._serverDetailsTable = new System.Windows.Forms.TableLayoutPanel();
			this._hostName = new System.Windows.Forms.Label();
			this._aeTitle = new System.Windows.Forms.Label();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.panel1 = new System.Windows.Forms.Panel();
			this._workItemToolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this._activityFilter = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this._statusFilter = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
			this._textFilter = new System.Windows.Forms.ToolStripTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._overviewPanel.SuspendLayout();
			this._dashboardTable.SuspendLayout();
			this._diskSpacePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._diskSpaceWarningIcon)).BeginInit();
			this._serverDetailsTable.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panel1.SuspendLayout();
			this._workItemToolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _workItemsTableView
			// 
			resources.ApplyResources(this._workItemsTableView, "_workItemsTableView");
			this._workItemsTableView.Name = "_workItemsTableView";
			this._workItemsTableView.ReadOnly = false;
			this._workItemsTableView.ShowToolbar = false;
			// 
			// _overviewPanel
			// 
			this._overviewPanel.BackColor = System.Drawing.SystemColors.Control;
			this._overviewPanel.Controls.Add(this._dashboardTable);
			resources.ApplyResources(this._overviewPanel, "_overviewPanel");
			this._overviewPanel.Name = "_overviewPanel";
			// 
			// _dashboardTable
			// 
			resources.ApplyResources(this._dashboardTable, "_dashboardTable");
			this._dashboardTable.Controls.Add(this.label1, 4, 4);
			this._dashboardTable.Controls.Add(this.label2, 7, 2);
			this._dashboardTable.Controls.Add(this.label10, 1, 4);
			this._dashboardTable.Controls.Add(this.label11, 1, 5);
			this._dashboardTable.Controls.Add(this._totalStudies, 2, 4);
			this._dashboardTable.Controls.Add(this._failures, 2, 5);
			this._dashboardTable.Controls.Add(this._reindexLink, 7, 4);
			this._dashboardTable.Controls.Add(this._openFileStoreLink, 7, 3);
			this._dashboardTable.Controls.Add(this._localServerConfigLink, 7, 5);
			this._dashboardTable.Controls.Add(this._diskSpacePanel, 4, 5);
			this._dashboardTable.Controls.Add(this._diskSpace, 5, 4);
			this._dashboardTable.Controls.Add(this._logFileLink, 7, 6);
			this._dashboardTable.Controls.Add(this._studyRulesLink, 7, 7);
			this._dashboardTable.Controls.Add(this._statusLight, 0, 1);
			this._dashboardTable.Controls.Add(this._serverDetailsTable, 1, 1);
			this._dashboardTable.Name = "_dashboardTable";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label10
			// 
			resources.ApplyResources(this.label10, "label10");
			this.label10.Name = "label10";
			// 
			// label11
			// 
			resources.ApplyResources(this.label11, "label11");
			this.label11.Name = "label11";
			// 
			// _totalStudies
			// 
			resources.ApplyResources(this._totalStudies, "_totalStudies");
			this._totalStudies.Name = "_totalStudies";
			// 
			// _failures
			// 
			resources.ApplyResources(this._failures, "_failures");
			this._failures.Name = "_failures";
			// 
			// _reindexLink
			// 
			resources.ApplyResources(this._reindexLink, "_reindexLink");
			this._reindexLink.Name = "_reindexLink";
			this._reindexLink.TabStop = true;
			this._reindexLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._reindexLink_LinkClicked);
			// 
			// _openFileStoreLink
			// 
			resources.ApplyResources(this._openFileStoreLink, "_openFileStoreLink");
			this._openFileStoreLink.Name = "_openFileStoreLink";
			this._openFileStoreLink.TabStop = true;
			this._openFileStoreLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._openFileStoreLink_LinkClicked);
			// 
			// _localServerConfigLink
			// 
			resources.ApplyResources(this._localServerConfigLink, "_localServerConfigLink");
			this._localServerConfigLink.Name = "_localServerConfigLink";
			this._localServerConfigLink.TabStop = true;
			this._localServerConfigLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._localServerConfigLink_LinkClicked);
			// 
			// _diskSpacePanel
			// 
			this._dashboardTable.SetColumnSpan(this._diskSpacePanel, 2);
			this._diskSpacePanel.Controls.Add(this._diskSpaceMeter);
			this._diskSpacePanel.Controls.Add(this._diskSpaceWarningIcon);
			this._diskSpacePanel.Controls.Add(this._diskSpaceWarningMessage);
			resources.ApplyResources(this._diskSpacePanel, "_diskSpacePanel");
			this._diskSpacePanel.Name = "_diskSpacePanel";
			this._dashboardTable.SetRowSpan(this._diskSpacePanel, 3);
			// 
			// _diskSpaceMeter
			// 
			resources.ApplyResources(this._diskSpaceMeter, "_diskSpaceMeter");
			this._diskSpaceMeter.Name = "_diskSpaceMeter";
			// 
			// _diskSpaceWarningIcon
			// 
			resources.ApplyResources(this._diskSpaceWarningIcon, "_diskSpaceWarningIcon");
			this._diskSpaceWarningIcon.Name = "_diskSpaceWarningIcon";
			this._diskSpaceWarningIcon.TabStop = false;
			// 
			// _diskSpaceWarningMessage
			// 
			resources.ApplyResources(this._diskSpaceWarningMessage, "_diskSpaceWarningMessage");
			this._diskSpaceWarningMessage.AutoEllipsis = true;
			this._diskSpaceWarningMessage.ForeColor = System.Drawing.Color.Red;
			this._diskSpaceWarningMessage.Name = "_diskSpaceWarningMessage";
			// 
			// _diskSpace
			// 
			this._diskSpace.AutoEllipsis = true;
			resources.ApplyResources(this._diskSpace, "_diskSpace");
			this._diskSpace.Name = "_diskSpace";
			// 
			// _logFileLink
			// 
			resources.ApplyResources(this._logFileLink, "_logFileLink");
			this._logFileLink.Name = "_logFileLink";
			this._logFileLink.TabStop = true;
			this._logFileLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._logFileLink_LinkClicked);
			// 
			// _studyRulesLink
			// 
			resources.ApplyResources(this._studyRulesLink, "_studyRulesLink");
			this._studyRulesLink.Name = "_studyRulesLink";
			this._studyRulesLink.TabStop = true;
			this._studyRulesLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._studyRulesLink_LinkClicked);
			// 
			// _statusLight
			// 
			resources.ApplyResources(this._statusLight, "_statusLight");
			this._statusLight.LightHoverTextGreen = "Local server running";
			this._statusLight.LightHoverTextRed = "Local server not running";
			this._statusLight.LightHoverTextYellow = null;
			this._statusLight.LinkHoverTextGreen = null;
			this._statusLight.LinkHoverTextRed = null;
			this._statusLight.LinkHoverTextYellow = null;
			this._statusLight.LinkVisible = false;
			this._statusLight.Name = "_statusLight";
			this._dashboardTable.SetRowSpan(this._statusLight, 2);
			// 
			// _serverDetailsTable
			// 
			resources.ApplyResources(this._serverDetailsTable, "_serverDetailsTable");
			this._dashboardTable.SetColumnSpan(this._serverDetailsTable, 5);
			this._serverDetailsTable.Controls.Add(this._hostName, 0, 1);
			this._serverDetailsTable.Controls.Add(this._aeTitle, 0, 0);
			this._serverDetailsTable.Name = "_serverDetailsTable";
			this._dashboardTable.SetRowSpan(this._serverDetailsTable, 3);
			// 
			// _hostName
			// 
			resources.ApplyResources(this._hostName, "_hostName");
			this._hostName.Name = "_hostName";
			// 
			// _aeTitle
			// 
			resources.ApplyResources(this._aeTitle, "_aeTitle");
			this._aeTitle.Name = "_aeTitle";
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._overviewPanel);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.panel1);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._workItemToolStrip);
			this.panel1.Controls.Add(this._workItemsTableView);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _workItemToolStrip
			// 
			resources.ApplyResources(this._workItemToolStrip, "_workItemToolStrip");
			this._workItemToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._workItemToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._workItemToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this._activityFilter,
            this.toolStripLabel2,
            this._statusFilter,
            this.toolStripLabel3,
            this._textFilter,
            this.toolStripSeparator1});
			this._workItemToolStrip.Name = "_workItemToolStrip";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			resources.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
			// 
			// _activityFilter
			// 
			this._activityFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._activityFilter.Name = "_activityFilter";
			resources.ApplyResources(this._activityFilter, "_activityFilter");
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.Name = "toolStripLabel2";
			resources.ApplyResources(this.toolStripLabel2, "toolStripLabel2");
			// 
			// _statusFilter
			// 
			this._statusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._statusFilter.Name = "_statusFilter";
			resources.ApplyResources(this._statusFilter, "_statusFilter");
			// 
			// toolStripLabel3
			// 
			this.toolStripLabel3.Name = "toolStripLabel3";
			resources.ApplyResources(this.toolStripLabel3, "toolStripLabel3");
			// 
			// _textFilter
			// 
			this._textFilter.Name = "_textFilter";
			resources.ApplyResources(this._textFilter, "_textFilter");
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// ActivityMonitorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "ActivityMonitorComponentControl";
			this._overviewPanel.ResumeLayout(false);
			this._dashboardTable.ResumeLayout(false);
			this._dashboardTable.PerformLayout();
			this._diskSpacePanel.ResumeLayout(false);
			this._diskSpacePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._diskSpaceWarningIcon)).EndInit();
			this._serverDetailsTable.ResumeLayout(false);
			this._serverDetailsTable.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this._workItemToolStrip.ResumeLayout(false);
			this._workItemToolStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _workItemsTableView;
		private System.Windows.Forms.Panel _overviewPanel;
		private System.Windows.Forms.Label label1;
		private Meter _diskSpaceMeter;
		private System.Windows.Forms.Label _hostName;
		private System.Windows.Forms.LinkLabel _studyRulesLink;
		private System.Windows.Forms.LinkLabel _localServerConfigLink;
        private System.Windows.Forms.TableLayoutPanel _dashboardTable;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label _totalStudies;
		private System.Windows.Forms.Label _failures;
		private System.Windows.Forms.Label _diskSpace;
		private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel _diskSpacePanel;
		private IndicatorLight _statusLight;
		private System.Windows.Forms.LinkLabel _reindexLink;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolTip _toolTip;
		private System.Windows.Forms.LinkLabel _openFileStoreLink;
        private System.Windows.Forms.Label _diskSpaceWarningMessage;
        private System.Windows.Forms.PictureBox _diskSpaceWarningIcon;
        private System.Windows.Forms.ToolStrip _workItemToolStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox _activityFilter;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox _statusFilter;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox _textFilter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.LinkLabel _logFileLink;
        private System.Windows.Forms.Label _aeTitle;
        private System.Windows.Forms.TableLayoutPanel _serverDetailsTable;

	}
}
