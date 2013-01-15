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
			this._attachments = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.SuspendLayout();
			// 
			// _attachments
			// 
			this._attachments.ColumnHeaderTooltip = null;
			this._attachments.Dock = System.Windows.Forms.DockStyle.Fill;
			this._attachments.Location = new System.Drawing.Point(4, 2);
			this._attachments.MinimumSize = new System.Drawing.Size(0, 100);
			this._attachments.MultiSelect = false;
			this._attachments.Name = "_attachments";
			this._attachments.ReadOnly = false;
			this._attachments.Size = new System.Drawing.Size(475, 500);
			this._attachments.SortButtonTooltip = null;
			this._attachments.TabIndex = 0;
			this._attachments.ItemDoubleClicked += new System.EventHandler(this._attachments_ItemDoubleClicked);
			// 
			// AttachedDocumentPreviewComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this._attachments);
			this.Name = "AttachedDocumentPreviewComponentControl";
			this.Padding = new System.Windows.Forms.Padding(4, 2, 2, 7);
			this.Size = new System.Drawing.Size(481, 509);
			this.Load += new System.EventHandler(this.AttachedDocumentPreviewComponentControl_Load);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _attachments;
    }
}
