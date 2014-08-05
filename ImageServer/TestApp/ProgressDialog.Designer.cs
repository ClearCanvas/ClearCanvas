namespace ClearCanvas.ImageServer.TestApp
{
	partial class ProgressDialog
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
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._message = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _progressBar
			// 
			this._progressBar.Location = new System.Drawing.Point(27, 47);
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(453, 19);
			this._progressBar.TabIndex = 0;
			// 
			// _message
			// 
			this._message.AutoSize = true;
			this._message.Location = new System.Drawing.Point(27, 31);
			this._message.Name = "_message";
			this._message.Size = new System.Drawing.Size(35, 13);
			this._message.TabIndex = 1;
			this._message.Text = "label1";
			// 
			// ProgressDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(492, 93);
			this.Controls.Add(this._message);
			this.Controls.Add(this._progressBar);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "ProgressDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar _progressBar;
		private System.Windows.Forms.Label _message;
	}
}