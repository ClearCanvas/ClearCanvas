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
	/// Represents a list of <see cref="SeriesNode"/>s (series-level data nodes) in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class SeriesNodeCollection : IList<SeriesNode>, IUidCollection {
		private const string EX_ALREADYEXIST = "That series already exists in a different location in the study builder tree hierarchy.";
		private const string EX_DOESNOTEXIST = "That series does not exist in this list.";
		private readonly List<SeriesNode> _series = new List<SeriesNode>();
		private readonly StudyNode _study;

		/// <summary>
		/// Constructs a collection owned by the specified study.
		/// </summary>
		/// <param name="study"></param>
		internal SeriesNodeCollection(StudyNode study) {
			_study = study;
		}

		/// <summary>
		/// Returns a series node with the given series instance UID, creating a new <see cref="SeriesNode"/> if one does not already exist.
		/// </summary>
		/// <param name="seriesUid">The series instance UID to lookup.</param>
		/// <returns>A series node.</returns>
		public SeriesNode GetSeriesByUid(string seriesUid) {
			try {
				return (SeriesNode)((IUidCollection)this)[seriesUid];
			} catch (KeyNotFoundException) {
				SeriesNode series = new SeriesNode();
				this.Add(series);
				return series;
			}
		}

		/// <summary>
		/// Returns a SOP instance node with data similar to the attributres in the provided <see cref="DicomAttributeCollection"/>
		/// based on the series instance UID, creating a new <see cref="SeriesNode"/> if one does not already exist.
		/// </summary>
		/// <param name="dicomDataSet">The <see cref="DicomAttributeCollection"/> containing the data to lookup, and to base a new <see cref="SeriesNode"/> on if one does not already exist.</param>
		/// <returns>A series node.</returns>
		public SeriesNode GetSeriesByUid(DicomAttributeCollection dicomDataSet) {
			try {
				string seriesUid = dicomDataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
				return (SeriesNode)((IUidCollection)this)[seriesUid];
			} catch (KeyNotFoundException) {
				SeriesNode series = new SeriesNode(dicomDataSet);
				this.Add(series);
				return series;
			}
		}

		/// <summary>
		/// Returns the index of the given series node.
		/// </summary>
		/// <param name="series">Tje seris node to lookup.</param>
		/// <returns>The index of the node in the list, or -1 if the node is not in the list.</returns>
		public int IndexOf(SeriesNode series) {
			return _series.IndexOf(series);
		}

		/// <summary>
		/// Inserts a series node into the list as the specified index.
		/// </summary>
		/// <param name="index">The index at which to insert the node.</param>
		/// <param name="series">The series to add to the list.</param>
		public void Insert(int index, SeriesNode series) {
			if (series.Parent != null)
				throw new ArgumentException(EX_ALREADYEXIST);
			series.Parent = _study;
			_series.Insert(index, series);
		}

		/// <summary>
		/// Removes the series node at the specified index from the list.
		/// </summary>
		/// <param name="index">The index of the node to remove.</param>
		public void RemoveAt(int index) {
			if (_series[index].Parent != _study)
				throw new ArgumentException(EX_DOESNOTEXIST);
			_series[index].Parent = null;
			_series.RemoveAt(index);
		}

		/// <summary>
		/// Gets or sets the series node at the specified index in the list.
		/// </summary>
		/// <param name="index">The index of the node.</param>
		public SeriesNode this[int index] {
			get { return _series[index]; }
			set {
				if (_series[index].Parent != _study)
					throw new ArgumentException(EX_DOESNOTEXIST);
				if (value.Parent != null)
					throw new ArgumentException(EX_ALREADYEXIST);
				value.Parent = _study;
				_series[index].Parent = null;

				_series[index] = value;
			}
		}

		/// <summary>
		/// Adds a new series node to the end of the list.
		/// </summary>
		/// <param name="series">The series to add to the list.</param>
		public void Add(SeriesNode series) {
			if (series.Parent != null)
				throw new ArgumentException(EX_ALREADYEXIST);
			series.Parent = _study;
			_series.Add(series);
		}

		/// <summary>
		/// Removes all series nodes from the list.
		/// </summary>
		public void Clear() {
			foreach (SeriesNode node in _series) {
				node.Parent = null;
			}
			_series.Clear();
		}

		/// <summary>
		/// Checks if the list contains the specified series node.
		/// </summary>
		/// <param name="series">The series node to lookup.</param>
		/// <returns>True if the collection contains the given series, False if otherwise.</returns>
		public bool Contains(SeriesNode series) {
			return _series.Contains(series);
		}

		/// <summary>
		/// Copes all the series nodes in this list into a <see cref="SeriesNode"/> array, starting at the specified array index.
		/// </summary>
		/// <param name="array">The array to which the nodes are copied.</param>
		/// <param name="arrayIndex">The array index at which copying begins.</param>
		public void CopyTo(SeriesNode[] array, int arrayIndex) {
			foreach (SeriesNode series in _series) {
				array[arrayIndex++] = series;
			}
		}

		/// <summary>
		/// Gets the number of series nodes contained in this list.
		/// </summary>
		public int Count {
			get { return _series.Count; }
		}

		/// <summary>
		/// Gets whether or not this list is read-only.
		/// </summary>
		public bool IsReadOnly {
			get { return false; }
		}

		/// <summary>
		/// Removes the given series node from this list.
		/// </summary>
		/// <param name="series">The series node to remove from this list.</param>
		/// <returns>True if the series was successfully removed, False if otherwise.</returns>
		public bool Remove(SeriesNode series) {
			if (series.Parent != _study)
				throw new ArgumentException(EX_DOESNOTEXIST);
			series.Parent = null;
			return _series.Remove(series);
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates throuygh the <see cref="SeriesNode"/>s contained in this list.
		/// </summary>
		/// <returns>A <see cref="SeriesNode"/> iterator.</returns>
		public IEnumerator<SeriesNode> GetEnumerator() {
			return _series.GetEnumerator();
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
				foreach (SeriesNode series in _series) {
					if (series.InstanceUid == uid)
						return series;
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
			foreach (SeriesNode series in _series) {
				if (series.InstanceUid == uid)
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
			foreach (SeriesNode series in _series) {
				array[arrayIndex++] = series.InstanceUid;
			}
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates through the instance UIDs of the data nodes contained in this collection.
		/// </summary>
		/// <returns>A <see cref="string"/> iterator.</returns>
		IEnumerator<string> IUidCollection.GetEnumerator() {
			List<string> list = new List<string>();
			foreach (SeriesNode series in _series) {
				list.Add(series.InstanceUid);
			}
			return list.GetEnumerator();
		}

		#endregion
	}
}
