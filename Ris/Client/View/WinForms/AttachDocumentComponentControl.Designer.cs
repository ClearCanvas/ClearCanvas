namespace ClearCanvas.Ris.Client.View.WinForms
{
	partial class AttachDocumentComponentControl
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
			this._category = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._file = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this._browseButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _category
			// 
			this._category.DataSource = null;
			this._category.DisplayMember = "";
			this._category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
			this._category.LabelText = "Category of document";
			this._category.Location = new System.Drawing.Point(24, 120);
			this._category.Margin = new System.Windows.Forms.Padding(2);
			this._category.Name = "_category";
			this._category.Size = new System.Drawing.Size(364, 41);
			this._category.TabIndex = 2;
			this._category.Value = null;
			// 
			// _file
			// 
			this._file.LabelText = "File";
			this._file.Location = new System.Drawing.Point(24, 35);
			this._file.Margin = new System.Windows.Forms.Padding(2);
			this._file.Mask = "";
			this._file.Name = "_file";
			this._file.PasswordChar = '\0';
			this._file.Size = new System.Drawing.Size(315, 41);
			this._file.TabIndex = 0;
			this._file.ToolTip = null;
			this._file.Value = null;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(313, 199);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 4;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(232, 199);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 3;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _browseButton
			// 
			this._browseButton.Location = new System.Drawing.Point(362, 51);
			this._browseButton.Name = "_browseButton";
			this._browseButton.Size = new System.Drawing.Size(26, 23);
			this._browseButton.TabIndex = 1;
			this._browseButton.Text = "...";
			this._browseButton.UseVisualStyleBackColor = true;
			this._browseButton.Click += new System.EventHandler(this._browseButton_Click);
			// 
			// AttachDocumentComponentControl
			// 
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._browseButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._file);
			this.Controls.Add(this._category);
			this.Name = "AttachDocumentComponentControl";
			this.Size = new System.Drawing.Size(422, 240);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _category;
		private ClearCanvas.Desktop.View.WinForms.TextField _file;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _browseButton;
	}
}
