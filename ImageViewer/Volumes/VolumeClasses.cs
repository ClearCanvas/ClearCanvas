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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Represents a 3-dimensional raster volume with unsigned 16-bit values.
	/// </summary>
	public sealed class U16Volume : Volume
	{
		private ushort[] _array;

		public U16Volume(ushort[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue, string sourceSeriesInstanceUid)
			: this(array, arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, sourceSeriesInstanceUid) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public U16Volume(ushort[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient,
		                 Matrix volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue, string sourceSeriesInstanceUid)
			: this(array, arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient,
			       VolumeSopDataSourcePrototype.Create(dicomAttributeModel, 16, 16, false), paddingValue, sourceSeriesInstanceUid, 0, 0) {}

		internal U16Volume(ushort[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, VolumeSopDataSourcePrototype dataSourcePrototype, int paddingValue, string sourceSeriesInstanceUid, int minVolumeValue, int maxVolumeValue)
			: base(arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, dataSourcePrototype, paddingValue, sourceSeriesInstanceUid, minVolumeValue, maxVolumeValue)
		{
			_array = array;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_array = null;
		}

		public ushort[] ArrayData
		{
			get { return _array; }
		}

		public override Array Array
		{
			get { return _array; }
		}

		public override bool Signed
		{
			get { return false; }
		}

		public override int BitsPerVoxel
		{
			get { return 16; }
		}

		protected override int GetArrayValue(int i)
		{
			return _array[i];
		}
	}

	/// <summary>
	/// Represents a 3-dimensional raster volume with signed 16-bit values.
	/// </summary>
	public sealed class S16Volume : Volume
	{
		private short[] _array;

		public S16Volume(short[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue, string sourceSeriesInstanceUid)
			: this(array, arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, sourceSeriesInstanceUid) {}

		/// <summary>
		/// Constructs a <see cref="Volume"/> using a volume data array of signed 16-bit words.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(ClearCanvas.ImageViewer.IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public S16Volume(short[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue, string sourceSeriesInstanceUid)
			: this(array, arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, VolumeSopDataSourcePrototype.Create(dicomAttributeModel, 16, 16, true), paddingValue, sourceSeriesInstanceUid, 0, 0) {}

		internal S16Volume(short[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, VolumeSopDataSourcePrototype dataSourcePrototype, int paddingValue, string sourceSeriesInstanceUid, int minVolumeValue, int maxVolumeValue)
			: base(arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, dataSourcePrototype, paddingValue, sourceSeriesInstanceUid, minVolumeValue, maxVolumeValue)
		{
			_array = array;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_array = null;
		}

		public short[] ArrayData
		{
			get { return _array; }
		}

		public override Array Array
		{
			get { return _array; }
		}

		public override bool Signed
		{
			get { return true; }
		}

		public override int BitsPerVoxel
		{
			get { return 16; }
		}

		protected override int GetArrayValue(int i)
		{
			return _array[i];
		}
	}

	/// <summary>
	/// Represents a 3-dimensional raster volume with unsigned 8-bit values.
	/// </summary>
	public sealed class U8Volume : Volume
	{
		private byte[] _array;

		public U8Volume(byte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue, string sourceSeriesInstanceUid)
			: this(array, arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, sourceSeriesInstanceUid) {}

		/// <summary>
		/// Constructs a <see cref="Volume"/> using a volume data array of unsigned 8-bit words.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public U8Volume(byte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue, string sourceSeriesInstanceUid)
			: this(array, arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, VolumeSopDataSourcePrototype.Create(dicomAttributeModel, 8, 8, false), paddingValue, sourceSeriesInstanceUid, 0, 0) {}

		internal U8Volume(byte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, VolumeSopDataSourcePrototype dataSourcePrototype, int paddingValue, string sourceSeriesInstanceUid, int minVolumeValue, int maxVolumeValue)
			: base(arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, dataSourcePrototype, paddingValue, sourceSeriesInstanceUid, minVolumeValue, maxVolumeValue)
		{
			_array = array;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_array = null;
		}

		public byte[] ArrayData
		{
			get { return _array; }
		}

		public override Array Array
		{
			get { return _array; }
		}

		public override bool Signed
		{
			get { return false; }
		}

		public override int BitsPerVoxel
		{
			get { return 8; }
		}

		protected override int GetArrayValue(int i)
		{
			return _array[i];
		}
	}

	/// <summary>
	/// Represents a 3-dimensional raster volume with signed 8-bit values.
	/// </summary>
	public sealed class S8Volume : Volume
	{
		private sbyte[] _array;

		public S8Volume(sbyte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue, string sourceSeriesInstanceUid)
			: this(array, arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, sourceSeriesInstanceUid) {}

		/// <summary>
		/// Constructs a <see cref="Volume"/> using a volume data array of signed 8-bit words.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public S8Volume(sbyte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue, string sourceSeriesInstanceUid)
			: this(array, arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, VolumeSopDataSourcePrototype.Create(dicomAttributeModel, 8, 8, true), paddingValue, sourceSeriesInstanceUid, 0, 0) {}

		internal S8Volume(sbyte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumeOriginPatient, Matrix volumeOrientationPatient, VolumeSopDataSourcePrototype dataSourcePrototype, int paddingValue, string sourceSeriesInstanceUid, int minVolumeValue, int maxVolumeValue)
			: base(arrayDimensions, voxelSpacing, volumeOriginPatient, volumeOrientationPatient, dataSourcePrototype, paddingValue, sourceSeriesInstanceUid, minVolumeValue, maxVolumeValue)
		{
			_array = array;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_array = null;
		}

		public sbyte[] ArrayData
		{
			get { return _array; }
		}

		public override Array Array
		{
			get { return _array; }
		}

		public override bool Signed
		{
			get { return true; }
		}

		public override int BitsPerVoxel
		{
			get { return 8; }
		}

		protected override int GetArrayValue(int i)
		{
			return _array[i];
		}
	}
}