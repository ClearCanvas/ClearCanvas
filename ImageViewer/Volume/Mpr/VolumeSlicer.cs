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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Configuration;
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
				this.Dispose(true);
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
			//TODO (cr Oct 2009): setting outside, let consumer set 'auto' or 'value'

			if (_sliceSpacing == 0f)
			{
				_sliceSpacing = _slicerParams.SliceSpacing;
				if (_sliceSpacing == 0f)
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

		public IEnumerable<ISopDataSource> CreateAsyncSliceSops(string seriesInstanceUid)
		{
			return CreateSliceSopsCore(seriesInstanceUid, s => new AsyncVolumeSliceSopDataSource(s));
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
				var slice = new VolumeSlice(_volume.Clone(), _slicerParams, initialThroughPoint - n*spacingVector);
				slice[DicomTags.SliceThickness].SetFloat32(0, thicknessAndSpacing);
				slice[DicomTags.SpacingBetweenSlices].SetFloat32(0, thicknessAndSpacing);
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
			MprSettings settings = MprSettings.DefaultInstance;

			Vector3D spacingVector = ActualSliceSpacingVector;
			if (settings.AutoSliceSpacing)
			{
				// adjust magnitude of vector by whole factor based on max volume spacing
				if (spacingVector.Magnitude < _volume.GetMaximumSpacing()/2f)
				{
					int spacingFactor = (int) (_volume.GetMaximumSpacing()/spacingVector.Magnitude);
					spacingVector *= spacingFactor;
				}
			}
			else
			{
				// adjust magnitude by value specified by user
				spacingVector *= settings.SliceSpacingFactor;
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