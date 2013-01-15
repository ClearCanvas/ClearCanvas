namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms {
	partial class ColumnPickerComponentPanel {
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.GroupBox _grpAvailableColumns;
			System.Windows.Forms.GroupBox _grpDicomColumns;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnPickerComponentPanel));
			System.Windows.Forms.GroupBox _grpDicomTag;
			System.Windows.Forms.Label _lblDicomTagElement;
			System.Windows.Forms.Label _lblDicomTagGroup;
			System.Windows.Forms.GroupBox _grpSpecialColumns;
			System.Windows.Forms.GroupBox _grpSelectedColumns;
			this._btnAddDicomColumn = new System.Windows.Forms.Button();
			this._lstDicomColumns = new System.Windows.Forms.ListBox();
			this._txtFilterDicomColumns = new System.Windows.Forms.TextBox();
			this._splitSpecialColumnsResizer = new System.Windows.Forms.Splitter();
			this._btnAddDicomTag = new System.Windows.Forms.Button();
			this._txtDicomTagElement = new System.Windows.Forms.TextBox();
			this._txtDicomTagGroup = new System.Windows.Forms.TextBox();
			this._btnAddSpecialColumn = new System.Windows.Forms.Button();
			this._lstSpecialColumns = new System.Windows.Forms.ListBox();
			this._btnMoveColumnDown = new System.Windows.Forms.Button();
			this._btnMoveColumnUp = new System.Windows.Forms.Button();
			this._btnDelColumn = new System.Windows.Forms.Button();
			this._lstSelectedColumns = new System.Windows.Forms.ListBox();
			this.pnlSplitter = new System.Windows.Forms.SplitContainer();
			this._tooltips = new System.Windows.Forms.ToolTip(this.components);
			_grpAvailableColumns = new System.Windows.Forms.GroupBox();
			_grpDicomColumns = new System.Windows.Forms.GroupBox();
			_grpDicomTag = new System.Windows.Forms.GroupBox();
			_lblDicomTagElement = new System.Windows.Forms.Label();
			_lblDicomTagGroup = new System.Windows.Forms.Label();
			_grpSpecialColumns = new System.Windows.Forms.GroupBox();
			_grpSelectedColumns = new System.Windows.Forms.GroupBox();
			_grpAvailableColumns.SuspendLayout();
			_grpDicomColumns.SuspendLayout();
			_grpDicomTag.SuspendLayout();
			_grpSpecialColumns.SuspendLayout();
			_grpSelectedColumns.SuspendLayout();
			this.pnlSplitter.Panel1.SuspendLayout();
			this.pnlSplitter.Panel2.SuspendLayout();
			this.pnlSplitter.SuspendLayout();
			this.SuspendLayout();
			// 
			// _grpAvailableColumns
			// 
			_grpAvailableColumns.Controls.Add(_grpDicomColumns);
			_grpAvailableColumns.Controls.Add(this._splitSpecialColumnsResizer);
			_grpAvailableColumns.Controls.Add(_grpDicomTag);
			_grpAvailableColumns.Controls.Add(_grpSpecialColumns);
			resources.ApplyResources(_grpAvailableColumns, "_grpAvailableColumns");
			_grpAvailableColumns.Name = "_grpAvailableColumns";
			_grpAvailableColumns.TabStop = false;
			// 
			// _grpDicomColumns
			// 
			_grpDicomColumns.Controls.Add(this._btnAddDicomColumn);
			_grpDicomColumns.Controls.Add(this._lstDicomColumns);
			_grpDicomColumns.Controls.Add(this._txtFilterDicomColumns);
			resources.ApplyResources(_grpDicomColumns, "_grpDicomColumns");
			_grpDicomColumns.Name = "_grpDicomColumns";
			_grpDicomColumns.TabStop = false;
			// 
			// _btnAddDicomColumn
			// 
			resources.ApplyResources(this._btnAddDicomColumn, "_btnAddDicomColumn");
			this._btnAddDicomColumn.Name = "_btnAddDicomColumn";
			this._tooltips.SetToolTip(this._btnAddDicomColumn, resources.GetString("_btnAddDicomColumn.ToolTip"));
			this._btnAddDicomColumn.UseVisualStyleBackColor = true;
			this._btnAddDicomColumn.Click += new System.EventHandler(this.OnAddDicomColumnClick);
			// 
			// _lstDicomColumns
			// 
			resources.ApplyResources(this._lstDicomColumns, "_lstDicomColumns");
			this._lstDicomColumns.FormattingEnabled = true;
			this._lstDicomColumns.Name = "_lstDicomColumns";
			this._lstDicomColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this._lstDicomColumns.Sorted = true;
			this._lstDicomColumns.DoubleClick += new System.EventHandler(this._lstDicomColumns_DoubleClick);
			this._lstDicomColumns.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._lstDicomColumns_PreviewKeyDown);
			this._lstDicomColumns.KeyDown += new System.Windows.Forms.KeyEventHandler(this._lstDicomColumns_KeyDown);
			// 
			// _txtFilterDicomColumns
			// 
			resources.ApplyResources(this._txtFilterDicomColumns, "_txtFilterDicomColumns");
			this._txtFilterDicomColumns.Name = "_txtFilterDicomColumns";
			this._tooltips.SetToolTip(this._txtFilterDicomColumns, resources.GetString("_txtFilterDicomColumns.ToolTip"));
			this._txtFilterDicomColumns.TextChanged += new System.EventHandler(this._txtFilterDicomColumns_TextChanged);
			this._txtFilterDicomColumns.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._txtFilterDicomColumns_PreviewKeyDown);
			this._txtFilterDicomColumns.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtFilterDicomColumns_KeyDown);
			// 
			// _splitSpecialColumnsResizer
			// 
			resources.ApplyResources(this._splitSpecialColumnsResizer, "_splitSpecialColumnsResizer");
			this._splitSpecialColumnsResizer.Name = "_splitSpecialColumnsResizer";
			this._splitSpecialColumnsResizer.TabStop = false;
			// 
			// _grpDicomTag
			// 
			_grpDicomTag.Controls.Add(this._btnAddDicomTag);
			_grpDicomTag.Controls.Add(this._txtDicomTagElement);
			_grpDicomTag.Controls.Add(_lblDicomTagElement);
			_grpDicomTag.Controls.Add(this._txtDicomTagGroup);
			_grpDicomTag.Controls.Add(_lblDicomTagGroup);
			resources.ApplyResources(_grpDicomTag, "_grpDicomTag");
			_grpDicomTag.Name = "_grpDicomTag";
			_grpDicomTag.TabStop = false;
			// 
			// _btnAddDicomTag
			// 
			resources.ApplyResources(this._btnAddDicomTag, "_btnAddDicomTag");
			this._btnAddDicomTag.Name = "_btnAddDicomTag";
			this._tooltips.SetToolTip(this._btnAddDicomTag, resources.GetString("_btnAddDicomTag.ToolTip"));
			this._btnAddDicomTag.UseVisualStyleBackColor = true;
			this._btnAddDicomTag.Click += new System.EventHandler(this.OnAddDicomTagClick);
			// 
			// _txtDicomTagElement
			// 
			resources.ApplyResources(this._txtDicomTagElement, "_txtDicomTagElement");
			this._txtDicomTagElement.Name = "_txtDicomTagElement";
			this._txtDicomTagElement.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._txtDicomTagElement_PreviewKeyDown);
			this._txtDicomTagElement.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtDicomTagElement_KeyDown);
			// 
			// _lblDicomTagElement
			// 
			resources.ApplyResources(_lblDicomTagElement, "_lblDicomTagElement");
			_lblDicomTagElement.Name = "_lblDicomTagElement";
			// 
			// _txtDicomTagGroup
			// 
			resources.ApplyResources(this._txtDicomTagGroup, "_txtDicomTagGroup");
			this._txtDicomTagGroup.Name = "_txtDicomTagGroup";
			this._txtDicomTagGroup.TextChanged += new System.EventHandler(this._txtDicomTagGroup_TextChanged);
			this._txtDicomTagGroup.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._txtDicomTagGroup_PreviewKeyDown);
			this._txtDicomTagGroup.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtDicomTagGroup_KeyDown);
			// 
			// _lblDicomTagGroup
			// 
			resources.ApplyResources(_lblDicomTagGroup, "_lblDicomTagGroup");
			_lblDicomTagGroup.Name = "_lblDicomTagGroup";
			// 
			// _grpSpecialColumns
			// 
			_grpSpecialColumns.Controls.Add(this._btnAddSpecialColumn);
			_grpSpecialColumns.Controls.Add(this._lstSpecialColumns);
			resources.ApplyResources(_grpSpecialColumns, "_grpSpecialColumns");
			_grpSpecialColumns.Name = "_grpSpecialColumns";
			_grpSpecialColumns.TabStop = false;
			// 
			// _btnAddSpecialColumn
			// 
			resources.ApplyResources(this._btnAddSpecialColumn, "_btnAddSpecialColumn");
			this._btnAddSpecialColumn.Name = "_btnAddSpecialColumn";
			this._tooltips.SetToolTip(this._btnAddSpecialColumn, resources.GetString("_btnAddSpecialColumn.ToolTip"));
			this._btnAddSpecialColumn.UseVisualStyleBackColor = true;
			this._btnAddSpecialColumn.Click += new System.EventHandler(this.OnAddSpecialColumnClick);
			// 
			// _lstSpecialColumns
			// 
			resources.ApplyResources(this._lstSpecialColumns, "_lstSpecialColumns");
			this._lstSpecialColumns.FormattingEnabled = true;
			this._lstSpecialColumns.Name = "_lstSpecialColumns";
			this._lstSpecialColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this._lstSpecialColumns.Sorted = true;
			this._lstSpecialColumns.DoubleClick += new System.EventHandler(this._lstSpecialColumns_DoubleClick);
			this._lstSpecialColumns.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._lstSpecialColumns_PreviewKeyDown);
			this._lstSpecialColumns.KeyDown += new System.Windows.Forms.KeyEventHandler(this._lstSpecialColumns_KeyDown);
			// 
			// _grpSelectedColumns
			// 
			_grpSelectedColumns.Controls.Add(this._btnMoveColumnDown);
			_grpSelectedColumns.Controls.Add(this._btnMoveColumnUp);
			_grpSelectedColumns.Controls.Add(this._btnDelColumn);
			_grpSelectedColumns.Controls.Add(this._lstSelectedColumns);
			resources.ApplyResources(_grpSelectedColumns, "_grpSelectedColumns");
			_grpSelectedColumns.Name = "_grpSelectedColumns";
			_grpSelectedColumns.TabStop = false;
			// 
			// _btnMoveColumnDown
			// 
			resources.ApplyResources(this._btnMoveColumnDown, "_btnMoveColumnDown");
			this._btnMoveColumnDown.Name = "_btnMoveColumnDown";
			this._tooltips.SetToolTip(this._btnMoveColumnDown, resources.GetString("_btnMoveColumnDown.ToolTip"));
			this._btnMoveColumnDown.UseVisualStyleBackColor = true;
			this._btnMoveColumnDown.Click += new System.EventHandler(this._btnMoveColumnDown_Click);
			// 
			// _btnMoveColumnUp
			// 
			resources.ApplyResources(this._btnMoveColumnUp, "_btnMoveColumnUp");
			this._btnMoveColumnUp.Name = "_btnMoveColumnUp";
			this._tooltips.SetToolTip(this._btnMoveColumnUp, resources.GetString("_btnMoveColumnUp.ToolTip"));
			this._btnMoveColumnUp.UseVisualStyleBackColor = true;
			this._btnMoveColumnUp.Click += new System.EventHandler(this._btnMoveColumnUp_Click);
			// 
			// _btnDelColumn
			// 
			resources.ApplyResources(this._btnDelColumn, "_btnDelColumn");
			this._btnDelColumn.Name = "_btnDelColumn";
			this._tooltips.SetToolTip(this._btnDelColumn, resources.GetString("_btnDelColumn.ToolTip"));
			this._btnDelColumn.UseVisualStyleBackColor = true;
			this._btnDelColumn.Click += new System.EventHandler(this._btnDelColumn_Click);
			// 
			// _lstSelectedColumns
			// 
			resources.ApplyResources(this._lstSelectedColumns, "_lstSelectedColumns");
			this._lstSelectedColumns.FormattingEnabled = true;
			this._lstSelectedColumns.Name = "_lstSelectedColumns";
			this._lstSelectedColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			// 
			// pnlSplitter
			// 
			resources.ApplyResources(this.pnlSplitter, "pnlSplitter");
			this.pnlSplitter.Name = "pnlSplitter";
			// 
			// pnlSplitter.Panel1
			// 
			this.pnlSplitter.Panel1.Controls.Add(_grpAvailableColumns);
			// 
			// pnlSplitter.Panel2
			// 
			this.pnlSplitter.Panel2.Controls.Add(_grpSelectedColumns);
			// 
			// ColumnPickerComponentPanel
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlSplitter);
			this.Name = "ColumnPickerComponentPanel";
			_grpAvailableColumns.ResumeLayout(false);
			_grpDicomColumns.ResumeLayout(false);
			_grpDicomColumns.PerformLayout();
			_grpDicomTag.ResumeLayout(false);
			_grpDicomTag.PerformLayout();
			_grpSpecialColumns.ResumeLayout(false);
			_grpSelectedColumns.ResumeLayout(false);
			this.pnlSplitter.Panel1.ResumeLayout(false);
			this.pnlSplitter.Panel2.ResumeLayout(false);
			this.pnlSplitter.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox _lstSelectedColumns;
		private System.Windows.Forms.SplitContainer pnlSplitter;
		private System.Windows.Forms.ListBox _lstSpecialColumns;
		private System.Windows.Forms.Button _btnAddSpecialColumn;
		private System.Windows.Forms.Splitter _splitSpecialColumnsResizer;
		private System.Windows.Forms.Button _btnAddDicomColumn;
		private System.Windows.Forms.ListBox _lstDicomColumns;
		private System.Windows.Forms.TextBox _txtFilterDicomColumns;
		private System.Windows.Forms.Button _btnAddDicomTag;
		private System.Windows.Forms.TextBox _txtDicomTagElement;
		private System.Windows.Forms.TextBox _txtDicomTagGroup;
		private System.Windows.Forms.Button _btnMoveColumnDown;
		private System.Windows.Forms.Button _btnMoveColumnUp;
		private System.Windows.Forms.Button _btnDelColumn;
		private System.Windows.Forms.ToolTip _tooltips;
	}
}
