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
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._reason, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._otherReason, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(455, 225);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _reason
			// 
			this._reason.AutoSize = true;
			this._reason.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._reason.DataSource = null;
			this._reason.DisplayMember = "";
			this._reason.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._reason.LabelText = "Reason";
			this._reason.Location = new System.Drawing.Point(2, 2);
			this._reason.Margin = new System.Windows.Forms.Padding(2);
			this._reason.Name = "_reason";
			this._reason.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this._reason.Size = new System.Drawing.Size(451, 41);
			this._reason.TabIndex = 0;
			this._reason.Value = null;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._btnCancel);
			this.flowLayoutPanel1.Controls.Add(this._btnOK);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 193);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel1.Size = new System.Drawing.Size(449, 29);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// _btnCancel
			// 
			this._btnCancel.Location = new System.Drawing.Point(371, 3);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 1;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
			// 
			// _btnOK
			// 
			this._btnOK.Location = new System.Drawing.Point(290, 3);
			this._btnOK.Name = "_btnOK";
			this._btnOK.Size = new System.Drawing.Size(75, 23);
			this._btnOK.TabIndex = 0;
			this._btnOK.Text = "OK";
			this._btnOK.UseVisualStyleBackColor = true;
			this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
			// 
			// _otherReason
			// 
			this._otherReason.AutoSize = true;
			this._otherReason.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._otherReason.Dock = System.Windows.Forms.DockStyle.Fill;
			this._otherReason.LabelText = "Additional Comments";
			this._otherReason.Location = new System.Drawing.Point(2, 47);
			this._otherReason.Margin = new System.Windows.Forms.Padding(2);
			this._otherReason.MaximumLength = 2147483647;
			this._otherReason.Name = "_otherReason";
			this._otherReason.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this._otherReason.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
			this._otherReason.Size = new System.Drawing.Size(451, 141);
			this._otherReason.TabIndex = 1;
			this._otherReason.Value = null;
			// 
			// ProtocolReasonComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ProtocolReasonComponentControl";
			this.Size = new System.Drawing.Size(455, 225);
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
