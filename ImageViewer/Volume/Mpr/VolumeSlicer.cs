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
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.VTK;
using ClearCanvas.ImageViewer.Volume.Mpr.Configuration;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using ClearCanvas.ImageViewer.Volumes;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// Factory for slices of a <see cref="Volumes.Volume"/>.
	/// </summary>
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

		#region Pixel Data Generation

		// This method is used by the VolumeSlice to generate pixel data on demand
		internal byte[] CreateSliceNormalizedPixelData(Vector3D throughPoint)
		{
			Matrix resliceAxes = new Matrix(_slicerParams.SlicingPlaneRotation);
			resliceAxes[3, 0] = throughPoint.X;
			resliceAxes[3, 1] = throughPoint.Y;
			resliceAxes[3, 2] = throughPoint.Z;

#if SLAB
			using (vtkImageData imageData = GenerateVtkSlab(resliceAxes, 10))  // baked 10 voxels for testing
			{
				byte[] pixelData = MipPixelDataFromVtkSlab(imageData);
				imageData.ReleaseData();

				return pixelData;
			}
#else
			using (vtkImageData imageData = GenerateVtkSlice(resliceAxes))
			{
				byte[] pixelData = CreatePixelDataFromVtkSlice(imageData);
				imageData.ReleaseData();

				return pixelData;
			}
#endif
		}

		// Extract slice in specified orientation
		private vtkImageData GenerateVtkSlice(Matrix resliceAxes)
		{
			using (vtkImageReslice reslicer = new vtkImageReslice())
			{
				VtkHelper.RegisterVtkErrorEvents(reslicer);

				// Obtain a pinned VTK volume for the reslicer. We'll release this when
				//	VTK is done reslicing.
				using (var volume = _volume.Volume.CreateVtkVolumeHandle())
				{
					reslicer.SetInput(volume.vtkImageData);
					reslicer.SetInformationInput(volume.vtkImageData);

					// Must instruct reslicer to output 2D images
					reslicer.SetOutputDimensionality(2);

					// Use the volume's padding value for all pixels that are outside the volume
					reslicer.SetBackgroundLevel(_volume.PaddingValue);

					// This effectively ensures that the image reslicer uses the original spacing in the output images
					reslicer.SetOutputSpacing(_volume.VoxelSpacing.X, _volume.VoxelSpacing.Y, _volume.VoxelSpacing.Z);

					using (vtkMatrix4x4 resliceAxesMatrix = VtkHelper.ConvertToVtkMatrix(resliceAxes))
					{
						reslicer.SetResliceAxes(resliceAxesMatrix);

						// Clamp the output based on the slice extent
						int sliceExtentX = GetSliceExtentX();
						int sliceExtentY = GetSliceExtentY();
						reslicer.SetOutputExtent(0, sliceExtentX - 1, 0, sliceExtentY - 1, 0, 0);

						// Set the output origin to reflect the slice through point. The slice extent is
						//	centered on the slice through point.
						// VTK output origin is derived from the center image being 0,0
						float originX = -sliceExtentX*EffectiveSpacing/2;
						float originY = -sliceExtentY*EffectiveSpacing/2;
						reslicer.SetOutputOrigin(originX, originY, 0);

						switch (_slicerParams.InterpolationMode)
						{
							case VolumeSlicerInterpolationMode.NearestNeighbor:
								reslicer.SetInterpolationModeToNearestNeighbor();
								break;
							case VolumeSlicerInterpolationMode.Linear:
								reslicer.SetInterpolationModeToLinear();
								break;
							case VolumeSlicerInterpolationMode.Cubic:
								reslicer.SetInterpolationModeToCubic();
								break;
						}

						using (vtkExecutive exec = reslicer.GetExecutive())
						{
							VtkHelper.RegisterVtkErrorEvents(exec);
							exec.Update();
						}

						var output = reslicer.GetOutput();
						//Just in case VTK uses the matrix internally.
						return output;
					}
				}
			}
		}

		private static byte[] CreatePixelDataFromVtkSlice(vtkImageData sliceImageData)
		{
			int[] sliceDimensions = sliceImageData.GetDimensions();
			int sliceDataSize = sliceDimensions[0]*sliceDimensions[1];
			IntPtr sliceDataPtr = sliceImageData.GetScalarPointer();

			if (sliceDataPtr.Equals(IntPtr.Zero)) return null;

			byte[] pixelData = MemoryManager.Allocate<byte>(sliceDataSize*sizeof (short));

			Marshal.Copy(sliceDataPtr, pixelData, 0, sliceDataSize*sizeof (short));
			return pixelData;
		}

		#region Slabbing Code

#if SLAB

	// Extract slab in specified orientation, if slabThickness is 1, this is identical
	//	to GenerateVtkSlice above, so they should be collapsed at some point.
	// TODO: Tie into Dicom for slice, will need to adjust thickness at least
		private vtkImageData GenerateVtkSlab(Matrix resliceAxes, int slabThicknessInVoxels)
		{
            VtkHelper.StaticInitializationHack();

			// Thickness should be at least 1
			if (slabThicknessInVoxels < 1)
				slabThicknessInVoxels = 1;

			using (vtkImageReslice reslicer = new vtkImageReslice())
			{
				VtkHelper.RegisterVtkErrorEvents(reslicer);

				// Obtain a pinned VTK volume for the reslicer. We'll release this when
				//	VTK is done reslicing.
				vtkImageData volumeVtkWrapper = _volume.ObtainPinnedVtkVolume();
				reslicer.SetInput(volumeVtkWrapper);
				reslicer.SetInformationInput(volumeVtkWrapper);

				if (slabThicknessInVoxels > 1)
					reslicer.SetOutputDimensionality(3);
				else
					reslicer.SetOutputDimensionality(3);

				// Use the volume's padding value for all pixels that are outside the volume
				reslicer.SetBackgroundLevel(_volume.PadValue);

				// This ensures VTK obeys the real spacing, results in all VTK slices being isotropic.
				//	Effective spacing is the minimum of these three.
				reslicer.SetOutputSpacing(_volume.Spacing.X, _volume.Spacing.Y, _volume.Spacing.Z);

				reslicer.SetResliceAxes(VtkHelper.ConvertToVtkMatrix(resliceAxes));

				// Clamp the output based on the slice extent
				int sliceExtentX = GetSliceExtentX();
				int sliceExtentY = GetSliceExtentY();
				reslicer.SetOutputExtent(0, sliceExtentX - 1, 0, sliceExtentY - 1, 0, slabThicknessInVoxels-1);

				// Set the output origin to reflect the slice through point. The slice extent is
				//	centered on the slice through point.
				// VTK output origin is derived from the center image being 0,0
				float originX = -sliceExtentX * EffectiveSpacing / 2;
				float originY = -sliceExtentY * EffectiveSpacing / 2;
				reslicer.SetOutputOrigin(originX, originY, 0);

				switch (_slicing.InterpolationMode)
				{
					case InterpolationModes.NearestNeighbor:
						reslicer.SetInterpolationModeToNearestNeighbor();
						break;
					case InterpolationModes.Linear:
						reslicer.SetInterpolationModeToLinear();
						break;
					case InterpolationModes.Cubic:
						reslicer.SetInterpolationModeToCubic();
						break;
				}

				using (vtkExecutive exec = reslicer.GetExecutive())
				{
					VtkHelper.RegisterVtkErrorEvents(exec);

					exec.Update();

					_volume.ReleasePinnedVtkVolume();

					return reslicer.GetOutput();
				}
			}
		}

		private static byte[] MipPixelDataFromVtkSlab(vtkImageData slabImageData)
		{
            VtkHelper.StaticInitializationHack();

#if true // Do our own MIP, albeit slowly
			int[] sliceDimensions = slabImageData.GetDimensions();
			int sliceDataSize = sliceDimensions[0] * sliceDimensions[1];
			IntPtr slabDataPtr = slabImageData.GetScalarPointer();

			byte[] pixelData = MemoryManager.Allocate<byte>(sliceDataSize * sizeof(short));

			// Init with first slice
			Marshal.Copy(slabDataPtr, pixelData, 0, sliceDataSize * sizeof(short));

			// Walk through other slices, finding maximum
			unsafe
			{
				short* psSlab = (short*) slabDataPtr;

				fixed (byte* pbFrame = pixelData)
				{
					short* psFrame = (short*)pbFrame;
					for (int sliceIndex = 1; sliceIndex < sliceDimensions[2]; sliceIndex++)
					{
						for (int i = 0; i < sliceDataSize-1; ++i)
						{
							int slabIndex = sliceIndex * sliceDataSize + i;
							if (psSlab[slabIndex] > psFrame[i])
								psFrame[i] = psSlab[slabIndex];
						}
					}
				}
			}

			return pixelData;

#else // Ideally we'd use VTK to do the MIP (MinIP, Average...)
				vtkVolumeRayCastMIPFunction mip = new vtkVolumeRayCastMIPFunction();
				vtkVolumeRayCastMapper mapper = new vtkVolumeRayCastMapper();

				mapper.SetVolumeRayCastFunction(mip);
				mapper.SetInput(slabImageData);

				//TODO: Need to figure out how to use mapper to output vtkImageData

				vtkImageAlgorithm algo = new vtkImageAlgorithm();
				algo.SetInput(mapper.GetOutputDataObject(0));
				
				using (vtkExecutive exec = mapper.GetExecutive())
				{
					VtkHelper.RegisterVtkErrorEvents(exec);
					exec.Update();

					// Note: These report no output port, must have to do something else to get mapper to give us data
					//return exec.GetOutputData(0);
					return mapper.GetOutputDataObject(0);
				}
#endif
		}
#endif

		#endregion

		// Derived frome either a specified extent in millimeters or from the volume dimensions (default)
		private int GetSliceExtentX()
		{
			if (_slicerParams.SliceExtentXMillimeters != 0f)
				return (int) (_slicerParams.SliceExtentXMillimeters/EffectiveSpacing + 0.5f);
			else
				return MaxOutputImageDimension;
		}

		// Derived frome either a specified extent in millimeters or from the volume dimensions (default)
		private int GetSliceExtentY()
		{
			if (_slicerParams.SliceExtentYMillimeters != 0f)
				return (int) (_slicerParams.SliceExtentYMillimeters/EffectiveSpacing + 0.5f);
			else
				return MaxOutputImageDimension;
		}

		private int MaxOutputImageDimension
		{
			get
			{
				// This doesn't give us enough extra room, so I decided to use the diagonal along long and short dimensions
				//return (int)(LongAxisMagnitude / EffectiveSpacing + 0.5f);
				float longOutputDimension = _volume.GetLongAxisMagnitude()/EffectiveSpacing;
				float shortOutputDimenstion = _volume.GetShortAxisMagnitude()/EffectiveSpacing;
				return (int) Math.Sqrt(longOutputDimension*longOutputDimension + shortOutputDimenstion*shortOutputDimenstion);
			}
		}

		/// <summary>
		/// The effective spacing defines output spacing for slices generated by the VolumeSlicer.
		/// </summary>
		private float EffectiveSpacing
		{
			// Because we supply the real spacing to the VTK reslicer, the slices are interpolated
			//	as if the volume were isotropic. This results in an effective spacing that is the
			//	minimum spacing for the volume.
			get { return _volume.GetMinimumSpacing(); }
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