namespace ClearCanvas.Dicom.TestTools.TestScp
{
	partial class Form1
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._startStop = new System.Windows.Forms.Button();
			this._delayAssociationAccept = new System.Windows.Forms.RadioButton();
			this._rejectAssociation = new System.Windows.Forms.RadioButton();
			this._delayAssociationRelease = new System.Windows.Forms.RadioButton();
			this._doNothing = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// _startStop
			// 
			this._startStop.Location = new System.Drawing.Point(92, 170);
			this._startStop.Name = "_startStop";
			this._startStop.Size = new System.Drawing.Size(96, 23);
			this._startStop.TabIndex = 0;
			this._startStop.Text = "Start Listening";
			this._startStop.UseVisualStyleBackColor = true;
			this._startStop.Click += new System.EventHandler(this._startStop_Click);
			// 
			// _delayAssociationAccept
			// 
			this._delayAssociationAccept.AutoSize = true;
			this._delayAssociationAccept.Location = new System.Drawing.Point(41, 57);
			this._delayAssociationAccept.Name = "_delayAssociationAccept";
			this._delayAssociationAccept.Size = new System.Drawing.Size(189, 17);
			this._delayAssociationAccept.TabIndex = 1;
			this._delayAssociationAccept.Text = "Delay Association Accept (ARTIM)";
			this._delayAssociationAccept.UseVisualStyleBackColor = true;
			// 
			// _rejectAssociation
			// 
			this._rejectAssociation.AutoSize = true;
			this._rejectAssociation.Location = new System.Drawing.Point(41, 87);
			this._rejectAssociation.Name = "_rejectAssociation";
			this._rejectAssociation.Size = new System.Drawing.Size(113, 17);
			this._rejectAssociation.TabIndex = 2;
			this._rejectAssociation.Text = "Reject Association";
			this._rejectAssociation.UseVisualStyleBackColor = true;
			// 
			// _delayAssociationRelease
			// 
			this._delayAssociationRelease.AutoSize = true;
			this._delayAssociationRelease.Location = new System.Drawing.Point(41, 117);
			this._delayAssociationRelease.Name = "_delayAssociationRelease";
			this._delayAssociationRelease.Size = new System.Drawing.Size(194, 17);
			this._delayAssociationRelease.TabIndex = 3;
			this._delayAssociationRelease.Text = "Delay Association Release (ARTIM)";
			this._delayAssociationRelease.UseVisualStyleBackColor = true;
			// 
			// _doNothing
			// 
			this._doNothing.AutoSize = true;
			this._doNothing.Checked = true;
			this._doNothing.Location = new System.Drawing.Point(41, 27);
			this._doNothing.Name = "_doNothing";
			this._doNothing.Size = new System.Drawing.Size(117, 17);
			this._doNothing.TabIndex = 0;
			this._doNothing.TabStop = true;
			this._doNothing.Text = "Do Nothing Special";
			this._doNothing.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this._doNothing);
			this.Controls.Add(this._delayAssociationRelease);
			this.Controls.Add(this._rejectAssociation);
			this.Controls.Add(this._delayAssociationAccept);
			this.Controls.Add(this._startStop);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _startStop;
		private System.Windows.Forms.RadioButton _delayAssociationAccept;
		private System.Windows.Forms.RadioButton _rejectAssociation;
		private System.Windows.Forms.RadioButton _delayAssociationRelease;
		private System.Windows.Forms.RadioButton _doNothing;
	}
}