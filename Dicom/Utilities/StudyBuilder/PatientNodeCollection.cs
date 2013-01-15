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
	/// Represents a collection of <see cref="PatientNode"/>s (patient-level data nodes) in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class PatientNodeCollection : ICollection<PatientNode>, ICollection<string> {
		private const string EX_ALREADYEXISTS = "That patient already exists in a different location in the study builder tree hierarchy.";
		private const string EX_DOESNOTEXIST = "That patient does not exist in this collection.";
		private readonly Dictionary<string, PatientNode> _patients = new Dictionary<string, PatientNode>();
		private readonly StudyBuilder _builder;

		/// <summary>
		/// Constructs a collection owned by the specified builder.
		/// </summary>
		/// <param name="builder">The builder that owns the collection.</param>
		internal PatientNodeCollection(StudyBuilder builder) {
			_builder = builder;
		}

		/// <summary>
		/// Gets the patient node associated with the given patient ID.
		/// </summary>
		/// <param name="patientId">The patient ID to lookup.</param>
		public PatientNode this[string patientId] {
			get {
				foreach (PatientNode patient in _patients.Values) {
					if (patient.PatientId == patientId)
						return patient;
				}
				throw new KeyNotFoundException();
			}
		}

		/// <summary>
		/// Returns a patient node with the given patient ID, creating a new <see cref="PatientNode"/> if one does not already exist.
		/// </summary>
		/// <param name="patientId">The patient ID to lookup.</param>
		/// <returns>A patient node.</returns>
		public PatientNode GetPatientById(string patientId) {
			try {
				return this[patientId];
			} catch (KeyNotFoundException) {
				PatientNode patient = new PatientNode(patientId);
				this.Add(patient);
				return patient;
			}
		}

		/// <summary>
		/// Returns a patient node with data similar to the provided data set
		/// based on the patient ID, creating a new patient if one does not already exist.
		/// </summary>
		/// <param name="dicomDataSet">The <see cref="DicomAttributeCollection"/> to lookup, and to base a new <see cref="PatientNode"/> on if one does not already exist.</param>
		/// <returns>A patient node.</returns>
		public PatientNode GetPatientById(DicomAttributeCollection dicomDataSet) {
			try {
				string patientId = dicomDataSet[DicomTags.PatientId].GetString(0, "");
				return this[patientId];
			} catch (Exception) {
				PatientNode patient = new PatientNode(dicomDataSet);
				this.Add(patient);
				return patient;
			}
		}

		/// <summary>
		/// Adds a patient node to the collection.
		/// </summary>
		/// <param name="patient">The patient to add to the collection.</param>
		public void Add(PatientNode patient) {
			if(patient.Parent!=null)
				throw new ArgumentException(EX_ALREADYEXISTS);
			patient.Parent = _builder.Root;
			_patients.Add(patient.Key, patient);
		}

		/// <summary>
		/// Adds a new patient node with the given patient ID to the collection.
		/// </summary>
		/// <param name="patientId">The patient ID used to create the new patient node.</param>
		public void Add(string patientId) {
			this.Add(new PatientNode(patientId));
		}

		/// <summary>
		/// Removes all patient nodes from the collection.
		/// </summary>
		public void Clear() {
			foreach (PatientNode node in _patients.Values) {
				node.Parent = null;
			}
			_patients.Clear();
		}

		/// <summary>
		/// Checks if the collection contains the specified patient node.
		/// </summary>
		/// <param name="patient">The patient node to lookup.</param>
		/// <returns>True if the collection contains the given patient, False if otherwise.</returns>
		public bool Contains(PatientNode patient) {
			return _patients.ContainsValue(patient);
		}

		/// <summary>
		/// Checks if the collection contains a patient node with the given patient ID.
		/// </summary>
		/// <param name="patientId">The patient ID to lookup.</param>
		/// <returns>True if the collection contains the given patient, False if otherwise.</returns>
		public bool Contains(string patientId) {
			foreach (PatientNode patient in _patients.Values) {
				if (patient.PatientId == patientId)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Copies all the patients in this collection into a <see cref="PatientNode"/> array, starting at the specified array index.
		/// </summary>
		/// <param name="array">The array to which the nodes are copied.</param>
		/// <param name="arrayIndex">The array index at which copying begins.</param>
		public void CopyTo(PatientNode[] array, int arrayIndex) {
			foreach (PatientNode patient in _patients.Values) {
				array[arrayIndex++] = patient;
			}
		}

		/// <summary>
		/// Copies all the patient IDs in this collection into a <see cref="string"/> array, starting at the specified array index.
		/// </summary>
		/// <param name="array">The array to which the nodes are copied.</param>
		/// <param name="arrayIndex">The array index at which copying begins.</param>
		public void CopyTo(string[] array, int arrayIndex) {
			foreach (PatientNode patient in _patients.Values) {
				array[arrayIndex++] = patient.PatientId;
			}
		}

		/// <summary>
		/// Gets the number of patient nodes contained in this collection.
		/// </summary>
		public int Count {
			get { return _patients.Count; }
		}

		/// <summary>
		/// Gets whether or not this collection is read-only.
		/// </summary>
		public bool IsReadOnly {
			get { return false; }
		}

		/// <summary>
		/// Removes the given patient node from this collection.
		/// </summary>
		/// <param name="patient">The patient node to remove from this collection.</param>
		/// <returns>True if the patient was successfully removed, False if otherwise.</returns>
		public bool Remove(PatientNode patient) {
			if(patient.Parent != _builder.Root)
				throw new ArgumentException(EX_DOESNOTEXIST);
			patient.Parent = null;
			return _patients.Remove(patient.Key);
		}

		/// <summary>
		/// Removes the patient node with the given patient ID from this collection.
		/// </summary>
		/// <param name="patientId">The patient ID to remove from this collection.</param>
		/// <returns>True if the patient was successfully removed, False if otherwise.</returns>
		public bool Remove(string patientId) {
			foreach (PatientNode patient in _patients.Values) {
				if (patient.PatientId == patientId)
					return this.Remove(patient);
			}
			return false;
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates through the <see cref="PatientNode"/>s contained in this collection.
		/// </summary>
		/// <returns>A <see cref="PatientNode"/> iterator.</returns>
		public IEnumerator<PatientNode> GetEnumerator() {
			return _patients.Values.GetEnumerator();
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates through the patient IDs of the patient nodes contained in this collection.
		/// </summary>
		/// <returns>A <see cref="string"/> iterator.</returns>
		IEnumerator<string> IEnumerable<string>.GetEnumerator() {
			List<string> list = new List<string>();
			foreach (PatientNode patient in _patients.Values) {
				list.Add(patient.PatientId);
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
	}
}
