namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class CannedTextInplaceLookupControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CannedTextInplaceLookupControl));
			this._suggestBox = new ClearCanvas.Desktop.View.WinForms.SuggestComboBox();
			this._findButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _suggestBox
			// 
			this._suggestBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._suggestBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this._suggestBox.FormattingEnabled = true;
			this._suggestBox.Location = new System.Drawing.Point(-1, 18);
			this._suggestBox.Name = "_suggestBox";
			this._suggestBox.Size = new System.Drawing.Size(250, 130);
			this._suggestBox.SuggestionProvider = null;
			this._suggestBox.TabIndex = 2;
			this._suggestBox.Value = null;
			this._suggestBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._suggestBox_KeyPress);
			this._suggestBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._suggestBox_KeyDown);
			this._suggestBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this._suggestBox_Format);
			// 
			// _findButton
			// 
			this._findButton.Image = ((System.Drawing.Image)(resources.GetObject("_findButton.Image")));
			this._findButton.Location = new System.Drawing.Point(228, -1);
			this._findButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
			this._findButton.Name = "_findButton";
			this._findButton.Size = new System.Drawing.Size(25, 24);
			this._findButton.TabIndex = 1;
			this._findButton.UseVisualStyleBackColor = true;
			this._findButton.Visible = false;
			this._findButton.Click += new System.EventHandler(this._findButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Canned Text";
			// 
			// CannedTextInplaceLookupControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._findButton);
			this.Controls.Add(this._suggestBox);
			this.Name = "CannedTextInplaceLookupControl";
			this.Size = new System.Drawing.Size(250, 145);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.SuggestComboBox _suggestBox;
		private System.Windows.Forms.Button _findButton;
		private System.Windows.Forms.Label label1;
    }
}
