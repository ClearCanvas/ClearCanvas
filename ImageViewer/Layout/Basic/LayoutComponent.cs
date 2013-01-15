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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
	/// Defines an extension point for views onto a <see cref="LayoutComponent"/>
	/// </summary>
	[ExtensionPoint()]
	public sealed class LayoutComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// This component allows a user to control the layout of an associated image viewer.
	/// Set the <see cref="LayoutComponent.Subject"/> property to the <see cref="IImageViewer"/>
	/// that is to be controlled.
	/// </summary>
	[AssociateView(typeof (LayoutComponentViewExtensionPoint))]
	public class LayoutComponent : ImageViewerToolComponent
	{
		private int _imageBoxRows;
		private int _imageBoxColumns;
		private int _tileRows;
		private int _tileColumns;

		/// <summary>
		/// Constructor
		/// </summary>
		public LayoutComponent(IDesktopWindow desktopWindow)
			: base(desktopWindow) {}

		#region Public properties and methods for use by the view

		/// <summary>
		/// Indicates to the view whether the image-box section of the user-interface should be enabled
		/// </summary>
		public bool ImageBoxSectionEnabled
		{
			get
			{
				return this.ImageViewer != null && !this.ImageViewer.PhysicalWorkspace.Locked;
			}
		}

		/// <summary>
		/// Indicates to the view whether the tile section of the user-interface should be enabled
		/// </summary>
		public bool TileSectionEnabled
		{
			get
			{
				return this.ImageBoxSectionEnabled && this.ImageViewer.PhysicalWorkspace.SelectedImageBox != null;
			}
		}

		/// <summary>
		/// Gets the maximum allowable rows for image boxes.
		/// </summary>
		public int MaximumImageBoxRows
		{
			get { return LayoutSettings.MaximumImageBoxRows; }
		}

		/// <summary>
		/// Gets the maximum allowable columns for image boxes.
		/// </summary>
		public int MaximumImageBoxColumns
		{
			get { return LayoutSettings.MaximumImageBoxColumns; }
		}

		/// <summary>
		/// Gets the maximum allowable rows for tiles.
		/// </summary>
		public int MaximumTileRows
		{
			get { return LayoutSettings.MaximumTileRows; }
		}

		/// <summary>
		/// Gets the maximum allowable columns for tiles.
		/// </summary>
		public int MaximumTileColumns
		{
			get { return LayoutSettings.MaximumTileColumns; }
		}

		/// <summary>
		/// Gets/sets the number of image box rows
		/// </summary>
		public int ImageBoxRows
		{
			get { return _imageBoxRows; }
			set
			{
				int newValue = Math.Max(value, 1);
				newValue = Math.Min(newValue, this.MaximumImageBoxRows);
				if (newValue == _imageBoxRows)
					return;

				_imageBoxRows = newValue;
				NotifyPropertyChanged("ImageBoxRows");
			}
		}

		/// <summary>
		/// Gets/sets the number of image box columns
		/// </summary>
		public int ImageBoxColumns
		{
			get { return _imageBoxColumns; }
			set
			{
				int newValue = Math.Max(value, 1);
				newValue = Math.Min(newValue, this.MaximumImageBoxColumns);

				if (newValue == _imageBoxColumns)
					return;

				_imageBoxColumns = newValue;
				NotifyPropertyChanged("ImageBoxColumns");
			}
		}

		/// <summary>
		/// Gets/sets the number of tile rows
		/// </summary>
		public int TileRows
		{
			get { return _tileRows; }
			set
			{
				int newValue = Math.Max(value, 1);
				newValue = Math.Min(newValue, this.MaximumTileRows);
				if (newValue == _tileRows)
					return;

				_tileRows = newValue;
				NotifyPropertyChanged("TileRows");
			}
		}

		/// <summary>
		/// Gets/sets the number of tile columns
		/// </summary>
		public int TileColumns
		{
			get { return _tileColumns; }
			set
			{
				int newValue = Math.Max(value, 1);
				newValue = Math.Min(newValue, this.MaximumTileColumns);
				if (newValue == _tileColumns)
					return;

				_tileColumns = newValue;
				NotifyPropertyChanged("TileColumns");
			}
		}

		public void Configure()
		{
			LayoutConfigurationComponent.Configure(this.Host.DesktopWindow);
		}

		/// <summary>
		/// Called by the view to apply the image layout to the subject
		/// </summary>
		public void ApplyImageBoxLayout()
		{
			if (this.ImageViewer == null)
				return;

			SetImageBoxLayout(this.ImageViewer, this.ImageBoxRows, this.ImageBoxColumns);

			Update();
		}

		/// <summary>
		/// Called by the view to apply the tile layout to the subject
		/// </summary>
		public void ApplyTileLayout()
		{
			if (this.ImageViewer == null)
				return;

			SetTileLayout(this.ImageViewer, this.TileRows, this.TileColumns);

			Update();
		}

		#endregion

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			// stop listening to the old image viewer, if one was set
			if (e.DeactivatedImageViewer != null)
			{
				e.DeactivatedImageViewer.PhysicalWorkspace.LockedChanged -= OnPhysicalWorkspaceLockedChanged;
				e.DeactivatedImageViewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;
			}

			// start listening to the new image viewer, if one has been set
			if (e.ActivatedImageViewer != null)
			{
				e.ActivatedImageViewer.PhysicalWorkspace.LockedChanged += OnPhysicalWorkspaceLockedChanged;
				e.ActivatedImageViewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;
			}

			Update();
		}

		private void OnPhysicalWorkspaceLockedChanged(object sender, EventArgs e)
		{
			Update();
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			Update();
		}

		private void Update()
		{
			if (this.ImageViewer != null)
			{
				ImageBoxRows = this.ImageViewer.PhysicalWorkspace.Rows;
				ImageBoxColumns = this.ImageViewer.PhysicalWorkspace.Columns;

				if (this.ImageViewer.PhysicalWorkspace.SelectedImageBox != null)
				{
					TileRows = this.ImageViewer.PhysicalWorkspace.SelectedImageBox.Rows;
					TileColumns = this.ImageViewer.PhysicalWorkspace.SelectedImageBox.Columns;
				}
			}

			NotifyPropertyChanged("ImageBoxSectionEnabled");
			NotifyPropertyChanged("TileSectionEnabled");
		}

		public static void SetImageBoxLayout(IImageViewer imageViewer, int rows, int columns)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");
			Platform.CheckArgumentRange(rows, 1, LayoutSettings.MaximumImageBoxRows, "rows");
			Platform.CheckArgumentRange(columns, 1, LayoutSettings.MaximumImageBoxColumns, "columns");

			IPhysicalWorkspace physicalWorkspace = imageViewer.PhysicalWorkspace;
			if (physicalWorkspace.Locked)
				return;

			var memorableCommand = new MemorableUndoableCommand(physicalWorkspace)
			                           {BeginState = physicalWorkspace.CreateMemento()};

		    var fillingStrategy = ImageBoxFillingStrategy.Create();
		    var context = new ImageBoxFillingStrategyContext(imageViewer, p => p.SetImageBoxGrid(rows, columns),
		                                                     imageBox => imageBox.SetTileGrid(1, 1));
            fillingStrategy.SetContext(context);
            fillingStrategy.FillImageBoxes();
			physicalWorkspace.Draw();
			physicalWorkspace.SelectDefaultImageBox();

			memorableCommand.EndState = physicalWorkspace.CreateMemento();
			var historyCommand = new DrawableUndoableCommand(physicalWorkspace) {Name = SR.CommandLayoutImageBoxes};
		    historyCommand.Enqueue(memorableCommand);

			imageViewer.CommandHistory.AddCommand(historyCommand);
		}

		public static void SetTileLayout(IImageViewer imageViewer, int rows, int columns)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");
			Platform.CheckArgumentRange(rows, 1, LayoutSettings.MaximumTileRows, "rows");
			Platform.CheckArgumentRange(columns, 1, LayoutSettings.MaximumTileColumns, "columns");

			IImageBox imageBox = imageViewer.PhysicalWorkspace.SelectedImageBox;			
			if (imageBox == null || imageBox.ParentPhysicalWorkspace.Locked)
				return;

			var memorableCommand = new MemorableUndoableCommand(imageBox) {BeginState = imageBox.CreateMemento()};

		    int index = imageBox.TopLeftPresentationImageIndex;

			imageBox.SetTileGrid(rows, columns);
			imageBox.TopLeftPresentationImageIndex = index;
			imageBox.Draw();
			imageBox.SelectDefaultTile();

			memorableCommand.EndState = imageBox.CreateMemento();

			var historyCommand = new DrawableUndoableCommand(imageBox) {Name = SR.CommandLayoutTiles};
		    historyCommand.Enqueue(memorableCommand);
			imageViewer.CommandHistory.AddCommand(historyCommand);
		}
	}
}