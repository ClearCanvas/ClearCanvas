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

// Define this to enable the experimental slab code. You also need to enable the dependent DLLs in
//	ImageViewer_dis.proj (search for Slab)
//#define SLAB

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using ClearCanvas.ImageViewer.Volumes;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// Factory for slices of a <see cref="Volumes.Volume"/>.
	/// </summary>
	/// <remarks>
	/// This factory type has been deprecated in favour of its equivalent in the core volume framework (<see cref="VolumeSliceFactory"/>).
	/// </remarks>
	[Obsolete("Use VolumeSliceFactory instead.")]
	public class VolumeSlicer : IDisposable
	{
		private readonly IVolumeReference _volume;
		private readonly IVolumeSlicerParams _slicerParams;

		private float _sliceSpacing;

		public VolumeSlicer(Volumes.Volume volume, IVolumeSlicerParams slicerParams)
		{
			_volume = volume.CreateReference();
			_slicerParams = slicerParams;
		}

		public VolumeSlicer(IVolumeReference volumeReference, IVolumeSlicerParams slicerParams)
		{
			_volume = volumeReference.Clone();
			_slicerParams = slicerParams;
		}

		public Volumes.Volume Volume
		{
			get { return _volume.Volume; }
		}

		public IVolumeSlicerParams SlicerParams
		{
			get { return _slicerParams; }
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				_volume.Dispose();
			}
		}

		#region Slicing

		private float GetSliceSpacing()
		{
			if (FloatComparer.AreEqual(0, _sliceSpacing))
			{
				_sliceSpacing = _slicerParams.SliceSpacing;
				if (FloatComparer.AreEqual(0, _sliceSpacing))
					_sliceSpacing = GetDefaultSpacing();
			}
			return _sliceSpacing;
		}

		public IEnumerable<ISopDataSource> CreateSliceSops()
		{
			return CreateSliceSops(null);
		}

		public IEnumerable<ISopDataSource> CreateSliceSops(string seriesInstanceUid)
		{
			return CreateSliceSopsCore(seriesInstanceUid, s => new VolumeSliceSopDataSource(s));
		}

		private IEnumerable<ISopDataSource> CreateSliceSopsCore(string seriesInstanceUid, Func<VolumeSlice, ISopDataSource> sopDataSourceConstructor)
		{
			if (string.IsNullOrWhiteSpace(seriesInstanceUid))
				seriesInstanceUid = DicomUid.GenerateUid().UID;

			var n = 0;
			foreach (var sop in CreateSlices().Select(sopDataSourceConstructor))
			{
				sop[DicomTags.SeriesInstanceUid].SetString(0, seriesInstanceUid);
				sop[DicomTags.InstanceNumber].SetInt32(0, ++n);
				yield return sop;
			}
		}

		public IEnumerable<VolumeSlice> CreateSlices()
		{
			// get the slice spacing in voxel units
			var sliceSpacing = GetSliceSpacing();

			// get the normal to the plane of slicing (the plane of slicing is the XY plane in the slicing coordinate frame)
			var slicePlaneNormal = GetSliceNormalVector();

			// get a point through which the slices should pass
			var sliceThroughPoint = GetSliceThroughPoint();

			// compute the number of slices required in this slicing rotation, and the slice location at which to start
			int sliceCount;
			float startingSliceLocation;
			{
				// the algorithm for this involves computing the bounding cube for the volume *in the slicing coordinate frame*
				// the number of required slices is thus the span of the Z components (i.e. slice location) of the bounding cube, divided by the slice spacing
				// this algorithm reduces to determining the projection of each of the 8 corners of the volume onto the normal vector of the slice plane

				var minSliceLocation = float.MaxValue;
				var maxSliceLocation = float.MinValue;

				var volumeDimensions = _volume.VolumeSize;
				foreach (var corner in new[]
				                       	{
				                       		new Vector3D(0, 0, 0),
				                       		new Vector3D(volumeDimensions.X, 0, 0),
				                       		new Vector3D(0, volumeDimensions.Y, 0),
				                       		new Vector3D(0, 0, volumeDimensions.Z),
				                       		new Vector3D(volumeDimensions.X, volumeDimensions.Y, 0),
				                       		new Vector3D(volumeDimensions.X, 0, volumeDimensions.Z),
				                       		new Vector3D(0, volumeDimensions.Y, volumeDimensions.Z),
				                       		new Vector3D(volumeDimensions.X, volumeDimensions.Y, volumeDimensions.Z)
				                       	})
				{
					// project the corner vector onto the slice plane normal vector
					var zCoord = slicePlaneNormal.Dot(corner);
					minSliceLocation = Math.Min(minSliceLocation, zCoord);
					maxSliceLocation = Math.Max(maxSliceLocation, zCoord);
				}

				// divide the span of the slice locations by the slice spacing to determine the required number of slices
				sliceCount = (int) Math.Ceiling((maxSliceLocation - minSliceLocation)/sliceSpacing);

				// the starting slice location could be either end, but we choose the larger end to be consistent with the original implementation
				startingSliceLocation = maxSliceLocation;
			}

			// compute the incremental spacing vector
			var spacingVector = sliceSpacing*slicePlaneNormal;

			// compute the slice location of the specified through point
			var throughPointSliceLocation = slicePlaneNormal.Dot(sliceThroughPoint);

			// compute the through point of the first slice
			// (subtract an extra spacing vector, because we're computing from the larger end of the volume voxels, while VTK draws slices from the smaller end of the voxels.
			var initialThroughPoint = sliceThroughPoint + (startingSliceLocation - throughPointSliceLocation)/sliceSpacing*spacingVector - spacingVector;

			var thicknessAndSpacing = Math.Abs(GetSliceSpacing());

			// generate the slice SOPs by computing additional through points
			for (var n = 0; n < sliceCount; n++)
			{
				var slice = CreateSlice(_volume.Clone(), _slicerParams, thicknessAndSpacing, initialThroughPoint - n*spacingVector);
				yield return slice;
			}
		}

		private Vector3D GetSliceThroughPoint()
		{
			Vector3D throughPoint;
			if (_slicerParams.SliceThroughPointPatient != null)
				throughPoint = _volume.ConvertToVolume(_slicerParams.SliceThroughPointPatient);
			else
				throughPoint = _volume.VolumeCenter;
			return throughPoint;
		}

		/// <summary>
		/// This should be useful for implementing external spacing controls. Actual slice spacing
		/// is tied to actual volume resolution and can be useful in determing spacing values.
		/// E.g. you can use the actual spacing (or multiples of) to establish defaults. You also 
		/// might only allow spacing values that are multiples of the actual spacing.
		/// </summary>
		public float ActualSliceSpacing
		{
			get { return ActualSliceSpacingVector.Magnitude; }
		}

		#region VolumeSlice helper methods

		private static VolumeSlice CreateSlice(IVolumeReference volumeReference, IVolumeSlicerParams slicerParams, float thicknessAndSpacing, Vector3D throughPoint)
		{
			// compute Rows and Columns to reflect actual output size
			var frameSize = GetSliceExtent(volumeReference, slicerParams);

			// compute Pixel Spacing
			var effectiveSpacing = GetEffectiveSpacing(volumeReference);

			// compute Image Orientation (Patient)
			var matrix = new Matrix(slicerParams.SlicingPlaneRotation);
			matrix[3, 0] = throughPoint.X;
			matrix[3, 1] = throughPoint.Y;
			matrix[3, 2] = throughPoint.Z;
			var resliceAxesPatientOrientation = volumeReference.RotateToPatientOrientation(matrix);

			// compute Image Position (Patient)
			var topLeftOfSlicePatient = GetTopLeftOfSlicePatient(frameSize, throughPoint, volumeReference, slicerParams);

			var args = new VolumeSliceArgs(frameSize.Height, frameSize.Width, effectiveSpacing, effectiveSpacing,
			                               new Vector3D(resliceAxesPatientOrientation[0, 0], resliceAxesPatientOrientation[0, 1], resliceAxesPatientOrientation[0, 2]),
			                               new Vector3D(resliceAxesPatientOrientation[1, 0], resliceAxesPatientOrientation[1, 1], resliceAxesPatientOrientation[1, 2]),
			                               thicknessAndSpacing, Convert(slicerParams.InterpolationMode));

			return new VolumeSlice(volumeReference, true, args, topLeftOfSlicePatient, thicknessAndSpacing);
		}

		private static VolumeInterpolationMode Convert(VolumeSlicerInterpolationMode mode)
		{
			switch (mode)
			{
				case VolumeSlicerInterpolationMode.NearestNeighbor:
					return VolumeInterpolationMode.NearestNeighbor;

				case VolumeSlicerInterpolationMode.Linear:
					return VolumeInterpolationMode.Linear;
				case VolumeSlicerInterpolationMode.Cubic:
					return VolumeInterpolationMode.Cubic;
				default:
					throw new ArgumentOutOfRangeException("mode");
			}
		}

		// VTK treats the reslice point as the center of the output image. Given the plane orientation
		//	and size of the output image, we can derive the top left of the output image in patient space
		private static Vector3D GetTopLeftOfSlicePatient(Size frameSize, Vector3D throughPoint, IVolumeHeader volume, IVolumeSlicerParams slicerParams)
		{
			// This is the center of the output image
			var centerImageCoord = new PointF(frameSize.Width/2f, frameSize.Height/2f);

			// These offsets define the x and y vector magnitudes to arrive at our point
			var effectiveSpacing = GetEffectiveSpacing(volume);
			var offsetX = centerImageCoord.X*effectiveSpacing;
			var offsetY = centerImageCoord.Y*effectiveSpacing;

			// To determine top left of slice in volume, subtract offset vectors along x and y
			//
			// Our reslice plane x and y vectors
			var resliceAxes = slicerParams.SlicingPlaneRotation;
			var xVec = new Vector3D(resliceAxes[0, 0], resliceAxes[0, 1], resliceAxes[0, 2]);
			var yVec = new Vector3D(resliceAxes[1, 0], resliceAxes[1, 1], resliceAxes[1, 2]);

			// Offset along x and y from reslicePoint
			var topLeftOfSliceVolume = throughPoint - (offsetX*xVec + offsetY*yVec);

			// Convert volume point to patient space
			return volume.ConvertToPatient(topLeftOfSliceVolume);
		}

		// Derived from either a specified extent in millimeters or from the volume dimensions (default)
		private static Size GetSliceExtent(IVolumeHeader volume, IVolumeSlicerParams slicerParams)
		{
			var effectiveSpacing = GetEffectiveSpacing(volume);
			var longOutputDimension = volume.GetLongAxisMagnitude()/effectiveSpacing;
			var shortOutputDimenstion = volume.GetShortAxisMagnitude()/effectiveSpacing;
			var diagonalDimension = (int) Math.Sqrt(longOutputDimension*longOutputDimension + shortOutputDimenstion*shortOutputDimenstion);

			var columns = diagonalDimension;
			if (!FloatComparer.AreEqual(slicerParams.SliceExtentXMillimeters, 0f))
				columns = (int) (slicerParams.SliceExtentXMillimeters/effectiveSpacing + 0.5f);

			var rows = diagonalDimension;
			if (!FloatComparer.AreEqual(slicerParams.SliceExtentYMillimeters, 0f))
				rows = (int) (slicerParams.SliceExtentYMillimeters/effectiveSpacing + 0.5f);

			return new Size(columns, rows);
		}

		/// <summary>
		/// The effective spacing defines output pixel spacing for slices generated by the VolumeSlicer.
		/// </summary>
		private static float GetEffectiveSpacing(IVolumeHeader volume)
		{
			// Because we supply the real spacing to the VTK reslicer, the slices are interpolated
			//	as if the volume were isotropic. This results in an effective spacing that is the
			//	minimum spacing for the volume.
			//
			// N.B.: this behaviour is different than if had asked VTK to render it, because we are
			//  asking directly for pixel data out. If VTK renders it, then we scrape the rendering
			//  for the pixel data, the spacing would be exactly 1mm because the interpolation would
			//  happened during rendering.
			return volume.GetMinimumSpacing();
		}

		#endregion

		#endregion

		#region Implementation

		#region Slice Spacing

		// This uses the slice plane and volume spacing to arrive at the actual spacing
		//	vector along the orthogonal vector
		private Vector3D ActualSliceSpacingVector
		{
			get
			{
				Vector3D normalVec = GetSliceNormalVector();

				// Normal components by spacing components
				Vector3D actualSliceSpacingVector = new Vector3D(normalVec.X*_volume.VoxelSpacing.X,
				                                                 normalVec.Y*_volume.VoxelSpacing.Y, normalVec.Z*_volume.VoxelSpacing.Z);

				return actualSliceSpacingVector;
			}
		}

		private float GetDefaultSpacing()
		{
			Vector3D spacingVector = ActualSliceSpacingVector;
			{
				// adjust magnitude of vector by whole factor based on max volume spacing
				if (spacingVector.Magnitude < _volume.GetMaximumSpacing()/2f)
				{
					int spacingFactor = (int) (_volume.GetMaximumSpacing()/spacingVector.Magnitude);
					spacingVector *= spacingFactor;
				}
			}
			return spacingVector.Magnitude;
		}

		#endregion

		#region Reslice Matrix helpers

		/// <summary>
		/// Gets the directional unit vector of the normal to the slicing plane.
		/// </summary>
		private Vector3D GetSliceNormalVector()
		{
			var resliceAxes = _slicerParams.SlicingPlaneRotation;
			return new Vector3D(resliceAxes[2, 0], resliceAxes[2, 1], resliceAxes[2, 2]);
		}

		#endregion

		#endregion
	}
}