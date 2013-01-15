namespace ClearCanvas.Desktop.View.WinForms
{
	partial class UserUpgradeProgressForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserUpgradeProgressForm));
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._message = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// _progressBar
			// 
			resources.ApplyResources(this._progressBar, "_progressBar");
			this._progressBar.Name = "_progressBar";
			this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			// 
			// _message
			// 
			resources.ApplyResources(this._message, "_message");
			this._message.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._message.Name = "_message";
			this._message.ReadOnly = true;
			this._message.TabStop = false;
			// 
			// UserUpgradeProgressForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this._progressBar);
			this.Controls.Add(this._message);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UserUpgradeProgressForm";
			this.ShowInTaskbar = false;
			this.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar _progressBar;
		private System.Windows.Forms.TextBox _message;
	}
}