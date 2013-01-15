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
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics.Utilities
{
	public class GraphicsVisibilityHelper
	{
		private readonly GraphicCollection _graphics;
		private readonly Dictionary<IGraphic, bool> _visibility = new Dictionary<IGraphic, bool>();

		public GraphicsVisibilityHelper(GraphicCollection graphics)
		{
			_graphics = graphics;
			foreach (var graphic in _graphics)
				_visibility[graphic] = graphic.Visible;
		}

		public void ShowAll()
		{
			SetVisible(true);
		}

		public void HideAll()
		{
			SetVisible(false);
		}

		public void RestoreAll()
		{
			foreach (var graphic in _graphics)
			{
				bool visible;
				if (_visibility.TryGetValue(graphic, out visible))
					graphic.Visible = visible;
			}
		}

		//TODO: extension method?
		public static void HideAll(GraphicCollection graphics)
		{
			SetVisible(graphics, false);
		}

		//TODO: extension method?
		public static void ShowAll(GraphicCollection graphics)
		{
			SetVisible(graphics, true);
		}

		public static GraphicsVisibilityHelper CreateForApplicationGraphics(IPresentationImage image)
		{
			var provider = image as IApplicationGraphicsProvider;
			if (provider == null)
				return null;

			return new GraphicsVisibilityHelper(provider.ApplicationGraphics);
		}

		public static GraphicsVisibilityHelper CreateForOverlayGraphics(IPresentationImage image)
		{
			var provider = image as IOverlayGraphicsProvider;
			if (provider == null)
				return null;

			return new GraphicsVisibilityHelper(provider.OverlayGraphics);
		}

		private void SetVisible(bool visible)
		{
			SetVisible(_graphics, visible);
		}

		private static void SetVisible(GraphicCollection graphics, bool visible)
		{
			foreach (var graphic in graphics)
				graphic.Visible = visible;

		}
	}
}
