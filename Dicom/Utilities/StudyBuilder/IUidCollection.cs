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

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// Represents a collection of <see cref="StudyBuilderNode"/> objects that have associated DICOM UIDs (e.g. studies, series and SOP instances).
	/// </summary>
	public interface IUidCollection
	{
		/// <summary>
		/// Gets the <see cref="StudyBuilderNode"/> associated with the given UID.
		/// </summary>
		/// <param name="uid">The DICOM UID of the node to retrieve from the collection.</param>
		StudyBuilderNode this[string uid] { get; }

		/// <summary>
		/// Checks if the collection contains a node with the specified UID.
		/// </summary>
		/// <param name="uid">The DICOM UID of the node to check in the collection.</param>
		/// <returns>True if the collection has such a node, False otherwise.</returns>
		bool Contains(string uid);

		/// <summary>
		/// Gets the number of nodes in the collection.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Copies the UIDs of the nodes in the collection to a <see cref="string"/> array, starting at a particular array index.
		/// </summary>
		/// <param name="array">The array to copy the UIDs into.</param>
		/// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
		void CopyTo(string[] array, int arrayIndex);

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates through the instance UIDs of the data nodes contained in this collection.
		/// </summary>
		/// <returns>A <see cref="string"/> iterator.</returns>
		IEnumerator<string> GetEnumerator();
	}
}