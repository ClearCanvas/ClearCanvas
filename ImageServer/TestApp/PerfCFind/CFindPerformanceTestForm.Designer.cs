namespace ClearCanvas.ImageServer.TestApp.PerfCFind
{
	partial class CFindPerformanceTestForm
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
			this._hostnameTextbox = new System.Windows.Forms.TextBox();
			this._aeTitleTextbox = new System.Windows.Forms.TextBox();
			this._calledAETitleTextbox = new System.Windows.Forms.TextBox();
			this._portTextbox = new System.Windows.Forms.TextBox();
			this.Hostname = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._queryButton = new System.Windows.Forms.Button();
			this._queryResultGridView = new System.Windows.Forms.DataGridView();
			this.label4 = new System.Windows.Forms.Label();
			this._studyDateTextBox = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this._queryResultGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// _hostnameTextbox
			// 
			this._hostnameTextbox.Location = new System.Drawing.Point(55, 32);
			this._hostnameTextbox.Name = "_hostnameTextbox";
			this._hostnameTextbox.Size = new System.Drawing.Size(100, 20);
			this._hostnameTextbox.TabIndex = 0;
			this._hostnameTextbox.Text = "PROCESS3";
			// 
			// _aeTitleTextbox
			// 
			this._aeTitleTextbox.Location = new System.Drawing.Point(179, 32);
			this._aeTitleTextbox.Name = "_aeTitleTextbox";
			this._aeTitleTextbox.Size = new System.Drawing.Size(100, 20);
			this._aeTitleTextbox.TabIndex = 1;
			this._aeTitleTextbox.Text = "THANH-W8";
			// 
			// _calledAETitleTextbox
			// 
			this._calledAETitleTextbox.Location = new System.Drawing.Point(299, 32);
			this._calledAETitleTextbox.Name = "_calledAETitleTextbox";
			this._calledAETitleTextbox.Size = new System.Drawing.Size(100, 20);
			this._calledAETitleTextbox.TabIndex = 2;
			this._calledAETitleTextbox.Text = "PACS1";
			// 
			// _portTextbox
			// 
			this._portTextbox.Location = new System.Drawing.Point(421, 32);
			this._portTextbox.Name = "_portTextbox";
			this._portTextbox.Size = new System.Drawing.Size(100, 20);
			this._portTextbox.TabIndex = 3;
			this._portTextbox.Text = "1234";
			// 
			// Hostname
			// 
			this.Hostname.AutoSize = true;
			this.Hostname.Location = new System.Drawing.Point(55, 13);
			this.Hostname.Name = "Hostname";
			this.Hostname.Size = new System.Drawing.Size(55, 13);
			this.Hostname.TabIndex = 1;
			this.Hostname.Text = "Hostname";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(176, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Client AE Title";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(296, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(78, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Server AE Title";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(418, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Server Port";
			// 
			// _queryButton
			// 
			this._queryButton.Location = new System.Drawing.Point(691, 29);
			this._queryButton.Name = "_queryButton";
			this._queryButton.Size = new System.Drawing.Size(75, 23);
			this._queryButton.TabIndex = 5;
			this._queryButton.Text = "Query";
			this._queryButton.UseVisualStyleBackColor = true;
			this._queryButton.Click += new System.EventHandler(this._queryButton_Click);
			// 
			// _queryResultGridView
			// 
			this._queryResultGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this._queryResultGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this._queryResultGridView.Location = new System.Drawing.Point(12, 74);
			this._queryResultGridView.Name = "_queryResultGridView";
			this._queryResultGridView.Size = new System.Drawing.Size(754, 359);
			this._queryResultGridView.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(557, 12);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "Study Date (from)";
			// 
			// _studyDateTextBox
			// 
			this._studyDateTextBox.Location = new System.Drawing.Point(560, 31);
			this._studyDateTextBox.Name = "_studyDateTextBox";
			this._studyDateTextBox.Size = new System.Drawing.Size(108, 20);
			this._studyDateTextBox.TabIndex = 4;
			// 
			// CFindPerformanceTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(778, 445);
			this.Controls.Add(this._studyDateTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._queryResultGridView);
			this.Controls.Add(this._queryButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Hostname);
			this.Controls.Add(this._portTextbox);
			this.Controls.Add(this._calledAETitleTextbox);
			this.Controls.Add(this._aeTitleTextbox);
			this.Controls.Add(this._hostnameTextbox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "CFindPerformanceTestForm";
			this.Text = "CFindPerformanceTestForm";
			((System.ComponentModel.ISupportInitialize)(this._queryResultGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _hostnameTextbox;
		private System.Windows.Forms.TextBox _aeTitleTextbox;
		private System.Windows.Forms.TextBox _calledAETitleTextbox;
		private System.Windows.Forms.TextBox _portTextbox;
		private System.Windows.Forms.Label Hostname;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button _queryButton;
		private System.Windows.Forms.DataGridView _queryResultGridView;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox _studyDateTextBox;
	}
}