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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CannedTextSummaryComponentControl));
			this._cannedTexts = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.SuspendLayout();
			// 
			// _cannedTexts
			// 
			resources.ApplyResources(this._cannedTexts, "_cannedTexts");
			this._cannedTexts.FilterTextBoxVisible = true;
			this._cannedTexts.Name = "_cannedTexts";
			this._cannedTexts.ReadOnly = false;
			this._cannedTexts.TabStop = false;
			this._cannedTexts.ItemDoubleClicked += new System.EventHandler(this._cannedTexts_ItemDoubleClicked);
			this._cannedTexts.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this._cannedTexts_ItemDrag);
			// 
			// CannedTextSummaryComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cannedTexts);
			this.Name = "CannedTextSummaryComponentControl";
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _cannedTexts;
    }
}
