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

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volumes
{
	public sealed class VolumeSliceArgs
	{
		private readonly Vector3D _rowOrientationPatient;
		private readonly Vector3D _columnOrientationPatient;
		private readonly int _columns;
		private readonly int _rows;
		private readonly float _sliceThickness;
		private readonly int _subsamples;
		private readonly float _rowSpacing;
		private readonly float _columnSpacing;
		private readonly VolumeInterpolationMode _interpolation;
		private readonly VolumeProjectionMode _projection;

		public VolumeSliceArgs(int rows, int columns,
		                       float rowSpacing, float columnSpacing,
		                       Vector3D rowOrientationPatient,
		                       Vector3D columnOrientationPatient,
		                       float sliceThickness,
		                       VolumeInterpolationMode interpolation)
			: this(rows, columns, rowSpacing, columnSpacing, rowOrientationPatient, columnOrientationPatient, sliceThickness, 0, interpolation, VolumeProjectionMode.Average) {}

		public VolumeSliceArgs(int rows, int columns,
		                       float rowSpacing, float columnSpacing,
		                       Vector3D rowOrientationPatient,
		                       Vector3D columnOrientationPatient,
		                       float sliceThickness, int subsamples,
		                       VolumeInterpolationMode interpolation,
		                       VolumeProjectionMode projection)
		{
			Platform.CheckPositive(rows, "rows");
			Platform.CheckPositive(columns, "columns");
			Platform.CheckPositive(rowSpacing, "rowSpacing");
			Platform.CheckPositive(columnSpacing, "columnSpacing");
			Platform.CheckPositive(sliceThickness, "sliceThickness");
			Platform.CheckForNullReference(rowOrientationPatient, "rowOrientationPatient");
			Platform.CheckForNullReference(columnOrientationPatient, "columnOrientationPatient");

			_rows = rows;
			_columns = columns;
			_rowSpacing = rowSpacing;
			_columnSpacing = columnSpacing;
			_rowOrientationPatient = rowOrientationPatient.Normalize();
			_columnOrientationPatient = columnOrientationPatient.Normalize();
			_sliceThickness = sliceThickness;
			_subsamples = subsamples;
			_interpolation = interpolation;
			_projection = projection;
		}

		/// <summary>
		/// Gets the row orientation unit vector in patient coordinates.
		/// </summary>
		public Vector3D RowOrientationPatient
		{
			get { return _rowOrientationPatient; }
		}

		/// <summary>
		/// Gets the column orientation unit vector in patient coordinates.
		/// </summary>
		public Vector3D ColumnOrientationPatient
		{
			get { return _columnOrientationPatient; }
		}

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
		public int Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Gets the number of rows.
		/// </summary>
		public int Rows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the thickness, in patient units, represented by each slice.
		/// </summary>
		public float SliceThickness
		{
			get { return _sliceThickness; }
		}

		/// <summary>
		/// Gets the number of subsamples to take during reslicing.
		/// </summary>
		public int Subsamples
		{
			get { return _subsamples; }
		}

		/// <summary>
		/// Gets the row spacing.
		/// </summary>
		public float RowSpacing
		{
			get { return _rowSpacing; }
		}

		/// <summary>
		/// Gets the column spacing.
		/// </summary>
		public float ColumnSpacing
		{
			get { return _columnSpacing; }
		}

		/// <summary>
		/// Gets the interpolation method to use when reslicing.
		/// </summary>
		public VolumeInterpolationMode Interpolation
		{
			get { return _interpolation; }
		}

		/// <summary>
		/// Gets the projection method to use when reslicing with subsamples.
		/// </summary>
		public VolumeProjectionMode Projection
		{
			get { return _projection; }
		}
	}
}