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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeComponentBaseControl));
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
			resources.ApplyResources(this._duplicateGroupBox, "_duplicateGroupBox");
			this._duplicateGroupBox.Controls.Add(this._targetItem);
			this._duplicateGroupBox.Controls.Add(this._sourceItem);
			this._duplicateGroupBox.Controls.Add(this._switchButton);
			this._duplicateGroupBox.Name = "_duplicateGroupBox";
			this._duplicateGroupBox.TabStop = false;
			// 
			// _targetItem
			// 
			this._targetItem.DataSource = null;
			this._targetItem.DisplayMember = "";
			this._targetItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._targetItem, "_targetItem");
			this._targetItem.Name = "_targetItem";
			this._targetItem.Value = null;
			// 
			// _sourceItem
			// 
			this._sourceItem.DataSource = null;
			this._sourceItem.DisplayMember = "";
			this._sourceItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._sourceItem, "_sourceItem");
			this._sourceItem.Name = "_sourceItem";
			this._sourceItem.Value = null;
			// 
			// _switchButton
			// 
			this._switchButton.Image = global::ClearCanvas.Ris.Client.View.WinForms.SR.SwapSmall;
			resources.ApplyResources(this._switchButton, "_switchButton");
			this._switchButton.Name = "_switchButton";
			this._switchButton.UseVisualStyleBackColor = true;
			this._switchButton.Click += new System.EventHandler(this._switchButton_Click);
			// 
			// _acceptButton
			// 
			resources.ApplyResources(this._acceptButton, "_acceptButton");
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// MergeComponentBaseControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._duplicateGroupBox);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Name = "MergeComponentBaseControl";
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
