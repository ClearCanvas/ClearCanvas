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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An extension point for graphic tools.
	/// </summary>
	[ExtensionPoint()]
	public sealed class GraphicToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	/// <summary>
	/// An interface for graphic tools.
	/// </summary>
	public interface IGraphicToolContext : IToolContext
	{
		/// <summary>
		/// Gets the <see cref="IGraphic"/> that the tool applies to.
		/// </summary>
		IGraphic Graphic { get; }

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/>.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }
	}

	/// <summary>
	/// Base implementation of <see cref="IGraphicToolContext"/>.
	/// </summary>
	public class GraphicToolContext : ToolContext, IGraphicToolContext
	{
		private readonly IGraphic _graphic;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GraphicToolContext(IGraphic graphic)
		{
			_graphic = graphic;
		}

		/// <summary>
		/// Gets the graphic that the tool applies to.
		/// </summary>
		public IGraphic Graphic
		{
			get { return _graphic; }
		}

		/// <summary>
		/// Gets the owning <see cref="IDesktopWindow"/>.
		/// </summary>
		public IDesktopWindow DesktopWindow
		{
			get
			{
				if (_graphic.ImageViewer == null)
					return null;
				return _graphic.ImageViewer.DesktopWindow;
			}
		}
	}
}
