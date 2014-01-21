#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using vtk;

namespace ClearCanvas.ImageViewer.Vtk
{
	/// <summary>
	/// Wraps a <see cref="vtk.vtkImageData"/> object holding a handle to a managed data array that is passed to VTK.
	/// </summary>
	public sealed class VtkImageDataHandle : IDisposable
	{
		private bool _disposed = false;
		private GCHandle _pinningHandle;
		private vtkImageData _vtkImageData;

		/// <summary>
		/// Initializes a new instance of the <see cref="VtkImageDataHandle"/> class with the <see cref="vtkImageData"/> to wrap and
		/// a <see cref="GCHandleType.Pinned"/> <see cref="GCHandle"/> to the managed data array to be used by VTK.
		/// </summary>
		/// <remarks>
		/// When this <see cref="VtkImageDataHandle"/> is disposed, both the <paramref name="vtkImageData"/> and
		/// the <paramref name="pinningHandle"/> will be released.
		/// </remarks>
		/// <param name="vtkImageData">The <see cref="vtk.vtkImageData"/> object holding the handle to a managed data array.</param>
		/// <param name="pinningHandle">A <see cref="GCHandle"/> pinning the managed data array.</param>
		public VtkImageDataHandle(vtkImageData vtkImageData, GCHandle pinningHandle)
		{
			Platform.CheckForNullReference(vtkImageData, "vtkImageData");

			_vtkImageData = vtkImageData;
			_pinningHandle = pinningHandle;
		}

		~VtkImageDataHandle()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex);
			}
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex);
			}
		}

		private void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing && _vtkImageData != null)
			{
				_vtkImageData.GetPointData().Dispose();
				_vtkImageData.Dispose();
				_vtkImageData = null;
			}

			_pinningHandle.Free();
			_disposed = true;
		}

		/// <summary>
		/// Gets the <see cref="vtk.vtkImageData"/> object holding the handle to a managed data array.
		/// </summary>
		public vtkImageData vtkImageData
		{
			get { return _vtkImageData; }
		}
	}
}