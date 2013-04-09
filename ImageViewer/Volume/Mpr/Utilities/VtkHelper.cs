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
using System.Threading;
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

        private static readonly object _initializationLock = false;
        private static volatile bool _initializing = false;
        private static volatile bool _initialized = false;

        internal static void StaticInitializationHack()
        {
           if (_initialized)
               return;

            lock (_initializationLock)
            {
                if (_initialized || _initializing)
                    return;

                _initializing = true;
                //Create one signed and one unsigned.
                CreateAndResliceVolume();
                CreateAndResliceVolume();
            }
        }

	    private static int _count = 0;
        private static void CreateAndResliceVolume()
        {
            var width = 512;
            var height = 512;
            var depth = 200;

            var count = Interlocked.Increment(ref _count);
            var signed = count % 2 > 0;
            if (signed)
            {
                var data = new short[width * height * depth];
                using (var vtk = CreateVtkVolume(data, width, height, depth))
                {
                    ResliceVolume(vtk);
                    vtk.GetPointData().Dispose();
                }
            }
            else
            {
                var data = new ushort[width * height * depth];
                using (var vtk = CreateVtkVolume(data, width, height, depth))
                {
                    ResliceVolume(vtk);
                    vtk.GetPointData().Dispose();
                }
            }
        }

        private static void ResliceVolume(vtkImageData volume)
        {
            using (vtkImageReslice reslicer = new vtkImageReslice())
            {
                VtkHelper.RegisterVtkErrorEvents(reslicer);
                {
                    reslicer.SetInput(volume);
                    reslicer.SetInformationInput(volume);

                    // Must instruct reslicer to output 2D images
                    reslicer.SetOutputDimensionality(2);

                    // Use the volume's padding value for all pixels that are outside the volume
                    reslicer.SetBackgroundLevel(0);

                    // This ensures VTK obeys the real spacing, results in all VTK slices being isotropic.
                    //	Effective spacing is the minimum of these three.
                    reslicer.SetOutputSpacing(1.0, 1.0, 1.0);

                    using (vtkMatrix4x4 resliceAxesMatrix = VtkHelper.ConvertToVtkMatrix(new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } }))
                    {
                        reslicer.SetResliceAxes(resliceAxesMatrix);

                        // Clamp the output based on the slice extent
                        int sliceExtentX = 50;
                        int sliceExtentY = 50;
                        reslicer.SetOutputExtent(0, sliceExtentX - 1, 0, sliceExtentY - 1, 0, 0);

                        // Set the output origin to reflect the slice through point. The slice extent is
                        //	centered on the slice through point.
                        // VTK output origin is derived from the center image being 0,0
                        float originX = -sliceExtentX * 1f / 2;
                        float originY = -sliceExtentY * 1f / 2;
                        reslicer.SetOutputOrigin(originX, originY, 0);

                        reslicer.SetInterpolationModeToLinear();

                        //lock (_lock0)
                        {
                            using (vtkExecutive exec = reslicer.GetExecutive())
                            {
                                VtkHelper.RegisterVtkErrorEvents(exec);
                                exec.Update();
                            }
                        }

                        using (var output = reslicer.GetOutput())
                        {
                            // just to give it something to do with the output
                            if (output == null) ;
                        }

                        //tput.
                        //Just in case VTK uses the matrix internally.
                        //return output;
                    }
                }
            }

        }

        private static vtkImageData CreateVtkVolume(ushort[] data, int width, int height, int depth)
        {
            //lock (_lock3)
            {
                vtkImageData vtkVolume = new vtkImageData();

                VtkHelper.RegisterVtkErrorEvents(vtkVolume);

                vtkVolume.SetDimensions(width, height, depth);
                vtkVolume.SetOrigin(0, 0, 0);
                vtkVolume.SetSpacing(1.0, 1.0, 1.0);

                {
                    using (var array = VtkHelper.ConvertToVtkUnsignedShortArray(data))
                    {
                        //lock (_lock2)
                        {
                            vtkVolume.SetScalarTypeToUnsignedShort();
                        }
                        vtkVolume.GetPointData().SetScalars(array);

                        // This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
                        //lock (_lock1)
                        {
                            vtkVolume.UpdateInformation();
                        }
                    }
                }

                return vtkVolume;
            }
        }

        private static vtkImageData CreateVtkVolume(short[] data, int width, int height, int depth)
        {
            //lock (_lock3)
            {
                vtkImageData vtkVolume = new vtkImageData();

                VtkHelper.RegisterVtkErrorEvents(vtkVolume);

                vtkVolume.SetDimensions(width, height, depth);
                vtkVolume.SetOrigin(0, 0, 0);
                vtkVolume.SetSpacing(1.0, 1.0, 1.0);

                {
                    using (var array = VtkHelper.ConvertToVtkShortArray(data))
                    {
                        //lock (_lock2)
                        {
                            vtkVolume.SetScalarTypeToShort();
                        }
                        vtkVolume.GetPointData().SetScalars(array);

                        // This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
                        //lock (_lock1)
                        {
                            vtkVolume.UpdateInformation();
                        }
                    }
                }

                return vtkVolume;
            }
        }
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

		#region Convert to VTK helpers

        private static vtkMatrix4x4 ConvertToVtkMatrix(double[,] matrix)
        {
            vtkMatrix4x4 vtkMatrix = new vtkMatrix4x4();

            for (int row = 0; row < 4; row++)
                for (int column = 0; column < 4; column++)
                    vtkMatrix.SetElement(column, row, matrix[row, column]);

            return vtkMatrix;
        }

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

		public static vtkShortArray ConvertToVtkShortArray(short[] shortArray)
		{
			vtkShortArray vtkShortArray = new vtkShortArray();
			vtkShortArray.SetArray(shortArray, (VtkIdType) shortArray.Length, 1);
			return vtkShortArray;
		}

		public static vtkUnsignedShortArray ConvertToVtkUnsignedShortArray(ushort[] ushortArray)
		{
			vtkUnsignedShortArray vtkUnsignedShortArray = new vtkUnsignedShortArray();
			vtkUnsignedShortArray.SetArray(ushortArray, (VtkIdType) ushortArray.Length, 1);
			return vtkUnsignedShortArray;
		}

		#endregion
    }
}