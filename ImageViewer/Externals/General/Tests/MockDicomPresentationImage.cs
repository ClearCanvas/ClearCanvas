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

using System;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Externals.General.Tests
{
	public class MockDicomPresentationImage : BasicPresentationImage, IDicomPresentationImage
	{
		private DicomFile _dicomFile;
		private LocalSopDataSource _sopDataSource;
		private ImageSop _imageSop;
		private Frame _frame;
		private string _filename;

		public MockDicomPresentationImage() : this(string.Format("{0}.dcm", Guid.NewGuid())) {}

		public MockDicomPresentationImage(string filename) : base(new GrayscaleImageGraphic(10, 10))
		{
			if (Path.IsPathRooted(filename))
				_filename = filename;
			else
				_filename = Path.Combine(Environment.CurrentDirectory, filename);

			_dicomFile = new DicomFile();
			_dicomFile.DataSet[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
			_dicomFile.DataSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			_dicomFile.MetaInfo[DicomTags.MediaStorageSopClassUid].SetStringValue(_dicomFile.DataSet[DicomTags.SopClassUid].ToString());
			_dicomFile.MetaInfo[DicomTags.MediaStorageSopInstanceUid].SetStringValue(_dicomFile.DataSet[DicomTags.SopInstanceUid].ToString());
			_dicomFile.Save(_filename);
			_sopDataSource = new LocalSopDataSource(_dicomFile);
			_imageSop = new ImageSop(_sopDataSource);
			_frame = new MockFrame(_imageSop, 1);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_frame = null;
				_dicomFile = null;

				if (_imageSop != null)
				{
					_imageSop.Dispose();
					_imageSop = null;
				}

				if (_sopDataSource != null)
				{
					_sopDataSource.Dispose();
					_sopDataSource = null;
				}

				if (_filename != null)
				{
					if (File.Exists(_filename))
						File.Delete(_filename);
					_filename = null;
				}
			}
			base.Dispose(disposing);
		}

		public string Filename
		{
			get { return _filename; }
		}

		public DicomAttribute this[uint dicomTag]
		{
			get { return _dicomFile.DataSet[dicomTag]; }
		}

		public override IPresentationImage CreateFreshCopy()
		{
			throw new NotImplementedException();
		}

		#region IDicomPresentationImage Members

		GraphicCollection IDicomPresentationImage.DicomGraphics
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IImageSopProvider Members

		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		public Frame Frame
		{
			get { return _frame; }
		}

		#endregion

		#region ISopProvider Members

		public Sop Sop
		{
			get { return _imageSop; }
		}

		#endregion

		#region IPatientPresentationProvider Members (Not Implemented)

		public IPatientPresentation PatientPresentation
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IPatientCoordinateMappingProvider Members (Not Implemented)

		public IPatientCoordinateMapping PatientCoordinateMapping
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		private class MockFrame : Frame
		{
			public MockFrame(ImageSop parent, int number) : base(parent, number) {}
		}
	}
}

#endif