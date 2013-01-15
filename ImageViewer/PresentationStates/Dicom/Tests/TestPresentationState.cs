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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom.Tests
{
	internal class TestPresentationState : DicomSoftcopyPresentationStateBase<TestPresentationImage>
	{
		public TestPresentationState() : base(SopClass.RawDataStorage) {}

		public SpatialTransformModuleIod SerializeSpatialTransform(TestPresentationImage image)
		{
			SpatialTransformModuleIod dataset = new SpatialTransformModuleIod();
			base.SerializeSpatialTransform(dataset, CreateImageCollection(image));
			return dataset;
		}

		public TestPresentationImage DeserializeSpatialTransform(SpatialTransformModuleIod dataset)
		{
			TestPresentationImage image = new TestPresentationImage();
			base.DeserializeSpatialTransform(dataset, image);
			return image;
		}

		protected override void PerformTypeSpecificSerialization(DicomPresentationImageCollection<TestPresentationImage> images)
		{
			throw new NotImplementedException();
		}

		protected override void PerformTypeSpecificDeserialization(DicomPresentationImageCollection<TestPresentationImage> images)
		{
			throw new NotImplementedException();
		}

		private static DicomPresentationImageCollection<TestPresentationImage> CreateImageCollection(params TestPresentationImage[] images)
		{
			return new DicomPresentationImageCollection<TestPresentationImage>(images);
		}
	}
}

#endif