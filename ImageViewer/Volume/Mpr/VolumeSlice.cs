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
using ClearCanvas.ImageViewer.Volumes;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public class VolumeSlice : IDicomAttributeProvider, IDisposable
	{
		private IVolumeReference _volumeReference;
		private IVolumeSlicerParams _slicerParams;
		private IDicomAttributeProvider _prototypeDataSet;
		private Vector3D _throughPoint;

		public VolumeSlice(Volumes.Volume volume, IVolumeSlicerParams slicerParams, Vector3D throughPoint)
			: this(volume.CreateTransientReference(), slicerParams, throughPoint) {}

		internal VolumeSlice(IVolumeReference volumeReference, IVolumeSlicerParams slicerParams, Vector3D throughPoint)
		{
			Platform.CheckForNullReference(throughPoint, "throughPoint");
			var volume = volumeReference.Volume;

			_volumeReference = volumeReference;
			_slicerParams = slicerParams;
			_throughPoint = throughPoint;

			// keep a direct reference to the prototype, so that attribute values are available even if the referenced volume is unloaded via memory management
			_prototypeDataSet = volume.DataSet;

			// JY: ideally, each slicing plane is represented by a single multiframe SOP where the individual slices are the frames.
			// We need to support multi-valued Slice Location in the base viewer first.
			// When that is implemented, the SOPs should be created on the first frame of the slicing (i.e. one of the end slices)
			// and the Slice Location Vector will simply store the slice locations relative to that defined in these attributes.
			// Also, the rows and columns will have to be computed to be the MAX possible size (all frames must have same size)

			// compute Rows and Columns to reflect actual output size
			var frameSize = GetSliceExtent(volume, slicerParams);
			Colums = frameSize.Width;
			Rows = frameSize.Height;

			// compute Image Orientation (Patient)
			var matrix = new Matrix(slicerParams.SlicingPlaneRotation);
			matrix[3, 0] = _throughPoint.X;
			matrix[3, 1] = _throughPoint.Y;
			matrix[3, 2] = _throughPoint.Z;

			var resliceAxesPatientOrientation = _volumeReference.Volume.RotateToPatientOrientation(matrix);
			var orientation = new DicomAttributeDS(DicomTags.ImageOrientationPatient);
			orientation.SetFloat32(0, resliceAxesPatientOrientation[0, 0]);
			orientation.SetFloat32(1, resliceAxesPatientOrientation[0, 1]);
			orientation.SetFloat32(2, resliceAxesPatientOrientation[0, 2]);
			orientation.SetFloat32(3, resliceAxesPatientOrientation[1, 0]);
			orientation.SetFloat32(4, resliceAxesPatientOrientation[1, 1]);
			orientation.SetFloat32(5, resliceAxesPatientOrientation[1, 2]);
			ImageOrientationPatient = orientation.ToString();

			// compute Image Position (Patient)
			var topLeftOfSlicePatient = GetTopLeftOfSlicePatient(frameSize, _throughPoint, volume, slicerParams);
			var position = new DicomAttributeDS(DicomTags.ImagePositionPatient);
			position.SetFloat32(0, topLeftOfSlicePatient.X);
			position.SetFloat32(1, topLeftOfSlicePatient.Y);
			position.SetFloat32(2, topLeftOfSlicePatient.Z);
			ImagePositionPatient = position.ToString();
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

			if (_volumeReference != null)
			{
				_volumeReference.Dispose();
				_volumeReference = null;
			}

			_prototypeDataSet = null;
			_slicerParams = null;
			_throughPoint = null;
		}

		public int Rows { get; private set; }
		public int Colums { get; private set; }
		public string ImageOrientationPatient { get; private set; }
		public string ImagePositionPatient { get; private set; }

		public IVolumeReference VolumeReference
		{
			get { return _volumeReference; }
		}

		public byte[] GetPixelData()
		{
			using (var slicer = new VolumeSlicer(_volumeReference, _slicerParams))
			{
				return slicer.CreateSliceNormalizedPixelData(_throughPoint);
			}
		}

		#region Implementation of IDicomAttributeProvider

		public DicomAttribute this[DicomTag tag]
		{
			get { return _prototypeDataSet[tag]; }
			set { _prototypeDataSet[tag] = value; }
		}

		public DicomAttribute this[uint tag]
		{
			get { return _prototypeDataSet[tag]; }
			set { _prototypeDataSet[tag] = value; }
		}

		public bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			return _prototypeDataSet.TryGetAttribute(tag, out attribute);
		}

		public bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			return _prototypeDataSet.TryGetAttribute(tag, out attribute);
		}

		#endregion

		#region Misc Helpers for computing SOP attribute values

		// VTK treats the reslice point as the center of the output image. Given the plane orientation
		//	and size of the output image, we can derive the top left of the output image in patient space
		private static Vector3D GetTopLeftOfSlicePatient(Size frameSize, Vector3D throughPoint, Volumes.Volume volume, IVolumeSlicerParams slicerParams)
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
		private static Size GetSliceExtent(Volumes.Volume volume, IVolumeSlicerParams slicerParams)
		{
			var effectiveSpacing = GetEffectiveSpacing(volume);
			var longOutputDimension = volume.LongAxisMagnitude/effectiveSpacing;
			var shortOutputDimenstion = volume.ShortAxisMagnitude/effectiveSpacing;
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
		/// The effective spacing defines output spacing for slices generated by the VolumeSlicer.
		/// </summary>
		private static float GetEffectiveSpacing(Volumes.Volume volume)
		{
			// Because we supply the real spacing to the VTK reslicer, the slices are interpolated
			//	as if the volume were isotropic. This results in an effective spacing that is the
			//	minimum spacing for the volume.
			return volume.MinimumSpacing;
		}

		#endregion
	}
}