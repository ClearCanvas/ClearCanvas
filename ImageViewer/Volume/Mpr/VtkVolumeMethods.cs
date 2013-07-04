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

using System.Runtime.InteropServices;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	internal static class VtkVolumeMethods
	{
		public static VtkVolumeHandle CreateVtkVolumeHandle(this Volumes.Volume @this)
		{
			// Technically, the volume should be pinned before creating the "volume" because it stores a pointer to the array.
			var volumeArrayPinned = GCHandle.Alloc(@this.Array, GCHandleType.Pinned);
			var vtkVolume = CreateVtkVolume(@this);
			return new VtkVolumeHandle(vtkVolume, volumeArrayPinned);
		}

		private static vtkImageData CreateVtkVolume(Volumes.Volume @this)
		{
			vtkImageData vtkVolume = new vtkImageData();

			VtkHelper.RegisterVtkErrorEvents(vtkVolume);

			vtkVolume.SetDimensions(@this.ArrayDimensions.Width, @this.ArrayDimensions.Height, @this.ArrayDimensions.Depth);
			vtkVolume.SetOrigin(0, 0, 0);
			vtkVolume.SetSpacing(@this.VoxelSpacing.X, @this.VoxelSpacing.Y, @this.VoxelSpacing.Z);

			if (!@this.Signed)
			{
				using (vtkUnsignedShortArray array = VtkHelper.ConvertToVtkUnsignedShortArray(@this.Array))
				{
					vtkVolume.SetScalarTypeToUnsignedShort();
					vtkVolume.GetPointData().SetScalars(array);

					// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
					vtkVolume.UpdateInformation();
				}
			}
			else
			{
				using (var array = VtkHelper.ConvertToVtkShortArray(@this.Array))
				{
					vtkVolume.SetScalarTypeToShort();
					vtkVolume.GetPointData().SetScalars(array);

					// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
					vtkVolume.UpdateInformation();
				}
			}

			return vtkVolume;
		}
	}
}