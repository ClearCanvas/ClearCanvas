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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparers that compare some aspect of
	/// DICOM series.
	/// </summary>
	public abstract class DicomSeriesComparer : DisplaySetComparer, IComparer<Series>, IComparer<Sop>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomSeriesComparer"/>.
		/// </summary>
		protected DicomSeriesComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomSeriesComparer"/>.
		/// </summary>
		protected DicomSeriesComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="IDisplaySet"/>s.
		/// </summary>
		public override int Compare(IDisplaySet x, IDisplaySet y)
		{
			if (x.PresentationImages.Count == 0 || y.PresentationImages.Count == 0)
				return 0;

			IImageSopProvider provider1 = x.PresentationImages[0] as IImageSopProvider;
			IImageSopProvider provider2 = y.PresentationImages[0] as IImageSopProvider;

			if (provider1 == null)
			{
				if (provider2 == null)
					return 0; // x == y
				else
					return -ReturnValue; // x > y (because we want it at the end for non-reverse sorting)
			}
			else
			{
				if (provider2 == null)
					return ReturnValue; // x < y (because we want it at the end for non-reverse sorting)
			}

			return Compare(provider1.ImageSop, provider2.ImageSop);
		}

		#endregion

		/// <summary>
		/// Compares two <see cref="Series"/>.
		/// </summary>
		/// <remarks>Simply calls <see cref="Compare(ClearCanvas.ImageViewer.StudyManagement.Sop,ClearCanvas.ImageViewer.StudyManagement.Sop)"/>,
		/// passing the first <see cref="Sop"/> in each <see cref="Series"/>.</remarks>
		public int Compare(Series x, Series y)
		{
			if (x.Sops.Count == 0 || y.Sops.Count == 0)
				return 0;

			return Compare(x.Sops[0], y.Sops[0]);
		}

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s.
		/// </summary>
		/// <remarks>
		/// The relevant DICOM series property to be compared
		/// is taken from the <see cref="ImageSop"/>.
		/// </remarks>
		public abstract int Compare(Sop x, Sop y);
	}
}
