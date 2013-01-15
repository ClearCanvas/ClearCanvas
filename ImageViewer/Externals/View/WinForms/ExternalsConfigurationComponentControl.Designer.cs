namespace ClearCanvas.ImageViewer.Externals.View.WinForms
{
	partial class ExternalsConfigurationComponentControl {
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
			System.Windows.Forms.Panel _pnlContent;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExternalsConfigurationComponentControl));
			System.Windows.Forms.FlowLayoutPanel _pnlButtons;
			this._listExternals = new System.Windows.Forms.ListView();
			this.colLabel = new System.Windows.Forms.ColumnHeader();
			this.colDescription = new System.Windows.Forms.ColumnHeader();
			this._btnAdd = new System.Windows.Forms.Button();
			this._btnRemove = new System.Windows.Forms.Button();
			this._btnEdit = new System.Windows.Forms.Button();
			this._mnuExternalTypes = new System.Windows.Forms.ContextMenuStrip(this.components);
			_pnlContent = new System.Windows.Forms.Panel();
			_pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
			_pnlContent.SuspendLayout();
			_pnlButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pnlContent
			// 
			_pnlContent.Controls.Add(this._listExternals);
			_pnlContent.Controls.Add(_pnlButtons);
			resources.ApplyResources(_pnlContent, "_pnlContent");
			_pnlContent.Name = "_pnlContent";
			// 
			// _listExternals
			// 
			this._listExternals.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLabel,
            this.colDescription});
			resources.ApplyResources(this._listExternals, "_listExternals");
			this._listExternals.FullRowSelect = true;
			this._listExternals.GridLines = true;
			this._listExternals.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._listExternals.HideSelection = false;
			this._listExternals.LabelEdit = true;
			this._listExternals.MultiSelect = false;
			this._listExternals.Name = "_listExternals";
			this._listExternals.UseCompatibleStateImageBehavior = false;
			this._listExternals.View = System.Windows.Forms.View.Details;
			this._listExternals.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this._listExternals_AfterLabelEdit);
			this._listExternals.SelectedIndexChanged += new System.EventHandler(this._listExternals_SelectedIndexChanged);
			this._listExternals.DoubleClick += new System.EventHandler(this._listExternals_DoubleClick);
			// 
			// colLabel
			// 
			resources.ApplyResources(this.colLabel, "colLabel");
			// 
			// colDescription
			// 
			resources.ApplyResources(this.colDescription, "colDescription");
			// 
			// _pnlButtons
			// 
			_pnlButtons.Controls.Add(this._btnAdd);
			_pnlButtons.Controls.Add(this._btnRemove);
			_pnlButtons.Controls.Add(this._btnEdit);
			resources.ApplyResources(_pnlButtons, "_pnlButtons");
			_pnlButtons.Name = "_pnlButtons";
			// 
			// _btnAdd
			// 
			resources.ApplyResources(this._btnAdd, "_btnAdd");
			this._btnAdd.Name = "_btnAdd";
			this._btnAdd.UseVisualStyleBackColor = true;
			this._btnAdd.Click += new System.EventHandler(this._btnAdd_Click);
			// 
			// _btnRemove
			// 
			resources.ApplyResources(this._btnRemove, "_btnRemove");
			this._btnRemove.Name = "_btnRemove";
			this._btnRemove.UseVisualStyleBackColor = true;
			this._btnRemove.Click += new System.EventHandler(this._btnRemove_Click);
			// 
			// _btnEdit
			// 
			resources.ApplyResources(this._btnEdit, "_btnEdit");
			this._btnEdit.Name = "_btnEdit";
			this._btnEdit.UseVisualStyleBackColor = true;
			this._btnEdit.Click += new System.EventHandler(this._btnEdit_Click);
			// 
			// _mnuExternalTypes
			// 
			this._mnuExternalTypes.Name = "_mnuExternalTypes";
			resources.ApplyResources(this._mnuExternalTypes, "_mnuExternalTypes");
			// 
			// ExternalsConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(_pnlContent);
			this.Name = "ExternalsConfigurationComponentControl";
			_pnlContent.ResumeLayout(false);
			_pnlButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView _listExternals;
		private System.Windows.Forms.ColumnHeader colLabel;
		private System.Windows.Forms.Button _btnAdd;
		private System.Windows.Forms.Button _btnRemove;
		private System.Windows.Forms.ColumnHeader colDescription;
		private System.Windows.Forms.Button _btnEdit;
		private System.Windows.Forms.ContextMenuStrip _mnuExternalTypes;
	}
}