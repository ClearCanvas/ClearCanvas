namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class ReassignComponentControl
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
			this._radiologist = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _radiologist
			// 
			this._radiologist.AutoSize = true;
			this._radiologist.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._radiologist.LabelText = "Reassign to radiologist:";
			this._radiologist.Location = new System.Drawing.Point(8, 8);
			this._radiologist.Margin = new System.Windows.Forms.Padding(2);
			this._radiologist.Name = "_radiologist";
			this._radiologist.Size = new System.Drawing.Size(290, 43);
			this._radiologist.TabIndex = 0;
			this._radiologist.Value = null;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(223, 57);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 2;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(142, 57);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 1;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// ReassignComponentControl
			// 
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._radiologist);
			this.Name = "ReassignComponentControl";
			this.Size = new System.Drawing.Size(318, 89);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Ris.Client.View.WinForms.LookupField _radiologist;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
    }
}
