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

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	internal class VtkVolumeHandle : IDisposable
	{
		private bool _disposed = false;
		private GCHandle _volumeArrayPinned;
		private vtkImageData _vtkVolume;

		public VtkVolumeHandle(vtkImageData vtkVolume, GCHandle volumeArrayPinned)
		{
			_vtkVolume = vtkVolume;
			_volumeArrayPinned = volumeArrayPinned;
		}

		~VtkVolumeHandle()
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

			if (disposing && _vtkVolume != null)
			{
				_vtkVolume.GetPointData().Dispose();
				_vtkVolume.Dispose();
				_vtkVolume = null;
			}

			_volumeArrayPinned.Free();
			_disposed = true;
		}

		public vtkImageData vtkImageData
		{
			get { return _vtkVolume; }
		}
	}
}