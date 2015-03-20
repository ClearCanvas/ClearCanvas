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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	public class VolumePresentationImageRenderer : IRenderer
	{
		private VtkRenderingSurface _surface;
		private vtkRenderer _vtkRenderer;

		public vtkRenderWindowInteractor Interactor
		{
			get { return _surface.Interactor; }
		}

		#region IRenderer Members

		public IRenderingSurface CreateRenderingSurface(IntPtr windowID, int width, int height, RenderingSurfaceType type)
		{
			if (_surface == null)
				_surface = new VtkRenderingSurface(windowID);
			else
				_surface.WindowID = windowID;

			_surface.ClientRectangle = new Rectangle(0, 0, width, height);

			return _surface;
		}

		public void Draw(DrawArgs args)
		{
			CreateRenderer();
			AddLayers(args);
			_surface.Draw();
		}

		#endregion

		private void CreateRenderer()
		{
			if (_vtkRenderer == null)
			{
				_vtkRenderer = new vtkRenderer();
				_vtkRenderer.SetBackground(0.0f, 0.0f, 0.0f);
				_surface.RenderWindow.AddRenderer(_vtkRenderer);
			}
		}

		private void AddLayers(DrawArgs args)
		{
			IAssociatedTissues volume = args.SceneGraph.ParentPresentationImage as IAssociatedTissues;

			if (volume == null)
				return;

			GraphicCollection layers = volume.TissueLayers;
			vtkPropCollection props = _vtkRenderer.GetViewProps();

			foreach (var graphic in layers)
			{
				var volumeGraphic = (VolumeGraphic) graphic;
				if (props.IsItemPresent(volumeGraphic.VtkProp) == 0)
					_vtkRenderer.AddViewProp(volumeGraphic.VtkProp);
			}
		}

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
			if (disposing && _surface != null)
			{
				_surface.Dispose();
			}
		}
	}
}