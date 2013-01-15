#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Utilities.CleanupReconcile
{
    public partial class MainForm : Form
    {
        #region Private Members
        private FolderScanner _scanner;
        private BackgroundTask _moveFolderBackgroundTask;
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }


        private void ScanButton_Click(object sender, EventArgs e)
        {
            _scanner = new FolderScanner();
            _scanner.Path = Path.Text;
            _scanner.ProgressUpdated += ScannerProgressUpdated;
            _scanner.Terminated += ScannerTerminated;
            StopButton.Enabled = true;

            EnableViewResultButtons(false);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            _scanner.StartAsync();
            
        }

        private void EnableViewResultButtons(bool b)
        {
            ShowBackupFilesOnlyCases.Enabled = b;
            ShowStudyWasDeletedCases.Enabled = b;
            ShowStudyResentCases.Enabled = b;
            ShowUnknownProblemCases.Enabled = b;
            ShowEmptyFoldersCase.Enabled = b;
            ShowStudyDoesNotExistCases.Enabled = b;
        }

        void ScannerTerminated(object sender, EventArgs e)
        {
            ScanButton.Text = "Scan";
            StopButton.Enabled = false;

            dataGridView1.DataSource = _scanner.ScanResultSet.Results;
            dataGridView1.Update();

            UpdateScanSummary();
            
            progressBar1.Value = 0;
            EnableViewResultButtons(true);
        }

        void ScannerProgressUpdated(object sender, EventArgs e)
        {
            FolderScanner scanner = sender as FolderScanner;
            progressBar1.Value = scanner.ScanResultSet.Progress.Percent;

            ScanButton.Text = String.Format("Scanning... {0}%", scanner.ScanResultSet.Progress.Percent);

            UpdateScanSummary();
        }

        private void UpdateScanSummary()
        {
            TotalScanned.Text = String.Format("{0}", _scanner.ScanResultSet.TotalScanned);
            ScanFailed.Text = String.Format("{0}", _scanner.ScanResultSet.ScanFailedCount);
            Skipped.Text = String.Format("{0}", _scanner.ScanResultSet.SkippedCount);
            InSIQ.Text = String.Format("{0}", _scanner.ScanResultSet.InSIQCount);
            Orphanned.Text = String.Format("{0}", _scanner.ScanResultSet.TotalScanned - _scanner.ScanResultSet.SkippedCount - _scanner.ScanResultSet.InSIQCount - _scanner.ScanResultSet.ScanFailedCount);
            EmptyCount.Text = String.Format("{0}", _scanner.ScanResultSet.EmptyCount);
            StudyDeletedCount.Text = String.Format("{0}", _scanner.ScanResultSet.DeletedStudyCount);
            BackupOrTempOnlyCount.Text = String.Format("{0}", _scanner.ScanResultSet.BackupOrTempOnlyCount);
            StudyWasResentCount.Text = String.Format("{0}", _scanner.ScanResultSet.StudyWasResentCount);
            UnidentifiedCount.Text = String.Format("{0}", _scanner.ScanResultSet.UnidentifiedCount + _scanner.ScanResultSet.InWorkQueueCount);
            StudyDoesNotExistCount.Text = String.Format("{0}", _scanner.ScanResultSet.StudyDoesNotExistCount);
        }

        private void ReconcileCleanupUtils_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_scanner!=null)
            {
                _scanner.Stop();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (_scanner != null)
                _scanner.Stop();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var gv = (DataGridView) sender;
            var list = gv.DataSource as List<ScanResultEntry>;

            if (list!=null)
            {
                var result = list[e.RowIndex];

                if (result.IsEmpty)
                {
                    e.CellStyle.BackColor = Color.Green;
                }
                else if (result.BackupFilesOnly)
                {
                    e.CellStyle.BackColor = Color.YellowGreen;
                }
                else if (result.StudyWasOnceDeleted)
                {
                    e.CellStyle.BackColor = Color.Yellow;
                }
                
            }

        }

        private void ShowStudyResentCases_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.StudyWasResent);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            dataGridView1.Refresh();
            MoveFolderButton.Enabled = true;
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowStudyWasDeletedCases_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.StudyWasOnceDeleted);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            MoveFolderButton.Enabled = true;
            dataGridView1.Refresh();
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowBackupFilesOnlyCases_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.BackupFilesOnly);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            dataGridView1.Refresh();
            MoveFolderButton.Enabled = true;
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowUnknownProblemCases_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.Undetermined || item.IsInWorkQueue);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            dataGridView1.Refresh();
            MoveFolderButton.Enabled = false;
            tabControl1.SelectedTab = ResultsTab;
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var gv = (DataGridView)sender;
            gv.Sort(gv.Columns[e.ColumnIndex], System.ComponentModel.ListSortDirection.Ascending);
        }

        private void ShowStudyDoesNotExistCases_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.StudyNoLongerExists);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            dataGridView1.Refresh();
            MoveFolderButton.Enabled = true;
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowEmptyFoldersCase_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.IsEmpty);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            dataGridView1.Refresh();
            MoveFolderButton.Enabled = true;
            tabControl1.SelectedTab = ResultsTab;
        }

        private void MoveFolders(SortableResultList<ScanResultEntry> list)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            _moveFolderBackgroundTask = new BackgroundTask(DoMoveFolderAsync, true, list);
            _moveFolderBackgroundTask.ProgressUpdated += delegate(object sender, BackgroundTaskProgressEventArgs ev)
                                                             {
                                                                 progressBar1.Value = ev.Progress.Percent;
                                                             };

            _moveFolderBackgroundTask.Terminated += delegate
                                                        {
                                                            progressBar1.Value = 0;
                                                            MoveFolderButton.Text = "Move Folders";
                                                            MoveFolderButton.Enabled = true;
                                                        };

            MoveFolderButton.Text = "Stop";
            _moveFolderBackgroundTask.Run();
        }

        private void DoMoveFolderAsync(IBackgroundTaskContext context)
        {
            var list = context.UserState as SortableResultList<ScanResultEntry>;
            int entryCount = list.Count;
            int counter = 0;
            foreach (ScanResultEntry entry in list)
            {
                if (context.CancelRequested)
                    break;

                DirectoryInfo dir = new DirectoryInfo(entry.Path);
                if (dir.Exists)
                {
                    string path = System.IO.Path.Combine(folderBrowserDialog1.SelectedPath, dir.Name);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    DirectoryUtility.Move(entry.Path, path);
                    counter++;
                    context.ReportProgress(new BackgroundTaskProgress(counter*100/entryCount, String.Empty));
                }

            }
        }

        private void MoveFolderButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button.Text != "Stop")
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    var list = dataGridView1.DataSource as SortableResultList<ScanResultEntry>;
                    MoveFolders(list);
                }
            }
            else
            {
                MoveFolderButton.Text = "Stopping...";
                MoveFolderButton.Enabled = false;
                _moveFolderBackgroundTask.RequestCancel();
            }
        }
    }
}