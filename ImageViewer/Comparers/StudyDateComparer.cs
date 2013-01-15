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
using System.Diagnostics;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Collections.Generic;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="ImageSop"/>s based on study date.
	/// </summary>
	public class StudyDateComparer : DicomStudyComparer, IComparer<IStudyData>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyDateComparer"/>.
		/// </summary>
		/// <remarks>
		/// By default, the <see cref="ComparerBase.Reverse"/> property is set
		/// to true because, normally, we want the image sets sorted with the
		/// most recent studies at the beginning.
		/// </remarks>
		public StudyDateComparer()
			: this(true)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StudyDateComparer"/>.
		/// </summary>
		public StudyDateComparer(bool reverse)
			: base(reverse)
		{
		}

		private IEnumerable<IComparable> GetCompareValues(IImageSet imageSet)
		{
			DateTime? studyDate = null;
			DateTime? studyTime = null;

			IDicomImageSetDescriptor descriptor = imageSet.Descriptor as IDicomImageSetDescriptor;
			if (descriptor == null)
			{
				if (imageSet.DisplaySets.Count == 0)
				{
					Debug.Assert(false, "All image sets being sorted must have at least one display set with at least one image in order for them to be sorted properly.");
				}
				else if (imageSet.DisplaySets[0].PresentationImages.Count == 0)
				{
					Debug.Assert(false, "All image sets being sorted must have at least one display set with at least one image in order for them to be sorted properly.");
				}
				else 
				{
					ISopProvider provider = imageSet.DisplaySets[0].PresentationImages[0] as ISopProvider;
					if (provider != null)
					{
						studyDate = DateParser.Parse(provider.Sop.StudyDate);
						studyTime = TimeParser.Parse(provider.Sop.StudyTime);
					}
				}
			}
			else
			{
				studyDate = DateParser.Parse(descriptor.SourceStudy.StudyDate);
				studyTime = TimeParser.Parse(descriptor.SourceStudy.StudyTime);
			}

			yield return studyDate;
			yield return studyTime;
			yield return imageSet.Name;
		}

		private IEnumerable<IComparable> GetCompareValues(IStudyData studyData)
		{
			yield return DateParser.Parse(studyData.StudyDate);
			yield return TimeParser.Parse(studyData.StudyTime);
			yield return studyData.StudyDescription;
		}

		private IEnumerable<IComparable> GetCompareValues(Sop sop)
		{
			yield return DateParser.Parse(sop.StudyDate);
			yield return TimeParser.Parse(sop.StudyTime);
			yield return sop.StudyDescription;
		}

		/// <summary>
		/// Compares two <see cref="IImageSet"/>s.
		/// </summary>
		public override int Compare(IImageSet x, IImageSet y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s.
		/// </summary>
		/// <remarks>
		/// The relevant DICOM study property to be compared
		/// is taken from the <see cref="ImageSop"/>.
		/// </remarks>
		public override int Compare(Sop x, Sop y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}

		#region IComparer<IStudyData> Members

		/// <summary>
		/// Compares two <see cref="IStudyData"/> objects.
		/// </summary>
		public int Compare(IStudyData x, IStudyData y)
		{
			return base.Compare(GetCompareValues(x), GetCompareValues(y));
		}

		#endregion
	}
}
