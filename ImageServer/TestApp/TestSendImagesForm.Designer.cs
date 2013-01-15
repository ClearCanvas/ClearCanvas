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
            this.LoadSamples = new System.Windows.Forms.Button();
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
            this.AssociationPerStudy = new System.Windows.Forms.NumericUpDown();
            this.NewStudy = new System.Windows.Forms.Button();
            this.RandomPatient = new System.Windows.Forms.Button();
            this.GenerateImages = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.AutoRun = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.omitStudyDate = new System.Windows.Forms.CheckBox();
            this.omitBirthdate = new System.Windows.Forms.CheckBox();
            this.omitGender = new System.Windows.Forms.CheckBox();
            this.omitIssuerOfPatientId = new System.Windows.Forms.CheckBox();
            this.omitPatientId = new System.Windows.Forms.CheckBox();
            this.omitPatientName = new System.Windows.Forms.CheckBox();
            this.omitAccession = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.StudyDate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.PatientsBirthdate = new System.Windows.Forms.TextBox();
            this.StudyInstanceUid = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.PatientsSex = new System.Windows.Forms.TextBox();
            this.AccessionNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.IssuerOfPatientsId = new System.Windows.Forms.TextBox();
            this.PatientsName = new System.Windows.Forms.TextBox();
            this.PatientsId = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.Resend = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.MaxSeries = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AssociationPerStudy)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxSeries)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // LoadSamples
            // 
            this.LoadSamples.Location = new System.Drawing.Point(454, 22);
            this.LoadSamples.Name = "LoadSamples";
            this.LoadSamples.Size = new System.Drawing.Size(105, 38);
            this.LoadSamples.TabIndex = 0;
            this.LoadSamples.Text = "Load Samples ...";
            this.LoadSamples.UseVisualStyleBackColor = true;
            this.LoadSamples.Click += new System.EventHandler(this.LoadSamples_Click);
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
            this.panel1.Controls.Add(this.LoadSamples);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(732, 78);
            this.panel1.TabIndex = 10;
            // 
            // ServerAE
            // 
            this.ServerAE.Location = new System.Drawing.Point(76, 44);
            this.ServerAE.Name = "ServerAE";
            this.ServerAE.Size = new System.Drawing.Size(100, 20);
            this.ServerAE.TabIndex = 8;
            this.ServerAE.Text = "ImageServer";
            // 
            // ServerPort
            // 
            this.ServerPort.Location = new System.Drawing.Point(281, 47);
            this.ServerPort.Name = "ServerPort";
            this.ServerPort.Size = new System.Drawing.Size(100, 20);
            this.ServerPort.TabIndex = 8;
            this.ServerPort.Text = "5001";
            // 
            // ServerHost
            // 
            this.ServerHost.Location = new System.Drawing.Point(281, 19);
            this.ServerHost.Name = "ServerHost";
            this.ServerHost.Size = new System.Drawing.Size(100, 20);
            this.ServerHost.TabIndex = 8;
            this.ServerHost.Text = "localhost";
            // 
            // LocalAE
            // 
            this.LocalAE.Location = new System.Drawing.Point(76, 19);
            this.LocalAE.Name = "LocalAE";
            this.LocalAE.Size = new System.Drawing.Size(100, 20);
            this.LocalAE.TabIndex = 8;
            this.LocalAE.Text = "TEST";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(212, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Server Port";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(212, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 13);
            this.label11.TabIndex = 7;
            this.label11.Text = "Server Host";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 47);
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
            // AssociationPerStudy
            // 
            this.AssociationPerStudy.Location = new System.Drawing.Point(34, 58);
            this.AssociationPerStudy.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.AssociationPerStudy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.AssociationPerStudy.Name = "AssociationPerStudy";
            this.AssociationPerStudy.Size = new System.Drawing.Size(73, 20);
            this.AssociationPerStudy.TabIndex = 9;
            this.AssociationPerStudy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NewStudy
            // 
            this.NewStudy.Enabled = false;
            this.NewStudy.Location = new System.Drawing.Point(185, 27);
            this.NewStudy.Name = "NewStudy";
            this.NewStudy.Size = new System.Drawing.Size(105, 38);
            this.NewStudy.TabIndex = 10;
            this.NewStudy.Text = "New Study";
            this.NewStudy.UseVisualStyleBackColor = true;
            this.NewStudy.Click += new System.EventHandler(this.NewStudy_Click);
            // 
            // RandomPatient
            // 
            this.RandomPatient.Enabled = false;
            this.RandomPatient.Location = new System.Drawing.Point(39, 27);
            this.RandomPatient.Name = "RandomPatient";
            this.RandomPatient.Size = new System.Drawing.Size(105, 38);
            this.RandomPatient.TabIndex = 10;
            this.RandomPatient.Text = "New Patient";
            this.RandomPatient.UseVisualStyleBackColor = true;
            this.RandomPatient.Click += new System.EventHandler(this.RandomPatient_Click);
            // 
            // GenerateImages
            // 
            this.GenerateImages.Enabled = false;
            this.GenerateImages.Location = new System.Drawing.Point(143, 17);
            this.GenerateImages.Name = "GenerateImages";
            this.GenerateImages.Size = new System.Drawing.Size(105, 38);
            this.GenerateImages.TabIndex = 9;
            this.GenerateImages.Text = "Send New Images";
            this.GenerateImages.UseVisualStyleBackColor = true;
            this.GenerateImages.Click += new System.EventHandler(this.SendRandom_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 78);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(732, 504);
            this.panel2.TabIndex = 31;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.AutoRun);
            this.groupBox3.Location = new System.Drawing.Point(410, 167);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(295, 72);
            this.groupBox3.TabIndex = 53;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Autorun";
            // 
            // AutoRun
            // 
            this.AutoRun.Enabled = false;
            this.AutoRun.Location = new System.Drawing.Point(143, 19);
            this.AutoRun.Name = "AutoRun";
            this.AutoRun.Size = new System.Drawing.Size(105, 38);
            this.AutoRun.TabIndex = 47;
            this.AutoRun.Text = "Start Auto-Run";
            this.AutoRun.UseVisualStyleBackColor = true;
            this.AutoRun.Click += new System.EventHandler(this.AutoRun_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.omitStudyDate);
            this.groupBox2.Controls.Add(this.omitBirthdate);
            this.groupBox2.Controls.Add(this.omitGender);
            this.groupBox2.Controls.Add(this.omitIssuerOfPatientId);
            this.groupBox2.Controls.Add(this.omitPatientId);
            this.groupBox2.Controls.Add(this.omitPatientName);
            this.groupBox2.Controls.Add(this.omitAccession);
            this.groupBox2.Controls.Add(this.RandomPatient);
            this.groupBox2.Controls.Add(this.NewStudy);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.StudyDate);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.PatientsBirthdate);
            this.groupBox2.Controls.Add(this.StudyInstanceUid);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.PatientsSex);
            this.groupBox2.Controls.Add(this.AccessionNumber);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.IssuerOfPatientsId);
            this.groupBox2.Controls.Add(this.PatientsName);
            this.groupBox2.Controls.Add(this.PatientsId);
            this.groupBox2.Location = new System.Drawing.Point(12, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(369, 330);
            this.groupBox2.TabIndex = 52;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Study && Demographics";
            // 
            // omitStudyDate
            // 
            this.omitStudyDate.AutoSize = true;
            this.omitStudyDate.Location = new System.Drawing.Point(307, 267);
            this.omitStudyDate.Name = "omitStudyDate";
            this.omitStudyDate.Size = new System.Drawing.Size(47, 17);
            this.omitStudyDate.TabIndex = 47;
            this.omitStudyDate.Text = "Omit";
            this.omitStudyDate.UseVisualStyleBackColor = true;
            // 
            // omitBirthdate
            // 
            this.omitBirthdate.AutoSize = true;
            this.omitBirthdate.Location = new System.Drawing.Point(307, 205);
            this.omitBirthdate.Name = "omitBirthdate";
            this.omitBirthdate.Size = new System.Drawing.Size(47, 17);
            this.omitBirthdate.TabIndex = 47;
            this.omitBirthdate.Text = "Omit";
            this.omitBirthdate.UseVisualStyleBackColor = true;
            // 
            // omitGender
            // 
            this.omitGender.AutoSize = true;
            this.omitGender.Location = new System.Drawing.Point(307, 176);
            this.omitGender.Name = "omitGender";
            this.omitGender.Size = new System.Drawing.Size(47, 17);
            this.omitGender.TabIndex = 47;
            this.omitGender.Text = "Omit";
            this.omitGender.UseVisualStyleBackColor = true;
            // 
            // omitIssuerOfPatientId
            // 
            this.omitIssuerOfPatientId.AutoSize = true;
            this.omitIssuerOfPatientId.Location = new System.Drawing.Point(307, 149);
            this.omitIssuerOfPatientId.Name = "omitIssuerOfPatientId";
            this.omitIssuerOfPatientId.Size = new System.Drawing.Size(47, 17);
            this.omitIssuerOfPatientId.TabIndex = 47;
            this.omitIssuerOfPatientId.Text = "Omit";
            this.omitIssuerOfPatientId.UseVisualStyleBackColor = true;
            // 
            // omitPatientId
            // 
            this.omitPatientId.AutoSize = true;
            this.omitPatientId.Location = new System.Drawing.Point(307, 121);
            this.omitPatientId.Name = "omitPatientId";
            this.omitPatientId.Size = new System.Drawing.Size(47, 17);
            this.omitPatientId.TabIndex = 47;
            this.omitPatientId.Text = "Omit";
            this.omitPatientId.UseVisualStyleBackColor = true;
            // 
            // omitPatientName
            // 
            this.omitPatientName.AutoSize = true;
            this.omitPatientName.Location = new System.Drawing.Point(307, 91);
            this.omitPatientName.Name = "omitPatientName";
            this.omitPatientName.Size = new System.Drawing.Size(47, 17);
            this.omitPatientName.TabIndex = 47;
            this.omitPatientName.Text = "Omit";
            this.omitPatientName.UseVisualStyleBackColor = true;
            // 
            // omitAccession
            // 
            this.omitAccession.AutoSize = true;
            this.omitAccession.Location = new System.Drawing.Point(307, 241);
            this.omitAccession.Name = "omitAccession";
            this.omitAccession.Size = new System.Drawing.Size(47, 17);
            this.omitAccession.TabIndex = 47;
            this.omitAccession.Text = "Omit";
            this.omitAccession.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "Patient Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 39;
            this.label2.Text = "Patient Id";
            // 
            // StudyDate
            // 
            this.StudyDate.Location = new System.Drawing.Point(154, 267);
            this.StudyDate.Name = "StudyDate";
            this.StudyDate.Size = new System.Drawing.Size(147, 20);
            this.StudyDate.TabIndex = 43;
            this.StudyDate.Text = "19661221";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Issuer Of Patient Id";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(37, 242);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 45;
            this.label6.Text = "Accession Number";
            // 
            // PatientsBirthdate
            // 
            this.PatientsBirthdate.Location = new System.Drawing.Point(154, 205);
            this.PatientsBirthdate.Name = "PatientsBirthdate";
            this.PatientsBirthdate.Size = new System.Drawing.Size(147, 20);
            this.PatientsBirthdate.TabIndex = 35;
            this.PatientsBirthdate.Text = "19661221";
            // 
            // StudyInstanceUid
            // 
            this.StudyInstanceUid.Location = new System.Drawing.Point(154, 297);
            this.StudyInstanceUid.Name = "StudyInstanceUid";
            this.StudyInstanceUid.Size = new System.Drawing.Size(147, 20);
            this.StudyInstanceUid.TabIndex = 41;
            this.StudyInstanceUid.Text = "1.2.3.5.6.4.3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 181);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Gender";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(37, 300);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 13);
            this.label8.TabIndex = 46;
            this.label8.Text = "Study Instance Uid";
            // 
            // PatientsSex
            // 
            this.PatientsSex.Location = new System.Drawing.Point(154, 174);
            this.PatientsSex.Name = "PatientsSex";
            this.PatientsSex.Size = new System.Drawing.Size(147, 20);
            this.PatientsSex.TabIndex = 34;
            this.PatientsSex.Text = "M";
            // 
            // AccessionNumber
            // 
            this.AccessionNumber.Location = new System.Drawing.Point(154, 239);
            this.AccessionNumber.Name = "AccessionNumber";
            this.AccessionNumber.Size = new System.Drawing.Size(147, 20);
            this.AccessionNumber.TabIndex = 42;
            this.AccessionNumber.Text = "TGH1029392";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Birthdate";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(36, 272);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 44;
            this.label7.Text = "Study Date";
            // 
            // IssuerOfPatientsId
            // 
            this.IssuerOfPatientsId.Location = new System.Drawing.Point(154, 147);
            this.IssuerOfPatientsId.Name = "IssuerOfPatientsId";
            this.IssuerOfPatientsId.Size = new System.Drawing.Size(147, 20);
            this.IssuerOfPatientsId.TabIndex = 33;
            this.IssuerOfPatientsId.Text = "TGH";
            // 
            // PatientsName
            // 
            this.PatientsName.Location = new System.Drawing.Point(154, 89);
            this.PatientsName.Name = "PatientsName";
            this.PatientsName.Size = new System.Drawing.Size(147, 20);
            this.PatientsName.TabIndex = 31;
            this.PatientsName.Text = "John^Smith";
            // 
            // PatientsId
            // 
            this.PatientsId.Location = new System.Drawing.Point(154, 119);
            this.PatientsId.Name = "PatientsId";
            this.PatientsId.Size = new System.Drawing.Size(147, 20);
            this.PatientsId.TabIndex = 32;
            this.PatientsId.Text = "PATID-1001";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.MaxSeries);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.AssociationPerStudy);
            this.groupBox1.Controls.Add(this.GenerateImages);
            this.groupBox1.Controls.Add(this.Resend);
            this.groupBox1.Location = new System.Drawing.Point(410, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 120);
            this.groupBox1.TabIndex = 51;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Send Control Panel";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(31, 40);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(76, 13);
            this.label13.TabIndex = 7;
            this.label13.Text = "# Associations";
            // 
            // Resend
            // 
            this.Resend.Enabled = false;
            this.Resend.Location = new System.Drawing.Point(143, 67);
            this.Resend.Name = "Resend";
            this.Resend.Size = new System.Drawing.Size(105, 38);
            this.Resend.TabIndex = 49;
            this.Resend.Text = "Re-Send";
            this.Resend.UseVisualStyleBackColor = true;
            this.Resend.Click += new System.EventHandler(this.Resend_Click);
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 371);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(732, 133);
            this.textBox1.TabIndex = 50;
            // 
            // MaxSeries
            // 
            this.MaxSeries.Location = new System.Drawing.Point(34, 94);
            this.MaxSeries.Name = "MaxSeries";
            this.MaxSeries.Size = new System.Drawing.Size(73, 20);
            this.MaxSeries.TabIndex = 50;
            this.MaxSeries.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // TestSendImagesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 582);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "TestSendImagesForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Random Image Sender";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AssociationPerStudy)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxSeries)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button LoadSamples;
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
        private System.Windows.Forms.NumericUpDown AssociationPerStudy;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox omitAccession;
        private System.Windows.Forms.CheckBox omitStudyDate;
        private System.Windows.Forms.CheckBox omitBirthdate;
        private System.Windows.Forms.CheckBox omitGender;
        private System.Windows.Forms.CheckBox omitIssuerOfPatientId;
        private System.Windows.Forms.CheckBox omitPatientId;
        private System.Windows.Forms.CheckBox omitPatientName;
        private System.Windows.Forms.NumericUpDown MaxSeries;
    }
}