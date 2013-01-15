namespace ClearCanvas.Desktop.Help
{
	partial class UpdateAvailableForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateAvailableForm));
			this._ok = new System.Windows.Forms.Button();
			this._text = new System.Windows.Forms.Label();
			this._link = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// _ok
			// 
			resources.ApplyResources(this._ok, "_ok");
			this._ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._ok.Name = "_ok";
			this._ok.UseVisualStyleBackColor = true;
			this._ok.Click += new System.EventHandler(this.OnOk);
			// 
			// _text
			// 
			resources.ApplyResources(this._text, "_text");
			this._text.Name = "_text";
			// 
			// _link
			// 
			resources.ApplyResources(this._link, "_link");
			this._link.Name = "_link";
			this._link.TabStop = true;
			this._link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnDownloadNow);
			// 
			// UpdateAvailableForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._ok;
			this.Controls.Add(this._ok);
			this.Controls.Add(this._link);
			this.Controls.Add(this._text);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateAvailableForm";
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.Label _text;
		private System.Windows.Forms.LinkLabel _link;

	}
}
