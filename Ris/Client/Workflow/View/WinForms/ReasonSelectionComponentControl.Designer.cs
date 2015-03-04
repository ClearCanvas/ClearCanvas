namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class ReasonSelectionComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReasonSelectionComponentControl));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._reason = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._btnCancel = new System.Windows.Forms.Button();
			this._btnOK = new System.Windows.Forms.Button();
			this._otherReason = new ClearCanvas.Desktop.View.WinForms.RichTextField();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._reason, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._otherReason, 0, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _reason
			// 
			resources.ApplyResources(this._reason, "_reason");
			this._reason.DataSource = null;
			this._reason.DisplayMember = "";
			this._reason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._reason.Name = "_reason";
			this._reason.Value = null;
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Controls.Add(this._btnCancel);
			this.flowLayoutPanel1.Controls.Add(this._btnOK);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// _btnCancel
			// 
			resources.ApplyResources(this._btnCancel, "_btnCancel");
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
			// 
			// _btnOK
			// 
			resources.ApplyResources(this._btnOK, "_btnOK");
			this._btnOK.Name = "_btnOK";
			this._btnOK.UseVisualStyleBackColor = true;
			this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
			// 
			// _otherReason
			// 
			resources.ApplyResources(this._otherReason, "_otherReason");
			this._otherReason.MaximumLength = 2147483647;
			this._otherReason.Name = "_otherReason";
			this._otherReason.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
			this._otherReason.Value = null;
			// 
			// ReasonSelectionComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ReasonSelectionComponentControl";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _reason;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Button _btnOK;
		private ClearCanvas.Desktop.View.WinForms.RichTextField _otherReason;
    }
}
