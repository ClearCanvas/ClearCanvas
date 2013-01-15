namespace ClearCanvas.Desktop.View.WinForms
{
	partial class RichTextField
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RichTextField));
			this._richTextBox = new System.Windows.Forms.RichTextBox();
			this._label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _richTextBox
			// 
			resources.ApplyResources(this._richTextBox, "_richTextBox");
			this._richTextBox.Name = "_richTextBox";
			// 
			// _label
			// 
			resources.ApplyResources(this._label, "_label");
			this._label.Name = "_label";
			// 
			// RichTextField
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._label);
			this.Controls.Add(this._richTextBox);
			this.Name = "RichTextField";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this._richTextBox_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this._richTextBox_DragEnter);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox _richTextBox;
		private System.Windows.Forms.Label _label;
	}
}
