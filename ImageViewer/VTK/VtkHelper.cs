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
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using vtk;

namespace ClearCanvas.ImageViewer.Vtk
{
	internal static class VtkHelper
	{
		public static Size GetSize2(this vtkRenderWindow renderWindow)
		{
			var sz = renderWindow.GetSize();
			return new Size(sz[0], sz[1]);
		}

		public static void RegisterVtkErrorEvents(this vtkObject obj)
		{
			obj.AddObserver((uint) EventIds.ErrorEvent, VtkEventCallback);
			obj.AddObserver((uint) EventIds.WarningEvent, VtkEventCallback);
		}

		private static void VtkEventCallback(vtkObject vtkObj, uint eventId, object obj, IntPtr ptr)
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

		public static double[] ConvertToVtkColor(Color c)
		{
			return new[]
			       	{
			       		_singleChannelByteToDouble[c.R],
			       		_singleChannelByteToDouble[c.G],
			       		_singleChannelByteToDouble[c.B]
			       	};
		}

		public static double[] ConvertToVtkColor(int xrgb)
		{
			return new[]
			       	{
			       		_singleChannelByteToDouble[(xrgb >> 16) & 0x0FF],
			       		_singleChannelByteToDouble[(xrgb >> 8) & 0x0FF],
			       		_singleChannelByteToDouble[(xrgb) & 0x0FF]
			       	};
		}

		private static readonly double[] _singleChannelByteToDouble = new double[256];

		static unsafe VtkHelper()
		{
			// set up VTK logging
			var fileOutputWindow = new vtkFileOutputWindow();
			fileOutputWindow.SetFileName(Path.Combine(Platform.LogDirectory, "vtk.log"));
			vtkOutputWindow.SetInstance(fileOutputWindow);

			fixed (double* p = _singleChannelByteToDouble)
			{
				var pData = p;
				for (var n = 0; n < 256; ++n)
					*(pData++) = n/255.0;
			}
		}

		public static void SetElements(this vtkMatrix4x4 vtkMatrix, Matrix matrix)
		{
			for (var i = 0; i < 16; i++)
			{
				var row = (i & 0x0C) >> 2;
				var column = i & 0x03;
				vtkMatrix.SetElement(row, column, matrix[row, column]);
			}
		}
	}
}