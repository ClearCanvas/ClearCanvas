namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class CannedTextSummaryComponentControl
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
			this._cannedTexts = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.SuspendLayout();
			// 
			// _cannedTexts
			// 
			this._cannedTexts.Dock = System.Windows.Forms.DockStyle.Fill;
			this._cannedTexts.FilterTextBoxVisible = true;
			this._cannedTexts.Location = new System.Drawing.Point(0, 0);
			this._cannedTexts.Name = "_cannedTexts";
			this._cannedTexts.ReadOnly = false;
			this._cannedTexts.Size = new System.Drawing.Size(785, 291);
			this._cannedTexts.TabIndex = 0;
			this._cannedTexts.TabStop = false;
			this._cannedTexts.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this._cannedTexts_ItemDrag);
			this._cannedTexts.ItemDoubleClicked += new System.EventHandler(this._cannedTexts_ItemDoubleClicked);
			// 
			// CannedTextSummaryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cannedTexts);
			this.Name = "CannedTextSummaryComponentControl";
			this.Size = new System.Drawing.Size(785, 291);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _cannedTexts;
    }
}
