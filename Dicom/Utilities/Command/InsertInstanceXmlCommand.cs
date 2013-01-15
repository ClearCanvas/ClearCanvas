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
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.Dicom.Utilities.Command
{
	/// <summary>
	/// Insert DICOM file into a <see cref="StudyXml"/> file and save to disk.
	/// </summary>
	public class InsertInstanceXmlCommand : CommandBase
	{
		#region Private Members

		private readonly StudyXml _stream;
		private readonly string _path;
	    private string _seriesInstanceUid;
	    private string _sopInstanceUid;
		#endregion


		#region Constructors

		public InsertInstanceXmlCommand(StudyXml stream, string path)
			: base("Insert into Study XML", true)
		{
			Platform.CheckForNullReference(stream, "StudyStream object");
			Platform.CheckForNullReference(path, "Path to DICOM File");

			_stream = stream;
			_path = path;
		}

		#endregion

		
		#region Overridden Protected Methods

		protected override void OnExecute(CommandProcessor theProcessor)
		{
		    if (!File.Exists(_path))
			{
				Platform.Log(LogLevel.Error, "Unexpected error finding file to add to XML {0}", _path);
				throw new ApplicationException("Unexpected error finding file to add to XML {0}" + _path);
			}

			var finfo = new FileInfo(_path);
			long fileSize = finfo.Length;

			var dicomFile = new DicomFile(_path);
			dicomFile.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);

		    _sopInstanceUid = dicomFile.DataSet[DicomTags.SopInstanceUid];
		    _seriesInstanceUid = dicomFile.DataSet[DicomTags.SeriesInstanceUid];

			// Setup the insert parameters
			if (false == _stream.AddFile(dicomFile, fileSize))
			{
				Platform.Log(LogLevel.Error, "Unexpected error adding SOP to XML Study Descriptor for file {0}",
				             _path);
				throw new ApplicationException("Unexpected error adding SOP to XML Study Descriptor for SOP: " +
				                               dicomFile.MediaStorageSopInstanceUid);
			}
		}

		protected override void OnUndo()
		{
		    _stream.RemoveInstance(_seriesInstanceUid, _sopInstanceUid);
		}

		#endregion
	}
}