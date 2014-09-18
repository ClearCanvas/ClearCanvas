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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Process
{
	/// <summary>
	/// Insert DICOM file into a <see cref="StudyXml"/> file and save to disk.
	/// </summary>
	public class InsertStudyXmlCommand : CommandBase
	{
		#region Private Members

		private readonly DicomFile _file;
		private readonly StudyXml _stream;
		private readonly StudyStorageLocation _studyStorageLocation;

		#endregion

		#region Private Static Members
		private static readonly StudyXmlOutputSettings _outputSettings = ImageServerCommonConfiguration.DefaultStudyXmlOutputSettings;
		#endregion

		#region Properties

		public long FileSize { get; private set; }

		#endregion

		#region Constructors

		public InsertStudyXmlCommand(DicomFile file, StudyXml stream, StudyStorageLocation storageLocation)
			: base("Insert into Study XML", true)
		{
			Platform.CheckForNullReference(file, "Dicom File object");
			Platform.CheckForNullReference(stream, "StudyStream object");
			Platform.CheckForNullReference(storageLocation, "Study Storage Location");

			_file = file;
			_stream = stream;
			_studyStorageLocation = storageLocation;
		}

		#endregion

		#region Private Methods

		private static void WriteStudyStream(string streamFile, string gzStreamFile, StudyXml theStream)
		{
			var theMemento = theStream.GetMemento(_outputSettings);

			// allocate the random number generator here, in case we need it below
			Random rand = new Random();
			string tmpStreamFile = streamFile + "_tmp";
			string tmpGzStreamFile =  gzStreamFile + "_tmp";
			for (int i = 0; ; i++)
				try
				{
					FileUtils.Delete(tmpStreamFile);
					FileUtils.Delete(tmpGzStreamFile);

					using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(tmpStreamFile, FileMode.CreateNew),
					                  gzipStream = FileStreamOpener.OpenForSoleUpdate(tmpGzStreamFile, FileMode.CreateNew))
					{
						StudyXmlIo.WriteXmlAndGzip(theMemento, xmlStream, gzipStream);
						xmlStream.Close();
						gzipStream.Close();
					}

					FileUtils.Delete(streamFile);
					File.Move(tmpStreamFile, streamFile);
					FileUtils.Delete(gzStreamFile);
					File.Move(tmpGzStreamFile,gzStreamFile);
					return;
				}
				catch (IOException)
				{
					if (i < 5)
					{
						Thread.Sleep(rand.Next(5, 50)); // Sleep 5-50 milliseconds
						continue;
					}

					throw;
				}
		}
       
		#endregion

		#region Overridden Protected Methods

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			FileSize = 0;
			var finfo = new FileInfo(_file.Filename);
			if (finfo.Exists)
				FileSize = finfo.Length;

			// Setup the insert parameters
			if (false == _stream.AddFile(_file, FileSize, _outputSettings))
			{
				Platform.Log(LogLevel.Error, "Unexpected error adding SOP to XML Study Descriptor for file {0}",
				             _file.Filename);
				throw new ApplicationException("Unexpected error adding SOP to XML Study Descriptor for SOP: " +
				                               _file.MediaStorageSopInstanceUid);
			}

			// Write it back out.  We flush it out with every added image so that if a failure happens,
			// we can recover properly.
			string streamFile =
				Path.Combine(_studyStorageLocation.GetStudyPath(), _studyStorageLocation.StudyInstanceUid + ".xml");
			string gzStreamFile = streamFile + ".gz";

			WriteStudyStream(streamFile, gzStreamFile, _stream);
		}

		protected override void OnUndo()
		{
		    Platform.Log(LogLevel.Info, "Undoing InsertStudyXmlCommand");
			_stream.RemoveFile(_file);

			string streamFile =
				Path.Combine(_studyStorageLocation.GetStudyPath(), _studyStorageLocation.StudyInstanceUid + ".xml");
			string gzStreamFile = streamFile + ".gz";
			WriteStudyStream(streamFile, gzStreamFile, _stream);
		}

		#endregion
	}
}