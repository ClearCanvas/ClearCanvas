namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	partial class ReportingMppsDocumentationComponentControl
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
			this._mppsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._comments = new System.Windows.Forms.RichTextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _mppsTableView
			// 
			this._mppsTableView.ColumnHeaderTooltip = null;
			this._mppsTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._mppsTableView.Location = new System.Drawing.Point(0, 0);
			this._mppsTableView.Name = "_mppsTableView";
			this._mppsTableView.ReadOnly = false;
			this._mppsTableView.ShowToolbar = false;
			this._mppsTableView.Size = new System.Drawing.Size(483, 146);
			this._mppsTableView.SortButtonTooltip = null;
			this._mppsTableView.TabIndex = 2;
			this._mppsTableView.TabStop = false;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._mppsTableView);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._comments);
			this.splitContainer1.Size = new System.Drawing.Size(483, 379);
			this.splitContainer1.SplitterDistance = 146;
			this.splitContainer1.TabIndex = 3;
			// 
			// _comments
			// 
			this._comments.Dock = System.Windows.Forms.DockStyle.Fill;
			this._comments.Location = new System.Drawing.Point(0, 0);
			this._comments.Name = "_comments";
			this._comments.ReadOnly = true;
			this._comments.Size = new System.Drawing.Size(483, 229);
			this._comments.TabIndex = 0;
			this._comments.Text = "";
			// 
			// ReportingMppsDocumentationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "ReportingMppsDocumentationComponentControl";
			this.Size = new System.Drawing.Size(483, 379);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Desktop.View.WinForms.TableView _mppsTableView;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.RichTextBox _comments;
	}
}
