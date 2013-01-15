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
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparing <see cref="Sop"/>s.
	/// </summary>
	public abstract class DicomSopComparer : PresentationImageComparer, IComparer<Sop>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomSopComparer"/>.
		/// </summary>
		protected DicomSopComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomSopComparer"/>.
		/// </summary>
		protected DicomSopComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IPresentationImage> Members

		/// <summary>
		/// Compares two <see cref="IPresentationImage"/>s.
		/// </summary>
		public override int Compare(IPresentationImage x, IPresentationImage y)
		{
			ISopProvider xProvider = x as ISopProvider;
			ISopProvider yProvider = y as ISopProvider;

			if (ReferenceEquals(xProvider, yProvider))
				return 0; //same object or both are null

			//at this point, at least one of x or y is non-null and they are not the same object

			if (xProvider == null)
				return -ReturnValue; // x > y (because we want x at the end for non-reverse sorting)
			if (yProvider == null)
				return ReturnValue; // x < y (because we want y at the end for non-reverse sorting)

			if (ReferenceEquals(xProvider.Sop, yProvider.Sop))
				return 0;

			return Compare(xProvider.Sop, yProvider.Sop);
		}

		#endregion

		/// <summary>
		/// Compares two <see cref="Sop"/>s.
		/// </summary>
		public abstract int Compare(Sop x, Sop y);
	}
}
