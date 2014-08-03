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

#if UNIT_TESTS

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
	public class TestDataSource : DicomMessageSopDataSource
	{
		private DicomFile _file;

		public TestDataSource(DicomFile file)
			: base(file)
		{
			_file = file;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_file = null;
		}

		public bool IsDisposed
		{
			get { return _file == null; }
		}

		public DicomFile File
		{
			get { return _file; }
		}

		public override DicomAttribute this[DicomTag tag]
		{
			get
			{
				DicomAttribute attribute;
				return _file.MetaInfo.TryGetAttribute(tag, out attribute) ? attribute : _file.DataSet[tag];
			}
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				DicomAttribute attribute;
				return _file.MetaInfo.TryGetAttribute(tag, out attribute) ? attribute : _file.DataSet[tag];
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			return _file.MetaInfo.TryGetAttribute(tag, out attribute) || _file.DataSet.TryGetAttribute(tag, out attribute);
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			return _file.MetaInfo.TryGetAttribute(tag, out attribute) || _file.DataSet.TryGetAttribute(tag, out attribute);
		}

		public static TestDataSource CreateImageSopDataSource(int? numberOfFrames = null, string sopClassUid = null)
		{
			var dcf = new DicomFile();
			dcf.DataSet[DicomTags.SopClassUid].SetStringValue(dcf.MediaStorageSopClassUid = (!string.IsNullOrEmpty(sopClassUid) ? sopClassUid : SopClass.SecondaryCaptureImageStorageUid));
			dcf.DataSet[DicomTags.SopInstanceUid].SetStringValue(dcf.MediaStorageSopInstanceUid = DicomUid.GenerateUid().UID);
			dcf.DataSet[DicomTags.Rows].SetInt32(0, 512);
			dcf.DataSet[DicomTags.Columns].SetInt32(0, 512);
			dcf.DataSet[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
			dcf.DataSet[DicomTags.PixelRepresentation].SetInt32(0, 0);
			dcf.DataSet[DicomTags.BitsAllocated].SetInt32(0, 16);
			dcf.DataSet[DicomTags.BitsStored].SetInt32(0, 16);
			dcf.DataSet[DicomTags.HighBit].SetInt32(0, 15);
			dcf.DataSet[DicomTags.PixelData].SetNullValue();
			if (numberOfFrames.HasValue) dcf.DataSet[DicomTags.NumberOfFrames].SetInt32(0, numberOfFrames.Value);
			return new TestDataSource(dcf);
		}
	}
}

#endif