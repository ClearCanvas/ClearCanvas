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
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	/// <summary>
	/// An adapter to unify the interface of dicom presentation images
	/// that have valid slice information (e.g. in 3D patient coordinate system).
	/// </summary>
	internal sealed class DicomImagePlane
	{
		#region Private Fields

		private readonly IPresentationImage _sourceImage;
		private readonly ISpatialTransform _sourceImageTransform;
		private readonly Frame _sourceFrame;

		#endregion

		private DicomImagePlane(IPresentationImage sourceImage, ISpatialTransform sourceImageTransform, Frame sourceFrame)
		{
			_sourceImage = sourceImage;
			_sourceImageTransform = sourceImageTransform;
			_sourceFrame = sourceFrame;
		}

		#region Factory Method

		public static DicomImagePlane FromImage(IPresentationImage sourceImage)
		{
			if (sourceImage == null)
				return null;

			Frame frame = GetFrame(sourceImage);
			ISpatialTransform transform = GetSpatialTransform(sourceImage);

			if (transform == null || frame == null)
				return null;

			if (String.IsNullOrEmpty(frame.FrameOfReferenceUid) || String.IsNullOrEmpty(frame.ParentImageSop.StudyInstanceUid))
				return null;

			if (!frame.ImagePlaneHelper.IsValid)
				return null;

			return new DicomImagePlane(sourceImage, transform, frame);
		}

		#endregion

		#region Private Methods

		private static ISpatialTransform GetSpatialTransform(IPresentationImage image)
		{
			return image is ISpatialTransformProvider ? ((ISpatialTransformProvider) image).SpatialTransform : null;
		}

		private static Frame GetFrame(IPresentationImage image)
		{
			return image is IImageSopProvider ? ((IImageSopProvider) image).Frame : null;
		}

		#endregion

		#region Public Properties

		public IPresentationImage SourceImage
		{
			get { return _sourceImage; }
		}

		public ISpatialTransform SourceImageTransform
		{
			get { return _sourceImageTransform; }
		}

		public string StudyInstanceUid
		{
			get { return _sourceFrame.ParentImageSop.StudyInstanceUid; }
		}

		public string SeriesInstanceUid
		{
			get { return _sourceFrame.ParentImageSop.SeriesInstanceUid; }
		}

		public string SopInstanceUid
		{
			get { return _sourceFrame.ParentImageSop.SopInstanceUid; }
		}

		public int InstanceNumber
		{
			get { return _sourceFrame.ParentImageSop.InstanceNumber; }
		}

		public int FrameNumber
		{
			get { return _sourceFrame.FrameNumber; }
		}

		public string FrameOfReferenceUid
		{
			get { return _sourceFrame.FrameOfReferenceUid; }
		}

		public float Thickness
		{
			get { return (float) _sourceFrame.SliceThickness; }
		}

		public float Spacing
		{
			get { return (float) _sourceFrame.SpacingBetweenSlices; }
		}

		public Vector3D Normal
		{
			get { return _sourceFrame.ImagePlaneHelper.ImageNormalPatient; }
		}

		public Vector3D PositionPatientTopLeft
		{
			get { return _sourceFrame.ImagePlaneHelper.ImageTopLeftPatient; }
		}

		public Vector3D PositionPatientTopRight
		{
			get { return _sourceFrame.ImagePlaneHelper.ImageTopRightPatient; }
		}

		public Vector3D PositionPatientBottomLeft
		{
			get { return _sourceFrame.ImagePlaneHelper.ImageBottomLeftPatient; }
		}

		public Vector3D PositionPatientBottomRight
		{
			get { return _sourceFrame.ImagePlaneHelper.ImageBottomRightPatient; }
		}

		public Vector3D PositionPatientCenterOfImage
		{
			get { return _sourceFrame.ImagePlaneHelper.ImageCenterPatient; }
		}

		public Vector3D PositionImagePlaneTopLeft
		{
			get { return _sourceFrame.ImagePlaneHelper.TopLeftImagePlane; }
		}

		#endregion

		#region Public Methods

		public Vector3D ConvertToPatient(PointF imagePoint)
		{
			return _sourceFrame.ImagePlaneHelper.ConvertToPatient(imagePoint);
		}

		public Vector3D ConvertToImagePlane(Vector3D positionPatient)
		{
			return _sourceFrame.ImagePlaneHelper.ConvertToImagePlane(positionPatient);
		}

		public Vector3D ConvertToImagePlane(Vector3D positionPatient, Vector3D originPatient)
		{
			return _sourceFrame.ImagePlaneHelper.ConvertToImagePlane(positionPatient, originPatient);
		}

		public PointF ConvertToImage(PointF positionMillimetres)
		{
			return _sourceFrame.ImagePlaneHelper.ConvertToImage2(positionMillimetres);
		}

		public bool IsInSameFrameOfReference(DicomImagePlane other)
		{
			Frame otherFrame = other._sourceFrame;

			if (_sourceFrame.ParentImageSop.StudyInstanceUid != otherFrame.ParentImageSop.StudyInstanceUid)
				return false;

			return _sourceFrame.FrameOfReferenceUid == otherFrame.FrameOfReferenceUid;
		}

		public bool IsParallelTo(DicomImagePlane other, float angleTolerance)
		{
			return _sourceFrame.ImagePlaneHelper.IsParallelTo(other._sourceFrame.ImagePlaneHelper, angleTolerance);
		}

		public bool IsOrthogonalTo(DicomImagePlane other, float angleTolerance)
		{
			return _sourceFrame.ImagePlaneHelper.IsOrthogonalTo(other._sourceFrame.ImagePlaneHelper, angleTolerance);
		}

		public float GetAngleBetween(DicomImagePlane other)
		{
			return _sourceFrame.ImagePlaneHelper.GetAngleBetween(other._sourceFrame.ImagePlaneHelper);
		}

		public bool GetIntersectionPoints(DicomImagePlane other, out Vector3D intersectionPointPatient1, out Vector3D intersectionPointPatient2)
		{
			return _sourceFrame.ImagePlaneHelper.IntersectsWith(other._sourceFrame.ImagePlaneHelper, out intersectionPointPatient1, out intersectionPointPatient2);
		}

		#endregion
	}
}