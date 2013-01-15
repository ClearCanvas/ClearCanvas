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

namespace ClearCanvas.Ris.Client.View.WinForms
{
	partial class BiographyVisitHistoryComponentControl
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
			this._visitList = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._detailPanel = new System.Windows.Forms.Panel();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _visitList
			// 
			this._visitList.Dock = System.Windows.Forms.DockStyle.Fill;
			this._visitList.Location = new System.Drawing.Point(0, 0);
			this._visitList.MultiSelect = false;
			this._visitList.Name = "_visitList";
			this._visitList.ReadOnly = false;
			this._visitList.ShowToolbar = false;
			this._visitList.Size = new System.Drawing.Size(461, 577);
			this._visitList.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(4, 2);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._visitList);
			this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.Controls.Add(this._detailPanel);
			this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.splitContainer1.Size = new System.Drawing.Size(976, 577);
			this.splitContainer1.SplitterDistance = 465;
			this.splitContainer1.TabIndex = 1;
			// 
			// _detailPanel
			// 
			this._detailPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this._detailPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._detailPanel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this._detailPanel.Location = new System.Drawing.Point(3, 0);
			this._detailPanel.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
			this._detailPanel.Name = "_detailPanel";
			this._detailPanel.Padding = new System.Windows.Forms.Padding(1);
			this._detailPanel.Size = new System.Drawing.Size(504, 577);
			this._detailPanel.TabIndex = 0;
			// 
			// BiographyVisitHistoryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.splitContainer1);
			this.Name = "BiographyVisitHistoryComponentControl";
			this.Padding = new System.Windows.Forms.Padding(4, 2, 2, 7);
			this.Size = new System.Drawing.Size(982, 586);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _visitList;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Panel _detailPanel;
	}
}
