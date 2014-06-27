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
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Volumes;
using ClearCanvas.ImageViewer.Vtk.Utilities;
using vtk;

namespace ClearCanvas.ImageViewer.Vtk
{
	/// <summary>
	/// Extends <see cref="IVolumeSlicerCoreProvider"/> with the <see cref="VtkVolumeSlicerCore"/> implementation of volume reslicers.
	/// </summary>
	[ExtensionOf(typeof (VolumeSlicerCoreProviderExtensionPoint))]
	internal sealed class VtkVolumeSlicerCoreProvider : IVolumeSlicerCoreProvider
	{
		public bool IsSupported(VolumeSliceArgs args)
		{
			return true;
		}

		public IVolumeSlicerCore CreateSlicerCore(IVolumeReference volumeReference, VolumeSliceArgs args)
		{
			return new VtkVolumeSlicerCore(volumeReference, args);
		}
	}

	/// <summary>
	/// Implements <see cref="IVolumeSlicerCore"/> using the VTK <see cref="vtk.vtkImageReslice"/> utility.
	/// </summary>
	public class VtkVolumeSlicerCore : VolumeSlicerCore
	{
		/// <summary>
		/// Initializes a new instance of <see cref="VtkVolumeSlicerCore"/>.
		/// </summary>
		/// <param name="volumeReference"></param>
		/// <param name="args"></param>
		public VtkVolumeSlicerCore(IVolumeReference volumeReference, VolumeSliceArgs args)
			: base(volumeReference, args) {}

		public override byte[] CreateSlicePixelData(Vector3D imagePositionPatient)
		{
			var rowOrientation = RowOrientationPatient;
			var columnOrientation = ColumnOrientationPatient;
			var stackOrientation = rowOrientation.Cross(columnOrientation).Normalize();

			// VTK reslice axes are in the volume coordinate system and should be positioned at the center of the desired output image
			var reslicePosition = imagePositionPatient + Columns*ColumnSpacing/2f*rowOrientation + Rows*RowSpacing/2f*columnOrientation;
			reslicePosition = VolumeReference.ConvertToVolume(reslicePosition);

			var resliceAxes = new Matrix(new[,]
			                             	{
			                             		{rowOrientation.X, rowOrientation.Y, rowOrientation.Z, 0},
			                             		{columnOrientation.X, columnOrientation.Y, columnOrientation.Z, 0},
			                             		{stackOrientation.X, stackOrientation.Y, stackOrientation.Z, 0},
			                             		{0, 0, 0, 1}
			                             	});
			resliceAxes = VolumeReference.RotateToVolumeOrientation(resliceAxes).Transpose();
			resliceAxes.SetColumn(3, reslicePosition.X, reslicePosition.Y, reslicePosition.Z, 1);

			if (Subsamples > 1)
			{
				// if subsampling is required, extract the thick slice ("slab") from the volume and then aggregate it using the specified projection method
				return GetSlabPixelData(VolumeReference, resliceAxes, stackOrientation, Rows, Columns, Subsamples, RowSpacing, ColumnSpacing, SliceThickness, Interpolation, Projection);
			}
			else
			{
				// extract the (thin) slice from the volume
				return GetSlicePixelData(VolumeReference, resliceAxes, Rows, Columns, RowSpacing, ColumnSpacing, Interpolation);
			}
		}

		/// <summary>
		/// Gets the pixel data representing a thin slice of the volume.
		/// </summary>
		private static byte[] GetSlicePixelData(IVolumeReference volumeReference, Matrix resliceAxes, int rows, int columns, float rowSpacing, float columnSpacing, VolumeInterpolationMode interpolation)
		{
			using (var reslicer = new vtkImageReslice())
			using (var resliceAxesMatrix = new vtkMatrix4x4())
			using (var executive = reslicer.GetExecutive())
			using (var volume = volumeReference.Volume.CreateVtkVolumeHandle())
			{
				// update the reslice axes matrix with the values from the slicing orientation
				resliceAxesMatrix.SetElements(resliceAxes);

				// register for errors on the reslicer
				reslicer.RegisterVtkErrorEvents();

				// set input to the volume
				reslicer.SetInput(volume.vtkImageData);
				reslicer.SetInformationInput(volume.vtkImageData);

				// instruct reslicer to output 2D images
				reslicer.SetOutputDimensionality(2);

				// use the volume's padding value for all pixels that are outside the volume
				reslicer.SetBackgroundLevel(volumeReference.PaddingValue);

				// ensure the slicer outputs at our desired spacing
				reslicer.SetOutputSpacing(columnSpacing, rowSpacing, Math.Min(columnSpacing, rowSpacing));

				// set the reslice axes
				reslicer.SetResliceAxes(resliceAxesMatrix);

				// clamp the output based on the slice extent
				reslicer.SetOutputExtent(0, columns - 1, 0, rows - 1, 0, 0);

				// set the output origin to the slice through-point (output image will be centered on this location)
				// VTK output origin is derived from the center image being 0,0
				reslicer.SetOutputOrigin(-columns*columnSpacing/2, -rows*rowSpacing/2, 0);

				// select the requested interpolation mode
				switch (interpolation)
				{
					case VolumeInterpolationMode.NearestNeighbor:
						reslicer.SetInterpolationModeToNearestNeighbor();
						break;
					case VolumeInterpolationMode.Linear:
						reslicer.SetInterpolationModeToLinear();
						break;
					case VolumeInterpolationMode.Cubic:
						reslicer.SetInterpolationModeToCubic();
						break;
				}

				// register for errors on the reslicer executive
				executive.RegisterVtkErrorEvents();
				executive.Update();

				// get the slice output
				using (var imageData = reslicer.GetOutput())
				{
					var pixelData = ReadVtkImageData(imageData);
					imageData.ReleaseData();
					return pixelData;
				}
			}
		}

		/// <summary>
		/// Gets the pixel data representing a thick slice (a.k.a. slab) of the volume.
		/// </summary>
		private static byte[] GetSlabPixelData(IVolumeReference volumeReference, Matrix resliceAxes, Vector3D stackOrientation, int rows, int columns, int subsamples, float rowSpacing, float columnSpacing, float sliceThickness, VolumeInterpolationMode interpolation, VolumeProjectionMode projection)
		{
			if (subsamples <= 1) return GetSlicePixelData(volumeReference, resliceAxes, rows, columns, rowSpacing, columnSpacing, interpolation);
			var subsampleSpacing = sliceThickness/(subsamples - 1);

			using (var reslicer = new vtkImageReslice())
			using (var resliceAxesMatrix = new vtkMatrix4x4())
			using (var executive = reslicer.GetExecutive())
			using (var volume = volumeReference.Volume.CreateVtkVolumeHandle())
			{
				// update the reslice axes matrix with the values from the slicing orientation
				resliceAxesMatrix.SetElements(resliceAxes);

				// determine offset for start of slab (we centre the slab on the requested slice position, as DICOM defines "image position (patient)" to be centre of the thick slice)
				var slabOffset = volumeReference.RotateToVolumeOrientation(-sliceThickness/2f*stackOrientation) + new Vector3D(resliceAxes[0, 3], resliceAxes[1, 3], resliceAxes[2, 3]);
				resliceAxesMatrix.SetElement(0, 3, slabOffset.X);
				resliceAxesMatrix.SetElement(1, 3, slabOffset.Y);
				resliceAxesMatrix.SetElement(2, 3, slabOffset.Z);

				// register for errors on the reslicer
				reslicer.RegisterVtkErrorEvents();

				// set input to the volume
				reslicer.SetInput(volume.vtkImageData);
				reslicer.SetInformationInput(volume.vtkImageData);

				// instruct reslicer to output a 3D slab volume
				reslicer.SetOutputDimensionality(3);

				// use the volume's padding value for all pixels that are outside the volume
				reslicer.SetBackgroundLevel(volumeReference.PaddingValue);

				// ensure the slicer outputs at our desired spacing
				reslicer.SetOutputSpacing(columnSpacing, rowSpacing, subsampleSpacing);

				// set the reslice axes
				reslicer.SetResliceAxes(resliceAxesMatrix);

				// clamp the output based on the slice extent
				reslicer.SetOutputExtent(0, columns - 1, 0, rows - 1, 0, subsamples - 1);

				// set the output origin to the slice through-point (output image will be centered on this location)
				// VTK output origin is derived from the center image being 0,0
				reslicer.SetOutputOrigin(-columns*columnSpacing/2, -rows*rowSpacing/2, 0);

				// select the requested interpolation mode
				switch (interpolation)
				{
					case VolumeInterpolationMode.NearestNeighbor:
						reslicer.SetInterpolationModeToNearestNeighbor();
						break;
					case VolumeInterpolationMode.Linear:
						reslicer.SetInterpolationModeToLinear();
						break;
					case VolumeInterpolationMode.Cubic:
						reslicer.SetInterpolationModeToCubic();
						break;
				}

				// select the requested slab projection mode
				Action<IntPtr, byte[], int, int, int, bool> slabAggregator;
				switch (projection)
				{
					case VolumeProjectionMode.Maximum:
						slabAggregator = SlabProjection.AggregateSlabMaximumIntensity;
						break;
					case VolumeProjectionMode.Minimum:
						slabAggregator = SlabProjection.AggregateSlabMinimumIntensity;
						break;
					case VolumeProjectionMode.Average:
					default:
						slabAggregator = SlabProjection.AggregateSlabAverageIntensity;
						break;
				}

				// register for errors on the reslicer executive
				executive.RegisterVtkErrorEvents();
				executive.Update();

				// get the slice output
				using (var imageData = reslicer.GetOutput())
				{
					var pixelData = SlabVtkImageData(imageData, slabAggregator, volumeReference.BitsPerVoxel, volumeReference.Signed);
					imageData.ReleaseData();
					return pixelData;
				}
			}
		}

		private static byte[] ReadVtkImageData(vtkImageData imageData)
		{
			// get the pointer to the image data
			var pData = imageData.GetScalarPointer();
			if (pData.Equals(IntPtr.Zero)) return null;

			// get number of pixels in data
			var dataDimensions = imageData.GetDimensions();
			var dataCount = dataDimensions[0]*dataDimensions[1]*dataDimensions[2];

			// compute byte length of data
			var dataLength = dataCount*imageData.GetScalarSize();

			// copy data to managed buffer
			var pixelData = MemoryManager.Allocate<byte>(dataLength);
			Marshal.Copy(pData, pixelData, 0, dataLength);
			return pixelData;
		}

		private static byte[] SlabVtkImageData(vtkImageData slabData, Action<IntPtr, byte[], int, int, int, bool> slabAggregator, int bitsPerVoxel, bool signed)
		{
			// get the pointer to the image data
			var pData = slabData.GetScalarPointer();
			if (pData.Equals(IntPtr.Zero)) return null;

			// get number of subsamples and pixels per subsample
			var dataDimensions = slabData.GetDimensions();
			var subsamplePixels = dataDimensions[0]*dataDimensions[1];
			var subsamples = dataDimensions[2];

			// compute byte length of slabbed output data
			var dataLength = subsamplePixels*slabData.GetScalarSize();

			// slab data to managed buffer
			var pixelData = MemoryManager.Allocate<byte>(dataLength);
			slabAggregator.Invoke(pData, pixelData, subsamples, subsamplePixels, bitsPerVoxel/8, signed);
			return pixelData;
		}

		#region VTK Reslice Static Initialization Hack

		static VtkVolumeSlicerCore()
		{
			//VTK initializes some static variables (collections, actually) internally in a way that is not thread safe.
			//It seems that, at least for the code we use to do reslicing, the collections are fully initialized after 
			//the first slice is created from the first volume. Problems arise, however, if 2 slices are being created
			//simultaneously on different threads.
			//For the record, individual VTK objects are not thread-safe and should never be shared across threads.
			//This hack exists only to workaround the static collections issue. Note also, that this hack is not future-proof;
			//if we start using different parts of VTK in the future, we need to make sure this hack still works.

			//Create one signed and one unsigned, just to make sure.
			CreateAndResliceVolume(false);
			CreateAndResliceVolume(true);
		}

		private static void CreateAndResliceVolume(bool signed)
		{
			const int width = 10;
			const int height = 10;
			const int depth = 10;

			Volume vol;
			if (signed)
			{
				vol = new S16Volume(new short[width*height*depth],
				                    new Size3D(width, height, depth), new Vector3D(1, 1, 1),
				                    new Vector3D(0, 0, 0), Matrix3D.GetIdentity(),
				                    new DicomAttributeCollection(){ValidateVrLengths = false, ValidateVrValues = false},
				                    short.MinValue);
			}
			else
			{
				vol = new U16Volume(new ushort[width*height*depth],
				                    new Size3D(width, height, depth), new Vector3D(1, 1, 1),
				                    new Vector3D(0, 0, 0), Matrix3D.GetIdentity(),
				                    new DicomAttributeCollection(),
				                    ushort.MinValue);
			}

			try
			{
				using (var volumeReference = vol.CreateReference())
				{
					GetSlicePixelData(volumeReference, Matrix.GetIdentity(4), 20, 20, 0.5f, 0.5f, VolumeInterpolationMode.Linear);
					GetSlabPixelData(volumeReference, Matrix.GetIdentity(4), new Vector3D(0, 0, 1), 20, 20, 3, 0.5f, 0.5f, 1, VolumeInterpolationMode.Linear, VolumeProjectionMode.Average);
				}
			}
			finally
			{
				vol.Dispose();
			}
		}

		#endregion
	}
}