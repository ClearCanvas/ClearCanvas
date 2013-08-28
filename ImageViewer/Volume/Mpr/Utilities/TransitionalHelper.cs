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
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volumes;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	public static class TransitionalHelper
	{
		public static IEnumerable<ISopDataSource> CreateSliceSops(this IEnumerable<VolumeSlice> slices, string seriesInstanceUid)
		{
			return CreateSliceSopsCore(slices, seriesInstanceUid, s => new VolumeSliceSopDataSource(s));
		}

		public static IEnumerable<ISopDataSource> CreateAsyncSliceSops(this IEnumerable<VolumeSlice> slices, string seriesInstanceUid)
		{
			return CreateSliceSopsCore(slices, seriesInstanceUid, s => new AsyncVolumeSliceSopDataSource(s));
		}

		private static IEnumerable<ISopDataSource> CreateSliceSopsCore(IEnumerable<VolumeSlice> slices, string seriesInstanceUid, Func<VolumeSlice, ISopDataSource> sopDataSourceConstructor)
		{
			if (string.IsNullOrWhiteSpace(seriesInstanceUid))
				seriesInstanceUid = DicomUid.GenerateUid().UID;

			var n = 0;
			foreach (var sop in slices.Select(sopDataSourceConstructor))
			{
				sop[DicomTags.SeriesInstanceUid].SetString(0, seriesInstanceUid);
				sop[DicomTags.InstanceNumber].SetInt32(0, ++n);
				yield return sop;
			}
		}
	}
}