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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using ClearCanvas.ImageViewer.Vtk.Rendering;
using vtk;

namespace ClearCanvas.ImageViewer.Vtk.Tests
{
	[Cloneable]
	internal class VtkTestPresentationImage : BasicPresentationImage3D, IImageSopProvider, IVtkPresentationImage
	{
		[CloneIgnore]
		private IFrameReference _frameReference;

		public VtkTestPresentationImage()
			: base(100, 100, 100)
		{
			var dcf = new DicomFile();
			FillDicomDataSet(dcf.MetaInfo, dcf.DataSet);

			using (var dataSource = new TestDataSource(dcf))
			using (var sop = (ImageSop) Sop.Create(dataSource))
			{
				_frameReference = sop.Frames[1].CreateTransientReference();
			}
		}

		private static void FillDicomDataSet(DicomAttributeCollection metainfo, DicomAttributeCollection dataset)
		{
			dataset[DicomTags.PatientId].SetStringValue("PATIENT");
			dataset[DicomTags.PatientsName].SetStringValue("TEST^VTK IMAGE");
			dataset[DicomTags.StudyId].SetStringValue("STUDY");
			dataset[DicomTags.SeriesDescription].SetStringValue("SERIES");
			dataset[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
			dataset[DicomTags.FrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID);
			dataset[DicomTags.PixelSpacing].SetStringValue(@"1\1");
			dataset[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
			dataset[DicomTags.BitsStored].SetInt32(0, 16);
			dataset[DicomTags.BitsAllocated].SetInt32(0, 16);
			dataset[DicomTags.HighBit].SetInt32(0, 15);
			dataset[DicomTags.PixelRepresentation].SetInt32(0, 0);
			dataset[DicomTags.Rows].SetInt32(0, 100);
			dataset[DicomTags.Columns].SetInt32(0, 100);
			dataset[DicomTags.WindowCenter].SetInt32(0, 32768);
			dataset[DicomTags.WindowWidth].SetInt32(0, 65536);
			dataset[DicomTags.WindowCenterWidthExplanation].SetString(0, "Full Window");

			metainfo[DicomTags.SopInstanceUid].SetStringValue(dataset[DicomTags.SopInstanceUid].ToString());
			metainfo[DicomTags.SopClassUid].SetStringValue(dataset[DicomTags.SopClassUid].ToString());
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected VtkTestPresentationImage(VtkTestPresentationImage source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);

			_frameReference = source._frameReference.Clone();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_frameReference != null)
				{
					_frameReference.Dispose();
					_frameReference = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override IAnnotationLayout CreateAnnotationLayout()
		{
			return AnnotationLayoutFactory.CreateLayoutByImageSop(this);
		}

		public VtkSceneGraph CreateSceneGraph()
		{
			return new VtkRootProp(this);
		}

		protected override IRenderer CreateImageRenderer()
		{
			return new VtkPresentationImageRenderer();
		}

		public override IPresentationImage CreateFreshCopy()
		{
			return new VtkTestPresentationImage();
		}

		public override string ToString()
		{
			return Frame.ParentImageSop.InstanceNumber.ToString("d");
		}

		#region IImageSopProvider Members

		/// <summary>
		/// Gets this presentation image's associated <see cref="ImageSop"/>.
		/// </summary>
		/// <remarks>
		/// Use <see cref="ImageSop"/> to access DICOM tags.
		/// </remarks>
		public ImageSop ImageSop
		{
			get { return Frame.ParentImageSop; }
		}

		/// <summary>
		/// Gets this presentation image's associated <see cref="Frame"/>.
		/// </summary>
		public Frame Frame
		{
			get { return _frameReference.Frame; }
		}

		#endregion

		#region ISopProvider Members

		Sop ISopProvider.Sop
		{
			get { return ImageSop; }
		}

		#endregion

		#region VtkRootProp Class

		private class VtkRootProp : BasicVtkSceneGraph<VtkTestPresentationImage>
		{
			public VtkRootProp(VtkTestPresentationImage owner)
				: base(owner) {}

			protected override vtkProp3D CreateModelRootProp()
			{
				var root = AddNewVtkObject<vtkAssembly>("root");
				AddAnnotations(root);
				return root;
			}

			private void AddAnnotations(vtkAssembly vtkAssembly)
			{
				// origin ball
				{
					var sphere = AddNewVtkObject<vtkSphereSource>();
					sphere.SetCenter(0, 0, 0);
					sphere.SetRadius(10);

					var map = AddNewVtkObject<vtkPolyDataMapper>();
					using (var outputPort = sphere.GetOutputPort())
						map.SetInputConnection(outputPort);

					var actor = AddNewVtkObject<vtkActor>();
					actor.SetMapper(map);
					vtkAssembly.AddPart(actor);

					using (var property = actor.GetProperty())
						property.SetColor(1, 1, 0);
				}

				// centroid ball
				{
					var sphere = AddNewVtkObject<vtkEarthSource>();
					sphere.SetRadius(10);
					sphere.SetOnRatio(5);
					sphere.OutlineOff();

					var map = AddNewVtkObject<vtkPolyDataMapper>();
					using (var outputPort = sphere.GetOutputPort())
						map.SetInputConnection(outputPort);

					var actor = AddNewVtkObject<vtkActor>();
					actor.SetMapper(map);
					actor.SetPosition(50, 50, 50);
					vtkAssembly.AddPart(actor);

					using (var property = actor.GetProperty())
					{
						property.SetColor(0, 0.6, 0);
						property.BackfaceCullingOn();
						property.SetInterpolationToPhong();
					}

					var sphere2 = AddNewVtkObject<vtkSphereSource>();
					sphere2.SetRadius(9.7);
					sphere2.SetPhiResolution(360);
					sphere2.SetThetaResolution(360);

					var map2 = AddNewVtkObject<vtkPolyDataMapper>();
					using (var outputPort = sphere2.GetOutputPort())
						map2.SetInputConnection(outputPort);

					var actor2 = AddNewVtkObject<vtkActor>();
					actor2.SetMapper(map2);
					actor2.SetPosition(50, 50, 50);
					vtkAssembly.AddPart(actor2);

					using (var property = actor2.GetProperty())
						property.SetColor(0, 0, 1);
				}

				// +x axis
				{
					var line = AddNewVtkObject<vtkLineSource>();
					line.SetPoint1(0, 0, 0);
					line.SetPoint2(100, 0, 0);

					var map = AddNewVtkObject<vtkPolyDataMapper>();
					using (var outputPort = line.GetOutputPort())
						map.SetInputConnection(outputPort);

					var actor = AddNewVtkObject<vtkActor>();
					actor.SetMapper(map);
					vtkAssembly.AddPart(actor);

					using (var property = actor.GetProperty())
						property.SetColor(0, 1, 0);
				}

				// +y axis
				{
					var line = AddNewVtkObject<vtkLineSource>();
					line.SetPoint1(0, 0, 0);
					line.SetPoint2(0, 100, 0);

					var map = AddNewVtkObject<vtkPolyDataMapper>();
					using (var outputPort = line.GetOutputPort())
						map.SetInputConnection(outputPort);

					var actor = AddNewVtkObject<vtkActor>();
					actor.SetMapper(map);
					vtkAssembly.AddPart(actor);

					using (var property = actor.GetProperty())
						property.SetColor(1, 0, 0);
				}

				// +z axis
				{
					var line = AddNewVtkObject<vtkLineSource>();
					line.SetPoint1(0, 0, 0);
					line.SetPoint2(0, 0, 100);

					var map = AddNewVtkObject<vtkPolyDataMapper>();
					using (var outputPort = line.GetOutputPort())
						map.SetInputConnection(outputPort);

					var actor = AddNewVtkObject<vtkActor>();
					actor.SetMapper(map);
					vtkAssembly.AddPart(actor);

					using (var property = actor.GetProperty())
						property.SetColor(0, 0, 1);
				}
			}
		}

		#endregion
	}
}

#endif