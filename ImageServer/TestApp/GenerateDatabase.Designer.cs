namespace ClearCanvas.ImageServer.TestApp
{
    partial class GenerateDatabase
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
            this._dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._textBoxStudiesPerDay = new System.Windows.Forms.TextBox();
            this._numericUpDownPercentWeekend = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new CheckableGroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this._textBoxPort = new System.Windows.Forms.TextBox();
            this._textBoxHost = new System.Windows.Forms.TextBox();
            this._textBoxRemoteAETitle = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this._checkBoxImageServerDatabase = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this._comboBoxServerPartition = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this._textBoxTotalStudies = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._checkBoxRF = new System.Windows.Forms.CheckBox();
            this._checkBoxMG = new System.Windows.Forms.CheckBox();
            this._checkBoxXA = new System.Windows.Forms.CheckBox();
            this._checkBoxUS = new System.Windows.Forms.CheckBox();
            this._checkBoxDX = new System.Windows.Forms.CheckBox();
            this._checkBoxCR = new System.Windows.Forms.CheckBox();
            this._checkBoxMR = new System.Windows.Forms.CheckBox();
            this._checkBoxCT = new System.Windows.Forms.CheckBox();
            this._buttonGenerate = new System.Windows.Forms.Button();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._numericUpDownPercentWeekend)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dateTimePickerStart
            // 
            this._dateTimePickerStart.Location = new System.Drawing.Point(6, 32);
            this._dateTimePickerStart.Name = "_dateTimePickerStart";
            this._dateTimePickerStart.Size = new System.Drawing.Size(200, 20);
            this._dateTimePickerStart.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Study Date Start";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Studies Per Day";
            // 
            // _textBoxStudiesPerDay
            // 
            this._textBoxStudiesPerDay.Location = new System.Drawing.Point(6, 76);
            this._textBoxStudiesPerDay.Name = "_textBoxStudiesPerDay";
            this._textBoxStudiesPerDay.Size = new System.Drawing.Size(200, 20);
            this._textBoxStudiesPerDay.TabIndex = 3;
            this._textBoxStudiesPerDay.Text = "1800";
            // 
            // _numericUpDownPercentWeekend
            // 
            this._numericUpDownPercentWeekend.Location = new System.Drawing.Point(265, 32);
            this._numericUpDownPercentWeekend.Name = "_numericUpDownPercentWeekend";
            this._numericUpDownPercentWeekend.Size = new System.Drawing.Size(143, 20);
            this._numericUpDownPercentWeekend.TabIndex = 4;
            this._numericUpDownPercentWeekend.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(262, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Weekend Percent Reduction";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this._textBoxPort);
            this.groupBox1.Controls.Add(this._textBoxHost);
            this.groupBox1.Controls.Add(this._textBoxRemoteAETitle);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this._checkBoxImageServerDatabase);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this._comboBoxServerPartition);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this._textBoxTotalStudies);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this._numericUpDownPercentWeekend);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this._dateTimePickerStart);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this._textBoxStudiesPerDay);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(503, 251);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(268, 198);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Port";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(268, 152);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Host";
            // 
            // _textBoxPort
            // 
            this._textBoxPort.Location = new System.Drawing.Point(268, 218);
            this._textBoxPort.Name = "_textBoxPort";
            this._textBoxPort.Size = new System.Drawing.Size(140, 20);
            this._textBoxPort.TabIndex = 14;
            this._textBoxPort.Text = "11112";
            // 
            // _textBoxHost
            // 
            this._textBoxHost.Location = new System.Drawing.Point(268, 168);
            this._textBoxHost.Name = "_textBoxHost";
            this._textBoxHost.Size = new System.Drawing.Size(140, 20);
            this._textBoxHost.TabIndex = 13;
            this._textBoxHost.Text = "localhost";
            // 
            // _textBoxRemoteAETitle
            // 
            this._textBoxRemoteAETitle.Location = new System.Drawing.Point(268, 122);
            this._textBoxRemoteAETitle.Name = "_textBoxRemoteAETitle";
            this._textBoxRemoteAETitle.Size = new System.Drawing.Size(140, 20);
            this._textBoxRemoteAETitle.TabIndex = 12;
            this._textBoxRemoteAETitle.Text = "STEVE_SHOW";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(265, 105);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Remote AE Title";
            // 
            // _checkBoxImageServerDatabase
            // 
            this._checkBoxImageServerDatabase.AutoSize = true;
            this._checkBoxImageServerDatabase.Location = new System.Drawing.Point(9, 113);
            this._checkBoxImageServerDatabase.Name = "_checkBoxImageServerDatabase";
            this._checkBoxImageServerDatabase.Size = new System.Drawing.Size(150, 17);
            this._checkBoxImageServerDatabase.TabIndex = 10;
            this._checkBoxImageServerDatabase.Text = "Insert Directly in Database";
            this._checkBoxImageServerDatabase.UseVisualStyleBackColor = true;
            this._checkBoxImageServerDatabase.CheckedChanged += new System.EventHandler(this._checkBoxImageServerDatabase_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Server Partition";
            // 
            // _comboBoxServerPartition
            // 
            this._comboBoxServerPartition.FormattingEnabled = true;
            this._comboBoxServerPartition.Location = new System.Drawing.Point(9, 158);
            this._comboBoxServerPartition.Name = "_comboBoxServerPartition";
            this._comboBoxServerPartition.Size = new System.Drawing.Size(197, 21);
            this._comboBoxServerPartition.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(262, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Total Studies";
            // 
            // _textBoxTotalStudies
            // 
            this._textBoxTotalStudies.Location = new System.Drawing.Point(265, 75);
            this._textBoxTotalStudies.Name = "_textBoxTotalStudies";
            this._textBoxTotalStudies.Size = new System.Drawing.Size(143, 20);
            this._textBoxTotalStudies.TabIndex = 6;
            this._textBoxTotalStudies.Text = "50000";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._checkBoxRF);
            this.groupBox2.Controls.Add(this._checkBoxMG);
            this.groupBox2.Controls.Add(this._checkBoxXA);
            this.groupBox2.Controls.Add(this._checkBoxUS);
            this.groupBox2.Controls.Add(this._checkBoxDX);
            this.groupBox2.Controls.Add(this._checkBoxCR);
            this.groupBox2.Controls.Add(this._checkBoxMR);
            this.groupBox2.Controls.Add(this._checkBoxCT);
            this.groupBox2.Location = new System.Drawing.Point(12, 279);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(502, 93);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Modalities";
            // 
            // _checkBoxRF
            // 
            this._checkBoxRF.AutoSize = true;
            this._checkBoxRF.Checked = true;
            this._checkBoxRF.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxRF.Location = new System.Drawing.Point(71, 20);
            this._checkBoxRF.Name = "_checkBoxRF";
            this._checkBoxRF.Size = new System.Drawing.Size(40, 17);
            this._checkBoxRF.TabIndex = 7;
            this._checkBoxRF.Text = "RF";
            this._checkBoxRF.UseVisualStyleBackColor = true;
            // 
            // _checkBoxMG
            // 
            this._checkBoxMG.AutoSize = true;
            this._checkBoxMG.Checked = true;
            this._checkBoxMG.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxMG.Location = new System.Drawing.Point(201, 20);
            this._checkBoxMG.Name = "_checkBoxMG";
            this._checkBoxMG.Size = new System.Drawing.Size(43, 17);
            this._checkBoxMG.TabIndex = 6;
            this._checkBoxMG.Text = "MG";
            this._checkBoxMG.UseVisualStyleBackColor = true;
            // 
            // _checkBoxXA
            // 
            this._checkBoxXA.AutoSize = true;
            this._checkBoxXA.Checked = true;
            this._checkBoxXA.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxXA.Location = new System.Drawing.Point(71, 44);
            this._checkBoxXA.Name = "_checkBoxXA";
            this._checkBoxXA.Size = new System.Drawing.Size(40, 17);
            this._checkBoxXA.TabIndex = 5;
            this._checkBoxXA.Text = "XA";
            this._checkBoxXA.UseVisualStyleBackColor = true;
            // 
            // _checkBoxUS
            // 
            this._checkBoxUS.AutoSize = true;
            this._checkBoxUS.Checked = true;
            this._checkBoxUS.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxUS.Location = new System.Drawing.Point(201, 44);
            this._checkBoxUS.Name = "_checkBoxUS";
            this._checkBoxUS.Size = new System.Drawing.Size(41, 17);
            this._checkBoxUS.TabIndex = 4;
            this._checkBoxUS.Text = "US";
            this._checkBoxUS.UseVisualStyleBackColor = true;
            // 
            // _checkBoxDX
            // 
            this._checkBoxDX.AutoSize = true;
            this._checkBoxDX.Checked = true;
            this._checkBoxDX.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxDX.Location = new System.Drawing.Point(136, 20);
            this._checkBoxDX.Name = "_checkBoxDX";
            this._checkBoxDX.Size = new System.Drawing.Size(41, 17);
            this._checkBoxDX.TabIndex = 3;
            this._checkBoxDX.Text = "DX";
            this._checkBoxDX.UseVisualStyleBackColor = true;
            // 
            // _checkBoxCR
            // 
            this._checkBoxCR.AutoSize = true;
            this._checkBoxCR.Checked = true;
            this._checkBoxCR.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxCR.Location = new System.Drawing.Point(136, 44);
            this._checkBoxCR.Name = "_checkBoxCR";
            this._checkBoxCR.Size = new System.Drawing.Size(41, 17);
            this._checkBoxCR.TabIndex = 2;
            this._checkBoxCR.Text = "CR";
            this._checkBoxCR.UseVisualStyleBackColor = true;
            // 
            // _checkBoxMR
            // 
            this._checkBoxMR.AutoSize = true;
            this._checkBoxMR.Checked = true;
            this._checkBoxMR.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxMR.Location = new System.Drawing.Point(7, 44);
            this._checkBoxMR.Name = "_checkBoxMR";
            this._checkBoxMR.Size = new System.Drawing.Size(43, 17);
            this._checkBoxMR.TabIndex = 1;
            this._checkBoxMR.Text = "MR";
            this._checkBoxMR.UseVisualStyleBackColor = true;
            // 
            // _checkBoxCT
            // 
            this._checkBoxCT.AutoSize = true;
            this._checkBoxCT.Checked = true;
            this._checkBoxCT.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxCT.Location = new System.Drawing.Point(7, 20);
            this._checkBoxCT.Name = "_checkBoxCT";
            this._checkBoxCT.Size = new System.Drawing.Size(40, 17);
            this._checkBoxCT.TabIndex = 0;
            this._checkBoxCT.Text = "CT";
            this._checkBoxCT.UseVisualStyleBackColor = true;
            // 
            // _buttonGenerate
            // 
            this._buttonGenerate.Location = new System.Drawing.Point(358, 378);
            this._buttonGenerate.Name = "_buttonGenerate";
            this._buttonGenerate.Size = new System.Drawing.Size(75, 23);
            this._buttonGenerate.TabIndex = 8;
            this._buttonGenerate.Text = "Generate";
            this._buttonGenerate.UseVisualStyleBackColor = true;
            this._buttonGenerate.Click += new System.EventHandler(this._buttonGenerate_Click);
            // 
            // _progressBar
            // 
            this._progressBar.Location = new System.Drawing.Point(13, 378);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(339, 23);
            this._progressBar.TabIndex = 9;
            // 
            // _buttonCancel
            // 
            this._buttonCancel.Location = new System.Drawing.Point(439, 378);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(75, 23);
            this._buttonCancel.TabIndex = 10;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            this._buttonCancel.Click += new System.EventHandler(this._buttonCancel_Click);
            // 
            // GenerateDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 414);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._progressBar);
            this.Controls.Add(this._buttonGenerate);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "GenerateDatabase";
            this.Text = "GenerateDatabase";
            ((System.ComponentModel.ISupportInitialize)(this._numericUpDownPercentWeekend)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker _dateTimePickerStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _textBoxStudiesPerDay;
        private System.Windows.Forms.NumericUpDown _numericUpDownPercentWeekend;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox _checkBoxMR;
        private System.Windows.Forms.CheckBox _checkBoxCT;
        private System.Windows.Forms.CheckBox _checkBoxRF;
        private System.Windows.Forms.CheckBox _checkBoxMG;
        private System.Windows.Forms.CheckBox _checkBoxXA;
        private System.Windows.Forms.CheckBox _checkBoxUS;
        private System.Windows.Forms.CheckBox _checkBoxDX;
        private System.Windows.Forms.CheckBox _checkBoxCR;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _textBoxTotalStudies;
        private System.Windows.Forms.Button _buttonGenerate;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Button _buttonCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox _comboBoxServerPartition;
        private System.Windows.Forms.CheckBox _checkBoxImageServerDatabase;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox _textBoxPort;
        private System.Windows.Forms.TextBox _textBoxHost;
        private System.Windows.Forms.TextBox _textBoxRemoteAETitle;
    }
}