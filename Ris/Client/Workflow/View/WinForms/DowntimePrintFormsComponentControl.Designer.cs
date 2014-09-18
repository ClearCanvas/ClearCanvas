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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DowntimePrintFormsComponentControl));
			this._printButton = new System.Windows.Forms.Button();
			this._numberOfForms = new System.Windows.Forms.NumericUpDown();
			this._closeButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._numberOfForms)).BeginInit();
			this.SuspendLayout();
			// 
			// _printButton
			// 
			resources.ApplyResources(this._printButton, "_printButton");
			this._printButton.Name = "_printButton";
			this._printButton.UseVisualStyleBackColor = true;
			this._printButton.Click += new System.EventHandler(this._printButton_Click);
			// 
			// _numberOfForms
			// 
			resources.ApplyResources(this._numberOfForms, "_numberOfForms");
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
			this._numberOfForms.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _closeButton
			// 
			resources.ApplyResources(this._closeButton, "_closeButton");
			this._closeButton.Name = "_closeButton";
			this._closeButton.UseVisualStyleBackColor = true;
			this._closeButton.Click += new System.EventHandler(this._cancelPrintingButton_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// DowntimePrintFormsComponentControl
			// 
			this.AcceptButton = this._printButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._closeButton;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._numberOfForms);
			this.Controls.Add(this._closeButton);
			this.Controls.Add(this._printButton);
			this.Name = "DowntimePrintFormsComponentControl";
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
