namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    partial class ProtocolGroupEditorComponentControl
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
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this._protocolCodesTabPage = new System.Windows.Forms.TabPage();
			this._codesSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
			this._readingGroupsTabPage = new System.Windows.Forms.TabPage();
			this._readingGroupsSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this.tabControl1.SuspendLayout();
			this._protocolCodesTabPage.SuspendLayout();
			this._readingGroupsTabPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(584, 541);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._acceptButton.Location = new System.Drawing.Point(503, 541);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 0;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this._protocolCodesTabPage);
			this.tabControl1.Controls.Add(this._readingGroupsTabPage);
			this.tabControl1.Location = new System.Drawing.Point(6, 124);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(655, 411);
			this.tabControl1.TabIndex = 1;
			// 
			// _protocolCodesTabPage
			// 
			this._protocolCodesTabPage.Controls.Add(this._codesSelector);
			this._protocolCodesTabPage.Location = new System.Drawing.Point(4, 22);
			this._protocolCodesTabPage.Name = "_protocolCodesTabPage";
			this._protocolCodesTabPage.Padding = new System.Windows.Forms.Padding(3);
			this._protocolCodesTabPage.Size = new System.Drawing.Size(647, 385);
			this._protocolCodesTabPage.TabIndex = 0;
			this._protocolCodesTabPage.Text = "Codes";
			this._protocolCodesTabPage.UseVisualStyleBackColor = true;
			// 
			// _codesSelector
			// 
			this._codesSelector.AvailableItemsTable = null;
			this._codesSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this._codesSelector.Location = new System.Drawing.Point(3, 3);
			this._codesSelector.Name = "_codesSelector";
			this._codesSelector.ReadOnly = false;
			this._codesSelector.SelectedItemsTable = null;
			this._codesSelector.Size = new System.Drawing.Size(641, 379);
			this._codesSelector.TabIndex = 0;
			// 
			// _readingGroupsTabPage
			// 
			this._readingGroupsTabPage.Controls.Add(this._readingGroupsSelector);
			this._readingGroupsTabPage.Location = new System.Drawing.Point(4, 22);
			this._readingGroupsTabPage.Name = "_readingGroupsTabPage";
			this._readingGroupsTabPage.Padding = new System.Windows.Forms.Padding(3);
			this._readingGroupsTabPage.Size = new System.Drawing.Size(647, 389);
			this._readingGroupsTabPage.TabIndex = 1;
			this._readingGroupsTabPage.Text = "Reading Groups";
			this._readingGroupsTabPage.UseVisualStyleBackColor = true;
			// 
			// _readingGroupsSelector
			// 
			this._readingGroupsSelector.AutoSize = true;
			this._readingGroupsSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._readingGroupsSelector.AvailableItemsTable = null;
			this._readingGroupsSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this._readingGroupsSelector.Location = new System.Drawing.Point(3, 3);
			this._readingGroupsSelector.Name = "_readingGroupsSelector";
			this._readingGroupsSelector.ReadOnly = false;
			this._readingGroupsSelector.SelectedItemsTable = null;
			this._readingGroupsSelector.ShowToolbars = false;
			this._readingGroupsSelector.Size = new System.Drawing.Size(641, 383);
			this._readingGroupsSelector.TabIndex = 0;
			// 
			// _name
			// 
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(3, 3);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(626, 40);
			this._name.TabIndex = 0;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _description
			// 
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(5, 48);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(626, 69);
			this._description.TabIndex = 1;
			this._description.Value = null;
			// 
			// ProtocolGroupEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._description);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._name);
			this.Name = "ProtocolGroupEditorComponentControl";
			this.Size = new System.Drawing.Size(664, 569);
			this.tabControl1.ResumeLayout(false);
			this._protocolCodesTabPage.ResumeLayout(false);
			this._readingGroupsTabPage.ResumeLayout(false);
			this._readingGroupsTabPage.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage _protocolCodesTabPage;
		private System.Windows.Forms.TabPage _readingGroupsTabPage;
        private ClearCanvas.Desktop.View.WinForms.TextField _name;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
        private ClearCanvas.Desktop.View.WinForms.ListItemSelector _codesSelector;
		private ClearCanvas.Desktop.View.WinForms.ListItemSelector _readingGroupsSelector;
    }
}
