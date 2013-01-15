namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	partial class ActionModelsToolComponentControl
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
			this._pnlHeader = new System.Windows.Forms.Panel();
			this._cboActionModel = new System.Windows.Forms.ComboBox();
			this._lblActionModel = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this._pnlHostedControl = new System.Windows.Forms.Panel();
			this._lyoButtons = new System.Windows.Forms.FlowLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this._pnlBottom = new System.Windows.Forms.Panel();
			this._pnlNote = new System.Windows.Forms.Panel();
			this._pnlHeader.SuspendLayout();
			this._lyoButtons.SuspendLayout();
			this._pnlBottom.SuspendLayout();
			this._pnlNote.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pnlHeader
			// 
			this._pnlHeader.Controls.Add(this._cboActionModel);
			this._pnlHeader.Controls.Add(this._lblActionModel);
			this._pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this._pnlHeader.Location = new System.Drawing.Point(0, 0);
			this._pnlHeader.Name = "_pnlHeader";
			this._pnlHeader.Padding = new System.Windows.Forms.Padding(5);
			this._pnlHeader.Size = new System.Drawing.Size(755, 33);
			this._pnlHeader.TabIndex = 0;
			// 
			// _cboActionModel
			// 
			this._cboActionModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cboActionModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboActionModel.FormattingEnabled = true;
			this._cboActionModel.Location = new System.Drawing.Point(86, 8);
			this._cboActionModel.Name = "_cboActionModel";
			this._cboActionModel.Size = new System.Drawing.Size(661, 21);
			this._cboActionModel.TabIndex = 3;
			this._cboActionModel.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// _lblActionModel
			// 
			this._lblActionModel.AutoSize = true;
			this._lblActionModel.Location = new System.Drawing.Point(8, 11);
			this._lblActionModel.Name = "_lblActionModel";
			this._lblActionModel.Size = new System.Drawing.Size(72, 13);
			this._lblActionModel.TabIndex = 0;
			this._lblActionModel.Text = "Action Model:";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(542, 3);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 2;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click_1);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(461, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Save";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// _pnlHostedControl
			// 
			this._pnlHostedControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._pnlHostedControl.Location = new System.Drawing.Point(0, 33);
			this._pnlHostedControl.Name = "_pnlHostedControl";
			this._pnlHostedControl.Size = new System.Drawing.Size(755, 379);
			this._pnlHostedControl.TabIndex = 0;
			// 
			// _lyoButtons
			// 
			this._lyoButtons.Controls.Add(this.button2);
			this._lyoButtons.Controls.Add(this.button1);
			this._lyoButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lyoButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this._lyoButtons.Location = new System.Drawing.Point(135, 0);
			this._lyoButtons.Name = "_lyoButtons";
			this._lyoButtons.Size = new System.Drawing.Size(620, 29);
			this._lyoButtons.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(30, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Note";
			this.label2.Visible = false;
			// 
			// _pnlBottom
			// 
			this._pnlBottom.Controls.Add(this._lyoButtons);
			this._pnlBottom.Controls.Add(this._pnlNote);
			this._pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._pnlBottom.Location = new System.Drawing.Point(0, 412);
			this._pnlBottom.Name = "_pnlBottom";
			this._pnlBottom.Size = new System.Drawing.Size(755, 29);
			this._pnlBottom.TabIndex = 2;
			// 
			// _pnlNote
			// 
			this._pnlNote.Controls.Add(this.label2);
			this._pnlNote.Dock = System.Windows.Forms.DockStyle.Left;
			this._pnlNote.Location = new System.Drawing.Point(0, 0);
			this._pnlNote.Name = "_pnlNote";
			this._pnlNote.Padding = new System.Windows.Forms.Padding(5);
			this._pnlNote.Size = new System.Drawing.Size(135, 29);
			this._pnlNote.TabIndex = 2;
			// 
			// ActionModelsToolComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._pnlHostedControl);
			this.Controls.Add(this._pnlBottom);
			this.Controls.Add(this._pnlHeader);
			this.Name = "ActionModelsToolComponentControl";
			this.Size = new System.Drawing.Size(755, 441);
			this._pnlHeader.ResumeLayout(false);
			this._pnlHeader.PerformLayout();
			this._lyoButtons.ResumeLayout(false);
			this._pnlBottom.ResumeLayout(false);
			this._pnlNote.ResumeLayout(false);
			this._pnlNote.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _pnlHeader;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label _lblActionModel;
		private System.Windows.Forms.Panel _pnlHostedControl;
		private System.Windows.Forms.ComboBox _cboActionModel;
		private System.Windows.Forms.FlowLayoutPanel _lyoButtons;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel _pnlBottom;
		private System.Windows.Forms.Panel _pnlNote;
	}
}
