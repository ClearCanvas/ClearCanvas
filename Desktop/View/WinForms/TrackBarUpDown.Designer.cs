#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Desktop.View.WinForms
{
	partial class TrackBarUpDown
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrackBarUpDown));
			this._numericUpDown = new System.Windows.Forms.NumericUpDown();
			this._trackBar = new System.Windows.Forms.TrackBar();
			((System.ComponentModel.ISupportInitialize)(this._numericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._trackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// _numericUpDown
			// 
			resources.ApplyResources(this._numericUpDown, "_numericUpDown");
			this._numericUpDown.Name = "_numericUpDown";
			this._numericUpDown.Value = new decimal(new int[] {
            99,
            0,
            0,
            0});
			// 
			// _trackBar
			// 
			resources.ApplyResources(this._trackBar, "_trackBar");
			this._trackBar.Name = "_trackBar";
			// 
			// TrackBarUpDown
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._trackBar);
			this.Controls.Add(this._numericUpDown);
			this.Name = "TrackBarUpDown";
			((System.ComponentModel.ISupportInitialize)(this._numericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._trackBar)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NumericUpDown _numericUpDown;
		private System.Windows.Forms.TrackBar _trackBar;
	}
}
