namespace ClearCanvas.Ris.Client.View.WinForms
{
	partial class DescriptiveSpinControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DescriptiveSpinControl));
			this._textBox = new System.Windows.Forms.TextBox();
			this._numericUpDown = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this._numericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// _textBox
			// 
			resources.ApplyResources(this._textBox, "_textBox");
			this._textBox.Name = "_textBox";
			this._textBox.ReadOnly = true;
			// 
			// _numericUpDown
			// 
			resources.ApplyResources(this._numericUpDown, "_numericUpDown");
			this._numericUpDown.Name = "_numericUpDown";
			// 
			// DescriptiveSpinControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._numericUpDown);
			this.Controls.Add(this._textBox);
			this.Name = "DescriptiveSpinControl";
			((System.ComponentModel.ISupportInitialize)(this._numericUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _textBox;
		private System.Windows.Forms.NumericUpDown _numericUpDown;
	}
}
