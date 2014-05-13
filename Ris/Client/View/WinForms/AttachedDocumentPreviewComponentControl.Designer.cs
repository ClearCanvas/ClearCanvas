namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class AttachedDocumentPreviewComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttachedDocumentPreviewComponentControl));
			this._attachments = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.SuspendLayout();
			// 
			// _attachments
			// 
			resources.ApplyResources(this._attachments, "_attachments");
			this._attachments.MultiSelect = false;
			this._attachments.Name = "_attachments";
			this._attachments.ReadOnly = false;
			this._attachments.ItemDoubleClicked += new System.EventHandler(this._attachments_ItemDoubleClicked);
			// 
			// AttachedDocumentPreviewComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._attachments);
			this.Name = "AttachedDocumentPreviewComponentControl";
			this.Load += new System.EventHandler(this.AttachedDocumentPreviewComponentControl_Load);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _attachments;
    }
}
