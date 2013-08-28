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
using System.Linq;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Factory class for creating <see cref="VolumeSlice2"/> instances representing 2-dimensional slices of a <see cref="Volume"/>.
	/// </summary>
	public sealed class VolumeSliceFactory
	{
		private VolumeInterpolationMode _interpolation = VolumeInterpolationMode.Linear;

		/// <summary>
		/// Gets or sets the row orientation of the output as a directional vector in patient coordinates.
		/// </summary>
		/// <remarks>
		/// <para>If not specified, the original row orientation of the source volume will be used.</para>
		/// </remarks>
		public Vector3D RowOrientationPatient { get; set; }

		/// <summary>
		/// Gets or sets the column orientation of the output as a directional vector in patient coordinates.
		/// </summary>
		/// <remarks>
		/// <para>If not specified, the original column orientation of the source volume will be used.</para>
		/// </remarks>
		public Vector3D ColumnOrientationPatient { get; set; }

		/// <summary>
		/// Gets or sets the spacing, in patient units, between consecutive rows of the output.
		/// </summary>
		/// <remarks>
		/// <para>If not specified, a suitable isotropic spacing (i.e. <see cref="RowSpacing"/> will be the same as <see cref="ColumnSpacing"/>) will be derived based on the source volume spacing.</para>
		/// </remarks>
		public float? RowSpacing { get; set; }

		/// <summary>
		/// Gets or sets the spacing, in patient units, between consecutive columns of the output.
		/// </summary>
		/// <remarks>
		/// <para>If not specified, a suitable isotropic spacing (i.e. <see cref="ColumnSpacing"/> will be the same as <see cref="RowSpacing"/>) will be derived based on the source volume spacing.</para>
		/// </remarks>
		public float? ColumnSpacing { get; set; }

		/// <summary>
		/// Gets or sets the number of rows in the output.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If not specified, a suitable value will be derived from <see cref="SliceHeight"/> or, failing that,
		/// chosen such that the entire source volume will be visible in the output.
		/// </para>
		/// </remarks>
		public int? Rows { get; set; }

		/// <summary>
		/// Gets or sets the number of columns in the output.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If not specified, a suitable value will be derived from <see cref="SliceWidth"/> or, failing that,
		/// chosen such that the entire source volume will be visible in the output.
		/// </para>
		/// </remarks>
		public int? Columns { get; set; }

		/// <summary>
		/// Gets or sets the width, in patient units, of each individual slice of the output.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If <see cref="Columns"/> is specified, that value will take precendence.
		/// If not specified, a suitable value will be chosen such that the entire source volume will be visible in the output.
		/// </para>
		/// </remarks>
		public float? SliceWidth { get; set; }

		/// <summary>
		/// Gets or sets the height, in patient units, of each individual slice of the output.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If <see cref="Rows"/> is specified, that value will take precendence.
		/// If not specified, a suitable value will be chosen such that the entire source volume will be visible in the output.
		/// </para>
		/// </remarks>
		public float? SliceHeight { get; set; }

		/// <summary>
		/// Gets or sets the spacing, in patient units, between consecutive slices of the output.
		/// </summary>
		public float? SliceSpacing { get; set; }

		/// <summary>
		/// Gets or sets the thickness, in patient units, represented by each individual slice of the output.
		/// </summary>
		public float? SliceThickness { get; set; }

		/// <summary>
		/// Gets or sets the interpolation mode of the output.
		/// </summary>
		/// <remarks>
		/// <para>The default value is <see cref="VolumeInterpolationMode.Linear"/>.</para>
		/// </remarks>
		public VolumeInterpolationMode Interpolation
		{
			get { return _interpolation; }
			set { _interpolation = value; }
		}

		public IList<VolumeSlice2> CreateSlices(IVolumeReference volumeReference)
		{
			return CreateSlicesCore(volumeReference, null, null);
		}

		public IList<VolumeSlice2> CreateSlices(IVolumeReference volumeReference, Vector3D startPosition)
		{
			return CreateSlicesCore(volumeReference, startPosition, null);
		}

		public IList<VolumeSlice2> CreateSlices(IVolumeReference volumeReference, Vector3D startPosition, Vector3D endPosition)
		{
			return CreateSlicesCore(volumeReference, startPosition, endPosition);
		}

		private IList<VolumeSlice2> CreateSlicesCore(IVolumeReference volumeReference, Vector3D startPosition, Vector3D endPosition)
		{
			// get the pixel spacing (defaults to isotropic spacing based on smallest volume spacing dimension)
			var pixelSpacing = GetPixelSpacing(volumeReference);

			// get the spacing between slices (defaults to smallest volume spacing dimension)
			var sliceSpacing = SliceSpacing ?? GetMinimumComponent(volumeReference.VoxelSpacing);

			// get the thickness of each slice (defaults to slice spacing)
			var sliceThickness = SliceThickness ?? sliceSpacing;

			// get the axes of the output plane and its normal in patient coordinates - these are the axes of the slicer frame
			var slicerAxisX = (RowOrientationPatient ?? volumeReference.VolumeOrientationPatientX).Normalize();
			var slicerAxisY = (ColumnOrientationPatient ?? volumeReference.VolumeOrientationPatientY).Normalize();
			var slicerAxisZ = slicerAxisX.Cross(slicerAxisY).Normalize();

			// project the corners of the volume on to the slicer axes to determine the bounds of the volume in the slicer frame
			float minBoundsX = float.MaxValue, minBoundsY = float.MaxValue, minBoundsZ = float.MaxValue;
			float maxBoundsX = float.MinValue, maxBoundsY = float.MinValue, maxBoundsZ = float.MinValue;
			var volumeDimensions = volumeReference.VolumeSize;
			foreach (var corner in new[]
			                       	{
			                       		new Vector3D(0, 0, 0),
			                       		new Vector3D(0, 0, volumeDimensions.Z),
			                       		new Vector3D(0, volumeDimensions.Y, 0),
			                       		new Vector3D(0, volumeDimensions.Y, volumeDimensions.Z),
			                       		new Vector3D(volumeDimensions.X, 0, 0),
			                       		new Vector3D(volumeDimensions.X, 0, volumeDimensions.Z),
			                       		new Vector3D(volumeDimensions.X, volumeDimensions.Y, 0),
			                       		new Vector3D(volumeDimensions.X, volumeDimensions.Y, volumeDimensions.Z)
			                       	}.Select(volumeReference.ConvertToPatient))
			{
				var projection = corner.Dot(slicerAxisX);
				if (minBoundsX > projection) minBoundsX = projection;
				if (maxBoundsX < projection) maxBoundsX = projection;

				projection = corner.Dot(slicerAxisY);
				if (minBoundsY > projection) minBoundsY = projection;
				if (maxBoundsY < projection) maxBoundsY = projection;

				projection = corner.Dot(slicerAxisZ);
				if (minBoundsZ > projection) minBoundsZ = projection;
				if (maxBoundsZ < projection) maxBoundsZ = projection;
			}

			// get the origin of the slicer frame in patient coordinates
			var slicerOrigin = minBoundsX*slicerAxisX + minBoundsY*slicerAxisY + minBoundsZ*slicerAxisZ;

			// get the dimensions (in patient units) of the region that bounds the volume projected to the slicer frame - i.e. the dimensions of the stack of output slices
			var stackWidth = SliceWidth.HasValue ? SliceWidth.Value : maxBoundsX - minBoundsX;
			var stackHeight = SliceHeight.HasValue ? SliceHeight.Value : maxBoundsY - minBoundsY;
			var stackDepth = maxBoundsZ - minBoundsZ;

			// get the rows and columns of the slice output
			var sliceColumns = Columns ?? (int) (Math.Abs(1.0*stackWidth/pixelSpacing.Width) + 0.5);
			var sliceRows = Rows ?? (int) (Math.Abs(1.0*stackHeight/pixelSpacing.Height) + 0.5);

			// capture all the slicer parameters in an args object
			var args = new VolumeSliceArgs
			           	{
			           		RowOrientationPatient = slicerAxisX,
			           		ColumnOrientationPatient = slicerAxisY,
			           		Rows = sliceRows,
			           		Columns = sliceColumns,
			           		RowSpacing = pixelSpacing.Height,
			           		ColumnSpacing = pixelSpacing.Width,
			           		SliceThickness = sliceThickness,
			           		Interpolation = Interpolation
			           	};

			// get the number of slices in the output
			var sliceCount = (int) (Math.Abs(1.0*stackDepth/sliceSpacing) + 0.5);

			// compute the increment in position between consecutive slices
			var slicePositionIncrement = slicerAxisZ*sliceSpacing;

			// compute the position of the first slice
			var firstSlicePosition = slicerOrigin;

			if (startPosition != null)
			{
				var slicerStart = startPosition - slicerOrigin;

				if (Columns.HasValue || SliceWidth.HasValue)
				{
					var offsetX = slicerAxisX.Dot(slicerStart) - sliceColumns*pixelSpacing.Width/2f;
					firstSlicePosition += offsetX*slicerAxisX;
				}

				if (Rows.HasValue || SliceHeight.HasValue)
				{
					var offsetY = slicerAxisY.Dot(slicerStart) - sliceRows*pixelSpacing.Height/2f;
					firstSlicePosition += offsetY*slicerAxisY;
				}

				if (endPosition != null)
				{
					var offsetZ = slicerAxisZ.Dot(slicerStart);
					firstSlicePosition += offsetZ*slicerAxisZ;

					var stackRange = slicerAxisZ.Dot(endPosition - slicerOrigin) - offsetZ;
					sliceCount = (int) (Math.Abs(1.0*stackRange/sliceSpacing) + 0.5);
				}
			}

			// create the output slices
			var list = new List<VolumeSlice2>();
			for (var n = 0; n < sliceCount; ++n)
			{
				var imagePositionPatient = firstSlicePosition + n*slicePositionIncrement;
				list.Add(new VolumeSlice2(volumeReference.Clone(), args, imagePositionPatient, sliceSpacing));
			}
			return list;
		}

		private SizeF GetPixelSpacing(IVolumeHeader volumeHeader)
		{
			var columnHasValue = ColumnSpacing.HasValue;
			var rowHasValue = RowSpacing.HasValue;
			if (!columnHasValue && !rowHasValue)
			{
				var spacing = GetMinimumComponent(volumeHeader.VoxelSpacing);
				return new SizeF(spacing, spacing);
			}
			else if (columnHasValue && rowHasValue)
			{
				return new SizeF(ColumnSpacing.Value, RowSpacing.Value);
			}
			else
			{
				var spacing = columnHasValue ? ColumnSpacing.Value : RowSpacing.Value;
				return new SizeF(spacing, spacing);
			}
		}

		private static float GetMinimumComponent(Vector3D vector3D)
		{
			return vector3D.X < vector3D.Y ? Math.Min(vector3D.X, vector3D.Z) : Math.Min(vector3D.Y, vector3D.Z);
		}
	}
}