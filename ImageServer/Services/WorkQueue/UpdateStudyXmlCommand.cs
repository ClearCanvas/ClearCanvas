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

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    public class UpdateStudyXmlCommand : CommandBase
    {
        #region Private Members

        private readonly DicomFile _file;
        private readonly StudyXml _stream;
        private readonly StudyStorageLocation _studyStorageLocation;
    	private DicomAttributeCollection _saveCollection;

        #endregion

		#region Private Static Members
		private static readonly StudyXmlOutputSettings _outputSettings = ImageServerCommonConfiguration.DefaultStudyXmlOutputSettings;
		#endregion

		#region Properties

		public long FileSize { get; private set; }

		#endregion

		#region Constructors

		public UpdateStudyXmlCommand(DicomFile file, StudyXml stream, StudyStorageLocation storageLocation)
            : base("Update Study XML", true)
        {
            Platform.CheckForNullReference(file, "Dicom File object");
            Platform.CheckForNullReference(stream, "StudyStream object");
            Platform.CheckForNullReference(storageLocation, "Study Storage Location");

            _file = file;
            _stream = stream;
            _studyStorageLocation = storageLocation;
        }

        #endregion

		protected override void OnExecute(CommandProcessor theProcessor)
        {
            // Setup the insert parameters
        	string seriesInstanceUid = _file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
        	string sopinstanceUid = _file.MediaStorageSopInstanceUid;
        	FileSize = 0;

			var finfo = new FileInfo(_file.Filename);
			if (finfo.Exists)
				FileSize = finfo.Length;

			// Save the collection for undo purposes
        	SeriesXml seriesXml = _stream[seriesInstanceUid];
			if (seriesXml != null)
			{
				InstanceXml instanceXml = seriesXml[sopinstanceUid];
				if (instanceXml != null)
				{
					_saveCollection = instanceXml.Collection;
				}
			}

            if (false == _stream.RemoveFile(_file))
            {
                Platform.Log(LogLevel.Warn, "SOP was unexpectedly not in XML Study Descriptor for file: {0}",
                             _file.Filename);
            }
			if (false == _stream.AddFile(_file, FileSize, _outputSettings))
            {
                Platform.Log(LogLevel.Error, "Unexpected error adding SOP to XML Study Descriptor for file {0}",
                             _file.Filename);
                throw new ApplicationException("Unexpected error adding SOP to XML Study Descriptor for SOP: " +
                                               _file.MediaStorageSopInstanceUid);
            }
            // Write it back out.  We flush it out with every added image so that if a failure happens,
            // we can recover properly.
            WriteStudyStream(
				Path.Combine(_studyStorageLocation.GetStudyPath(), _studyStorageLocation.StudyInstanceUid + ".xml"),
				Path.Combine(_studyStorageLocation.GetStudyPath(), _studyStorageLocation.StudyInstanceUid + ".xml.gz"),
				_stream);
        }

        protected override void OnUndo()
        {
            _stream.RemoveFile(_file);

	        if (_saveCollection != null)
	        {
		        var file = new DicomFile(_file.Filename, new DicomAttributeCollection(), _saveCollection);
		        long fileSize = 0;
		        var finfo = new FileInfo(file.Filename);
		        if (finfo.Exists)
			        fileSize = finfo.Length;
		        _stream.AddFile(file, fileSize, _outputSettings);
	        }
	        WriteStudyStream(
                Path.Combine(_studyStorageLocation.GetStudyPath(), _studyStorageLocation.StudyInstanceUid + ".xml"),
				Path.Combine(_studyStorageLocation.GetStudyPath(), _studyStorageLocation.StudyInstanceUid + ".xml.gz"),
                _stream);
        }

		private static void WriteStudyStream(string streamFile, string gzStreamFile, StudyXml theStream)
		{
			var theMemento = theStream.GetMemento(_outputSettings);

			// allocate the random number generator here, in case we need it below
			var rand = new Random();
			string tmpStreamFile = streamFile + "_tmp";
			string tmpGzStreamFile = gzStreamFile + "_tmp";
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
					File.Move(tmpGzStreamFile, gzStreamFile);
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
       
    }
}