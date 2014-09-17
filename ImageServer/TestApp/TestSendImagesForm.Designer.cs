namespace ClearCanvas.ImageServer.TestApp
{
    partial class TestSendImagesForm
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
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.panel1 = new System.Windows.Forms.Panel();
			this.ServerAE = new System.Windows.Forms.TextBox();
			this.ServerPort = new System.Windows.Forms.TextBox();
			this.ServerHost = new System.Windows.Forms.TextBox();
			this.LocalAE = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.checkableGroupBox2 = new ClearCanvas.ImageServer.TestApp.CheckableGroupBox();
			this.AutoRun = new System.Windows.Forms.Button();
			this.label13 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.minStudyDate = new System.Windows.Forms.TextBox();
			this.maxStudyDate = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.checkableGroupBox1 = new ClearCanvas.ImageServer.TestApp.CheckableGroupBox();
			this.Resend = new System.Windows.Forms.Button();
			this.panel3 = new System.Windows.Forms.Panel();
			this.RandomPatient = new System.Windows.Forms.Button();
			this.omitStudyDate = new System.Windows.Forms.CheckBox();
			this.StudyInstanceUid = new System.Windows.Forms.TextBox();
			this.omitBirthdate = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.omitGender = new System.Windows.Forms.CheckBox();
			this.PatientsBirthdate = new System.Windows.Forms.TextBox();
			this.omitIssuerOfPatientId = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.omitPatientId = new System.Windows.Forms.CheckBox();
			this.PatientsSex = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.omitPatientName = new System.Windows.Forms.CheckBox();
			this.AccessionNumber = new System.Windows.Forms.TextBox();
			this.StudyDate = new System.Windows.Forms.TextBox();
			this.omitAccession = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.PatientsId = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.PatientsName = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.NewStudy = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.IssuerOfPatientsId = new System.Windows.Forms.TextBox();
			this.GenerateImages = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.checkableGroupBox2.SuspendLayout();
			this.checkableGroupBox1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.ServerAE);
			this.panel1.Controls.Add(this.ServerPort);
			this.panel1.Controls.Add(this.ServerHost);
			this.panel1.Controls.Add(this.LocalAE);
			this.panel1.Controls.Add(this.label12);
			this.panel1.Controls.Add(this.label11);
			this.panel1.Controls.Add(this.label10);
			this.panel1.Controls.Add(this.label9);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(950, 92);
			this.panel1.TabIndex = 10;
			// 
			// ServerAE
			// 
			this.ServerAE.Location = new System.Drawing.Point(269, 35);
			this.ServerAE.Name = "ServerAE";
			this.ServerAE.Size = new System.Drawing.Size(100, 20);
			this.ServerAE.TabIndex = 2;
			this.ServerAE.Text = "ImageServer";
			// 
			// ServerPort
			// 
			this.ServerPort.Location = new System.Drawing.Point(375, 35);
			this.ServerPort.Name = "ServerPort";
			this.ServerPort.Size = new System.Drawing.Size(100, 20);
			this.ServerPort.TabIndex = 3;
			this.ServerPort.Text = "105";
			// 
			// ServerHost
			// 
			this.ServerHost.Location = new System.Drawing.Point(155, 36);
			this.ServerHost.Name = "ServerHost";
			this.ServerHost.Size = new System.Drawing.Size(100, 20);
			this.ServerHost.TabIndex = 1;
			this.ServerHost.Text = "localhost";
			// 
			// LocalAE
			// 
			this.LocalAE.Location = new System.Drawing.Point(23, 37);
			this.LocalAE.Name = "LocalAE";
			this.LocalAE.Size = new System.Drawing.Size(100, 20);
			this.LocalAE.TabIndex = 0;
			this.LocalAE.Text = "TEST";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(372, 20);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(60, 13);
			this.label12.TabIndex = 7;
			this.label12.Text = "Server Port";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(152, 20);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(63, 13);
			this.label11.TabIndex = 7;
			this.label11.Text = "Server Host";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(266, 20);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(55, 13);
			this.label10.TabIndex = 7;
			this.label10.Text = "Server AE";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(20, 22);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(50, 13);
			this.label9.TabIndex = 7;
			this.label9.Text = "Local AE";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.checkableGroupBox2);
			this.panel2.Controls.Add(this.checkableGroupBox1);
			this.panel2.Controls.Add(this.textBox1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 92);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(950, 691);
			this.panel2.TabIndex = 31;
			// 
			// textBox1
			// 
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.textBox1.Location = new System.Drawing.Point(0, 455);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(950, 236);
			this.textBox1.TabIndex = 50;
			// 
			// checkableGroupBox2
			// 
			this.checkableGroupBox2.Checked = false;
			this.checkableGroupBox2.Controls.Add(this.AutoRun);
			this.checkableGroupBox2.Controls.Add(this.label13);
			this.checkableGroupBox2.Controls.Add(this.label15);
			this.checkableGroupBox2.Controls.Add(this.minStudyDate);
			this.checkableGroupBox2.Controls.Add(this.maxStudyDate);
			this.checkableGroupBox2.Controls.Add(this.label14);
			this.checkableGroupBox2.Location = new System.Drawing.Point(13, 290);
			this.checkableGroupBox2.Name = "checkableGroupBox2";
			this.checkableGroupBox2.Size = new System.Drawing.Size(887, 100);
			this.checkableGroupBox2.TabIndex = 54;
			this.checkableGroupBox2.TabStop = false;
			this.checkableGroupBox2.Text = "Send Multiple Studies";
			this.checkableGroupBox2.TextColor = System.Drawing.Color.Blue;
			this.checkableGroupBox2.CheckStateChanged += new System.EventHandler(this.checkableGroupBox2_CheckStateChanged);
			// 
			// AutoRun
			// 
			this.AutoRun.Enabled = false;
			this.AutoRun.Location = new System.Drawing.Point(754, 36);
			this.AutoRun.Name = "AutoRun";
			this.AutoRun.Size = new System.Drawing.Size(105, 38);
			this.AutoRun.TabIndex = 10;
			this.AutoRun.Text = "Start Auto-Run";
			this.AutoRun.UseVisualStyleBackColor = true;
			this.AutoRun.Click += new System.EventHandler(this.AutoRun_Click);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(31, 38);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(325, 13);
			this.label13.TabIndex = 45;
			this.label13.Text = "This tool will generate random studies within the specific time range.";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(575, 36);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(107, 13);
			this.label15.TabIndex = 44;
			this.label15.Text = "Maximum Study Date";
			// 
			// minStudyDate
			// 
			this.minStudyDate.Location = new System.Drawing.Point(446, 54);
			this.minStudyDate.Name = "minStudyDate";
			this.minStudyDate.Size = new System.Drawing.Size(101, 20);
			this.minStudyDate.TabIndex = 43;
			this.minStudyDate.Text = "20100701";
			// 
			// maxStudyDate
			// 
			this.maxStudyDate.Location = new System.Drawing.Point(578, 54);
			this.maxStudyDate.Name = "maxStudyDate";
			this.maxStudyDate.Size = new System.Drawing.Size(101, 20);
			this.maxStudyDate.TabIndex = 43;
			this.maxStudyDate.Text = "20100701";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(443, 38);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(104, 13);
			this.label14.TabIndex = 44;
			this.label14.Text = "Minimum Study Date";
			// 
			// checkableGroupBox1
			// 
			this.checkableGroupBox1.Checked = false;
			this.checkableGroupBox1.Controls.Add(this.Resend);
			this.checkableGroupBox1.Controls.Add(this.panel3);
			this.checkableGroupBox1.Controls.Add(this.GenerateImages);
			this.checkableGroupBox1.Location = new System.Drawing.Point(12, 6);
			this.checkableGroupBox1.Name = "checkableGroupBox1";
			this.checkableGroupBox1.Size = new System.Drawing.Size(888, 262);
			this.checkableGroupBox1.TabIndex = 49;
			this.checkableGroupBox1.TabStop = false;
			this.checkableGroupBox1.Text = "Send a Single Study";
			this.checkableGroupBox1.TextColor = System.Drawing.Color.Blue;
			this.checkableGroupBox1.CheckStateChanged += new System.EventHandler(this.checkableGroupBox1_CheckStateChanged);
			// 
			// Resend
			// 
			this.Resend.Enabled = false;
			this.Resend.Location = new System.Drawing.Point(755, 76);
			this.Resend.Name = "Resend";
			this.Resend.Size = new System.Drawing.Size(105, 38);
			this.Resend.TabIndex = 9;
			this.Resend.Text = "Re-Send ";
			this.Resend.UseVisualStyleBackColor = true;
			this.Resend.Click += new System.EventHandler(this.Resend_Click);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.RandomPatient);
			this.panel3.Controls.Add(this.omitStudyDate);
			this.panel3.Controls.Add(this.StudyInstanceUid);
			this.panel3.Controls.Add(this.omitBirthdate);
			this.panel3.Controls.Add(this.label4);
			this.panel3.Controls.Add(this.omitGender);
			this.panel3.Controls.Add(this.PatientsBirthdate);
			this.panel3.Controls.Add(this.omitIssuerOfPatientId);
			this.panel3.Controls.Add(this.label8);
			this.panel3.Controls.Add(this.label6);
			this.panel3.Controls.Add(this.omitPatientId);
			this.panel3.Controls.Add(this.PatientsSex);
			this.panel3.Controls.Add(this.label3);
			this.panel3.Controls.Add(this.omitPatientName);
			this.panel3.Controls.Add(this.AccessionNumber);
			this.panel3.Controls.Add(this.StudyDate);
			this.panel3.Controls.Add(this.omitAccession);
			this.panel3.Controls.Add(this.label5);
			this.panel3.Controls.Add(this.PatientsId);
			this.panel3.Controls.Add(this.label2);
			this.panel3.Controls.Add(this.PatientsName);
			this.panel3.Controls.Add(this.label7);
			this.panel3.Controls.Add(this.NewStudy);
			this.panel3.Controls.Add(this.label1);
			this.panel3.Controls.Add(this.IssuerOfPatientsId);
			this.panel3.Location = new System.Drawing.Point(11, 28);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(698, 214);
			this.panel3.TabIndex = 48;
			// 
			// RandomPatient
			// 
			this.RandomPatient.Enabled = false;
			this.RandomPatient.Location = new System.Drawing.Point(21, 18);
			this.RandomPatient.Name = "RandomPatient";
			this.RandomPatient.Size = new System.Drawing.Size(105, 29);
			this.RandomPatient.TabIndex = 5;
			this.RandomPatient.Text = "Random";
			this.RandomPatient.UseVisualStyleBackColor = true;
			this.RandomPatient.Click += new System.EventHandler(this.RandomPatient_Click);
			// 
			// omitStudyDate
			// 
			this.omitStudyDate.AutoSize = true;
			this.omitStudyDate.Location = new System.Drawing.Point(637, 85);
			this.omitStudyDate.Name = "omitStudyDate";
			this.omitStudyDate.Size = new System.Drawing.Size(47, 17);
			this.omitStudyDate.TabIndex = 47;
			this.omitStudyDate.Text = "Omit";
			this.omitStudyDate.UseVisualStyleBackColor = true;
			// 
			// StudyInstanceUid
			// 
			this.StudyInstanceUid.Location = new System.Drawing.Point(484, 115);
			this.StudyInstanceUid.Name = "StudyInstanceUid";
			this.StudyInstanceUid.Size = new System.Drawing.Size(147, 20);
			this.StudyInstanceUid.TabIndex = 41;
			this.StudyInstanceUid.Text = "1.2.3.5.6.4.3";
			// 
			// omitBirthdate
			// 
			this.omitBirthdate.AutoSize = true;
			this.omitBirthdate.Location = new System.Drawing.Point(289, 174);
			this.omitBirthdate.Name = "omitBirthdate";
			this.omitBirthdate.Size = new System.Drawing.Size(47, 17);
			this.omitBirthdate.TabIndex = 47;
			this.omitBirthdate.Text = "Omit";
			this.omitBirthdate.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(18, 150);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(42, 13);
			this.label4.TabIndex = 37;
			this.label4.Text = "Gender";
			// 
			// omitGender
			// 
			this.omitGender.AutoSize = true;
			this.omitGender.Location = new System.Drawing.Point(289, 145);
			this.omitGender.Name = "omitGender";
			this.omitGender.Size = new System.Drawing.Size(47, 17);
			this.omitGender.TabIndex = 47;
			this.omitGender.Text = "Omit";
			this.omitGender.UseVisualStyleBackColor = true;
			// 
			// PatientsBirthdate
			// 
			this.PatientsBirthdate.Location = new System.Drawing.Point(136, 174);
			this.PatientsBirthdate.Name = "PatientsBirthdate";
			this.PatientsBirthdate.Size = new System.Drawing.Size(147, 20);
			this.PatientsBirthdate.TabIndex = 35;
			this.PatientsBirthdate.Text = "19661221";
			// 
			// omitIssuerOfPatientId
			// 
			this.omitIssuerOfPatientId.AutoSize = true;
			this.omitIssuerOfPatientId.Location = new System.Drawing.Point(289, 118);
			this.omitIssuerOfPatientId.Name = "omitIssuerOfPatientId";
			this.omitIssuerOfPatientId.Size = new System.Drawing.Size(47, 17);
			this.omitIssuerOfPatientId.TabIndex = 47;
			this.omitIssuerOfPatientId.Text = "Omit";
			this.omitIssuerOfPatientId.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(367, 118);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(97, 13);
			this.label8.TabIndex = 46;
			this.label8.Text = "Study Instance Uid";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(367, 60);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(96, 13);
			this.label6.TabIndex = 45;
			this.label6.Text = "Accession Number";
			// 
			// omitPatientId
			// 
			this.omitPatientId.AutoSize = true;
			this.omitPatientId.Location = new System.Drawing.Point(289, 90);
			this.omitPatientId.Name = "omitPatientId";
			this.omitPatientId.Size = new System.Drawing.Size(47, 17);
			this.omitPatientId.TabIndex = 47;
			this.omitPatientId.Text = "Omit";
			this.omitPatientId.UseVisualStyleBackColor = true;
			// 
			// PatientsSex
			// 
			this.PatientsSex.Location = new System.Drawing.Point(136, 143);
			this.PatientsSex.Name = "PatientsSex";
			this.PatientsSex.Size = new System.Drawing.Size(147, 20);
			this.PatientsSex.TabIndex = 34;
			this.PatientsSex.Text = "M";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(18, 116);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(97, 13);
			this.label3.TabIndex = 38;
			this.label3.Text = "Issuer Of Patient Id";
			// 
			// omitPatientName
			// 
			this.omitPatientName.AutoSize = true;
			this.omitPatientName.Location = new System.Drawing.Point(289, 60);
			this.omitPatientName.Name = "omitPatientName";
			this.omitPatientName.Size = new System.Drawing.Size(47, 17);
			this.omitPatientName.TabIndex = 47;
			this.omitPatientName.Text = "Omit";
			this.omitPatientName.UseVisualStyleBackColor = true;
			// 
			// AccessionNumber
			// 
			this.AccessionNumber.Location = new System.Drawing.Point(484, 57);
			this.AccessionNumber.Name = "AccessionNumber";
			this.AccessionNumber.Size = new System.Drawing.Size(147, 20);
			this.AccessionNumber.TabIndex = 42;
			this.AccessionNumber.Text = "TGH1029392";
			// 
			// StudyDate
			// 
			this.StudyDate.Location = new System.Drawing.Point(484, 85);
			this.StudyDate.Name = "StudyDate";
			this.StudyDate.Size = new System.Drawing.Size(147, 20);
			this.StudyDate.TabIndex = 43;
			this.StudyDate.Text = "19661221";
			// 
			// omitAccession
			// 
			this.omitAccession.AutoSize = true;
			this.omitAccession.Location = new System.Drawing.Point(637, 59);
			this.omitAccession.Name = "omitAccession";
			this.omitAccession.Size = new System.Drawing.Size(47, 17);
			this.omitAccession.TabIndex = 47;
			this.omitAccession.Text = "Omit";
			this.omitAccession.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(18, 181);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(49, 13);
			this.label5.TabIndex = 36;
			this.label5.Text = "Birthdate";
			// 
			// PatientsId
			// 
			this.PatientsId.Location = new System.Drawing.Point(136, 88);
			this.PatientsId.Name = "PatientsId";
			this.PatientsId.Size = new System.Drawing.Size(147, 20);
			this.PatientsId.TabIndex = 32;
			this.PatientsId.Text = "PATID-1001";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 91);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 13);
			this.label2.TabIndex = 39;
			this.label2.Text = "Patient Id";
			// 
			// PatientsName
			// 
			this.PatientsName.Location = new System.Drawing.Point(136, 58);
			this.PatientsName.Name = "PatientsName";
			this.PatientsName.Size = new System.Drawing.Size(147, 20);
			this.PatientsName.TabIndex = 7;
			this.PatientsName.Text = "John^Smith";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(366, 90);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(60, 13);
			this.label7.TabIndex = 44;
			this.label7.Text = "Study Date";
			// 
			// NewStudy
			// 
			this.NewStudy.Enabled = false;
			this.NewStudy.Location = new System.Drawing.Point(370, 17);
			this.NewStudy.Name = "NewStudy";
			this.NewStudy.Size = new System.Drawing.Size(105, 29);
			this.NewStudy.TabIndex = 6;
			this.NewStudy.Text = "Random";
			this.NewStudy.UseVisualStyleBackColor = true;
			this.NewStudy.Click += new System.EventHandler(this.NewStudy_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 13);
			this.label1.TabIndex = 40;
			this.label1.Text = "Patient Name";
			// 
			// IssuerOfPatientsId
			// 
			this.IssuerOfPatientsId.Location = new System.Drawing.Point(136, 116);
			this.IssuerOfPatientsId.Name = "IssuerOfPatientsId";
			this.IssuerOfPatientsId.Size = new System.Drawing.Size(147, 20);
			this.IssuerOfPatientsId.TabIndex = 33;
			this.IssuerOfPatientsId.Text = "TGH";
			// 
			// GenerateImages
			// 
			this.GenerateImages.Enabled = false;
			this.GenerateImages.Location = new System.Drawing.Point(755, 28);
			this.GenerateImages.Name = "GenerateImages";
			this.GenerateImages.Size = new System.Drawing.Size(105, 38);
			this.GenerateImages.TabIndex = 8;
			this.GenerateImages.Text = "Send More Images";
			this.GenerateImages.UseVisualStyleBackColor = true;
			this.GenerateImages.Click += new System.EventHandler(this.SendRandom_Click);
			// 
			// TestSendImagesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(950, 783);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "TestSendImagesForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Random Image Sender";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.checkableGroupBox2.ResumeLayout(false);
			this.checkableGroupBox2.PerformLayout();
			this.checkableGroupBox1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox ServerAE;
        private System.Windows.Forms.TextBox ServerPort;
        private System.Windows.Forms.TextBox ServerHost;
        private System.Windows.Forms.TextBox LocalAE;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button GenerateImages;
        private System.Windows.Forms.Button NewStudy;
        private System.Windows.Forms.Button RandomPatient;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox StudyDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox StudyInstanceUid;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox AccessionNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox PatientsName;
        private System.Windows.Forms.TextBox PatientsId;
        private System.Windows.Forms.TextBox IssuerOfPatientsId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PatientsSex;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PatientsBirthdate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AutoRun;
		private System.Windows.Forms.Button Resend;
		private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox omitAccession;
        private System.Windows.Forms.CheckBox omitStudyDate;
        private System.Windows.Forms.CheckBox omitBirthdate;
        private System.Windows.Forms.CheckBox omitGender;
        private System.Windows.Forms.CheckBox omitIssuerOfPatientId;
        private System.Windows.Forms.CheckBox omitPatientId;
		private System.Windows.Forms.CheckBox omitPatientName;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox minStudyDate;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox maxStudyDate;
		private System.Windows.Forms.Panel panel3;
		private CheckableGroupBox checkableGroupBox1;
		private CheckableGroupBox checkableGroupBox2;
    }
}