namespace ClearCanvas.Desktop.View.WinForms
{
	partial class WorkspaceDialogBoxForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkspaceDialogBoxForm));
			this._contentPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// _contentPanel
			// 
			resources.ApplyResources(this._contentPanel, "_contentPanel");
			this._contentPanel.Name = "_contentPanel";
			// 
			// WorkspaceDialogBoxForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._contentPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WorkspaceDialogBoxForm";
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _contentPanel;
	}
}