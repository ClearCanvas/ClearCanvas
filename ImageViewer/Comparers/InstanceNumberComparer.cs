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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="Sop"/>s based on Instance Number.
	/// </summary>
	public class InstanceNumberComparer : DicomSopComparer
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public InstanceNumberComparer()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public InstanceNumberComparer(bool reverse)
			: base(reverse)
		{
		}

		private static IEnumerable<IComparable> GetCompareValues(Sop sop)
		{
            //Group be common study level attributes
            yield return sop.StudyInstanceUid;

            //Group by common series level attributes
            //This sorts "FOR PRESENTATION" images to the beginning (except in reverse, of course).
            if (!sop.IsImage)
                yield return 1;
            else
                yield return ((ImageSop)sop).PresentationIntentType == "FOR PRESENTATION" ? 0 : 1;

            yield return sop.SeriesNumber;
            yield return sop.SeriesDescription;
            yield return sop.SeriesInstanceUid;

			yield return sop.InstanceNumber;
			yield return sop[DicomTags.AcquisitionNumber].GetInt32(0, 0);
		}

		/// <summary>
		/// Compares 2 <see cref="Sop"/>s based on Instance Number.
		/// </summary>
		public override int Compare(Sop x, Sop y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}
}