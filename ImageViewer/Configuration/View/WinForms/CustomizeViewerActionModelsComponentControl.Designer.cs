namespace ClearCanvas.ImageViewer.Configuration.View.WinForms {
	partial class CustomizeViewerActionModelsComponentControl {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomizeViewerActionModelsComponentControl));
            this._pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnOk = new System.Windows.Forms.Button();
            this._pnlMain = new System.Windows.Forms.Panel();
            this._pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pnlButtons
            // 
            this._pnlButtons.Controls.Add(this._btnCancel);
            this._pnlButtons.Controls.Add(this._btnOk);
            resources.ApplyResources(this._pnlButtons, "_pnlButtons");
            this._pnlButtons.Name = "_pnlButtons";
            // 
            // _btnCancel
            // 
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this._btnCancel, "_btnCancel");
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // _btnOk
            // 
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this._btnOk, "_btnOk");
            this._btnOk.Name = "_btnOk";
            this._btnOk.UseVisualStyleBackColor = true;
            this._btnOk.Click += new System.EventHandler(this._btnOk_Click);
            // 
            // _pnlMain
            // 
            resources.ApplyResources(this._pnlMain, "_pnlMain");
            this._pnlMain.Name = "_pnlMain";
            // 
            // CustomizeViewerActionModelsComponentControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._pnlMain);
            this.Controls.Add(this._pnlButtons);
            this.Name = "CustomizeViewerActionModelsComponentControl";
            this._pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel _pnlButtons;
		private System.Windows.Forms.Panel _pnlMain;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Button _btnOk;
	}
}
