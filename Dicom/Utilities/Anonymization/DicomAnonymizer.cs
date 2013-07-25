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
using System.Collections.ObjectModel;
using System.Diagnostics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Utilities.Anonymization
{
	/// <summary>
	/// Exception thrown by <see cref="DicomAnonymizer"/>.
	/// </summary>
	public class DicomAnonymizerException : DicomException
	{
		internal DicomAnonymizerException(string message, Exception innerException)
			: base(message, innerException) {}

		internal DicomAnonymizerException(string message)
			: base(message) {}
	}

	/// <summary>
	/// Exception thrown when validation failures occur in <see cref="DicomAnonymizer"/>.
	/// </summary>
	public class DicomAnonymizerValidationException : DicomAnonymizerException
	{
		internal DicomAnonymizerValidationException(string message, Exception innerException, ReadOnlyCollection<ValidationFailureDescription> validationFailures)
			: base(message, innerException)
		{
			ValidationFailures = validationFailures;
		}

		internal DicomAnonymizerValidationException(string message, ReadOnlyCollection<ValidationFailureDescription> validationFailures)
			: base(message)
		{
			ValidationFailures = validationFailures;
		}

		public readonly ReadOnlyCollection<ValidationFailureDescription> ValidationFailures;
	}

	/// <summary>
	/// A delegate used by <see cref="DicomAnonymizer"/> that allows client code to gain more control
	/// over study level anonymization.
	/// </summary>
	public delegate StudyData AnonymizeStudyDataDelegate(StudyData original);

	/// <summary>
	/// A delegate used by <see cref="DicomAnonymizer"/> that allows client code to gain more control
	/// over series level anonymization.
	/// </summary>
	public delegate SeriesData AnonymizeSeriesDataDelegate(SeriesData original);

	/// <summary>
	/// A class that assists in anonymizing/modifying patient and series level data in <see cref="DicomFile"/>s.
	/// </summary>
	/// <remarks>
	/// <para>
	/// For a given instance of <see cref="DicomAnonymizer"/>, each <see cref="DicomFile"/> passed to the
	/// <see cref="Anonymize"/> method is altered using <see cref="StudyDataPrototype"/> and 
	/// <see cref="SeriesDataPrototype"/> as templates for the data that gets replaced.  The fields
	/// in <see cref="StudyData"/> and <see cref="SeriesData"/> are not the only tags that get modified;
	/// they are simply the only ones that client code has control over outside of modifying the
	/// <see cref="DicomFile.DataSet"/> directly.
	/// </para>
	/// <para>
	/// Tags that may contain sensitive information are either removed or set to an empty value.  Uids
	/// that are deemed sensitive are replaced with generated ones.  Uids that have been replaced
	/// are kept consistent across related <see cref="DicomFile"/>s (for example, Study/Series Instance Uid)
	/// provided they are anonymized using the same instance of <see cref="DicomAnonymizer"/>.
	/// </para>
	/// <para>
	/// Although we have done our best to conform to <b>Dicom Supplement 55</b>, we cannot claim complete
	/// conformance as we do not:
	/// - Remove burned-in patient information from pixel data (Ultrasounds, for example)
	/// - Do any anonymization of the 'Content Sequence' for structured reports
	/// </para>
	/// </remarks>
	public partial class DicomAnonymizer
	{
		#region Private Fields

		private readonly Dictionary<string, StudyData> _anonymizedStudyDataMap = new Dictionary<string, StudyData>();
		private readonly Dictionary<string, SeriesData> _anonymizedSeriesDataMap = new Dictionary<string, SeriesData>();
		private readonly Dictionary<string, string> _uidMap = new Dictionary<string, string>();

		private readonly ValidationStrategy _validationStrategy;

		private StudyData _studyDataPrototype;
		private SeriesData _seriesDataPrototype;

		private AnonymizeStudyDataDelegate _anonymizeStudyDataDelegate;
		private AnonymizeSeriesDataDelegate _anonymizeSeriesDataDelegate;

		private DicomFile _currentFile;
		private DateTime? _oldStudyDate;
		private DateTime? _newStudyDate;
		private TimeSpan? _studyDateDifference;

		//Only for debugging right now, but could be used for auditing later.
		private readonly List<uint> _currentTagPath = new List<uint>();
		private int _recursionLevel = -1;

		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public DicomAnonymizer()
		{
			_validationStrategy = new ValidationStrategy();

			SpecificCharacterSet = @"ISO_IR 192";
		}

		#region Public Properties

		/// <summary>
		/// Gets or sets the validation options.
		/// </summary>
		public ValidationOptions ValidationOptions
		{
			get { return _validationStrategy.Options; }
			set { _validationStrategy.Options = value; }
		}

		/// <summary>
		/// A template for <see cref="StudyData"/> that will replace the corresponding
		/// fields in each <see cref="DicomFile"/> passed into <see cref="Anonymize"/>.
		/// </summary>
		public StudyData StudyDataPrototype
		{
			get
			{
				if (_studyDataPrototype == null)
					_studyDataPrototype = new StudyData();

				return _studyDataPrototype;
			}
			set
			{
				if (value == _studyDataPrototype)
					return;

				if (value == null)
					throw new ArgumentNullException("value", "StudyData prototype cannot be null.");

				_studyDataPrototype = value;
			}
		}

		/// <summary>
		/// A template for <see cref="SeriesData"/> that will replace the corresponding
		/// fields in each <see cref="DicomFile"/> passed into <see cref="Anonymize"/>.
		/// </summary>
		public SeriesData SeriesDataPrototype
		{
			get
			{
				if (_seriesDataPrototype == null)
					_seriesDataPrototype = new SeriesData();

				return _seriesDataPrototype;
			}
			set
			{
				if (value == _seriesDataPrototype)
					return;

				if (value == null)
					throw new ArgumentNullException("value", "SeriesData prototype cannot be null.");

				_seriesDataPrototype = value;
			}
		}

		/// <summary>
		/// Client code can provide a delegate in order to gain increased control over
		/// anonymization of study level data (for example, when an algorithm is used to
		/// determine anonymized values).
		/// </summary>
		/// <remarks>
		/// When no delegate is set, all files are anonymized using the data in
		/// <see cref="StudyDataPrototype"/>.
		/// </remarks>
		public AnonymizeStudyDataDelegate AnonymizeStudyDataDelegate
		{
			get { return _anonymizeStudyDataDelegate; }
			set
			{
				if (_anonymizeStudyDataDelegate == value)
					return;

				if (value == null)
					throw new ArgumentNullException("value", "AnonymizeStudyDataDelegate cannot be null.");

				_anonymizeStudyDataDelegate = value;
			}
		}

		/// <summary>
		/// Client code can provide a delegate in order to gain increased control over
		/// anonymization of series level data (for example, when an algorithm is used to
		/// determine anonymized values).
		/// </summary>
		/// <remarks>
		/// When no delegate is set, all files are anonymized using the data in
		/// <see cref="SeriesDataPrototype"/>.
		/// </remarks>
		public AnonymizeSeriesDataDelegate AnonymizeSeriesDataDelegate
		{
			get { return _anonymizeSeriesDataDelegate; }
			set
			{
				if (_anonymizeSeriesDataDelegate == value)
					return;

				if (value == null)
					throw new ArgumentNullException("value", "AnonymizeSeriesDataDelegate cannot be null.");

				_anonymizeSeriesDataDelegate = value;
			}
		}

		/// <summary>
		/// Gets or sets the DICOM specific character set to be used when encoding SOP instances.
		/// </summary>
		/// <remarks>
		/// By default, text attribute values will be encoded using UTF-8 Unicode (ISO-IR 192).
		/// If set to NULL or empty, values will be encoded using the default character repertoire (ISO-IR 6).
		/// </remarks>
		public string SpecificCharacterSet { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not private tags should be kept.
		/// </summary>
		public bool KeepPrivateTags { get; set; }

		#endregion

		#region Public Methods

		public virtual void Anonymize(DicomFile dicomFile)
		{
			if (dicomFile == null)
				throw new ArgumentNullException("dicomFile", "The input DicomFile cannot be null.");

			if (dicomFile.DataSet.IsEmpty())
				dicomFile.Load();

			_currentFile = dicomFile;

			try
			{
				StudyData anonymizedStudyData = GetAnonymizedStudyData();
				SeriesData anonymizedSeriesData = GetAnonymizedSeriesData();

				//massage attributes.
				Anonymize();

				anonymizedSeriesData.SaveTo(_currentFile);
				anonymizedStudyData.SaveTo(_currentFile);
			}
			catch (DicomAnonymizerException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new DicomAnonymizerException("An unexpected error has occurred.", e);
			}
			finally
			{
				_currentFile = null;
				_newStudyDate = null;
				_oldStudyDate = null;
				_studyDateDifference = null;
			}
		}

		#endregion

		#region Private Methods

		private StudyData GetAnonymizedStudyData()
		{
			StudyData originalData = new StudyData();
			originalData.LoadFrom(_currentFile);

			_oldStudyDate = originalData.StudyDate;

			if (string.IsNullOrEmpty(originalData.StudyInstanceUid))
				throw new DicomAnonymizerException("The StudyInstanceUid in the source file cannot be empty.");

			StudyData anonymizedData;
			if (_anonymizedStudyDataMap.ContainsKey(originalData.StudyInstanceUid))
			{
				anonymizedData = _anonymizedStudyDataMap[originalData.StudyInstanceUid];
			}
			else
			{
				anonymizedData = GetAnonymizedStudyData(originalData);

				// generate the new study uid if it hasn't already been remapped
				if (_uidMap.ContainsKey(originalData.StudyInstanceUid))
					anonymizedData.StudyInstanceUid = _uidMap[originalData.StudyInstanceUid];
				else
					anonymizedData.StudyInstanceUid = _uidMap[originalData.StudyInstanceUid] = DicomUid.GenerateUid().UID;

				if (String.IsNullOrEmpty(anonymizedData.StudyInstanceUid) || anonymizedData.StudyInstanceUid == originalData.StudyInstanceUid)
					throw new DicomAnonymizerException("An error occurred while generating a new Uid.");

				ReadOnlyCollection<ValidationFailureDescription> failures = _validationStrategy.GetValidationFailures(originalData, anonymizedData);
				if (failures.Count > 0)
					throw new DicomAnonymizerValidationException("At least one validation failure has occurred.", failures);

				_uidMap[originalData.StudyInstanceUid] = anonymizedData.StudyInstanceUid;

				//store the anonymized data.
				_anonymizedStudyDataMap[originalData.StudyInstanceUid] = anonymizedData;
			}

			_newStudyDate = anonymizedData.StudyDate;

			return anonymizedData;
		}

		private StudyData GetAnonymizedStudyData(StudyData original)
		{
			if (_anonymizeStudyDataDelegate != null)
				return _anonymizeStudyDataDelegate(original.Clone());
			else
				return StudyDataPrototype.Clone();
		}

		private SeriesData GetAnonymizedSeriesData()
		{
			string originalSeriesInstanceUid = _currentFile.DataSet[DicomTags.SeriesInstanceUid].ToString();
			if (string.IsNullOrEmpty(originalSeriesInstanceUid))
				throw new DicomAnonymizerException("The SeriesInstanceUid of the source file cannot be empty.");

			SeriesData anonymizedData;
			if (_anonymizedSeriesDataMap.ContainsKey(originalSeriesInstanceUid))
			{
				anonymizedData = _anonymizedSeriesDataMap[originalSeriesInstanceUid];
			}
			else
			{
				SeriesData originalData = new SeriesData();
				originalData.LoadFrom(_currentFile);

				anonymizedData = GetAnonymizedSeriesData(originalData);

				// generate the new series uid if it hasn't already been remapped
				if (_uidMap.ContainsKey(originalData.SeriesInstanceUid))
					anonymizedData.SeriesInstanceUid = _uidMap[originalData.SeriesInstanceUid];
				else
					anonymizedData.SeriesInstanceUid = _uidMap[originalData.SeriesInstanceUid] = DicomUid.GenerateUid().UID;

				if (String.IsNullOrEmpty(anonymizedData.SeriesInstanceUid) || anonymizedData.SeriesInstanceUid == originalData.SeriesInstanceUid)
					throw new DicomAnonymizerException("An error occurred while generating a new Uid.");

				ReadOnlyCollection<ValidationFailureDescription> failures = _validationStrategy.GetValidationFailures(originalData, anonymizedData);
				if (failures.Count > 0)
					throw new DicomAnonymizerValidationException("At least one validation failure has occurred.", failures);

				_uidMap[originalData.SeriesInstanceUid] = anonymizedData.SeriesInstanceUid;

				_anonymizedSeriesDataMap[originalSeriesInstanceUid] = anonymizedData;
			}

			return anonymizedData;
		}

		private SeriesData GetAnonymizedSeriesData(SeriesData original)
		{
			if (_anonymizeSeriesDataDelegate != null)
				return _anonymizeSeriesDataDelegate(original.Clone());
			else
				return SeriesDataPrototype.Clone();
		}

		private void Anonymize()
		{
			string oldUid = _currentFile.DataSet[DicomTags.SopInstanceUid].ToString();
			if (string.IsNullOrEmpty(oldUid))
				throw new DicomAnonymizerException("The SopInstanceUid of the source file cannot be empty.");

			string newUid;
			if (_uidMap.ContainsKey(oldUid))
			{
				newUid = _uidMap[oldUid];
			}
			else
			{
				newUid = DicomUid.GenerateUid().UID;
				_uidMap[oldUid] = newUid;
			}

			if (String.IsNullOrEmpty(newUid) || newUid == oldUid)
				throw new DicomAnonymizerException("An error occurred while generating a new Uid.");

			if (_newStudyDate != null)
			{
				_studyDateDifference = new TimeSpan(0);
				if (_oldStudyDate != null)
					_studyDateDifference = _newStudyDate.Value.Subtract(_oldStudyDate.Value);
			}

			try
			{
				var specificCharacterSet = SpecificCharacterSet ?? string.Empty;
				_currentFile.DataSet.SpecificCharacterSet = specificCharacterSet;
				_currentFile.DataSet[DicomTags.SpecificCharacterSet].SetStringValue(specificCharacterSet);
				_recursionLevel = -1;
				Anonymize(_currentFile.DataSet);
			}
			finally
			{
				_recursionLevel = -1;
				_currentTagPath.Clear();
			}

			_currentFile.DataSet[DicomTags.SopInstanceUid].SetStringValue(newUid);
			_currentFile.MetaInfo[DicomTags.SourceApplicationEntityTitle].SetStringValue(string.Empty);
			_currentFile.MetaInfo[DicomTags.MediaStorageSopInstanceUid].SetStringValue(newUid);
			_currentFile.MetaInfo[DicomTags.MediaStorageSopClassUid].Values = _currentFile.DataSet[DicomTags.SopClassUid].Values;
		}

		private void Anonymize(DicomAttributeCollection dataset)
		{
			++_recursionLevel;
			_currentTagPath.Add(0); //dummy value.

			List<uint> tagsToRemove = new List<uint>();

			foreach (DicomAttribute attribute in dataset)
			{
				_currentTagPath[_recursionLevel] = attribute.Tag.TagValue;

				foreach (DicomAttributeCollection subCollection in GetSubCollections(attribute))
					Anonymize(subCollection);

				if (attribute.Values != null)
				{
					if (IsAttributeToRemove(attribute))
					{
						tagsToRemove.Add(attribute.Tag.TagValue);
					}
					else if (IsPrivateAttribute(attribute))
					{
						if (!KeepPrivateTags) tagsToRemove.Add(attribute.Tag.TagValue);
					}
					else if (IsUidAttributeToRemap(attribute))
					{
						Trace.WriteLine(String.Format("Remapped Uid: {0}", GetTagPathDescription()));
						RemapUidAttribute(attribute);
					}
					else if (IsAttributeToNull(attribute))
					{
						Trace.WriteLine(String.Format("Nulled: {0}", GetTagPathDescription()));
						attribute.SetNullValue();
					}
					else if (IsDateTimeAttributeToAdjust(attribute))
					{
						Trace.WriteLine(String.Format("Adjusted Date: {0}", GetTagPathDescription()));
						AdjustDateTimeAttribute(attribute);
					}
				}
				else
				{
					tagsToRemove.Add(attribute.Tag.TagValue);
				}
			}

			tagsToRemove.ForEach(
				delegate(uint tag)
					{
						_currentTagPath[_recursionLevel] = tag;
						Trace.WriteLine(String.Format("Removed: {0}", GetTagPathDescription()));
						dataset[tag] = null;
					});

			_currentTagPath.RemoveAt(_recursionLevel--);
		}

		private void RemapUidAttribute(DicomAttribute attribute)
		{
			int i = 0;
			foreach (string oldUid in (string[]) attribute.Values)
			{
				if (!String.IsNullOrEmpty(oldUid))
				{
					string newUid;
					if (_uidMap.ContainsKey(oldUid))
					{
						newUid = _uidMap[oldUid];
					}
					else
					{
						newUid = DicomUid.GenerateUid().UID;
						if (String.IsNullOrEmpty(newUid) || newUid == oldUid)
							throw new DicomAnonymizerException("An error occurred while generating a new Uid.");

						_uidMap[oldUid] = newUid;
					}

					attribute.SetString(i++, newUid);
				}
			}
		}

		private void AdjustDateTimeAttribute(DicomAttribute attribute)
		{
			if (_studyDateDifference != null)
			{
				int i = 0;
				if (attribute is DicomAttributeDA)
				{
					foreach (string date in (string[]) attribute.Values)
						attribute.SetString(i++, AdjustDate(date, _studyDateDifference.Value));
				}
				else if (attribute is DicomAttributeDT)
				{
					foreach (string datetime in (string[]) attribute.Values)
						attribute.SetString(i++, AdjustDateTime(datetime, _studyDateDifference.Value));
				}
			}
			else
			{
				// blank the value
				attribute.SetNullValue();
			}
		}

		private string GetTagPathDescription()
		{
			return StringUtilities.Combine(_currentTagPath, "/", GetTagName);
		}

		#region Static Methods

		private static string GetTagName(uint tag)
		{
			DicomTag dcmTag = DicomTagDictionary.GetDicomTag(tag);
			if (dcmTag != null)
				return dcmTag.ToString();

			return String.Format("({0}) Unknown", tag.ToString("X8"));
		}

		private static string AdjustDateTime(string original, TimeSpan studyDateDifference)
		{
			DateTime? originalDate = DateTimeParser.Parse(original ?? "");
			if (originalDate == null) //don't add it if it wasn't there to begin with.
				return "";

			originalDate = originalDate.Value.Add(studyDateDifference);

			return originalDate.Value.ToString(DateParser.DicomDateFormat) + original.Trim().Substring(8);
		}

		private static string AdjustDate(string original, TimeSpan studyDateDifference)
		{
			DateTime? originalDate = DateParser.Parse(original ?? "");
			if (originalDate == null) //don't add it if it wasn't there to begin with.
				return "";

			originalDate = originalDate.Value.Add(studyDateDifference);

			return originalDate.Value.ToString(DateParser.DicomDateFormat);
		}

		private static bool IsAttributeToRemove(DicomAttribute attribute)
		{
			return TagsToRemove.Contains(attribute.Tag.TagValue);
		}

		private static bool IsPrivateAttribute(DicomAttribute attribute)
		{
			return (attribute.Tag.IsPrivate || attribute.Tag.Name == "Private Tag");
		}

		private static bool IsAttributeToNull(DicomAttribute attribute)
		{
			return TagsToNull.Contains(attribute.Tag.TagValue);
		}

		private static bool IsUidAttributeToRemap(DicomAttribute attribute)
		{
			return UidTagsToRemap.Contains(attribute.Tag.TagValue);
		}

		private static bool IsDateTimeAttributeToAdjust(DicomAttribute attribute)
		{
			return DateTimeTagsToAdjust.Contains(attribute.Tag.TagValue);
		}

		private static IEnumerable<DicomAttributeCollection> GetSubCollections(DicomAttribute attribute)
		{
			if (attribute is DicomAttributeSQ && attribute.Values != null)
			{
				DicomSequenceItem[] sequenceItems = (DicomSequenceItem[]) attribute.Values;
				foreach (DicomSequenceItem sequenceItem in sequenceItems)
					yield return sequenceItem;
			}
		}

		#endregion

		#endregion
	}
}