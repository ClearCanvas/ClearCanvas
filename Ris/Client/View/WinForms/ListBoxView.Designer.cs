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
			this._listBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listBox.FormattingEnabled = true;
			this._listBox.Location = new System.Drawing.Point(3, 28);
			this._listBox.Name = "_listBox";
			this._listBox.Size = new System.Drawing.Size(343, 225);
			this._listBox.TabIndex = 1;
			// 
			// _contextMenu
			// 
			this._contextMenu.Name = "_contextMenu";
			this._contextMenu.Size = new System.Drawing.Size(61, 4);
			this._contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._contextMenu_Opening);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._listBox, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._toolStrip, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(349, 267);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// _toolStrip
			// 
			this._toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._toolStrip.Location = new System.Drawing.Point(0, 0);
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.Size = new System.Drawing.Size(349, 25);
			this._toolStrip.TabIndex = 2;
			this._toolStrip.Text = "toolStrip1";
			// 
			// ListBoxView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ListBoxView";
			this.Size = new System.Drawing.Size(349, 267);
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
