namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems {
	partial class ListFilterControl {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListFilterControl));
			this._listBox = new System.Windows.Forms.CheckedListBox();
			this._buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._btnCancel = new System.Windows.Forms.Button();
			this._btnOk = new System.Windows.Forms.Button();
			this._buttonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _listBox
			// 
			this._listBox.CheckOnClick = true;
			resources.ApplyResources(this._listBox, "_listBox");
			this._listBox.FormattingEnabled = true;
			this._listBox.Name = "_listBox";
			this._listBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._listBox_ItemCheck);
			this._listBox.MouseLeave += new System.EventHandler(this._listBox_MouseLeave);
			// 
			// _buttonPanel
			// 
			this._buttonPanel.Controls.Add(this._btnCancel);
			this._buttonPanel.Controls.Add(this._btnOk);
			resources.ApplyResources(this._buttonPanel, "_buttonPanel");
			this._buttonPanel.Name = "_buttonPanel";
			// 
			// _btnCancel
			// 
			resources.ApplyResources(this._btnCancel, "_btnCancel");
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
			// 
			// _btnOk
			// 
			resources.ApplyResources(this._btnOk, "_btnOk");
			this._btnOk.Name = "_btnOk";
			this._btnOk.UseVisualStyleBackColor = true;
			this._btnOk.Click += new System.EventHandler(this._btnOk_Click);
			// 
			// ListFilterControl
			// 
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this._listBox);
			this.Controls.Add(this._buttonPanel);
			this.Name = "ListFilterControl";
			resources.ApplyResources(this, "$this");
			this._buttonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckedListBox _listBox;
		private System.Windows.Forms.FlowLayoutPanel _buttonPanel;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Button _btnOk;

	}
}
