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
using ClearCanvas.Common;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom.Network.Scu
{
	/// <summary>
	/// Used by <see cref="StorageScu"/> to specify the <see cref="DicomFile"/>s to transfer over the association.
	/// </summary>
	public class StorageInstance
	{
		#region Private Variables...

		private string _filename;
		private DicomStreamOpener _streamOpener = null;
		private SopClass _sopClass;
		private TransferSyntax _syntax;
		private bool _infoLoaded = false;
		private DicomFile _dicomFile = null;

		#endregion

		#region Public Properties

		/// <summary>
		/// The filename of the storage instance.
		/// </summary>
		public string Filename
		{
			get { return _filename; }
			set
			{
				_filename = value;
				_streamOpener = null;
			}
		}

		public DicomStreamOpener StreamOpener
		{
			get { return _streamOpener ?? (_streamOpener = !string.IsNullOrEmpty(_filename) ? DicomStreamOpener.Create(_filename) : null); }
			set
			{
				_streamOpener = value;
				_filename = String.Empty;
			}
		}

		/// <summary>
		/// The <see cref="SopClass"/> of the storage instance.
		/// </summary>
		public SopClass SopClass
		{
			get { return _sopClass; }
			set
			{
				_sopClass = value;
				if (_sopClass != null && _syntax != null)
					_infoLoaded = true;
			}
		}

		/// <summary>
		/// The SOP Instance Uid of the storage instance.
		/// </summary>
		public string SopInstanceUid { get; set; }

		/// <summary>
		/// The Study Instance Uid of the storage instance.
		/// </summary>
		public string StudyInstanceUid { get; set; }

		/// <summary>
		/// The Study Instance Uid of the storage instance.
		/// </summary>
		public string SeriesInstanceUid { get; set; }

		/// <summary>
		/// The Patient's Name of the storage instance.
		/// </summary>
		public string PatientsName { get; set; }

		/// <summary>
		/// The Patient Id of the storage instance.
		/// </summary>
		public string PatientId { get; set; }

		/// <summary>
		/// The <see cref="TransferSyntax"/> of the storage instance.
		/// </summary>
		public TransferSyntax TransferSyntax
		{
			get { return _syntax; }
			set
			{
				_syntax = value;
				if (_sopClass != null && _syntax != null)
					_infoLoaded = true;
			}
		}

		/// <summary>
		/// The <see cref="DicomStatus"/> returned from the remote SCP when the storage instance was transferred.
		/// </summary>
		public DicomStatus SendStatus { get; set; }

		/// <summary>
		/// The Message ID assigned to the instance when transferred.  Used to identify the response.
		/// </summary>
		public ushort SentMessageId { get; set; }

		/// <summary>
		/// An extended failure description if <see cref="SendStatus"/> is a failure status.
		/// </summary>
		public string ExtendedFailureDescription { get; set; }

		/// <summary>
		/// Offset to the dataset in the <see cref="DicomFile"/>, used if the file is streamed directly to the network
		/// </summary>
		public long MetaInfoFileLength { get; set; }

		/// <summary>
		/// Returns true if the <see cref="DicomFile"/> is already loaded into memory
		/// </summary>
		public bool FileIsLoaded
		{
			get { return _dicomFile != null; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dicomFile"></param>
		public StorageInstance(DicomFile dicomFile)
		{
			_dicomFile = dicomFile;

			string sopClassInFile = _dicomFile.DataSet[DicomTags.SopClassUid].ToString();
			if (!sopClassInFile.Equals(_dicomFile.SopClass.Uid))
			{
				Platform.Log(LogLevel.Warn, "SOP Class in Meta Info ({0}) does not match SOP Class in DataSet ({1})",
				             _dicomFile.SopClass.Uid, sopClassInFile);
				_sopClass = SopClass.GetSopClass(sopClassInFile);
				if (_sopClass == null)
				{
					Platform.Log(LogLevel.Warn, "Unknown SOP Class in dataset, reverting to meta info:  {0}", sopClassInFile);
					_sopClass = _dicomFile.SopClass;
				}
			}
			else
				_sopClass = _dicomFile.SopClass;

			_syntax = _dicomFile.TransferSyntax;
			SopInstanceUid = _dicomFile.MediaStorageSopInstanceUid;
			Filename = dicomFile.Filename;

			StudyInstanceUid = _dicomFile.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
			SeriesInstanceUid = _dicomFile.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
			PatientsName = _dicomFile.DataSet[DicomTags.PatientsName].GetString(0, string.Empty);
			PatientId = _dicomFile.DataSet[DicomTags.PatientId].GetString(0, string.Empty);
			_infoLoaded = true;
		}

		public StorageInstance(DicomMessage msg)
		{
			_sopClass = msg.SopClass;

			_syntax = msg.TransferSyntax;
			SopInstanceUid = msg.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);

			StudyInstanceUid = msg.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
			SeriesInstanceUid = msg.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
			PatientsName = msg.DataSet[DicomTags.PatientsName].GetString(0, string.Empty);
			PatientId = msg.DataSet[DicomTags.PatientId].GetString(0, string.Empty);
			_infoLoaded = true;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="filename"></param>
		public StorageInstance(string filename)
		{
			Filename = filename;
			StudyInstanceUid = string.Empty;
			SeriesInstanceUid = string.Empty;
			SopInstanceUid = string.Empty;
			PatientsName = string.Empty;
			PatientId = string.Empty;
		}

		public StorageInstance(DicomStreamOpener streamOpener)
		{
			StreamOpener = streamOpener;
			StudyInstanceUid = string.Empty;
			SeriesInstanceUid = string.Empty;
			SopInstanceUid = string.Empty;
			PatientsName = string.Empty;
			PatientId = string.Empty;
		}

		/// <summary>
		/// Constructor for primary usage with the <see cref="StorageCommitScu"/> class.
		/// </summary>
		/// <param name="sopClass">The SOP Class for a DICOM instance</param>
		/// <param name="sopInstanceUid">The SOP Instance UID of a DICOM instance</param>
		public StorageInstance(SopClass sopClass, string sopInstanceUid)
		{
			_sopClass = sopClass;
			SopInstanceUid = sopInstanceUid;
			StudyInstanceUid = string.Empty;
			SeriesInstanceUid = string.Empty;
			PatientsName = string.Empty;
			PatientId = string.Empty;
			_filename = string.Empty;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Load a <see cref="DicomFile"/> for the storage instance.
		/// </summary>
		/// <remarks>
		/// If the constructor that supplies a <see cref="DicomFile"/> is used, that file is returned.
		/// Otherwise, the file is loaded and returned.  Note that a reference is not kept for the file
		/// in this case.
		/// </remarks>
		/// <returns></returns>
		public DicomFile LoadFile()
		{
			if (_dicomFile != null)
				return _dicomFile;

			var theFile = new DicomFile();
			theFile.Load(StreamOpener, null, DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);

			// these fields must be loaded for auditing purposes
			StudyInstanceUid = theFile.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
			SeriesInstanceUid = theFile.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
			SopInstanceUid = theFile.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);
			PatientsName = theFile.DataSet[DicomTags.PatientsName].GetString(0, string.Empty);
			PatientId = theFile.DataSet[DicomTags.PatientId].GetString(0, string.Empty);

			MetaInfoFileLength = theFile.MetaInfoFileLength;
			return theFile;
		}

		/// <summary>
		/// Load enough information from the file to allow negotiation of the association.
		/// </summary>
		public void LoadInfo()
		{
			if (_infoLoaded)
				return;

			var theFile = new DicomFile();

			const uint stopTag = DicomTags.StudyId;
			theFile.Load(StreamOpener, DicomTagDictionary.GetDicomTag(stopTag), DicomReadOptions.Default);

			string sopClassInFile = theFile.DataSet[DicomTags.SopClassUid].ToString();
			if (!sopClassInFile.Equals(theFile.SopClass.Uid))
			{
				Platform.Log(LogLevel.Warn, "SOP Class in Meta Info ({0}) does not match SOP Class in DataSet ({1})",
				             theFile.SopClass.Uid, sopClassInFile);
				_sopClass = SopClass.GetSopClass(sopClassInFile);
				if (_sopClass == null)
				{
					Platform.Log(LogLevel.Warn, "Unknown SOP Class in dataset, reverting to meta info:  {0}", sopClassInFile);
					_sopClass = theFile.SopClass;
				}
			}
			else
				_sopClass = theFile.SopClass;

			_syntax = theFile.TransferSyntax;

			// these fields must be loaded for auditing purposes, and LoadFile() may not get called
			StudyInstanceUid = theFile.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
			SeriesInstanceUid = theFile.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
			SopInstanceUid = theFile.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);
			PatientsName = theFile.DataSet[DicomTags.PatientsName].GetString(0, string.Empty);
			PatientId = theFile.DataSet[DicomTags.PatientId].GetString(0, string.Empty);

			MetaInfoFileLength = theFile.MetaInfoFileLength;
			_infoLoaded = true;
		}

		/// <summary>
		/// Ensures the offset of the data set in the source stream is determined.
		/// </summary>
		internal void ParseMetaInfo()
		{
			if (MetaInfoFileLength != 0) return;

			var theFile = new DicomFile();

			const uint stopTag = DicomTags.RelatedGeneralSopClassUid;
			theFile.Load(StreamOpener, DicomTagDictionary.GetDicomTag(stopTag), DicomReadOptions.Default);

			MetaInfoFileLength = theFile.MetaInfoFileLength;
		}

		public override string ToString()
		{
			if (_streamOpener != null) return string.Format("Stream [{0}]", !string.IsNullOrEmpty(SopInstanceUid) ? SopInstanceUid : "Unknown");
			return Filename ?? string.Empty;
		}

		#endregion
	}
}