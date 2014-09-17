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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExternalPractitionerReplaceDisabledContactPointsTableItemControl));
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
			resources.ApplyResources(this._replacedWith, "_replacedWith");
			this._replacedWith.Name = "_replacedWith";
			this._replacedWith.Value = null;
			// 
			// _oldContactPointInfo
			// 
			resources.ApplyResources(this._oldContactPointInfo, "_oldContactPointInfo");
			this._oldContactPointInfo.BackColor = System.Drawing.Color.Transparent;
			this._oldContactPointInfo.Name = "_oldContactPointInfo";
			// 
			// _newContactPointInfo
			// 
			resources.ApplyResources(this._newContactPointInfo, "_newContactPointInfo");
			this._newContactPointInfo.BackColor = System.Drawing.Color.Transparent;
			this._newContactPointInfo.Name = "_newContactPointInfo";
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.BackColor = System.Drawing.Color.Transparent;
			this.groupBox1.Controls.Add(this._oldContactPointName);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this._oldContactPointInfo);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// _oldContactPointName
			// 
			resources.ApplyResources(this._oldContactPointName, "_oldContactPointName");
			this._oldContactPointName.BackColor = System.Drawing.Color.Transparent;
			this._oldContactPointName.Name = "_oldContactPointName";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Name = "label1";
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.BackColor = System.Drawing.Color.Transparent;
			this.groupBox2.Controls.Add(this._replacedWith);
			this.groupBox2.Controls.Add(this._newContactPointInfo);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// ExternalPractitionerReplaceDisabledContactPointsTableItemControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExternalPractitionerReplaceDisabledContactPointsTableItemControl";
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
