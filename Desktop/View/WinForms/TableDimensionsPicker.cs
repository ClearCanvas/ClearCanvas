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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// A control for picking the dimensions of a table.
	/// </summary>
	/// <remarks>
	/// <para>In this control, dimension width and height are equivalent to table columns and rows, respectively.</para>
	/// </remarks>
	[DefaultEvent("DimensionsSelected")]
	[DefaultProperty("Dimensions")]
	public class TableDimensionsPicker : Control
	{
		private static readonly Size _defaultDimensions = new Size(4, 4);
		private event EventHandler _dimensionsChanged;
		private event EventHandler _maxDimensionsChanged;
		private event EventHandler _hotDimensionsChanged;
		private event TableDimensionsEventHandler _dimensionsSelected;

		private Size _hotDimensions;
		private Size _maxDimensions;
		private Size _dimensions;

		#region Constructors

		/// <summary>
		/// Constructs a <see cref="TableDimensionsPicker"/> showing four rows and columns.
		/// </summary>
		public TableDimensionsPicker() : this(_defaultDimensions) {}

		/// <summary>
		/// Constructs a <see cref="TableDimensionsPicker"/> showing the specified number of rows and columns.
		/// </summary>
		/// <remarks>
		/// <para>Dimension width and height are equivalent to table columns and rows, respectively.</para>
		/// </remarks>
		/// <param name="maxRows">The number of rows to show on the control, and hence the maximum the user can select.</param>
		/// <param name="maxCols">The number of columns to show on the control, and hence the maximum the user can select.</param>
		public TableDimensionsPicker(int maxRows, int maxCols) : this(new Size(maxCols, maxRows)) {}

		/// <summary>
		/// Constructs a <see cref="TableDimensionsPicker"/> showing the specified table size.
		/// </summary>
		/// <remarks>
		/// <para>Dimension width and height are equivalent to table columns and rows, respectively.</para>
		/// </remarks>
		/// <param name="maxDimensions">The table size show on the control, and hence the maximum size the user can select.</param>
		public TableDimensionsPicker(Size maxDimensions)
		{
			//Platform
			_maxDimensions = maxDimensions;
			_hotDimensions = _dimensions = Size.Empty;
			ResetCellSpacing();
			ResetCellStyle();
			ResetHotCellStyle();
			ResetSelectedCellStyle();
			base.Size = GetDefaultSize();
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}

		#endregion

		#region Display Option Properties (and their Reset methods)

		#region CellSpacing

		private static readonly string _defaultCellSpacing = new TableDimensionsCellSpacing().ToString();
		private TableDimensionsCellSpacing _cellSpacing;

		/// <summary>
		/// Gets or sets the style of cells in the picker control.
		/// </summary>
		/// <remarks>
		/// The default style is to paint a border using the <see cref="SystemColors.WindowFrame">system frame color</see> and fill
		/// the cell with the <see cref="SystemColors.Control">system control face color</see>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The spacing between adjacent cells in the picker control.")]
		public TableDimensionsCellSpacing CellSpacing
		{
			get { return _cellSpacing; }
			set
			{
				if (_cellSpacing != value)
				{
					if (value == null)
						value = new TableDimensionsCellSpacing();

					if (_cellSpacing != null)
						((INotifyPropertyChanged) _cellSpacing).PropertyChanged -= StyleOrSpacingChanged;

					_cellSpacing = value;
					((INotifyPropertyChanged) _cellSpacing).PropertyChanged += StyleOrSpacingChanged;

					base.Invalidate();
				}
			}
		}

		private void ResetCellSpacing()
		{
			this.CellSpacing = new TableDimensionsCellSpacing();
		}

		private bool ShouldSerializeCellSpacing()
		{
			return this.CellSpacing.ToString() != _defaultCellSpacing;
		}

		#endregion

		#region CellStyle

		private static readonly string _defaultCellStyle = new TableDimensionsCellStyle(SystemColors.Control, SystemColors.WindowFrame, 1).ToString();
		private TableDimensionsCellStyle _cellStyle;

		/// <summary>
		/// Gets or sets the style of cells in the picker control.
		/// </summary>
		/// <remarks>
		/// The default style is to paint a border using the <see cref="SystemColors.WindowFrame">system frame color</see> and fill
		/// the cell with the <see cref="SystemColors.Control">system control face color</see>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The style of cells in the picker control.")]
		public TableDimensionsCellStyle CellStyle
		{
			get { return _cellStyle; }
			set
			{
				if (_cellStyle != value)
				{
					if (value == null)
						value = new TableDimensionsCellStyle();

					if (_cellStyle != null)
						((INotifyPropertyChanged) _cellStyle).PropertyChanged -= StyleOrSpacingChanged;

					_cellStyle = value;
					((INotifyPropertyChanged) _cellStyle).PropertyChanged += StyleOrSpacingChanged;

					base.Invalidate();
				}
			}
		}

		private void ResetCellStyle()
		{
			this.CellStyle = new TableDimensionsCellStyle(SystemColors.Control, SystemColors.WindowFrame, 1);
		}

		private bool ShouldSerializeCellStyle()
		{
			return this.CellStyle.ToString() != _defaultCellStyle;
		}

		#endregion

		#region HotCellStyle

		private static readonly string _defaultHotCellStyle = new TableDimensionsCellStyle(SystemColors.HotTrack).ToString();
		private TableDimensionsCellStyle _hotCellStyle;

		/// <summary>
		/// Gets or sets the style of hot-tracked cells in the picker control.
		/// </summary>
		/// <remarks>
		/// The default style is to fill the hot-tracked cells with the <see cref="SystemColors.HotTrack">system hot-tracking color</see>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The style of hot-tracked cells in the picker control.")]
		public TableDimensionsCellStyle HotCellStyle
		{
			get { return _hotCellStyle; }
			set
			{
				if (_hotCellStyle != value)
				{
					if (value == null)
						value = new TableDimensionsCellStyle();

					if (_hotCellStyle != null)
						((INotifyPropertyChanged) _hotCellStyle).PropertyChanged -= StyleOrSpacingChanged;

					_hotCellStyle = value;
					((INotifyPropertyChanged) _hotCellStyle).PropertyChanged += StyleOrSpacingChanged;

					base.Invalidate();
				}
			}
		}

		private void ResetHotCellStyle()
		{
			this.HotCellStyle = new TableDimensionsCellStyle(SystemColors.HotTrack);
		}

		private bool ShouldSerializeHotCellStyle()
		{
			return this.HotCellStyle.ToString() != _defaultHotCellStyle;
		}

		#endregion

		#region SelectedCellStyle

		private static readonly string _defaultSelectedCellStyle = new TableDimensionsCellStyle().ToString();
		private TableDimensionsCellStyle _selectedCellStyle;

		/// <summary>
		/// Gets or sets the style of currently selected cells in the picker control.
		/// </summary>
		/// <remarks>
		/// The default style is empty.
		/// </remarks>
		[Category("Appearance")]
		[Description("The style of currently selected cells in the picker control.")]
		public TableDimensionsCellStyle SelectedCellStyle
		{
			get { return _selectedCellStyle; }
			set
			{
				if (_selectedCellStyle != value)
				{
					if (value == null)
						value = new TableDimensionsCellStyle();

					if (_selectedCellStyle != null)
						((INotifyPropertyChanged) _selectedCellStyle).PropertyChanged -= StyleOrSpacingChanged;

					_selectedCellStyle = value;
					((INotifyPropertyChanged) _selectedCellStyle).PropertyChanged += StyleOrSpacingChanged;

					base.Invalidate();
				}
			}
		}

		private void ResetSelectedCellStyle()
		{
			this.SelectedCellStyle = new TableDimensionsCellStyle();
		}

		private bool ShouldSerializeSelectedCellStyle()
		{
			return this.SelectedCellStyle.ToString() != _defaultSelectedCellStyle;
		}

		#endregion

		#endregion

		#region Dimension Properties (and their Events and Reset methods)

		/// <summary>
		/// Fired when the <see cref="MaxDimensions"/> property changes.
		/// </summary>
		[Description("Fired when the maximum dimensions change.")]
		[Category("Property Changed")]
		public event EventHandler MaxDimensionsChanged
		{
			add { _maxDimensionsChanged += value; }
			remove { _maxDimensionsChanged -= value; }
		}

		/// <summary>
		/// Notifies listeners that the <see cref="MaxDimensions"/> property has changed.
		/// </summary>
		protected virtual void NotifyMaxDimensionsChanged()
		{
			_hotDimensions = Size.Empty;
			base.Invalidate();

			if (_maxDimensionsChanged != null)
				_maxDimensionsChanged(this, new EventArgs());
		}

		/// <summary>
		/// Gets or sets the maximum dimensions to show on the control.
		/// </summary>
		/// <remarks>
		/// The default maximum dimensions is 4x4.
		/// </remarks>
		[Bindable(true)]
		[Category("Behavior")]
		[Description("The maximum dimensions to show on the control.")]
		public Size MaxDimensions
		{
			get { return _maxDimensions; }
			set
			{
				if (_maxDimensions != value)
				{
					Platform.CheckPositive(value.Height, "value");
					Platform.CheckPositive(value.Width, "value");

					_maxDimensions = value;
					NotifyMaxDimensionsChanged();
				}
			}
		}

		/// <summary>
		/// Resets the <see cref="MaxDimensions"/> property to the default value.
		/// </summary>
		public void ResetMaxDimensions()
		{
			_maxDimensions = _defaultDimensions;
		}

		private bool ShouldSerializeMaxDimensions()
		{
			return _maxDimensions != _defaultDimensions;
		}

		/// <summary>
		/// Fired when the user selects new dimensions.
		/// </summary>
		/// <remarks>
		/// This event is only fired when the <see cref="Dimensions"/> property changes as a result of user action.
		/// Programmatically changing the property does not fired this event. However, both methods will trigger the
		/// <see cref="DimensionsChanged"/> event.
		/// </remarks>
		[Description("Fired when the user selects new dimensions.")]
		[Category("Action")]
		public event TableDimensionsEventHandler DimensionsSelected
		{
			add { _dimensionsSelected += value; }
			remove { _dimensionsSelected -= value; }
		}

		/// <summary>
		/// Notifies listeners that the user has selected new dimensions.
		/// </summary>
		protected virtual void NotifyDimensionsSelected()
		{
			if (_dimensionsSelected != null)
				_dimensionsSelected(this, new TableDimensionsEventArgs(this.Dimensions));
		}

		/// <summary>
		/// Fired when the <see cref="Dimensions"/> property changes.
		/// </summary>
		[Description("Fired when the currently selected dimensions change.")]
		[Category("Property Changed")]
		public event EventHandler DimensionsChanged
		{
			add { _dimensionsChanged += value; }
			remove { _dimensionsChanged -= value; }
		}

		/// <summary>
		/// Notifies listeners that the <see cref="Dimensions"/> property has changed.
		/// </summary>
		/// <param name="newDims">The new value of the property.</param>
		/// <param name="oldDims">The old value of the property.</param>
		protected virtual void NotifyDimensionsChanged(Size newDims, Size oldDims)
		{
			base.Invalidate(GetInvalidRegion(newDims, oldDims));

			if (_dimensionsChanged != null)
				_dimensionsChanged(this, new EventArgs());
		}

		/// <summary>
		/// Gets or sets the currently selected dimensions.
		/// </summary>
		/// <remarks>
		/// The default selected dimensions is 0x0.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the dimensions are greater than the <see cref="MaxDimensions"/>.</exception>
		[Category("Behavior")]
		[Description("The currently selected dimensions.")]
		public Size Dimensions
		{
			get { return _dimensions; }
			set
			{
				if (_dimensions != value)
				{
					Platform.CheckArgumentRange(value.Height, 0, MaxDimensions.Height, "value");
					Platform.CheckArgumentRange(value.Width, 0, MaxDimensions.Width, "value");

					Size old = _dimensions;
					_dimensions = value;
					NotifyDimensionsChanged(old, value);
				}
			}
		}

		/// <summary>
		/// Resets the <see cref="Dimensions"/> property to the default value.
		/// </summary>
		public void ResetDimensions()
		{
			_dimensions = Size.Empty;
		}

		private bool ShouldSerializeDimensions()
		{
			return _dimensions != Size.Empty;
		}

		/// <summary>
		/// Fired when the <see cref="HotDimensions"/> property changes.
		/// </summary>
		[Description("Fired when the hot-tracked dimensions change.")]
		[Category("Property Changed")]
		public event EventHandler HotDimensionsChanged
		{
			add { _hotDimensionsChanged += value; }
			remove { _hotDimensionsChanged -= value; }
		}

		/// <summary>
		/// Notifies listeners that the <see cref="HotDimensions"/> property has changed.
		/// </summary>
		/// <param name="newDims">The new value of the property.</param>
		/// <param name="oldDims">The old value of the property.</param>
		protected virtual void NotifyHotDimensionsChanged(Size newDims, Size oldDims)
		{
			base.Invalidate(GetInvalidRegion(newDims, oldDims));

			if (_hotDimensionsChanged != null)
				_hotDimensionsChanged(this, new EventArgs());
		}

		/// <summary>
		/// Gets the current dimensions that the cursor is hovering over.
		/// </summary>
		/// <remarks>
		/// If the cursor is not over the control, this property is 0x0 (i.e. <see cref="Size.Empty"/>).
		/// </remarks>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Size HotDimensions
		{
			get { return _hotDimensions; }
			private set
			{
				if (_hotDimensions != value)
				{
					Size old = _hotDimensions;
					_hotDimensions = value;
					NotifyHotDimensionsChanged(old, value);
				}
			}
		}

		#endregion

		#region Misc/Helper Properties

		/// <summary>
		/// Gets or sets the maximum number of rows shown on the control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int MaxRows
		{
			get { return this.MaxDimensions.Height; }
			set { this.MaxDimensions = new Size(this.MaxColumns, value); }
		}

		/// <summary>
		/// Gets or sets the maximum number of columns shown on the control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int MaxColumns
		{
			get { return this.MaxDimensions.Width; }
            set { this.MaxDimensions = new Size(value, this.MaxRows); }
		}

		/// <summary>
		/// Gets the row that the cursor is hovering over.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int HotRows
		{
			get { return this.HotDimensions.Height; }
		}

		/// <summary>
		/// Gets the column that the cursor is hovering over.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int HotColumns
		{
			get { return this.HotDimensions.Width; }
		}

		/// <summary>
		/// Gets or sets the currently selected number of rows.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Rows
		{
			get { return this.Dimensions.Height; }
			set { this.Dimensions = new Size(value, this.Columns); }
		}

		/// <summary>
		/// Gets or sets the current selected number of columns.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Columns
		{
			get { return this.Dimensions.Width; }
			set { this.Dimensions = new Size(this.Rows, value); }
		}

		#endregion

		#region System.Windows.Forms.Control Overrides

		/// <summary>
		/// Gets the default size of the control.
		/// </summary>
		protected override Size DefaultSize
		{
			get { return GetDefaultSize(); }
		}

		/// <summary>
		/// This control is always drawn double buffered.
		/// </summary>
		protected override bool DoubleBuffered
		{
			get { return true; }
			set { throw new NotSupportedException("This control is always drawn double buffered."); }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor
		{
			get { return base.ForeColor; }
			set { throw new NotSupportedException("This property is not relevant for this control."); }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Font Font
		{
			get { return base.Font; }
			set { }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Localizable(true)]
		public override RightToLeft RightToLeft
		{
			get { return base.RightToLeft; }
			set { }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text
		{
			get { return string.Empty; }
			set { }
		}

		/// <summary>
		/// Retrieves the size of a rectangular area into which a control can be fitted.
		/// </summary>
		/// <returns>An ordered pair of type <see cref="T:System.Drawing.Size"></see> representing the width and height of a rectangle.</returns>
		/// <param name="proposedSize">The custom-sized area for a control.</param>
		public override Size GetPreferredSize(Size proposedSize)
		{
			return this.DefaultSize;
		}

		#endregion

		#region Event Overrides

		protected override void OnEnabledChanged(EventArgs e)
		{
			if (base.Enabled)
			{
				Point p = base.PointToClient(Cursor.Position);
				if (new Rectangle(Point.Empty, base.Size).Contains(p))
					this.HotDimensions = new Size(GetCellAtPoint(p.X, p.Y));
			}
			else
			{
				this.HotDimensions = Size.Empty;
			}
			base.OnEnabledChanged(e);
		}

		///<summary>
		///Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"></see> event.
		///</summary>
		///
		///<param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data. </param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (base.Enabled)
				this.HotDimensions = new Size(GetCellAtPoint(e.X, e.Y));
			base.OnMouseMove(e);
		}

		///<summary>
		///Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave"></see> event.
		///</summary>
		///
		///<param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data. </param>
		protected override void OnMouseLeave(EventArgs e)
		{
			if (base.Enabled)
				this.HotDimensions = Size.Empty;
			base.OnMouseLeave(e);
		}

		///<summary>
		///Raises the <see cref="E:System.Windows.Forms.Control.MouseClick"></see> event.
		///</summary>
		///
		///<param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data. </param>
		protected override void OnMouseClick(MouseEventArgs e)
		{
			if (base.Enabled)
			{
				this.Dimensions = new Size(GetCellAtPoint(e.X, e.Y));
				base.OnMouseClick(e);
				NotifyDimensionsSelected();
			}
			else
			{
				base.OnMouseClick(e);
			}
		}

		///<summary>
		///Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
		///</summary>
		///
		///<param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data. </param>
		protected override void OnPaint(PaintEventArgs e)
		{
			int maxRow = this.MaxRows;
			int maxCol = this.MaxColumns;
			int selRow = this.Rows;
			int selCol = this.Columns;
			int hotRow = this.HotRows;
			int hotCol = this.HotColumns;

			base.OnPaint(e);

			Rectangle[,] cellBounds = new Rectangle[maxRow,maxCol];
			for (int row = 0; row < maxRow; row++)
			{
				for (int col = 0; col < maxCol; col++)
				{
					cellBounds[row, col] = GetCellBounds(row + 1, col + 1);
				}
			}

			PaintCellInteriors(e.Graphics, cellBounds, this.CellStyle.FillColor, maxRow, maxCol);
			PaintCellInteriors(e.Graphics, cellBounds, this.SelectedCellStyle.FillColor, selRow, selCol);
			PaintCellInteriors(e.Graphics, cellBounds, this.HotCellStyle.FillColor, hotRow, hotCol);
			PaintCellBorders(e.Graphics, cellBounds, this.CellStyle.BorderColor, this.CellStyle.BorderWidth, maxRow, maxCol);
			PaintCellBorders(e.Graphics, cellBounds, this.SelectedCellStyle.BorderColor, this.SelectedCellStyle.BorderWidth, selRow, selCol);
			PaintCellBorders(e.Graphics, cellBounds, this.HotCellStyle.BorderColor, this.HotCellStyle.BorderWidth, hotRow, hotCol);
		}

		///<summary>
		///Raises the <see cref="E:System.Windows.Forms.Control.Resize"></see> event.
		///</summary>
		///
		///<param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data. </param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			base.Invalidate();
		}

		#endregion

		#region Helper Methods

		private static void PaintCellBorders(Graphics g, Rectangle[,] cellBounds, Color lineColor, int lineWidth, int rows, int columns)
		{
			if (!lineColor.IsEmpty && lineColor != Color.Transparent && lineWidth > 0)
			{
				Pen pen = new Pen(lineColor, 1);
				for (int r = 0; r < rows; r++)
				{
					for (int c = 0; c < columns; c++)
					{
						Rectangle bounds = cellBounds[r, c];
						for (int n = 0; n < lineWidth; n++)
						{
							g.DrawRectangle(pen, bounds.Left + n, bounds.Top + n, bounds.Width - 2*n - 1, bounds.Height - 2*n - 1);
						}
					}
				}
				pen.Dispose();
			}
		}

		private static void PaintCellInteriors(Graphics g, Rectangle[,] cellBounds, Color fillColor, int rows, int columns)
		{
			if (!fillColor.IsEmpty && fillColor != Color.Transparent)
			{
				SolidBrush brush = new SolidBrush(fillColor);
				for (int r = 0; r < rows; r++)
				{
					for (int c = 0; c < columns; c++)
					{
						g.FillRectangle(brush, cellBounds[r, c]);
					}
				}
				brush.Dispose();
			}
		}

		private Size GetDefaultSize()
		{
			// this method is only called for default cell spacing (and that is 0) so we don't compute with it here
			const int DEFAULT_CELL_LENGTH = 24;
			return new Size(DEFAULT_CELL_LENGTH*this.MaxDimensions.Width, DEFAULT_CELL_LENGTH*this.MaxDimensions.Height);
		}

		private void StyleOrSpacingChanged(object sender, PropertyChangedEventArgs e)
		{
			base.Invalidate();
		}

		/// <summary>
		/// Gets the region invalidated by the changing of a dimension from one value to another.
		/// </summary>
		/// <param name="newSize">The new dimensions.</param>
		/// <param name="oldSize">The old dimensions.</param>
		/// <returns>The invalid region to repaint.</returns>
		protected Region GetInvalidRegion(Size newSize, Size oldSize)
		{
			Region r = new Region(new Rectangle(Point.Empty, new Size(GetCellBounds(oldSize.Height + 1, oldSize.Width + 1).Location)));
			r.Union(new Rectangle(Point.Empty, new Size(GetCellBounds(newSize.Height + 1, newSize.Width + 1).Location)));
			return r;
		}

		/// <summary>
		/// Computes the coordinates of the cell to which the cursor coordinates point.
		/// </summary>
		/// <param name="x">The X position of the cursor in client coordinates.</param>
		/// <param name="y">The Y position of the cursor in client coordinates.</param>
		/// <returns>The coordinates of the cell.</returns>
		protected Point GetCellAtPoint(int x, int y)
		{
			if (base.Height <= 0 || base.Width <= 0)
				return Point.Empty;

			int rows = Math.Min(MaxDimensions.Height, 1 + (int) (1.0*MaxDimensions.Height*y/(base.Height + this.CellSpacing.Vertical)));
			int cols = Math.Min(MaxDimensions.Width, 1 + (int) (1.0*MaxDimensions.Width*x/(base.Width + this.CellSpacing.Horizontal)));
			return new Point(cols, rows);
		}

		/// <summary>
		/// Computes the rectangular boundaries of the given cell.
		/// </summary>
		/// <param name="row">The row of the cell.</param>
		/// <param name="column">The column of the cell.</param>
		/// <returns>The location and dimensions of the cell.</returns>
		protected Rectangle GetCellBounds(int row, int column)
		{
			// this algorithm allows the cell spacing to remain constant while the cell widths vary in order to handle round off error
			float avgCellWidth = ((1 - this.MaxColumns)*this.CellSpacing.Horizontal + base.Width - 1)*1.0f/this.MaxColumns;
			float avgCellHeight = ((1 - this.MaxRows)*this.CellSpacing.Vertical + base.Height - 1)*1.0f/this.MaxRows;
			int left = (int) (avgCellWidth*(column - 1)) + (column - 1)*this.CellSpacing.Horizontal;
			int top = (int) (avgCellHeight*(row - 1)) + (row - 1)*this.CellSpacing.Vertical;
			int width = (int) (avgCellWidth*(column)) + (column - 1)*this.CellSpacing.Horizontal - left + 1;
			int height = (int) (avgCellHeight*(row)) + (row - 1)*this.CellSpacing.Vertical - top + 1;
			return new Rectangle(left, top, width, height);
		}

		#endregion
	}

	#region TableDimensionsEventArgs/Handler

	/// <summary>
	/// Represents the method that will handle the <see cref="TableDimensionsPicker.DimensionsSelected"/> event.
	/// </summary>
	/// <param name="sender">The <see cref="TableDimensionsPicker"/> that fired the event.</param>
	/// <param name="e">A <see cref="TableDimensionsEventArgs"/> that contains event data.</param>
	public delegate void TableDimensionsEventHandler(object sender, TableDimensionsEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="TableDimensionsPicker.DimensionsSelected"/> event.
	/// </summary>
	public class TableDimensionsEventArgs : EventArgs
	{
		private Size _dimensions;

		/// <summary>
		/// Constructs a <see cref="TableDimensionsEventArgs"/>.
		/// </summary>
		/// <param name="dimensions">The selected dimensions.</param>
		public TableDimensionsEventArgs(Size dimensions)
		{
			_dimensions = dimensions;
		}

		/// <summary>
		/// Gets the selected dimensions.
		/// </summary>
		public Size Dimensions
		{
			get { return _dimensions; }
		}

		/// <summary>
		/// Gets the selected number of rows (height of the selected dimensions).
		/// </summary>
		public int Rows
		{
			get { return _dimensions.Height; }
		}

		/// <summary>
		/// Gets the selected number of columns (width of the selected dimensions).
		/// </summary>
		public int Columns
		{
			get { return _dimensions.Width; }
		}
	}

	#endregion

	#region TableDimensionsCellStyle

	/// <summary>
	/// Represents the style for a cells in a <see cref="TableDimensionsPicker"/> control.
	/// </summary>
	[TypeConverter(typeof (ExpandableObjectConverter))]
	public sealed class TableDimensionsCellStyle : INotifyPropertyChanged
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private Color _fillColor;
		private Color _borderColor;
		private int _borderWidth;

		/// <summary>
		/// Constructs a <see cref="TableDimensionsCellStyle"/> using default values.
		/// </summary>
		public TableDimensionsCellStyle()
		{
			ResetFillColor();
			ResetBorderColor();
			ResetBorderWidth();
		}

		/// <summary>
		/// Constructs a <see cref="TableDimensionsCellStyle"/> using the specified interior color and no borders.
		/// </summary>
		public TableDimensionsCellStyle(Color fillColor)
		{
			_fillColor = fillColor;
			ResetBorderColor();
			ResetBorderWidth();
		}

		/// <summary>
		/// Constructs a <see cref="TableDimensionsCellStyle"/> using the specified border color, border width and no interior color.
		/// </summary>
		public TableDimensionsCellStyle(Color borderColor, int borderWidth)
		{
			ResetFillColor();
			_borderColor = borderColor;
			_borderWidth = borderWidth;
		}

		/// <summary>
		/// Constructs a <see cref="TableDimensionsCellStyle"/> using the specified interior color, border color and border width.
		/// </summary>
		public TableDimensionsCellStyle(Color fillColor, Color borderColor, int borderWidth)
		{
			_fillColor = fillColor;
			_borderColor = borderColor;
			_borderWidth = borderWidth;
		}

		/// <summary>
		/// Notifies listeners that the style has changed.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		private void FirePropertyChanged(string propertyName)
		{
			if (_propertyChanged != null)
				_propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Gets or sets the color of the cell interiors.
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[Description("The color of the cell interiors.")]
		public Color FillColor
		{
			get { return _fillColor; }
			set
			{
				if (_fillColor != value)
				{
					_fillColor = value;
					FirePropertyChanged("FillColor");
				}
			}
		}

		private void ResetFillColor()
		{
			this.FillColor = Color.Empty;
		}

		private bool ShouldSerializeFillColor()
		{
			return _fillColor != Color.Empty;
		}

		/// <summary>
		/// Gets or sets the color of the cell borders.
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[Description("The color of the cell borders.")]
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				if (_borderColor != value)
				{
					_borderColor = value;
					FirePropertyChanged("BorderColor");

					if (!_borderColor.IsEmpty && this.BorderWidth <= 0)
						this.BorderWidth = 1;
				}
			}
		}

		private void ResetBorderColor()
		{
			this.BorderColor = Color.Empty;
		}

		private bool ShouldSerializeBorderColor()
		{
			return _borderColor != Color.Empty;
		}

		/// <summary>
		/// Gets or sets the pixel width of the cell borders.
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The pixel width of the cell borders.")]
		public int BorderWidth
		{
			get { return _borderWidth; }
			set
			{
				if (_borderWidth != value)
				{
					Platform.CheckNonNegative(value, "value");

					_borderWidth = value;
					FirePropertyChanged("BorderWidth");

					if (_borderWidth > 0 && this.BorderColor.IsEmpty)
						this.BorderColor = Color.Black;
				}
			}
		}

		private void ResetBorderWidth()
		{
			this.BorderWidth = 0;
		}

		/// <summary>
		/// Returns a string that fully describes the style.
		/// </summary>
		/// <returns>A string representation of the style.</returns>
		public override string ToString()
		{
			return string.Format("Fill: {0}; Border: {1}, {2}", ConvertColor(_fillColor), ConvertColor(_borderColor), _borderWidth);
		}

		private static string ConvertColor(Color color)
		{
			if (color.IsNamedColor)
				return color.Name;
			else if (color.IsEmpty)
				return "Empty";
			else if (color.A > 0)
				return string.Format("ARGB({3:x2}{0:x2}{1:x2}{2:x2})", color.R, color.G, color.B, color.A);
			return string.Format("RGB({0:x2}{1:x2}{2:x2})", color.R, color.G, color.B);
		}
	}

	#endregion

	#region TableDimensionsCellSpacing

	/// <summary>
	/// Represents the spacing between adjacent cells in a <see cref="TableDimensionsPicker"/> control.
	/// </summary>
	[TypeConverter(typeof (ExpandableObjectConverter))]
	public sealed class TableDimensionsCellSpacing : INotifyPropertyChanged
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private int _horizontal;
		private int _vertical;

		/// <summary>
		/// Constructs a new <see cref="TableDimensionsCellSpacing"/> using the default values.
		/// </summary>
		public TableDimensionsCellSpacing()
		{
			ResetHorizontal();
			ResetVertical();
		}

		/// <summary>
		/// Constructs a new <see cref="TableDimensionsCellSpacing"/> using the specified spacing.
		/// </summary>
		/// <param name="horizontal">The horizontal spacing between cells.</param>
		/// <param name="vertical">The vertical spacing between cells.</param>
		public TableDimensionsCellSpacing(int horizontal, int vertical)
		{
			_horizontal = horizontal;
			_vertical = vertical;
		}

		/// <summary>
		/// Constructs a new <see cref="TableDimensionsCellSpacing"/> using the specified spacing dimensions.
		/// </summary>
		/// <param name="spacing">The dimensions of the spacing between cells (where <see cref="Size.Width">Width</see>
		/// is the horizontal spacing, and <see cref="Size.Height">Height</see> is the vertical).</param>
		public TableDimensionsCellSpacing(Size spacing)
		{
			_horizontal = spacing.Width;
			_vertical = spacing.Height;
		}

		/// <summary>
		/// Constructs a new <see cref="TableDimensionsCellSpacing"/> using the specified spacing dimensions.
		/// </summary>
		/// <param name="spacing">The dimensions of the spacing between cells (where <see cref="Point.X">X</see>
		/// is the horizontal spacing, and <see cref="Point.Y">Y</see> is the vertical).</param>
		public TableDimensionsCellSpacing(Point spacing)
		{
			_horizontal = spacing.X;
			_vertical = spacing.Y;
		}

		/// <summary>
		/// Notifies listeners that the spacing has changed.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		private void FirePropertyChanged(string propertyName)
		{
			if (_propertyChanged != null)
				_propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Gets or sets the pixel width of the space between horizontally adjacent cells.
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The pixel width of the space between horizontally adjacent cells.")]
		public int Horizontal
		{
			get { return _horizontal; }
			set
			{
				if (_horizontal != value)
				{
					Platform.CheckNonNegative(value, "value");

					_horizontal = value;
					FirePropertyChanged("Horizontal");
				}
			}
		}

		private void ResetHorizontal()
		{
			this.Horizontal = 0;
		}

		/// <summary>
		/// Gets or sets the pixel height of the space between vertically adjacent cells.
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The pixel height of the space between vertically adjacent cells.")]
		public int Vertical
		{
			get { return _vertical; }
			set
			{
				if (_vertical != value)
				{
					Platform.CheckNonNegative(value, "value");

					_vertical = value;
					FirePropertyChanged("Vertical");
				}
			}
		}

		private void ResetVertical()
		{
			this.Vertical = 0;
		}

		/// <summary>
		/// Returns a string that fully describes the cell spacing.
		/// </summary>
		/// <returns>A string representation of the spacing.</returns>
		public override string ToString()
		{
			return string.Format("{0}, {1}", _horizontal, _vertical);
		}
	}

	#endregion
}