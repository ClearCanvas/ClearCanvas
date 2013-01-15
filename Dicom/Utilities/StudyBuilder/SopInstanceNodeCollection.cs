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
	/// Represents a list of <see cref="SopInstanceNode"/>s (SOP instance-level data nodes) in the <see cref="StudyBuilder"/> tree hierarchy.
	/// </summary>
	public sealed class SopInstanceNodeCollection : IList<SopInstanceNode>, IUidCollection {
		private const string EX_ALREADYEXIST = "That SOP instance already exists in a different location in the study builder tree hierarchy.";
		private const string EX_DOESNOTEXIST = "That SOP instance does not exist in this list.";
		private readonly List<SopInstanceNode> _images = new List<SopInstanceNode>();
		private readonly SeriesNode _series;

		/// <summary>
		/// Constructs a collection owned by the specified series.
		/// </summary>
		/// <param name="series"></param>
		internal SopInstanceNodeCollection(SeriesNode series) {
			_series = series;
		}

		/// <summary>
		/// Returns a SOP instance node with the given SOP instance UID, creating a new <see cref="SopInstanceNode"/> if one does not already exist.
		/// </summary>
		/// <param name="sopInstanceUid">The SOP instance UID to lookup.</param>
		/// <returns>A SOP instance node.</returns>
		public SopInstanceNode GetImageByUid(string sopInstanceUid) {
			try {
				return (SopInstanceNode)((IUidCollection)this)[sopInstanceUid];
			} catch (KeyNotFoundException) {
				SopInstanceNode sopInstance = new SopInstanceNode();
				this.Add(sopInstance);
				return sopInstance;
			}
		}

		/// <summary>
		/// Returns a SOP instance node with data similar to the attributres in the provided <see cref="DicomFile"/>
		/// based on the SOP instance UID, creating a new <see cref="SopInstanceNode"/> if one does not already exist.
		/// </summary>
		/// <param name="dicomFile">The <see cref="DicomFile"/> containing the data to lookup, and to base a new <see cref="SopInstanceNode"/> on if one does not already exist.</param>
		/// <returns>A SOP instance node.</returns>
		public SopInstanceNode GetImageByUid(DicomFile dicomFile) {
			try {
				string sopInstanceUid = dicomFile.DataSet[DicomTags.SopInstanceUid].GetString(0, "");
				return (SopInstanceNode)((IUidCollection)this)[sopInstanceUid];
			} catch (KeyNotFoundException) {
				SopInstanceNode sopInstance = new SopInstanceNode(dicomFile);
				this.Add(sopInstance);
				return sopInstance;
			}
		}

		/// <summary>
		/// Returns the index of the given SOP instance node.
		/// </summary>
		/// <param name="sopInstance">The SOP instance node to lookup.</param>
		/// <returns>The index of the node in the list, or -1 if the node is not in the list.</returns>
		public int IndexOf(SopInstanceNode sopInstance) {
			return _images.IndexOf(sopInstance);
		}

		/// <summary>
		/// Inserts a SOP instance node into the list at the specified index.
		/// </summary>
		/// <param name="index">The index at which to insert the node.</param>
		/// <param name="sopInstance">The SOP instance to add to the list.</param>
		public void Insert(int index, SopInstanceNode sopInstance) {
			if (sopInstance.Parent != null)
				throw new ArgumentException(EX_ALREADYEXIST);
			sopInstance.Parent = _series;
			_images.Insert(index, sopInstance);
		}

		/// <summary>
		/// Removes the SOP instance node at the specified index from the list.
		/// </summary>
		/// <param name="index">The index of the node to remove.</param>
		public void RemoveAt(int index) {
			if (_images[index].Parent != _series)
				throw new ArgumentException(EX_DOESNOTEXIST);
			_images[index].Parent = null;
			_images.RemoveAt(index);
		}

		/// <summary>
		/// Gets or sets the SOP instance node at the specified index in the list.
		/// </summary>
		/// <param name="index">The index of the node.</param>
		public SopInstanceNode this[int index] {
			get { return _images[index]; }
			set {
				if (_images[index].Parent != _series)
					throw new ArgumentException(EX_DOESNOTEXIST);
				if (value.Parent != null)
					throw new ArgumentException(EX_ALREADYEXIST);
				value.Parent = _series;
				_images[index].Parent = null;

				_images[index] = value;
			}
		}

		/// <summary>
		/// Adds a new SOP instance node to the end of the list.
		/// </summary>
		/// <param name="sopInstance">The SOP instance to add to the list.</param>
		public void Add(SopInstanceNode sopInstance) {
			if (sopInstance.Parent != null)
				throw new ArgumentException(EX_ALREADYEXIST);
			sopInstance.Parent = _series;
			_images.Add(sopInstance);
		}

		/// <summary>
		/// Removes all SOP instance nodes from the list.
		/// </summary>
		public void Clear() {
			foreach (SopInstanceNode node in _images) {
				node.Parent = null;
			}
			_images.Clear();
		}

		/// <summary>
		/// Checks if the list contains the specified SOP instance node.
		/// </summary>
		/// <param name="sopInstance">The SOP instance node to lookup.</param>
		/// <returns>True if the collection contains the given SOP instance, False if otherwise.</returns>
		public bool Contains(SopInstanceNode sopInstance) {
			return _images.Contains(sopInstance);
		}

		/// <summary>
		/// Copes all the SOP instances in this list into a <see cref="SopInstanceNode"/> array, starting at the specified array index.
		/// </summary>
		/// <param name="array">The array to which the nodes are copied.</param>
		/// <param name="arrayIndex">The array index at which copying begins.</param>
		public void CopyTo(SopInstanceNode[] array, int arrayIndex) {
			foreach (SopInstanceNode image in _images) {
				array[arrayIndex++] = image;
			}
		}

		/// <summary>
		/// Gets the number of SOP instance nodes contained in this list.
		/// </summary>
		public int Count {
			get { return _images.Count; }
		}

		/// <summary>
		/// Gets whether or not this list is read-only.
		/// </summary>
		public bool IsReadOnly {
			get { return false; }
		}

		/// <summary>
		/// Removes the given SOP instance node from this list.
		/// </summary>
		/// <param name="sopInstance">The SOP instance node to remove from this list.</param>
		/// <returns>True if the SOP instance was successfully removed, False if otherwise.</returns>
		public bool Remove(SopInstanceNode sopInstance) {
			if (sopInstance.Parent != _series)
				throw new ArgumentException(EX_DOESNOTEXIST);
			sopInstance.Parent = null;
			return _images.Remove(sopInstance);
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates throuygh the <see cref="SopInstanceNode"/>s contained in this list.
		/// </summary>
		/// <returns>A <see cref="SopInstanceNode"/> iterator.</returns>
		public IEnumerator<SopInstanceNode> GetEnumerator() {
			return _images.GetEnumerator();
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
				foreach (SopInstanceNode image in _images) {
					if (image.InstanceUid == uid)
						return image;
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
			foreach (SopInstanceNode image in _images) {
				if (image.InstanceUid == uid)
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
			foreach (SopInstanceNode image in _images) {
				array[arrayIndex++] = image.InstanceUid;
			}
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates through the instance UIDs of the data nodes contained in this collection.
		/// </summary>
		/// <returns>A <see cref="string"/> iterator.</returns>
		IEnumerator<string> IUidCollection.GetEnumerator() {
			List<string> list = new List<string>();
			foreach (SopInstanceNode image in _images) {
				list.Add(image.InstanceUid);
			}
			return list.GetEnumerator();
		}

		#endregion
	}
}
