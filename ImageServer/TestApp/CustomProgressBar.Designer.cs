namespace ClearCanvas.ImageServer.TestApp
{
	partial class CustomProgressBar
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
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._message = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _progressBar
			// 
			this._progressBar.Location = new System.Drawing.Point(22, 42);
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(274, 23);
			this._progressBar.TabIndex = 0;
			// 
			// _message
			// 
			this._message.AutoSize = true;
			this._message.Location = new System.Drawing.Point(22, 23);
			this._message.Name = "_message";
			this._message.Size = new System.Drawing.Size(35, 13);
			this._message.TabIndex = 1;
			this._message.Text = "label1";
			// 
			// CustomProgressBar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._message);
			this.Controls.Add(this._progressBar);
			this.Name = "CustomProgressBar";
			this.Size = new System.Drawing.Size(317, 81);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar _progressBar;
		private System.Windows.Forms.Label _message;
	}
}
