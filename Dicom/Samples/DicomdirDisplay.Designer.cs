namespace ClearCanvas.Dicom.Samples
{
	partial class DicomdirDisplay
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
			this._treeViewDicomdir = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// _treeViewDicomdir
			// 
			this._treeViewDicomdir.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._treeViewDicomdir.Location = new System.Drawing.Point(12, 12);
			this._treeViewDicomdir.Name = "_treeViewDicomdir";
			this._treeViewDicomdir.Size = new System.Drawing.Size(701, 538);
			this._treeViewDicomdir.TabIndex = 0;
			// 
			// DicomdirDisplay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(725, 562);
			this.Controls.Add(this._treeViewDicomdir);
			this.Name = "DicomdirDisplay";
			this.Text = "DicomdirDisplay";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView _treeViewDicomdir;
	}
}