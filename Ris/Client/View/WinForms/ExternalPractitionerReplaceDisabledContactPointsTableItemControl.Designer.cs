namespace ClearCanvas.Ris.Client.View.WinForms
{
	partial class ExternalPractitionerReplaceDisabledContactPointsTableItemControl
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
			this._replacedWith = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._oldContactPointInfo = new System.Windows.Forms.Label();
			this._newContactPointInfo = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._oldContactPointName = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _replacedWith
			// 
			this._replacedWith.BackColor = System.Drawing.Color.Transparent;
			this._replacedWith.DataSource = null;
			this._replacedWith.DisplayMember = "";
			this._replacedWith.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._replacedWith.LabelText = "Name:";
			this._replacedWith.Location = new System.Drawing.Point(5, 18);
			this._replacedWith.Margin = new System.Windows.Forms.Padding(2);
			this._replacedWith.Name = "_replacedWith";
			this._replacedWith.Size = new System.Drawing.Size(207, 40);
			this._replacedWith.TabIndex = 4;
			this._replacedWith.Value = null;
			// 
			// _oldContactPointInfo
			// 
			this._oldContactPointInfo.AutoSize = true;
			this._oldContactPointInfo.BackColor = System.Drawing.Color.Transparent;
			this._oldContactPointInfo.Location = new System.Drawing.Point(6, 63);
			this._oldContactPointInfo.Margin = new System.Windows.Forms.Padding(3);
			this._oldContactPointInfo.Name = "_oldContactPointInfo";
			this._oldContactPointInfo.Size = new System.Drawing.Size(111, 13);
			this._oldContactPointInfo.TabIndex = 5;
			this._oldContactPointInfo.Text = "Old Contact Point Info";
			// 
			// _newContactPointInfo
			// 
			this._newContactPointInfo.AutoSize = true;
			this._newContactPointInfo.BackColor = System.Drawing.Color.Transparent;
			this._newContactPointInfo.Location = new System.Drawing.Point(6, 63);
			this._newContactPointInfo.Margin = new System.Windows.Forms.Padding(3);
			this._newContactPointInfo.Name = "_newContactPointInfo";
			this._newContactPointInfo.Size = new System.Drawing.Size(117, 13);
			this._newContactPointInfo.TabIndex = 6;
			this._newContactPointInfo.Text = "New Contact Point Info";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(738, 169);
			this.tableLayoutPanel1.TabIndex = 9;
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.BackColor = System.Drawing.Color.Transparent;
			this.groupBox1.Controls.Add(this._oldContactPointName);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this._oldContactPointInfo);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(363, 163);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Deactivated Contact Point";
			// 
			// _oldContactPointName
			// 
			this._oldContactPointName.AutoSize = true;
			this._oldContactPointName.BackColor = System.Drawing.Color.Transparent;
			this._oldContactPointName.Location = new System.Drawing.Point(6, 42);
			this._oldContactPointName.Name = "_oldContactPointName";
			this._oldContactPointName.Size = new System.Drawing.Size(121, 13);
			this._oldContactPointName.TabIndex = 7;
			this._oldContactPointName.Text = "Old Contact Point Name";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(6, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Name:";
			// 
			// groupBox2
			// 
			this.groupBox2.AutoSize = true;
			this.groupBox2.BackColor = System.Drawing.Color.Transparent;
			this.groupBox2.Controls.Add(this._replacedWith);
			this.groupBox2.Controls.Add(this._newContactPointInfo);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(372, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(363, 163);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Replacement Contact Point";
			// 
			// ExternalPractitionerReplaceDisabledContactPointsTableItemControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "ExternalPractitionerReplaceDisabledContactPointsTableItemControl";
			this.Size = new System.Drawing.Size(738, 169);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _replacedWith;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label _oldContactPointInfo;
		private System.Windows.Forms.Label _newContactPointInfo;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label _oldContactPointName;
		private System.Windows.Forms.Label label1;
	}
}
