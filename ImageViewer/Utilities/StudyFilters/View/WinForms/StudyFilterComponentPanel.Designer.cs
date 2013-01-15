namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	partial class StudyFilterComponentPanel {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StudyFilterComponentPanel));
			this._tableView = new ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.StudyFilterTableView();
			this._workspaceDivisor = new System.Windows.Forms.SplitContainer();
			this._toolbar = new System.Windows.Forms.ToolStrip();
			this._workspaceDivisor.Panel1.SuspendLayout();
			this._workspaceDivisor.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableView
			// 
			this._tableView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
			this._tableView.ColumnHeaderTooltip = null;
			resources.ApplyResources(this._tableView, "_tableView");
			this._tableView.Name = "_tableView";
			this._tableView.ReadOnly = false;
			this._tableView.ShowToolbar = false;
			this._tableView.SmartColumnSizing = true;
			this._tableView.SortButtonTooltip = null;
			this._tableView.StatusBarVisible = true;
			this._tableView.SelectionChanged += new System.EventHandler(this._tableView_SelectionChanged);
			// 
			// _workspaceDivisor
			// 
			resources.ApplyResources(this._workspaceDivisor, "_workspaceDivisor");
			this._workspaceDivisor.Name = "_workspaceDivisor";
			// 
			// _workspaceDivisor.Panel1
			// 
			this._workspaceDivisor.Panel1.Controls.Add(this._tableView);
			this._workspaceDivisor.Panel1.Controls.Add(this._toolbar);
			this._workspaceDivisor.Panel2Collapsed = true;
			// 
			// _toolbar
			// 
			resources.ApplyResources(this._toolbar, "_toolbar");
			this._toolbar.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._toolbar.Name = "_toolbar";
			// 
			// StudyFilterComponentPanel
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._workspaceDivisor);
			this.Name = "StudyFilterComponentPanel";
			this._workspaceDivisor.Panel1.ResumeLayout(false);
			this._workspaceDivisor.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.StudyFilterTableView _tableView;
		private System.Windows.Forms.SplitContainer _workspaceDivisor;
		private System.Windows.Forms.ToolStrip _toolbar;
	}
}