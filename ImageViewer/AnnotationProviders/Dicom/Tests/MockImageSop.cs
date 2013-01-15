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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.Tests
{
	internal class MockImageSop : ImageSop
	{
		private ISopDataSource _sopDataSource;

		public MockImageSop()
			: this(CreateSopDataSource()) {}

		private MockImageSop(ISopDataSource sopDataSource)
			: base(sopDataSource)
		{
			_sopDataSource = sopDataSource;
		}

		public override int NumberOfFrames
		{
			get { return 1; }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_sopDataSource != null)
				{
					_sopDataSource.Dispose();
					_sopDataSource = null;
				}
			}
			base.Dispose(disposing);
		}

		private static ISopDataSource CreateSopDataSource()
		{
			var uid = DicomUid.GenerateUid().UID;
			var dcf = new DicomFile();
			dcf.MediaStorageSopInstanceUid = uid;
			dcf.MediaStorageSopClassUid = DicomUids.SecondaryCaptureImageStorage.UID;
			dcf.DataSet[DicomTags.SopInstanceUid].SetStringValue(uid);
			dcf.DataSet[DicomTags.SopClassUid].SetStringValue(DicomUids.SecondaryCaptureImageStorage.UID);
			return new TestDataSource(dcf);
		}
	}
}

#endif