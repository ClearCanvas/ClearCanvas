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
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	internal static class VtkHelper
	{
		#region Error Handling helpers

		public static void RegisterVtkErrorEvents(vtkObject obj)
		{
			obj.AddObserver((uint)EventIds.ErrorEvent, VtkEventCallback);
			obj.AddObserver((uint)EventIds.WarningEvent, VtkEventCallback);
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
			vtkShortArray.SetArray(shortArray, (VtkIdType)shortArray.Length, 1);
			return vtkShortArray;
		}

		public static vtkUnsignedShortArray ConvertToVtkUnsignedShortArray(ushort[] ushortArray)
		{
			vtkUnsignedShortArray vtkUnsignedShortArray = new vtkUnsignedShortArray();
			vtkUnsignedShortArray.SetArray(ushortArray, (VtkIdType)ushortArray.Length, 1);
			return vtkUnsignedShortArray;
		}

		#endregion
	}
}