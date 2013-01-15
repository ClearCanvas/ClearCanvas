namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class DowntimePrintFormsComponentControl
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
			this._printButton = new System.Windows.Forms.Button();
			this._numberOfForms = new System.Windows.Forms.NumericUpDown();
			this._closeButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._numberOfForms)).BeginInit();
			this.SuspendLayout();
			// 
			// _printButton
			// 
			this._printButton.Location = new System.Drawing.Point(88, 97);
			this._printButton.Name = "_printButton";
			this._printButton.Size = new System.Drawing.Size(75, 23);
			this._printButton.TabIndex = 2;
			this._printButton.Text = "Print";
			this._printButton.UseVisualStyleBackColor = true;
			this._printButton.Click += new System.EventHandler(this._printButton_Click);
			// 
			// _numberOfForms
			// 
			this._numberOfForms.Location = new System.Drawing.Point(125, 41);
			this._numberOfForms.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._numberOfForms.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._numberOfForms.Name = "_numberOfForms";
			this._numberOfForms.Size = new System.Drawing.Size(119, 20);
			this._numberOfForms.TabIndex = 1;
			this._numberOfForms.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._numberOfForms.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _closeButton
			// 
			this._closeButton.Location = new System.Drawing.Point(169, 97);
			this._closeButton.Name = "_closeButton";
			this._closeButton.Size = new System.Drawing.Size(75, 23);
			this._closeButton.TabIndex = 3;
			this._closeButton.Text = "Close";
			this._closeButton.UseVisualStyleBackColor = true;
			this._closeButton.Click += new System.EventHandler(this._cancelPrintingButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(19, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Number of Forms";
			// 
			// DowntimePrintFormsComponentControl
			// 
			this.AcceptButton = this._printButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._closeButton;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._numberOfForms);
			this.Controls.Add(this._closeButton);
			this.Controls.Add(this._printButton);
			this.Name = "DowntimePrintFormsComponentControl";
			this.Size = new System.Drawing.Size(265, 133);
			((System.ComponentModel.ISupportInitialize)(this._numberOfForms)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _printButton;
		private System.Windows.Forms.NumericUpDown _numberOfForms;
		private System.Windows.Forms.Button _closeButton;
		private System.Windows.Forms.Label label1;

	}
}
