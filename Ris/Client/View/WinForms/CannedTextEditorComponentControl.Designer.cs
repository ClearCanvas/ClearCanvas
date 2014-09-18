namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class CannedTextEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CannedTextEditorComponentControl));
			this._editorTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._text = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._typeGroupBox = new System.Windows.Forms.GroupBox();
			this._radioGroup = new System.Windows.Forms.RadioButton();
			this._radioPersonal = new System.Windows.Forms.RadioButton();
			this._groups = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._category = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this._editorTableLayoutPanel.SuspendLayout();
			this._typeGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _editorTableLayoutPanel
			// 
			resources.ApplyResources(this._editorTableLayoutPanel, "_editorTableLayoutPanel");
			this._editorTableLayoutPanel.Controls.Add(this._text, 0, 4);
			this._editorTableLayoutPanel.Controls.Add(this._name, 0, 2);
			this._editorTableLayoutPanel.Controls.Add(this._typeGroupBox, 0, 0);
			this._editorTableLayoutPanel.Controls.Add(this._groups, 0, 1);
			this._editorTableLayoutPanel.Controls.Add(this._category, 0, 3);
			this._editorTableLayoutPanel.Name = "_editorTableLayoutPanel";
			// 
			// _text
			// 
			resources.ApplyResources(this._text, "_text");
			this._text.Name = "_text";
			this._text.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._text.Value = null;
			// 
			// _name
			// 
			resources.ApplyResources(this._name, "_name");
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.Value = null;
			// 
			// _typeGroupBox
			// 
			this._typeGroupBox.Controls.Add(this._radioGroup);
			this._typeGroupBox.Controls.Add(this._radioPersonal);
			resources.ApplyResources(this._typeGroupBox, "_typeGroupBox");
			this._typeGroupBox.Name = "_typeGroupBox";
			this._typeGroupBox.TabStop = false;
			// 
			// _radioGroup
			// 
			resources.ApplyResources(this._radioGroup, "_radioGroup");
			this._radioGroup.Name = "_radioGroup";
			this._radioGroup.TabStop = true;
			this._radioGroup.UseVisualStyleBackColor = true;
			// 
			// _radioPersonal
			// 
			resources.ApplyResources(this._radioPersonal, "_radioPersonal");
			this._radioPersonal.Name = "_radioPersonal";
			this._radioPersonal.TabStop = true;
			this._radioPersonal.UseVisualStyleBackColor = true;
			// 
			// _groups
			// 
			this._groups.DataSource = null;
			this._groups.DisplayMember = "";
			resources.ApplyResources(this._groups, "_groups");
			this._groups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._groups.Name = "_groups";
			this._groups.Value = null;
			// 
			// _category
			// 
			this._category.DataSource = null;
			this._category.DisplayMember = "";
			resources.ApplyResources(this._category, "_category");
			this._category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
			this._category.Name = "_category";
			this._category.Value = null;
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
			// CannedTextEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._editorTableLayoutPanel);
			this.Name = "CannedTextEditorComponentControl";
			this._editorTableLayoutPanel.ResumeLayout(false);
			this._typeGroupBox.ResumeLayout(false);
			this._typeGroupBox.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel _editorTableLayoutPanel;
		private ClearCanvas.Desktop.View.WinForms.TextAreaField _text;
        private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.GroupBox _typeGroupBox;
		private System.Windows.Forms.RadioButton _radioGroup;
		private System.Windows.Forms.RadioButton _radioPersonal;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _groups;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _category;
    }
}
