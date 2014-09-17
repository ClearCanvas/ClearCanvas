namespace ClearCanvas.Ris.Client.View.WinForms
{
	partial class DiagnosticServiceSummaryComponentControl
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagnosticServiceSummaryComponentControl));
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this._clearButton = new System.Windows.Forms.Button();
			this._searchButton = new System.Windows.Forms.Button();
			this._id = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._diagnosticServiceTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.flowLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
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
			// panel1
			// 
			this.panel1.Controls.Add(this._clearButton);
			this.panel1.Controls.Add(this._searchButton);
			this.panel1.Controls.Add(this._id);
			this.panel1.Controls.Add(this._name);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _clearButton
			// 
			resources.ApplyResources(this._clearButton, "_clearButton");
			this._clearButton.BackColor = System.Drawing.Color.Transparent;
			this._clearButton.FlatAppearance.BorderSize = 0;
			this._clearButton.Image = global::ClearCanvas.Ris.Client.View.WinForms.SR.ClearFilterSmall;
			this._clearButton.Name = "_clearButton";
			this.toolTip1.SetToolTip(this._clearButton, resources.GetString("_clearButton.ToolTip"));
			this._clearButton.UseVisualStyleBackColor = false;
			this._clearButton.Click += new System.EventHandler(this._clearButton_Click);
			// 
			// _searchButton
			// 
			resources.ApplyResources(this._searchButton, "_searchButton");
			this._searchButton.BackColor = System.Drawing.Color.Transparent;
			this._searchButton.FlatAppearance.BorderSize = 0;
			this._searchButton.Image = global::ClearCanvas.Ris.Client.View.WinForms.SR.SearchToolSmall;
			this._searchButton.Name = "_searchButton";
			this.toolTip1.SetToolTip(this._searchButton, resources.GetString("_searchButton.ToolTip"));
			this._searchButton.UseVisualStyleBackColor = false;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _id
			// 
			resources.ApplyResources(this._id, "_id");
			this._id.Mask = "";
			this._id.Name = "_id";
			this._id.Value = null;
			this._id.Enter += new System.EventHandler(this._field_Enter);
			this._id.Leave += new System.EventHandler(this._field_Leave);
			// 
			// _name
			// 
			resources.ApplyResources(this._name, "_name");
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.Value = null;
			this._name.Enter += new System.EventHandler(this._field_Enter);
			this._name.Leave += new System.EventHandler(this._field_Leave);
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._diagnosticServiceTableView, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _diagnosticServiceTableView
			// 
			resources.ApplyResources(this._diagnosticServiceTableView, "_diagnosticServiceTableView");
			this._diagnosticServiceTableView.Name = "_diagnosticServiceTableView";
			this._diagnosticServiceTableView.ReadOnly = false;
			this._diagnosticServiceTableView.ItemDoubleClicked += new System.EventHandler(this._diagnosticServiceTableView_ItemDoubleClicked);
			// 
			// DiagnosticServiceSummaryComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "DiagnosticServiceSummaryComponentControl";
			this.flowLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Panel panel1;
		private ClearCanvas.Desktop.View.WinForms.TextField _id;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private ClearCanvas.Desktop.View.WinForms.TableView _diagnosticServiceTableView;
		private System.Windows.Forms.Button _clearButton;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button _searchButton;


	}
}