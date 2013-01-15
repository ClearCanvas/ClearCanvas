namespace ClearCanvas.ImageViewer.TestTools.Rendering.TestApp
{
	partial class Form1
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
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this.renderingSurfaceContainer1 = new RenderingSurfaceContainer();
			this.renderingSurfaceContainer2 = new RenderingSurfaceContainer();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.Panel2.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// _splitContainer
			// 
			this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainer.Location = new System.Drawing.Point(0, 0);
			this._splitContainer.Name = "_splitContainer";
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Controls.Add(this.renderingSurfaceContainer1);
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.Controls.Add(this.renderingSurfaceContainer2);
			this._splitContainer.Size = new System.Drawing.Size(743, 393);
			this._splitContainer.SplitterDistance = 371;
			this._splitContainer.TabIndex = 1;
			// 
			// renderingSurfaceContainer1
			// 
			this.renderingSurfaceContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.renderingSurfaceContainer1.Location = new System.Drawing.Point(0, 0);
			this.renderingSurfaceContainer1.Name = "renderingSurfaceContainer1";
			this.renderingSurfaceContainer1.Size = new System.Drawing.Size(371, 393);
			this.renderingSurfaceContainer1.TabIndex = 0;
			// 
			// renderingSurfaceContainer2
			// 
			this.renderingSurfaceContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.renderingSurfaceContainer2.Location = new System.Drawing.Point(0, 0);
			this.renderingSurfaceContainer2.Name = "renderingSurfaceContainer2";
			this.renderingSurfaceContainer2.Size = new System.Drawing.Size(368, 393);
			this.renderingSurfaceContainer2.TabIndex = 0;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(743, 393);
			this.Controls.Add(this._splitContainer);
			this.Name = "Form1";
			this.Text = "Form1";
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.Panel2.ResumeLayout(false);
			this._splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer _splitContainer;
		private RenderingSurfaceContainer renderingSurfaceContainer1;
		private RenderingSurfaceContainer renderingSurfaceContainer2;

	}
}

