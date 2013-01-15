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
			this._textBox = new System.Windows.Forms.TextBox();
			this._numericUpDown = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this._numericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// _textBox
			// 
			this._textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._textBox.Location = new System.Drawing.Point(0, 0);
			this._textBox.Name = "_textBox";
			this._textBox.ReadOnly = true;
			this._textBox.Size = new System.Drawing.Size(100, 20);
			this._textBox.TabIndex = 0;
			// 
			// _numericUpDown
			// 
			this._numericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._numericUpDown.Location = new System.Drawing.Point(99, 0);
			this._numericUpDown.Name = "_numericUpDown";
			this._numericUpDown.Size = new System.Drawing.Size(18, 20);
			this._numericUpDown.TabIndex = 1;
			// 
			// DescriptiveSpinControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._numericUpDown);
			this.Controls.Add(this._textBox);
			this.Name = "DescriptiveSpinControl";
			this.Size = new System.Drawing.Size(119, 22);
			((System.ComponentModel.ISupportInitialize)(this._numericUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _textBox;
		private System.Windows.Forms.NumericUpDown _numericUpDown;
	}
}
