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

using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="ImageSop"/>s based on series number.
	/// </summary>
	public class SeriesNumberComparer : DicomSeriesComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SeriesNumberComparer"/>.
		/// </summary>
		public SeriesNumberComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="SeriesNumberComparer"/>.
		/// </summary>
		public SeriesNumberComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s based on series number.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override int Compare(Sop x, Sop y)
		{
			int seriesNumber1 = x.SeriesNumber;
			int seriesNumber2 = y.SeriesNumber;

			if (seriesNumber1 < seriesNumber2)
				return this.ReturnValue;
			else if (seriesNumber1 > seriesNumber2)
				return -this.ReturnValue;
			else
				return 0;
		}

		#endregion
	}
}
