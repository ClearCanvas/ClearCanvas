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
			resources.ApplyResources(this._searchField, "_searchField");
			this._searchField.Mask = "";
			this._searchField.Name = "_searchField";
			this._searchField.Value = null;
			// 
			// _searchResults
			// 
			resources.ApplyResources(this._searchResults, "_searchResults");
			this.tableLayoutPanel1.SetColumnSpan(this._searchResults, 2);
			this._searchResults.Name = "_searchResults";
			this._searchResults.ReadOnly = false;
			this._searchResults.ItemDoubleClicked += new System.EventHandler(this._searchResults_ItemDoubleClicked);
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._searchResults, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._searchField, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._searchButton);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _searchButton
			// 
			resources.ApplyResources(this._searchButton, "_searchButton");
			this._searchButton.Name = "_searchButton";
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// PatientSearchComponentControl
			// 
			this.AcceptButton = this._searchButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "PatientSearchComponentControl";
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
