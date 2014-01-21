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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.HierarchicalSeriesInstanceReference;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Helper class for building a hierarchical SOP instance reference sequence.
	/// </summary>
	public partial class HierarchicalSopInstanceReferenceDictionary : ICollection<HierarchicalSopInstanceReferenceDictionary.Entry>
	{
		private readonly Dictionary<string, Dictionary<SeriesKey, Dictionary<string, string>>> _dictionary = new Dictionary<string, Dictionary<SeriesKey, Dictionary<string, string>>>();

		/// <summary>
		/// Initializes a new instance of <see cref="HierarchicalSopInstanceReferenceDictionary"/>.
		/// </summary>
		public HierarchicalSopInstanceReferenceDictionary() {}

		/// <summary>
		/// Initializes a new instance of <see cref="HierarchicalSopInstanceReferenceDictionary"/>.
		/// </summary>
		public HierarchicalSopInstanceReferenceDictionary(IEnumerable<IHierarchicalSopInstanceReferenceMacro> hierarchicalSopInstanceReferenceSequence)
		{
			foreach (var study in hierarchicalSopInstanceReferenceSequence ?? Enumerable.Empty<IHierarchicalSopInstanceReferenceMacro>())
			{
				foreach (var series in study.ReferencedSeriesSequence ?? Enumerable.Empty<IHierarchicalSeriesInstanceReferenceMacro>())
				{
					foreach (var sop in series.ReferencedSopSequence ?? Enumerable.Empty<IReferencedSopSequence>())
					{
						AddReference(study.StudyInstanceUid, series.SeriesInstanceUid, sop.ReferencedSopClassUid, sop.ReferencedSopInstanceUid,
						             series.RetrieveAeTitle, series.RetrieveLocationUid, series.StorageMediaFileSetId, series.StorageMediaFileSetUid);
					}
				}
			}
		}

		/// <summary>
		/// Gets the number of unique SOP instance references in the dictionary.
		/// </summary>
		public int Count
		{
			get { return _dictionary.SelectMany(s => s.Value).SelectMany(s => s.Value).Count(); }
		}

		/// <summary>
		/// Adds a SOP instance reference to the dictionary.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance UID.</param>
		/// <param name="seriesInstanceUid">The series instance UID.</param>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <param name="sopInstanceUid">The SOP instance UID.</param>
		/// <param name="retrieveAeTitle">Optional value specifying the DICOM AE from which the SOP instance can be retrieved over the network.</param>
		/// <param name="retrieveLocationUid">Optional value specifying the UID of the location from which the SOP instance can be retrieved over the network</param>
		/// <param name="storageMediaFileSetId">Optional value specifying the identifier of the storage media on which the SOP instance resides.</param>
		/// <param name="storageMediaFileSetUid">Optional value specifying the UID of the storage media on which the SOP instance resides.</param>
		/// <exception cref="ArgumentNullException">Thrown if any of the arguments are null or empty.</exception>
		/// <exception cref="ArgumentException">Thrown if that SOP instance has already been added to the dictionary.</exception>
		/// <remarks>
		/// The dictionary does not allow for redefining the SOP class for a given instance using this method.
		/// To do so, the particular SOP instance should be removed first. A SOP instance can only be referenced as
		/// one SOP class.
		/// </remarks>
		public void AddReference(string studyInstanceUid, string seriesInstanceUid, string sopClassUid, string sopInstanceUid,
		                         string retrieveAeTitle = null, string retrieveLocationUid = null, string storageMediaFileSetId = null, string storageMediaFileSetUid = null)
		{
			var result = TryAddReference(studyInstanceUid, seriesInstanceUid, sopClassUid, sopInstanceUid, retrieveAeTitle, retrieveLocationUid, storageMediaFileSetId, storageMediaFileSetUid);
			if (!result)
			{
				const string msg = "That SOP Instance has already been added to the dictionary.";
				throw new ArgumentException(msg, "sopInstanceUid");
			}
		}

		/// <summary>
		/// Adds a SOP instance reference to the dictionary.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance UID.</param>
		/// <param name="seriesInstanceUid">The series instance UID.</param>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <param name="sopInstanceUid">The SOP instance UID.</param>
		/// <param name="retrieveAeTitle">Optional value specifying the DICOM AE from which the SOP instance can be retrieved over the network.</param>
		/// <param name="retrieveLocationUid">Optional value specifying the UID of the location from which the SOP instance can be retrieved over the network</param>
		/// <param name="storageMediaFileSetId">Optional value specifying the identifier of the storage media on which the SOP instance resides.</param>
		/// <param name="storageMediaFileSetUid">Optional value specifying the UID of the storage media on which the SOP instance resides.</param>
		/// <exception cref="ArgumentNullException">Thrown if any of the required arguments are null or empty.</exception>
		/// <returns>True if the SOP instance reference was added successfully; False if a reference already exists for the given SOP instance.</returns>
		public bool TryAddReference(string studyInstanceUid, string seriesInstanceUid, string sopClassUid, string sopInstanceUid,
		                            string retrieveAeTitle = null, string retrieveLocationUid = null, string storageMediaFileSetId = null, string storageMediaFileSetUid = null)
		{
			if (string.IsNullOrEmpty(studyInstanceUid))
				throw new ArgumentNullException("studyInstanceUid");
			if (string.IsNullOrEmpty(seriesInstanceUid))
				throw new ArgumentNullException("seriesInstanceUid");
			if (string.IsNullOrEmpty(sopClassUid))
				throw new ArgumentNullException("sopClassUid");
			if (string.IsNullOrEmpty(sopInstanceUid))
				throw new ArgumentNullException("sopInstanceUid");

			if (!_dictionary.ContainsKey(studyInstanceUid))
				_dictionary.Add(studyInstanceUid, new Dictionary<SeriesKey, Dictionary<string, string>>());
			var seriesDictionary = _dictionary[studyInstanceUid];

			var seriesKey = new SeriesKey(seriesInstanceUid, retrieveAeTitle, retrieveLocationUid, storageMediaFileSetId, storageMediaFileSetUid);
			if (!seriesDictionary.ContainsKey(seriesKey))
				seriesDictionary.Add(seriesKey, new Dictionary<string, string>());
			var sopDictionary = seriesDictionary[seriesKey];

			if (sopDictionary.ContainsKey(sopInstanceUid))
				return false;

			sopDictionary.Add(sopInstanceUid, sopClassUid);
			return true;
		}

		/// <summary>
		/// Attempts to remove the given SOP instance reference from the dictionary.
		/// </summary>
		/// <remarks>
		/// This overload removes all SOP instance references regardless of origin (i.e. Retrieve AE Title, Retrive Location UID and Storage Media File-Set).
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance UID.</param>
		/// <param name="seriesInstanceUid">The series instance UID.</param>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <param name="sopInstanceUid">The SOP instance UID.</param>
		/// <exception cref="ArgumentNullException">Thrown if any of the arguments are null or empty.</exception>
		/// <returns>True if the SOP instance reference was removed successfully; False if a reference does not exist for the given SOP instance.</returns>
		public bool TryRemoveReference(string studyInstanceUid, string seriesInstanceUid, string sopClassUid, string sopInstanceUid)
		{
			if (string.IsNullOrEmpty(studyInstanceUid))
				throw new ArgumentNullException("studyInstanceUid");
			if (string.IsNullOrEmpty(seriesInstanceUid))
				throw new ArgumentNullException("seriesInstanceUid");
			if (string.IsNullOrEmpty(sopClassUid))
				throw new ArgumentNullException("sopClassUid");
			if (string.IsNullOrEmpty(sopInstanceUid))
				throw new ArgumentNullException("sopInstanceUid");

			if (_dictionary.ContainsKey(studyInstanceUid))
			{
				var anyRemoved = false;
				var seriesDictionary = _dictionary[studyInstanceUid];
				foreach (var seriesEntry in seriesDictionary.Where(s => s.Key.SeriesInstanceUid == seriesInstanceUid).ToList())
				{
					var sopDictionary = seriesEntry.Value;
					if (sopDictionary.ContainsKey(sopInstanceUid))
					{
						if (sopDictionary[sopInstanceUid] == sopClassUid)
						{
							sopDictionary.Remove(sopInstanceUid);
							CompactDictionary(studyInstanceUid, seriesEntry.Key, sopDictionary, seriesDictionary);
							anyRemoved = true;
						}
					}
				}
				return anyRemoved;
			}
			return false;
		}

		/// <summary>
		/// Attempts to remove the given SOP instance reference from the dictionary.
		/// </summary>
		/// <remarks>
		/// This overload removes only the SOP instance reference that matches the specified origin (i.e. Retrieve AE Title, Retrive Location UID and Storage Media File-Set).
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance UID.</param>
		/// <param name="seriesInstanceUid">The series instance UID.</param>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <param name="sopInstanceUid">The SOP instance UID.</param>
		/// <param name="retrieveAeTitle">The DICOM AE from which the SOP instance can be retrieved over the network.</param>
		/// <param name="retrieveLocationUid">The UID of the location from which the SOP instance can be retrieved over the network</param>
		/// <param name="storageMediaFileSetId">The identifier of the storage media on which the SOP instance resides.</param>
		/// <param name="storageMediaFileSetUid">The UID of the storage media on which the SOP instance resides.</param>
		/// <exception cref="ArgumentNullException">Thrown if any of the arguments are null or empty.</exception>
		/// <returns>True if the SOP instance reference was removed successfully; False if a reference does not exist for the given SOP instance.</returns>
		public bool TryRemoveReference(string studyInstanceUid, string seriesInstanceUid, string sopClassUid, string sopInstanceUid,
		                               string retrieveAeTitle, string retrieveLocationUid, string storageMediaFileSetId, string storageMediaFileSetUid)
		{
			if (string.IsNullOrEmpty(studyInstanceUid))
				throw new ArgumentNullException("studyInstanceUid");
			if (string.IsNullOrEmpty(seriesInstanceUid))
				throw new ArgumentNullException("seriesInstanceUid");
			if (string.IsNullOrEmpty(sopClassUid))
				throw new ArgumentNullException("sopClassUid");
			if (string.IsNullOrEmpty(sopInstanceUid))
				throw new ArgumentNullException("sopInstanceUid");

			if (_dictionary.ContainsKey(studyInstanceUid))
			{
				var seriesKey = new SeriesKey(seriesInstanceUid, retrieveAeTitle, retrieveLocationUid, storageMediaFileSetId, storageMediaFileSetUid);
				var seriesDictionary = _dictionary[studyInstanceUid];
				if (seriesDictionary.ContainsKey(seriesKey))
				{
					var sopDictionary = seriesDictionary[seriesKey];
					if (sopDictionary.ContainsKey(sopInstanceUid))
					{
						if (sopDictionary[sopInstanceUid] == sopClassUid)
						{
							sopDictionary.Remove(sopInstanceUid);
							CompactDictionary(studyInstanceUid, seriesKey, sopDictionary, seriesDictionary);
							return true;
						}
					}
				}
			}
			return false;
		}

		private void CompactDictionary(string studyInstanceUid, SeriesKey seriesKey, Dictionary<string, string> sopDictionary, Dictionary<SeriesKey, Dictionary<string, string>> seriesDictionary)
		{
			// compacts the dictionary after having removed a single reference
			if (sopDictionary.Count == 0)
			{
				seriesDictionary.Remove(seriesKey);
				if (seriesDictionary.Count == 0)
				{
					_dictionary.Remove(studyInstanceUid);
				}
			}
		}

		/// <summary>
		/// Checks whether or not the given SOP instance is referenced in the dictionary.
		/// </summary>
		/// <remarks>
		/// This overload checks for any matching SOP instance reference regardless of origin (i.e. Retrieve AE Title, Retrive Location UID and Storage Media File-Set).
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance UID.</param>
		/// <param name="seriesInstanceUid">The series instance UID.</param>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <param name="sopInstanceUid">The SOP instance UID.</param>
		/// <exception cref="ArgumentNullException">Thrown if any of the arguments are null or empty.</exception>
		/// <returns>True if the SOP instance is referenced in the dictionary; False if a reference does not exist for the given SOP instance.</returns>
		public bool ContainsReference(string studyInstanceUid, string seriesInstanceUid, string sopClassUid, string sopInstanceUid)
		{
			Dictionary<SeriesKey, Dictionary<string, string>> seriesDictionary;
			if (_dictionary.TryGetValue(studyInstanceUid, out seriesDictionary))
			{
				foreach (var seriesEntry in seriesDictionary.Where(s => s.Key.SeriesInstanceUid == seriesInstanceUid))
				{
					Dictionary<string, string> sopDictionary;
					if (seriesDictionary.TryGetValue(seriesEntry.Key, out sopDictionary))
					{
						string sopClass;
						if (sopDictionary.TryGetValue(sopInstanceUid, out sopClass) && sopClass == sopClassUid)
							return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Checks whether or not the given SOP instance is referenced in the dictionary.
		/// </summary>
		/// <remarks>
		/// This overload checks for only the SOP instance reference that matches the specified origin (i.e. Retrieve AE Title, Retrive Location UID and Storage Media File-Set).
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance UID.</param>
		/// <param name="seriesInstanceUid">The series instance UID.</param>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <param name="sopInstanceUid">The SOP instance UID.</param>
		/// <param name="retrieveAeTitle">The DICOM AE from which the SOP instance can be retrieved over the network.</param>
		/// <param name="retrieveLocationUid">The UID of the location from which the SOP instance can be retrieved over the network</param>
		/// <param name="storageMediaFileSetId">The identifier of the storage media on which the SOP instance resides.</param>
		/// <param name="storageMediaFileSetUid">The UID of the storage media on which the SOP instance resides.</param>
		/// <exception cref="ArgumentNullException">Thrown if any of the arguments are null or empty.</exception>
		/// <returns>True if the SOP instance is referenced in the dictionary; False if a reference does not exist for the given SOP instance.</returns>
		public bool ContainsReference(string studyInstanceUid, string seriesInstanceUid, string sopClassUid, string sopInstanceUid,
		                              string retrieveAeTitle, string retrieveLocationUid, string storageMediaFileSetId, string storageMediaFileSetUid)
		{
			Dictionary<SeriesKey, Dictionary<string, string>> seriesDictionary;
			if (_dictionary.TryGetValue(studyInstanceUid, out seriesDictionary))
			{
				Dictionary<string, string> sopDictionary;
				var seriesKey = new SeriesKey(seriesInstanceUid, retrieveAeTitle, retrieveLocationUid, storageMediaFileSetId, storageMediaFileSetUid);
				if (seriesDictionary.TryGetValue(seriesKey, out sopDictionary))
				{
					string sopClass;
					return sopDictionary.TryGetValue(sopInstanceUid, out sopClass) && sopClass == sopClassUid;
				}
			}
			return false;
		}

		/// <summary>
		/// Clears the reference dictionary.
		/// </summary>
		public void Clear()
		{
			_dictionary.Clear();
		}

		/// <summary>
		/// Creates and initializes a <see cref="IHierarchicalSopInstanceReferenceMacro"/> to the given study instance.
		/// </summary>
		/// <param name="studyInstanceUid">The study instance UID.</param>
		protected virtual IHierarchicalSopInstanceReferenceMacro CreateStudyReference(string studyInstanceUid)
		{
			IHierarchicalSopInstanceReferenceMacro reference = new HierarchicalSopInstanceReferenceMacro(new DicomSequenceItem());
			reference.InitializeAttributes();
			reference.StudyInstanceUid = studyInstanceUid;
			return reference;
		}

		/// <summary>
		/// Creates and initializes a <see cref="IHierarchicalSeriesInstanceReferenceMacro"/> to the given series instance.
		/// </summary>
		/// <param name="seriesInstanceUid">The series instance UID.</param>
		/// <param name="retrieveAeTitle">Optional value specifying the DICOM AE from which the SOP instance can be retrieved over the network.</param>
		/// <param name="retrieveLocationUid">Optional value specifying the UID of the location from which the SOP instance can be retrieved over the network</param>
		/// <param name="storageMediaFileSetId">Optional value specifying the identifier of the storage media on which the SOP instance resides.</param>
		/// <param name="storageMediaFileSetUid">Optional value specifying the UID of the storage media on which the SOP instance resides.</param>
		protected virtual IHierarchicalSeriesInstanceReferenceMacro CreateSeriesReference(string seriesInstanceUid, string retrieveAeTitle = null, string retrieveLocationUid = null, string storageMediaFileSetId = null, string storageMediaFileSetUid = null)
		{
			IHierarchicalSeriesInstanceReferenceMacro reference = new HierarchicalSeriesInstanceReferenceMacro(new DicomSequenceItem());
			reference.InitializeAttributes();
			reference.SeriesInstanceUid = seriesInstanceUid;
			reference.RetrieveAeTitle = retrieveAeTitle;
			reference.RetrieveLocationUid = retrieveLocationUid;
			reference.StorageMediaFileSetId = storageMediaFileSetId;
			reference.StorageMediaFileSetUid = storageMediaFileSetUid;
			return reference;
		}

		/// <summary>
		/// Creates and initializes a <see cref="IReferencedSopSequence"/> to the given SOP instance.
		/// </summary>
		/// <param name="sopClassUid">The SOP class UID.</param>
		/// <param name="sopInstanceUid">The SOP instance UID.</param>
		protected virtual IReferencedSopSequence CreateSopReference(string sopClassUid, string sopInstanceUid)
		{
			IReferencedSopSequence reference = new HierarchicalSeriesInstanceReferenceMacro.ReferencedSopSequenceItem(new DicomSequenceItem());
			reference.InitializeAttributes();
			reference.ReferencedSopClassUid = sopClassUid;
			reference.ReferencedSopInstanceUid = sopInstanceUid;
			return reference;
		}

		/// <summary>
		/// Gets the references as an array of <see cref="IHierarchicalSopInstanceReferenceMacro"/>s.
		/// </summary>
		public IHierarchicalSopInstanceReferenceMacro[] ToArray()
		{
			return GetList().ToArray();
		}

		/// <summary>
		/// Gets the references as a readonly <see cref="IList{T}"/> of <see cref="IHierarchicalSopInstanceReferenceMacro"/>s.
		/// </summary>
		public IList<IHierarchicalSopInstanceReferenceMacro> ToList()
		{
			return GetList().AsReadOnly();
		}

		private List<IHierarchicalSopInstanceReferenceMacro> GetList()
		{
			List<IHierarchicalSopInstanceReferenceMacro> studyReferences = new List<IHierarchicalSopInstanceReferenceMacro>();
			foreach (var studyEntry in _dictionary)
			{
				IHierarchicalSopInstanceReferenceMacro studyReference = CreateStudyReference(studyEntry.Key);

				List<IHierarchicalSeriesInstanceReferenceMacro> seriesReferences = new List<IHierarchicalSeriesInstanceReferenceMacro>();
				foreach (var seriesEntry in studyEntry.Value)
				{
					var seriesReference = CreateSeriesReference(seriesEntry.Key.SeriesInstanceUid, seriesEntry.Key.RetrieveAeTitle, seriesEntry.Key.RetrieveLocationUid, seriesEntry.Key.StorageMediaFileSetId, seriesEntry.Key.StorageMediaFileSetUid);
					seriesReference.ReferencedSopSequence = seriesEntry.Value.Select(sop => CreateSopReference(sop.Value, sop.Key)).ToArray();
					seriesReferences.Add(seriesReference);
				}

				studyReference.ReferencedSeriesSequence = seriesReferences.ToArray();
				studyReferences.Add(studyReference);
			}

			return studyReferences;
		}

		/// <summary>
		/// Creates a hierarchical SOP instance reference sequence from the specified <see cref="HierarchicalSopInstanceReferenceDictionary"/>.
		/// </summary>
		public static implicit operator IHierarchicalSopInstanceReferenceMacro[](HierarchicalSopInstanceReferenceDictionary dictionary)
		{
			return dictionary.ToArray();
		}

		/// <summary>
		/// Creates an instance of <see cref="HierarchicalSopInstanceReferenceDictionary"/> from the specified hierarchical SOP instance reference sequence.
		/// </summary>
		public static explicit operator HierarchicalSopInstanceReferenceDictionary(IHierarchicalSopInstanceReferenceMacro[] hierarchicalSopInstanceReferenceSequence)
		{
			return new HierarchicalSopInstanceReferenceDictionary(hierarchicalSopInstanceReferenceSequence);
		}

		#region SeriesKey Type

		private struct SeriesKey : IEquatable<SeriesKey>
		{
			public readonly string SeriesInstanceUid;
			public readonly string RetrieveAeTitle;
			public readonly string RetrieveLocationUid;
			public readonly string StorageMediaFileSetId;
			public readonly string StorageMediaFileSetUid;

			public SeriesKey(string seriesInstanceUid, string retrieveAeTitle, string retrieveLocationUid, string storageMediaFileSetId, string storageMediaFileSetUid)
			{
				SeriesInstanceUid = !string.IsNullOrWhiteSpace(seriesInstanceUid) ? seriesInstanceUid.Trim() : string.Empty;
				RetrieveAeTitle = !string.IsNullOrWhiteSpace(retrieveAeTitle) ? retrieveAeTitle.Trim() : string.Empty;
				RetrieveLocationUid = !string.IsNullOrWhiteSpace(retrieveLocationUid) ? retrieveLocationUid.Trim() : string.Empty;
				StorageMediaFileSetId = !string.IsNullOrWhiteSpace(storageMediaFileSetId) ? storageMediaFileSetId.Trim() : string.Empty;
				StorageMediaFileSetUid = !string.IsNullOrWhiteSpace(storageMediaFileSetUid) ? storageMediaFileSetUid.Trim() : string.Empty;
			}

			public override int GetHashCode()
			{
				return SeriesInstanceUid.GetHashCode()
				       ^ RetrieveAeTitle.GetHashCode()
				       ^ RetrieveLocationUid.GetHashCode()
				       ^ StorageMediaFileSetId.GetHashCode()
				       ^ StorageMediaFileSetUid.GetHashCode();
			}

			public bool Equals(SeriesKey other)
			{
				return string.Equals(SeriesInstanceUid, other.SeriesInstanceUid)
				       && string.Equals(RetrieveAeTitle, other.RetrieveAeTitle)
				       && string.Equals(RetrieveLocationUid, other.RetrieveLocationUid)
				       && string.Equals(StorageMediaFileSetId, other.StorageMediaFileSetId)
				       && string.Equals(StorageMediaFileSetUid, other.StorageMediaFileSetUid);
			}

			public override bool Equals(object obj)
			{
				return obj is SeriesKey && Equals((SeriesKey) obj);
			}

			public override string ToString()
			{
				var sb = new StringBuilder(SeriesInstanceUid);
				if (!string.IsNullOrEmpty(RetrieveAeTitle)) sb.AppendFormat(" AETITLE={0}", RetrieveAeTitle);
				if (!string.IsNullOrEmpty(RetrieveLocationUid)) sb.AppendFormat(" LOC_UID={0}", RetrieveLocationUid);
				if (!string.IsNullOrEmpty(StorageMediaFileSetId)) sb.AppendFormat(" MEDIA_ID={0}", StorageMediaFileSetId);
				if (!string.IsNullOrEmpty(StorageMediaFileSetUid)) sb.AppendFormat(" MEDIA_UID={0}", StorageMediaFileSetUid);
				return sb.ToString();
			}
		}

		#endregion

		#region ICollection<Entry> Implementation

		bool ICollection<Entry>.IsReadOnly
		{
			get { return false; }
		}

		void ICollection<Entry>.Add(Entry entry)
		{
			AddReference(entry.StudyInstanceUid, entry.SeriesInstanceUid, entry.SopClassUid, entry.SopInstanceUid,
			             entry.RetrieveAeTitle, entry.RetrieveLocationUid, entry.StorageMediaFileSetId, entry.StorageMediaFileSetUid);
		}

		bool ICollection<Entry>.Remove(Entry entry)
		{
			return TryRemoveReference(entry.StudyInstanceUid, entry.SeriesInstanceUid, entry.SopClassUid, entry.SopInstanceUid,
			                          entry.RetrieveAeTitle, entry.RetrieveLocationUid, entry.StorageMediaFileSetId, entry.StorageMediaFileSetUid);
		}

		bool ICollection<Entry>.Contains(Entry entry)
		{
			return ContainsReference(entry.StudyInstanceUid, entry.SeriesInstanceUid, entry.SopClassUid, entry.SopInstanceUid,
			                         entry.RetrieveAeTitle, entry.RetrieveLocationUid, entry.StorageMediaFileSetId, entry.StorageMediaFileSetUid);
		}

		void ICollection<Entry>.CopyTo(Entry[] array, int arrayIndex)
		{
			var sourceArray = EnumerateEntries().ToArray();
			Array.Copy(sourceArray, 0, array, arrayIndex, sourceArray.Length);
		}

		public IEnumerator<Entry> GetEnumerator()
		{
			return EnumerateEntries().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private IEnumerable<Entry> EnumerateEntries()
		{
			return from study in _dictionary
			       from series in study.Value
			       from sop in series.Value
			       select new Entry(study.Key, series.Key.SeriesInstanceUid, sop.Value, sop.Key,
			                        series.Key.RetrieveAeTitle,
			                        series.Key.RetrieveLocationUid,
			                        series.Key.StorageMediaFileSetId,
			                        series.Key.StorageMediaFileSetUid);
		}

		public struct Entry : IEquatable<Entry>
		{
			public readonly string StudyInstanceUid;
			public readonly string SeriesInstanceUid;
			public readonly string SopInstanceUid;
			public readonly string SopClassUid;

			public readonly string RetrieveAeTitle;
			public readonly string RetrieveLocationUid;
			public readonly string StorageMediaFileSetId;
			public readonly string StorageMediaFileSetUid;

			public Entry(string studyInstanceUid, string seriesInstanceUid, string sopClassUid, string sopInstanceUid,
			             string retrieveAeTitle = null, string retrieveLocationUid = null, string storageMediaFileSetId = null, string storageMediaFileSetUid = null)
			{
				StudyInstanceUid = !string.IsNullOrWhiteSpace(studyInstanceUid) ? studyInstanceUid.Trim() : string.Empty;
				SeriesInstanceUid = !string.IsNullOrWhiteSpace(seriesInstanceUid) ? seriesInstanceUid.Trim() : string.Empty;
				SopInstanceUid = !string.IsNullOrWhiteSpace(sopInstanceUid) ? sopInstanceUid.Trim() : string.Empty;
				SopClassUid = !string.IsNullOrWhiteSpace(sopClassUid) ? sopClassUid.Trim() : string.Empty;
				RetrieveAeTitle = !string.IsNullOrWhiteSpace(retrieveAeTitle) ? retrieveAeTitle.Trim() : string.Empty;
				RetrieveLocationUid = !string.IsNullOrWhiteSpace(retrieveLocationUid) ? retrieveLocationUid.Trim() : string.Empty;
				StorageMediaFileSetId = !string.IsNullOrWhiteSpace(storageMediaFileSetId) ? storageMediaFileSetId.Trim() : string.Empty;
				StorageMediaFileSetUid = !string.IsNullOrWhiteSpace(storageMediaFileSetUid) ? storageMediaFileSetUid.Trim() : string.Empty;
			}

			public override int GetHashCode()
			{
				return StudyInstanceUid.GetHashCode()
				       ^ SeriesInstanceUid.GetHashCode()
				       ^ SopInstanceUid.GetHashCode()
				       ^ SopClassUid.GetHashCode();
			}

			public bool Equals(Entry other)
			{
				return string.Equals(StudyInstanceUid, other.StudyInstanceUid)
				       && string.Equals(SeriesInstanceUid, other.SeriesInstanceUid)
				       && string.Equals(SopInstanceUid, other.SopInstanceUid)
				       && string.Equals(SopClassUid, other.SopClassUid)
				       && string.Equals(RetrieveAeTitle, other.RetrieveAeTitle)
				       && string.Equals(RetrieveLocationUid, other.RetrieveLocationUid)
				       && string.Equals(StorageMediaFileSetId, other.StorageMediaFileSetId)
				       && string.Equals(StorageMediaFileSetUid, other.StorageMediaFileSetUid);
			}

			public override bool Equals(object obj)
			{
				return obj is Entry && Equals((Entry) obj);
			}
		}

		#endregion
	}
}