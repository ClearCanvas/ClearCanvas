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
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Volumes;
using vtk;

namespace ClearCanvas.ImageViewer.Vtk
{
	/// <summary>
	/// Extension methods for working with <see cref="Volume"/>s.
	/// </summary>
	public static class VtkVolumeMethods
	{
		/// <summary>
		/// Creates a <see cref="VtkImageDataHandle"/> wrapping the data in a <see cref="Volume"/>.
		/// </summary>
		public static VtkImageDataHandle CreateVtkVolumeHandle(this Volume volume)
		{
			Platform.CheckForNullReference(volume, "volume");

			// The volume must be pinned before creating the vtk object because it stores a pointer to the array.
			var volumeArrayPinned = GCHandle.Alloc(volume.Array, GCHandleType.Pinned);
			try
			{
				var vtkVolume = CreateVtkVolume(volume);
				return new VtkImageDataHandle(vtkVolume, volumeArrayPinned);
			}
			catch (Exception)
			{
				// if an exception happens during CreateVtkVolume, ensure the handle gets freed!
				volumeArrayPinned.Free();
				throw;
			}
		}

		private static vtkImageData CreateVtkVolume(Volume volume)
		{
			var vtkVolume = new vtkImageData();
			vtkVolume.RegisterVtkErrorEvents();
			vtkVolume.SetDimensions(volume.ArrayDimensions.Width, volume.ArrayDimensions.Height, volume.ArrayDimensions.Depth);
			vtkVolume.SetOrigin(0, 0, 0);
			vtkVolume.SetSpacing(volume.VoxelSpacing.X, volume.VoxelSpacing.Y, volume.VoxelSpacing.Z);

			if (volume.BitsPerVoxel == 16)
			{
				if (!volume.Signed)
				{
					using (var array = new vtkUnsignedShortArray())
					{
						array.SetArray((ushort[]) volume.Array, (VtkIdType) volume.ArrayLength, 1);

						vtkVolume.SetScalarTypeToUnsignedShort();
						vtkVolume.GetPointData().SetScalars(array);
					}
				}
				else
				{
					using (var array = new vtkShortArray())
					{
						array.SetArray((short[]) volume.Array, (VtkIdType) volume.ArrayLength, 1);

						vtkVolume.SetScalarTypeToShort();
						vtkVolume.GetPointData().SetScalars(array);
					}
				}
			}
			else if (volume.BitsPerVoxel == 8)
			{
				if (!volume.Signed)
				{
					using (var array = new vtkUnsignedCharArray())
					{
						array.SetArray((byte[]) volume.Array, (VtkIdType) volume.ArrayLength, 1);

						vtkVolume.SetScalarTypeToUnsignedChar();
						vtkVolume.GetPointData().SetScalars(array);
					}
				}
				else
				{
					using (var array = new vtkSignedCharArray())
					{
						array.SetArray((sbyte[]) volume.Array, (VtkIdType) volume.ArrayLength, 1);

						vtkVolume.SetScalarTypeToSignedChar();
						vtkVolume.GetPointData().SetScalars(array);
					}
				}
			}
			else
			{
				throw new NotSupportedException("Unsupported volume scalar type.");
			}

			// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
			vtkVolume.UpdateInformation();

			return vtkVolume;
		}
	}
}