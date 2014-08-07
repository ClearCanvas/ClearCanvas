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
			this._pnlTop = new System.Windows.Forms.Panel();
			this._pnlTop.SuspendLayout();
			this.SuspendLayout();
			// 
			// _suggestBox
			// 
			resources.ApplyResources(this._suggestBox, "_suggestBox");
			this._suggestBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this._suggestBox.FormattingEnabled = true;
			this._suggestBox.Name = "_suggestBox";
			this._suggestBox.SuggestionProvider = null;
			this._suggestBox.Value = null;
			this._suggestBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this._suggestBox_Format);
			this._suggestBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._suggestBox_KeyDown);
			this._suggestBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._suggestBox_KeyPress);
			// 
			// _findButton
			// 
			resources.ApplyResources(this._findButton, "_findButton");
			this._findButton.Name = "_findButton";
			this._findButton.UseVisualStyleBackColor = true;
			this._findButton.Click += new System.EventHandler(this._findButton_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _pnlTop
			// 
			this._pnlTop.Controls.Add(this.label1);
			this._pnlTop.Controls.Add(this._findButton);
			resources.ApplyResources(this._pnlTop, "_pnlTop");
			this._pnlTop.Name = "_pnlTop";
			// 
			// CannedTextInplaceLookupControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._suggestBox);
			this.Controls.Add(this._pnlTop);
			this.Name = "CannedTextInplaceLookupControl";
			this._pnlTop.ResumeLayout(false);
			this._pnlTop.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.SuggestComboBox _suggestBox;
		private System.Windows.Forms.Button _findButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel _pnlTop;
    }
}
