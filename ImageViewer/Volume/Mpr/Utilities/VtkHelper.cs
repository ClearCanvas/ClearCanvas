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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	internal static class VtkHelper
	{
		static VtkHelper()
		{
			var fileOutputWindow = new vtkFileOutputWindow();
			fileOutputWindow.SetFileName(Path.Combine(Platform.LogDirectory, "vtk.log"));
			vtkOutputWindow.SetInstance(fileOutputWindow);
		}

		#region VTK Static Initialization Hack

		//VTK initializes some static variables (collections, actually) internally in a way that is not thread safe.
		//It seems that, at least for the code we use to do reslicing, the collections are fully initialized after 
		//the first slice is created from the first volume. Problems arise, however, if 2 slices are being created
		//simultaneously on different threads.
		//For the record, individual VTK objects are not thread-safe and should never be shared across threads.
		//This hack exists only to workaround the static collections issue. Note also, that this hack is not future-proof;
		//if we start using different parts of VTK in the future, we need to make sure this hack still works.
		private static readonly object _initializationLock = new object();
		private static volatile bool _initialized;

		internal static void StaticInitializationHack()
		{
			if (_initialized)
				return;

			lock (_initializationLock)
			{
				if (_initialized)
					return;

				//Set _initialized right away, just in case there are recursive calls on the same thread.
				_initialized = true;

				//Create one signed and one unsigned, just to make sure.
				CreateAndResliceVolume(false);
				CreateAndResliceVolume(true);
			}
		}

		private static void CreateAndResliceVolume(bool signed)
		{
			const int width = 10;
			const int height = 10;
			const int depth = 10;

			if (signed)
			{
				var data = new short[width*height*depth];
				using (var vtk = CreateVtkVolume(data, width, height, depth))
				{
					ResliceVolume(vtk);
					vtk.GetPointData().Dispose();
				}
			}
			else
			{
				var data = new ushort[width*height*depth];
				using (var vtk = CreateVtkVolume(data, width, height, depth))
				{
					ResliceVolume(vtk);
					vtk.GetPointData().Dispose();
				}
			}
		}

		private static void ResliceVolume(vtkImageData volume)
		{
			using (var reslicer = new vtkImageReslice())
			{
				RegisterVtkErrorEvents(reslicer);
				reslicer.SetInput(volume);
				reslicer.SetInformationInput(volume);

				// Must instruct reslicer to output 2D images
				reslicer.SetOutputDimensionality(2);

				// Use the volume's padding value for all pixels that are outside the volume
				reslicer.SetBackgroundLevel(0);

				// This ensures VTK obeys the real spacing, results in all VTK slices being isotropic.
				//	Effective spacing is the minimum of these three.
				reslicer.SetOutputSpacing(1.0, 1.0, 1.0);

				using (vtkMatrix4x4 resliceAxesMatrix = ConvertToVtkMatrix(new double[,] {{1, 0, 0, 0}, {0, 1, 0, 0}, {0, 0, 1, 0}, {0, 0, 0, 1}}))
				{
					reslicer.SetResliceAxes(resliceAxesMatrix);

					// Clamp the output based on the slice extent
					const int sliceExtentX = 50;
					const int sliceExtentY = 50;
					reslicer.SetOutputExtent(0, sliceExtentX - 1, 0, sliceExtentY - 1, 0, 0);

					// Set the output origin to reflect the slice through point. The slice extent is
					//	centered on the slice through point.
					// VTK output origin is derived from the center image being 0,0
					const float originX = -sliceExtentX*1f/2;
					const float originY = -sliceExtentY*1f/2;
					reslicer.SetOutputOrigin(originX, originY, 0);

					reslicer.SetInterpolationModeToLinear();

					using (vtkExecutive exec = reslicer.GetExecutive())
					{
						RegisterVtkErrorEvents(exec);
						exec.Update();
					}

					using (var output = reslicer.GetOutput())
					{
						// just to give it something to do with the output
						GC.KeepAlive(output);
					}
				}
			}
		}

		private static vtkImageData CreateVtkVolume(ushort[] data, int width, int height, int depth)
		{
			var vtkVolume = new vtkImageData();

			RegisterVtkErrorEvents(vtkVolume);

			vtkVolume.SetDimensions(width, height, depth);
			vtkVolume.SetOrigin(0, 0, 0);
			vtkVolume.SetSpacing(1.0, 1.0, 1.0);

			using (var array = new vtkUnsignedShortArray())
			{
				array.SetArray(data, (VtkIdType) data.Length, 1);

				vtkVolume.SetScalarTypeToUnsignedShort();
				vtkVolume.GetPointData().SetScalars(array);

				// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
				vtkVolume.UpdateInformation();
			}

			return vtkVolume;
		}

		private static vtkImageData CreateVtkVolume(short[] data, int width, int height, int depth)
		{
			var vtkVolume = new vtkImageData();

			RegisterVtkErrorEvents(vtkVolume);

			vtkVolume.SetDimensions(width, height, depth);
			vtkVolume.SetOrigin(0, 0, 0);
			vtkVolume.SetSpacing(1.0, 1.0, 1.0);

			using (var array = new vtkShortArray())
			{
				array.SetArray(data, (VtkIdType) data.Length, 1);

				vtkVolume.SetScalarTypeToShort();
				vtkVolume.GetPointData().SetScalars(array);

				// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
				vtkVolume.UpdateInformation();
			}

			return vtkVolume;
		}

		private static vtkMatrix4x4 ConvertToVtkMatrix(double[,] matrix)
		{
			vtkMatrix4x4 vtkMatrix = new vtkMatrix4x4();

			for (int row = 0; row < 4; row++)
				for (int column = 0; column < 4; column++)
					vtkMatrix.SetElement(column, row, matrix[row, column]);

			return vtkMatrix;
		}

		#endregion

		#region Error Handling helpers

		public static void RegisterVtkErrorEvents(vtkObject obj)
		{
			obj.AddObserver((uint) EventIds.ErrorEvent, VtkEventCallback);
			obj.AddObserver((uint) EventIds.WarningEvent, VtkEventCallback);
		}

		public static void VtkEventCallback(vtkObject vtkObj, uint eventId, object obj, IntPtr ptr)
		{
			const string unexpectedMessage = "Unexpected VTK event received.";
			const string message = "VTK Event 0x{0:x4}: {1}";

			switch ((EventIds) eventId)
			{
				case EventIds.WarningEvent:
					string warnDetails = Marshal.PtrToStringAnsi(ptr);
					Platform.Log(LogLevel.Warn, message, eventId, warnDetails);
					break;
				case EventIds.ErrorEvent:
					string errorDetails = Marshal.PtrToStringAnsi(ptr);
					Platform.Log(LogLevel.Error, message, eventId, errorDetails);
					break;
				default:
					Platform.Log(LogLevel.Fatal, unexpectedMessage);
					Debug.Fail(unexpectedMessage);
					break;
			}
		}

		#endregion

		/// <summary>
		/// Converts a <see cref="Matrix"/> to a <see cref="vtkMatrix4x4"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="vtkMatrix4x4"/> matrix is equivalent to <see cref="Matrix"/> transposed!
		/// This is due to the fact that vtkMatrix4x4 uses (x,y) addresses whereas Matrix
		/// uses (row,column).
		/// </remarks>
		/// <param name="matrix">The source <see cref="Matrix"/>.</param>
		/// <returns>The equivalent <see cref="vtkMatrix4x4"/>.</returns>
		public static vtkMatrix4x4 ConvertToVtkMatrix(Matrix matrix)
		{
			vtkMatrix4x4 vtkMatrix = new vtkMatrix4x4();

			for (int row = 0; row < 4; row++)
				for (int column = 0; column < 4; column++)
					vtkMatrix.SetElement(column, row, matrix[row, column]);

			return vtkMatrix;
		}
	}
}