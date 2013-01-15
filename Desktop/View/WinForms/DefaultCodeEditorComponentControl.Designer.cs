namespace ClearCanvas.Desktop.View.WinForms
{
    partial class DefaultCodeEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefaultCodeEditorComponentControl));
			this._editor = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// _editor
			// 
			this._editor.AcceptsTab = true;
			this._editor.AutoWordSelection = true;
			this._editor.DetectUrls = false;
			resources.ApplyResources(this._editor, "_editor");
			this._editor.Name = "_editor";
			// 
			// DefaultCodeEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._editor);
			this.Name = "DefaultCodeEditorComponentControl";
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox _editor;
    }
}
