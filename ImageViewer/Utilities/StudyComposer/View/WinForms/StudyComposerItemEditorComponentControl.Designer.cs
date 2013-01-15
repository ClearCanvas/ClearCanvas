namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.View.WinForms {
	partial class StudyComposerItemEditorComponentControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.pnlMain = new System.Windows.Forms.Panel();
			this.pgvProps = new System.Windows.Forms.PropertyGrid();
			this.pnlHeader = new System.Windows.Forms.Panel();
			this.pnlInfo = new System.Windows.Forms.Panel();
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblName = new System.Windows.Forms.Label();
			this.picIcon = new System.Windows.Forms.PictureBox();
			this.lyoButtons = new System.Windows.Forms.FlowLayoutPanel();
			this.btnApply = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.pnlMain.SuspendLayout();
			this.pnlHeader.SuspendLayout();
			this.pnlInfo.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
			this.lyoButtons.SuspendLayout();
			this.pnlBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlMain
			// 
			this.pnlMain.Controls.Add(this.pgvProps);
			this.pnlMain.Controls.Add(this.pnlHeader);
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 0);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Padding = new System.Windows.Forms.Padding(5);
			this.pnlMain.Size = new System.Drawing.Size(381, 443);
			this.pnlMain.TabIndex = 0;
			// 
			// pgvProps
			// 
			this.pgvProps.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgvProps.Location = new System.Drawing.Point(5, 57);
			this.pgvProps.Name = "pgvProps";
			this.pgvProps.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.pgvProps.Size = new System.Drawing.Size(371, 381);
			this.pgvProps.TabIndex = 1;
			this.pgvProps.ToolbarVisible = false;
			// 
			// pnlHeader
			// 
			this.pnlHeader.Controls.Add(this.pnlInfo);
			this.pnlHeader.Controls.Add(this.picIcon);
			this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlHeader.Location = new System.Drawing.Point(5, 5);
			this.pnlHeader.Name = "pnlHeader";
			this.pnlHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.pnlHeader.Size = new System.Drawing.Size(371, 52);
			this.pnlHeader.TabIndex = 0;
			// 
			// pnlInfo
			// 
			this.pnlInfo.Controls.Add(this.lblDescription);
			this.pnlInfo.Controls.Add(this.lblName);
			this.pnlInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlInfo.Location = new System.Drawing.Point(64, 0);
			this.pnlInfo.Name = "pnlInfo";
			this.pnlInfo.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.pnlInfo.Size = new System.Drawing.Size(307, 47);
			this.pnlInfo.TabIndex = 0;
			// 
			// lblDescription
			// 
			this.lblDescription.AutoEllipsis = true;
			this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblDescription.Location = new System.Drawing.Point(5, 13);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(302, 34);
			this.lblDescription.TabIndex = 1;
			this.lblDescription.Text = "Description";
			// 
			// lblName
			// 
			this.lblName.AutoEllipsis = true;
			this.lblName.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblName.Location = new System.Drawing.Point(5, 0);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(302, 13);
			this.lblName.TabIndex = 0;
			this.lblName.Text = "Titley";
			// 
			// picIcon
			// 
			this.picIcon.Dock = System.Windows.Forms.DockStyle.Left;
			this.picIcon.Location = new System.Drawing.Point(0, 0);
			this.picIcon.Name = "picIcon";
			this.picIcon.Size = new System.Drawing.Size(64, 47);
			this.picIcon.TabIndex = 1;
			this.picIcon.TabStop = false;
			// 
			// lyoButtons
			// 
			this.lyoButtons.Controls.Add(this.btnApply);
			this.lyoButtons.Controls.Add(this.btnCancel);
			this.lyoButtons.Controls.Add(this.btnOk);
			this.lyoButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lyoButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.lyoButtons.Location = new System.Drawing.Point(5, 5);
			this.lyoButtons.Name = "lyoButtons";
			this.lyoButtons.Size = new System.Drawing.Size(371, 29);
			this.lyoButtons.TabIndex = 0;
			// 
			// btnApply
			// 
			this.btnApply.Location = new System.Drawing.Point(293, 3);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(75, 23);
			this.btnApply.TabIndex = 2;
			this.btnApply.Text = "&Apply";
			this.btnApply.UseVisualStyleBackColor = true;
			this.btnApply.Visible = false;
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(212, 3);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Visible = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(131, 3);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "&Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.lyoButtons);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 443);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Padding = new System.Windows.Forms.Padding(5);
			this.pnlBottom.Size = new System.Drawing.Size(381, 39);
			this.pnlBottom.TabIndex = 1;
			// 
			// StudyComposerItemEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlMain);
			this.Controls.Add(this.pnlBottom);
			this.Name = "StudyComposerItemEditorComponentControl";
			this.Size = new System.Drawing.Size(381, 482);
			this.pnlMain.ResumeLayout(false);
			this.pnlHeader.ResumeLayout(false);
			this.pnlInfo.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
			this.lyoButtons.ResumeLayout(false);
			this.pnlBottom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlMain;
		private System.Windows.Forms.FlowLayoutPanel lyoButtons;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlHeader;
		private System.Windows.Forms.PictureBox picIcon;
		private System.Windows.Forms.Panel pnlInfo;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.PropertyGrid pgvProps;
	}
}
