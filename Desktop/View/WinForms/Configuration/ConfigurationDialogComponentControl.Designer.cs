namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	partial class ConfigurationDialogComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationDialogComponentControl));
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._warningLayoutTable = new System.Windows.Forms.TableLayoutPanel();
			this._warningMessage = new System.Windows.Forms.Label();
			this._warningIcon = new System.Windows.Forms.Label();
			this._tableLayoutPanel.SuspendLayout();
			this._warningLayoutTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayoutPanel
			// 
			resources.ApplyResources(this._tableLayoutPanel, "_tableLayoutPanel");
			this._tableLayoutPanel.Controls.Add(this._warningLayoutTable, 0, 0);
			this._tableLayoutPanel.MinimumSize = new System.Drawing.Size(500, 300);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			// 
			// _warningLayoutTable
			// 
			resources.ApplyResources(this._warningLayoutTable, "_warningLayoutTable");
			this._warningLayoutTable.Controls.Add(this._warningIcon, 0, 0);
			this._warningLayoutTable.Controls.Add(this._warningMessage, 1, 0);
			this._warningLayoutTable.Name = "_warningLayoutTable";
			// 
			// _warningMessage
			// 
			this._warningMessage.AutoEllipsis = true;
			resources.ApplyResources(this._warningMessage, "_warningMessage");
			this._warningMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._warningMessage.Name = "_warningMessage";
			// 
			// _warningIcon
			// 
			resources.ApplyResources(this._warningIcon, "_warningIcon");
			this._warningIcon.Name = "_warningIcon";
			// 
			// ConfigurationDialogComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tableLayoutPanel);
			this.MinimumSize = new System.Drawing.Size(500, 300);
			this.Name = "ConfigurationDialogComponentControl";
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			this._warningLayoutTable.ResumeLayout(false);
			this._warningLayoutTable.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private System.Windows.Forms.TableLayoutPanel _warningLayoutTable;
		private System.Windows.Forms.Label _warningMessage;
		private System.Windows.Forms.Label _warningIcon;
	}
}
