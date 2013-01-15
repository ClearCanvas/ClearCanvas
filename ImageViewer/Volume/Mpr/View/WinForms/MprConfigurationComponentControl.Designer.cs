namespace ClearCanvas.ImageViewer.Volume.Mpr.View.WinForms {
	partial class MprConfigurationComponentControl {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MprConfigurationComponentControl));
			this._grpSliceSpacing = new System.Windows.Forms.GroupBox();
			this._txtProportionalSliceSpacing = new System.Windows.Forms.TextBox();
			this._lblProportionalSliceSpacing = new System.Windows.Forms.Label();
			this._radAutomaticSliceSpacing = new System.Windows.Forms.RadioButton();
			this._radProportionalSliceSpacing = new System.Windows.Forms.RadioButton();
			this._grpSliceSpacing.SuspendLayout();
			this.SuspendLayout();
			// 
			// _grpSliceSpacing
			// 
			this._grpSliceSpacing.Controls.Add(this._txtProportionalSliceSpacing);
			this._grpSliceSpacing.Controls.Add(this._lblProportionalSliceSpacing);
			this._grpSliceSpacing.Controls.Add(this._radAutomaticSliceSpacing);
			this._grpSliceSpacing.Controls.Add(this._radProportionalSliceSpacing);
			resources.ApplyResources(this._grpSliceSpacing, "_grpSliceSpacing");
			this._grpSliceSpacing.Name = "_grpSliceSpacing";
			this._grpSliceSpacing.TabStop = false;
			// 
			// _txtProportionalSliceSpacing
			// 
			resources.ApplyResources(this._txtProportionalSliceSpacing, "_txtProportionalSliceSpacing");
			this._txtProportionalSliceSpacing.Name = "_txtProportionalSliceSpacing";
			// 
			// _lblProportionalSliceSpacing
			// 
			resources.ApplyResources(this._lblProportionalSliceSpacing, "_lblProportionalSliceSpacing");
			this._lblProportionalSliceSpacing.Name = "_lblProportionalSliceSpacing";
			// 
			// _radAutomaticSliceSpacing
			// 
			resources.ApplyResources(this._radAutomaticSliceSpacing, "_radAutomaticSliceSpacing");
			this._radAutomaticSliceSpacing.Name = "_radAutomaticSliceSpacing";
			this._radAutomaticSliceSpacing.TabStop = true;
			this._radAutomaticSliceSpacing.UseVisualStyleBackColor = true;
			// 
			// _radProportionalSliceSpacing
			// 
			resources.ApplyResources(this._radProportionalSliceSpacing, "_radProportionalSliceSpacing");
			this._radProportionalSliceSpacing.Name = "_radProportionalSliceSpacing";
			this._radProportionalSliceSpacing.TabStop = true;
			this._radProportionalSliceSpacing.UseVisualStyleBackColor = true;
			// 
			// MprConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._grpSliceSpacing);
			this.Name = "MprConfigurationComponentControl";
			this._grpSliceSpacing.ResumeLayout(false);
			this._grpSliceSpacing.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox _grpSliceSpacing;
		private System.Windows.Forms.RadioButton _radProportionalSliceSpacing;
		private System.Windows.Forms.RadioButton _radAutomaticSliceSpacing;
		private System.Windows.Forms.TextBox _txtProportionalSliceSpacing;
		private System.Windows.Forms.Label _lblProportionalSliceSpacing;
	}
}
