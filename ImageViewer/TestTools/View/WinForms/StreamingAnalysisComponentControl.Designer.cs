#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.ImageViewer.TestTools.View.WinForms
{
    partial class StreamingAnalysisComponentControl
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
            if (disposing)
            {
				if (components != null)
					components.Dispose();

				if (_timer != null)
				{
					_timer.Dispose();
					_timer = null;
				}
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
			this._retrieveConcurrency = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._retrieveActive = new System.Windows.Forms.CheckBox();
			this._retrieveGroup = new System.Windows.Forms.GroupBox();
			this._retrievePriority = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this._plotAverage = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._refreshPlot = new System.Windows.Forms.Button();
			this._retrieveSpeedPlot = new NPlot.Windows.PlotSurface2D();
			this._clearRetrieve = new System.Windows.Forms.Button();
			this._retrieveItems = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.label1 = new System.Windows.Forms.Label();
			this._decompressGroup = new System.Windows.Forms.GroupBox();
			this._decompressPriority = new System.Windows.Forms.ComboBox();
			this._clearDecompress = new System.Windows.Forms.Button();
			this._decompressItems = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.label2 = new System.Windows.Forms.Label();
			this._decompressConcurrency = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._decompressActive = new System.Windows.Forms.CheckBox();
			this._addSelectedStudies = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this._retrieveConcurrency)).BeginInit();
			this._retrieveGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._plotAverage)).BeginInit();
			this._decompressGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._decompressConcurrency)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _retrieveConcurrency
			// 
			this._retrieveConcurrency.Location = new System.Drawing.Point(87, 39);
			this._retrieveConcurrency.Name = "_retrieveConcurrency";
			this._retrieveConcurrency.Size = new System.Drawing.Size(53, 20);
			this._retrieveConcurrency.TabIndex = 2;
			// 
			// _retrieveActive
			// 
			this._retrieveActive.AutoCheck = false;
			this._retrieveActive.AutoSize = true;
			this._retrieveActive.Location = new System.Drawing.Point(6, 40);
			this._retrieveActive.Name = "_retrieveActive";
			this._retrieveActive.Size = new System.Drawing.Size(56, 17);
			this._retrieveActive.TabIndex = 0;
			this._retrieveActive.Text = "Active";
			this._retrieveActive.UseVisualStyleBackColor = true;
			// 
			// _retrieveGroup
			// 
			this._retrieveGroup.Controls.Add(this._retrievePriority);
			this._retrieveGroup.Controls.Add(this.label3);
			this._retrieveGroup.Controls.Add(this._plotAverage);
			this._retrieveGroup.Controls.Add(this._refreshPlot);
			this._retrieveGroup.Controls.Add(this._retrieveSpeedPlot);
			this._retrieveGroup.Controls.Add(this._clearRetrieve);
			this._retrieveGroup.Controls.Add(this._retrieveItems);
			this._retrieveGroup.Controls.Add(this.label1);
			this._retrieveGroup.Controls.Add(this._retrieveConcurrency);
			this._retrieveGroup.Controls.Add(this._retrieveActive);
			this._retrieveGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this._retrieveGroup.Location = new System.Drawing.Point(0, 0);
			this._retrieveGroup.Name = "_retrieveGroup";
			this._retrieveGroup.Size = new System.Drawing.Size(488, 237);
			this._retrieveGroup.TabIndex = 3;
			this._retrieveGroup.TabStop = false;
			this._retrieveGroup.Text = "Retrieve";
			// 
			// _retrievePriority
			// 
			this._retrievePriority.FormattingEnabled = true;
			this._retrievePriority.Location = new System.Drawing.Point(19, 65);
			this._retrievePriority.Name = "_retrievePriority";
			this._retrievePriority.Size = new System.Drawing.Size(121, 21);
			this._retrievePriority.TabIndex = 11;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(166, 69);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(79, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Average every:";
			// 
			// _plotAverage
			// 
			this._plotAverage.Location = new System.Drawing.Point(249, 67);
			this._plotAverage.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this._plotAverage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._plotAverage.Name = "_plotAverage";
			this._plotAverage.Size = new System.Drawing.Size(53, 20);
			this._plotAverage.TabIndex = 9;
			this._plotAverage.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// _refreshPlot
			// 
			this._refreshPlot.Location = new System.Drawing.Point(308, 64);
			this._refreshPlot.Name = "_refreshPlot";
			this._refreshPlot.Size = new System.Drawing.Size(53, 23);
			this._refreshPlot.TabIndex = 5;
			this._refreshPlot.Text = "Refresh";
			this._refreshPlot.UseVisualStyleBackColor = true;
			this._refreshPlot.Click += new System.EventHandler(this._refreshPlot_Click);
			// 
			// _retrieveSpeedPlot
			// 
			this._retrieveSpeedPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._retrieveSpeedPlot.AutoScaleAutoGeneratedAxes = false;
			this._retrieveSpeedPlot.AutoScaleTitle = false;
			this._retrieveSpeedPlot.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this._retrieveSpeedPlot.DateTimeToolTip = false;
			this._retrieveSpeedPlot.Legend = null;
			this._retrieveSpeedPlot.LegendZOrder = -1;
			this._retrieveSpeedPlot.Location = new System.Drawing.Point(6, 93);
			this._retrieveSpeedPlot.Name = "_retrieveSpeedPlot";
			this._retrieveSpeedPlot.RightMenu = null;
			this._retrieveSpeedPlot.ShowCoordinates = true;
			this._retrieveSpeedPlot.Size = new System.Drawing.Size(476, 138);
			this._retrieveSpeedPlot.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
			this._retrieveSpeedPlot.TabIndex = 6;
			this._retrieveSpeedPlot.Text = "plotSurface2D1";
			this._retrieveSpeedPlot.Title = "";
			this._retrieveSpeedPlot.TitleFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this._retrieveSpeedPlot.XAxis1 = null;
			this._retrieveSpeedPlot.XAxis2 = null;
			this._retrieveSpeedPlot.YAxis1 = null;
			this._retrieveSpeedPlot.YAxis2 = null;
			// 
			// _clearRetrieve
			// 
			this._clearRetrieve.Location = new System.Drawing.Point(214, 37);
			this._clearRetrieve.Name = "_clearRetrieve";
			this._clearRetrieve.Size = new System.Drawing.Size(53, 23);
			this._clearRetrieve.TabIndex = 4;
			this._clearRetrieve.Text = "Clear";
			this._clearRetrieve.UseVisualStyleBackColor = true;
			this._clearRetrieve.Click += new System.EventHandler(this._clearRetrieve_Click);
			// 
			// _retrieveItems
			// 
			this._retrieveItems.LabelText = "#Items";
			this._retrieveItems.Location = new System.Drawing.Point(157, 21);
			this._retrieveItems.Margin = new System.Windows.Forms.Padding(2);
			this._retrieveItems.Mask = "";
			this._retrieveItems.Name = "_retrieveItems";
			this._retrieveItems.PasswordChar = '\0';
			this._retrieveItems.ReadOnly = true;
			this._retrieveItems.Size = new System.Drawing.Size(53, 41);
			this._retrieveItems.TabIndex = 8;
			this._retrieveItems.ToolTip = null;
			this._retrieveItems.Value = null;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(84, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Concurrency";
			// 
			// _decompressGroup
			// 
			this._decompressGroup.Controls.Add(this._decompressPriority);
			this._decompressGroup.Controls.Add(this._clearDecompress);
			this._decompressGroup.Controls.Add(this._decompressItems);
			this._decompressGroup.Controls.Add(this.label2);
			this._decompressGroup.Controls.Add(this._decompressConcurrency);
			this._decompressGroup.Controls.Add(this._decompressActive);
			this._decompressGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this._decompressGroup.Location = new System.Drawing.Point(0, 0);
			this._decompressGroup.Name = "_decompressGroup";
			this._decompressGroup.Size = new System.Drawing.Size(281, 237);
			this._decompressGroup.TabIndex = 5;
			this._decompressGroup.TabStop = false;
			this._decompressGroup.Text = "Decompress";
			// 
			// _decompressPriority
			// 
			this._decompressPriority.FormattingEnabled = true;
			this._decompressPriority.Location = new System.Drawing.Point(6, 66);
			this._decompressPriority.Name = "_decompressPriority";
			this._decompressPriority.Size = new System.Drawing.Size(121, 21);
			this._decompressPriority.TabIndex = 15;
			// 
			// _clearDecompress
			// 
			this._clearDecompress.Location = new System.Drawing.Point(212, 35);
			this._clearDecompress.Name = "_clearDecompress";
			this._clearDecompress.Size = new System.Drawing.Size(53, 23);
			this._clearDecompress.TabIndex = 12;
			this._clearDecompress.Text = "Clear";
			this._clearDecompress.UseVisualStyleBackColor = true;
			this._clearDecompress.Click += new System.EventHandler(this._clearDecompress_Click);
			// 
			// _decompressItems
			// 
			this._decompressItems.LabelText = "#Items";
			this._decompressItems.Location = new System.Drawing.Point(154, 19);
			this._decompressItems.Margin = new System.Windows.Forms.Padding(2);
			this._decompressItems.Mask = "";
			this._decompressItems.Name = "_decompressItems";
			this._decompressItems.PasswordChar = '\0';
			this._decompressItems.ReadOnly = true;
			this._decompressItems.Size = new System.Drawing.Size(53, 41);
			this._decompressItems.TabIndex = 14;
			this._decompressItems.ToolTip = null;
			this._decompressItems.Value = null;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(65, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 13);
			this.label2.TabIndex = 13;
			this.label2.Text = "Concurrency";
			// 
			// _decompressConcurrency
			// 
			this._decompressConcurrency.Location = new System.Drawing.Point(68, 37);
			this._decompressConcurrency.Name = "_decompressConcurrency";
			this._decompressConcurrency.Size = new System.Drawing.Size(53, 20);
			this._decompressConcurrency.TabIndex = 10;
			// 
			// _decompressActive
			// 
			this._decompressActive.AutoCheck = false;
			this._decompressActive.AutoSize = true;
			this._decompressActive.Location = new System.Drawing.Point(6, 34);
			this._decompressActive.Name = "_decompressActive";
			this._decompressActive.Size = new System.Drawing.Size(56, 17);
			this._decompressActive.TabIndex = 9;
			this._decompressActive.Text = "Active";
			this._decompressActive.UseVisualStyleBackColor = true;
			// 
			// _addSelectedStudies
			// 
			this._addSelectedStudies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._addSelectedStudies.Location = new System.Drawing.Point(710, 261);
			this._addSelectedStudies.Name = "_addSelectedStudies";
			this._addSelectedStudies.Size = new System.Drawing.Size(75, 23);
			this._addSelectedStudies.TabIndex = 17;
			this._addSelectedStudies.Text = "Add Studies";
			this._addSelectedStudies.UseVisualStyleBackColor = true;
			this._addSelectedStudies.Click += new System.EventHandler(this._addSelectedStudies_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(12, 18);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._retrieveGroup);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._decompressGroup);
			this.splitContainer1.Size = new System.Drawing.Size(773, 237);
			this.splitContainer1.SplitterDistance = 488;
			this.splitContainer1.TabIndex = 16;
			// 
			// StreamingAnalysisComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this._addSelectedStudies);
			this.Name = "StreamingAnalysisComponentControl";
			this.Size = new System.Drawing.Size(798, 294);
			((System.ComponentModel.ISupportInitialize)(this._retrieveConcurrency)).EndInit();
			this._retrieveGroup.ResumeLayout(false);
			this._retrieveGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._plotAverage)).EndInit();
			this._decompressGroup.ResumeLayout(false);
			this._decompressGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._decompressConcurrency)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _retrieveConcurrency;
		private System.Windows.Forms.CheckBox _retrieveActive;
		private System.Windows.Forms.GroupBox _retrieveGroup;
		private System.Windows.Forms.Label label1;
		private ClearCanvas.Desktop.View.WinForms.TextField _retrieveItems;
		private System.Windows.Forms.GroupBox _decompressGroup;
		private ClearCanvas.Desktop.View.WinForms.TextField _decompressItems;
		private System.Windows.Forms.Label label2;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _decompressConcurrency;
		private System.Windows.Forms.CheckBox _decompressActive;
		private System.Windows.Forms.Button _addSelectedStudies;
		private System.Windows.Forms.Button _clearRetrieve;
		private System.Windows.Forms.Button _clearDecompress;
		private NPlot.Windows.PlotSurface2D _retrieveSpeedPlot;
		private System.Windows.Forms.Button _refreshPlot;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Label label3;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _plotAverage;
		private System.Windows.Forms.ComboBox _retrievePriority;
		private System.Windows.Forms.ComboBox _decompressPriority;
    }
}
