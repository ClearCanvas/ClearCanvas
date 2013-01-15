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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	partial class VolumeSlicerParams
	{
		private class ReadOnlyVolumeSlicerParams : IVolumeSlicerParams
		{
			private const string _readOnly = "Cannot modify a read-only IVolumeSlicerParams.";

			private readonly string _description;
			private readonly Matrix _slicingPlaneRotation;
			private readonly Vector3D _sliceThroughPointPatient;
			private readonly VolumeSlicerInterpolationMode _interpolationMode;
			private readonly float _sliceExtentXMillimeters;
			private readonly float _sliceExtentYMillimeters;
			private readonly float _sliceSpacing;

			public ReadOnlyVolumeSlicerParams(IVolumeSlicerParams source)
			{
				if (source.SlicingPlaneRotation != null)
					_slicingPlaneRotation = new Matrix(source.SlicingPlaneRotation);
				if (source.SliceThroughPointPatient != null)
					_sliceThroughPointPatient = new Vector3D(source.SliceThroughPointPatient);
				_description = source.Description;
				_interpolationMode = source.InterpolationMode;
				_sliceExtentXMillimeters = source.SliceExtentXMillimeters;
				_sliceExtentYMillimeters = source.SliceExtentYMillimeters;
				_sliceSpacing = source.SliceSpacing;
			}

			public string Description
			{
				get { return _description; }
				set { throw new NotSupportedException(_readOnly); }
			}

			public Matrix SlicingPlaneRotation
			{
				get
				{
					if (_slicingPlaneRotation == null)
						return null;
					return new Matrix(_slicingPlaneRotation);
				}
				set { throw new NotSupportedException(_readOnly); }
			}

			public Vector3D SliceThroughPointPatient
			{
				get
				{
					if (_sliceThroughPointPatient == null)
						return null;
					return new Vector3D(_sliceThroughPointPatient);
				}
				set { throw new NotSupportedException(_readOnly); }
			}

			public VolumeSlicerInterpolationMode InterpolationMode
			{
				get { return _interpolationMode; }
				set { throw new NotSupportedException(_readOnly); }
			}

			public float SliceExtentXMillimeters
			{
				get { return _sliceExtentXMillimeters; }
				set { throw new NotSupportedException(_readOnly); }
			}

			public float SliceExtentYMillimeters
			{
				get { return _sliceExtentYMillimeters; }
				set { throw new NotSupportedException(_readOnly); }
			}

			public float SliceSpacing
			{
				get { return _sliceSpacing; }
				set { throw new NotSupportedException(_readOnly); }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}
		}
	}
}