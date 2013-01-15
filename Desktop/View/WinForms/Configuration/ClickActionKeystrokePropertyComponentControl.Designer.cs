namespace ClearCanvas.Desktop.View.WinForms.Configuration {
	partial class ClickActionKeystrokePropertyComponentControl {
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
			System.Windows.Forms.GroupBox groupBox1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClickActionKeystrokePropertyComponentControl));
			this.label1 = new System.Windows.Forms.Label();
			this._keyStrokeCaptureBox = new ClearCanvas.Desktop.View.WinForms.KeyStrokeCaptureBox();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(this.label1);
			groupBox1.Controls.Add(this._keyStrokeCaptureBox);
			resources.ApplyResources(groupBox1, "groupBox1");
			groupBox1.Name = "groupBox1";
			groupBox1.TabStop = false;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _keyStrokeCaptureBox
			// 
			resources.ApplyResources(this._keyStrokeCaptureBox, "_keyStrokeCaptureBox");
			this._keyStrokeCaptureBox.Name = "_keyStrokeCaptureBox";
			this._keyStrokeCaptureBox.ShowClearButton = true;
			this._keyStrokeCaptureBox.ValidateKeyStroke += new ClearCanvas.Desktop.View.WinForms.ValidateKeyStrokeEventHandler(this._keyStrokeCaptureBox_ValidateKeyStroke);
			// 
			// ClickActionKeystrokePropertyComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(groupBox1);
			this.Name = "ClickActionKeystrokePropertyComponentControl";
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.KeyStrokeCaptureBox _keyStrokeCaptureBox;
		private System.Windows.Forms.Label label1;

	}
}
