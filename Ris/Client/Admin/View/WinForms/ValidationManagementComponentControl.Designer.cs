namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class ValidationManagementComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValidationManagementComponentControl));
			this._appComponentTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.SuspendLayout();
			// 
			// _appComponentTableView
			// 
			resources.ApplyResources(this._appComponentTableView, "_appComponentTableView");
			this._appComponentTableView.MultiSelect = false;
			this._appComponentTableView.Name = "_appComponentTableView";
			this._appComponentTableView.ReadOnly = false;
			this._appComponentTableView.ItemDoubleClicked += new System.EventHandler(this._appComponentTableView_ItemDoubleClicked);
			// 
			// ValidationManagementComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._appComponentTableView);
			this.Name = "ValidationManagementComponentControl";
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _appComponentTableView;
    }
}
