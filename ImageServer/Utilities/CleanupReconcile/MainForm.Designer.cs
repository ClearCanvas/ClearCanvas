namespace ClearCanvas.ImageServer.Utilities.CleanupReconcile
{
    partial class MainForm
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
            this.Path = new System.Windows.Forms.TextBox();
            this.ScanButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.StopButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.SummaryTab = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.TotalScanned = new System.Windows.Forms.Label();
            this.ScanFailed = new System.Windows.Forms.Label();
            this.ShowBackupFilesOnlyCases = new System.Windows.Forms.Button();
            this.Skipped = new System.Windows.Forms.Label();
            this.ShowStudyWasDeletedCases = new System.Windows.Forms.Button();
            this.InSIQ = new System.Windows.Forms.Label();
            this.ShowStudyDoesNotExistCases = new System.Windows.Forms.Button();
            this.ShowUnknownProblemCases = new System.Windows.Forms.Button();
            this.ShowStudyResentCases = new System.Windows.Forms.Button();
            this.ShowEmptyFoldersCase = new System.Windows.Forms.Button();
            this.StudyDoesNotExistCount = new System.Windows.Forms.Label();
            this.StudyWasResentCount = new System.Windows.Forms.Label();
            this.StudyDeletedCount = new System.Windows.Forms.Label();
            this.BackupOrTempOnlyCount = new System.Windows.Forms.Label();
            this.EmptyCount = new System.Windows.Forms.Label();
            this.UnidentifiedCount = new System.Windows.Forms.Label();
            this.Orphanned = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ResultsTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.MoveFolderButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.SummaryTab.SuspendLayout();
            this.ResultsTab.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Path
            // 
            this.Path.Location = new System.Drawing.Point(71, 42);
            this.Path.Name = "Path";
            this.Path.Size = new System.Drawing.Size(419, 20);
            this.Path.TabIndex = 0;
            this.Path.Text = "C:\\FS\\ImageServer\\Reconcile";
            // 
            // ScanButton
            // 
            this.ScanButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScanButton.Location = new System.Drawing.Point(517, 21);
            this.ScanButton.Name = "ScanButton";
            this.ScanButton.Size = new System.Drawing.Size(147, 41);
            this.ScanButton.TabIndex = 1;
            this.ScanButton.Text = "Scan";
            this.ScanButton.UseVisualStyleBackColor = true;
            this.ScanButton.Click += new System.EventHandler(this.ScanButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 643);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(833, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // StopButton
            // 
            this.StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StopButton.Location = new System.Drawing.Point(683, 21);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(93, 41);
            this.StopButton.TabIndex = 1;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.SummaryTab);
            this.tabControl1.Controls.Add(this.ResultsTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 89);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(827, 551);
            this.tabControl1.TabIndex = 5;
            // 
            // SummaryTab
            // 
            this.SummaryTab.Controls.Add(this.label17);
            this.SummaryTab.Controls.Add(this.label14);
            this.SummaryTab.Controls.Add(this.label13);
            this.SummaryTab.Controls.Add(this.label16);
            this.SummaryTab.Controls.Add(this.label2);
            this.SummaryTab.Controls.Add(this.label1);
            this.SummaryTab.Controls.Add(this.label15);
            this.SummaryTab.Controls.Add(this.label19);
            this.SummaryTab.Controls.Add(this.label4);
            this.SummaryTab.Controls.Add(this.label12);
            this.SummaryTab.Controls.Add(this.TotalScanned);
            this.SummaryTab.Controls.Add(this.ScanFailed);
            this.SummaryTab.Controls.Add(this.ShowBackupFilesOnlyCases);
            this.SummaryTab.Controls.Add(this.Skipped);
            this.SummaryTab.Controls.Add(this.ShowStudyWasDeletedCases);
            this.SummaryTab.Controls.Add(this.InSIQ);
            this.SummaryTab.Controls.Add(this.ShowStudyDoesNotExistCases);
            this.SummaryTab.Controls.Add(this.ShowUnknownProblemCases);
            this.SummaryTab.Controls.Add(this.ShowStudyResentCases);
            this.SummaryTab.Controls.Add(this.ShowEmptyFoldersCase);
            this.SummaryTab.Controls.Add(this.StudyDoesNotExistCount);
            this.SummaryTab.Controls.Add(this.StudyWasResentCount);
            this.SummaryTab.Controls.Add(this.StudyDeletedCount);
            this.SummaryTab.Controls.Add(this.BackupOrTempOnlyCount);
            this.SummaryTab.Controls.Add(this.EmptyCount);
            this.SummaryTab.Controls.Add(this.UnidentifiedCount);
            this.SummaryTab.Controls.Add(this.Orphanned);
            this.SummaryTab.Controls.Add(this.label11);
            this.SummaryTab.Controls.Add(this.label9);
            this.SummaryTab.Controls.Add(this.label7);
            this.SummaryTab.Controls.Add(this.label6);
            this.SummaryTab.Controls.Add(this.label5);
            this.SummaryTab.Controls.Add(this.label8);
            this.SummaryTab.Controls.Add(this.label3);
            this.SummaryTab.Location = new System.Drawing.Point(4, 22);
            this.SummaryTab.Name = "SummaryTab";
            this.SummaryTab.Padding = new System.Windows.Forms.Padding(3);
            this.SummaryTab.Size = new System.Drawing.Size(819, 525);
            this.SummaryTab.TabIndex = 2;
            this.SummaryTab.Text = "Summary";
            this.SummaryTab.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(133, 457);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(480, 13);
            this.label17.TabIndex = 9;
            this.label17.Text = "Could be used for auto-reconciliation or the study is being processed (i.e., pres" +
                "ent in the work queue)";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(135, 389);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(349, 13);
            this.label14.TabIndex = 9;
            this.label14.Text = "Possible Cause: Study was deleted using the Web GUI  and then resent.";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(135, 328);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(305, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "Possible Cause:  study was manually deleted from the database";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(135, 154);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(410, 13);
            this.label16.TabIndex = 9;
            this.label16.Text = "Possible Cause:  folder was not cleaned up properly by the server in previous rel" +
                "eases";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(587, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Found In SIQ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Total Scanned";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(135, 204);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(413, 13);
            this.label15.TabIndex = 9;
            this.label15.Text = "Possible Cause:  folder was not cleaned up properly by the server. in previous re" +
                "leases";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(419, 47);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(93, 20);
            this.label19.TabIndex = 6;
            this.label19.Text = "Scan Failed";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(248, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Skipped";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(135, 268);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(385, 13);
            this.label12.TabIndex = 9;
            this.label12.Text = "Possible Cause:  study was manually deleted from the database and then resent.";
            // 
            // TotalScanned
            // 
            this.TotalScanned.AutoSize = true;
            this.TotalScanned.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalScanned.Location = new System.Drawing.Point(176, 47);
            this.TotalScanned.Name = "TotalScanned";
            this.TotalScanned.Size = new System.Drawing.Size(21, 20);
            this.TotalScanned.TabIndex = 7;
            this.TotalScanned.Text = "...";
            // 
            // ScanFailed
            // 
            this.ScanFailed.AutoSize = true;
            this.ScanFailed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScanFailed.Location = new System.Drawing.Point(515, 47);
            this.ScanFailed.Name = "ScanFailed";
            this.ScanFailed.Size = new System.Drawing.Size(21, 20);
            this.ScanFailed.TabIndex = 7;
            this.ScanFailed.Text = "...";
            // 
            // ShowBackupFilesOnlyCases
            // 
            this.ShowBackupFilesOnlyCases.Enabled = false;
            this.ShowBackupFilesOnlyCases.Location = new System.Drawing.Point(679, 125);
            this.ShowBackupFilesOnlyCases.Name = "ShowBackupFilesOnlyCases";
            this.ShowBackupFilesOnlyCases.Size = new System.Drawing.Size(99, 23);
            this.ShowBackupFilesOnlyCases.TabIndex = 8;
            this.ShowBackupFilesOnlyCases.Text = "View";
            this.ShowBackupFilesOnlyCases.UseVisualStyleBackColor = true;
            this.ShowBackupFilesOnlyCases.Click += new System.EventHandler(this.ShowBackupFilesOnlyCases_Click);
            // 
            // Skipped
            // 
            this.Skipped.AutoSize = true;
            this.Skipped.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Skipped.Location = new System.Drawing.Point(340, 47);
            this.Skipped.Name = "Skipped";
            this.Skipped.Size = new System.Drawing.Size(21, 20);
            this.Skipped.TabIndex = 7;
            this.Skipped.Text = "...";
            // 
            // ShowStudyWasDeletedCases
            // 
            this.ShowStudyWasDeletedCases.Enabled = false;
            this.ShowStudyWasDeletedCases.Location = new System.Drawing.Point(677, 355);
            this.ShowStudyWasDeletedCases.Name = "ShowStudyWasDeletedCases";
            this.ShowStudyWasDeletedCases.Size = new System.Drawing.Size(99, 23);
            this.ShowStudyWasDeletedCases.TabIndex = 8;
            this.ShowStudyWasDeletedCases.Text = "View";
            this.ShowStudyWasDeletedCases.UseVisualStyleBackColor = true;
            this.ShowStudyWasDeletedCases.Click += new System.EventHandler(this.ShowStudyWasDeletedCases_Click);
            // 
            // InSIQ
            // 
            this.InSIQ.AutoSize = true;
            this.InSIQ.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InSIQ.Location = new System.Drawing.Point(716, 47);
            this.InSIQ.Name = "InSIQ";
            this.InSIQ.Size = new System.Drawing.Size(21, 20);
            this.InSIQ.TabIndex = 7;
            this.InSIQ.Text = "...";
            // 
            // ShowStudyDoesNotExistCases
            // 
            this.ShowStudyDoesNotExistCases.Enabled = false;
            this.ShowStudyDoesNotExistCases.Location = new System.Drawing.Point(677, 296);
            this.ShowStudyDoesNotExistCases.Name = "ShowStudyDoesNotExistCases";
            this.ShowStudyDoesNotExistCases.Size = new System.Drawing.Size(99, 23);
            this.ShowStudyDoesNotExistCases.TabIndex = 8;
            this.ShowStudyDoesNotExistCases.Text = "View";
            this.ShowStudyDoesNotExistCases.UseVisualStyleBackColor = true;
            this.ShowStudyDoesNotExistCases.Click += new System.EventHandler(this.ShowStudyDoesNotExistCases_Click);
            // 
            // ShowUnknownProblemCases
            // 
            this.ShowUnknownProblemCases.Enabled = false;
            this.ShowUnknownProblemCases.Location = new System.Drawing.Point(677, 436);
            this.ShowUnknownProblemCases.Name = "ShowUnknownProblemCases";
            this.ShowUnknownProblemCases.Size = new System.Drawing.Size(99, 23);
            this.ShowUnknownProblemCases.TabIndex = 8;
            this.ShowUnknownProblemCases.Text = "View";
            this.ShowUnknownProblemCases.UseVisualStyleBackColor = true;
            this.ShowUnknownProblemCases.Click += new System.EventHandler(this.ShowUnknownProblemCases_Click);
            // 
            // ShowStudyResentCases
            // 
            this.ShowStudyResentCases.Enabled = false;
            this.ShowStudyResentCases.Location = new System.Drawing.Point(677, 232);
            this.ShowStudyResentCases.Name = "ShowStudyResentCases";
            this.ShowStudyResentCases.Size = new System.Drawing.Size(99, 23);
            this.ShowStudyResentCases.TabIndex = 8;
            this.ShowStudyResentCases.Text = "View";
            this.ShowStudyResentCases.UseVisualStyleBackColor = true;
            this.ShowStudyResentCases.Click += new System.EventHandler(this.ShowStudyResentCases_Click);
            // 
            // ShowEmptyFoldersCase
            // 
            this.ShowEmptyFoldersCase.Enabled = false;
            this.ShowEmptyFoldersCase.Location = new System.Drawing.Point(677, 175);
            this.ShowEmptyFoldersCase.Name = "ShowEmptyFoldersCase";
            this.ShowEmptyFoldersCase.Size = new System.Drawing.Size(99, 23);
            this.ShowEmptyFoldersCase.TabIndex = 8;
            this.ShowEmptyFoldersCase.Text = "View";
            this.ShowEmptyFoldersCase.UseVisualStyleBackColor = true;
            this.ShowEmptyFoldersCase.Click += new System.EventHandler(this.ShowEmptyFoldersCase_Click);
            // 
            // StudyDoesNotExistCount
            // 
            this.StudyDoesNotExistCount.AutoSize = true;
            this.StudyDoesNotExistCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StudyDoesNotExistCount.Location = new System.Drawing.Point(610, 299);
            this.StudyDoesNotExistCount.Name = "StudyDoesNotExistCount";
            this.StudyDoesNotExistCount.Size = new System.Drawing.Size(21, 20);
            this.StudyDoesNotExistCount.TabIndex = 7;
            this.StudyDoesNotExistCount.Text = "...";
            // 
            // StudyWasResentCount
            // 
            this.StudyWasResentCount.AutoSize = true;
            this.StudyWasResentCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StudyWasResentCount.Location = new System.Drawing.Point(610, 235);
            this.StudyWasResentCount.Name = "StudyWasResentCount";
            this.StudyWasResentCount.Size = new System.Drawing.Size(21, 20);
            this.StudyWasResentCount.TabIndex = 7;
            this.StudyWasResentCount.Text = "...";
            // 
            // StudyDeletedCount
            // 
            this.StudyDeletedCount.AutoSize = true;
            this.StudyDeletedCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StudyDeletedCount.Location = new System.Drawing.Point(610, 355);
            this.StudyDeletedCount.Name = "StudyDeletedCount";
            this.StudyDeletedCount.Size = new System.Drawing.Size(21, 20);
            this.StudyDeletedCount.TabIndex = 7;
            this.StudyDeletedCount.Text = "...";
            // 
            // BackupOrTempOnlyCount
            // 
            this.BackupOrTempOnlyCount.AutoSize = true;
            this.BackupOrTempOnlyCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackupOrTempOnlyCount.Location = new System.Drawing.Point(610, 125);
            this.BackupOrTempOnlyCount.Name = "BackupOrTempOnlyCount";
            this.BackupOrTempOnlyCount.Size = new System.Drawing.Size(21, 20);
            this.BackupOrTempOnlyCount.TabIndex = 7;
            this.BackupOrTempOnlyCount.Text = "...";
            // 
            // EmptyCount
            // 
            this.EmptyCount.AutoSize = true;
            this.EmptyCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmptyCount.Location = new System.Drawing.Point(610, 178);
            this.EmptyCount.Name = "EmptyCount";
            this.EmptyCount.Size = new System.Drawing.Size(21, 20);
            this.EmptyCount.TabIndex = 7;
            this.EmptyCount.Text = "...";
            // 
            // UnidentifiedCount
            // 
            this.UnidentifiedCount.AutoSize = true;
            this.UnidentifiedCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UnidentifiedCount.Location = new System.Drawing.Point(610, 439);
            this.UnidentifiedCount.Name = "UnidentifiedCount";
            this.UnidentifiedCount.Size = new System.Drawing.Size(21, 20);
            this.UnidentifiedCount.TabIndex = 7;
            this.UnidentifiedCount.Text = "...";
            // 
            // Orphanned
            // 
            this.Orphanned.AutoSize = true;
            this.Orphanned.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Orphanned.Location = new System.Drawing.Point(281, 91);
            this.Orphanned.Name = "Orphanned";
            this.Orphanned.Size = new System.Drawing.Size(21, 20);
            this.Orphanned.TabIndex = 7;
            this.Orphanned.Text = "...";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(85, 299);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(152, 20);
            this.label11.TabIndex = 6;
            this.label11.Text = "Study does not exist";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(85, 235);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(343, 20);
            this.label9.TabIndex = 6;
            this.label9.Text = "Study was inserted After the folder was created";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(87, 358);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(487, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "Study exists but was previously deleted After the folder was created ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(85, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(243, 20);
            this.label6.TabIndex = 6;
            this.label6.Text = "Contains only Backup/Temp Files";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(85, 180);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "Folder is empty";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(85, 436);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 20);
            this.label8.TabIndex = 6;
            this.label8.Text = "Undetermined";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(35, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(225, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Folders that are not In SIQ";
            // 
            // ResultsTab
            // 
            this.ResultsTab.Controls.Add(this.tableLayoutPanel1);
            this.ResultsTab.Location = new System.Drawing.Point(4, 22);
            this.ResultsTab.Name = "ResultsTab";
            this.ResultsTab.Padding = new System.Windows.Forms.Padding(3);
            this.ResultsTab.Size = new System.Drawing.Size(819, 525);
            this.ResultsTab.TabIndex = 0;
            this.ResultsTab.Text = "Results";
            this.ResultsTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.MoveFolderButton, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(813, 519);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 53);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(807, 463);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dataGridView1.Click += new System.EventHandler(this.dataGridView1_Click);
            // 
            // MoveFolderButton
            // 
            this.MoveFolderButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.MoveFolderButton.Location = new System.Drawing.Point(329, 8);
            this.MoveFolderButton.Name = "MoveFolderButton";
            this.MoveFolderButton.Size = new System.Drawing.Size(155, 34);
            this.MoveFolderButton.TabIndex = 5;
            this.MoveFolderButton.Text = "Move Folders ...";
            this.MoveFolderButton.UseVisualStyleBackColor = true;
            this.MoveFolderButton.Click += new System.EventHandler(this.MoveFolderButton_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(68, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(141, 16);
            this.label10.TabIndex = 6;
            this.label10.Text = "Reconcile Folder Path";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(833, 643);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.Path);
            this.panel1.Controls.Add(this.StopButton);
            this.panel1.Controls.Add(this.ScanButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(827, 80);
            this.panel1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 666);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.progressBar1);
            this.Name = "MainForm";
            this.Text = "Reconcile Folder Cleanup";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReconcileCleanupUtils_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.SummaryTab.ResumeLayout(false);
            this.SummaryTab.PerformLayout();
            this.ResultsTab.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox Path;
        private System.Windows.Forms.Button ScanButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage SummaryTab;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label TotalScanned;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label StudyDeletedCount;
        private System.Windows.Forms.Label BackupOrTempOnlyCount;
        private System.Windows.Forms.Label EmptyCount;
        private System.Windows.Forms.Label Orphanned;
        private System.Windows.Forms.Label InSIQ;
        private System.Windows.Forms.Label Skipped;
        private System.Windows.Forms.Label UnidentifiedCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label StudyWasResentCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button ShowEmptyFoldersCase;
        private System.Windows.Forms.Button ShowStudyResentCases;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button ShowStudyWasDeletedCases;
        private System.Windows.Forms.Button ShowBackupFilesOnlyCases;
        private System.Windows.Forms.Button ShowUnknownProblemCases;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label StudyDoesNotExistCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button ShowStudyDoesNotExistCases;
        private System.Windows.Forms.TabPage ResultsTab;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button MoveFolderButton;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label ScanFailed;
    }
}