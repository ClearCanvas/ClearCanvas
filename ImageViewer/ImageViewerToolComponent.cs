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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A base class for image viewer related application components.
	/// </summary>
	/// <remarks>
	/// There is often a need for an <see cref="ApplicationComponent"/> that will
	/// listen for changes in <see cref="ImageViewerComponent"/>activation.  
	/// For example, the shelf that appears when the image layout tool activated 
	/// has UI elements which should be enabled/disabled depending on whether the 
	/// active workspace contains an <see cref="ImageViewerComponent"/>.
	/// This base class encapsulates that functionality.  If you are developing an 
	/// <see cref="ApplicationComponent"/> that requires that kind of functionality,
	/// use this as your base class.
	/// </remarks>
	public abstract class ImageViewerToolComponent : ApplicationComponent
	{
		private IDesktopWindow _desktopWindow;
		private IImageViewer _imageViewer;

		/// <summary>
		/// Initializes a new instance of <see cref="ImageViewerToolComponent"/>.
		/// </summary>
		protected ImageViewerToolComponent(IDesktopWindow desktopWindow)
		{
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
			_desktopWindow = desktopWindow;
			ImageViewer = ImageViewerComponent.GetAsImageViewer(_desktopWindow.ActiveWorkspace);
		}

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/> associated with this object.
		/// </summary>
		public IDesktopWindow DesktopWindow
		{
			get { return _desktopWindow; }	
		}

		/// <summary>
		/// Gets the subject <see cref="IImageViewer"/> that this component is associated with.
		/// </summary>
		/// <remarks>
		/// <value>
		/// The currently active <see cref="IImageViewer"/> or <b>null</b> if the
		/// there is no currently active <see cref="IImageViewer"/>.
		/// </value>
		/// </remarks>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			private set
			{
				if (value != _imageViewer)
				{
					ActiveImageViewerChangedEventArgs e = new ActiveImageViewerChangedEventArgs(value, _imageViewer);

					try
					{
						OnActiveImageViewerChanging(e);
					}
					finally
					{
						_imageViewer = value;
						OnActiveImageViewerChanged(e);
					}
				}
			}
		}

		/// <summary>
		/// Called when the active image viewer is about to change.
		/// </summary>
		protected virtual void OnActiveImageViewerChanging(ActiveImageViewerChangedEventArgs e)
		{
		}

		/// <summary>
		/// Called when the active image viewer has changed.
		/// </summary>
		protected virtual void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
		}

		#region ApplicationComponent overrides

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Start"/>
		/// </summary>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Start()
		{
			base.Start();

			_desktopWindow.Workspaces.ItemActivationChanged += OnWorkspaceActivated;
		}

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Stop"/>
		/// </summary>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Stop()
		{
			_desktopWindow.Workspaces.ItemActivationChanged -= OnWorkspaceActivated;

			base.Stop();
		}

		#endregion

		private void OnWorkspaceActivated(object sender, ItemEventArgs<Workspace> e)
		{
			Workspace activeWorkspace = _desktopWindow.ActiveWorkspace;

			if (activeWorkspace == null)
			{
				this.ImageViewer = null;
			}
			else
			{
				IImageViewer imageViewer = ImageViewerComponent.GetAsImageViewer(activeWorkspace);
				this.ImageViewer = imageViewer;
			}
		}
	}
}
