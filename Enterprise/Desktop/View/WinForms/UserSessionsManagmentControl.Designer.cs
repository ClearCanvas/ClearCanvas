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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserSessionsManagmentControl));
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
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._sessionsTable, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// flowLayoutPanel3
			// 
			resources.ApplyResources(this.flowLayoutPanel3, "flowLayoutPanel3");
			this.flowLayoutPanel3.Controls.Add(this._userName);
			this.flowLayoutPanel3.Controls.Add(this._displayName);
			this.flowLayoutPanel3.Controls.Add(this._lastLogin);
			this.flowLayoutPanel3.Name = "flowLayoutPanel3";
			// 
			// _userName
			// 
			resources.ApplyResources(this._userName, "_userName");
			this._userName.Mask = "";
			this._userName.Name = "_userName";
			this._userName.ReadOnly = true;
			this._userName.TabStop = false;
			this._userName.Value = null;
			// 
			// _displayName
			// 
			resources.ApplyResources(this._displayName, "_displayName");
			this._displayName.Mask = "";
			this._displayName.Name = "_displayName";
			this._displayName.ReadOnly = true;
			this._displayName.TabStop = false;
			this._displayName.Value = null;
			// 
			// _lastLogin
			// 
			resources.ApplyResources(this._lastLogin, "_lastLogin");
			this._lastLogin.Mask = "";
			this._lastLogin.Name = "_lastLogin";
			this._lastLogin.ReadOnly = true;
			this._lastLogin.TabStop = false;
			this._lastLogin.Value = null;
			// 
			// _sessionsTable
			// 
			resources.ApplyResources(this._sessionsTable, "_sessionsTable");
			this._sessionsTable.Name = "_sessionsTable";
			this._sessionsTable.ReadOnly = false;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _closeButton
			// 
			resources.ApplyResources(this._closeButton, "_closeButton");
			this._closeButton.Name = "_closeButton";
			this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
			// 
			// UserSessionsManagmentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._closeButton);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "UserSessionsManagmentControl";
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
