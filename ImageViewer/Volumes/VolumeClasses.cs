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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Represents a 3-dimensional raster volume with unsigned 16-bit values.
	/// </summary>
	public sealed class U16Volume : Volume
	{
		private ushort[] _array;

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public U16Volume(ushort[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue)
			: this(array, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, 1, 0, RescaleUnits.None) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public U16Volume(ushort[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue)
			: this(array, new VolumeHeaderData(dicomAttributeModel, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, 16, 16, false, paddingValue, 1, 0, RescaleUnits.None), null, null) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public U16Volume(ushort[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue, double rescaleSlope, double rescaleIntercept, RescaleUnits rescaleUnits)
			: this(array, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, rescaleSlope, rescaleIntercept, rescaleUnits) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public U16Volume(ushort[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue, double rescaleSlope, double rescaleIntercept, RescaleUnits rescaleUnits)
			: this(array, new VolumeHeaderData(dicomAttributeModel, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, 16, 16, false, paddingValue, rescaleSlope, rescaleIntercept, rescaleUnits), null, null) {}

		internal U16Volume(ushort[] array, VolumeHeaderData volumeHeaderData, int? minVolumeValue, int? maxVolumeValue)
			: base(volumeHeaderData, minVolumeValue, maxVolumeValue)
		{
			_array = array;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_array = null;
		}

		/// <summary>
		/// Gets the array containing the volume data.
		/// </summary>
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

		protected override unsafe void GetMinMaxVolumeValue(out int minValue, out int maxValue)
		{
			fixed (ushort* pArray = _array)
			{
				var ptr = pArray;
				var len = _array.Length;
				var max = ushort.MinValue;
				var min = ushort.MaxValue;
				for (var i = 0; i < len; ++i)
				{
					var v = *(ptr++);
					if (max < v) max = v;
					if (v < min) min = v;
				}
				minValue = min;
				maxValue = max;
			}
		}
	}

	/// <summary>
	/// Represents a 3-dimensional raster volume with signed 16-bit values.
	/// </summary>
	public sealed class S16Volume : Volume
	{
		private short[] _array;

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public S16Volume(short[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue)
			: this(array, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, 1, 0, RescaleUnits.None) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public S16Volume(short[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue)
			: this(array, new VolumeHeaderData(dicomAttributeModel, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, 16, 16, true, paddingValue, 1, 0, RescaleUnits.None), null, null) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public S16Volume(short[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue, double rescaleSlope, double rescaleIntercept, RescaleUnits rescaleUnits)
			: this(array, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, rescaleSlope, rescaleIntercept, rescaleUnits) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public S16Volume(short[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue, double rescaleSlope, double rescaleIntercept, RescaleUnits rescaleUnits)
			: this(array, new VolumeHeaderData(dicomAttributeModel, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, 16, 16, true, paddingValue, rescaleSlope, rescaleIntercept, rescaleUnits), null, null) {}

		internal S16Volume(short[] array, VolumeHeaderData volumeHeaderData, int? minVolumeValue, int? maxVolumeValue)
			: base(volumeHeaderData, minVolumeValue, maxVolumeValue)
		{
			_array = array;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_array = null;
		}

		/// <summary>
		/// Gets the array containing the volume data.
		/// </summary>
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

		protected override unsafe void GetMinMaxVolumeValue(out int minValue, out int maxValue)
		{
			fixed (short* pArray = _array)
			{
				var ptr = pArray;
				var len = _array.Length;
				var max = short.MinValue;
				var min = short.MaxValue;
				for (var i = 0; i < len; ++i)
				{
					var v = *(ptr++);
					if (max < v) max = v;
					if (v < min) min = v;
				}
				minValue = min;
				maxValue = max;
			}
		}
	}

	/// <summary>
	/// Represents a 3-dimensional raster volume with unsigned 8-bit values.
	/// </summary>
	public sealed class U8Volume : Volume
	{
		private byte[] _array;

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public U8Volume(byte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue)
			: this(array, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, 1, 0, RescaleUnits.None) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public U8Volume(byte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue)
			: this(array, new VolumeHeaderData(dicomAttributeModel, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, 8, 8, false, paddingValue, 1, 0, RescaleUnits.None), null, null) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public U8Volume(byte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue, double rescaleSlope, double rescaleIntercept, RescaleUnits rescaleUnits)
			: this(array, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, rescaleSlope, rescaleIntercept, rescaleUnits) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public U8Volume(byte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue, double rescaleSlope, double rescaleIntercept, RescaleUnits rescaleUnits)
			: this(array, new VolumeHeaderData(dicomAttributeModel, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, 8, 8, false, paddingValue, rescaleSlope, rescaleIntercept, rescaleUnits), null, null) {}

		internal U8Volume(byte[] array, VolumeHeaderData volumeHeaderData, int? minVolumeValue, int? maxVolumeValue)
			: base(volumeHeaderData, minVolumeValue, maxVolumeValue)
		{
			_array = array;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_array = null;
		}

		/// <summary>
		/// Gets the array containing the volume data.
		/// </summary>
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

		protected override unsafe void GetMinMaxVolumeValue(out int minValue, out int maxValue)
		{
			fixed (byte* pArray = _array)
			{
				var ptr = pArray;
				var len = _array.Length;
				var max = byte.MinValue;
				var min = byte.MaxValue;
				for (var i = 0; i < len; ++i)
				{
					var v = *(ptr++);
					if (max < v) max = v;
					if (v < min) min = v;
				}
				minValue = min;
				maxValue = max;
			}
		}
	}

	/// <summary>
	/// Represents a 3-dimensional raster volume with signed 8-bit values.
	/// </summary>
	public sealed class S8Volume : Volume
	{
		private sbyte[] _array;

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public S8Volume(sbyte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue)
			: this(array, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, 1, 0, RescaleUnits.None) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public S8Volume(sbyte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue)
			: this(array, new VolumeHeaderData(dicomAttributeModel, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, 8, 8, true, paddingValue, 1, 0, RescaleUnits.None), null, null) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public S8Volume(sbyte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IDicomAttributeProvider attributeProvider, int paddingValue, double rescaleSlope, double rescaleIntercept, RescaleUnits rescaleUnits)
			: this(array, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, new[] {attributeProvider}, paddingValue, rescaleSlope, rescaleIntercept, rescaleUnits) {}

		/// <summary>
		/// Initializes the <see cref="Volume"/> using the specified volume data.
		/// </summary>
		/// <remarks>
		/// Consider using <see cref="Volume.Create(IDisplaySet)"/> or one of its overloads to automatically construct and fill a <see cref="Volume"/> of the appropriate type.
		/// </remarks>
		public S8Volume(sbyte[] array, Size3D arrayDimensions, Vector3D voxelSpacing, Vector3D volumePositionPatient, Matrix3D volumeOrientationPatient, IList<IDicomAttributeProvider> dicomAttributeModel, int paddingValue, double rescaleSlope, double rescaleIntercept, RescaleUnits rescaleUnits)
			: this(array, new VolumeHeaderData(dicomAttributeModel, arrayDimensions, voxelSpacing, volumePositionPatient, volumeOrientationPatient, 8, 8, true, paddingValue, rescaleSlope, rescaleIntercept, rescaleUnits), null, null) {}

		internal S8Volume(sbyte[] array, VolumeHeaderData volumeHeaderData, int? minVolumeValue, int? maxVolumeValue)
			: base(volumeHeaderData, minVolumeValue, maxVolumeValue)
		{
			_array = array;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_array = null;
		}

		/// <summary>
		/// Gets the array containing the volume data.
		/// </summary>
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

		protected override unsafe void GetMinMaxVolumeValue(out int minValue, out int maxValue)
		{
			fixed (sbyte* pArray = _array)
			{
				var ptr = pArray;
				var len = _array.Length;
				var max = sbyte.MinValue;
				var min = sbyte.MaxValue;
				for (var i = 0; i < len; ++i)
				{
					var v = *(ptr++);
					if (max < v) max = v;
					if (v < min) min = v;
				}
				minValue = min;
				maxValue = max;
			}
		}
	}
}