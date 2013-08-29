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
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Volumes;
using ClearCanvas.ImageViewer.Vtk;
using vtk;

namespace ClearCanvas.ImageViewer.VTK
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
			var stackOrientation = rowOrientation.Cross(columnOrientation);

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

			using (var imageData = GenerateVtkSlice(resliceAxes))
			{
				var pixelData = CreatePixelDataFromVtkSlice(imageData);
				imageData.ReleaseData();
				return pixelData;
			}
		}

		/// <summary>
		/// Slices the volume using the specified reslice axes.
		/// </summary>
		private vtkImageData GenerateVtkSlice(Matrix resliceAxes)
		{
			// read these parameters once to ensure consistency
			var columnSpacing = ColumnSpacing;
			var rowSpacing = RowSpacing;
			var columns = Columns;
			var rows = Rows;

			using (var reslicer = new vtkImageReslice())
			using (var resliceAxesMatrix = new vtkMatrix4x4())
			{
				// update the reslice axes matrix with the values from the slicing orientation
				resliceAxesMatrix.SetElements(resliceAxes);

				reslicer.RegisterVtkErrorEvents();

				// obtain a pinned VTK volume for the reslicer
				using (var volume = VolumeReference.Volume.CreateVtkVolumeHandle())
				{
					// set input to the volume
					reslicer.SetInput(volume.vtkImageData);
					reslicer.SetInformationInput(volume.vtkImageData);

					// instruct reslicer to output 2D images
					reslicer.SetOutputDimensionality(2);

					// use the volume's padding value for all pixels that are outside the volume
					reslicer.SetBackgroundLevel(VolumeReference.PaddingValue);

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
					switch (Interpolation)
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

					using (var executive = reslicer.GetExecutive())
					{
						executive.RegisterVtkErrorEvents();
						executive.Update();
					}

					var output = reslicer.GetOutput();
					//Just in case VTK uses the matrix internally.
					return output;
				}
			}
		}

		private static byte[] CreatePixelDataFromVtkSlice(vtkImageData sliceImageData)
		{
			var sliceDataPtr = sliceImageData.GetScalarPointer();
			if (sliceDataPtr.Equals(IntPtr.Zero)) return null;

			var scalarSize = sliceImageData.GetScalarSize();

			var sliceDimensions = sliceImageData.GetDimensions();
			var sliceDataSize = sliceDimensions[0]*sliceDimensions[1];

			var pixelData = MemoryManager.Allocate<byte>(sliceDataSize*sizeof (short));
			Marshal.Copy(sliceDataPtr, pixelData, 0, sliceDataSize*sizeof (short));
			return pixelData;
		}
	}
}