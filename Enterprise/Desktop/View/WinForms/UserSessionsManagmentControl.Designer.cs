namespace ClearCanvas.Enterprise.Desktop.View.WinForms
{
    partial class UserSessionsManagmentControl
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
			this._userName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._displayName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._lastLogin = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._sessionsTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label1 = new System.Windows.Forms.Label();
			this._closeButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(8, 5);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(0, 0);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(5, 284);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(707, 0);
			this.panel1.TabIndex = 3;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._sessionsTable, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(712, 253);
			this.tableLayoutPanel1.TabIndex = 5;
			// 
			// flowLayoutPanel3
			// 
			this.flowLayoutPanel3.AutoSize = true;
			this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel3.Controls.Add(this._userName);
			this.flowLayoutPanel3.Controls.Add(this._displayName);
			this.flowLayoutPanel3.Controls.Add(this._lastLogin);
			this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 3);
			this.flowLayoutPanel3.Name = "flowLayoutPanel3";
			this.flowLayoutPanel3.Padding = new System.Windows.Forms.Padding(0, 10, 10, 0);
			this.flowLayoutPanel3.Size = new System.Drawing.Size(626, 55);
			this.flowLayoutPanel3.TabIndex = 2;
			// 
			// _userName
			// 
			this._userName.LabelText = "User ID";
			this._userName.Location = new System.Drawing.Point(2, 12);
			this._userName.Margin = new System.Windows.Forms.Padding(2);
			this._userName.Mask = "";
			this._userName.Name = "_userName";
			this._userName.PasswordChar = '\0';
			this._userName.ReadOnly = true;
			this._userName.Size = new System.Drawing.Size(150, 41);
			this._userName.TabIndex = 0;
			this._userName.TabStop = false;
			this._userName.ToolTip = null;
			this._userName.Value = null;
			// 
			// _displayName
			// 
			this._displayName.LabelText = "Display Name";
			this._displayName.Location = new System.Drawing.Point(156, 12);
			this._displayName.Margin = new System.Windows.Forms.Padding(2);
			this._displayName.Mask = "";
			this._displayName.Name = "_displayName";
			this._displayName.PasswordChar = '\0';
			this._displayName.ReadOnly = true;
			this._displayName.Size = new System.Drawing.Size(304, 41);
			this._displayName.TabIndex = 1;
			this._displayName.TabStop = false;
			this._displayName.ToolTip = null;
			this._displayName.Value = null;
			// 
			// _lastLogin
			// 
			this._lastLogin.LabelText = "Last Login Time";
			this._lastLogin.Location = new System.Drawing.Point(464, 12);
			this._lastLogin.Margin = new System.Windows.Forms.Padding(2);
			this._lastLogin.Mask = "";
			this._lastLogin.Name = "_lastLogin";
			this._lastLogin.PasswordChar = '\0';
			this._lastLogin.ReadOnly = true;
			this._lastLogin.Size = new System.Drawing.Size(150, 41);
			this._lastLogin.TabIndex = 2;
			this._lastLogin.TabStop = false;
			this._lastLogin.ToolTip = null;
			this._lastLogin.Value = null;
			// 
			// _sessionsTable
			// 
			this._sessionsTable.ColumnHeaderTooltip = null;
			this._sessionsTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._sessionsTable.Location = new System.Drawing.Point(3, 84);
			this._sessionsTable.MinimumSize = new System.Drawing.Size(700, 150);
			this._sessionsTable.Name = "_sessionsTable";
			this._sessionsTable.ReadOnly = false;
			this._sessionsTable.Size = new System.Drawing.Size(706, 166);
			this._sessionsTable.SortButtonTooltip = null;
			this._sessionsTable.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Active Sessions";
			// 
			// _closeButton
			// 
			this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._closeButton.Location = new System.Drawing.Point(637, 259);
			this._closeButton.Name = "_closeButton";
			this._closeButton.Size = new System.Drawing.Size(75, 25);
			this._closeButton.TabIndex = 3;
			this._closeButton.Text = "Close";
			this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
			// 
			// UserSessionsManagmentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._closeButton);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "UserSessionsManagmentControl";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.Size = new System.Drawing.Size(717, 289);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private ClearCanvas.Desktop.View.WinForms.TextField _userName;
        private ClearCanvas.Desktop.View.WinForms.TextField _displayName;
		private ClearCanvas.Desktop.View.WinForms.TextField _lastLogin;
        private ClearCanvas.Desktop.View.WinForms.TableView _sessionsTable;
		private System.Windows.Forms.Button _closeButton;
		private System.Windows.Forms.Label label1;
    }
}
