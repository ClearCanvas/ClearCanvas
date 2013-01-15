namespace ClearCanvas.Ris.Client.View.WinForms
{
	partial class LookupHandlerCellEditorControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LookupHandlerCellEditorControl));
			this._suggestBox = new ClearCanvas.Desktop.View.WinForms.SuggestComboBox();
			this._findButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _suggestBox
			// 
			this._suggestBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._suggestBox.FormattingEnabled = true;
			this._suggestBox.Location = new System.Drawing.Point(0, 0);
			this._suggestBox.Name = "_suggestBox";
			this._suggestBox.Size = new System.Drawing.Size(202, 21);
			this._suggestBox.SuggestionProvider = null;
			this._suggestBox.TabIndex = 3;
			this._suggestBox.Value = null;
			this._suggestBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this._suggestBox_Format);
			// 
			// _findButton
			// 
			this._findButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._findButton.Image = ((System.Drawing.Image)(resources.GetObject("_findButton.Image")));
			this._findButton.Location = new System.Drawing.Point(205, -1);
			this._findButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
			this._findButton.Name = "_findButton";
			this._findButton.Size = new System.Drawing.Size(25, 24);
			this._findButton.TabIndex = 4;
			this._findButton.UseVisualStyleBackColor = true;
			this._findButton.Click += new System.EventHandler(this._findButton_Click);
			// 
			// LookupHandlerCellEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._findButton);
			this.Controls.Add(this._suggestBox);
			this.Name = "LookupHandlerCellEditorControl";
			this.Size = new System.Drawing.Size(231, 23);
			this.Load += new System.EventHandler(this.LookupHandlerCellEditorControl_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.SuggestComboBox _suggestBox;
		private System.Windows.Forms.Button _findButton;
	}
}
