namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class ProcedureTypeGroupSummaryComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcedureTypeGroupSummaryComponentControl));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._procedureTypeGroupTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.panel1 = new System.Windows.Forms.Panel();
			this._clearButton = new System.Windows.Forms.Button();
			this._searchButton = new System.Windows.Forms.Button();
			this._category = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._procedureTypeGroupTableView, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _procedureTypeGroupTableView
			// 
			resources.ApplyResources(this._procedureTypeGroupTableView, "_procedureTypeGroupTableView");
			this._procedureTypeGroupTableView.FilterTextBoxVisible = true;
			this._procedureTypeGroupTableView.Name = "_procedureTypeGroupTableView";
			this._procedureTypeGroupTableView.ReadOnly = false;
			this._procedureTypeGroupTableView.ItemDoubleClicked += new System.EventHandler(this._procedureTypeGroupTableView_ItemDoubleClicked);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._clearButton);
			this.panel1.Controls.Add(this._searchButton);
			this.panel1.Controls.Add(this._category);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _clearButton
			// 
			resources.ApplyResources(this._clearButton, "_clearButton");
			this._clearButton.BackColor = System.Drawing.Color.Transparent;
			this._clearButton.FlatAppearance.BorderSize = 0;
			this._clearButton.Image = global::ClearCanvas.Ris.Client.Admin.View.WinForms.SR.ClearFilterSmall;
			this._clearButton.Name = "_clearButton";
			this._clearButton.UseVisualStyleBackColor = false;
			this._clearButton.Click += new System.EventHandler(this._clearButton_Click);
			// 
			// _searchButton
			// 
			resources.ApplyResources(this._searchButton, "_searchButton");
			this._searchButton.BackColor = System.Drawing.Color.Transparent;
			this._searchButton.FlatAppearance.BorderSize = 0;
			this._searchButton.Image = global::ClearCanvas.Ris.Client.Admin.View.WinForms.SR.SearchToolSmall;
			this._searchButton.Name = "_searchButton";
			this._searchButton.UseVisualStyleBackColor = false;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _category
			// 
			this._category.DataSource = null;
			this._category.DisplayMember = "";
			this._category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._category, "_category");
			this._category.Name = "_category";
			this._category.Value = null;
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._okButton);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// ProcedureTypeGroupSummaryComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ProcedureTypeGroupSummaryComponentControl";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private ClearCanvas.Desktop.View.WinForms.TableView _procedureTypeGroupTableView;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Panel panel1;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _category;
		private System.Windows.Forms.Button _clearButton;
		private System.Windows.Forms.Button _searchButton;
    }
}
