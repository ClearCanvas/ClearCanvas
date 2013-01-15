namespace ClearCanvas.ImageViewer.TestTools.Rendering.TestApp
{
	partial class RenderingSurfaceContainer
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
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this._renderingSurface = new ClearCanvas.ImageViewer.TestTools.Rendering.TestApp.RenderingSurface();
			this._draw50 = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._draws = new System.Windows.Forms.TextBox();
			this._time = new System.Windows.Forms.TextBox();
			this._clearStats = new System.Windows.Forms.Button();
			this._clearBitmap = new System.Windows.Forms.Button();
			this._useBitmap = new System.Windows.Forms.Button();
			this._useBufferedGraphics = new System.Windows.Forms.CheckBox();
			this._customBackBuffer = new System.Windows.Forms.CheckBox();
			this._comboSource = new System.Windows.Forms.ComboBox();
			this._comboFormat = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.Panel2.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// _splitContainer
			// 
			this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this._splitContainer.Location = new System.Drawing.Point(0, 0);
			this._splitContainer.Name = "_splitContainer";
			this._splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Controls.Add(this._renderingSurface);
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.Controls.Add(this.button1);
			this._splitContainer.Panel2.Controls.Add(this._draw50);
			this._splitContainer.Panel2.Controls.Add(this.label4);
			this._splitContainer.Panel2.Controls.Add(this.label3);
			this._splitContainer.Panel2.Controls.Add(this._draws);
			this._splitContainer.Panel2.Controls.Add(this._time);
			this._splitContainer.Panel2.Controls.Add(this._clearStats);
			this._splitContainer.Panel2.Controls.Add(this._clearBitmap);
			this._splitContainer.Panel2.Controls.Add(this._useBitmap);
			this._splitContainer.Panel2.Controls.Add(this._useBufferedGraphics);
			this._splitContainer.Panel2.Controls.Add(this._customBackBuffer);
			this._splitContainer.Panel2.Controls.Add(this._comboSource);
			this._splitContainer.Panel2.Controls.Add(this._comboFormat);
			this._splitContainer.Panel2.Controls.Add(this.label2);
			this._splitContainer.Panel2.Controls.Add(this.label1);
			this._splitContainer.Size = new System.Drawing.Size(336, 398);
			this._splitContainer.SplitterDistance = 242;
			this._splitContainer.TabIndex = 0;
			// 
			// _renderingSurface
			// 
			this._renderingSurface.Bitmap = null;
			this._renderingSurface.CustomBackBuffer = true;
			this._renderingSurface.Dock = System.Windows.Forms.DockStyle.Fill;
			this._renderingSurface.Format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
			this._renderingSurface.Location = new System.Drawing.Point(0, 0);
			this._renderingSurface.Name = "_renderingSurface";
			this._renderingSurface.Size = new System.Drawing.Size(336, 242);
			this._renderingSurface.Source = ClearCanvas.ImageViewer.TestTools.Rendering.TestApp.GraphicsSource.Default;
			this._renderingSurface.TabIndex = 0;
			this._renderingSurface.UseBufferedGraphics = false;
			// 
			// _draw50
			// 
			this._draw50.Location = new System.Drawing.Point(251, 116);
			this._draw50.Name = "_draw50";
			this._draw50.Size = new System.Drawing.Size(60, 20);
			this._draw50.TabIndex = 13;
			this._draw50.Text = "Draw x50";
			this._draw50.UseVisualStyleBackColor = true;
			this._draw50.Click += new System.EventHandler(this._draw50_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(99, 120);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(30, 13);
			this.label4.TabIndex = 12;
			this.label4.Text = "Time";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 13);
			this.label3.TabIndex = 11;
			this.label3.Text = "Draws";
			// 
			// _draws
			// 
			this._draws.Location = new System.Drawing.Point(50, 117);
			this._draws.Name = "_draws";
			this._draws.ReadOnly = true;
			this._draws.Size = new System.Drawing.Size(41, 20);
			this._draws.TabIndex = 10;
			// 
			// _time
			// 
			this._time.Location = new System.Drawing.Point(135, 117);
			this._time.Name = "_time";
			this._time.ReadOnly = true;
			this._time.Size = new System.Drawing.Size(41, 20);
			this._time.TabIndex = 9;
			// 
			// _clearStats
			// 
			this._clearStats.Location = new System.Drawing.Point(191, 116);
			this._clearStats.Name = "_clearStats";
			this._clearStats.Size = new System.Drawing.Size(45, 20);
			this._clearStats.TabIndex = 8;
			this._clearStats.Text = "Clear";
			this._clearStats.UseVisualStyleBackColor = true;
			this._clearStats.Click += new System.EventHandler(this._clearStats_Click);
			// 
			// _clearBitmap
			// 
			this._clearBitmap.Location = new System.Drawing.Point(102, 80);
			this._clearBitmap.Name = "_clearBitmap";
			this._clearBitmap.Size = new System.Drawing.Size(74, 20);
			this._clearBitmap.TabIndex = 7;
			this._clearBitmap.Text = "Clear Bitmap";
			this._clearBitmap.UseVisualStyleBackColor = true;
			this._clearBitmap.Click += new System.EventHandler(this._clearBitmap_Click);
			// 
			// _useBitmap
			// 
			this._useBitmap.Location = new System.Drawing.Point(10, 80);
			this._useBitmap.Name = "_useBitmap";
			this._useBitmap.Size = new System.Drawing.Size(74, 20);
			this._useBitmap.TabIndex = 6;
			this._useBitmap.Text = "Use Bitmap";
			this._useBitmap.UseVisualStyleBackColor = true;
			this._useBitmap.Click += new System.EventHandler(this._useBitmap_Click);
			// 
			// _useBufferedGraphics
			// 
			this._useBufferedGraphics.AutoSize = true;
			this._useBufferedGraphics.Location = new System.Drawing.Point(191, 54);
			this._useBufferedGraphics.Name = "_useBufferedGraphics";
			this._useBufferedGraphics.Size = new System.Drawing.Size(130, 17);
			this._useBufferedGraphics.TabIndex = 5;
			this._useBufferedGraphics.Text = "Use BufferedGraphics";
			this._useBufferedGraphics.UseVisualStyleBackColor = true;
			// 
			// _customBackBuffer
			// 
			this._customBackBuffer.AutoSize = true;
			this._customBackBuffer.Location = new System.Drawing.Point(191, 20);
			this._customBackBuffer.Name = "_customBackBuffer";
			this._customBackBuffer.Size = new System.Drawing.Size(120, 17);
			this._customBackBuffer.TabIndex = 4;
			this._customBackBuffer.Text = "Custom Back Buffer";
			this._customBackBuffer.UseVisualStyleBackColor = true;
			// 
			// _comboSource
			// 
			this._comboSource.FormattingEnabled = true;
			this._comboSource.Location = new System.Drawing.Point(55, 53);
			this._comboSource.Name = "_comboSource";
			this._comboSource.Size = new System.Drawing.Size(121, 21);
			this._comboSource.TabIndex = 3;
			// 
			// _comboFormat
			// 
			this._comboFormat.FormattingEnabled = true;
			this._comboFormat.Location = new System.Drawing.Point(55, 18);
			this._comboFormat.Name = "_comboFormat";
			this._comboFormat.Size = new System.Drawing.Size(121, 21);
			this._comboFormat.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Source:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Format:";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(251, 90);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(60, 20);
			this.button1.TabIndex = 14;
			this.button1.Text = "Show Stats";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// RenderingSurfaceContainer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._splitContainer);
			this.Name = "RenderingSurfaceContainer";
			this.Size = new System.Drawing.Size(336, 398);
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.Panel2.ResumeLayout(false);
			this._splitContainer.Panel2.PerformLayout();
			this._splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer _splitContainer;
		private RenderingSurface _renderingSurface;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox _comboFormat;
		private System.Windows.Forms.ComboBox _comboSource;
		private System.Windows.Forms.CheckBox _customBackBuffer;
		private System.Windows.Forms.CheckBox _useBufferedGraphics;
		private System.Windows.Forms.Button _clearBitmap;
		private System.Windows.Forms.Button _useBitmap;
		private System.Windows.Forms.TextBox _draws;
		private System.Windows.Forms.TextBox _time;
		private System.Windows.Forms.Button _clearStats;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button _draw50;
		private System.Windows.Forms.Button button1;

	}
}
