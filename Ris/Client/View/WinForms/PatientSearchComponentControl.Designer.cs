namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class PatientSearchComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatientSearchComponentControl));
			this._searchField = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._searchResults = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this._searchButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _searchField
			// 
			this._searchField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._searchField.LabelText = "Enter Patient Search Criteria:";
			this._searchField.Location = new System.Drawing.Point(2, 2);
			this._searchField.Margin = new System.Windows.Forms.Padding(2);
			this._searchField.Mask = "";
			this._searchField.Name = "_searchField";
			this._searchField.PasswordChar = '\0';
			this._searchField.Size = new System.Drawing.Size(333, 41);
			this._searchField.TabIndex = 0;
			this._searchField.ToolTip = "Enter search criteria (MRN#, Name, or Healthcard ID)";
			this._searchField.Value = null;
			// 
			// _searchResults
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._searchResults, 2);
			this._searchResults.Dock = System.Windows.Forms.DockStyle.Fill;
			this._searchResults.Location = new System.Drawing.Point(3, 53);
			this._searchResults.Name = "_searchResults";
			this._searchResults.ReadOnly = false;
			this._searchResults.Size = new System.Drawing.Size(361, 234);
			this._searchResults.TabIndex = 2;
			this._searchResults.ItemDoubleClicked += new System.EventHandler(this._searchResults_ItemDoubleClicked);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.Controls.Add(this._searchResults, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._searchField, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(367, 290);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._searchButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(337, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(30, 50);
			this.panel1.TabIndex = 1;
			// 
			// _searchButton
			// 
			this._searchButton.Image = ((System.Drawing.Image)(resources.GetObject("_searchButton.Image")));
			this._searchButton.Location = new System.Drawing.Point(4, 17);
			this._searchButton.Margin = new System.Windows.Forms.Padding(2);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(24, 24);
			this._searchButton.TabIndex = 0;
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// PatientSearchComponentControl
			// 
			this.AcceptButton = this._searchButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "PatientSearchComponentControl";
			this.Size = new System.Drawing.Size(367, 290);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _searchField;
        private ClearCanvas.Desktop.View.WinForms.TableView _searchResults;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button _searchButton;
    }
}
