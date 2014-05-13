namespace ClearCanvas.Ris.Client.View.WinForms
{
	partial class ListBoxView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListBoxView));
			this._listBox = new ClearCanvas.Ris.Client.View.WinForms.ListBoxWithDragSupport();
			this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _listBox
			// 
			this._listBox.AllowDrop = true;
			this._listBox.BackColor = System.Drawing.SystemColors.Window;
			this._listBox.ContextMenuStrip = this._contextMenu;
			resources.ApplyResources(this._listBox, "_listBox");
			this._listBox.FormattingEnabled = true;
			this._listBox.Name = "_listBox";
			// 
			// _contextMenu
			// 
			this._contextMenu.Name = "_contextMenu";
			resources.ApplyResources(this._contextMenu, "_contextMenu");
			this._contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._contextMenu_Opening);
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._listBox, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._toolStrip, 0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _toolStrip
			// 
			this._toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			resources.ApplyResources(this._toolStrip, "_toolStrip");
			this._toolStrip.Name = "_toolStrip";
			// 
			// ListBoxView
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ListBoxView";
			this.Load += new System.EventHandler(this.ListBoxView_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ListBoxWithDragSupport _listBox;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ToolStrip _toolStrip;
		private System.Windows.Forms.ContextMenuStrip _contextMenu;
	}
}
