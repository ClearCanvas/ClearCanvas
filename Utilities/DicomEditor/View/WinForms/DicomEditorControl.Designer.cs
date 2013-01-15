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

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    partial class DicomEditorControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DicomEditorControl));
			this._dicomTagTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._dicomEditorTitleBar = new ClearCanvas.Desktop.View.WinForms.TitleBar();
			this.SuspendLayout();
			// 
			// _dicomTagTable
			// 
			this._dicomTagTable.ColumnHeaderTooltip = null;
			resources.ApplyResources(this._dicomTagTable, "_dicomTagTable");
			this._dicomTagTable.Name = "_dicomTagTable";
			this._dicomTagTable.ReadOnly = false;
			this._dicomTagTable.SortButtonTooltip = null;
			// 
			// _dicomEditorTitleBar
			// 
			resources.ApplyResources(this._dicomEditorTitleBar, "_dicomEditorTitleBar");
			this._dicomEditorTitleBar.Name = "_dicomEditorTitleBar";
			// 
			// DicomEditorControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._dicomTagTable);
			this.Controls.Add(this._dicomEditorTitleBar);
			this.Name = "DicomEditorControl";
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _dicomTagTable;
        private ClearCanvas.Desktop.View.WinForms.TitleBar _dicomEditorTitleBar;
    }
}
