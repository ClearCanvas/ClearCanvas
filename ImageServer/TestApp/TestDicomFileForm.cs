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
using System.IO;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class TestDicomFileForm : Form
    {
        public TestDicomFileForm()
        {
            
            InitializeComponent();
        }

        private void checkBoxLoadTest_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                WorkQueueTypeEnum t = WorkQueueTypeEnum.CompressStudy;
                
                using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                {
                    IInsertStudyStorage insert = read.GetBroker<IInsertStudyStorage>();

                    InsertStudyStorageParameters criteria = new InsertStudyStorageParameters();

                    criteria.StudyInstanceUid = "1.2.3.4";
					criteria.FilesystemKey = FilesystemMonitor.Instance.GetFilesystems().GetEnumerator().Current.Filesystem.GetKey();
                    criteria.Folder = "20070101";
                	criteria.StudyStatusEnum = StudyStatusEnum.Online;
                	criteria.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
                    IList<StudyStorageLocation> storage = insert.Find(criteria);

                    StudyStorageLocation storageEntry = storage[0];
                }
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x);
            }
        }



		public static unsafe class CopyClass
        {
            // The unsafe keyword allows pointers to be used within
            // the following method:
            static public void UnsafeCopy(byte[] src, int srcIndex,
                byte[] dst, int dstIndex, int count)
            {
                if (src == null || srcIndex < 0 ||
                    dst == null || dstIndex < 0 || count < 0)
                {
                    throw new ArgumentException();
                }
                int srcLen = src.Length;
                int dstLen = dst.Length;
                if (srcLen - srcIndex < count ||
                    dstLen - dstIndex < count)
                {
                    throw new ArgumentException();
                }


                // The following fixed statement pins the location of
                // the src and dst objects in memory so that they will
                // not be moved by garbage collection.          
                fixed (byte* pSrc = src, pDst = dst)
                {
                    byte* ps = pSrc;
                    byte* pd = pDst;

                    // Loop over the count in blocks of 4 bytes, copying an
                    // integer (4 bytes) at a time:
                    for (int n = 0; n < count / 4; n++)
                    {
                        *((int*)pd) = *((int*)ps);
                        pd += 4;
                        ps += 4;
                    }

                    // Complete the copy by moving any bytes that weren't
                    // moved in blocks of 4:
                    for (int n = 0; n < count % 4; n++)
                    {
                        *pd = *ps;
                        pd++;
                        ps++;
                    }
                }
            }

        }

        private void CopyTest()
        {
            byte[] source = new byte[200*1024*1024];
            byte[] dest = new byte[200*1024*1024];

            for (int i = 0; i < source.Length; i++)
                source[i] = (byte)(i%256);

            long startTicks = DateTime.Now.Ticks;

            Buffer.BlockCopy(source, 0, dest, 0, source.Length);

            long endTicks = DateTime.Now.Ticks;

            Platform.Log(LogLevel.Info, "BlockCopy : {0} ms", new TimeSpan(endTicks - startTicks).TotalMilliseconds);

            startTicks = DateTime.Now.Ticks;

            Array.Copy(source, dest, source.Length);

            endTicks = DateTime.Now.Ticks;

            Platform.Log(LogLevel.Info, "ArrayCopy : {0} ms", new TimeSpan(endTicks - startTicks).TotalMilliseconds);

            startTicks = DateTime.Now.Ticks;

            CopyClass.UnsafeCopy(source, 0, dest, 0, source.Length);

            endTicks = DateTime.Now.Ticks;

            Platform.Log(LogLevel.Info, "UnsafeCopy : {0} ms", new TimeSpan(endTicks - startTicks).TotalMilliseconds);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //CopyTest();

			openFileDialog.Filter = "DICOM|*.dcm";
			openFileDialog.ShowDialog();

            if (!File.Exists(openFileDialog.FileName))
                return;

            DicomFile dicomFile = new DicomFile(openFileDialog.FileName);

            dicomFile.Load();

            //dicomFile.DataSet[DicomTags.PatientsName].SetEmptyValue();
			dicomFile.DataSet[DicomTags.Modality].SetEmptyValue();
        	//dicomFile.DataSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);

			try
			{
				dicomFile.DataSet[DicomTags.PatientsBirthDate].SetStringValue("12/12/2009");
			}
			catch (Exception)
			{
				
			}

			saveFileDialog.Filter = "DICOM|*.dcm";
			if (DialogResult.OK == saveFileDialog.ShowDialog())
			{
				dicomFile.Save(saveFileDialog.FileName);
			}
        }

		private void buttonSelectDirectory_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.Description = "Select Source Directory to Scan";
			DialogResult result = folderBrowserDialog.ShowDialog();
			if (result != System.Windows.Forms.DialogResult.OK)
				return;

			DicomFileCleanup cleanup = new DicomFileCleanup();
			cleanup.SourceDirectory = folderBrowserDialog.SelectedPath;


			folderBrowserDialog.Description = "Select Destination Directory to Copy Files";
			result = folderBrowserDialog.ShowDialog();
			if (result != System.Windows.Forms.DialogResult.OK)
				return;

			cleanup.DestinationDirectory = folderBrowserDialog.SelectedPath;

			cleanup.Scan();
		}

		private void buttonSearchForStudies_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.Description = "Select Source Directory to Search";
			DialogResult result = folderBrowserDialog.ShowDialog();
			if (result != System.Windows.Forms.DialogResult.OK)
				return;

			DicomFileCleanup cleanup = new DicomFileCleanup();
			cleanup.SourceDirectory = folderBrowserDialog.SelectedPath;


			folderBrowserDialog.Description = "Select Destination Directory to Copy Found Files";
			result = folderBrowserDialog.ShowDialog();
			if (result != System.Windows.Forms.DialogResult.OK)
				return;

			cleanup.DestinationDirectory = folderBrowserDialog.SelectedPath;

			cleanup.SearchDirectories(new DirectoryInfo(cleanup.SourceDirectory));
		}
    }
}