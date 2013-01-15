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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.Samples
{
	/// <summary>
	/// Simple class for reading DICOMDIR files and sending the images they reference to a remote AE.
	/// </summary>
	public class DicomdirReader
	{
		private readonly string _aeTitle;
		private DicomDirectory _dir;

	    public DicomdirReader(string aeTitle)
		{
		    InstanceRecords = 0;
		    SeriesRecords = 0;
		    StudyRecords = 0;
		    PatientRecords = 0;
		    _aeTitle = aeTitle;
		}

	    public DicomDirectory Dicomdir
		{
			get { return _dir;}
		}

	    public int PatientRecords { get; set; }

	    public int StudyRecords { get; set; }

	    public int SeriesRecords { get; set; }

	    public int InstanceRecords { get; set; }

	    /// <summary>
		/// Load a DICOMDIR
		/// </summary>
		/// <param name="filename"></param>
		public void Load(string filename)
		{
			try
			{
				_dir = new DicomDirectory(_aeTitle);

				_dir.Load(filename);


				// Show a simple traversal
				foreach (DirectoryRecordSequenceItem patientRecord in _dir.RootDirectoryRecordCollection)
				{
					PatientRecords++;
					foreach (DirectoryRecordSequenceItem studyRecord in patientRecord.LowerLevelDirectoryRecordCollection)
					{
						StudyRecords++;
						foreach (DirectoryRecordSequenceItem seriesRecord in studyRecord.LowerLevelDirectoryRecordCollection)
						{
							SeriesRecords++;
							foreach (DirectoryRecordSequenceItem instanceRecord in seriesRecord.LowerLevelDirectoryRecordCollection)
							{
								InstanceRecords++;
							}
						}
					}
				}

				Platform.Log(LogLevel.Info, "Loaded DICOMDIR with {0} Patient Records, {1} Study Records, {2} Series Records, and {3} Image Records",
					PatientRecords,StudyRecords,SeriesRecords,InstanceRecords);

			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception reading DICOMDIR: {0}", filename);
			}
		}

		/// <summary>
		/// Send the images of a loaded DICOMDIR to a remote AE.
		/// </summary>
		/// <param name="rootPath"></param>
		/// <param name="aeTitle"></param>
		/// <param name="host"></param>
		/// <param name="port"></param>
		public void Send(string rootPath, string aeTitle, string host, int port)
		{
			if (_dir == null) return;

            var scu = new StorageScu("DICOMDIR", aeTitle, host, port);

			foreach (DirectoryRecordSequenceItem patientRecord in _dir.RootDirectoryRecordCollection)
			{
				foreach (DirectoryRecordSequenceItem studyRecord in patientRecord.LowerLevelDirectoryRecordCollection)
				{
					foreach (DirectoryRecordSequenceItem seriesRecord in studyRecord.LowerLevelDirectoryRecordCollection)
					{
						foreach (DirectoryRecordSequenceItem instanceRecord in seriesRecord.LowerLevelDirectoryRecordCollection)
						{
							string path = rootPath;

							foreach (string subpath in instanceRecord[DicomTags.ReferencedFileId].Values as string[])
								path = Path.Combine(path, subpath);

							scu.AddStorageInstance(new StorageInstance(path));
						}
					}
				}
			}

			// Do the send
			scu.Send();
		}
	}
}
