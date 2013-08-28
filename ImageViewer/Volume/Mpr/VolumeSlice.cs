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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using ClearCanvas.ImageViewer.Volumes;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <remarks>
	/// This type has been deprecated in favour of its equivalent in the core volume framework (<see cref="VolumeSlice2"/>).
	/// </remarks>
	[Obsolete("Use VolumeSlice2 instead.")]
	public class VolumeSlice : IVolumeSlice
	{
		private VolumeSlice2 _volumeSlice;

		public VolumeSlice(Volumes.Volume volume, IVolumeSlicerParams slicerParams, Vector3D throughPoint)
			: this(volume.CreateReference(), slicerParams, throughPoint) {}

		internal VolumeSlice(IVolumeReference volumeReference, IVolumeSlicerParams slicerParams, Vector3D throughPoint)
		{
			Platform.CheckForNullReference(throughPoint, "throughPoint");

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
			                               slicerParams.SliceSpacing, Convert(slicerParams.InterpolationMode));

			_volumeSlice = new VolumeSlice2(volumeReference, true, args, topLeftOfSlicePatient, slicerParams.SliceSpacing);
		}

		~VolumeSlice()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Dispose method threw an exception.");
			}
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Dispose method threw an exception.");
			}
		}

		protected void Dispose(bool disposing)
		{
			if (!disposing) return;

			if (_volumeSlice != null)
			{
				_volumeSlice.Dispose();
				_volumeSlice = null;
			}
		}

		public int Rows
		{
			get { return _volumeSlice.Rows; }
		}

		public int Columns
		{
			get { return _volumeSlice.Columns; }
		}

		public string PixelSpacing
		{
			get { return _volumeSlice.PixelSpacing; }
		}

		public string ImageOrientationPatient
		{
			get { return _volumeSlice.ImageOrientationPatient; }
		}

		public string ImagePositionPatient
		{
			get { return _volumeSlice.ImagePositionPatient; }
		}

		public string SliceThickness
		{
			get { return _volumeSlice.SliceThickness; }
		}

		public string SpacingBetweenSlices
		{
			get { return _volumeSlice.SpacingBetweenSlices; }
		}

		public IVolumeReference VolumeReference
		{
			get { return _volumeSlice.VolumeReference; }
		}

		public byte[] GetPixelData()
		{
			return _volumeSlice.GetPixelData();
		}

		#region Implementation of IDicomAttributeProvider

		public DicomAttribute this[DicomTag tag]
		{
			get { return _volumeSlice[tag]; }
			set { _volumeSlice[tag] = value; }
		}

		public DicomAttribute this[uint tag]
		{
			get { return _volumeSlice[tag]; }
			set { _volumeSlice[tag] = value; }
		}

		public bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			return _volumeSlice.TryGetAttribute(tag, out attribute);
		}

		public bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			return _volumeSlice.TryGetAttribute(tag, out attribute);
		}

		#endregion

		#region Misc Helpers for computing SOP attribute values

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
	}
}