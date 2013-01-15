namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	partial class ToolbarConfigurationComponentControl {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolbarConfigurationComponentControl));
			this._flowToolbarPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._wrapPanel = new System.Windows.Forms.Panel();
			this._wrapToolbars = new System.Windows.Forms.CheckBox();
			this._toolbarSize = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._flowToolbarPanel.SuspendLayout();
			this._wrapPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _flowToolbarPanel
			// 
			this._flowToolbarPanel.Controls.Add(this._toolbarSize);
			this._flowToolbarPanel.Controls.Add(this._wrapPanel);
			resources.ApplyResources(this._flowToolbarPanel, "_flowToolbarPanel");
			this._flowToolbarPanel.Name = "_flowToolbarPanel";
			// 
			// _wrapPanel
			// 
			this._wrapPanel.Controls.Add(this._wrapToolbars);
			resources.ApplyResources(this._wrapPanel, "_wrapPanel");
			this._wrapPanel.Name = "_wrapPanel";
			// 
			// _wrapToolbars
			// 
			resources.ApplyResources(this._wrapToolbars, "_wrapToolbars");
			this._wrapToolbars.Name = "_wrapToolbars";
			this._wrapToolbars.UseVisualStyleBackColor = true;
			// 
			// _toolbarSize
			// 
			resources.ApplyResources(this._toolbarSize, "_toolbarSize");
			this._toolbarSize.DataSource = null;
			this._toolbarSize.DisplayMember = "";
			this._toolbarSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._toolbarSize.Name = "_toolbarSize";
			this._toolbarSize.Value = null;
			// 
			// ToolbarConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._flowToolbarPanel);
			this.Name = "ToolbarConfigurationComponentControl";
			this._flowToolbarPanel.ResumeLayout(false);
			this._flowToolbarPanel.PerformLayout();
			this._wrapPanel.ResumeLayout(false);
			this._wrapPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox _wrapToolbars;
		private System.Windows.Forms.Panel _wrapPanel;
		private System.Windows.Forms.FlowLayoutPanel _flowToolbarPanel;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _toolbarSize;
	}
}