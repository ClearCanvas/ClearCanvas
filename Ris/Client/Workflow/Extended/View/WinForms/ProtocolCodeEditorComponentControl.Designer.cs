namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    partial class ProtocolCodeEditorComponentControl
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._name, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._description, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(379, 182);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._acceptButton);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 150);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel1.Size = new System.Drawing.Size(358, 29);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(280, 3);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(199, 3);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 0;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = false;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _name
			// 
			this._name.AutoSize = true;
			this._name.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._name.Dock = System.Windows.Forms.DockStyle.Top;
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(2, 2);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(360, 40);
			this._name.TabIndex = 0;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _description
			// 
			this._description.AutoSize = true;
			this._description.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._description.Dock = System.Windows.Forms.DockStyle.Fill;
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(2, 46);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(360, 99);
			this._description.TabIndex = 1;
			this._description.Value = null;
			// 
			// ProtocolCodeEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ProtocolCodeEditorComponentControl";
			this.Size = new System.Drawing.Size(379, 182);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _name;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
    }
}
