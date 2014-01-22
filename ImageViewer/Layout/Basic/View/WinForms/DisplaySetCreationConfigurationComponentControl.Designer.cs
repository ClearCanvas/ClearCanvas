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

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    partial class DisplaySetCreationConfigurationComponentControl
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplaySetCreationConfigurationComponentControl));
			this._createSingleImageDisplaySets = new System.Windows.Forms.CheckBox();
			this._modality = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this._creationGroup = new System.Windows.Forms.GroupBox();
			this._showOriginalSeries = new System.Windows.Forms.CheckBox();
			this._createAllImagesDisplaySet = new System.Windows.Forms.CheckBox();
			this._presentationGroupBox = new System.Windows.Forms.GroupBox();
			this._invertImages = new System.Windows.Forms.CheckBox();
			this._tooltip = new System.Windows.Forms.ToolTip(this.components);
			this._splitMixedMultiframeSeries = new System.Windows.Forms.CheckBox();
			this._showOriginalMixedMultiframeSeries = new System.Windows.Forms.CheckBox();
			this._splitEchos = new System.Windows.Forms.CheckBox();
			this._showOriginalMultiEchoSeries = new System.Windows.Forms.CheckBox();
			this._selectedOverlaysGroupBox = new System.Windows.Forms.GroupBox();
			this._overlaysPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._splittingGroup = new System.Windows.Forms.GroupBox();
			this._showOriginalMultiStackSeries = new System.Windows.Forms.CheckBox();
			this._splitStacks = new System.Windows.Forms.CheckBox();
			this._creationGroup.SuspendLayout();
			this._presentationGroupBox.SuspendLayout();
			this._selectedOverlaysGroupBox.SuspendLayout();
			this._splittingGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// _createSingleImageDisplaySets
			// 
			resources.ApplyResources(this._createSingleImageDisplaySets, "_createSingleImageDisplaySets");
			this._createSingleImageDisplaySets.Name = "_createSingleImageDisplaySets";
			this._tooltip.SetToolTip(this._createSingleImageDisplaySets, resources.GetString("_createSingleImageDisplaySets.ToolTip"));
			this._createSingleImageDisplaySets.UseVisualStyleBackColor = true;
			// 
			// _modality
			// 
			this._modality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._modality.FormattingEnabled = true;
			resources.ApplyResources(this._modality, "_modality");
			this._modality.Name = "_modality";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _creationGroup
			// 
			this._creationGroup.Controls.Add(this._showOriginalSeries);
			this._creationGroup.Controls.Add(this._createAllImagesDisplaySet);
			this._creationGroup.Controls.Add(this._createSingleImageDisplaySets);
			resources.ApplyResources(this._creationGroup, "_creationGroup");
			this._creationGroup.Name = "_creationGroup";
			this._creationGroup.TabStop = false;
			// 
			// _showOriginalSeries
			// 
			resources.ApplyResources(this._showOriginalSeries, "_showOriginalSeries");
			this._showOriginalSeries.Name = "_showOriginalSeries";
			this._tooltip.SetToolTip(this._showOriginalSeries, resources.GetString("_showOriginalSeries.ToolTip"));
			this._showOriginalSeries.UseVisualStyleBackColor = true;
			// 
			// _createAllImagesDisplaySet
			// 
			resources.ApplyResources(this._createAllImagesDisplaySet, "_createAllImagesDisplaySet");
			this._createAllImagesDisplaySet.Name = "_createAllImagesDisplaySet";
			this._tooltip.SetToolTip(this._createAllImagesDisplaySet, resources.GetString("_createAllImagesDisplaySet.ToolTip"));
			this._createAllImagesDisplaySet.UseVisualStyleBackColor = true;
			// 
			// _presentationGroupBox
			// 
			this._presentationGroupBox.Controls.Add(this._invertImages);
			resources.ApplyResources(this._presentationGroupBox, "_presentationGroupBox");
			this._presentationGroupBox.Name = "_presentationGroupBox";
			this._presentationGroupBox.TabStop = false;
			// 
			// _invertImages
			// 
			resources.ApplyResources(this._invertImages, "_invertImages");
			this._invertImages.Name = "_invertImages";
			this._tooltip.SetToolTip(this._invertImages, resources.GetString("_invertImages.ToolTip"));
			this._invertImages.UseVisualStyleBackColor = true;
			// 
			// _splitMixedMultiframeSeries
			// 
			resources.ApplyResources(this._splitMixedMultiframeSeries, "_splitMixedMultiframeSeries");
			this._splitMixedMultiframeSeries.Name = "_splitMixedMultiframeSeries";
			this._tooltip.SetToolTip(this._splitMixedMultiframeSeries, resources.GetString("_splitMixedMultiframeSeries.ToolTip"));
			this._splitMixedMultiframeSeries.UseVisualStyleBackColor = true;
			// 
			// _showOriginalMixedMultiframeSeries
			// 
			resources.ApplyResources(this._showOriginalMixedMultiframeSeries, "_showOriginalMixedMultiframeSeries");
			this._showOriginalMixedMultiframeSeries.Name = "_showOriginalMixedMultiframeSeries";
			this._tooltip.SetToolTip(this._showOriginalMixedMultiframeSeries, resources.GetString("_showOriginalMixedMultiframeSeries.ToolTip"));
			this._showOriginalMixedMultiframeSeries.UseVisualStyleBackColor = true;
			// 
			// _splitEchos
			// 
			resources.ApplyResources(this._splitEchos, "_splitEchos");
			this._splitEchos.Name = "_splitEchos";
			this._tooltip.SetToolTip(this._splitEchos, resources.GetString("_splitEchos.ToolTip"));
			this._splitEchos.UseVisualStyleBackColor = true;
			// 
			// _showOriginalMultiEchoSeries
			// 
			resources.ApplyResources(this._showOriginalMultiEchoSeries, "_showOriginalMultiEchoSeries");
			this._showOriginalMultiEchoSeries.Name = "_showOriginalMultiEchoSeries";
			this._tooltip.SetToolTip(this._showOriginalMultiEchoSeries, resources.GetString("_showOriginalMultiEchoSeries.ToolTip"));
			this._showOriginalMultiEchoSeries.UseVisualStyleBackColor = true;
			// 
			// _selectedOverlaysGroupBox
			// 
			this._selectedOverlaysGroupBox.Controls.Add(this._overlaysPanel);
			resources.ApplyResources(this._selectedOverlaysGroupBox, "_selectedOverlaysGroupBox");
			this._selectedOverlaysGroupBox.Name = "_selectedOverlaysGroupBox";
			this._selectedOverlaysGroupBox.TabStop = false;
			this._tooltip.SetToolTip(this._selectedOverlaysGroupBox, resources.GetString("_selectedOverlaysGroupBox.ToolTip"));
			// 
			// _overlaysPanel
			// 
			resources.ApplyResources(this._overlaysPanel, "_overlaysPanel");
			this._overlaysPanel.Name = "_overlaysPanel";
			// 
			// _splittingGroup
			// 
			this._splittingGroup.Controls.Add(this._showOriginalMultiStackSeries);
			this._splittingGroup.Controls.Add(this._splitStacks);
			this._splittingGroup.Controls.Add(this._showOriginalMultiEchoSeries);
			this._splittingGroup.Controls.Add(this._splitEchos);
			this._splittingGroup.Controls.Add(this._showOriginalMixedMultiframeSeries);
			this._splittingGroup.Controls.Add(this._splitMixedMultiframeSeries);
			resources.ApplyResources(this._splittingGroup, "_splittingGroup");
			this._splittingGroup.Name = "_splittingGroup";
			this._splittingGroup.TabStop = false;
			// 
			// _showOriginalMultiStackSeries
			// 
			resources.ApplyResources(this._showOriginalMultiStackSeries, "_showOriginalMultiStackSeries");
			this._showOriginalMultiStackSeries.Name = "_showOriginalMultiStackSeries";
			this._tooltip.SetToolTip(this._showOriginalMultiStackSeries, resources.GetString("_showOriginalMultiStackSeries.ToolTip"));
			this._showOriginalMultiStackSeries.UseVisualStyleBackColor = true;
			// 
			// _splitStacks
			// 
			resources.ApplyResources(this._splitStacks, "_splitStacks");
			this._splitStacks.Name = "_splitStacks";
			this._tooltip.SetToolTip(this._splitStacks, resources.GetString("_splitStacks.ToolTip"));
			this._splitStacks.UseVisualStyleBackColor = true;
			// 
			// DisplaySetCreationConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._selectedOverlaysGroupBox);
			this.Controls.Add(this._splittingGroup);
			this.Controls.Add(this._presentationGroupBox);
			this.Controls.Add(this._creationGroup);
			this.Controls.Add(this._modality);
			this.Controls.Add(this.label1);
			this.Name = "DisplaySetCreationConfigurationComponentControl";
			this._creationGroup.ResumeLayout(false);
			this._creationGroup.PerformLayout();
			this._presentationGroupBox.ResumeLayout(false);
			this._presentationGroupBox.PerformLayout();
			this._selectedOverlaysGroupBox.ResumeLayout(false);
			this._splittingGroup.ResumeLayout(false);
			this._splittingGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _createSingleImageDisplaySets;
		private System.Windows.Forms.ComboBox _modality;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox _creationGroup;
		private System.Windows.Forms.GroupBox _presentationGroupBox;
		private System.Windows.Forms.CheckBox _invertImages;
        private System.Windows.Forms.CheckBox _showOriginalSeries;
        private System.Windows.Forms.CheckBox _createAllImagesDisplaySet;
        private System.Windows.Forms.ToolTip _tooltip;
        private System.Windows.Forms.CheckBox _splitMixedMultiframeSeries;
        private System.Windows.Forms.CheckBox _showOriginalMixedMultiframeSeries;
        private System.Windows.Forms.CheckBox _splitEchos;
        private System.Windows.Forms.CheckBox _showOriginalMultiEchoSeries;
        private System.Windows.Forms.GroupBox _splittingGroup;
        private System.Windows.Forms.GroupBox _selectedOverlaysGroupBox;
        private System.Windows.Forms.FlowLayoutPanel _overlaysPanel;
		private System.Windows.Forms.CheckBox _showOriginalMultiStackSeries;
		private System.Windows.Forms.CheckBox _splitStacks;
    }
}
