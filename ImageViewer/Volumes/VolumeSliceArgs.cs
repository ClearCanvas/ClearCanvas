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
		private readonly float _rowSpacing;
		private readonly float _columnSpacing;
		private readonly VolumeInterpolationMode _interpolation;

		public VolumeSliceArgs(int rows, int columns,
		                       float rowSpacing, float columnSpacing,
		                       Vector3D rowOrientationPatient,
		                       Vector3D columnOrientationPatient,
		                       float sliceThickness,
		                       VolumeInterpolationMode interpolation)
		{
			_rows = rows;
			_columns = columns;
			_rowSpacing = rowSpacing;
			_columnSpacing = columnSpacing;
			_rowOrientationPatient = rowOrientationPatient;
			_columnOrientationPatient = columnOrientationPatient;
			_sliceThickness = sliceThickness;
			_interpolation = interpolation;
		}

		public Vector3D RowOrientationPatient
		{
			get { return _rowOrientationPatient; }
		}

		public Vector3D ColumnOrientationPatient
		{
			get { return _columnOrientationPatient; }
		}

		public int Columns
		{
			get { return _columns; }
		}

		public int Rows
		{
			get { return _rows; }
		}

		public float SliceThickness
		{
			get { return _sliceThickness; }
		}

		public float RowSpacing
		{
			get { return _rowSpacing; }
		}

		public float ColumnSpacing
		{
			get { return _columnSpacing; }
		}

		public VolumeInterpolationMode Interpolation
		{
			get { return _interpolation; }
		}
	}
}