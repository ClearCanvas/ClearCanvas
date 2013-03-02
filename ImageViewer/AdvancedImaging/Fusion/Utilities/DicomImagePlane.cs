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
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Utilities
{
	//TODO (CR Sept 2010): now that we've put this in 3 places, we have a use case for generalizing it.
	//It may be ok as-is but we just need to think about it a bit before putting it in the core viewer code.
	//It may make more sense to just make it part of ImagePlaneHelper.

	using DicomImagePlaneDataCache = Dictionary<string, DicomImagePlane>;

	/// <summary>
	/// An adapter to unify the interface of dicom presentation images
	/// that have valid slice information (e.g. in 3D patient coordinate system).
	/// </summary>
	internal class DicomImagePlane
	{
		#region Private Fields

        //TODO (Phoenix5): #10730 - remove this when it's fixed.
		[ThreadStatic]
		private static DicomImagePlaneDataCache _imagePlaneDataCache;
		[ThreadStatic]
		private static int _referenceCount;

		private Frame _sourceFrame;

		private Vector3D _normal;
		private Vector3D _positionPatientTopLeft;
		private Vector3D _positionPatientTopRight;
		private Vector3D _positionPatientBottomLeft;
		private Vector3D _positionPatientBottomRight;
		private Vector3D _positionPatientCenterOfImage;
		private Vector3D _positionImagePlaneTopLeft;

		#endregion

		private DicomImagePlane()
		{
		}

		private static DicomImagePlaneDataCache ImagePlaneDataCache
		{
			get
			{
				if (_imagePlaneDataCache == null)
					_imagePlaneDataCache = new DicomImagePlaneDataCache();
				return _imagePlaneDataCache;
			}
		}

		public static void InitializeCache()
		{
			++_referenceCount;
		}

		public static void ReleaseCache()
		{
			if (_referenceCount > 0)
				--_referenceCount;

			if (_referenceCount == 0)
				ImagePlaneDataCache.Clear();
		}

		#region Factory Method

		public static DicomImagePlane FromImage(IPresentationImage sourceImage)
		{
			if (sourceImage == null)
				return null;

			Frame frame = GetFrame(sourceImage);
			return FromFrame(frame);
		}

		public static DicomImagePlane FromFrame(Frame frame)
		{
			if (frame == null)
				return null;

			if (String.IsNullOrEmpty(frame.FrameOfReferenceUid) || String.IsNullOrEmpty(frame.ParentImageSop.StudyInstanceUid))
				return null;

			DicomImagePlane plane;
			if (_referenceCount > 0)
				plane = CreateFromCache(frame);
			else
				plane = CreateFromFrame(frame);

			if (plane != null)
			{
				plane._sourceFrame = frame;
			}

			return plane;
		}

		#endregion

		#region Private Methods

		private static SpatialTransform GetSpatialTransform(IPresentationImage image)
		{
			if (image is ISpatialTransformProvider)
				return ((ISpatialTransformProvider)image).SpatialTransform as SpatialTransform;

			return null;
		}

		private static Frame GetFrame(IPresentationImage image)
		{
			if (image is IImageSopProvider)
				return ((IImageSopProvider)image).Frame;

			return null;
		}

		private static DicomImagePlane CreateFromCache(Frame frame)
		{
			string key = String.Format("{0}:{1}", frame.ParentImageSop.SopInstanceUid, frame.FrameNumber);

			DicomImagePlane cachedData;
			if (ImagePlaneDataCache.ContainsKey(key))
			{
				cachedData = ImagePlaneDataCache[key];
			}
			else
			{
				cachedData = CreateFromFrame(frame);
				if (cachedData != null)
					ImagePlaneDataCache[key] = cachedData;
			}

			if (cachedData != null)
			{
				DicomImagePlane plane = new DicomImagePlane();
				plane.InitializeWithCachedData(cachedData);
				return plane;
			}

			return null;
		}

		private static DicomImagePlane CreateFromFrame(Frame frame)
		{
			int height = frame.Rows - 1;
			int width = frame.Columns - 1;

			DicomImagePlane plane = new DicomImagePlane();
			plane.PositionPatientTopLeft = frame.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
			plane.PositionPatientTopRight = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width, 0));
			plane.PositionPatientBottomLeft = frame.ImagePlaneHelper.ConvertToPatient(new PointF(0, height));
			plane.PositionPatientBottomRight = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width, height));
			plane.PositionPatientCenterOfImage = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width / 2F, height / 2F));

			plane.Normal = frame.ImagePlaneHelper.GetNormalVector();

			if (plane.Normal == null || plane.PositionPatientCenterOfImage == null)
				return null;

			// here, we want the position in the coordinate system of the image plane, 
			// without moving the origin (e.g. leave it at the patient origin).
			plane.PositionImagePlaneTopLeft = frame.ImagePlaneHelper.ConvertToImagePlane(plane.PositionPatientTopLeft, Vector3D.Null);

			return plane;
		}

		private void InitializeWithCachedData(DicomImagePlane cachedData)
		{
			Normal = cachedData.Normal;
			PositionPatientTopLeft = cachedData.PositionPatientTopLeft;
			PositionPatientTopRight = cachedData.PositionPatientTopRight;
			PositionPatientBottomLeft = cachedData.PositionPatientBottomLeft;
			PositionPatientBottomRight = cachedData.PositionPatientBottomRight;
			PositionPatientCenterOfImage = cachedData.PositionPatientCenterOfImage;
			PositionImagePlaneTopLeft = cachedData.PositionImagePlaneTopLeft;
		}

		#endregion

		#region Public Properties

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
			get { return (float)_sourceFrame.SliceThickness; }
		}

		public float Spacing
		{
			get { return (float)_sourceFrame.SpacingBetweenSlices; }
		}

		public Vector3D Normal
		{
			get { return _normal; }
			private set { _normal = value; }
		}

		public Vector3D PositionPatientTopLeft
		{
			get { return _positionPatientTopLeft; }
			private set { _positionPatientTopLeft = value; }
		}

		public Vector3D PositionPatientTopRight
		{
			get { return _positionPatientTopRight; }
			private set { _positionPatientTopRight = value; }
		}

		public Vector3D PositionPatientBottomLeft
		{
			get { return _positionPatientBottomLeft; }
			private set { _positionPatientBottomLeft = value; }
		}

		public Vector3D PositionPatientBottomRight
		{
			get { return _positionPatientBottomRight; }
			private set { _positionPatientBottomRight = value; }
		}

		public Vector3D PositionPatientCenterOfImage
		{
			get { return _positionPatientCenterOfImage; }
			private set { _positionPatientCenterOfImage = value; }
		}

		public Vector3D PositionImagePlaneTopLeft
		{
			get { return _positionImagePlaneTopLeft; }
			private set { _positionImagePlaneTopLeft = value; }
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
			return (PointF)_sourceFrame.ImagePlaneHelper.ConvertToImage(positionMillimetres);
		}

		public bool IsInSameFrameOfReference(DicomImagePlane other)
		{
			Frame otherFrame = other._sourceFrame;

			if (_sourceFrame.ParentImageSop.StudyInstanceUid != otherFrame.ParentImageSop.StudyInstanceUid)
				return false;

			return this._sourceFrame.FrameOfReferenceUid == otherFrame.FrameOfReferenceUid;
		}

		public bool IsParallelTo(DicomImagePlane other, float angleTolerance)
		{
			return Normal.IsParallelTo(other.Normal, angleTolerance);
		}

		public bool IsOrthogonalTo(DicomImagePlane other, float angleTolerance)
		{
			return Normal.IsOrthogonalTo(other.Normal, angleTolerance);
		}

		public float GetAngleBetween(DicomImagePlane other)
		{
			return Normal.GetAngleBetween(other.Normal);
		}

		public bool GetIntersectionPoints(DicomImagePlane other, out Vector3D intersectionPointPatient1, out Vector3D intersectionPointPatient2)
		{
			intersectionPointPatient1 = intersectionPointPatient2 = null;

			Vector3D[,] lineSegmentsImagePlaneBounds = new Vector3D[,]
				{
					// Bounding line segments of this (reference) image plane.
					{ PositionPatientTopLeft, PositionPatientTopRight },
					{ PositionPatientTopLeft, PositionPatientBottomLeft },
					{ PositionPatientBottomRight, PositionPatientTopRight  },
					{ PositionPatientBottomRight, PositionPatientBottomLeft}
				};

			List<Vector3D> planeIntersectionPoints = new List<Vector3D>();

			for (int i = 0; i < 4; ++i)
			{
				// Intersect the bounding line segments of the reference image with the plane of the target image.
				Vector3D intersectionPoint = Vector3D.GetLinePlaneIntersection(other.Normal, other.PositionPatientCenterOfImage,
																		lineSegmentsImagePlaneBounds[i, 0],
																		lineSegmentsImagePlaneBounds[i, 1], true);
				if (intersectionPoint != null)
					planeIntersectionPoints.Add(intersectionPoint);
			}

			if (planeIntersectionPoints.Count < 2)
				return false;

			intersectionPointPatient1 = planeIntersectionPoints[0];
			intersectionPointPatient2 = CollectionUtils.SelectFirst(planeIntersectionPoints,
			                                                        delegate(Vector3D point) { return !planeIntersectionPoints[0].Equals(point); });

			return intersectionPointPatient1 != null && intersectionPointPatient2 != null;
		}

		#endregion
	}
}
