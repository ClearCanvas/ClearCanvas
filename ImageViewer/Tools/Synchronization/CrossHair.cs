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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	public partial class SpatialLocatorTool
	{
		// The crosshair is a container for a graphic that is tied to an imagebox;
		// the container moves the graphic between different images in the imagebox.
		private class CrossHair : IDisposable
		{
			#region Private Fields

			private readonly SpatialLocatorTool _spatialLocatorTool;

			private IPresentationImage _presentationImage;
			private PointF _imagePoint;
			private CrosshairGraphic _crosshairGraphic;
			private bool _dirty;

			#endregion

			public readonly IImageBox ImageBox;

			public CrossHair(IImageBox imageBox, SpatialLocatorTool spatialLocatorTool)
			{
				ImageBox = imageBox;
				_spatialLocatorTool = spatialLocatorTool;

				_crosshairGraphic = new CrosshairGraphic();
				_crosshairGraphic.Drawing += OnGraphicDrawing;
			}

			#region Private Methods

			private void OnGraphicDrawing(object sender, EventArgs e)
			{
				_dirty = false;
			}

			private void RemoveGraphicFromCurrentImage()
			{
				CompositeGraphic parentGraphic = _crosshairGraphic.ParentGraphic as CompositeGraphic;
				if (parentGraphic != null)
					parentGraphic.Graphics.Remove(_crosshairGraphic);
			}

			private void AddGraphicToCurrentImage()
			{
				CompositeGraphic container = _spatialLocatorTool._coordinator.GetSpatialLocatorCompositeGraphic(_presentationImage);
				if (container != null)
				{
					container.Graphics.Add(_crosshairGraphic);
					SetGraphicAnchorPoint();
				}
			}

			private void SetGraphicAnchorPoint()
			{
				if (_presentationImage != null)
				{
					_crosshairGraphic.CoordinateSystem = CoordinateSystem.Source;
					_crosshairGraphic.Anchor = ImagePoint;
					_crosshairGraphic.ResetCoordinateSystem();
				}
			}

			#endregion

			#region Public Properties

			public IPresentationImage Image
			{
				get { return _presentationImage; }
				set
				{
					if (_presentationImage == value)
						return;

					RemoveGraphicFromCurrentImage();

					_presentationImage = value;
					_dirty = true;

					if (_presentationImage == null)
						return;

					ImageBox.TopLeftPresentationImage = _presentationImage;
					AddGraphicToCurrentImage();
				}
			}

			public PointF ImagePoint
			{
				get { return _imagePoint; }
				set
				{
					if (_imagePoint == value)
						return;

					_imagePoint = value;
					_dirty = true;

					SetGraphicAnchorPoint();
				}
			}

			public bool Dirty
			{
				get { return _dirty; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (_crosshairGraphic != null)
				{
					_crosshairGraphic.Drawing -= OnGraphicDrawing;

					RemoveGraphicFromCurrentImage();

					_crosshairGraphic.Dispose();
					_crosshairGraphic = null;
				}
			}

			#endregion
		}
	}
}