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
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                                     | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._okButton);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 465);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(702, 30);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(625, 2);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(546, 2);
			this._okButton.Margin = new System.Windows.Forms.Padding(2);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 0;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._clearButton);
			this.panel1.Controls.Add(this._searchButton);
			this.panel1.Controls.Add(this._id);
			this.panel1.Controls.Add(this._name);
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(700, 54);
			this.panel1.TabIndex = 0;
			// 
			// _clearButton
			// 
			this._clearButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._clearButton.BackColor = System.Drawing.Color.Transparent;
			this._clearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._clearButton.FlatAppearance.BorderSize = 0;
			this._clearButton.Image = global::ClearCanvas.Ris.Client.View.WinForms.SR.ClearFilterSmall;
			this._clearButton.Location = new System.Drawing.Point(332, 19);
			this._clearButton.Margin = new System.Windows.Forms.Padding(0);
			this._clearButton.Name = "_clearButton";
			this._clearButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._clearButton.Size = new System.Drawing.Size(30, 30);
			this._clearButton.TabIndex = 5;
			this._clearButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolTip1.SetToolTip(this._clearButton, "Clear search query");
			this._clearButton.UseVisualStyleBackColor = false;
			this._clearButton.Click += new System.EventHandler(this._clearButton_Click);
			// 
			// _searchButton
			// 
			this._searchButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._searchButton.BackColor = System.Drawing.Color.Transparent;
			this._searchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._searchButton.FlatAppearance.BorderSize = 0;
			this._searchButton.Image = global::ClearCanvas.Ris.Client.View.WinForms.SR.SearchToolSmall;
			this._searchButton.Location = new System.Drawing.Point(301, 19);
			this._searchButton.Margin = new System.Windows.Forms.Padding(0);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(30, 30);
			this._searchButton.TabIndex = 4;
			this.toolTip1.SetToolTip(this._searchButton, "Search");
			this._searchButton.UseVisualStyleBackColor = false;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _id
			// 
			this._id.LabelText = "ID";
			this._id.Location = new System.Drawing.Point(0, 6);
			this._id.Margin = new System.Windows.Forms.Padding(2);
			this._id.Mask = "";
			this._id.Name = "_id";
			this._id.PasswordChar = '\0';
			this._id.Size = new System.Drawing.Size(152, 41);
			this._id.TabIndex = 0;
			this._id.ToolTip = null;
			this._id.Value = null;
			this._id.Leave += new System.EventHandler(this._field_Leave);
			this._id.Enter += new System.EventHandler(this._field_Enter);
			// 
			// _name
			// 
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(150, 6);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(150, 41);
			this._name.TabIndex = 1;
			this._name.ToolTip = null;
			this._name.Value = null;
			this._name.Leave += new System.EventHandler(this._field_Leave);
			this._name.Enter += new System.EventHandler(this._field_Enter);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._diagnosticServiceTableView, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(706, 497);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _diagnosticServiceTableView
			// 
			this._diagnosticServiceTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._diagnosticServiceTableView.Location = new System.Drawing.Point(4, 64);
			this._diagnosticServiceTableView.Margin = new System.Windows.Forms.Padding(4);
			this._diagnosticServiceTableView.Name = "_diagnosticServiceTableView";
			this._diagnosticServiceTableView.ReadOnly = false;
			this._diagnosticServiceTableView.Size = new System.Drawing.Size(698, 395);
			this._diagnosticServiceTableView.TabIndex = 1;
			this._diagnosticServiceTableView.ItemDoubleClicked += new System.EventHandler(this._diagnosticServiceTableView_ItemDoubleClicked);
			// 
			// DiagnosticServiceSummaryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "DiagnosticServiceSummaryComponentControl";
			this.Size = new System.Drawing.Size(706, 497);
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