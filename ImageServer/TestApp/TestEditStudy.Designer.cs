namespace ClearCanvas.ImageServer.TestApp
{
    partial class TestEditStudyForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestEditStudyForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.patientIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patientsNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patientsBirthDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.studyInstanceUidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patientsSexDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.studyDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.studyTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accessionNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.studyIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.studyDescriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.referringPhysiciansNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.numberOfStudyRelatedSeriesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.numberOfStudyRelatedInstancesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.studyBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.imageServerDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.imageServerDataSet = new ClearCanvas.ImageServer.TestApp.ImageServerDataSet();
            this.studyTableAdapter = new ClearCanvas.ImageServer.TestApp.ImageServerDataSetTableAdapters.StudyTableAdapter();
            this.studyStorageTableAdapter = new ClearCanvas.ImageServer.TestApp.ImageServerDataSetTableAdapters.StudyStorageTableAdapter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.studyBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageServerDataSetBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageServerDataSet)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.patientIdDataGridViewTextBoxColumn,
            this.patientsNameDataGridViewTextBoxColumn,
            this.patientsBirthDateDataGridViewTextBoxColumn,
            this.studyInstanceUidDataGridViewTextBoxColumn,
            this.patientsSexDataGridViewTextBoxColumn,
            this.studyDateDataGridViewTextBoxColumn,
            this.studyTimeDataGridViewTextBoxColumn,
            this.accessionNumberDataGridViewTextBoxColumn,
            this.studyIdDataGridViewTextBoxColumn,
            this.studyDescriptionDataGridViewTextBoxColumn,
            this.referringPhysiciansNameDataGridViewTextBoxColumn,
            this.numberOfStudyRelatedSeriesDataGridViewTextBoxColumn,
            this.numberOfStudyRelatedInstancesDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.studyBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1102, 376);
            this.dataGridView1.TabIndex = 0;
            // 
            // patientIdDataGridViewTextBoxColumn
            // 
            this.patientIdDataGridViewTextBoxColumn.DataPropertyName = "PatientId";
            this.patientIdDataGridViewTextBoxColumn.HeaderText = "PatientId";
            this.patientIdDataGridViewTextBoxColumn.Name = "patientIdDataGridViewTextBoxColumn";
            this.patientIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // patientsNameDataGridViewTextBoxColumn
            // 
            this.patientsNameDataGridViewTextBoxColumn.DataPropertyName = "PatientsName";
            this.patientsNameDataGridViewTextBoxColumn.HeaderText = "PatientsName";
            this.patientsNameDataGridViewTextBoxColumn.Name = "patientsNameDataGridViewTextBoxColumn";
            this.patientsNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // patientsBirthDateDataGridViewTextBoxColumn
            // 
            this.patientsBirthDateDataGridViewTextBoxColumn.DataPropertyName = "PatientsBirthDate";
            this.patientsBirthDateDataGridViewTextBoxColumn.HeaderText = "PatientsBirthDate";
            this.patientsBirthDateDataGridViewTextBoxColumn.Name = "patientsBirthDateDataGridViewTextBoxColumn";
            this.patientsBirthDateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // studyInstanceUidDataGridViewTextBoxColumn
            // 
            this.studyInstanceUidDataGridViewTextBoxColumn.DataPropertyName = "StudyInstanceUid";
            this.studyInstanceUidDataGridViewTextBoxColumn.HeaderText = "StudyInstanceUid";
            this.studyInstanceUidDataGridViewTextBoxColumn.Name = "studyInstanceUidDataGridViewTextBoxColumn";
            this.studyInstanceUidDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // patientsSexDataGridViewTextBoxColumn
            // 
            this.patientsSexDataGridViewTextBoxColumn.DataPropertyName = "PatientsSex";
            this.patientsSexDataGridViewTextBoxColumn.HeaderText = "PatientsSex";
            this.patientsSexDataGridViewTextBoxColumn.Name = "patientsSexDataGridViewTextBoxColumn";
            this.patientsSexDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // studyDateDataGridViewTextBoxColumn
            // 
            this.studyDateDataGridViewTextBoxColumn.DataPropertyName = "StudyDate";
            this.studyDateDataGridViewTextBoxColumn.HeaderText = "StudyDate";
            this.studyDateDataGridViewTextBoxColumn.Name = "studyDateDataGridViewTextBoxColumn";
            this.studyDateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // studyTimeDataGridViewTextBoxColumn
            // 
            this.studyTimeDataGridViewTextBoxColumn.DataPropertyName = "StudyTime";
            this.studyTimeDataGridViewTextBoxColumn.HeaderText = "StudyTime";
            this.studyTimeDataGridViewTextBoxColumn.Name = "studyTimeDataGridViewTextBoxColumn";
            this.studyTimeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // accessionNumberDataGridViewTextBoxColumn
            // 
            this.accessionNumberDataGridViewTextBoxColumn.DataPropertyName = "AccessionNumber";
            this.accessionNumberDataGridViewTextBoxColumn.HeaderText = "AccessionNumber";
            this.accessionNumberDataGridViewTextBoxColumn.Name = "accessionNumberDataGridViewTextBoxColumn";
            this.accessionNumberDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // studyIdDataGridViewTextBoxColumn
            // 
            this.studyIdDataGridViewTextBoxColumn.DataPropertyName = "StudyId";
            this.studyIdDataGridViewTextBoxColumn.HeaderText = "StudyId";
            this.studyIdDataGridViewTextBoxColumn.Name = "studyIdDataGridViewTextBoxColumn";
            this.studyIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // studyDescriptionDataGridViewTextBoxColumn
            // 
            this.studyDescriptionDataGridViewTextBoxColumn.DataPropertyName = "StudyDescription";
            this.studyDescriptionDataGridViewTextBoxColumn.HeaderText = "StudyDescription";
            this.studyDescriptionDataGridViewTextBoxColumn.Name = "studyDescriptionDataGridViewTextBoxColumn";
            this.studyDescriptionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // referringPhysiciansNameDataGridViewTextBoxColumn
            // 
            this.referringPhysiciansNameDataGridViewTextBoxColumn.DataPropertyName = "ReferringPhysiciansName";
            this.referringPhysiciansNameDataGridViewTextBoxColumn.HeaderText = "ReferringPhysiciansName";
            this.referringPhysiciansNameDataGridViewTextBoxColumn.Name = "referringPhysiciansNameDataGridViewTextBoxColumn";
            this.referringPhysiciansNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // numberOfStudyRelatedSeriesDataGridViewTextBoxColumn
            // 
            this.numberOfStudyRelatedSeriesDataGridViewTextBoxColumn.DataPropertyName = "NumberOfStudyRelatedSeries";
            this.numberOfStudyRelatedSeriesDataGridViewTextBoxColumn.HeaderText = "NumberOfStudyRelatedSeries";
            this.numberOfStudyRelatedSeriesDataGridViewTextBoxColumn.Name = "numberOfStudyRelatedSeriesDataGridViewTextBoxColumn";
            this.numberOfStudyRelatedSeriesDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // numberOfStudyRelatedInstancesDataGridViewTextBoxColumn
            // 
            this.numberOfStudyRelatedInstancesDataGridViewTextBoxColumn.DataPropertyName = "NumberOfStudyRelatedInstances";
            this.numberOfStudyRelatedInstancesDataGridViewTextBoxColumn.HeaderText = "NumberOfStudyRelatedInstances";
            this.numberOfStudyRelatedInstancesDataGridViewTextBoxColumn.Name = "numberOfStudyRelatedInstancesDataGridViewTextBoxColumn";
            this.numberOfStudyRelatedInstancesDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // studyBindingSource
            // 
            this.studyBindingSource.DataMember = "Study";
            this.studyBindingSource.DataSource = this.imageServerDataSetBindingSource;
            // 
            // imageServerDataSetBindingSource
            // 
            this.imageServerDataSetBindingSource.DataSource = this.imageServerDataSet;
            this.imageServerDataSetBindingSource.Position = 0;
            // 
            // imageServerDataSet
            // 
            this.imageServerDataSet.DataSetName = "ImageServerDataSet";
            this.imageServerDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // studyTableAdapter
            // 
            this.studyTableAdapter.ClearBeforeFill = true;
            // 
            // studyStorageTableAdapter
            // 
            this.studyStorageTableAdapter.ClearBeforeFill = true;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.ApplyButton);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 376);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1102, 327);
            this.panel1.TabIndex = 1;
            // 
            // ApplyButton
            // 
            this.ApplyButton.Location = new System.Drawing.Point(816, 93);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(134, 46);
            this.ApplyButton.TabIndex = 1;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.Apply_Click);
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(626, 327);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Enabled = true;
            this.RefreshTimer.Interval = 1000;
            this.RefreshTimer.Tick += new System.EventHandler(this.RefreshTimer_Tick);
            // 
            // TestEditStudyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1102, 703);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "TestEditStudyForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "TestEditStudy";
            this.Load += new System.EventHandler(this.TestEditStudyForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.studyBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageServerDataSetBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageServerDataSet)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource imageServerDataSetBindingSource;
        private ImageServerDataSet imageServerDataSet;
        private ClearCanvas.ImageServer.TestApp.ImageServerDataSetTableAdapters.StudyTableAdapter studyTableAdapter;
        private ClearCanvas.ImageServer.TestApp.ImageServerDataSetTableAdapters.StudyStorageTableAdapter studyStorageTableAdapter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.BindingSource studyBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn patientIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn patientsNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn patientsBirthDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn studyInstanceUidDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn patientsSexDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn studyDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn studyTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn accessionNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn studyIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn studyDescriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn referringPhysiciansNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn numberOfStudyRelatedSeriesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn numberOfStudyRelatedInstancesDataGridViewTextBoxColumn;
        private System.Windows.Forms.Timer RefreshTimer;
    }
}