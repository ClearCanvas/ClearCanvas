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

namespace ClearCanvas.Dicom.Utilities.StudyBuilder {
	/// <summary>
	/// Represents a collection of <see cref="StudyNode">StudyNodes</see> (study-level data nodes) in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class StudyNodeCollection : ICollection<StudyNode>, ICollection<string>, IUidCollection {
		private const string EX_ALREADYEXISTS = "That study already exists in a different location in the study builder tree hierarchy.";
		private const string EX_DOESNOTEXIST = "That study does not exist in this collection.";
		private readonly Dictionary<string, StudyNode> _studies = new Dictionary<string, StudyNode>();
		private readonly PatientNode _patient;

		/// <summary>
		/// Constructs a collection owned by the specified patient.
		/// </summary>
		/// <param name="patient">The patient node that owns the collection.</param>
		internal StudyNodeCollection(PatientNode patient) {
			_patient = patient;
		}

		/// <summary>
		/// Gets the study node associated with the given study ID.
		/// </summary>
		/// <param name="studyId">The study ID to lookup.</param>
		public StudyNode this[string studyId] {
			get {
				foreach (StudyNode study in _studies.Values) {
					if (study.StudyId == studyId)
						return study;
				}
				throw new KeyNotFoundException();
			}
		}

		/// <summary>
		/// Returns a study node with the given study ID, creating a new <see cref="StudyNode"/> if one does not already exist.
		/// </summary>
		/// <param name="studyId">The study ID to lookup.</param>
		/// <returns>A study node.</returns>
		public StudyNode GetStudyById(string studyId) {
			try {
				return this[studyId];
			} catch (KeyNotFoundException) {
				StudyNode study = new StudyNode(studyId);
				this.Add(study);
				return study;
			}
		}

		/// <summary>
		/// Returns a study node with data similar to the provided data set
		/// based on the study ID, creating a new study if one does not already exist.
		/// </summary>
		/// <param name="dicomDataSet">The <see cref="DicomAttributeCollection"/> to lookup, and to base a new <see cref="StudyNode"/> on if one does not already exist.</param>
		/// <returns>A study node.</returns>
		public StudyNode GetStudyById(DicomAttributeCollection dicomDataSet) {
			try {
				string studyId = dicomDataSet[DicomTags.StudyId].GetString(0, "");
				return this[studyId];
			} catch (Exception) {
				StudyNode study = new StudyNode(dicomDataSet);
				this.Add(study);
				return study;
			}
		}

		/// <summary>
		/// Returns a study node with the given study instance UID, creating a new <see cref="StudyNode"/> if one does not already exist.
		/// </summary>
		/// <param name="studyUid">The study instance UID to lookup.</param>
		/// <returns>A study node.</returns>
		public StudyNode GetStudyByUid(string studyUid) {
			try {
				return (StudyNode)((IUidCollection)this)[studyUid];
			} catch (KeyNotFoundException) {
				StudyNode study = new StudyNode();
				this.Add(study);
				return study;
			}
		}

		/// <summary>
		/// Returns a study node with data similar to the provided data set
		/// based on the study instance UID, creating a new <see cref="StudyNode"/> if one does not already exist.
		/// </summary>
		/// <param name="dicomDataSet">The <see cref="DicomAttributeCollection"/> to lookup, and to base a new <see cref="StudyNode"/> on if one does not already exist.</param>
		/// <returns>A study node.</returns>
		public StudyNode GetStudyByUid(DicomAttributeCollection dicomDataSet) {
			try {
				string studyUid = dicomDataSet[DicomTags.StudyInstanceUid].GetString(0, "");
				return (StudyNode)((IUidCollection)this)[studyUid];
			} catch (KeyNotFoundException) {
				StudyNode study = new StudyNode(dicomDataSet);
				this.Add(study);
				return study;
			}
		}

		/// <summary>
		/// Adds a study node to the collection.
		/// </summary>
		/// <param name="study">The study to add to the collection.</param>
		public void Add(StudyNode study) {
			if (study.Parent != null)
				throw new ArgumentException(EX_ALREADYEXISTS);
			study.Parent = _patient;
			_studies.Add(study.Key, study);
		}

		/// <summary>
		/// Adds a new study node with the given study ID to the collection.
		/// </summary>
		/// <param name="studyId">The study ID used to create the new study node.</param>
		public void Add(string studyId) {
			this.Add(new StudyNode(studyId));
		}

		/// <summary>
		/// Removes all study nodes from the collection.
		/// </summary>
		public void Clear() {
			foreach (StudyNode node in _studies.Values) {
				node.Parent = null;
			}
			_studies.Clear();
		}

		/// <summary>
		/// Checks if the collection contains the specified study node.
		/// </summary>
		/// <param name="study">The study node to lookup.</param>
		/// <returns>True if the collection contains the given study, False if otherwise.</returns>
		public bool Contains(StudyNode study) {
			return _studies.ContainsValue(study);
		}

		/// <summary>
		/// Checks if the collection contains a study node with the given study ID.
		/// </summary>
		/// <param name="studyId">The study ID to lookup.</param>
		/// <returns>True if the collection contains the given study, False if otherwise.</returns>
		public bool Contains(string studyId) {
			foreach (StudyNode study in _studies.Values) {
				if (study.StudyId == studyId)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Copies all the studies in this collection into a <see cref="StudyNode"/> array, starting at the specified array index.
		/// </summary>
		/// <param name="array">The array to which the nodes are copied.</param>
		/// <param name="arrayIndex">The array index at which copying begins.</param>
		public void CopyTo(StudyNode[] array, int arrayIndex) {
			foreach (StudyNode study in _studies.Values) {
				array[arrayIndex++] = study;
			}
		}

		/// <summary>
		/// Copies all the study IDs in this collection into a <see cref="string"/> array, starting at the specified array index.
		/// </summary>
		/// <param name="array">The array to which the nodes are copied.</param>
		/// <param name="arrayIndex">The array index at which copying begins.</param>
		public void CopyTo(string[] array, int arrayIndex) {
			foreach (StudyNode study in _studies.Values) {
				array[arrayIndex++] = study.StudyId;
			}
		}

		/// <summary>
		/// Gets the number of study nodes contained in this collection.
		/// </summary>
		public int Count {
			get { return _studies.Count; }
		}

		/// <summary>
		/// Gets whether or not this collection is read-only.
		/// </summary>
		public bool IsReadOnly {
			get { return false; }
		}

		/// <summary>
		/// Removes the given study node from this collection.
		/// </summary>
		/// <param name="study">The study node to remove from this collection.</param>
		/// <returns>True if the study was successfully removed, False if otherwise.</returns>
		public bool Remove(StudyNode study) {
			if (study.Parent != _patient)
				throw new ArgumentException(EX_DOESNOTEXIST);
			study.Parent = null;
			return _studies.Remove(study.Key);
		}

		/// <summary>
		/// Removes the study node with the given study ID from this collection.
		/// </summary>
		/// <param name="studyId">The study ID to remove from this collection.</param>
		/// <returns>True if the study was successfully removed, False if otherwise.</returns>
		public bool Remove(string studyId) {
			foreach (StudyNode study in _studies.Values) {
				if (study.StudyId == studyId)
					return this.Remove(study);
			}
			return false;
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates through the <see cref="StudyNode"/>s contained in this collection.
		/// </summary>
		/// <returns>A <see cref="StudyNode"/> iterator.</returns>
		public IEnumerator<StudyNode> GetEnumerator() {
			return _studies.Values.GetEnumerator();
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates through the study IDs of the study nodes contained in this collection.
		/// </summary>
		/// <returns>A <see cref="string"/> iterator.</returns>
		IEnumerator<string> IEnumerable<string>.GetEnumerator() {
			List<string> list = new List<string>();
			foreach (StudyNode study in _studies.Values) {
				list.Add(study.StudyId);
			}
			return list.GetEnumerator();
		}

		///<summary>
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#region IUidCollection Members

		/// <summary>
		/// Gets the <see cref="StudyBuilderNode"/> associated with the given UID.
		/// </summary>
		/// <param name="uid">The DICOM UID of the node to retrieve from the collection.</param>
		StudyBuilderNode IUidCollection.this[string uid] {
			get {
				foreach (StudyNode study in _studies.Values) {
					if (study.InstanceUid == uid)
						return study;
				}
				throw new KeyNotFoundException();
			}
		}

		/// <summary>
		/// Checks if the collection contains a node with the specified UID.
		/// </summary>
		/// <param name="uid">The DICOM UID of the node to check in the collection.</param>
		/// <returns>True if the collection has such a node, False otherwise.</returns>
		bool IUidCollection.Contains(string uid) {
			foreach (StudyNode study in _studies.Values) {
				if (study.InstanceUid == uid)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Copies the UIDs of the nodes in the collection to a <see cref="string"/> array, starting at a particular array index.
		/// </summary>
		/// <param name="array">The array to copy the UIDs into.</param>
		/// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
		void IUidCollection.CopyTo(string[] array, int arrayIndex) {
			foreach (StudyNode study in _studies.Values) {
				array[arrayIndex++] = study.InstanceUid;
			}
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates through the instance UIDs of the data nodes contained in this collection.
		/// </summary>
		/// <returns>A <see cref="string"/> iterator.</returns>
		IEnumerator<string> IUidCollection.GetEnumerator() {
			List<string> list = new List<string>();
			foreach (StudyNode study in _studies.Values) {
				list.Add(study.InstanceUid);
			}
			return list.GetEnumerator();
		}

		#endregion
	}
}
