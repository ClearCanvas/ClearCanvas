namespace ClearCanvas.ImageServer.TestApp
{
	partial class ArchiveTestForm
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
			this._studyListGridView = new System.Windows.Forms.DataGridView();
			this.label2 = new System.Windows.Forms.Label();
			this._accessionTextBox = new System.Windows.Forms.TextBox();
			this._queryButton = new System.Windows.Forms.Button();
			this._purgeStudyBtn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._studyListGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// _studyListGridView
			// 
			this._studyListGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this._studyListGridView.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._studyListGridView.Location = new System.Drawing.Point(0, 89);
			this._studyListGridView.Name = "_studyListGridView";
			this._studyListGridView.ReadOnly = true;
			this._studyListGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this._studyListGridView.Size = new System.Drawing.Size(1051, 465);
			this._studyListGridView.TabIndex = 0;
			this._studyListGridView.SelectionChanged += new System.EventHandler(this._studyListGridView_SelectionChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Accession #";
			// 
			// _accessionTextBox
			// 
			this._accessionTextBox.Location = new System.Drawing.Point(15, 34);
			this._accessionTextBox.Name = "_accessionTextBox";
			this._accessionTextBox.Size = new System.Drawing.Size(147, 20);
			this._accessionTextBox.TabIndex = 4;
			// 
			// _queryButton
			// 
			this._queryButton.Location = new System.Drawing.Point(191, 31);
			this._queryButton.Name = "_queryButton";
			this._queryButton.Size = new System.Drawing.Size(75, 23);
			this._queryButton.TabIndex = 5;
			this._queryButton.Text = "Query";
			this._queryButton.UseVisualStyleBackColor = true;
			this._queryButton.Click += new System.EventHandler(this._queryButton_Click);
			// 
			// _purgeStudyBtn
			// 
			this._purgeStudyBtn.Location = new System.Drawing.Point(298, 32);
			this._purgeStudyBtn.Name = "_purgeStudyBtn";
			this._purgeStudyBtn.Size = new System.Drawing.Size(118, 23);
			this._purgeStudyBtn.TabIndex = 6;
			this._purgeStudyBtn.Text = "Purge Selected";
			this._purgeStudyBtn.UseVisualStyleBackColor = true;
			this._purgeStudyBtn.Click += new System.EventHandler(this._purgeStudyBtn_Click);
			// 
			// ArchiveTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1051, 554);
			this.Controls.Add(this._purgeStudyBtn);
			this.Controls.Add(this._queryButton);
			this.Controls.Add(this._accessionTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._studyListGridView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "ArchiveTestForm";
			this.Text = "Archive Test";
			((System.ComponentModel.ISupportInitialize)(this._studyListGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView _studyListGridView;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _accessionTextBox;
		private System.Windows.Forms.Button _queryButton;
		private System.Windows.Forms.Button _purgeStudyBtn;
	}
}