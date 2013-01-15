namespace ClearCanvas.ImageViewer.Tools.Reporting.View.WinForms
{
	partial class KeyImageInformationEditorComponentControl {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyImageInformationEditorComponentControl));
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.pnlTitle = new System.Windows.Forms.Panel();
			this.cboTitle = new System.Windows.Forms.ComboBox();
			this.lblTitle = new System.Windows.Forms.Label();
			this.pnlDesc = new System.Windows.Forms.Panel();
			this.txtDesc = new System.Windows.Forms.TextBox();
			this.lblDesc = new System.Windows.Forms.Label();
			this.pnlSeriesDesc = new System.Windows.Forms.Panel();
			this.txtSeriesDesc = new System.Windows.Forms.TextBox();
			this.lblSeriesDesc = new System.Windows.Forms.Label();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1.SuspendLayout();
			this.pnlTitle.SuspendLayout();
			this.pnlDesc.SuspendLayout();
			this.pnlSeriesDesc.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Controls.Add(this.pnlTitle);
			this.flowLayoutPanel1.Controls.Add(this.pnlDesc);
			this.flowLayoutPanel1.Controls.Add(this.pnlSeriesDesc);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// pnlTitle
			// 
			this.pnlTitle.Controls.Add(this.cboTitle);
			this.pnlTitle.Controls.Add(this.lblTitle);
			resources.ApplyResources(this.pnlTitle, "pnlTitle");
			this.pnlTitle.Name = "pnlTitle";
			// 
			// cboTitle
			// 
			resources.ApplyResources(this.cboTitle, "cboTitle");
			this.cboTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboTitle.FormattingEnabled = true;
			this.cboTitle.Name = "cboTitle";
			// 
			// lblTitle
			// 
			resources.ApplyResources(this.lblTitle, "lblTitle");
			this.lblTitle.Name = "lblTitle";
			// 
			// pnlDesc
			// 
			this.pnlDesc.Controls.Add(this.txtDesc);
			this.pnlDesc.Controls.Add(this.lblDesc);
			resources.ApplyResources(this.pnlDesc, "pnlDesc");
			this.pnlDesc.Name = "pnlDesc";
			// 
			// txtDesc
			// 
			resources.ApplyResources(this.txtDesc, "txtDesc");
			this.txtDesc.Name = "txtDesc";
			// 
			// lblDesc
			// 
			resources.ApplyResources(this.lblDesc, "lblDesc");
			this.lblDesc.Name = "lblDesc";
			// 
			// pnlSeriesDesc
			// 
			this.pnlSeriesDesc.Controls.Add(this.txtSeriesDesc);
			this.pnlSeriesDesc.Controls.Add(this.lblSeriesDesc);
			resources.ApplyResources(this.pnlSeriesDesc, "pnlSeriesDesc");
			this.pnlSeriesDesc.Name = "pnlSeriesDesc";
			// 
			// txtSeriesDesc
			// 
			resources.ApplyResources(this.txtSeriesDesc, "txtSeriesDesc");
			this.txtSeriesDesc.Name = "txtSeriesDesc";
			// 
			// lblSeriesDesc
			// 
			resources.ApplyResources(this.lblSeriesDesc, "lblSeriesDesc");
			this.lblSeriesDesc.Name = "lblSeriesDesc";
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this.OnCancel);
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this.OnOk);
			// 
			// KeyImageInformationEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "KeyImageInformationEditorComponentControl";
			this.flowLayoutPanel1.ResumeLayout(false);
			this.pnlTitle.ResumeLayout(false);
			this.pnlTitle.PerformLayout();
			this.pnlDesc.ResumeLayout(false);
			this.pnlDesc.PerformLayout();
			this.pnlSeriesDesc.ResumeLayout(false);
			this.pnlSeriesDesc.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Panel pnlTitle;
		private System.Windows.Forms.ComboBox cboTitle;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Panel pnlDesc;
		private System.Windows.Forms.TextBox txtDesc;
		private System.Windows.Forms.Label lblDesc;
		private System.Windows.Forms.Panel pnlSeriesDesc;
		private System.Windows.Forms.TextBox txtSeriesDesc;
		private System.Windows.Forms.Label lblSeriesDesc;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
	}
}