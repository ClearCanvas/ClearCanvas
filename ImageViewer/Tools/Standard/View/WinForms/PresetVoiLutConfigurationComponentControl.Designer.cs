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

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    partial class PresetVoiLutConfigurationComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PresetVoiLutConfigurationComponentControl));
			this._presetVoiLuts = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._comboModality = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._comboAddPreset = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._addPresetButton = new System.Windows.Forms.Button();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _presetVoiLuts
			// 
			this._presetVoiLuts.ColumnHeaderTooltip = null;
			this._tableLayoutPanel.SetColumnSpan(this._presetVoiLuts, 2);
			resources.ApplyResources(this._presetVoiLuts, "_presetVoiLuts");
			this._presetVoiLuts.MultiSelect = false;
			this._presetVoiLuts.Name = "_presetVoiLuts";
			this._presetVoiLuts.ReadOnly = false;
			this._presetVoiLuts.SortButtonTooltip = null;
			// 
			// _comboModality
			// 
			this._comboModality.DataSource = null;
			this._comboModality.DisplayMember = "";
			resources.ApplyResources(this._comboModality, "_comboModality");
			this._comboModality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboModality.Name = "_comboModality";
			this._comboModality.Value = null;
			// 
			// _tableLayoutPanel
			// 
			resources.ApplyResources(this._tableLayoutPanel, "_tableLayoutPanel");
			this._tableLayoutPanel.Controls.Add(this._comboModality, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._presetVoiLuts, 0, 1);
			this._tableLayoutPanel.Controls.Add(this._comboAddPreset, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._addPresetButton, 1, 2);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			// 
			// _comboAddPreset
			// 
			this._comboAddPreset.DataSource = null;
			this._comboAddPreset.DisplayMember = "";
			resources.ApplyResources(this._comboAddPreset, "_comboAddPreset");
			this._comboAddPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboAddPreset.Name = "_comboAddPreset";
			this._comboAddPreset.Value = null;
			// 
			// _addPresetButton
			// 
			resources.ApplyResources(this._addPresetButton, "_addPresetButton");
			this._addPresetButton.Name = "_addPresetButton";
			this._addPresetButton.UseVisualStyleBackColor = true;
			// 
			// PresetVoiLutConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "PresetVoiLutConfigurationComponentControl";
			this._tableLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _presetVoiLuts;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _comboModality;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _comboAddPreset;
		private System.Windows.Forms.Button _addPresetButton;
    }
}
