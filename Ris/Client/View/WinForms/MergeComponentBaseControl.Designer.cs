namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class MergeComponentBaseControl
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
			this._duplicateGroupBox = new System.Windows.Forms.GroupBox();
			this._targetItem = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._sourceItem = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._switchButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._duplicateGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _duplicateGroupBox
			// 
			this._duplicateGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._duplicateGroupBox.Controls.Add(this._targetItem);
			this._duplicateGroupBox.Controls.Add(this._sourceItem);
			this._duplicateGroupBox.Controls.Add(this._switchButton);
			this._duplicateGroupBox.Location = new System.Drawing.Point(3, 3);
			this._duplicateGroupBox.Name = "_duplicateGroupBox";
			this._duplicateGroupBox.Size = new System.Drawing.Size(644, 120);
			this._duplicateGroupBox.TabIndex = 0;
			this._duplicateGroupBox.TabStop = false;
			this._duplicateGroupBox.Text = "Select items to merge";
			// 
			// _targetItem
			// 
			this._targetItem.DataSource = null;
			this._targetItem.DisplayMember = "";
			this._targetItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._targetItem.LabelText = "and replace usages with this:";
			this._targetItem.Location = new System.Drawing.Point(8, 69);
			this._targetItem.Margin = new System.Windows.Forms.Padding(2);
			this._targetItem.Name = "_targetItem";
			this._targetItem.Size = new System.Drawing.Size(565, 41);
			this._targetItem.TabIndex = 5;
			this._targetItem.Value = null;
			// 
			// _sourceItem
			// 
			this._sourceItem.DataSource = null;
			this._sourceItem.DisplayMember = "";
			this._sourceItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._sourceItem.LabelText = "De-activate this:";
			this._sourceItem.Location = new System.Drawing.Point(7, 24);
			this._sourceItem.Margin = new System.Windows.Forms.Padding(2);
			this._sourceItem.Name = "_sourceItem";
			this._sourceItem.Size = new System.Drawing.Size(565, 41);
			this._sourceItem.TabIndex = 4;
			this._sourceItem.Value = null;
			// 
			// _switchButton
			// 
			this._switchButton.Image = global::ClearCanvas.Ris.Client.View.WinForms.SR.SwapSmall;
			this._switchButton.Location = new System.Drawing.Point(592, 55);
			this._switchButton.Name = "_switchButton";
			this._switchButton.Size = new System.Drawing.Size(35, 32);
			this._switchButton.TabIndex = 1;
			this._switchButton.UseVisualStyleBackColor = true;
			this._switchButton.Click += new System.EventHandler(this._switchButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._acceptButton.Location = new System.Drawing.Point(491, 129);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 1;
			this._acceptButton.Text = "Merge";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(572, 129);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 2;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// MergeComponentBaseControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._duplicateGroupBox);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Name = "MergeComponentBaseControl";
			this.Size = new System.Drawing.Size(653, 160);
			this._duplicateGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.GroupBox _duplicateGroupBox;
		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _switchButton;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _targetItem;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _sourceItem;
    }
}
