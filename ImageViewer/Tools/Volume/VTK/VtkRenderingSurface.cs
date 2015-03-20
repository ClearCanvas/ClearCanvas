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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Rendering;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	public class VtkRenderingSurface : IRenderingSurface
	{
		private IntPtr _windowID;
		private IntPtr _contextID;
		private vtkWin32OpenGLRenderWindow _vtkWin32OpenGLRW;
		private Rectangle _clientRectangle;
		private Rectangle _clipRectangle;

		public VtkRenderingSurface(IntPtr windowID)
		{
			_windowID = windowID;

			vtkWin32OpenGLRenderWindow window = new vtkWin32OpenGLRenderWindow();
			SetRenderWindow(window);
		}

		#region Public properties

		#region IRenderingSurface Members

		public IntPtr WindowID
		{
			get { return _windowID; }
			set
			{
				if (_windowID != value)
				{
					_windowID = value;

					if (_windowID == IntPtr.Zero)
						_vtkWin32OpenGLRW.Clean();
					else
						SetRenderWindowID();
				}
			}
		}

		public IntPtr ContextID
		{
			get { return _contextID; }
			set { _contextID = value; }
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			set
			{
				if (value.Width == 0 || value.Height == 0)
					return;

				if (_clientRectangle != value)
				{
					_clientRectangle = value;

					if (this.Interactor != null &&
					    this.Interactor.GetInitialized() != 0)
					{
						this.Interactor.UpdateSize(_clientRectangle.Width, _clientRectangle.Height);
					}
				}
			}
		}

		public Rectangle ClipRectangle
		{
			get { return _clipRectangle; }
			set { _clipRectangle = value; }
		}

		public RenderingSurfaceType Type
		{
			get { return RenderingSurfaceType.Onscreen; }
		}

		public event EventHandler Invalidated
		{
			add { }
			remove { }
		}

		#endregion

		public vtkWin32OpenGLRenderWindow RenderWindow
		{
			get { return _vtkWin32OpenGLRW; }
		}

		public vtkRenderWindowInteractor Interactor
		{
			get
			{
				if (_vtkWin32OpenGLRW == null)
					return null;

				vtkRenderWindowInteractor rwi = _vtkWin32OpenGLRW.GetInteractor();

				if (rwi == null)
					return null;

				return rwi;
			}
		}

		#endregion

		#region Private properties

		private Control HostControl
		{
			get
			{
				if (this.WindowID == IntPtr.Zero)
					return null;
				else
					return Control.FromHandle(this.WindowID);
			}
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_vtkWin32OpenGLRW != null)
				{
					_vtkWin32OpenGLRW.Dispose();
					_vtkWin32OpenGLRW = null;
				}
			}
		}

		#endregion

		#region Public methods

		public void Draw()
		{
			if (this.Interactor != null)
			{
				if (this.Interactor.GetInitialized() == 0)
					this.InitializeInteractor();

				this.Interactor.Render();
			}
		}

		#endregion

		#region Private methods

		private void SetRenderWindow(vtkWin32OpenGLRenderWindow win)
		{
			_vtkWin32OpenGLRW = vtkWin32OpenGLRenderWindow.SafeDownCast(win);

			if (_vtkWin32OpenGLRW != null)
			{
				vtkGenericRenderWindowInteractor iren = new vtkGenericRenderWindowInteractor();
				iren.SetRenderWindow(_vtkWin32OpenGLRW);

				vtkInteractorStyleTrackballCamera style = new vtkInteractorStyleTrackballCamera();
				iren.SetInteractorStyle(style);
				style.Dispose();

				// The control must wait to initialize the interactor until it has
				// been given a parent window. Until then, initializing the interactor
				// will always fail.

				// release our hold on interactor
				iren.Dispose();
			}
		}

		private void InitializeInteractor()
		{
			vtkGenericRenderWindowInteractor iren = vtkGenericRenderWindowInteractor.SafeDownCast(this.Interactor);

			SetRenderWindowID();

			iren.Initialize();

			if (iren.GetInitialized() != 0)
				iren.UpdateSize(this.HostControl.Width, this.HostControl.Height);
		}

		private void SetRenderWindowID()
		{
			if (this.WindowID != IntPtr.Zero)
			{
				_vtkWin32OpenGLRW.SetWindowId(this.WindowID);
				_vtkWin32OpenGLRW.SetParentId(this.HostControl.Parent.Handle);
			}
		}

		#endregion
	}
}