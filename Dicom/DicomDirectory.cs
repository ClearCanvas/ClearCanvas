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
using ClearCanvas.Common;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// This class reads and/or writes a Dicom Directory file.  
    /// </summary>
    /// <example>
    /// using (DicomDirectory dicomDirectory = new DicomDirectory())
    /// {
    ///     dicomDirectory.SourceApplicationEntityTitle = "UNO";
    ///     dicomDirectory.FileSetId = "My File Set Desc";
    ///     dicomDirectory.AddFile("C:\DicomImages\SomeFile.dcm", "DIR001\\IMAGE001.DCM");
    ///     dicomDirectory.AddFile("C:\DicomImages\AnotherFile.dcm", "DIR002\\IMAGE002.DCM");
    ///     dicomDirectory.AddFile("C:\DicomImages\AnotherFile3.dcm", null);
    ///     dicomDirectory.Save("C:\\Temp\\DICOMDIR");
    /// }
    /// </example>
	/// <example>
	/// using (DicomDirectory dicomDirectory = new DicomDirectory())
	/// {
	///     dicomDirectory.Load("C:\\Temp\\DICOMDIR");
	/// 
	///		int patientRecords = 0;
	///		int studyRecords = 0;
	///		int seriesRecords = 0;
	///		int instanceRecords = 0;
	///
	///		// Show a simple traversal, counting the records at each level
	///		foreach (DirectoryRecordSequenceItem patientRecord in reader.RootDirectoryRecordCollection)
	///		{
	///			patientRecords++;
	///			foreach (DirectoryRecordSequenceItem studyRecord in patientRecord.LowerLevelDirectoryRecordCollection)
	///			{
	///				studyRecords++;
	///				foreach (DirectoryRecordSequenceItem seriesRecord in studyRecord.LowerLevelDirectoryRecordCollection)
	///				{
	///					seriesRecords++;
	///					foreach (DirectoryRecordSequenceItem instanceRecord in seriesRecord.LowerLevelDirectoryRecordCollection)
	///					{
	///						instanceRecords++;
	///					}
	///				}
	///			}
	///		}
	/// }
	/// </example>
	public class DicomDirectory : IDisposable
    {
        #region Private Variables
        /// <summary>The directory record sequence item that all the directory record items gets added to.</summary>
        private readonly DicomAttributeSQ _directoryRecordSequence;

        /// <summary>The Dicom Directory File</summary>
        private DicomFile _dicomDirFile;

        /// <summary>File Name to be saved to (Param to Save method)</summary>
        private string _saveFileName;

        /// <summary>Contains the ongoing fileOffset to determine the offset tags for each Item</summary>
        private uint _fileOffset;

        /// <summary>Contains the first directory record of in the root of the DICOMDIR.</summary>
        private DirectoryRecordSequenceItem _rootRecord;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the DicomDirectory class.
        /// </summary>
        /// <remarks>Sets most default values which can be changed via </remarks>
        /// <param name="aeTitle">The AE Title of the Media Reader/Writer accessing the DICOMDIR</param>
        public DicomDirectory(string aeTitle)
        {
            try
            {
                _dicomDirFile = new DicomFile();

                _dicomDirFile.MetaInfo[DicomTags.FileMetaInformationVersion].Values = new byte[] { 0x00, 0x01 };
                _dicomDirFile.MediaStorageSopClassUid = DicomUids.MediaStorageDirectoryStorage.UID;
                _dicomDirFile.SourceApplicationEntityTitle = aeTitle;
                _dicomDirFile.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

                //_dicomDirFile.PrivateInformationCreatorUid = String.Empty;
                _dicomDirFile.DataSet[DicomTags.FileSetId].Values = String.Empty;
                ImplementationVersionName = DicomImplementation.Version;
                ImplementationClassUid = DicomImplementation.ClassUID.UID;

                _dicomDirFile.MediaStorageSopInstanceUid = DicomUid.GenerateUid().UID;

                // Set zero value so we can calculate the file Offset
				_dicomDirFile.DataSet[DicomTags.OffsetOfTheFirstDirectoryRecordOfTheRootDirectoryEntity].SetUInt32(0, 0);
                _dicomDirFile.DataSet[DicomTags.OffsetOfTheLastDirectoryRecordOfTheRootDirectoryEntity].SetUInt32(0,0);
                _dicomDirFile.DataSet[DicomTags.FileSetConsistencyFlag].SetUInt16(0,0);

                _directoryRecordSequence = (DicomAttributeSQ)_dicomDirFile.DataSet[DicomTags.DirectoryRecordSequence];
            }
            catch (Exception ex)
            {
				Platform.Log(LogLevel.Error, ex, "Exception initializing DicomDirectory");
                throw;
            }
        }
        #endregion

        #region Public Properties

		/// <summary>
		/// An enumerable collection for traversing the <see cref="DirectoryRecordSequenceItem"/> records in the root of the DICOMDIR.
		/// </summary>
		public DirectoryRecordCollection RootDirectoryRecordCollection
		{
			get
			{
				return new DirectoryRecordCollection(RootDirectoryRecord);
			}
		}

		/// <summary>
		/// Gets the root directory record.  May be set to null if no directory records exist.
		/// </summary>
    	public DirectoryRecordSequenceItem RootDirectoryRecord
    	{
			get { return _rootRecord; }
    	}

        //NOTE: these are mostly wrappers around the DicomFile properties

        /// <summary>
        /// Gets or sets the file set id.
        /// </summary>
        /// <value>The file set id.</value>
        /// <remarks>User or implementation specific Identifier (up to 16 characters), intended to be a short human readable label to easily (but
        /// not necessarily uniquely) identify a specific File-set to
        /// facilitate operator manipulation of the physical media on
        /// which the File-set is stored. </remarks>
        public string FileSetId
        {
            get { return _dicomDirFile.DataSet[DicomTags.FileSetId].GetString(0, String.Empty); }
            set
            {
                if (value != null && value.Trim().Length > 16)
					throw new ArgumentException("FileSetId can only be a maximum of 16 characters", "value");

                _dicomDirFile.DataSet[DicomTags.FileSetId].SetString(0, value == null ? "" : value.Trim());
            }
        }

        /// <summary>
        /// The DICOM Application Entity (AE) Title of the AE which wrote this file's 
        /// content (or last updated it).  If used, it allows the tracin of the source 
        /// of errors in the event of media interchange problems.  The policies associated
        /// with AE Titles are the same as those defined in PS 3.8 of the DICOM Standard. 
        /// </summary>
        public string SourceApplicationEntityTitle
        {
            get { return _dicomDirFile.SourceApplicationEntityTitle; }
            set
            {
                _dicomDirFile.SourceApplicationEntityTitle = value;
            }
        }

        /// <summary>
        /// Identifies a version for an Implementation Class UID (002,0012) using up to 
        /// 16 characters of the repertoire.  It follows the same policies as defined in 
        /// PS 3.7 of the DICOM Standard (association negotiation).
        /// </summary>
        public string ImplementationVersionName
        {
            get { return _dicomDirFile.ImplementationVersionName; }
            set
            {
                _dicomDirFile.ImplementationVersionName = value;
            }
        }

        /// <summary>
        /// Uniquely identifies the implementation which wrote this file and its content.  It provides an 
        /// unambiguous identification of the type of implementation which last wrote the file in the 
        /// event of interchagne problems.  It follows the same policies as defined by PS 3.7 of the DICOM Standard
        /// (association negotiation).
        /// </summary>        
        public string ImplementationClassUid
        {
            get { return _dicomDirFile.ImplementationClassUid; }
            set
            {
                _dicomDirFile.ImplementationClassUid = value;
            }
        }

        /// <summary>
        /// The transfer syntax the file is encoded in.
        /// </summary>
        /// <remarks>
        /// This property returns a TransferSyntax object for the transfer syntax encoded 
        /// in the tag Transfer Syntax UID (0002,0010).
        /// </remarks>
        public TransferSyntax TransferSyntax
        {
            get { return _dicomDirFile.TransferSyntax; }
            set { _dicomDirFile.TransferSyntax = value; }
        }

        /// <summary>
        /// Uniquiely identifies the SOP Instance associated with the Data Set placed in the file and following the File Meta Information.
        /// </summary>
        public string MediaStorageSopInstanceUid
        {
            get { return _dicomDirFile.MediaStorageSopInstanceUid; }
            set { _dicomDirFile.MediaStorageSopInstanceUid = value; }
        }

        /// <summary>
        /// Identifies a version for an Implementation Class UID (002,0012) using up to 
        /// 16 characters of the repertoire.  It follows the same policies as defined in 
        /// PS 3.7 of the DICOM Standard (association negotiation).
        /// </summary>
        public string PrivateInformationCreatorUid
        {
            get { return _dicomDirFile.PrivateInformationCreatorUid; }
            set { _dicomDirFile.PrivateInformationCreatorUid = value; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Saves the DICOMDIR to the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Save(string fileName)
        {
            const DicomWriteOptions options = DicomWriteOptions.Default;

            if (_rootRecord == null)
                throw new InvalidOperationException("No Dicom Files added, cannot save dicom directory");

            _saveFileName = fileName;

			// Clear so that the calculations work properly on the length.
            _directoryRecordSequence.ClearSequenceItems();

            //Calculate the offset in the file to the beginning of the Directory Record Sequence Tag
            _fileOffset = 128 // Preamble Length
                + 4 // DICM Characters
                + _dicomDirFile.MetaInfo.CalculateWriteLength(_dicomDirFile.TransferSyntax, DicomWriteOptions.CalculateGroupLengths) // Must calc including Group lengths for (0002,0000)
                + _dicomDirFile.DataSet.CalculateWriteLength(0, DicomTags.DirectoryRecordSequence - 1, _dicomDirFile.TransferSyntax, options); // Length without the Directory Record Sequence Attribute

            //Add the offset for the Directory Record sequence tag itself
            _fileOffset += 4; // element tag
            if (_dicomDirFile.TransferSyntax.ExplicitVr)
            {
                _fileOffset += 2 + 2 + 4; // 2 (VR) + 2 (reserved) + 4 (length)
            }
            else
            {
                _fileOffset += 4; // length
            }

            // go through the tree of records and add them back into the dataset.
            AddDirectoryRecordsToSequenceItem(_rootRecord);

            // Double check to make sure at least one file was added.
            if (_rootRecord != null)
            {
                // Calculate offsets for each directory record
                CalculateOffsets(_dicomDirFile.TransferSyntax, options);

                // Traverse through the tree and set the offsets.
                SetOffsets(_rootRecord);

                //Set the offsets in the dataset 
                _dicomDirFile.DataSet[DicomTags.OffsetOfTheFirstDirectoryRecordOfTheRootDirectoryEntity].Values = _rootRecord.Offset;

                DirectoryRecordSequenceItem lastRoot = _rootRecord;
                while (lastRoot.NextDirectoryRecord != null) lastRoot = lastRoot.NextDirectoryRecord;

                _dicomDirFile.DataSet[DicomTags.OffsetOfTheLastDirectoryRecordOfTheRootDirectoryEntity].Values =
                    lastRoot.Offset;
            }
            else
            {
                _dicomDirFile.DataSet[DicomTags.OffsetOfTheFirstDirectoryRecordOfTheRootDirectoryEntity].Values = 0;
                _dicomDirFile.DataSet[DicomTags.OffsetOfTheLastDirectoryRecordOfTheRootDirectoryEntity].Values = 0;
            }

            try
            {
                _dicomDirFile.Save(fileName, options);

            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Error saving dicom File {0}", fileName);
            	throw;
            }
        }

		/// <summary>
		/// Loads the specified DICOMDIR file.
		/// </summary>
		/// <param name="filename">The path to the DICOMDIR file.</param>
		public void Load(string filename)
		{
			try
			{
				_dicomDirFile.Load(DicomReadOptions.Default, filename);

			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Error loading dicom File {0}", filename);
				throw;
			}

			// Create a Dictionary containing the offsets within the DICOMDIR of each directory record and the 
			// corresponding DirectoryREcordSequenceItem objects.
			Dictionary<uint, DirectoryRecordSequenceItem> lookup = new Dictionary<uint, DirectoryRecordSequenceItem>();

			foreach (DirectoryRecordSequenceItem sqItem in _directoryRecordSequence.Values as DicomSequenceItem[])
			{
				lookup.Add(sqItem.Offset, sqItem);
			}

			// Get the root Directory Record.
			uint offset = _dicomDirFile.DataSet[DicomTags.OffsetOfTheFirstDirectoryRecordOfTheRootDirectoryEntity].GetUInt32(0, 0);
			if (!lookup.TryGetValue(offset, out _rootRecord) && offset != 0)
				throw new DicomDataException("Unable to find root directory record in File");

			// Now traverse through the remainder of the directory records, and match up the offsets with the directory
			// records so we can build up the tree structure.
			foreach (DirectoryRecordSequenceItem sqItem in _directoryRecordSequence.Values as DicomSequenceItem[])
			{
				offset = sqItem[DicomTags.OffsetOfTheNextDirectoryRecord].GetUInt32(0, 0);

				DirectoryRecordSequenceItem foundItem;
				if (lookup.TryGetValue(offset, out foundItem))
					sqItem.NextDirectoryRecord = foundItem;
				else
					sqItem.NextDirectoryRecord = null;

				offset = sqItem[DicomTags.OffsetOfReferencedLowerLevelDirectoryEntity].GetUInt32(0, 0);

				sqItem.LowerLevelDirectoryRecord = lookup.TryGetValue(offset, out foundItem) ? foundItem : null;
			}			
		}

        /// <summary>
        /// Adds the dicom image file to the list of images to add.
        /// </summary>
        /// <param name="dicomFile">The dicom file.</param>
        /// <param name="optionalDicomDirFileLocation">specifies the file location in the Directory Record ReferencedFileId 
        /// tag.  If is null or empty, it will use a relative path to the dicom File from the specified DICOM Dir filename in the Save() method.</param>
        public void AddFile(DicomFile dicomFile, string optionalDicomDirFileLocation)
        {
            if (dicomFile == null)
                throw new ArgumentNullException("dicomFile");

			InsertFile(dicomFile, optionalDicomDirFileLocation);
        }

        /// <summary>
        /// Adds the dicom image file to the list of images to add.
        /// </summary>
        /// <param name="dicomFileName">Name of the dicom file.</param>
        /// <param name="optionalDicomDirFileLocation">specifies the file location in the Directory Record ReferencedFileId 
        /// tag.  If is null or empty, it will use a relative path to the dicom File from the specified DICOM Dir filename in the Save() method.</param>
        public void AddFile(string dicomFileName, string optionalDicomDirFileLocation)
        {
            if (String.IsNullOrEmpty(dicomFileName))
                throw new ArgumentNullException("dicomFileName");

			if (File.Exists(dicomFileName))
			{
				DicomFile dicomFile = new DicomFile(dicomFileName);
				InsertFile(dicomFile, optionalDicomDirFileLocation);
			}
			else
				throw new FileNotFoundException("cannot add DicomFile, does not exist", dicomFileName);
        }

        /// <summary>
        /// Dumps the contents of the dicomDirFile.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="options">The dump options.</param>
        /// <returns></returns>
        public string Dump(string prefix, DicomDumpOptions options)
        {
            return _dicomDirFile.Dump(prefix, options);
        }

        #endregion

        #region Private Methods
		/// <summary>
		/// Called to insert a DICOM file into the directory record structure.
		/// </summary>
		/// <param name="dicomFile"></param>
		/// <param name="optionalRelativeRootPath"></param>
		private void InsertFile(DicomFile dicomFile, string optionalRelativeRootPath)
		{
			try
			{
				if (dicomFile.DataSet.Count == 0)
					dicomFile.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);

				DirectoryRecordSequenceItem patientRecord;
				DirectoryRecordSequenceItem studyRecord;
				DirectoryRecordSequenceItem seriesRecord;

				if (_rootRecord == null)
					_rootRecord = patientRecord = CreatePatientItem(dicomFile);
				else
					patientRecord = GetExistingOrCreateNewPatient(_rootRecord, dicomFile);

				if (patientRecord.LowerLevelDirectoryRecord == null)
					patientRecord.LowerLevelDirectoryRecord = studyRecord = CreateStudyItem(dicomFile);
				else
					studyRecord = GetExistingOrCreateNewStudy(patientRecord.LowerLevelDirectoryRecord, dicomFile);

				if (studyRecord.LowerLevelDirectoryRecord == null)
					studyRecord.LowerLevelDirectoryRecord = seriesRecord = CreateSeriesItem(dicomFile);
				else
					seriesRecord = GetExistingOrCreateNewSeries(studyRecord.LowerLevelDirectoryRecord, dicomFile);

				if (seriesRecord.LowerLevelDirectoryRecord == null)
					seriesRecord.LowerLevelDirectoryRecord = CreateImageItem(dicomFile, optionalRelativeRootPath);
				else
					GetExistingOrCreateNewImage(seriesRecord.LowerLevelDirectoryRecord, dicomFile, optionalRelativeRootPath);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Error adding image {0} to directory file", dicomFile.Filename);
				throw;
			}
		}

    	/// <summary>
        /// Traverse the directory record tree and insert them into the directory record sequence.
        /// </summary>
        private void AddDirectoryRecordsToSequenceItem(DirectoryRecordSequenceItem root)
        {
            if (root == null)
                return;

            _directoryRecordSequence.AddSequenceItem(root);

            if (root.LowerLevelDirectoryRecord != null)
                AddDirectoryRecordsToSequenceItem(root.LowerLevelDirectoryRecord);

            if (root.NextDirectoryRecord != null)
                AddDirectoryRecordsToSequenceItem(root.NextDirectoryRecord);
        }

        /// <summary>
        /// Finds the next directory record of the specified <paramref name="recordType"/>, starting at the specified <paramref name="startIndex"/>
        /// </summary>
        /// <param name="recordType">Type of the record.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        private void CalculateOffsets(TransferSyntax syntax, DicomWriteOptions options)
        {
            foreach (DicomSequenceItem sq in (DicomSequenceItem[])_dicomDirFile.DataSet[DicomTags.DirectoryRecordSequence].Values)
            {
                DirectoryRecordSequenceItem record = sq as DirectoryRecordSequenceItem;
                if (record == null)
                    throw new ApplicationException("Unexpected type for directory record: " + sq.GetType());

                record.Offset = _fileOffset;

                _fileOffset += 4 + 4; // Sequence Item Tag

                _fileOffset += record.CalculateWriteLength(syntax, options & ~DicomWriteOptions.CalculateGroupLengths);
                if (!Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequenceItem))
                    _fileOffset += 4 + 4; // Sequence Item Delimitation Item
            }
            if (!Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequence))
                _fileOffset += 4 + 4; // Sequence Delimitation Item
        }

        /// <summary>
        /// Traverse at the image level to see if the image exists or create a new image if it doesn't.
        /// </summary>
        /// <param name="images"></param>
        /// <param name="file"></param>
        /// <param name="optionalDicomDirFileLocation"></param>
        /// <returns></returns>
        private void GetExistingOrCreateNewImage(DirectoryRecordSequenceItem images, DicomFile file, string optionalDicomDirFileLocation)
        {
            DirectoryRecordSequenceItem currentImage = images;
            while (currentImage != null)
            {
                if (currentImage[DicomTags.ReferencedSopInstanceUidInFile].Equals(file.DataSet[DicomTags.SopInstanceUid]))
                {
                	return;
                }
            	if (currentImage.NextDirectoryRecord == null)
                {
                    currentImage.NextDirectoryRecord = CreateImageItem(file, optionalDicomDirFileLocation);
                	return;
                }
                currentImage = currentImage.NextDirectoryRecord;
            }
        	return;
        }

        /// <summary>
        /// Create an image Directory record
        /// </summary>
        /// <param name="dicomFile">The dicom file.</param>
        /// <param name="optionalDicomDirFileLocation">The optional dicom dir file location.</param>
        private DirectoryRecordSequenceItem CreateImageItem(DicomFile dicomFile, string optionalDicomDirFileLocation)
        {
            if (String.IsNullOrEmpty(optionalDicomDirFileLocation))
            {
                optionalDicomDirFileLocation = EvaluateRelativePath(_saveFileName, dicomFile.Filename);
            }

        	DirectoryRecordType type;
			if (DirectoryRecordDictionary.TryGetDirectoryRecordType(dicomFile.SopClass.Uid, out type))
			{
				string name;
				DirectoryRecordTypeDictionary.TryGetName(type, out name);

				IDictionary<uint, object> dicomTags = new Dictionary<uint, object>();
				dicomTags.Add(DicomTags.ReferencedFileId, optionalDicomDirFileLocation);
				dicomTags.Add(DicomTags.ReferencedSopClassUidInFile, dicomFile.SopClass.Uid);
				dicomTags.Add(DicomTags.ReferencedSopInstanceUidInFile, dicomFile.MediaStorageSopInstanceUid);
				dicomTags.Add(DicomTags.ReferencedTransferSyntaxUidInFile, dicomFile.TransferSyntaxUid);

				// NOTE:  This is a bit problematic, but sufficient for now. We should take into account
				// which tags are type 2 and which are type 1 and which are conditional when setting them 
				// in AddSequenceItem
				List<uint> tagList;
				if (DirectoryRecordDictionary.TryGetDirectoryRecordTagList(type, out tagList))
					foreach (uint tag in tagList)
						dicomTags.Add(tag, null);

				return AddSequenceItem(type, dicomFile.DataSet, dicomTags);
			}

        	return null;
        }
        #endregion

        #region Private Static Methods
        /// <summary>
        /// Traverse through the tree of directory records, and set the values for the offsets for each 
        /// record.
        /// </summary>
        private static void SetOffsets(DirectoryRecordSequenceItem root)
        {
            if (root.NextDirectoryRecord != null)
            {
                root[DicomTags.OffsetOfTheNextDirectoryRecord].Values = root.NextDirectoryRecord.Offset;
                SetOffsets(root.NextDirectoryRecord);
            }
            else
                root[DicomTags.OffsetOfTheNextDirectoryRecord].Values = 0;

            if (root.LowerLevelDirectoryRecord != null)
            {
                root[DicomTags.OffsetOfReferencedLowerLevelDirectoryEntity].Values = root.LowerLevelDirectoryRecord.Offset;
                SetOffsets(root.LowerLevelDirectoryRecord);
            }
            else
                root[DicomTags.OffsetOfReferencedLowerLevelDirectoryEntity].Values = 0;
        }

        /// <summary>
        /// Traverse at the Patient level to check if a Patient exists or create a Patient if it doesn't exist.
        /// </summary>
        /// <param name="patients"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private static DirectoryRecordSequenceItem GetExistingOrCreateNewPatient(DirectoryRecordSequenceItem patients, DicomMessageBase file)
        {
            DirectoryRecordSequenceItem currentPatient = patients;
            while (currentPatient != null)
            {
                if (currentPatient[DicomTags.PatientId].Equals(file.DataSet[DicomTags.PatientId])
                    && currentPatient[DicomTags.PatientsName].Equals(file.DataSet[DicomTags.PatientsName]))
                {
                    return currentPatient;
                }
                if (currentPatient.NextDirectoryRecord == null)
                {
                    currentPatient.NextDirectoryRecord = CreatePatientItem(file);
                    return currentPatient.NextDirectoryRecord;
                }
                currentPatient = currentPatient.NextDirectoryRecord;
            }
            return null;
        }

        /// <summary>
        /// Traverse at the Study level to check if a Study exists or create a Study if it doesn't exist.
        /// </summary>
        /// <param name="studies"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private static DirectoryRecordSequenceItem GetExistingOrCreateNewStudy(DirectoryRecordSequenceItem studies, DicomMessageBase file)
        {
            DirectoryRecordSequenceItem currentStudy = studies;
            while (currentStudy != null)
            {
                if (currentStudy[DicomTags.StudyInstanceUid].Equals(file.DataSet[DicomTags.StudyInstanceUid]))
                {
                    return currentStudy;
                }
                if (currentStudy.NextDirectoryRecord == null)
                {
                    currentStudy.NextDirectoryRecord = CreateStudyItem(file);
                    return currentStudy.NextDirectoryRecord;
                }
                currentStudy = currentStudy.NextDirectoryRecord;
            }
            return null;
        }

        /// <summary>
        /// Traverse at the Series level to check if a Series exists, or create a Series if it doesn't exist.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private static DirectoryRecordSequenceItem GetExistingOrCreateNewSeries(DirectoryRecordSequenceItem series, DicomMessageBase file)
        {
            DirectoryRecordSequenceItem currentSeries = series;
            while (currentSeries != null)
            {
                if (currentSeries[DicomTags.SeriesInstanceUid].Equals(file.DataSet[DicomTags.SeriesInstanceUid]))
                {
                    return currentSeries;
                }
                if (currentSeries.NextDirectoryRecord == null)
                {
                    currentSeries.NextDirectoryRecord = CreateSeriesItem(file);
                    return currentSeries.NextDirectoryRecord;
                }
                currentSeries = currentSeries.NextDirectoryRecord;
            }
            return null;
        }

        /// <summary>
        /// Adds a sequence item to temporarydictionary with the current offset.
        /// </summary>
        /// <param name="recordType">Type of the record.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>The newly created DirectoryRecord</returns>
        /// <remarks>Tags are a dictionary of tags and optional values - if the value is null, then it will get the value from the specified dataset</remarks>
        private static DirectoryRecordSequenceItem AddSequenceItem(DirectoryRecordType recordType, IDicomAttributeProvider dataSet, IDictionary<uint, object> tags)
        {
            DirectoryRecordSequenceItem dicomSequenceItem = new DirectoryRecordSequenceItem();
            dicomSequenceItem[DicomTags.OffsetOfTheNextDirectoryRecord].Values = 0;
            dicomSequenceItem[DicomTags.RecordInUseFlag].Values = 0xFFFF;
            dicomSequenceItem[DicomTags.OffsetOfReferencedLowerLevelDirectoryEntity].Values = 0;

        	string recordName;
        	DirectoryRecordTypeDictionary.TryGetName(recordType, out recordName);
            dicomSequenceItem[DicomTags.DirectoryRecordType].Values = recordName;

        	DicomAttribute charSetAttrib;
			if (dataSet.TryGetAttribute(DicomTags.SpecificCharacterSet, out charSetAttrib))
				dicomSequenceItem[DicomTags.SpecificCharacterSet] = charSetAttrib.Copy();

            foreach (uint dicomTag in tags.Keys)
            {
                try
                {
                    DicomTag dicomTag2 = DicomTagDictionary.GetDicomTag(dicomTag);
                	DicomAttribute attrib;
                    if (tags[dicomTag] != null)
                    {
                        dicomSequenceItem[dicomTag].Values = tags[dicomTag];
                    }
                    else if (dataSet.TryGetAttribute(dicomTag, out attrib))
                    {
                        dicomSequenceItem[dicomTag].Values = attrib.Values;
                    }
                    else
                    {
                        Platform.Log(LogLevel.Info, 
							"Cannot find dicomTag {0} for record type {1}", dicomTag2 != null ? dicomTag2.ToString() : dicomTag.ToString(), recordType);
                    }
                }
                catch (Exception ex)
                {
					Platform.Log(LogLevel.Error, ex, "Exception adding dicomTag {0} to directory record for record type {1}", dicomTag, recordType);
                }
            }

            return dicomSequenceItem;
        }

        /// <summary>
        /// Create a Patient Directory Record
        /// </summary>
        /// <param name="dicomFile">The dicom file or message.</param>
        private static DirectoryRecordSequenceItem CreatePatientItem(DicomMessageBase dicomFile)
        {
            if (dicomFile == null)
                throw new ArgumentNullException("dicomFile");

            IDictionary<uint, object> dicomTags = new Dictionary<uint, object>();
            dicomTags.Add(DicomTags.PatientsName, null);
            dicomTags.Add(DicomTags.PatientId, null);
            dicomTags.Add(DicomTags.PatientsBirthDate, null);
            dicomTags.Add(DicomTags.PatientsSex, null);

            return AddSequenceItem(DirectoryRecordType.Patient, dicomFile.DataSet, dicomTags);
        }

        /// <summary>
        /// Create a Study Directory Record
        /// </summary>
        /// <param name="dicomFile">The dicom file.</param>
        private static DirectoryRecordSequenceItem CreateStudyItem(DicomMessageBase dicomFile)
        {
            IDictionary<uint, object> dicomTags = new Dictionary<uint, object>();
            dicomTags.Add(DicomTags.StudyInstanceUid, null);
            dicomTags.Add(DicomTags.StudyId, null);
            dicomTags.Add(DicomTags.StudyDate, null);
            dicomTags.Add(DicomTags.StudyTime, null);
            dicomTags.Add(DicomTags.AccessionNumber, null);
            dicomTags.Add(DicomTags.StudyDescription, null);

            return AddSequenceItem(DirectoryRecordType.Study, dicomFile.DataSet, dicomTags);
        }

        /// <summary>
        /// Create a Series Directory Record
        /// </summary>
        /// <param name="dicomFile">The dicom file.</param>
        private static DirectoryRecordSequenceItem CreateSeriesItem(DicomMessageBase dicomFile)
        {
            IDictionary<uint, object> dicomTags = new Dictionary<uint, object>();
            dicomTags.Add(DicomTags.SeriesInstanceUid, null);
            dicomTags.Add(DicomTags.Modality, null);
            dicomTags.Add(DicomTags.SeriesDate, null);
            dicomTags.Add(DicomTags.SeriesTime, null);
            dicomTags.Add(DicomTags.SeriesNumber, null);
            dicomTags.Add(DicomTags.SeriesDescription, null);
            //dicomTags.Add(DicomTags.SeriesDescription, dicomFile.DataSet[DicomTags.SeriesDescription].GetString(0, String.Empty));

            return AddSequenceItem(DirectoryRecordType.Series, dicomFile.DataSet, dicomTags);
        }

        /// <summary>
        /// Evaluates the relative path to <paramref name="absoluteFilePath"/> from <paramref name="mainDirPath"/>.
        /// </summary>
        /// <param name="mainDirPath">The main dir path.</param>
        /// <param name="absoluteFilePath">The absolute file path.</param>
        /// <returns></returns>
        private static string EvaluateRelativePath(string mainDirPath, string absoluteFilePath)
        {
            string[] firstPathParts = mainDirPath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string[] secondPathParts = absoluteFilePath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

            int sameCounter = 0;
            for (int i = 0; i < Math.Min(firstPathParts.Length, secondPathParts.Length); i++)
            {
                if (!firstPathParts[i].ToLower().Equals(secondPathParts[i].ToLower()))
                {
                    break;
                }
                sameCounter++;
            }

            if (sameCounter == 0)
            {
                return absoluteFilePath;
            }

            string newPath = String.Empty;

            for (int i = sameCounter; i < firstPathParts.Length; i++)
            {
                if (i > sameCounter)
                {
                    newPath += Path.DirectorySeparatorChar;
                }
                newPath += "..";
            }

            if (newPath.Length == 0)
            {
                newPath = ".";
            }

            for (int i = sameCounter; i < secondPathParts.Length; i++)
            {
                newPath += Path.DirectorySeparatorChar;
                newPath += secondPathParts[i];
            }

            return newPath;
        }
        #endregion

        #region IDisposable Members

        private bool _disposed;
        public void Dispose()
        {
			if (!_disposed)
            {
                if (_dicomDirFile != null)
                    _dicomDirFile = null;

				_disposed = true;
            }
        }

        #endregion
    }

	
	/// <summary>
	/// Dictionary of the directory records required for specific SopClasses.
	/// </summary>
	internal static class DirectoryRecordDictionary
	{
		#region Private Members
		private static Dictionary<string, DirectoryRecordType> _sopClassLookup = new Dictionary<string, DirectoryRecordType>();
		private static Dictionary<DirectoryRecordType, List<uint>> _tagLookupList = new Dictionary<DirectoryRecordType, List<uint>>();
		#endregion

		#region Constructors
		static DirectoryRecordDictionary()
		{
			_sopClassLookup.Add(SopClass.AmbulatoryEcgWaveformStorageUid, DirectoryRecordType.Waveform);
			_sopClassLookup.Add(SopClass.BasicTextSrStorageUid, DirectoryRecordType.SrDocument);
			_sopClassLookup.Add(SopClass.BasicVoiceAudioWaveformStorageUid, DirectoryRecordType.Waveform);
			_sopClassLookup.Add(SopClass.BlendingSoftcopyPresentationStateStorageSopClassUid, DirectoryRecordType.Presentation);
			_sopClassLookup.Add(SopClass.CardiacElectrophysiologyWaveformStorageUid, DirectoryRecordType.Waveform);
			_sopClassLookup.Add(SopClass.ChestCadSrStorageUid, DirectoryRecordType.SrDocument);
			_sopClassLookup.Add(SopClass.ColorSoftcopyPresentationStateStorageSopClassUid, DirectoryRecordType.Presentation);
			_sopClassLookup.Add(SopClass.ComprehensiveSrStorageUid, DirectoryRecordType.SrDocument);
			_sopClassLookup.Add(SopClass.ComputedRadiographyImageStorageUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.CtImageStorageUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.DeformableSpatialRegistrationStorageUid, DirectoryRecordType.Registration);
			_sopClassLookup.Add(SopClass.DigitalIntraOralXRayImageStorageForPresentationUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.DigitalIntraOralXRayImageStorageForProcessingUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.DigitalMammographyXRayImageStorageForPresentationUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.DigitalMammographyXRayImageStorageForProcessingUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.DigitalXRayImageStorageForPresentationUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.EncapsulatedCdaStorageUid, DirectoryRecordType.Hl7StrucDoc);
			_sopClassLookup.Add(SopClass.EncapsulatedPdfStorageUid, DirectoryRecordType.EncapDoc);
			_sopClassLookup.Add(SopClass.EnhancedCtImageStorageUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.EnhancedMrImageStorageUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.EnhancedSrStorageUid, DirectoryRecordType.SrDocument);
			_sopClassLookup.Add(SopClass.EnhancedXaImageStorageUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.EnhancedXrfImageStorageUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.GeneralEcgWaveformStorageUid, DirectoryRecordType.Waveform);
			_sopClassLookup.Add(SopClass.GrayscaleSoftcopyPresentationStateStorageSopClassUid, DirectoryRecordType.Presentation);
			_sopClassLookup.Add(SopClass.HangingProtocolStorageUid, DirectoryRecordType.HangingProtocol);
			_sopClassLookup.Add(SopClass.HemodynamicWaveformStorageUid, DirectoryRecordType.Waveform);
			_sopClassLookup.Add(SopClass.KeyObjectSelectionDocumentStorageUid, DirectoryRecordType.KeyObjectDoc);
			_sopClassLookup.Add(SopClass.MammographyCadSrStorageUid, DirectoryRecordType.SrDocument);
			_sopClassLookup.Add(SopClass.MrImageStorageUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.MrSpectroscopyStorageUid, DirectoryRecordType.Spectroscopy);
			_sopClassLookup.Add(SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.MultiFrameSingleBitSecondaryCaptureImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.MultiFrameTrueColorSecondaryCaptureImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.NuclearMedicineImageStorageRetiredUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.NuclearMedicineImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.OphthalmicPhotography16BitImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.OphthalmicPhotography8BitImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.OphthalmicTomographyImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.PositronEmissionTomographyImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.PseudoColorSoftcopyPresentationStateStorageSopClassUid, DirectoryRecordType.Presentation);
            _sopClassLookup.Add(SopClass.RawDataStorageUid, DirectoryRecordType.RawData);
            _sopClassLookup.Add(SopClass.RealWorldValueMappingStorageUid, DirectoryRecordType.ValueMap);
            _sopClassLookup.Add(SopClass.RtBeamsTreatmentRecordStorageUid, DirectoryRecordType.RtTreatRecord);
            _sopClassLookup.Add(SopClass.RtBrachyTreatmentRecordStorageUid, DirectoryRecordType.RtTreatRecord);
            _sopClassLookup.Add(SopClass.RtDoseStorageUid, DirectoryRecordType.RtDose);
            _sopClassLookup.Add(SopClass.RtImageStorageUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.RtIonBeamsTreatmentRecordStorageUid, DirectoryRecordType.RtTreatRecord);
            _sopClassLookup.Add(SopClass.RtIonPlanStorageUid, DirectoryRecordType.RtPlan);
            _sopClassLookup.Add(SopClass.RtPlanStorageUid, DirectoryRecordType.RtPlan);
            _sopClassLookup.Add(SopClass.RtStructureSetStorageUid, DirectoryRecordType.RtStructureSet);
            _sopClassLookup.Add(SopClass.RtTreatmentSummaryRecordStorageUid, DirectoryRecordType.RtTreatRecord);
            _sopClassLookup.Add(SopClass.SecondaryCaptureImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.SegmentationStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.SpatialFiducialsStorageUid, DirectoryRecordType.Fiducial);
            _sopClassLookup.Add(SopClass.SpatialRegistrationStorageUid, DirectoryRecordType.Registration);
            _sopClassLookup.Add(SopClass.StereometricRelationshipStorageUid, DirectoryRecordType.Stereometric);
            _sopClassLookup.Add(SopClass.SubstanceAdministrationLoggingSopClassUid, DirectoryRecordType.SrDocument);
            _sopClassLookup.Add(SopClass.UltrasoundImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.UltrasoundImageStorageRetiredUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.UltrasoundMultiFrameImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.UltrasoundMultiFrameImageStorageRetiredUid,DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.VideoEndoscopicImageStorageUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.VideoMicroscopicImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.VideoPhotographicImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.VlEndoscopicImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.VlMicroscopicImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.VlPhotographicImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.VlSlideCoordinatesMicroscopicImageStorageUid, DirectoryRecordType.Image);
			_sopClassLookup.Add(SopClass.XRay3dAngiographicImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.XRay3dCraniofacialImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.XRayAngiographicBiPlaneImageStorageRetiredUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.XRayAngiographicImageStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.XRayRadiationDoseSrStorageUid, DirectoryRecordType.Image);
            _sopClassLookup.Add(SopClass.XRayRadiofluoroscopicImageStorageUid, DirectoryRecordType.Image);

			// At some point this will need to be improved to make the 
			// the DICOMDIRs compliant.  IE, we're not looking at Type2 and conditional tags
			// properly and inserting them.

			//RT DOSE
			List<uint> tagList = new List<uint>();
			tagList.Add(DicomTags.InstanceNumber);
			tagList.Add(DicomTags.DoseSummationType);
			_tagLookupList.Add(DirectoryRecordType.RtDose, tagList);

			//RT STRUCTURE SET
			tagList = new List<uint>
			          	{
			          		DicomTags.InstanceNumber,
			          		DicomTags.StructureSetLabel,
			          		DicomTags.StructureSetDate,
			          		DicomTags.StructureSetTime
			          	};
			_tagLookupList.Add(DirectoryRecordType.RtStructureSet, tagList);

			//RT PLAN
			tagList = new List<uint>
			          	{
			          		DicomTags.InstanceNumber,
			          		DicomTags.RtPlanLabel,
			          		DicomTags.RtPlanDate,
			          		DicomTags.RtPlanTime
			          	};
			_tagLookupList.Add(DirectoryRecordType.RtPlan, tagList);

			//RT TREAT RECORD
			tagList = new List<uint>
			          	{
			          		DicomTags.InstanceNumber,
			          		//TODO Some of the 0x3008 group tags are not in the dictionary, see ticket #5162
			          	};
			_tagLookupList.Add(DirectoryRecordType.RtTreatRecord, tagList);

			//PRESENTATION
			tagList = new List<uint>
			          	{
			          		DicomTags.PresentationCreationDate,
							DicomTags.PresentationCreationTime,
							DicomTags.ReferencedSeriesSequence,
							DicomTags.BlendingSequence
			          	};
			_tagLookupList.Add(DirectoryRecordType.Presentation, tagList);

			//WAVEFORM
			tagList = new List<uint>
			          	{
			          		DicomTags.InstanceNumber,
							DicomTags.ContentDate,
							DicomTags.ContentTime
			          	};
			_tagLookupList.Add(DirectoryRecordType.Waveform, tagList);

			//SR DOCUMENT
			tagList = new List<uint>
			          	{
			          		DicomTags.InstanceNumber,
							DicomTags.CompletionFlag,
							DicomTags.VerificationFlag,
							DicomTags.ContentDate,
							DicomTags.ContentTime,
							DicomTags.VerificationDateTime,
							DicomTags.ConceptNameCodeSequence
			          	};
			_tagLookupList.Add(DirectoryRecordType.SrDocument, tagList);

			//KEY OBJECT DOC
			tagList = new List<uint>
			          	{
			          		DicomTags.InstanceNumber,
							DicomTags.ContentDate,
							DicomTags.ContentTime,
							DicomTags.ConceptNameCodeSequence
			          	};
			_tagLookupList.Add(DirectoryRecordType.KeyObjectDoc, tagList);

			//SPECTROSCOPY
			tagList = new List<uint>
			          	{
			          		DicomTags.ImageType,
							DicomTags.ContentDate,
							DicomTags.ContentTime,
							DicomTags.InstanceNumber,
							DicomTags.NumberOfFrames,
							DicomTags.Rows,
							DicomTags.Columns,
							DicomTags.DataPointRows,
							DicomTags.DataPointColumns

			          	};
			_tagLookupList.Add(DirectoryRecordType.Spectroscopy, tagList);

			//RAW DATA
			tagList = new List<uint>
			          	{
							DicomTags.ContentDate,
							DicomTags.ContentTime,
							DicomTags.InstanceNumber

			          	};
			_tagLookupList.Add(DirectoryRecordType.RawData, tagList);

			//REGISTRATION
			tagList = new List<uint>
			          	{
							DicomTags.ContentDate,
							DicomTags.ContentTime,
							DicomTags.InstanceNumber,
							DicomTags.ContentLabel,
							DicomTags.ContentDescription,
							DicomTags.ContentCreatorsName,
							DicomTags.PersonIdentificationCodeSequence

			          	};
			_tagLookupList.Add(DirectoryRecordType.Registration, tagList);

			//FUDICIAL
			tagList = new List<uint>
			          	{
							DicomTags.ContentDate,
							DicomTags.ContentTime,
							DicomTags.InstanceNumber,
							DicomTags.ContentLabel,
							DicomTags.ContentDescription,
							DicomTags.ContentCreatorsName,
							DicomTags.PersonIdentificationCodeSequence

			          	};
			_tagLookupList.Add(DirectoryRecordType.Fiducial, tagList);

			//HANGING PROTOCOL
			tagList = new List<uint>
			          	{
							DicomTags.HangingProtocolName,
							DicomTags.HangingProtocolDescription,
							DicomTags.HangingProtocolLevel,
							DicomTags.HangingProtocolCreator,
							DicomTags.HangingProtocolCreationDatetime,
							DicomTags.HangingProtocolDefinitionSequence,
							DicomTags.NumberOfPriorsReferenced,
							DicomTags.HangingProtocolUserIdentificationCodeSequence
			          	};
			_tagLookupList.Add(DirectoryRecordType.HangingProtocol, tagList);

			//ENCAP DOC
			tagList = new List<uint>
			          	{
							DicomTags.ContentDate,
							DicomTags.ContentTime,
							DicomTags.InstanceNumber,
							DicomTags.DocumentTitle,
							DicomTags.MimeTypeOfEncapsulatedDocument
			          	};
			_tagLookupList.Add(DirectoryRecordType.EncapDoc, tagList);


			//HL7 STRUC DOC
			tagList = new List<uint>
			          	{
							DicomTags.Hl7InstanceIdentifier,
							DicomTags.Hl7DocumentEffectiveTime
			          	};
			_tagLookupList.Add(DirectoryRecordType.Hl7StrucDoc, tagList);

			//VALUE MAP
			tagList = new List<uint>
			          	{
			          		DicomTags.ContentDate,
			          		DicomTags.ContentTime,
			          		DicomTags.InstanceNumber,
			          		DicomTags.ContentLabel,
			          		DicomTags.ContentDescription,
			          		DicomTags.ContentCreatorsName,
			          		DicomTags.PersonIdentificationCodeSequence
			          	};
			_tagLookupList.Add(DirectoryRecordType.ValueMap, tagList);

			//STEREOMETRIC
			tagList = new List<uint>();
			_tagLookupList.Add(DirectoryRecordType.Stereometric, tagList);

			//PRIVATE
			tagList = new List<uint>();
			_tagLookupList.Add(DirectoryRecordType.Private, tagList);

		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the <see cref="DirectoryRecordType"/> for a given SopClass UID.
		/// </summary>
		/// <param name="uid">The SOP Class UID string.</param>
		/// <param name="type">The output directory record type.</param>
		/// <returns>Returns true if the directory record type is found, or else false.</returns>
		internal static bool TryGetDirectoryRecordType(string uid, out DirectoryRecordType type)
		{
			return _sopClassLookup.TryGetValue(uid, out type);
		}

		/// <summary>
		/// Get a list of tags to be populated into a <see cref="DirectoryRecordSequenceItem"/> for the 
		/// specified <see cref="DirectoryRecordType"/>.
		/// </summary>
		/// <param name="type">The directory record type to get the tag list for.</param>
		/// <param name="tagList">The list of tags to be included.</param>
		/// <returns></returns>
		internal static bool TryGetDirectoryRecordTagList(DirectoryRecordType type, out List<uint> tagList)
		{
			return _tagLookupList.TryGetValue(type, out tagList);
		}
		#endregion
	}
}
