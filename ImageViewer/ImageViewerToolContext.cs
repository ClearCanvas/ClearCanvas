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

using System.ComponentModel;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines an image viewer tool context.
	/// </summary>
	public interface IImageViewerToolContext : IToolContext
	{
		/// <summary>
		/// Gets the <see cref="IImageViewer"/>.
		/// </summary>
		IImageViewer Viewer { get; }

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/>.
		/// </summary>
		IDesktopWindow DesktopWindow { get; }

		/// <summary>
		/// Fired when the image viewer is preparing to close, in order to allow tools an opportunity to abort closing the viewer.
		/// </summary>
		/// <remarks>
		/// This event is separate and distinct from <see cref="IImageViewer.Closing"/> which, despite its name, is fired as the
		/// image viewer is already in the process of closing.
		/// </remarks>
		event CancelEventHandler ViewerClosing;
	}

	public partial class ImageViewerComponent
	{
		/// <summary>
		/// A basic implementation of <see cref="IImageViewerToolContext"/>.
		/// </summary>
		protected class ImageViewerToolContext : ToolContext, IImageViewerToolContext
		{
			private readonly ImageViewerComponent _component;

			/// <summary>
			/// Constructs a new <see cref="ImageViewerToolContext"/>.
			/// </summary>
			/// <param name="component">The <see cref="ImageViewerComponent"/> that owns the tools.</param>
			public ImageViewerToolContext(ImageViewerComponent component)
			{
				_component = component;
			}

			#region IImageViewerToolContext Members

			/// <summary>
			/// Gets the <see cref="IImageViewer"/>.
			/// </summary>
			public IImageViewer Viewer
			{
				get { return _component; }
			}

			/// <summary>
			/// Gets the <see cref="IDesktopWindow"/>.
			/// </summary>
			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			/// <summary>
			/// Fired when the image viewer is preparing to close, in order to allow tools an opportunity to abort closing the viewer.
			/// </summary>
			/// <remarks>
			/// This event is separate and distinct from <see cref="IImageViewer.Closing"/> which, despite the usual semantics associated
			/// with its name, is fired as the image viewer is already in the process of closing.
			/// </remarks>
			public event CancelEventHandler ViewerClosing
			{
				add { _component._closingInternal += value; }
				remove { _component._closingInternal -= value; }
			}

			#endregion
		}
	}
}