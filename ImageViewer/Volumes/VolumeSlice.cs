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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volumes
{
	public class VolumeSlice : IDicomAttributeProvider, IDisposable
	{
		private IVolumeReference _volumeReference;
		private VolumeSliceArgs _sliceArgs;
		private Vector3D _imagePositionPatient;

		public VolumeSlice(Volume volume, VolumeSliceArgs sliceArgs, Vector3D imagePositionPatient, float? spacingBetweenSlices = null)
			: this(volume.CreateReference(), sliceArgs, imagePositionPatient, spacingBetweenSlices) {}

		public VolumeSlice(IVolumeReference volumeReference, bool ownReference, VolumeSliceArgs sliceArgs, Vector3D imagePositionPatient, float? spacingBetweenSlices = null)
			: this(ownReference ? volumeReference : volumeReference.Clone(), sliceArgs, imagePositionPatient, spacingBetweenSlices) {}

		internal VolumeSlice(IVolumeReference volumeReference, VolumeSliceArgs sliceArgs, Vector3D imagePositionPatient, float? spacingBetweenSlices)
		{
			Platform.CheckForNullReference(volumeReference, "volumeReference");
			Platform.CheckForNullReference(sliceArgs, "slicerArgs");
			Platform.CheckForNullReference(imagePositionPatient, "imagePositionPatient");

			_volumeReference = volumeReference;
			_sliceArgs = sliceArgs;
			_imagePositionPatient = imagePositionPatient;

			// compute Rows and Columns to reflect actual output size
			Columns = sliceArgs.Columns;
			Rows = sliceArgs.Rows;

			// compute Slice Thickness
			var sliceThickness = sliceArgs.SliceThickness;
			SliceThickness = new DicomAttributeDS(DicomTags.SliceThickness) {Values = sliceThickness}.ToString();
			SpacingBetweenSlices = spacingBetweenSlices.HasValue ? new DicomAttributeDS(DicomTags.SpacingBetweenSlices) {Values = spacingBetweenSlices.Value}.ToString() : string.Empty;

			// compute Pixel Spacing
			var spacing = new DicomAttributeDS(DicomTags.PixelSpacing);
			spacing.SetFloat32(0, sliceArgs.RowSpacing);
			spacing.SetFloat32(1, sliceArgs.ColumnSpacing);
			PixelSpacing = spacing.ToString();

			// compute Image Orientation (Patient)
			var rowOrientation = sliceArgs.RowOrientationPatient;
			var columnOrientation = sliceArgs.ColumnOrientationPatient;
			var orientation = new DicomAttributeDS(DicomTags.ImageOrientationPatient);
			orientation.SetFloat32(0, rowOrientation.X);
			orientation.SetFloat32(1, rowOrientation.Y);
			orientation.SetFloat32(2, rowOrientation.Z);
			orientation.SetFloat32(3, columnOrientation.X);
			orientation.SetFloat32(4, columnOrientation.Y);
			orientation.SetFloat32(5, columnOrientation.Z);
			ImageOrientationPatient = orientation.ToString();

			// compute Image Position (Patient)
			var position = new DicomAttributeDS(DicomTags.ImagePositionPatient);
			position.SetFloat32(0, _imagePositionPatient.X);
			position.SetFloat32(1, _imagePositionPatient.Y);
			position.SetFloat32(2, _imagePositionPatient.Z);
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

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;

			if (_volumeReference != null)
			{
				_volumeReference.Dispose();
				_volumeReference = null;
			}

			_sliceArgs = null;
			_imagePositionPatient = null;
		}

		public int Rows { get; private set; }
		public int Columns { get; private set; }
		public string PixelSpacing { get; private set; }
		public string SliceThickness { get; private set; }
		public string SpacingBetweenSlices { get; private set; }
		public string ImageOrientationPatient { get; private set; }
		public string ImagePositionPatient { get; private set; }

		public IVolumeReference VolumeReference
		{
			get { return _volumeReference; }
		}

		public virtual byte[] GetPixelData()
		{
			using (var slicer = VolumeSlicerCore.Create(_volumeReference, _sliceArgs))
			{
				return slicer.CreateSlicePixelData(_imagePositionPatient);
			}
		}

		#region Implementation of IDicomAttributeProvider

		public virtual DicomAttribute this[DicomTag tag]
		{
			get { return _volumeReference.DataSet[tag]; }
			set { _volumeReference.DataSet[tag] = value; }
		}

		public virtual DicomAttribute this[uint tag]
		{
			get { return _volumeReference.DataSet[tag]; }
			set { _volumeReference.DataSet[tag] = value; }
		}

		public virtual bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			return _volumeReference.DataSet.TryGetAttribute(tag, out attribute);
		}

		public virtual bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			return _volumeReference.DataSet.TryGetAttribute(tag, out attribute);
		}

		#endregion
	}
}