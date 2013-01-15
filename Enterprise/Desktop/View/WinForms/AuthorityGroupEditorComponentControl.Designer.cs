#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Enterprise.Desktop.View.WinForms
{
    partial class AuthorityGroupEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthorityGroupEditorComponentControl));
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this._authorityGroupName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._tokenTreeView = new Crownwood.DotNetMagic.Controls.TreeControl();
			this.label1 = new System.Windows.Forms.Label();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._authorityGroupDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._authorityGroupDataGroup = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			resources.ApplyResources(this._acceptButton, "_acceptButton");
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _authorityGroupName
			// 
			resources.ApplyResources(this._authorityGroupName, "_authorityGroupName");
			this._authorityGroupName.Name = "_authorityGroupName";
			this._authorityGroupName.ToolTip = null;
			this._authorityGroupName.Value = null;
			// 
			// _tokenTreeView
			// 
			resources.ApplyResources(this._tokenTreeView, "_tokenTreeView");
			this._tokenTreeView.AutoEdit = false;
			this._tokenTreeView.CheckStates = Crownwood.DotNetMagic.Controls.CheckStates.ThreeStateCheck;
			this._tokenTreeView.FocusNode = null;
			this._tokenTreeView.HotBackColor = System.Drawing.Color.Empty;
			this._tokenTreeView.HotForeColor = System.Drawing.Color.Empty;
			this._tokenTreeView.Name = "_tokenTreeView";
			this._tokenTreeView.SelectedNode = null;
			this._tokenTreeView.SelectedNoFocusBackColor = System.Drawing.SystemColors.Control;
			this._tokenTreeView.AfterSelect += new Crownwood.DotNetMagic.Controls.NodeEventHandler(this._tokenTreeView_AfterSelect);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _description
			// 
			resources.ApplyResources(this._description, "_description");
			this._description.Name = "_description";
			this._description.ReadOnly = true;
			this._description.ToolTip = null;
			this._description.Value = null;
			// 
			// _authorityGroupDescription
			// 
			resources.ApplyResources(this._authorityGroupDescription, "_authorityGroupDescription");
			this._authorityGroupDescription.Name = "_authorityGroupDescription";
			this._authorityGroupDescription.ToolTip = null;
			this._authorityGroupDescription.Value = null;
			// 
			// _authorityGroupDataGroup
			// 
			resources.ApplyResources(this._authorityGroupDataGroup, "_authorityGroupDataGroup");
			this._authorityGroupDataGroup.Name = "_authorityGroupDataGroup";
			this._authorityGroupDataGroup.UseVisualStyleBackColor = true;
			// 
			// AuthorityGroupEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._authorityGroupDataGroup);
			this.Controls.Add(this._authorityGroupDescription);
			this.Controls.Add(this._description);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._tokenTreeView);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._authorityGroupName);
			this.Name = "AuthorityGroupEditorComponentControl";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _authorityGroupName;
		private Crownwood.DotNetMagic.Controls.TreeControl _tokenTreeView;
		private System.Windows.Forms.Label label1;
		private ClearCanvas.Desktop.View.WinForms.TextField _description;
        private ClearCanvas.Desktop.View.WinForms.TextField _authorityGroupDescription;
        private System.Windows.Forms.CheckBox _authorityGroupDataGroup;
    }
}
