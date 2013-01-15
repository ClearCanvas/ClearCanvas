namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class SmartTimeField
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
            this._input = new ClearCanvas.Desktop.View.WinForms.SuggestComboField();
            this.SuspendLayout();
            // 
            // _input
            // 
            this._input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._input.LabelText = "label";
            this._input.Location = new System.Drawing.Point(0, 2);
            this._input.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._input.Name = "_input";
            this._input.Size = new System.Drawing.Size(177, 50);
            this._input.TabIndex = 0;
            this._input.Value = null;
            // 
            // SmartTimeField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._input);
            this.Name = "SmartTimeField";
            this.Size = new System.Drawing.Size(177, 51);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.SuggestComboField _input;
    }
}
