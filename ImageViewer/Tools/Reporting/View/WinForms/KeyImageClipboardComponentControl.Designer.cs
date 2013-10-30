namespace ClearCanvas.ImageViewer.Tools.Reporting.View.WinForms
{
	partial class KeyImageClipboardComponentControl
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
			this._pnlSelectContext = new System.Windows.Forms.Panel();
			this._cboCurrentContext = new System.Windows.Forms.ComboBox();
			this._pnlMain = new System.Windows.Forms.Panel();
			this._pnlSelectContext.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pnlSelectContext
			// 
			this._pnlSelectContext.Controls.Add(this._cboCurrentContext);
			this._pnlSelectContext.Dock = System.Windows.Forms.DockStyle.Top;
			this._pnlSelectContext.Location = new System.Drawing.Point(0, 0);
			this._pnlSelectContext.Name = "_pnlSelectContext";
			this._pnlSelectContext.Padding = new System.Windows.Forms.Padding(1);
			this._pnlSelectContext.Size = new System.Drawing.Size(310, 23);
			this._pnlSelectContext.TabIndex = 0;
			// 
			// _cboCurrentContext
			// 
			this._cboCurrentContext.Dock = System.Windows.Forms.DockStyle.Fill;
			this._cboCurrentContext.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboCurrentContext.FormattingEnabled = true;
			this._cboCurrentContext.Location = new System.Drawing.Point(1, 1);
			this._cboCurrentContext.Name = "_cboCurrentContext";
			this._cboCurrentContext.Size = new System.Drawing.Size(308, 21);
			this._cboCurrentContext.TabIndex = 1;
			// 
			// _pnlMain
			// 
			this._pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this._pnlMain.Location = new System.Drawing.Point(0, 23);
			this._pnlMain.Name = "_pnlMain";
			this._pnlMain.Size = new System.Drawing.Size(310, 507);
			this._pnlMain.TabIndex = 1;
			// 
			// KeyImageClipboardComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._pnlMain);
			this.Controls.Add(this._pnlSelectContext);
			this.Name = "KeyImageClipboardComponentControl";
			this.Size = new System.Drawing.Size(310, 530);
			this._pnlSelectContext.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _pnlSelectContext;
		private System.Windows.Forms.ComboBox _cboCurrentContext;
		private System.Windows.Forms.Panel _pnlMain;
	}
}
