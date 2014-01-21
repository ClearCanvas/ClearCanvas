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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="Frame"/>s based on instance number and frame number.
	/// </summary>
	public class InstanceAndFrameNumberComparer : DicomFrameComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="InstanceAndFrameNumberComparer"/>.
		/// </summary>
		public InstanceAndFrameNumberComparer() {}

		/// <summary>
		/// Initializes a new instance of <see cref="InstanceAndFrameNumberComparer"/>.
		/// </summary>
		public InstanceAndFrameNumberComparer(bool reverse)
			: base(reverse) {}

		private static IEnumerable<IComparable> GetCompareValues(Frame frame)
		{
			//Group by common study level attributes
			yield return frame.StudyInstanceUid;

			//Group by common series level attributes
			//This sorts "FOR PRESENTATION" images to the beginning (except in reverse, of course).
			yield return frame.ParentImageSop.PresentationIntentType == "FOR PRESENTATION" ? 0 : 1;
			yield return frame.ParentImageSop.SeriesNumber;
			yield return frame.ParentImageSop.SeriesDescription;
			yield return frame.SeriesInstanceUid;

			yield return frame.ParentImageSop.InstanceNumber;
			yield return frame.FrameNumber;
			//as a last resort.
			yield return frame.AcquisitionNumber;
		}

		/// <summary>
		/// Compares two <see cref="Frame"/>s based on instance number and frame number.
		/// </summary>
		public override int Compare(Frame x, Frame y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}
}