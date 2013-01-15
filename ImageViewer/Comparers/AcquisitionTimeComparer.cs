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
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="Frame"/>s based on acquisition date and time.
	/// </summary>
	public class AcquisitionTimeComparer : DicomFrameComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="AcquisitionTimeComparer"/>.
		/// </summary>
		public AcquisitionTimeComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="AcquisitionTimeComparer"/>.
		/// </summary>
		public AcquisitionTimeComparer(bool reverse)
			: base(reverse)
		{
		}

		private static IEnumerable<IComparable> GetCompareValues(Frame frame)
		{
			//Group be common study level attributes
            yield return frame.StudyInstanceUid;

            //Group by common series level attributes
            //This sorts "FOR PRESENTATION" images to the beginning (except in reverse, of course).
            yield return frame.ParentImageSop.PresentationIntentType == "FOR PRESENTATION" ? 0 : 1;
            yield return frame.ParentImageSop.SeriesNumber;
            yield return frame.ParentImageSop.SeriesDescription;
            yield return frame.SeriesInstanceUid;

			DateTime? datePart = null;
			TimeSpan? timePart = null;

			//then sort by acquisition datetime.
			DateTime? acquisitionDateTime = DateTimeParser.Parse(frame.AcquisitionDateTime);
			if (acquisitionDateTime != null)
			{
				datePart = acquisitionDateTime.Value.Date;
				timePart = acquisitionDateTime.Value.TimeOfDay;
			}
			else 
			{
				datePart = DateParser.Parse(frame.AcquisitionDate);
				if (datePart != null)
				{
					//only set the time part if there is a valid date part.
					DateTime? acquisitionTime = TimeParser.Parse(frame.AcquisitionTime);
					if (acquisitionTime != null)
						timePart = acquisitionTime.Value.TimeOfDay;
				}
			}

			yield return datePart;
			yield return timePart;

			//as a last resort.
			yield return frame.ParentImageSop.InstanceNumber;
			yield return frame.FrameNumber;
			yield return frame.AcquisitionNumber;
		}

		/// <summary>
		/// Compares two <see cref="Frame"/>s based on acquisition date and time.
		/// </summary>
		public override int Compare(Frame x, Frame y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}
}
