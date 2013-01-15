namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
	partial class LocationSummaryComponentControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._locationTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.panel1 = new System.Windows.Forms.Panel();
            this._facilityComboBox = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._clearButton = new System.Windows.Forms.Button();
            this._searchButton = new System.Windows.Forms.Button();
            this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._locationTableView, 0, 1);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(670, 488);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // _locationTableView
            // 
            this._locationTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._locationTableView.Location = new System.Drawing.Point(4, 64);
            this._locationTableView.Margin = new System.Windows.Forms.Padding(4);
            this._locationTableView.Name = "_locationTableView";
            this._locationTableView.ReadOnly = false;
            this._locationTableView.Size = new System.Drawing.Size(662, 386);
            this._locationTableView.TabIndex = 1;
            this._locationTableView.ItemDoubleClicked += new System.EventHandler(this._locationTableView_ItemDoubleClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._facilityComboBox);
            this.panel1.Controls.Add(this._clearButton);
            this.panel1.Controls.Add(this._searchButton);
            this.panel1.Controls.Add(this._name);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(661, 54);
            this.panel1.TabIndex = 0;
            // 
            // _facilityComboBox
            // 
            this._facilityComboBox.DataSource = null;
            this._facilityComboBox.DisplayMember = "";
            this._facilityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._facilityComboBox.LabelText = "Facility";
            this._facilityComboBox.Location = new System.Drawing.Point(2, 6);
            this._facilityComboBox.Margin = new System.Windows.Forms.Padding(2);
            this._facilityComboBox.Name = "_facilityComboBox";
            this._facilityComboBox.Size = new System.Drawing.Size(212, 39);
            this._facilityComboBox.TabIndex = 4;
            this._facilityComboBox.Value = null;
            this._facilityComboBox.Enter += new System.EventHandler(this._field_Enter);
            this._facilityComboBox.Leave += new System.EventHandler(this._field_Leave);
            // 
            // _clearButton
            // 
            this._clearButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._clearButton.BackColor = System.Drawing.Color.Transparent;
            this._clearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._clearButton.FlatAppearance.BorderSize = 0;
            this._clearButton.Image = global::ClearCanvas.Ris.Client.Admin.View.WinForms.SR.ClearFilterSmall;
            this._clearButton.Location = new System.Drawing.Point(394, 20);
            this._clearButton.Margin = new System.Windows.Forms.Padding(0);
            this._clearButton.Name = "_clearButton";
            this._clearButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._clearButton.Size = new System.Drawing.Size(30, 30);
            this._clearButton.TabIndex = 3;
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
            this._searchButton.Image = global::ClearCanvas.Ris.Client.Admin.View.WinForms.SR.SearchToolSmall;
            this._searchButton.Location = new System.Drawing.Point(363, 20);
            this._searchButton.Margin = new System.Windows.Forms.Padding(0);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(30, 30);
            this._searchButton.TabIndex = 2;
            this.toolTip1.SetToolTip(this._searchButton, "Search");
            this._searchButton.UseVisualStyleBackColor = false;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // _name
            // 
            this._name.LabelText = "Name";
            this._name.Location = new System.Drawing.Point(211, 7);
            this._name.Margin = new System.Windows.Forms.Padding(2);
            this._name.Mask = "";
            this._name.Name = "_name";
            this._name.PasswordChar = '\0';
            this._name.Size = new System.Drawing.Size(150, 41);
            this._name.TabIndex = 1;
            this._name.ToolTip = null;
            this._name.Value = null;
            this._name.Enter += new System.EventHandler(this._field_Enter);
            this._name.Leave += new System.EventHandler(this._field_Leave);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._okButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 456);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(666, 30);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(589, 2);
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
            this._okButton.Location = new System.Drawing.Point(510, 2);
            this._okButton.Margin = new System.Windows.Forms.Padding(2);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 0;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // LocationSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LocationSummaryComponentControl";
            this.Size = new System.Drawing.Size(670, 488);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private ClearCanvas.Desktop.View.WinForms.TableView _locationTableView;
		private System.Windows.Forms.Panel panel1;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _facilityComboBox;
		private System.Windows.Forms.Button _clearButton;
		private System.Windows.Forms.Button _searchButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}
