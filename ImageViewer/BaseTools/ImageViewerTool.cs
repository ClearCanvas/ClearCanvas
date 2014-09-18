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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// A base class for image viewer tools.
	/// </summary>
	public abstract class ImageViewerTool : Tool<IImageViewerToolContext>
	{
		private bool _enabled = true;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected ImageViewerTool() {}

		/// <summary>
		/// Disposes of this object; override this method to do any necessary cleanup.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected override void Dispose(bool disposing)
		{
			ImageViewer.EventBroker.TileSelected -= OnTileSelected;
			ImageViewer.EventBroker.PresentationImageSelected -= OnPresentationImageSelected;

			base.Dispose(disposing);
		}

		/// <summary>
		/// Initializes the <see cref="ImageViewerTool"/>.
		/// </summary>
		public override void Initialize()
		{
			ImageViewer.EventBroker.TileSelected += OnTileSelected;
			ImageViewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;

			base.Initialize();
		}

		/// <summary>
		/// Gets or sets a value indicating whether the tool is enabled.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Enabled"/> property has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/> associated with this tool.
		/// </summary>
		protected IDesktopWindow DesktopWindow
		{
			get { return Context.DesktopWindow; }
		}

		/// <summary>
		/// Gets the <see cref="IImageViewer"/> associated with this tool.
		/// </summary>
		protected IImageViewer ImageViewer
		{
			get { return Context.Viewer; }
		}

		/// <summary>
		/// Gets the selected <see cref="IPresentationImage"/>.
		/// </summary>
		/// <value>The selected <see cref="IPresentationImage"/> or <b>null</b>
		/// if no <see cref="IPresentationImage"/> is currently selected.</value>
		protected IPresentationImage SelectedPresentationImage
		{
			get { return ImageViewer != null ? ImageViewer.SelectedPresentationImage : null; }
		}

		/// <summary>
		/// Gets the selected <see cref="IImageGraphicProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IImageGraphicProvider"/> or <b>null</b>
		/// if no <see cref="IImageGraphicProvider"/> is currently selected.</value>
		protected IImageGraphicProvider SelectedImageGraphicProvider
		{
			get { return SelectedPresentationImage as IImageGraphicProvider; }
		}

		/// <summary>
		/// Gets the selected <see cref="IImageSopProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IImageSopProvider"/> or <b>null</b>
		/// if no <see cref="IImageSopProvider"/> is currently selected.</value>
		protected IImageSopProvider SelectedImageSopProvider
		{
			get { return SelectedPresentationImage as IImageSopProvider; }
		}

		/// <summary>
		/// Gets the selected <see cref="ISpatialTransformProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="ISpatialTransformProvider"/> or <b>null</b>
		/// if no <see cref="ISpatialTransformProvider"/> is currently selected.</value>
		protected ISpatialTransformProvider SelectedSpatialTransformProvider
		{
			get { return SelectedPresentationImage as ISpatialTransformProvider; }
		}

		/// <summary>
		/// Gets the selected <see cref="IVoiLutProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IVoiLutProvider"/> or <b>null</b>
		/// if no <see cref="IVoiLutProvider"/> is currently selected.</value>
		protected IVoiLutProvider SelectedVoiLutProvider
		{
			get { return SelectedPresentationImage as IVoiLutProvider; }
		}

		/// <summary>
		/// Gets the selected <see cref="IOverlayGraphicsProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IOverlayGraphicsProvider"/> or <b>null</b>
		/// if no <see cref="IOverlayGraphicsProvider"/> is currently selected.</value>
		protected IOverlayGraphicsProvider SelectedOverlayGraphicsProvider
		{
			get { return SelectedPresentationImage as IOverlayGraphicsProvider; }
		}

		/// <summary>
		/// Gets the selected <see cref="IAnnotationLayoutProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IAnnotationLayoutProvider"/> or <b>null</b>
		/// if no <see cref="IAnnotationLayoutProvider"/> is currently selected.</value>
		protected IAnnotationLayoutProvider SelectedAnnotationLayoutProvider
		{
			get { return SelectedPresentationImage as IAnnotationLayoutProvider; }
		}

		/// <summary>
		/// Event Handler for <see cref="EventBroker.TileSelected"/>.
		/// </summary>
		protected virtual void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			Enabled = e.SelectedTile.PresentationImage != null;
		}

		/// <summary>
		/// Event Handler for <see cref="EventBroker.PresentationImageSelected"/>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			Enabled = e.SelectedPresentationImage != null;
		}
	}
}