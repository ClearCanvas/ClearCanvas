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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Class representing a DICOM Part 10 Format File.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class represents a DICOM Part 10 format file.  The class inherits off an AbstractMessage class.  The class contains
	/// <see cref="DicomAttributeCollection"/> instances for the Meta Info (group 0x0002 attributes) and Data Set. 
	/// </para>
	/// </remarks>
	public class DicomFile : DicomMessageBase
	{
		#region Private Members

		private String _filename = String.Empty;
		private DicomStreamOpener _streamOpener;

		#endregion

		#region Constructors

		/// <summary>
		/// Create a DicomFile instance from existing MetaInfo and DataSet.
		/// </summary>
		/// <param name="filename">The name for the file.</param>
		/// <param name="metaInfo">A <see cref="DicomAttributeCollection"/> for the MetaInfo (group 0x0002 attributes).</param>
		/// <param name="dataSet">A <see cref="DicomAttributeCollection"/> for the DataSet.</param>
		public DicomFile(String filename, DicomAttributeCollection metaInfo, DicomAttributeCollection dataSet)
		{
			MetaInfo = metaInfo;
			DataSet = dataSet;
			_filename = filename;

			if (!MetaInfo.Contains(DicomTags.ImplementationVersionName))
				ImplementationVersionName = DicomImplementation.Version;
			if (!MetaInfo.Contains(DicomTags.ImplementationClassUid))
				ImplementationClassUid = DicomImplementation.ClassUID.UID;

			// If the meta info doesn't already specify the transfer syntax, give it the default transfer syntax of ELE
			if (string.IsNullOrEmpty(MetaInfo[DicomTags.TransferSyntaxUid].ToString()))
				MetaInfo[DicomTags.TransferSyntaxUid].SetStringValue(TransferSyntax.ExplicitVrLittleEndian.UidString);

			if (!MetaInfo.Contains(DicomTags.FileMetaInformationVersion))
				MetaInfo[DicomTags.FileMetaInformationVersion].Values = new byte[] {0x00, 0x01};
		}

		/// <summary>
		/// Create a new empty DICOM Part 10 format file.
		/// </summary>
		/// <param name="filename"></param>
		public DicomFile(String filename)
		{
			MetaInfo = new DicomAttributeCollection(0x00020000, 0x0002FFFF);
			DataSet = new DicomAttributeCollection(0x00040000, 0xFFFFFFFF);

			ImplementationVersionName = DicomImplementation.Version;
			ImplementationClassUid = DicomImplementation.ClassUID.UID;
			MetaInfo[DicomTags.TransferSyntaxUid].SetStringValue(TransferSyntax.ExplicitVrLittleEndian.UidString);
			MetaInfo[DicomTags.FileMetaInformationVersion].Values = new byte[] {0x00, 0x01};

			_filename = filename;
		}

		/// <summary>
		/// Create a new empty DICOM Part 10 format file.
		/// </summary>
		public DicomFile()
		{
			MetaInfo = new DicomAttributeCollection(0x00020000, 0x0002FFFF);
			DataSet = new DicomAttributeCollection(0x00040000, 0xFFFFFFFF);

			ImplementationVersionName = DicomImplementation.Version;
			ImplementationClassUid = DicomImplementation.ClassUID.UID;
			MetaInfo[DicomTags.TransferSyntaxUid].SetStringValue(TransferSyntax.ExplicitVrLittleEndian.UidString);
			MetaInfo[DicomTags.FileMetaInformationVersion].Values = new byte[] {0x00, 0x01};

			_filename = String.Empty;
		}

		/// <summary>
		/// Creates a new DicomFile instance from an existing <see cref="DicomMessage"/> instance.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This routine assigns the existing <see cref="DicomMessage.DataSet"/> into the new 
		/// DicomFile instance.
		/// </para>
		/// <para>
		/// A new <see cref="DicomAttributeCollection"/> is created for the MetaInfo.  The 
		/// Media Storage SOP Instance UID, Media Storage SOP Class UID, Implementation Version Name,
		/// and Implementation Class UID tags are automatically set in the meta information.
		/// </para>
		/// </remarks>
		/// <param name="msg"></param>
		/// <param name="filename"></param>
		public DicomFile(DicomMessage msg, String filename)
		{
			MetaInfo = new DicomAttributeCollection(0x00020000, 0x0002FFFF);
			DataSet = msg.DataSet;

			MediaStorageSopInstanceUid = msg.AffectedSopInstanceUid;
			MediaStorageSopClassUid = msg.AffectedSopClassUid;
			ImplementationVersionName = DicomImplementation.Version;
			ImplementationClassUid = DicomImplementation.ClassUID.UID;
			if (msg.TransferSyntax.Encapsulated)
				MetaInfo[DicomTags.TransferSyntaxUid].SetStringValue(msg.TransferSyntax.UidString);
			else
				MetaInfo[DicomTags.TransferSyntaxUid].SetStringValue(TransferSyntax.ExplicitVrLittleEndian.UidString);
			MetaInfo[DicomTags.FileMetaInformationVersion].Values = new byte[] {0x00, 0x01};

			_filename = filename;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Indicates if <see cref="Load"/> has been called on this file.
		/// </summary>
		public bool Loaded { get; set; }

		/// <summary>
		/// The length of the MetaInfo in the file.  This value is only valid if <see cref="DicomFile.Load"/>  or if <see cref="DicomFile.Save"/> has been called on the file.
		/// </summary>
		public long MetaInfoFileLength { get; set; }

		/// <summary>
		/// The filename of the file.
		/// </summary>
		/// <remarks>
		/// This property sets/gets the filename associated with the file.
		/// </remarks>
		public String Filename
		{
			get { return _filename; }
			set
			{
				_filename = value;
				_streamOpener = null;
			}
		}

		/// <summary>
		/// The SOP Class of the file.
		/// </summary>
		/// <remarks>
		/// This property returns a <see cref="SopClass"/> object for the sop class
		/// encoded in the tag Media Storage SOP Class UID (0002,0002).
		/// </remarks>
		public SopClass SopClass
		{
			get
			{
				String sopClassUid = MetaInfo[DicomTags.MediaStorageSopClassUid].GetString(0, String.Empty);

				SopClass sop = SopClass.GetSopClass(sopClassUid) ??
				               new SopClass("Unknown Sop Class", sopClassUid, SopClassCategory.Uncategorized);

				return sop;
			}
		}

		/// <summary>
		/// The transfer syntax the file is encoded in.
		/// </summary>
		/// <remarks>
		/// This property returns a TransferSyntax object for the transfer syntax encoded 
		/// in the tag Transfer Syntax UID (0002,0010).
		/// </remarks>
		public override TransferSyntax TransferSyntax
		{
			get
			{
				String transferSyntaxUid = MetaInfo[DicomTags.TransferSyntaxUid];

				return TransferSyntax.GetTransferSyntax(transferSyntaxUid);
			}
			set { MetaInfo[DicomTags.TransferSyntaxUid].SetStringValue(value.UidString); }
		}

		internal DicomStreamOpener StreamOpener
		{
			get { return _streamOpener ?? (_streamOpener = (!string.IsNullOrEmpty(_filename) ? DicomStreamOpener.Create(_filename) : null)); }
			set
			{
				_streamOpener = value;
				_filename = String.Empty;
			}
		}

		#endregion

		#region Meta Info Properties

		/// <summary>
		/// Uniquiely identifies the SOP Class associated with the Data Set.  SOP Class UIDs allowed for 
		/// media storage are specified in PS3.4 of the DICOM Standard - Media Storage Application Profiles.
		/// </summary>
		public string MediaStorageSopClassUid
		{
			get { return MetaInfo[DicomTags.MediaStorageSopClassUid].GetString(0, String.Empty); }
			set { MetaInfo[DicomTags.MediaStorageSopClassUid].Values = value; }
		}

		/// <summary>
		/// Uniquiely identifies the SOP Instance associated with the Data Set placed in the file and following the File Meta Information.
		/// </summary>
		public string MediaStorageSopInstanceUid
		{
			get { return MetaInfo[DicomTags.MediaStorageSopInstanceUid].GetString(0, String.Empty); }
			set { MetaInfo[DicomTags.MediaStorageSopInstanceUid].Values = value; }
		}

		/// <summary>
		/// Uniquely identifies the implementation which wrote this file and its content.  It provides an 
		/// unambiguous identification of the type of implementation which last wrote the file in the 
		/// event of interchagne problems.  It follows the same policies as defined by PS 3.7 of the DICOM Standard
		/// (association negotiation).
		/// </summary>
		public string ImplementationClassUid
		{
			get { return MetaInfo[DicomTags.ImplementationClassUid].GetString(0, String.Empty); }
			set { MetaInfo[DicomTags.ImplementationClassUid].Values = value; }
		}

		/// <summary>
		/// Identifies a version for an Implementation Class UID (002,0012) using up to 
		/// 16 characters of the repertoire.  It follows the same policies as defined in 
		/// PS 3.7 of the DICOM Standard (association negotiation).
		/// </summary>
		public string ImplementationVersionName
		{
			get { return MetaInfo[DicomTags.ImplementationVersionName].GetString(0, String.Empty); }
			set { MetaInfo[DicomTags.ImplementationVersionName].Values = value; }
		}

		/// <summary>
		/// Uniquely identifies the Transfer Syntax used to encode the following Data Set.  
		/// This Transfer Syntax does not apply to the File Meta Information.
		/// </summary>
		public string TransferSyntaxUid
		{
			get { return MetaInfo[DicomTags.TransferSyntaxUid].GetString(0, String.Empty); }
			set { MetaInfo[DicomTags.TransferSyntaxUid].Values = value; }
		}

		/// <summary>
		/// The DICOM Application Entity (AE) Title of the AE which wrote this file's 
		/// content (or last updated it).  If used, it allows the tracing of the source 
		/// of errors in the event of media interchange problems.  The policies associated
		/// with AE Titles are the same as those defined in PS 3.8 of the DICOM Standard. 
		/// </summary>
		public string SourceApplicationEntityTitle
		{
			get { return MetaInfo[DicomTags.SourceApplicationEntityTitle].GetString(0, String.Empty); }
			set { MetaInfo[DicomTags.SourceApplicationEntityTitle].Values = value; }
		}

		/// <summary>
		/// Identifies a version for an Implementation Class UID (002,0012) using up to 
		/// 16 characters of the repertoire.  It follows the same policies as defined in 
		/// PS 3.7 of the DICOM Standard (association negotiation).
		/// </summary>
		public string PrivateInformationCreatorUid
		{
			get { return MetaInfo[DicomTags.PrivateInformationCreatorUid].GetString(0, String.Empty); }
			set { MetaInfo[DicomTags.PrivateInformationCreatorUid].Values = value; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Load a DICOM file with the default <see cref="DicomReadOptions"/> set.
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, the routine will assume
		/// the file is not a Part 10 format file, and is instead encoded as just a DataSet
		/// with the transfer syntax set to Implicit VR Little Endian.
		/// </remarks>
		/// <param name="filename">The path of the file to load.</param>
		public void Load(string filename)
		{
			Platform.CheckForEmptyString(filename, "filename");
			Filename = filename;
			Load(DicomReadOptions.Default);
		}

		/// <summary>
		/// Load a DICOM file (as set by the <see cref="Filename"/> property) with the 
		/// default <see cref="DicomReadOptions"/> set.
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, the routine will assume
		/// the file is not a Part 10 format file, and is instead encoded as just a DataSet
		/// with the transfer syntax set to Implicit VR Little Endian.
		/// </remarks>
		public void Load()
		{
			Load(DicomReadOptions.Default);
		}

		/// <summary>
		/// Load a DICOM file.
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, the routine will assume
		/// the file is not a Part 10 format file, and is instead encoded as just a DataSet
		/// with the transfer syntax set to Implicit VR Little Endian.
		/// </remarks>
		/// <param name="options">The options to use when reading the file.</param>
		/// <param name="filename">The path of the file to load.</param>
		public void Load(DicomReadOptions options, string filename)
		{
			Platform.CheckForEmptyString(filename, "filename");
			Filename = filename;
			Load(null, options);
		}

		/// <summary>
		/// Load a DICOM file (as set by the <see cref="Filename"/> property).
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, the routine will assume
		/// the file is not a Part 10 format file, and is instead encoded as just a DataSet
		/// with the transfer syntax set to Implicit VR Little Endian.
		/// </remarks>
		/// <param name="options">The options to use when reading the file.</param>
		public void Load(DicomReadOptions options)
		{
			Load(null, options);
		}

		/// <summary>
		/// Load a DICOM file.
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, the routine will assume
		/// the file is not a Part 10 format file, and is instead encoded as just a DataSet
		/// with the transfer syntax set to Implicit VR Little Endian.
		/// </remarks>
		/// <param name="stopTag">A tag to stop at when reading the file.  See the constants in <see cref="DicomTags"/>.</param>
		/// <param name="options">The options to use when reading the file.</param>
		/// <param name="filename">The path of the file to load.</param>
		public void Load(uint stopTag, DicomReadOptions options, string filename)
		{
			Platform.CheckForEmptyString(filename, "filename");
			Filename = filename;
			Load(stopTag, options);
		}

		/// <summary>
		/// Load a DICOM file (as set by the <see cref="Filename"/> property).
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, the routine will assume
		/// the file is not a Part 10 format file, and is instead encoded as just a DataSet
		/// with the transfer syntax set to Implicit VR Little Endian.
		/// </remarks>
		/// <param name="stopTag">A tag to stop at when reading the file.  See the constants in <see cref="DicomTags"/>.</param>
		/// <param name="options">The options to use when reading the file.</param>
		public void Load(uint stopTag, DicomReadOptions options)
		{
			DicomTag stopDicomTag = DicomTagDictionary.GetDicomTag(stopTag) ??
			                        new DicomTag(stopTag, "Bogus Tag", "BogusTag", DicomVr.NONE, false, 1, 1, false);
			Load(stopDicomTag, options);
		}

		/// <summary>
		/// Load a DICOM file (as set by the <see cref="Filename"/> property).
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, the routine will assume
		/// the file is not a Part 10 format file, and is instead encoded as just a DataSet
		/// with the transfer syntax set to Implicit VR Little Endian.
		/// </remarks>
		/// <param name="stopTag"></param>
		/// <param name="options">The options to use when reading the file.</param>
		/// <param name="filename">The path of the file to load.</param>
		public void Load(DicomTag stopTag, DicomReadOptions options, string filename)
		{
			Platform.CheckForEmptyString(filename, "filename");
			Filename = filename;
			Load(stopTag, options);
		}

		/// <summary>
		/// Load a DICOM file (as set by the <see cref="Filename"/> property).
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, the routine will assume
		/// the file is not a Part 10 format file, and is instead encoded as just a DataSet
		/// with the transfer syntax set to Implicit VR Little Endian.
		/// </remarks>
		/// <param name="stopTag"></param>
		/// <param name="options">The options to use when reading the file.</param>
		public void Load(DicomTag stopTag, DicomReadOptions options)
		{
			var streamOpener = StreamOpener;
			Platform.CheckForNullReference(streamOpener, "filename"); // the only reason why stream opener is null here is because filename is empty
			using (var stream = streamOpener.Open())
			{
				LoadCore(stream, streamOpener, stopTag, options);
				stream.Close();
			}
		}

		/// <summary>
		/// Load a DICOM file from an input stream.
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, and 
		/// <see cref="Stream.CanSeek"/> is true for <paramref name="stream"/>, 
		/// the routine will assume the file is not a Part 10 format file, and is 
		/// instead encoded as just a DataSet with the transfer syntax set to 
		/// Implicit VR Little Endian.
		/// </remarks>
		/// <param name="stream">The input stream to read from.</param>
		public void Load(Stream stream)
		{
			const DicomReadOptions options = DicomReadOptions.Default;
			Platform.CheckForNullReference(stream, "stream");
			LoadCore(stream, null, null, options);
		}

		/// <summary>
		/// Load a DICOM file from an input stream, given a delegate to open the stream.
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, and 
		/// <see cref="Stream.CanSeek"/> is true for the stream returned by <paramref name="streamOpener"/>, 
		/// the routine will assume the file is not a Part 10 format file, and is 
		/// instead encoded as just a DataSet with the transfer syntax set to 
		/// Implicit VR Little Endian.
		/// 
		/// Also, if you are using the <see cref="DicomReadOptions.StorePixelDataReferences"/> option with
		/// a <see cref="Stream"/> as opposed to simply a file name, you must use this method so that the
		/// stream can be reopenened internally whenever pixel data is accessed.
		/// </remarks>
		/// <param name="streamOpener">A delegate that opens the stream to read from.</param>
		/// <param name="stopTag">The dicom tag to stop the reading at.</param>
		/// <param name="options">The dicom read options to consider.</param>
		public void Load(DicomStreamOpener streamOpener, DicomTag stopTag, DicomReadOptions options)
		{
			Platform.CheckForNullReference(streamOpener, "streamOpener");
			StreamOpener = streamOpener;
			using (var stream = streamOpener.Open())
			{
				LoadCore(stream, streamOpener, stopTag, options);
				stream.Close();
			}
		}

		/// <summary>
		/// Load a DICOM file from an input stream.
		/// </summary>
		/// <remarks>
		/// Note:  If the file does not contain DICM encoded in it, and 
		/// <see cref="Stream.CanSeek"/> is true for <paramref name="stream"/>, 
		/// the routine will assume the file is not a Part 10 format file, and is 
		/// instead encoded as just a DataSet with the transfer syntax set to 
		/// Implicit VR Little Endian.
		/// 
		/// Also, this overload cannot be used directly with <see cref="DicomReadOptions.StorePixelDataReferences"/>,
		/// as there must be a way to re-open the same stream at a later time. If the option is required,
		/// use the <see cref="Load(Func{Stream}, DicomTag, DicomReadOptions)">overload</see> that accepts a delegate for opening the stream.
		/// </remarks>
		/// <param name="stream">The input stream to read from.</param>
		/// <param name="stopTag">The dicom tag to stop the reading at.</param>
		/// <param name="options">The dicom read options to consider.</param>
		public void Load(Stream stream, DicomTag stopTag, DicomReadOptions options)
		{
			Platform.CheckForNullReference(stream, "stream");
			LoadCore(stream, null, stopTag, options);
		}

		private void LoadCore(Stream stream, DicomStreamOpener streamOpener, DicomTag stopTag, DicomReadOptions options)
		{
			// TODO CR (24 Jan 2014): DICOM stream read only uses tag value, so the real implementation should be the uint overload!
			if (stopTag == null)
				stopTag = new DicomTag(0xFFFFFFFF, "Bogus Tag", "BogusTag", DicomVr.NONE, false, 1, 1, false);

			DicomStreamReader dsr;

			var iStream = stream ?? streamOpener.Open();
			if (iStream.CanSeek)
			{
				iStream.Seek(128, SeekOrigin.Begin);
				if (!FileHasPart10Header(iStream))
				{
					if (!Flags.IsSet(options, DicomReadOptions.ReadNonPart10Files))
						throw new DicomException(String.Format("File is not part 10 format file: {0}", Filename));

					iStream.Seek(0, SeekOrigin.Begin);
					dsr = new DicomStreamReader(iStream)
					      	{
					      		StreamOpener = streamOpener,
					      		TransferSyntax = TransferSyntax.ImplicitVrLittleEndian,
					      		Dataset = DataSet
					      	};
					DicomReadStatus stat = dsr.Read(stopTag, options);
					if (stat != DicomReadStatus.Success)
					{
						Platform.Log(LogLevel.Error, "Unexpected error when reading file: {0}", Filename);
						throw new DicomException("Unexpected read error with file: " + Filename);
					}

					TransferSyntax = TransferSyntax.ImplicitVrLittleEndian;
					if (DataSet.Contains(DicomTags.SopClassUid))
						MediaStorageSopClassUid = DataSet[DicomTags.SopClassUid].ToString();
					if (DataSet.Contains(DicomTags.SopInstanceUid))
						MediaStorageSopInstanceUid = DataSet[DicomTags.SopInstanceUid].ToString();

					Loaded = true;
					return;
				}
			}
			else
			{
				// TODO CR (04 Apr 2014): this code here is almost identical to the seekable stream above, except that we use the 4CC wrapper
				// we can combine these two when we trust that the wrapper works in all cases
				iStream = FourCcReadStream.Create(iStream);

				// Read the 128 byte header first, then check for DICM
				iStream.SeekEx(128, SeekOrigin.Begin);

				if (!FileHasPart10Header(iStream))
				{
					if (!Flags.IsSet(options, DicomReadOptions.ReadNonPart10Files))
						throw new DicomException(String.Format("File is not part 10 format file: {0}", Filename));

					iStream.Seek(0, SeekOrigin.Begin);
					dsr = new DicomStreamReader(iStream)
					      	{
					      		StreamOpener = streamOpener,
					      		TransferSyntax = TransferSyntax.ImplicitVrLittleEndian,
					      		Dataset = DataSet
					      	};
					DicomReadStatus stat = dsr.Read(stopTag, options);
					if (stat != DicomReadStatus.Success)
					{
						Platform.Log(LogLevel.Error, "Unexpected error when reading file: {0}", Filename);
						throw new DicomException("Unexpected read error with file: " + Filename);
					}

					TransferSyntax = TransferSyntax.ImplicitVrLittleEndian;
					if (DataSet.Contains(DicomTags.SopClassUid))
						MediaStorageSopClassUid = DataSet[DicomTags.SopClassUid].ToString();
					if (DataSet.Contains(DicomTags.SopInstanceUid))
						MediaStorageSopInstanceUid = DataSet[DicomTags.SopInstanceUid].ToString();

					Loaded = true;
					return;
				}
			}

			dsr = new DicomStreamReader(iStream)
			      	{
			      		TransferSyntax = TransferSyntax.ExplicitVrLittleEndian,
			      		StreamOpener = streamOpener,
			      		Dataset = MetaInfo
			      	};

			DicomReadStatus readStat =
				dsr.Read(new DicomTag(0x0002FFFF, "Bogus Tag", "BogusTag", DicomVr.UNvr, false, 1, 1, false), options);
			if (readStat != DicomReadStatus.Success)
			{
				Platform.Log(LogLevel.Error, "Unexpected error when reading file Meta info for file: {0}", Filename);
				throw new DicomException("Unexpected failure reading file Meta info for file: " + Filename);
			}

			MetaInfoFileLength = dsr.EndGroupTwo + 128 + 4;

			dsr.Dataset = DataSet;
			dsr.TransferSyntax = TransferSyntax;
			readStat = dsr.Read(stopTag, options);
			if (readStat != DicomReadStatus.Success)
			{
				Platform.Log(LogLevel.Error, "Unexpected error ({0}) when reading file at offset {2}: {1}", readStat, Filename, dsr.BytesRead);
				throw new DicomException("Unexpected failure (" + readStat + ") reading file at offset " + dsr.BytesRead + ": " + Filename);
			}

			Loaded = true;
		}

		/// <summary>
		/// Internal routine to see if the file is encoded as a DICOM Part 10 format file.
		/// </summary>
		/// <param name="fs">The <see cref="FileStream"/> being used to read the file.</param>
		/// <returns>true if the file has a DICOM Part 10 format file header.</returns>
		protected static bool FileHasPart10Header(Stream fs)
		{
			return (!(fs.ReadByte() != (byte) 'D' ||
			          fs.ReadByte() != (byte) 'I' ||
			          fs.ReadByte() != (byte) 'C' ||
			          fs.ReadByte() != (byte) 'M'));
		}

		/// <summary>
		/// Save the file as a DICOM Part 10 format file (as set by the <see cref="Filename"/> property) with 
		/// the default <see cref="DicomWriteOptions"/>.
		/// </summary>
		/// <returns>true on success, false on failure.</returns>
		public bool Save()
		{
			return Save(DicomWriteOptions.Default);
		}

		/// <summary>
		/// Save the file as a DICOM Part 10 format file (as set by the <see cref="Filename"/> property).
		/// </summary>
		/// <param name="options">The options to use when saving the file.</param>
		/// <returns></returns>
		public bool Save(DicomWriteOptions options)
		{
			string path = Path.GetDirectoryName(Filename);
			if (!string.IsNullOrEmpty(path))
			{
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
			}

			using (FileStream fs = File.Create(Filename))
			{
				bool b = Save(fs, options);
				fs.Flush();
				fs.Close();
				return b;
			}
		}

		/// <summary>
		/// Save the file as a DICOM Part 10 format file with the default <see cref="DicomWriteOptions"/>.
		/// </summary>
		/// <returns>true on success, false on failure.</returns>
		public bool Save(string file)
		{
			if (file == null) throw new ArgumentNullException("file");
			Filename = file;
			return Save(DicomWriteOptions.Default);
		}

		/// <summary>
		/// Save the file as a DICOM Part 10 format file.
		/// </summary>
		/// <param name="options">The options to use when saving the file.</param>
		/// <param name="file">The path of the file to save to.</param>
		/// <returns></returns>
		public bool Save(string file, DicomWriteOptions options)
		{
			if (file == null) throw new ArgumentNullException("file");

			Filename = file;

			using (FileStream fs = File.Create(Filename))
			{
				bool b = Save(fs, options);
				fs.Flush();
				fs.Close();
				return b;
			}
		}

		/// <summary>
		/// Save the file as a DICOM Part 10 format file.
		/// </summary>
		/// <param name="options">The options to use when saving the file.</param>
		/// <param name="iStream">The <see cref="Stream"/> to Save the DICOM file to.</param>
		/// <returns></returns>
		public bool Save(Stream iStream, DicomWriteOptions options)
		{
			if (iStream == null) throw new ArgumentNullException("iStream");

			// Original code has seek() here, but there may be use cases where
			// a user wants to add the file into a stream (that may contain other data)
			// and the seek would cause the method to not support that.
			byte[] prefix = new byte[128];
			iStream.Write(prefix, 0, 128);

			iStream.WriteByte((byte) 'D');
			iStream.WriteByte((byte) 'I');
			iStream.WriteByte((byte) 'C');
			iStream.WriteByte((byte) 'M');

			DicomStreamWriter dsw = new DicomStreamWriter(iStream);
			dsw.Write(TransferSyntax.ExplicitVrLittleEndian,
			          MetaInfo, options | DicomWriteOptions.CalculateGroupLengths);

			MetaInfoFileLength = iStream.Position;

			dsw.Write(TransferSyntax, DataSet, options);

			iStream.Flush();

			return true;
		}

		#endregion

		#region Dump

		/// <summary>
		/// Method to dump the contents of a file to a StringBuilder object.
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="prefix"></param>
		/// <param name="options">The dump options.</param>
		public override void Dump(StringBuilder sb, string prefix, DicomDumpOptions options)
		{
			if (sb == null) throw new NullReferenceException("sb");
			sb.Append(prefix).AppendLine("File: " + Filename).AppendLine();
			sb.Append(prefix).Append("MetaInfo:").AppendLine();
			MetaInfo.Dump(sb, prefix, options);
			sb.AppendLine().Append(prefix).Append("DataSet:").AppendLine();
			DataSet.Dump(sb, prefix, options);
			sb.AppendLine();
		}

		#endregion

		#region FourCcReadStream

		/// <summary>
		/// Used to buffer the first 132 bytes of an unseekable DICOM stream, so that we can reset if it turns out to not be a Part 10 file
		/// </summary>
		private class FourCcReadStream : Stream
		{
			private Stream _realStream;
			private Stream _prefixStream;
			private long _position;

			public static FourCcReadStream Create(Stream realStream)
			{
				var prefixBuffer = new byte[132]; // 128 prefix + 4CC

				var bytesRead = 0;
				while (bytesRead < 132)
				{
					// fill the entire buffer - if Read returns 0, then we encountered an EOF and the stream is definitely not a part 10 file, but might still be valid!
					var read = realStream.Read(prefixBuffer, bytesRead, prefixBuffer.Length - bytesRead);
					if (read == 0) break;
					bytesRead += read;
				}

				return new FourCcReadStream(new MemoryStream(prefixBuffer, 0, bytesRead, false), realStream);
			}

			private FourCcReadStream(Stream prefix, Stream realStream)
			{
				_prefixStream = prefix;
				_realStream = realStream;
				_position = 0;
			}

			public override void Close()
			{
				if (_realStream != null)
					_realStream.Close();

				if (_prefixStream != null)
					_prefixStream.Close();

				base.Close();
			}

			protected override void Dispose(bool disposing)
			{
				if (!disposing) return;

				if (_realStream != null)
				{
					_realStream.Dispose();
					_realStream = null;
				}

				if (_prefixStream != null)
				{
					_prefixStream.Dispose();
					_prefixStream = null;
				}
			}

			public override bool CanRead
			{
				get { return true; }
			}

			public override bool CanSeek
			{
				get { return false; }
			}

			public override bool CanWrite
			{
				get { return false; }
			}

			public override long Length
			{
				get { return _realStream.Length; }
			}

			public override long Position
			{
				get { return _position; }
				set { throw new NotSupportedException(); }
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				var bytesRead = (_position < _prefixStream.Length ? _prefixStream : _realStream).Read(buffer, offset, count);
				_position += bytesRead;
				return bytesRead;
			}

			public override int ReadByte()
			{
				var result = (_position < _prefixStream.Length ? _prefixStream : _realStream).ReadByte();
				if (result >= 0) ++_position;
				return result;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				if (_position <= _prefixStream.Length && offset == 0 && origin == SeekOrigin.Begin)
					return _position = _prefixStream.Position = 0;
				throw new InvalidOperationException("Unable to reset stream when the position has already advanced past the prefix");
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override void Flush()
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}
		}

		#endregion
	}
}