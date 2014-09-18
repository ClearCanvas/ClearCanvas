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
    partial class WorklistFilterEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorklistFilterEditorComponentControl));
			this._portable = new ClearCanvas.Desktop.View.WinForms.DropListPickerField();
			this._priority = new ClearCanvas.Desktop.View.WinForms.DropListPickerField();
			this._patientClass = new ClearCanvas.Desktop.View.WinForms.DropListPickerField();
			this._facilities = new ClearCanvas.Desktop.View.WinForms.DropListPickerField();
			this._orderingPractitioner = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this.SuspendLayout();
			// 
			// _portable
			// 
			resources.ApplyResources(this._portable, "_portable");
			this._portable.Name = "_portable";
			// 
			// _priority
			// 
			resources.ApplyResources(this._priority, "_priority");
			this._priority.Name = "_priority";
			// 
			// _patientClass
			// 
			resources.ApplyResources(this._patientClass, "_patientClass");
			this._patientClass.Name = "_patientClass";
			// 
			// _facilities
			// 
			resources.ApplyResources(this._facilities, "_facilities");
			this._facilities.Name = "_facilities";
			// 
			// _orderingPractitioner
			// 
			resources.ApplyResources(this._orderingPractitioner, "_orderingPractitioner");
			this._orderingPractitioner.Name = "_orderingPractitioner";
			this._orderingPractitioner.Value = null;
			// 
			// WorklistFilterEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._orderingPractitioner);
			this.Controls.Add(this._portable);
			this.Controls.Add(this._patientClass);
			this.Controls.Add(this._priority);
			this.Controls.Add(this._facilities);
			this.Name = "WorklistFilterEditorComponentControl";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.DropListPickerField _portable;
        private ClearCanvas.Desktop.View.WinForms.DropListPickerField _priority;
        private ClearCanvas.Desktop.View.WinForms.DropListPickerField _patientClass;
		private ClearCanvas.Desktop.View.WinForms.DropListPickerField _facilities;
		private ClearCanvas.Ris.Client.View.WinForms.LookupField _orderingPractitioner;
    }
}
