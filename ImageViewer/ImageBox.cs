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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// An extension point for views onto <see cref="ImageBox"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ImageBoxViewExtensionPoint : ExtensionPoint<IView> {}

	/// <summary>
	/// A container for <see cref="ITile"/> objects.
	/// </summary>
	[AssociateView(typeof (ImageBoxViewExtensionPoint))]
	public class ImageBox : IImageBox
	{
		#region Private Fields

		private TileCollection _tiles;
		private IImageViewer _imageViewer;
		private PhysicalWorkspace _parentPhysicalWorkspace;
		private DisplaySet _displaySet;
		private Tile _selectedTile;
		private RectangleF _normalizedRectangle;
		private bool _selected = false;
		private bool _enabled = true;
		private int _rows;
		private int _columns;
		private static int _borderWidth = 2;
		private static int _insetWidth = 5;
		private static Color _selectedColor = Color.Orange;
		private static Color _unselectedColor = Color.DarkGray;

		private event EventHandler _drawingEvent;
		private event EventHandler<ItemEventArgs<IImageBox>> _selectionChangedEvent;
		private event EventHandler<DisplaySetChangedEventArgs> _displaySetChangedEvent;
		private event EventHandler<ItemEventArgs<ITile>> _tileAddedEvent;
		private event EventHandler<ItemEventArgs<ITile>> _tileRemovedEvent;
		private event EventHandler _layoutCompletedEvent;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ImageBox"/>.
		/// </summary>
		public ImageBox()
		{
			this.Tiles.ItemAdded += OnTileAdded;
			this.Tiles.ItemRemoved += OnTileRemoved;
			ExtensionData = new ExtensionData();
		}

		#region Public properties

		/// <summary>
		/// Gets the collection of <see cref="ITile"/> objects that belong
		/// to this <see cref="ImageBox"/>.
		/// </summary>
		public TileCollection Tiles
		{
			get
			{
				if (_tiles == null)
					_tiles = new TileCollection();

				return _tiles;
			}
		}

		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="ImageBox"/> is not part of the 
		/// physical workspace yet.</value>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			internal set
			{
				_imageViewer = value;

				if (_imageViewer != null)
				{
					foreach (Tile tile in this.Tiles)
						tile.ImageViewer = _imageViewer;

					if (_displaySet != null)
						_displaySet.ImageViewer = _imageViewer;
				}
			}
		}

		/// <summary>
		/// Gets the parent <see cref="IPhysicalWorkspace"/>
		/// </summary>
		/// <value>The parent <see cref="IPhysicalWorkspace"/> or <b>null</b> if the 
		/// <see cref="ImageBox"/> has not
		/// been added to the <see cref="IPhysicalWorkspace"/> yet.</value>
		public IPhysicalWorkspace ParentPhysicalWorkspace
		{
			get { return _parentPhysicalWorkspace; }
			internal set { _parentPhysicalWorkspace = value as PhysicalWorkspace; }
		}

		/// <summary>
		/// Gets or sets this <see cref="ImageBox"/>'s normalized rectangle.
		/// </summary>
		/// <remarks>
		/// Normalized coordinates specify the top-left corner,
		/// width and height of the <see cref="ImageBox"/> as a 
		/// fraction of the physical workspace.  For example, if the
		/// <see cref="NormalizedRectangle"/> is (left=0.25, top=0.0, width=0.5, height=0.5) 
		/// and the physical workspace has dimensions of (width=1000, height=800), the 
		/// <see cref="ImageBox"/> rectangle would be (left=250, top=0, width=500, height=400)
		/// </remarks>
		public RectangleF NormalizedRectangle
		{
			get { return _normalizedRectangle; }
			set
			{
				RectangleUtilities.VerifyNormalizedRectangle(value);
				_normalizedRectangle = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="DisplaySet"/> associated with this <see cref="ImageBox"/>.
		/// </summary>
		/// <value>The <see cref="DisplaySet"/> associated with this <see cref="ImageBox"/>.
		/// <b>null</b> if the <see cref="ImageBox"/> is empty.</value>
		/// <remarks>
		/// Setting this property to a <see cref="DisplaySet"/> automatically populates the tiles
		/// in this <see cref="ImageBox"/> with presentation images contained in the 
		/// <see cref="DisplaySet"/>.  Any <see cref="DisplaySet"/> previously associated with
		/// this <see cref="ImageBox"/> is removed.  Setting this property to <b>null</b>
		/// results in an empty <see cref="ImageBox"/> and empty tiles.
		/// </remarks>
		public IDisplaySet DisplaySet
		{
			get { return _displaySet; }
			set
			{
				if (DisplaySetLocked)
					throw new InvalidOperationException("The display set is currently locked.");

				DisplaySetChangedEventArgs eventArgs = null;

				// If the display set has changed, remember the change
				if (_displaySet != value)
				{
					eventArgs = new DisplaySetChangedEventArgs(_displaySet, value);
					if (ImageViewer != null)
						ImageViewer.EventBroker.OnDisplaySetChanging(new DisplaySetChangingEventArgs(_displaySet, value));
				}

				// Break association with the old display set (should we dispose too?)
				if (_displaySet != null)
				{
					_displaySet.ImageBox = null;
					_displaySet.Selected = false;
				}

				// Assign the new DisplaySet.  Value can be null.
				_displaySet = value as DisplaySet;

				// If there's no display set associated with this image box,
				// then make sure there are no images associated with any of
				// its tiles.
				if (_displaySet == null)
				{
					foreach (Tile tile in this.Tiles)
						tile.PresentationImage = null;
				}
					// If there *is* a DisplaySet associated with
					// this ImageBox...
				else
				{
					_displaySet.ImageBox = this;

					if (_displaySet.ImageViewer == null)
						_displaySet.ImageViewer = this.ImageViewer;

					// Force the images in the display set to fill
					// the tiles of this image box
					this.TopLeftPresentationImageIndex = 0;
					_displaySet.Selected = this.Selected;
				}

				// If the display set has changed, let subscribers know
				if (eventArgs != null)
				{
					EventsHelper.Fire(_displaySetChangedEvent, this, eventArgs);
					if (ImageViewer != null)
						this.ImageViewer.EventBroker.OnDisplaySetChanged(eventArgs);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether or not <see cref="DisplaySet"/> is locked and cannot currently be changed.
		/// </summary>
		/// <remarks>
		/// The intended usage of this property is such that code responsible for the layout
		/// of the <see cref="IImageViewer"/> manage this property's state, and other parts of the code,
		/// like tools for example, should simply check the value and respond appropriately.
		/// </remarks>
		public bool DisplaySetLocked { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ImageBox"/> is
		/// selected.
		/// </summary>
		/// <remarks>
		/// <see cref="ImageBox"/> selection is mutually exclusive.  That is,
		/// only one <see cref="ImageBox"/> is ever selected at a given time.  
		/// </remarks>
		public bool Selected
		{
			get { return _selected; }
			private set
			{
				if (_selected != value)
				{
					_selected = value;
					EventsHelper.Fire(_selectionChangedEvent, this, new ItemEventArgs<IImageBox>(this));
				}
			}
		}

		/// <summary>
		/// Gets the currently selected <see cref="ITile"/>.
		/// </summary>
		public ITile SelectedTile
		{
			get { return _selectedTile; }
			internal set
			{
				// Deselect the old Tile
				if (_selectedTile != null)
					_selectedTile.Deselect();

				_selectedTile = value as Tile;

				if (_selectedTile == null)
					return;

				// Select this ImageBox
				Select();
			}
		}

		/// <summary>
		/// Gets the array index of the currently selected <see cref="ITile"/>.
		/// </summary>
		public int IndexOfSelectedTile
		{
			get
			{
				if (this.SelectedTile == null)
					return -1;
				else
					return this.Tiles.IndexOf(this.SelectedTile);
			}
		}

		/// <summary>
		/// Gets or sets the colour of the border when the image box
		/// is selected.
		/// </summary>
		public static Color SelectedColor
		{
			get { return _selectedColor; }
			set { _selectedColor = value; }
		}

		/// <summary>
		/// Gets or sets the colour of the border when the image box
		/// is not selected.
		/// </summary>
		public static Color UnselectedColor
		{
			get { return _unselectedColor; }
			set { _unselectedColor = value; }
		}

		/// <summary>
		/// Gets the current border colour.
		/// </summary>
		public Color BorderColor
		{
			get
			{
				if (this.Selected)
					return _selectedColor;
				else
					return _unselectedColor;
			}
		}

		/// <summary>
		/// Gets or sets the width of the border in pixels.
		/// </summary>
		public static int BorderWidth
		{
			get { return _borderWidth; }
			set { _borderWidth = value; }
		}

		/// <summary>
		/// Gets or sets the inset width of the border in pixels.
		/// </summary>
		public static int InsetWidth
		{
			get { return _insetWidth; }
			set { _insetWidth = value; }
		}

		/// <summary>
		/// Gets the number of rows of tiles in this <see cref="ImageBox"/>.
		/// </summary>
		public int Rows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the number of columns of tiles in this <see cref="ImageBox"/>.
		/// </summary>
		public int Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Gets or sets the <see cref="IPresentationImage"/> in the top-left 
		/// <see cref="Tile"/> of this <see cref="ImageBox"/>.
		/// </summary>
		/// <remarks>
		/// Because a <see cref="IDisplaySet"/> is an <i>ordered</i> set of 
		/// presentation images, setting this property to a specified
		/// <see cref="IPresentationImage"/> image results in the images that follow 
		/// to "flow" into the other tiles from left to right, top to bottom so that
		/// order is preserved.
		/// </remarks>
		/// <exception cref="ArgumentException"><b>TopLeftPresentationImage</b>
		/// is not found this image box's <see cref="IDisplaySet"/></exception>
		public IPresentationImage TopLeftPresentationImage
		{
			get
			{
				if (this.Tiles.Count == 0)
					return null;

				ITile tile = this.Tiles[0];

				return tile.PresentationImage;
			}
			set
			{
				if (this.DisplaySet == null)
					return;

				Platform.CheckForNullReference(value, "TopLeftPresentationImage");

				// If specified presentationImage cannot be found in DisplaySet, an
				// exception will be thrown in DisplaySet.IndexOfPresentationImage
				int imageIndex = _displaySet.PresentationImages.IndexOf(value);

				FlowImages(imageIndex);
			}
		}

		/// <summary>
		/// Gets or sets the index of the <see cref="PresentationImage"/> that is
		/// to be placed in the top-left <see cref="Tile"/> of this <see cref="ImageBox"/>.
		/// </summary>
		/// <remarks>
		/// The index is the index of the <see cref="PresentationImage"/> in the
		/// <see cref="DisplaySet"/>.  Because a <see cref="DisplaySet"/> is an 
		/// <i>ordered</i> set of presentation images, setting this property to a specified
		/// <see cref="PresentationImage"/> image results in the images that follow 
		/// to "flow" into the other tiles from left to right, top to bottom so that
		/// order is preserved.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <see cref="TopLeftPresentationImageIndex"/> is less than 0 or greater 
		/// than or equal to the number of presentation images in this
		/// image box's <see cref="IDisplaySet"/></exception>
		public int TopLeftPresentationImageIndex
		{
			get
			{
				if (this.TopLeftPresentationImage == null)
					return -1;
				else
					return _displaySet.PresentationImages.IndexOf(this.TopLeftPresentationImage);
			}
			set
			{
				if (this.DisplaySet == null || this.DisplaySet.PresentationImages.Count == 0)
					return;

				int imageIndex;

				if (value < 0)
					imageIndex = 0;
				else if (value > this.DisplaySet.PresentationImages.Count - 1)
					imageIndex = this.DisplaySet.PresentationImages.Count - 1;
				else
					imageIndex = value;

				FlowImages(imageIndex);
			}
		}

		/// <summary>
		/// Gets the <see cref="Tile"/> at the specified row and column.
		/// </summary>
		/// <exception cref="IndexOutOfRangeException"><pararef name="row"/> or
		/// <pararef name="column"/> is less than 0 or is greater than or equal to 
		/// the <see cref="ImageBox.Rows"/> or <see cref="ImageBox.Columns"/> respectively.
		/// </exception>
		public ITile this[int row, int column]
		{
			get
			{
				Platform.CheckIndexRange(row, 0, this.Rows - 1, this);
				Platform.CheckIndexRange(column, 0, this.Columns - 1, this);

				int index = row*this.Columns + column;
				return this.Tiles[index];
			}
		}

		/// <summary>
		/// Gets or sets whether the image box is currently enabled.
		/// </summary>
		public bool Enabled
		{
			get { return _parentPhysicalWorkspace.Enabled ? _enabled : false; }
			set { _enabled = value; }
		}

		public ExtensionData ExtensionData { get; private set; }

		#endregion

		#region Public events

		/// <summary>
		/// Occurs when the <see cref="ImageBox"/> is about to be drawn.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawingEvent += value; }
			remove { _drawingEvent -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="Selected"/> property has changed.
		/// </summary>
		public event EventHandler<ItemEventArgs<IImageBox>> SelectionChanged
		{
			add { _selectionChangedEvent += value; }
			remove { _selectionChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="DisplaySet"/> property has changed.
		/// </summary>
		public event EventHandler<DisplaySetChangedEventArgs> DisplaySetChanged
		{
			add { _displaySetChangedEvent += value; }
			remove { _displaySetChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when an <see cref="ITile"/> has been added.
		/// </summary>
		public event EventHandler<ItemEventArgs<ITile>> TileAdded
		{
			add { _tileAddedEvent += value; }
			remove { _tileAddedEvent -= value; }
		}

		/// <summary>
		/// Occurs when an <see cref="ITile"/> has been removed.
		/// </summary>
		public event EventHandler<ItemEventArgs<ITile>> TileRemoved
		{
			add { _tileRemovedEvent += value; }
			remove { _tileRemovedEvent -= value; }
		}

		/// <summary>
		/// Occurs after all <see cref="Tile"/> objects have been added
		/// in the <see cref="SetTileGrid"/> method.
		/// </summary>
		public event EventHandler LayoutCompleted
		{
			add { _layoutCompletedEvent += value; }
			remove { _layoutCompletedEvent -= value; }
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="ImageBox"/>.
		/// </summary>
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
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeTiles();
				_tiles = null;

				if (ExtensionData != null)
				{
					ExtensionData.Dispose();
					ExtensionData = null;
				}
			}
		}

		private void DisposeTiles()
		{
			if (this.Tiles == null)
				return;

			foreach (Tile tile in this.Tiles)
				tile.Dispose();

			this.Tiles.Clear();
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Creates a rectangular grid of tiles.
		/// </summary>
		/// <remarks>
		/// Each time this method is called, existing tiles in the <see cref="ImageBox"/>
		/// are removed and new ones added.  The exception is when the number of rows
		/// and columns has not changed, in which case the method does nothing
		/// and returns immediately.
		/// </remarks>
		/// <param name="numberOfRows">Number of <see cref="ImageBox"/> rows.</param>
		/// <param name="numberOfColumns">Number of <see cref="ImageBox"/> columns.</param>
		/// <exception cref="ArgumentException"><paramref name="numberOfRows"/> or 
		/// <paramref name="numberOfColumns"/> is less than 1.</exception>
		public void SetTileGrid(int numberOfRows, int numberOfColumns)
		{
			Platform.CheckPositive(numberOfRows, "numberOfRows");
			Platform.CheckPositive(numberOfColumns, "numberOfColumns");

			// Don't bother if nothing's changed.
			if (numberOfRows == this.Rows &&
			    numberOfColumns == this.Columns)
				return;

			_rows = numberOfRows;
			_columns = numberOfColumns;

			DisposeTiles();

			double tileWidth = 1.0d/numberOfColumns;
			double tileHeight = 1.0d/numberOfRows;

			for (int row = 0; row < numberOfRows; row++)
			{
				for (int column = 0; column < numberOfColumns; column++)
				{
					double x = column*tileWidth;
					double y = row*tileHeight;
					RectangleF rect = new RectangleF((float) x, (float) y, (float) tileWidth, (float) tileHeight);

					Tile tile = new Tile();
					tile.NormalizedRectangle = rect;
					this.Tiles.Add(tile);
				}
			}

			EventsHelper.Fire(_layoutCompletedEvent, this, EventArgs.Empty);
		}

		/// <summary>
		/// Helper method for setting the display set of this image box.
		/// </summary>
		/// <param name="displaySet">The display set to show in this image box, or NULL to clear it.</param>
		/// <param name="createFreshCopy">Value indicating whether or not <see cref="IDisplaySet.CreateFreshCopy"/> should be called on the display set.</param>
		/// <param name="addToCommandHistory">Value indicating whether or not the viewer command history should be updated.</param>
		/// <param name="selectDefaultTile">Value indicating whether or not <see cref="IImageBox.SelectDefaultTile"/> should be called after updating the display set.</param>
		public void SetDisplaySet(IDisplaySet displaySet, bool createFreshCopy = false, bool addToCommandHistory = true, bool selectDefaultTile = true)
		{
			var memorableCommand = addToCommandHistory ? new MemorableUndoableCommand(this) {BeginState = CreateMemento()} : null;

			DisplaySet = displaySet != null && createFreshCopy ? displaySet.CreateFreshCopy() : displaySet;
			Draw();
			if (selectDefaultTile) SelectDefaultTile();

			if (addToCommandHistory)
			{
				memorableCommand.EndState = CreateMemento();

				var historyCommand = new DrawableUndoableCommand(this);
				historyCommand.Enqueue(memorableCommand);
				ImageViewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		/// <summary>
		/// Draws the <see cref="ImageBox"/>.
		/// </summary>
		public void Draw()
		{
			// Let others know that we're about to draw
			ImageBoxDrawingEventArgs args = new ImageBoxDrawingEventArgs(this);
			if (this.ImageViewer != null && this.ImageViewer.EventBroker != null)
				this.ImageViewer.EventBroker.OnImageBoxDrawing(args);

			EventsHelper.Fire(_drawingEvent, this, EventArgs.Empty);
		}

		/// <summary>
		/// Selects the top left tile.
		/// </summary>
		public void SelectDefaultTile()
		{
			ITile topLeftTile = this.Tiles[0];

			if (topLeftTile != null)
				topLeftTile.Select();
		}

		private void Select()
		{
			if (!this.Selected)
			{
				//Platform.CheckMemberIsSet(this.DisplaySet, "ImageBox.DisplaySet");
				Platform.CheckMemberIsSet(this.ParentPhysicalWorkspace, "ImageBox.ParentPhysicalWorkspace");
				Platform.CheckMemberIsSet(this.ImageViewer, "ImageBox.ImageViewer");

				this.Selected = true;
				_parentPhysicalWorkspace.SelectedImageBox = this;
				this.ImageViewer.EventBroker.OnImageBoxSelected(new ImageBoxSelectedEventArgs(this));

				if (_displaySet != null)
					_displaySet.Selected = true;
			}
		}

		#region IMemorable Members

		/// <summary>
		/// Creates a memento for this <see cref="ImageBox"/>.
		/// </summary>
		/// <returns>A memento for this <see cref="ImageBox"/>.</returns>
		/// <remarks>
		/// This method is used to remember the current state of a
		/// <see cref="ImageBox"/>.  The memento remembers the actual <see cref="Tile"/>
		/// <i>instances</i> contained in the <see cref="ImageBox"/>.  Calling
		/// <see cref="ImageBox.SetMemento"/> at a later time restores those instances.
		/// </remarks>
		public virtual object CreateMemento()
		{
			object displaySetMemento = null;
			if (this.DisplaySet != null)
				displaySetMemento = this.DisplaySet.CreateMemento();

			ImageBoxMemento imageBoxMemento =
				new ImageBoxMemento(this.DisplaySet,
				                    this.DisplaySetLocked,
				                    displaySetMemento,
				                    this.Rows,
				                    this.Columns,
				                    this.TopLeftPresentationImageIndex,
				                    this.NormalizedRectangle,
				                    this.IndexOfSelectedTile);

			return imageBoxMemento;
		}

		/// <summary>
		/// Sets this <see cref="ImageBox"/> with a previously created memento.
		/// </summary>
		/// <param name="memento">Memento to set.</param>
		/// <remarks>
		/// This method restores the state of a <see cref="ImageBox"/> with
		/// a memento previously created by <see cref="ImageBox.CreateMemento"/>.
		/// </remarks>
		public virtual void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");

			ImageBoxMemento imageBoxMemento = (ImageBoxMemento) memento;

			if (imageBoxMemento.Rows != this.Rows || imageBoxMemento.Columns != this.Columns)
			{
				_rows = 0;
				_columns = 0;

				if (imageBoxMemento.Rows > 0 && imageBoxMemento.Columns > 0)
					this.SetTileGrid(imageBoxMemento.Rows, imageBoxMemento.Columns);
			}

			_normalizedRectangle = RectangleF.Empty;

			DisplaySetLocked = false;
			this.DisplaySet = imageBoxMemento.DisplaySet;
			if (this.DisplaySet != null)
				this.DisplaySet.SetMemento(imageBoxMemento.DisplaySetMemento);

			this.DisplaySetLocked = imageBoxMemento.DisplaySetLocked;

			this.NormalizedRectangle = imageBoxMemento.NormalizedRectangle;

			if (imageBoxMemento.TopLeftPresentationImageIndex != -1)
				this.TopLeftPresentationImageIndex = imageBoxMemento.TopLeftPresentationImageIndex;

			if (imageBoxMemento.IndexOfSelectedTile != -1)
			{
				ITile selectedTile = this.Tiles[imageBoxMemento.IndexOfSelectedTile];
				selectedTile.Select();
			}
		}

		#endregion

		#endregion

		#region Internal/private methods

		internal void Deselect()
		{
			if (this.Selected)
			{
				//Platform.CheckMemberIsSet(this.DisplaySet, "ImageBox.DisplaySet");

				this.Selected = false;

				if (_displaySet != null)
					_displaySet.Selected = false;

				if (this.SelectedTile != null)
				{
					_selectedTile.Deselect();
					_selectedTile = null;
				}
			}
		}

		private void FlowImages(int imageIndex)
		{
			Platform.CheckArgumentRange(imageIndex, 0, this.DisplaySet.PresentationImages.Count, "startIndex");

			int startImageIndex;
			int maxStartImageIndex = _displaySet.PresentationImages.Count - this.Tiles.Count;

			// Case when there are as many or more images than tiles
			if (maxStartImageIndex >= 0)
			{
				if (imageIndex > maxStartImageIndex)
					startImageIndex = maxStartImageIndex;
				else
					startImageIndex = imageIndex;
			}
				// Case when there are fewer images than tiles
			else
			{
				startImageIndex = 0;
			}

			// If the starting index is less than the top-left index, or if there's no
			// image in the top left tile yet, then we iterate through the tiles in ascending order.
			// Otherwise, we iterate through in descending order.
			// The reason is this:  Consider the example when we have 4 images in 4 tiles, where
			// the number corresponds to the image, and the position in the list corresponds to the
			// tile in which the images resides.  If we want to reflow the images so that the first image is 3,
			// this is what the list would look like after each iteration:
			//
			// 2,3,4,5
			// 3,3,4,5
			// 3,4,4,5
			// 3,4,5,5
			// 3,4,5,6
			//
			// The problem here is that at the end of any given iteration, the same image can be
			// in two tiles simultaneously.  This causes a number synchronization problems.  What we want
			// is for there to ever only be one instance of an image in the current tiles.
			// The way to do this is to iterate through the list backwards like this:
			//
			// 2,3,4,5
			// 2,3,4,6
			// 2,3,5,6
			// 2,4,5,6
			// 3,4,5,6
			if (startImageIndex <= this.TopLeftPresentationImageIndex ||
			    this.TopLeftPresentationImageIndex == -1)
			{
				for (int tileIndex = 0; tileIndex < this.Tiles.Count; tileIndex++)
					SetImage(startImageIndex, tileIndex);
			}
			else
			{
				for (int tileIndex = this.Tiles.Count - 1; tileIndex >= 0; tileIndex--)
					SetImage(startImageIndex, tileIndex);
			}
		}

		private void SetImage(int startImageIndex, int tileIndex)
		{
			Tile tile = this.Tiles[tileIndex] as Tile;

			// If there's an image, put it in a tile
			if (startImageIndex + tileIndex < _displaySet.PresentationImages.Count)
			{
				tile.PresentationImage = this.DisplaySet.PresentationImages[startImageIndex + tileIndex];
			}
				// If there are no images left (the case when there are fewer images than tiles)
				// then just set the tile to blank
			else
			{
				tile.PresentationImage = null;
				tile.Deselect();
			}
		}

		private void OnTileAdded(object sender, ListEventArgs<ITile> e)
		{
			Tile tile = (Tile) e.Item;
			tile.ImageViewer = this.ImageViewer;
			tile.ParentImageBox = this;
			EventsHelper.Fire(_tileAddedEvent, this, new ItemEventArgs<ITile>(e.Item));
		}

		private void OnTileRemoved(object sender, ListEventArgs<ITile> e)
		{
			if (e.Item.Selected)
				this.SelectedTile = null;

			EventsHelper.Fire(_tileRemovedEvent, this, new ItemEventArgs<ITile>(e.Item));
		}

		#endregion
	}
}