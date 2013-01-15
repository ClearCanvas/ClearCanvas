namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	partial class MonitorConfigurationApplicationComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorConfigurationApplicationComponentControl));
			this._singleWindowRadio = new System.Windows.Forms.RadioButton();
			this._separateWindowRadio = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _singleWindowRadio
			// 
			resources.ApplyResources(this._singleWindowRadio, "_singleWindowRadio");
			this._singleWindowRadio.Name = "_singleWindowRadio";
			this._singleWindowRadio.TabStop = true;
			this._singleWindowRadio.UseVisualStyleBackColor = true;
			// 
			// _separateWindowRadio
			// 
			resources.ApplyResources(this._separateWindowRadio, "_separateWindowRadio");
			this._separateWindowRadio.Name = "_separateWindowRadio";
			this._separateWindowRadio.TabStop = true;
			this._separateWindowRadio.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._singleWindowRadio);
			this.groupBox1.Controls.Add(this._separateWindowRadio);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// MonitorConfigurationApplicationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "MonitorConfigurationApplicationComponentControl";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RadioButton _singleWindowRadio;
		private System.Windows.Forms.RadioButton _separateWindowRadio;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}