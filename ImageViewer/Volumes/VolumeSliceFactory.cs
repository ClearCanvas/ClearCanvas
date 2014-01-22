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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Factory class for creating <see cref="VolumeSlice"/> instances representing 2-dimensional slices of a <see cref="Volume"/>.
	/// </summary>
	public sealed class VolumeSliceFactory
	{
		private VolumeInterpolationMode _interpolation = VolumeInterpolationMode.Linear;
		private VolumeProjectionMode _projection = VolumeProjectionMode.Average;

		/// <summary>
		/// Gets or sets the row orientation of the output as a directional vector in patient coordinates.
		/// </summary>
		/// <remarks>
		/// <para>If not specified, the original row orientation of the source volume will be used.</para>
		/// <para>In DICOM, this parameter is equivalent to the row component of the Image Position (Patient).</para>
		/// </remarks>
		public Vector3D RowOrientationPatient { get; set; }

		/// <summary>
		/// Gets or sets the column orientation of the output as a directional vector in patient coordinates.
		/// </summary>
		/// <remarks>
		/// <para>If not specified, the original column orientation of the source volume will be used.</para>
		/// <para>In DICOM, this parameter is equivalent to the column component of the Image Position (Patient).</para>
		/// </remarks>
		public Vector3D ColumnOrientationPatient { get; set; }

		/// <summary>
		/// Gets or sets the stack orientation of the output slices as a directional vector in patient coordinates.
		/// </summary>
		/// <remarks>
		/// <para>In general, the stack orientation does not need to be specified, and the direction orthogonal to both the slice plane will be used.</para>
		/// <para>In DICOM, this parameter is equivalent to the vector formed by the difference of the Image Position (Patient) of two consecutive images.</para>
		/// </remarks>
		public Vector3D StackOrientationPatient { get; set; }

		/// <summary>
		/// Gets or sets the spacing, in patient units, between consecutive rows of the output.
		/// </summary>
		/// <remarks>
		/// <para>If not specified, a suitable isotropic spacing (i.e. <see cref="RowSpacing"/> will be the same as <see cref="ColumnSpacing"/>) will be derived based on the source volume spacing.</para>
		/// <para>In DICOM, this parameter is equivalent to the row component of the Pixel Spacing.</para>
		/// </remarks>
		public float? RowSpacing { get; set; }

		/// <summary>
		/// Gets or sets the spacing, in patient units, between consecutive columns of the output.
		/// </summary>
		/// <remarks>
		/// <para>If not specified, a suitable isotropic spacing (i.e. <see cref="ColumnSpacing"/> will be the same as <see cref="RowSpacing"/>) will be derived based on the source volume spacing.</para>
		/// <para>In DICOM, this parameter is equivalent to the column component of the Pixel Spacing.</para>
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

		/// <summary>
		/// Gets or sets the projection mode of the output.
		/// </summary>
		/// <remarks>
		/// <para>The default value is <see cref="VolumeProjectionMode.Average"/>.</para>
		/// </remarks>
		public VolumeProjectionMode Projection
		{
			get { return _projection; }
			set { _projection = value; }
		}

		/// <summary>
		/// Creates a <see cref="VolumeSlice"/> representing the slice at the specified position in a <see cref="Volume"/>.
		/// </summary>
		/// <param name="volumeReference">A <see cref="IVolumeReference"/> for the <see cref="Volume"/>.</param>
		/// <param name="position">The position, in patient coordinates, of the slice to be created.</param>
		/// <returns>A <see cref="VolumeSlice"/> instance representing the requested slice.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="volumeReference"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="position"/> is null.</exception>
		public VolumeSlice CreateSlice(IVolumeReference volumeReference, Vector3D position)
		{
			Platform.CheckForNullReference(volumeReference, "volumeReference");
			Platform.CheckForNullReference(position, "position");

			return CreateSlicesCore(volumeReference, position, null, 1).Single();
		}

		/// <summary>
		/// Creates <see cref="VolumeSlice"/>s representing slices in a <see cref="Volume"/>.
		/// </summary>
		/// <param name="volumeReference">A <see cref="IVolumeReference"/> for the <see cref="Volume"/>.</param>
		/// <returns>A list of <see cref="VolumeSlice"/> instances representing the slices.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="volumeReference"/> is null.</exception>
		public IList<VolumeSlice> CreateSlices(IVolumeReference volumeReference)
		{
			Platform.CheckForNullReference(volumeReference, "volumeReference");

			return CreateSlicesCore(volumeReference, null, null, 0).ToList();
		}

		/// <summary>
		/// Creates <see cref="VolumeSlice"/>s representing slices around a specified position in a <see cref="Volume"/>.
		/// </summary>
		/// <remarks>
		/// If extent of the slices is not fixed in any way (e.g. via specifying <see cref="Columns"/>, <see cref="SliceWidth"/>, etc.),
		/// then <paramref name="position"/> is effectively ignored and the slices will cover the entirety of the volume.
		/// </remarks>
		/// <param name="volumeReference">A <see cref="IVolumeReference"/> for the <see cref="Volume"/>.</param>
		/// <param name="position">Reference position, in patient coordinates, around which slices will be created.</param>
		/// <returns>A list of <see cref="VolumeSlice"/> instances representing the slices.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="volumeReference"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="position"/> is null.</exception>
		public IList<VolumeSlice> CreateSlices(IVolumeReference volumeReference, Vector3D position)
		{
			Platform.CheckForNullReference(volumeReference, "volumeReference");
			Platform.CheckForNullReference(position, "position");

			return CreateSlicesCore(volumeReference, position, null, 0).ToList();
		}

		/// <summary>
		/// Creates <see cref="VolumeSlice"/>s representing a number of slices starting from the specified position in a <see cref="Volume"/>.
		/// </summary>
		/// <param name="volumeReference">A <see cref="IVolumeReference"/> for the <see cref="Volume"/>.</param>
		/// <param name="startPosition">Position, in patient coordinates, of the first slice.</param>
		/// <param name="count">The number of slices to be created.</param>
		/// <returns>A list of <see cref="VolumeSlice"/> instances representing the slices.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="volumeReference"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="startPosition"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="count"/> is less than or equal to zero.</exception>
		public IList<VolumeSlice> CreateSlices(IVolumeReference volumeReference, Vector3D startPosition, int count)
		{
			Platform.CheckForNullReference(volumeReference, "volumeReference");
			Platform.CheckForNullReference(startPosition, "startPosition");
			Platform.CheckPositive(count, "count");

			return CreateSlicesCore(volumeReference, startPosition, null, count).ToList();
		}

		/// <summary>
		/// Creates <see cref="VolumeSlice"/>s representing slices between two positions in a <see cref="Volume"/>.
		/// </summary>
		/// <param name="volumeReference">A <see cref="IVolumeReference"/> for the <see cref="Volume"/>.</param>
		/// <param name="startPosition">Position, in patient coordinates, of the first slice.</param>
		/// <param name="endPosition">Position, in patient coordinates, of the last slice.</param>
		/// <returns>A list of <see cref="VolumeSlice"/> instances representing the slices.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="volumeReference"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="startPosition"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="endPosition"/> is null.</exception>
		public IList<VolumeSlice> CreateSlices(IVolumeReference volumeReference, Vector3D startPosition, Vector3D endPosition)
		{
			Platform.CheckForNullReference(volumeReference, "volumeReference");
			Platform.CheckForNullReference(startPosition, "startPosition");
			Platform.CheckForNullReference(endPosition, "endPosition");

			return CreateSlicesCore(volumeReference, startPosition, endPosition, 0).ToList();
		}

		private IEnumerable<VolumeSlice> CreateSlicesCore(IVolumeReference volumeReference, Vector3D startPosition, Vector3D endPosition, int count)
		{
			// get the axes of the output plane and its normal in patient coordinates - these are the axes of the slicer frame
			var slicerAxisX = (RowOrientationPatient ?? volumeReference.VolumeOrientationPatientX).Normalize();
			var slicerAxisY = (ColumnOrientationPatient ?? volumeReference.VolumeOrientationPatientY).Normalize();
			var slicerAxisZ = (StackOrientationPatient ?? slicerAxisX.Cross(slicerAxisY)).Normalize();

			// get the pixel spacing (defaults to isotropic spacing based on smallest volume spacing dimension)
			var pixelSpacing = GetPixelSpacing(volumeReference);

			// get the spacing between slices (defaults to smallest volume spacing dimension)
			var sliceSpacing = GetSliceSpacing(volumeReference, slicerAxisZ);

			// get the thickness of each slice (defaults to slice spacing)
			var sliceThickness = SliceThickness ?? sliceSpacing;

			// get the ideal subsampling for the slice thickness
			var sliceSubsamples = Math.Max(1, (int) (sliceThickness/GetIdealSliceSpacing(volumeReference, slicerAxisZ) + 0.5));

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
			var args = new VolumeSliceArgs(sliceRows, sliceColumns, pixelSpacing.Height, pixelSpacing.Width, slicerAxisX, slicerAxisY, sliceThickness, sliceSubsamples, Interpolation, Projection);

			// compute the image position patient for each output slice and create the slices
			return GetSlicePositions(slicerOrigin, slicerAxisX, slicerAxisY, slicerAxisZ, stackDepth, sliceRows, sliceColumns, pixelSpacing, sliceSpacing, sliceThickness, startPosition, endPosition, count)
				.Select(p => new VolumeSlice(volumeReference.Clone(), true, args, p, sliceSpacing));
		}

		private IEnumerable<Vector3D> GetSlicePositions(Vector3D slicerOrigin, Vector3D slicerAxisX, Vector3D slicerAxisY, Vector3D slicerAxisZ, float stackDepth, int sliceRows, int sliceColumns, SizeF pixelSpacing, float sliceSpacing, float sliceThickness, Vector3D startPosition, Vector3D endPosition, int count)
		{
			// compute the increment in position between consecutive slices
			var slicePositionIncrement = slicerAxisZ*sliceSpacing;

			// get the number of slices in the output when slicing the entire volume
			var sliceCount = (int) (Math.Abs(1.0*stackDepth/sliceSpacing) + 0.5);

			// compute the position of the first slice when slicing the entire volume
			var firstSlicePosition = slicerOrigin;

			// if a start position (and, optionally, end position or total count) is provided, we adjust the slice position and count accordingly
			if (startPosition != null)
			{
				// NOTE: The overall logic here is that, even if a position is given for the first slice, it does not make sense to arbitrarily
				// reslice from that point to the edge of the volume, because the other half of the volume will not be in the output.
				// Therefore, we only make use of the individual components of the position for which the client code has provided fixed bounds
				// because then the client code is explicitly requested a subrange of the volume in that dimension, and any auto-calculated
				// bounds will still include the entire volume in that dimension.
				// e.g. we use the X component of the position if the output width is fixed, and the output stack will be horizontally centred on this position

				// get the requested start position in the slicer frame
				var slicerStart = startPosition - slicerOrigin;

				// if the width of the output is fixed, we horizontally centre the slices on the location provided
				if (Columns.HasValue || SliceWidth.HasValue)
				{
					// projecting the start position on to the X axis then subtracting half the width of the image gives us an offset along X to get desired starting location
					var offsetX = slicerAxisX.Dot(slicerStart) - sliceColumns*pixelSpacing.Width/2f;
					firstSlicePosition += offsetX*slicerAxisX;
				}

				// if the height of the output is fixed, we vertically centre the slices on the location provided
				if (Rows.HasValue || SliceHeight.HasValue)
				{
					// projecting the start position on to the Y axis then subtracting half the width of the image gives us an offset along Y to get desired starting location
					var offsetY = slicerAxisY.Dot(slicerStart) - sliceRows*pixelSpacing.Height/2f;
					firstSlicePosition += offsetY*slicerAxisY;
				}

				// if end position or total slice count is provided, we will slice the selected range only
				if (endPosition != null)
				{
					// projecting the start position on to the Z axis gives us an offset along Z to get desired starting location
					var offsetZ = slicerAxisZ.Dot(slicerStart);
					firstSlicePosition += offsetZ*slicerAxisZ;

					// recompute the slice count based on the magnitude of the Z component delta between the end and start positions
					var slicerStop = endPosition - slicerOrigin;
					var stackRange = slicerAxisZ.Dot(slicerStop) - offsetZ;
					sliceCount = (int) (Math.Abs(1.0*stackRange/sliceSpacing) + 0.5);
				}
				else if (count > 0)
				{
					// projecting the start position on to the Z axis gives us an offset along Z to get desired starting location
					var offsetZ = slicerAxisZ.Dot(slicerStart);
					firstSlicePosition += offsetZ*slicerAxisZ;

					// the slice count is exactly as provided
					sliceCount = count;
				}
			}

			// compute the positions for the requested slices
			return Enumerable.Range(0, Math.Max(1, sliceCount)).Select(n => firstSlicePosition + n*slicePositionIncrement);
		}

		private float GetSliceSpacing(IVolumeHeader volumeHeader, Vector3D slicerAxisZ)
		{
			return SliceSpacing ?? (SliceThickness.HasValue ? SliceThickness.Value : GetIdealSliceSpacing(volumeHeader, slicerAxisZ));
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

		private static float GetIdealSliceSpacing(IVolumeHeader volumeHeader, Vector3D unitSpacingAxis)
		{
			// the ideal spacing is simply the diagonal of the voxel projected on to the spacing axis
			// any larger than this value, and it becomes possible for an entire voxel to fit in between two consecutive output locations (i.e. missed for interpolation)
			// any smaller than this value, and some voxels will have two or more output locations within their bounds
			return Math.Abs(unitSpacingAxis.Dot(volumeHeader.RotateToPatientOrientation(volumeHeader.VoxelSpacing)));
		}

		private static float GetMinimumComponent(Vector3D vector3D)
		{
			return vector3D.X < vector3D.Y ? Math.Min(vector3D.X, vector3D.Z) : Math.Min(vector3D.Y, vector3D.Z);
		}
	}
}