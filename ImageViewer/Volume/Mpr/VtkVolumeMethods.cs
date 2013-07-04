using System;
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
			var volumeArrayPinned = GCHandle.Alloc(!@this.Signed ? (Array) @this.DataU16 : @this.DataS16, GCHandleType.Pinned);
			var vtkVolume = CreateVtkVolume(@this);
			return new VtkVolumeHandle(vtkVolume, volumeArrayPinned);
		}

		private static vtkImageData CreateVtkVolume(Volumes.Volume @this)
		{
			vtkImageData vtkVolume = new vtkImageData();

			VtkHelper.RegisterVtkErrorEvents(vtkVolume);

			vtkVolume.SetDimensions(@this.ArrayDimensions.Width, @this.ArrayDimensions.Height, @this.ArrayDimensions.Depth);
			vtkVolume.SetOrigin(@this.Origin.X, @this.Origin.Y, @this.Origin.Z);
			vtkVolume.SetSpacing(@this.VoxelSpacing.X, @this.VoxelSpacing.Y, @this.VoxelSpacing.Z);

			if (!@this.Signed)
			{
				using (vtkUnsignedShortArray array = VtkHelper.ConvertToVtkUnsignedShortArray(@this.DataU16))
				{
					vtkVolume.SetScalarTypeToUnsignedShort();
					vtkVolume.GetPointData().SetScalars(array);

					// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
					vtkVolume.UpdateInformation();
				}
			}
			else
			{
				using (var array = VtkHelper.ConvertToVtkShortArray(@this.DataS16))
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