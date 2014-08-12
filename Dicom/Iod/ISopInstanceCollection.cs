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

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Represents the collection of SOP instances in a <see cref="ISeries"/>.
	/// </summary>
	public interface ISopInstanceCollection : IEnumerable<ISopInstance>
	{
		/// <summary>
		/// Gets the number of SOP instances in the series.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets a value indicating whether the specified SOP instance UID exists in this series.
		/// </summary>
		/// <param name="sopInstanceUid"></param>
		/// <returns></returns>
		bool Contains(string sopInstanceUid);

		/// <summary>
		/// Gets the specified SOP instance.
		/// </summary>
		/// <param name="sopInstanceUid"> </param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">The specified SOP instance UID is not valid.</exception>
		ISopInstance Get(string sopInstanceUid);

		/// <summary>
		/// Gets the specified SOP instance.
		/// </summary>
		/// <param name="sopInstanceUid"> </param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">The specified SOP instance UID is not valid.</exception>
		ISopInstance this[string sopInstanceUid] { get; }

		/// <summary>
		/// Gets the specified SOP instance, if it exists.
		/// </summary>
		/// <param name="sopInstanceUid"> </param>
		/// <param name="sopInstance"> </param>
		/// <returns></returns>
		bool TryGet(string sopInstanceUid, out ISopInstance sopInstance);
	}
}
