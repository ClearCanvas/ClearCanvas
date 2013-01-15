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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer
{
    //TODO: Update documentation, particular for how and when LayoutCompleted fires, because it has changed.
	/// <summary>
	/// A container for image boxes.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="PhysicalWorkspace"/> and its related classes <see cref="ImageBox"/> and
	/// <see cref="Tile"/> collectively describe how images are positioned and sized
	/// on the screen.  A <see cref="PhysicalWorkspace"/> contains zero or more image 
	/// boxes, which in turn each contain zero or more tiles.  Image boxes can be 
	/// arranged arbitrarily in a workspace whereas tiles in an image box must be laid 
	/// out as a rectangular grid. A tile contains a <see cref="PresentationImage"/>.
	/// </para>
	/// <para>
	/// Physical workspace layouts are described in normalized coordinates.
	/// The top left corner of the workspace corresponds to (0,0) and the bottom right
	/// (1,1).  When an <see cref="IImageBox"/> is defined and added to a workspace, 
	/// it is done using that coordinate system.  See 
	/// <see cref="IImageBox.NormalizedRectangle"/> for an example.
	/// </para>
	/// </remarks>
	public class PhysicalWorkspace : IPhysicalWorkspace
	{
		#region Private Fields

		private readonly IImageViewer _imageViewer;
		private ImageBoxCollection _imageBoxes;
		private ImageBox _selectedImageBox;
		private int _rows;
		private int _columns;
		private bool _enabled = true;
	    private bool _needLayoutCompleted = true;
        private event EventHandler _drawingEvent;
		private event EventHandler _layoutCompletedEvent;
		private event EventHandler _enabledChanged;
		private event EventHandler _lockedChanged;

		private Rectangle _screenRectangle;
		private event EventHandler _screenRectangleChanged;

		#endregion

		internal PhysicalWorkspace(IImageViewer imageViewer)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");

            _imageViewer = imageViewer;
			this.ImageBoxes.ItemAdded += OnImageBoxAdded;
			this.ImageBoxes.ItemRemoved += OnImageBoxRemoved;
		}

		#region Public properties

		/// <summary>
		/// Gets the collection of <see cref="IImageBox"/> objects that belong
		/// to this <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>When a <see cref="PhysicalWorkspace "/> is first instantiated,
		/// <see cref="ImageBoxes"/> is empty.</remarks>
		public ImageBoxCollection ImageBoxes
		{
			get 
			{
				if (_imageBoxes == null)
					_imageBoxes = new ImageBoxCollection();

				return _imageBoxes; 
			}
		}

		/// <summary>
		/// Gets the number of rows of <see cref="IImageBox"/> objects in the
		/// <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Rows"/> is <i>only</i> valid if <see cref="SetImageBoxGrid(int, int)"/> has
		/// been called.  Otherwise, the value is meaningless.
		/// </remarks>
		public int Rows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the number of columns of <see cref="IImageBox"/> objects in the
		/// <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Columns"/> is <i>only</i> valid if <see cref="SetImageBoxGrid(int, int)"/> has
		/// been called.  Otherwise, the value is meaningless.
		/// </remarks>
		public int Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Returns the image box at a specified row and column index.
		/// </summary>
		/// <param name="row">the zero-based row index of the image box to retrieve</param>
		/// <param name="column">the zero-based column index of the image box to retrieve</param>
		/// <returns>the image box at the specified row and column indices</returns>
		/// <remarks>This method is only valid if <see cref="SetImageBoxGrid(int, int)"/> has been called and/or the 
		/// layout is, in fact, rectangular.</remarks>
		/// <exception cref="InvalidOperationException">Thrown if the layout is not currently rectangular</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if either of the row/column indices are out of range</exception>
		public IImageBox this[int row, int column]
		{
			get
			{
				if (this.Rows <= 0 || this.Columns <= 0)
					throw new InvalidOperationException(SR.ExceptionLayoutIsNotRectangular);

				Platform.CheckArgumentRange(row, 0, this.Rows - 1, "row");
				Platform.CheckArgumentRange(column, 0, this.Columns - 1, "column");
				
				return this.ImageBoxes[row * this.Columns + column];
			}
		}

		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		/// <summary>
		/// Gets the associated <see cref="ILogicalWorkspace"/>.
		/// </summary>
		public ILogicalWorkspace LogicalWorkspace
		{
			get { return this.ImageViewer.LogicalWorkspace; }
		}

		/// <summary>
		/// Gets the selected <see cref="IImageBox"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IImageBox"/>, or <b>null</b> if
		/// no <see cref="IImageBox"/> is currently selected.</value>
		public IImageBox SelectedImageBox
		{
			get { return _selectedImageBox; }
			internal set
			{
				if (_selectedImageBox != null)
					_selectedImageBox.Deselect();

				_selectedImageBox = value as ImageBox;
			}
		}

		/// <summary>
		/// Gets or sets whether the workspace is currently enabled.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets whether the <see cref="IPhysicalWorkspace"/>'s layout
		/// is locked.
		/// </summary>
		/// <remarks>
		/// This property is useful for cases where a read-only layout is appropriate/desired.
		/// </remarks>
		public bool Locked
		{
			get { return ImageBoxes.Locked; }
			set
			{
				if (ImageBoxes.Locked == value)
					return;
				
				SetLocked(value);
				EventsHelper.Fire(_lockedChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets the rectangle that the <see cref="IPhysicalWorkspace"/> occupies
		/// in virtual screen coordinates.
		/// </summary>
		public Rectangle ScreenRectangle
		{
			get { return _screenRectangle; }
			set 
			{
				if (_screenRectangle.Equals(value))
					return;

				_screenRectangle = value;
				OnScreenRectangleChanged();
			}
		}

		#endregion

		#region Public events

		/// <summary>
		/// Occurs when the <see cref="PhysicalWorkspace"/> is drawn.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawingEvent += value; }
			remove { _drawingEvent -= value; }
		}

		/// <summary>
		/// Occurs when <see cref="Enabled"/> has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Occurs when <see cref="Locked"/> has changed.
		/// </summary>
		public event EventHandler LockedChanged
		{
			add { _lockedChanged += value; }
			remove { _lockedChanged -= value; }
		}

		/// <summary>
		/// Occurs when all changes to image box collection are complete.
		/// </summary>
		/// <remarks>
		/// <see cref="LayoutCompleted"/> is raised by the Framework when
		/// <see cref="SetImageBoxGrid(int, int)"/> has been called.  If you are adding/removing
		/// <see cref="IImageBox"/> objects manually, you should raise this event when
		/// you're done by calling <see cref="OnLayoutCompleted"/>.  This event is
		/// consumed by the view to reduce flicker when layouts are changed.  
		/// In that way, it is similar to the WinForms methods <b>SuspendLayout</b>
		/// and <b>ResumeLayout</b>.
		/// </remarks>
		public event EventHandler LayoutCompleted
		{
			add { _layoutCompletedEvent += value; }
			remove { _layoutCompletedEvent -= value; }
		}

		/// <summary>
		/// Occurs when <see cref="IPhysicalWorkspace.ScreenRectangle"/> changes.
		/// </summary>
		public event EventHandler ScreenRectangleChanged
		{
			add { _screenRectangleChanged += value; }
			remove { _screenRectangleChanged -= value; }
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="PhysicalWorkspace"/>.
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
				SetLocked(false);
				DisposeImageBoxes();
			}
		}

		private void DisposeImageBoxes()
		{
			if (this.ImageBoxes == null)
				return;

			SetLocked(false);

			foreach (ImageBox imageBox in this.ImageBoxes)
				imageBox.Dispose();

			_imageBoxes = null;
		}

		private void SetLocked(bool value)
		{
			ImageBoxes.Locked = value;
			foreach (IImageBox box in ImageBoxes)
				box.Tiles.Locked = value;
		}

		#endregion

		#region Public methods

		//TODO (cr Oct 2009): SetImageBoxGrid(imageBox[,]) would allow the same physical image box to be reused
		//which has a lot of advantages.

		/// <summary>
		/// Creates a rectangular <see cref="IImageBox"/> grid.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// <see cref="SetImageBoxGrid(int, int)"/> is a convenience method that adds
		/// <see cref="IImageBox"/> objects to the <see cref="IPhysicalWorkspace"/>
		/// in a rectangular grid.
		/// </remarks>
		/// <exception cref="ArgumentException"><paramref name="rows"/> or 
		/// <paramref name="columns"/> is less than 1.</exception>
		public void SetImageBoxGrid(int rows, int columns)
		{
			Platform.CheckPositive(rows, "rows");
			Platform.CheckPositive(columns, "columns");

			if (_rows == rows && _columns == columns)
				return;

			this.ImageBoxes.Clear();
			for (int i = 0; i < rows * columns; ++i)
				this.ImageBoxes.Add(new ImageBox());

			_rows = rows;
			_columns = columns;

		    _needLayoutCompleted = true;
			SetImageBoxGrid();
		}

		/// <summary>
		/// Raises the <see cref="LayoutCompleted"/> event.
		/// </summary>
		/// <remarks>
		/// If you are adding/removing <see cref="IImageBox"/> objects manually 
		/// (i.e., instead of using <see cref="SetImageBoxGrid(int, int)"/>), you should call
		/// <see cref="OnLayoutCompleted"/> to raise the <see cref="LayoutCompleted"/> event.  
		/// This event is consumed by the view to reduce flicker when layouts are changed.  
		/// In that way, it is similar to the WinForms methods <b>SuspendLayout</b>
		/// and <b>ResumeLayout</b>.
		/// </remarks>
		public void OnLayoutCompleted()
		{
            //Force the normalized rectangles to be correct on layout completed.
		    SetImageBoxGrid();
            EventsHelper.Fire(_layoutCompletedEvent, this, EventArgs.Empty);
            _needLayoutCompleted = false;
		}

		/// <summary>
		/// Selects the first <see cref="IImageBox"/> in the image box collection.
		/// </summary>
		/// <remarks>
		/// When <see cref="SetImageBoxGrid(int, int)"/> has been used to setup the 
		/// <see cref="IPhysicalWorkspace"/>, the first <see cref="IImageBox"/> in the
		/// image box collection will be the top-left <see cref="IImageBox"/>.
		/// </remarks>
		public void SelectDefaultImageBox()
		{
			if (this.ImageBoxes.Count > 0)
			{
                foreach(IImageBox imageBox in ImageBoxes)
                {
                    if (imageBox.DisplaySet != null)
                    {
                        imageBox.SelectDefaultTile();
                        return;
                    }
                }
				
			}
		}

		/// <summary>
		/// Draws the <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		public void Draw()
		{
            if (_needLayoutCompleted)
                OnLayoutCompleted();

			EventsHelper.Fire(_drawingEvent, this, EventArgs.Empty);
		}

		#region IMemorable Members

		/// <summary>
		/// Creates a memento for this <see cref="PhysicalWorkspace"/>.
		/// </summary>
		/// <returns>A memento for this <see cref="PhysicalWorkspace"/>.</returns>
		/// <remarks>
		/// This method is used to remember the current state of a
		/// <see cref="PhysicalWorkspace"/>.  The memento remembers the actual <see cref="ImageBox"/>
		/// <i>instances</i> contained in the <see cref="PhysicalWorkspace"/>.  Calling
		/// <see cref="SetMemento"/> at a later time restores 
		/// those instances.
		/// </remarks>
		public virtual object CreateMemento()
		{
			List<object> imageBoxMementos = new List<object>();

			foreach (ImageBox imageBox in this.ImageBoxes)
				imageBoxMementos.Add(imageBox.CreateMemento());

			PhysicalWorkspaceMemento workspaceMemento =
				new PhysicalWorkspaceMemento(new ImageBoxCollection(this.ImageBoxes),
											 imageBoxMementos, 
											 this.Rows,
											 this.Columns);

			return workspaceMemento;
		}

		/// <summary>
		/// Sets this <see cref="PhysicalWorkspace"/> with a previously created memento.
		/// </summary>
		/// <param name="memento">Memento to set.</param>
		/// <remarks>
		/// This method restores the state of a <see cref="PhysicalWorkspace"/> with
		/// a memento previously created by <see cref="CreateMemento"/>.
		/// </remarks>
		public virtual void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");

			PhysicalWorkspaceMemento workspaceMemento = (PhysicalWorkspaceMemento) memento;

			//locked is not part of the memento.
			bool locked = Locked;
			SetLocked(false);

			this.ImageBoxes.Clear();
			
			for (int i = 0; i < workspaceMemento.ImageBoxes.Count; i++)
			{
				object imageBoxMemento = workspaceMemento.ImageBoxMementos[i];
				IImageBox imageBox = workspaceMemento.ImageBoxes[i];
				imageBox.SetMemento(imageBoxMemento);

				this.ImageBoxes.Add(imageBox);
			}

			_rows = workspaceMemento.Rows;
			_columns = workspaceMemento.Columns;

			SetImageBoxGrid();

			//restore locked state.
			SetLocked(locked);

			OnLayoutCompleted();

			//The command in the command history should be drawable.
			//Draw();
		}

		#endregion

		#endregion

	    #region Private methods

		private void OnImageBoxAdded(object sender, ListEventArgs<IImageBox> e)
		{
		    _needLayoutCompleted = true;
            _rows = _columns = -1;

			ImageBox imageBox = (ImageBox)e.Item;
			imageBox.ImageViewer = this.ImageViewer;
			imageBox.ParentPhysicalWorkspace = this;
		}

		private void OnImageBoxRemoved(object sender, ListEventArgs<IImageBox> e)
		{
            _needLayoutCompleted = true;
            _rows = _columns = -1;

			if (e.Item.Selected)
				this.SelectedImageBox = null;

			e.Item.DisplaySetLocked = false;
			e.Item.DisplaySet = null;
		}

		private void OnScreenRectangleChanged()
		{
			SetImageBoxGrid();
			EventsHelper.Fire(_screenRectangleChanged, this, EventArgs.Empty);
		}

		private void SetImageBoxGrid()
		{
			if (_rows < 0 || _columns < 0)
				return;

			bool succeeded = false;

			try
			{
				succeeded = OptimizeImageBoxGrid();
				if (!succeeded)
					Platform.Log(LogLevel.Debug, "Failed to optimize image box rectangles; defaulting to simple method.");
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Failed to optimize image box rectangles; defaulting to simple method.");
			}

			if (!succeeded)
			{
				//just do the default.
				double imageBoxWidth = (1.0d / _columns);
				double imageBoxHeight = (1.0d / _rows);

				for (int row = 0; row < _rows; row++)
				{
					for (int column = 0; column < _columns; column++)
					{
						double x = column * imageBoxWidth;
						double y = row * imageBoxHeight;
						this[row, column].NormalizedRectangle = new RectangleF((float)x, (float)y, (float)imageBoxWidth, (float)imageBoxHeight);
					}
				}
			}
		}

		// Note: this algorithm is not bulletproof, but will always work when the workspace is completely visible
		// on any combination of monitors.  More specifically, where it starts to fail is when the center of the (default positioned)
		// image box falls off of all of the screens.  In those rare/extreme cases, we don't try to do any optimization
		// and use the simple rectangle calculation.
		private bool OptimizeImageBoxGrid()
		{
			Rectangle usableWorkspaceArea = GetUsableArea();

			if (usableWorkspaceArea.IsEmpty || usableWorkspaceArea.Width < 20 || usableWorkspaceArea.Height < 20)
				return false;

			float rectangleWidth = usableWorkspaceArea.Width / (float)_columns;
			float rectangleHeight = usableWorkspaceArea.Height / (float)_rows;

			var imageBoxesSet = new List<IImageBox>();

			foreach (Screen screen in Screen.AllScreens)
			{
				int firstRow = -1;
				int lastRow = -1;
				int firstColumn = -1;
				int lastColumn = -1;

				//Find the first and last row/column defining a sub-grid that will be kept on this screen.
				//We use the center of each image box in the non-optimized case to determine what screen
				//a particular image box should be on.
				for (int row = 0; row < _rows; ++row)
				{
					for (int column = 0; column < _columns; ++column)
					{
						int rectangleCentreX = usableWorkspaceArea.Left + (int)(rectangleWidth * (column + 0.5F));
						int rectangleCentreY = usableWorkspaceArea.Top + (int)(rectangleHeight * (row + 0.5F));

						if (screen.Bounds.Contains(rectangleCentreX, rectangleCentreY))
						{
							if (firstRow < 0)
								firstRow = row;

							lastRow = row;

							if (firstColumn < 0)
								firstColumn = column;

							lastColumn = column;
						}
					}
				}

				Rectangle workspaceArea = ScreenRectangle;
				Rectangle screenUsableArea = Rectangle.Intersect(workspaceArea, screen.WorkingArea);

				//Subdivide the occupied screen area into a grid based on the start/end row/column determined above.
				if (!screenUsableArea.IsEmpty && firstRow >= 0 && firstColumn >= 0)
				{
					RectangleF normalizedScreenUsableArea = RectangleUtilities.CalculateNormalizedSubRectangle(workspaceArea, screenUsableArea);

					int screenRows = lastRow - firstRow + 1;
					int screenColumns = lastColumn - firstColumn + 1;

					float adjustedWidth = normalizedScreenUsableArea.Width / screenColumns;
					float adjustedHeight = normalizedScreenUsableArea.Height / screenRows;

					for (int screenRow = 0; screenRow < screenRows; ++screenRow)
					{
						for (int screenColumn = 0; screenColumn < screenColumns; ++screenColumn)
						{
							IImageBox imageBox = this[firstRow + screenRow, firstColumn + screenColumn];

							//same one was already set; fail.
							if (imageBoxesSet.Contains(imageBox))
								return false;

							imageBoxesSet.Add(imageBox);

							float x = normalizedScreenUsableArea.Left + screenColumn * adjustedWidth;
							float y = normalizedScreenUsableArea.Top + screenRow * adjustedHeight;

							imageBox.NormalizedRectangle = new RectangleF(x, y, adjustedWidth, adjustedHeight);
						}
					}
				}
			}

			//Success only if every image box's rectangle was set exactly once.
			return imageBoxesSet.Count == ImageBoxes.Count;
		}

		private Rectangle GetUsableArea()
		{
			List<Rectangle> intersections = new List<Rectangle>();
			foreach (Screen screen in Screen.AllScreens)
			{
				Rectangle intersection = Rectangle.Intersect(screen.WorkingArea, ScreenRectangle);
				if (!intersection.IsEmpty)
					intersections.Add(intersection);
			}

			if (intersections.Count == 0)
				return Rectangle.Empty;

			Rectangle union = intersections[0];
			for(int i = 1; i < intersections.Count; ++i)
				union = Rectangle.Union(union, intersections[i]);

			return union;
		}

		#endregion
	}
}