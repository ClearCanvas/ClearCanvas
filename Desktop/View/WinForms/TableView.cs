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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class TableView : UserControl
    {
        private event EventHandler _itemDoubleClicked;
        private event EventHandler _selectionChanged;
        private readonly DelayedEventPublisher _delayedSelectionChangedPublisher;
        private event EventHandler<ItemDragEventArgs> _itemDrag;

        private ActionModelNode _toolbarModel;
        private ActionModelNode _menuModel;

        private ITable _table;
        private bool _multiLine;

    	private bool _filterBoxVisible = false;
    	private bool _filterLabelVisible = false;
    	private bool _smartColumnSizing = false;
        private bool _isLoaded = false;

        private const int CELL_SUBROW_HEIGHT = 18;
        private readonly int _rowHeight = 0;
		private Font _subRowFont;

    	private ISelection _rememberedSelection;
		private int _rememberedScrollPosition;

    	private string _sortButtonTooltipBase;
    	private string _columnHeaderTooltipBase;

		private Bitmap _checkmarkBitmap;

		public TableView()
        {
			SuppressSelectionChangedEvent = false;
			InitializeComponent();

            // if we allow the framework to generate columns, there seems to be a bug with 
            // setting the minimum column width > 100 pixels
            // therefore, turn off the auto-generate and create the columns ourselves
            _dataGridView.AutoGenerateColumns = false;

            _rowHeight = this.DataGridView.RowTemplate.Height;
            this.DataGridView.RowPrePaint += SetCustomBackground;
            this.DataGridView.RowPostPaint += DisplayCellSubRows;
            this.DataGridView.RowPostPaint += OutlineCell;
            this.DataGridView.RowPostPaint += SetLinkColor;

			// System.Component.DesignMode does not work in control constructors
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
				// Use a DelayedEventPublisher to make fixes for bugs 386 and 8032 a little clearer.  Previously used a Timer directly
				// to delay the events
				_delayedSelectionChangedPublisher = new DelayedEventPublisher((sender, args) => NotifySelectionChanged(), 50);
			}

			try
			{
				var resolver = new ResourceResolver(typeof (TableView), false);
				using (var s = resolver.OpenResource("checkmark.png"))
				{
					_checkmarkBitmap = new Bitmap(s);
				}
			}
			catch (Exception) {}
        }

    	private void PerformDispose(bool disposing)
    	{
    		if (disposing)
    		{
    			if (_checkmarkBitmap != null)
    			{
    				_checkmarkBitmap.Dispose();
    				_checkmarkBitmap = null;
    			}
    		}
    	}

        #region Design Time properties and Events

        [DefaultValue(false)]
        public bool SortButtonVisible
        {
            get { return _sortButton.Visible; }
            set { _sortButton.Visible = value; }
        }

        [DefaultValue(false)]
        public bool FilterLabelVisible
        {
            get { return _filterLabelVisible; }
            set
            {
                // reading Visible property of ToolStripItems directly yields whether or not the item is currently showing, NOT the last set value
                _filterLabelVisible = value;
                _filterLabel.Visible = _filterLabelVisible && _filterBoxVisible;
            }
        }

        [DefaultValue(false)]
        public bool FilterTextBoxVisible
        {
            get { return _filterBoxVisible; }
            set
            {
                // reading Visible property of ToolStripItems directly yields whether or not the item is currently showing, NOT the last set value
                _filterBoxVisible = value;
                _filterTextBox.Visible = value;
                _clearFilterButton.Visible = value;
                _filterLabel.Visible = _filterLabelVisible && _filterBoxVisible;
            }
        }

        [DefaultValue(100)]
		[Localizable(true)]
        public int FilterTextBoxWidth
        {
            get { return _filterTextBox.Width; }
            set { _filterTextBox.Size = new Size(value, _filterTextBox.Height); }
        }

        [DefaultValue(true)]
        public bool ReadOnly
        {
            get { return _dataGridView.ReadOnly; }
            set { _dataGridView.ReadOnly = value; }
        }

        [DefaultValue(true)]
        public bool MultiSelect
        {
            get { return _dataGridView.MultiSelect; }
            set { _dataGridView.MultiSelect = value; }
        }

		/// <summary>
		/// Gets or sets a value indicating the automatic column sizing mode.
		/// </summary>
		/// <remarks>
		/// Setting this property disables the <see cref="SmartColumnSizing"/> algorithm.
		/// </remarks>
		[DefaultValue(DataGridViewAutoSizeColumnsMode.Fill)]
		public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
    	{
			get { return _dataGridView.AutoSizeColumnsMode; }
			set
			{
				this.SmartColumnSizing = false;
				_dataGridView.AutoSizeColumnsMode = value;
			}
    	}

		/// <summary>
		/// Gets or sets a value enabling the smart column sizing algorithm.
		/// </summary>
		/// <remarks>
		/// This algorithm overrides the <see cref="AutoSizeColumnsMode"/> property.
		/// </remarks>
		[DefaultValue(false)]
		[Description("Enables or disables the smart column sizing algorithm. Enabling this algorithm overrides the AutoSizeColumnsMode property.")]
    	public bool SmartColumnSizing
    	{
			get { return _smartColumnSizing; }
			set
			{
				_smartColumnSizing = value;
				if (_smartColumnSizing)
					_dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
				else
					this.ResetSmartColumnSizing();
			}
    	}

        [DefaultValue(false)]
        [Description("Enables or disables multi-line rows.  If enabled, text longer than the column width is wrapped and the row is auto-sized. If disabled, a single line of truncated text is followed by an ellipsis")]
        public bool MultiLine
        {
            get { return _multiLine; }
            set
            {
                _multiLine = value;
                if (_multiLine)
                {
                    this._dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    this._dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }
                else
                {
                    this._dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                    this._dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                }
            }
        }

        [Obsolete("Toolstrip item display style is controlled by ToolStripBuilder.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
        {
            get { return ToolStripItemDisplayStyle.Image; }
            set
            {
            	// this is not a settable property anymore, but this is here for backward compile-time compatability
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int FirstDisplayedScrollingRowIndex
        {
            get { return _dataGridView.FirstDisplayedScrollingRowIndex; }
            set
            {
                try
                {
                    _dataGridView.FirstDisplayedScrollingRowIndex = value;
                    _dataGridView.PerformLayout();
                }
                catch (InvalidOperationException)
                {
                    // #9466 if there isn't enough space to scroll first row into view, just ignore it - when user resizes us, the first visible row will change anyway
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get { return _toolStrip.Visible; }
            set { _toolStrip.Visible = value; }
        }

        [DefaultValue(false)]
        public bool StatusBarVisible
        {
            get { return _statusStrip.Visible; }
            set { _statusStrip.Visible = value; }
        }

        [DefaultValue(true)]
        public bool ShowColumnHeading
        {
            get { return _dataGridView.ColumnHeadersVisible; }
            set { _dataGridView.ColumnHeadersVisible = value; }
        }

		[DefaultValue("Sort By")]
		[Localizable(true)]
		public string SortButtonTooltip
		{
			get { return _sortButtonTooltipBase; }
			set { _sortButtonTooltipBase = value; }
		}

		[DefaultValue("Sort By")]
		[Localizable(true)]
		public string ColumnHeaderTooltip
		{
			get { return _columnHeaderTooltipBase; }
			set { _columnHeaderTooltipBase = value; }
		}

		[DefaultValue(null)]
		[Description("Specifies the font to be used by sub-rows.")]
		public Font SubRowFont
    	{
			get { return _subRowFont; }
			set { _subRowFont = value; }
    	}

        public event EventHandler SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }

        public event EventHandler ItemDoubleClicked
        {
            add { _itemDoubleClicked += value; }
            remove { _itemDoubleClicked -= value; }
        }

        public event EventHandler<ItemDragEventArgs> ItemDrag
        {
            add { _itemDrag += value; }
            remove { _itemDrag -= value; }
        }

        #endregion

        #region Public Properties and Events

        [Obsolete("Toolstrip item alignment is controlled by ToolStripBuilder.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RightToLeft ToolStripRightToLeft
        {
            get { return RightToLeft.No; }
			set
			{
				// this is not a settable property anymore, but this is here for backward compile-time compatability
			}
		}

    	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    	public bool SuppressSelectionChangedEvent { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SuppressForceSelectionDisplay { get; set; }


    	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    	public ActionModelNode ToolbarModel
    	{
    		get { return _toolbarModel; }
    		set
    		{
    			_toolbarModel = value;

    			// Defer initialization of ToolStrip until after Load() has been called
    			// so that parameters from application settings are initialized properly
    			if (_isLoaded) InitializeToolStrip();
    		}
    	}


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;

                // Defer initialization of ToolStrip until after Load() has been called
                // so that parameters from application settings are initialized properly
                if (_isLoaded) InitializeMenu();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Localizable(true)]
        public string StatusText
        {
            get { return _statusLabel.Text; }
            set { _statusLabel.Text = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITable Table
        {
        	get { return _table; }
        	set
        	{
        		if (this.SmartColumnSizing)
        		{
        			this.PerformSmartColumnSizing(() => SetTable(value));
        		}
        		else
        		{
        			SetTable(value);
        		}
        	}
        }

    	private void SetTable(ITable value)
    	{
    		UnsubscribeFromOldTable();

    		_table = value;

    		// by setting the datasource to null here, we eliminate the SelectionChanged events that
    		// would get fired during the call to InitColumns()
    		_dataGridView.DataSource = null;

    		InitColumns();

    		if (_table != null)
    		{
    			// Set a cell padding to provide space for the top of the focus 
    			// rectangle and for the content that spans multiple columns. 
    			var newPadding = new Padding(0, 1, 0,
    			                             CELL_SUBROW_HEIGHT*(_table.CellRowCount - 1));
    			this.DataGridView.RowTemplate.DefaultCellStyle.Padding = newPadding;

    			// Set the row height to accommodate the content that 
    			// spans multiple columns.
    			this.DataGridView.RowTemplate.Height = _rowHeight + CELL_SUBROW_HEIGHT*(_table.CellRowCount - 1);

    			// DataSource must be set after RowTemplate in order for changes to take effect
    			_dataGridView.DataSource = new TableAdapter(_table);
    			_dataGridView.ColumnHeaderMouseClick += _dataGridView_ColumnHeaderMouseClick;

				_table.Items.TransactionStarted += _table_Items_TransactionStarted;
				_table.Items.TransactionCompleted += _table_Items_TransactionCompleted;
			}

    		InitializeSortButton();
    		IntializeFilter();
    	}

        /// <summary>
        /// Gets/sets the current selection
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISelection Selection
        {
            get
            {
                return GetSelectionHelper();
            }
            set
            {
                // if someone tries to assign null, just convert it to an empty selection - this makes everything easier
                var newSelection = value ?? new Selection();

                // get the existing selection
                var existingSelection = GetSelectionHelper();

                if (!existingSelection.Equals(newSelection))
                {
                    // de-select any rows that should not be selected
                    foreach (DataGridViewRow row in _dataGridView.SelectedRows)
                    {
                        if (!CollectionUtils.Contains(newSelection.Items,
							item => Equals(item, row.DataBoundItem)))
                        {
                            row.Selected = false;
                        }
                    }

                    // select any rows that should be selected
                    foreach (var item in newSelection.Items)
                    {
                        var row = CollectionUtils.SelectFirst(_dataGridView.Rows,
							(DataGridViewRow r) => Equals(item, r.DataBoundItem));
                        if (row != null)
                            row.Selected = true;
                    }

                    ForceSelectionDisplay();

                    NotifySelectionChanged();
                }
            }
        }

        /// <summary>
        /// Whenever the table is refreshed/modified by a component it tends to jump the DataGridView display
        /// to the top of the list, this isn't desirable. The following method forces the given selection to
        /// be visible on the control.
        /// </summary>
        private void ForceSelectionDisplay()
        {
			if (SuppressForceSelectionDisplay)
				return;

            // check if ALL the selected entries are not visible to the user
            if (CollectionUtils.TrueForAll(_dataGridView.SelectedRows, (DataGridViewRow row) => !row.Displayed)
				&& _table.Items.Count != 0)
            {
                // create an array to capture the indicies of the selection collection (lol)
                // indicies needed for index position calculation of viewable index
                var selectedRows = new int[_dataGridView.SelectedRows.Count];
                var i = 0;
                foreach (DataGridViewRow row in _dataGridView.SelectedRows)
                {
                    selectedRows[i] = row.Index;
                    i++;
                }

                // create variables for the index of the last row and the number of rows displayable
                // by the control without scrolling
                // row differential then becomes the index in which the all the last displayable rows starts at
                var lastRow = _dataGridView.Rows.GetLastRow(new DataGridViewElementStates());
                var displayedRows = _dataGridView.DisplayedRowCount(false) - 1;
                var rowDifferential = lastRow - displayedRows; // calculate the differential 

                // pre-existing tables
                if (selectedRows.Length != 0)
                {
                    // if the first selection is less than the boundary last range of displayable
                    // rows, then set the first viewable row to the first selection, if not, set it
                    // to the boundary
                    if (selectedRows[0] < rowDifferential)
                        FirstDisplayedScrollingRowIndex = selectedRows[0];
                    else if (selectedRows[0] > rowDifferential)
                        FirstDisplayedScrollingRowIndex = rowDifferential;
                }
                // new tables obviously will have no entries in selectedRows therefore
                // automatically set it to the row differential which will probably be 0
                else
                {
                    FirstDisplayedScrollingRowIndex = 0;
                }
            }
            // strange oddity, this part actually never gets activated for some strange reason
            // intended to preserve the current index if there are displayable items already on screen
            else
            {
                if (FirstDisplayedScrollingRowIndex > 0)
                    FirstDisplayedScrollingRowIndex = FirstDisplayedScrollingRowIndex;
            }
        }

        /// <summary>
        /// Exposes the KeyDown event of the underlying data grid view.
        /// </summary>
        public event KeyEventHandler DataGridKeyDown
        {
            add { _dataGridView.KeyDown += value; }
            remove { _dataGridView.KeyDown -= value; }
        }

		/// <summary>
		/// Begins editing the specified column in the first selected row.
		/// </summary>
		/// <param name="column">Zero-based column index of column to edit.</param>
		/// <param name="selectAll"></param>
		public bool BeginEdit(int column, bool selectAll)
		{
			var firstSelRow = (DataGridViewRow)CollectionUtils.FirstElement(_dataGridView.SelectedRows);
			if (firstSelRow != null)
			{
				var rowIndex = firstSelRow.Index;

				_dataGridView.CurrentCell = _dataGridView[column, rowIndex];

				return _dataGridView.BeginEdit(selectAll);
			}
			return false;
		}

		/// <summary>
		/// Begins editing the specified column in the first selected row.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="selectAll"></param>
		/// <returns></returns>
		public bool BeginEdit(ITableColumn column, bool selectAll)
		{
			Platform.CheckForNullReference(column, "column");
			var colIndex = _table.Columns.IndexOf(column);
			if(colIndex < 0)
				throw new ArgumentException("Specified column does not exist in this table.");

			return BeginEdit(colIndex, selectAll);
		}

    	/// <summary>
    	/// Immediately commits any outstanding edits in progress.
    	/// </summary>
    	public bool EndEdit()
    	{
    		if (_dataGridView.IsCurrentCellInEditMode)
    		{
    			return _dataGridView.EndEdit();
    		}
    		return true;
    	}

        #endregion

		#region Smart Column Sizing

    	private delegate void BeforeSizingOperationDelegate();
		private Dictionary<string, int> _manualColumnWidths = null;
		private bool _isInternalColumnWidthChange = false;

    	private void ResetSmartColumnSizing()
    	{
    		_manualColumnWidths = null;
    		_isInternalColumnWidthChange = false;
    		_smartColumnSizing = false;
    	}

		private void PerformSmartColumnSizing(BeforeSizingOperationDelegate beforeSizingOperation)
    	{
    		_isInternalColumnWidthChange = true;
    		_dataGridView.AutoSizeColumnsMode = (_manualColumnWidths == null) ? DataGridViewAutoSizeColumnsMode.Fill : DataGridViewAutoSizeColumnsMode.None;
    		this.SuspendLayout();

    		try
    		{
    			if (beforeSizingOperation != null)
    				beforeSizingOperation.Invoke();

    			if (_manualColumnWidths != null)
    			{
    				int totalColumnWidth = 0;
    				foreach (DataGridViewColumn column in _dataGridView.Columns)
    					totalColumnWidth += column.Visible ? GetManualColumnWidth(column) : 0;

    				float clientAreaWidth = _dataGridView.ClientSize.Width;
    				VScrollBar scrollBar = (VScrollBar) CollectionUtils.SelectFirst(_dataGridView.Controls, c => c is VScrollBar);
    				if (scrollBar != null && scrollBar.Visible)
    					clientAreaWidth -= scrollBar.Width;

    				float widthMultiplier = 1;

    				if (totalColumnWidth < clientAreaWidth)
    					widthMultiplier = (clientAreaWidth)/totalColumnWidth;

    				foreach (DataGridViewColumn column in _dataGridView.Columns)
    					column.Width = (int) (GetManualColumnWidth(column)*widthMultiplier);
    			}
    		}
    		finally
    		{
    			this.ResumeLayout(true);
    			_dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
    			_isInternalColumnWidthChange = false;
    		}
    	}

    	private int GetManualColumnWidth(DataGridViewColumn column)
    	{
    		if (!_manualColumnWidths.ContainsKey(column.Name))
    			return column.Width;
    		return _manualColumnWidths[column.Name];
    	}

    	protected override void OnSizeChanged(EventArgs e)
    	{
    		base.OnSizeChanged(e);
    		if (this.SmartColumnSizing)
    			this.PerformSmartColumnSizing(null);
    	}

    	protected override void OnLoad(EventArgs e)
    	{
    		base.OnLoad(e);
    		if (this.SmartColumnSizing)
    			this.PerformSmartColumnSizing(null);
    	}

    	private void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
    	{
    		if (this.SmartColumnSizing)
    		{
    			if (!_isInternalColumnWidthChange)
    			{
    				if (_manualColumnWidths == null)
    					_manualColumnWidths = new Dictionary<string, int>();
    				foreach (DataGridViewColumn column in _dataGridView.Columns)
    					_manualColumnWidths[column.Name] = column.Width;
    			}
    		}
    	}

		#endregion

		protected ToolStrip ToolStrip
        {
            get { return _toolStrip; }
        }

        protected new ContextMenuStrip ContextMenuStrip
        {
            get { return _contextMenu; }
        }

        private void InitializeToolStrip()
        {
            ToolStripBuilder.Clear(_toolStrip.Items);
            if (_toolbarModel != null)
            {
                ToolStripBuilder.BuildToolbar(_toolStrip.Items, _toolbarModel.ChildNodes);
            }
        }

        private void InitializeMenu()
        {
            ToolStripBuilder.Clear(_contextMenu.Items);
            if (_menuModel != null)
            {
                ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
            }
        }

        private Selection GetSelectionHelper()
        {
            return new Selection(
                CollectionUtils.Map(_dataGridView.SelectedRows,
                                    (DataGridViewRow row) => row.DataBoundItem));
        }

        private void InitColumns()
        {
            // clear the old columns
            _dataGridView.Columns.Clear();

            if (_table != null)
            {
                var fontSize = this.Font.SizeInPoints;
                foreach (ITableColumn col in _table.Columns)
                {
                    // this is ugly but somebody's gotta do it
                    DataGridViewColumn dgcol;
					if (col.ColumnType == typeof(bool))
					{
						// if read-only, we use an image column so we can display our own checkbox
						dgcol = col.ReadOnly ? (DataGridViewColumn)
							new DataGridViewImageColumn {SortMode = DataGridViewColumnSortMode.Automatic, DefaultCellStyle = {NullValue = null}} :
							new DataGridViewCheckBoxColumn();
					}
					else if (col.ColumnType == typeof(Image) || col.ColumnType == typeof(IconSet))
					{
						// Set the default to display nothing if not icons are provided.
						// Otherwise WinForms will by default display an ugly icon with 'x'
						dgcol = new DataGridViewImageColumn {SortMode = DataGridViewColumnSortMode.Automatic, DefaultCellStyle = {NullValue = null}};
					}
					else if (col.HasClickableLink)
					{
						dgcol = new DataGridViewLinkColumn();
						var linkColumn = (DataGridViewLinkColumn)dgcol;
						linkColumn.LinkBehavior = LinkBehavior.SystemDefault;
						linkColumn.TrackVisitedState = false;
						linkColumn.SortMode = DataGridViewColumnSortMode.Automatic;
					}
					else
					{
						// assume any other type of column will be displayed as text
						// if it provides a custom editor, then we need to use a sub-class of the text box column
						dgcol = (col.GetCellEditor() != null) ?
							(DataGridViewColumn)new CustomEditableTableViewColumn(_table, col) : new DataGridViewTextBoxColumn();
					}

                    // initialize the necessary properties
                    dgcol.Name = col.Name;
                    dgcol.HeaderText = col.DisplayName;
                    dgcol.DataPropertyName = col.Name;
                    dgcol.ReadOnly = col.ReadOnly;
                    dgcol.MinimumWidth = (int)(col.WidthFactor * _table.BaseColumnWidthChars * fontSize);
                    dgcol.FillWeight = col.WidthFactor;
                    dgcol.Visible = col.Visible;

                    // Associate the ITableColumn with the DataGridViewColumn
                    dgcol.Tag = col;

                    col.VisibleChanged += OnColumnVisibilityChanged;

                    _dataGridView.Columns.Add(dgcol);
                }
            	_table.Columns.ItemsChanged += OnColumnsChanged;
            }
        }

        private void UnsubscribeFromOldTable()
        {
            if (_table != null)
            {
                foreach (ITableColumn column in _table.Columns)
                    column.VisibleChanged -= OnColumnVisibilityChanged;

				_dataGridView.ColumnHeaderMouseClick -= _dataGridView_ColumnHeaderMouseClick;
				_table.Columns.ItemsChanged -= OnColumnsChanged;
				_table.Items.TransactionStarted -= _table_Items_TransactionStarted;
				_table.Items.TransactionCompleted -= _table_Items_TransactionCompleted;
			}
        }

		protected virtual void OnDataError(DataGridViewDataErrorEventArgs e)
		{
			if (e.Exception != null)
			{
				System.Windows.Forms.MessageBox.Show(this, e.Exception.Message, Application.Name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				e.ThrowException = false;
			}
		}

    	private void OnColumnsChanged(object sender, ItemChangedEventArgs e)
    	{
    		this.Table = this.Table;
    	}

        private void OnColumnVisibilityChanged(object sender, EventArgs e)
        {
            // NY: Yes, I know, this is really cheap. The original plan was
            // to use anonymous delegates to "bind" the ITableColumn to the
            // DataGridViewColumn, but unsubscribing from ITableColumn.VisiblityChanged
            // was problematic.  This is the next best thing if we want
            // easy unsubscription.
            var column = (ITableColumn)sender;  //Invalid cast is a programming error, so let exception be thrown
            var dgcolumn = FindDataGridViewColumn(column);

            if (dgcolumn != null)
                dgcolumn.Visible = column.Visible;
        }

        private DataGridViewColumn FindDataGridViewColumn(ITableColumn column)
        {
            foreach (DataGridViewColumn dgcolumn in _dataGridView.Columns)
            {
                if (dgcolumn.Tag == column)
                    return dgcolumn;
            }

            return null;
        }

        // Paints the custom background for each row
        private void SetCustomBackground(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if ((e.State & DataGridViewElementStates.Selected) ==
                        DataGridViewElementStates.Selected)
            {
                // do nothing?
                return;
            }

            if (_table != null)
            {
                var rowBounds = GetAdjustedRowBounds(e.RowBounds);

                // Color.FromName("Empty") does not return Color.Empty, so need to manually check for Empty
                var colorName = _table.GetItemBackgroundColor(_table.Items[e.RowIndex]);
                var backgroundColor = string.IsNullOrEmpty(colorName) || colorName.Equals("Empty") ? Color.Empty : Color.FromName(colorName);

                if (backgroundColor.Equals(Color.Empty))
                {
                    backgroundColor = e.InheritedRowStyle.BackColor;
                }

                // Paint the custom selection background.
                using (Brush backbrush =
                    new SolidBrush(backgroundColor))
                {
                    e.PaintParts &= ~DataGridViewPaintParts.Background;
                    e.Graphics.FillRectangle(backbrush, rowBounds);
                }
            }
        }

        // Paints the custom outline for each row
        private void OutlineCell(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var rowBounds = GetAdjustedRowBounds(e.RowBounds);

            if (_table != null)
            {
                const int penWidth = 2;
                var outline = new Rectangle(
                    rowBounds.Left + penWidth / 2,
                    rowBounds.Top + penWidth / 2 + 1,
                    rowBounds.Width - penWidth,
                    rowBounds.Height - penWidth - 2);

                var colorName = _table.GetItemOutlineColor(_table.Items[e.RowIndex]);
                var outlineColor = string.IsNullOrEmpty(colorName) || colorName.Equals("Empty") ? Color.Empty : Color.FromName(colorName);
                using (var outlinePen = new Pen(outlineColor, penWidth))
                {
                    e.Graphics.DrawRectangle(outlinePen, outline);
                }
            }
        }

        // Paints the content that spans multiple columns and the focus rectangle.
        private void DisplayCellSubRows(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var rowBounds = GetAdjustedRowBounds(e.RowBounds);

            SolidBrush forebrush = null;
            try
            {
                // Determine the foreground color.
                if ((e.State & DataGridViewElementStates.Selected) ==
                    DataGridViewElementStates.Selected)
                {
                    forebrush = new SolidBrush(e.InheritedRowStyle.SelectionForeColor);
                }
                else
                {
                    forebrush = new SolidBrush(e.InheritedRowStyle.ForeColor);
                }

                // Store text for each subrow
                var sb = new StringBuilder[_table.CellRowCount];
                for (var i = 0; i < _table.CellRowCount; i++)
                {
                    sb[i] = new StringBuilder();
                }

                for (var i = 0; i < _table.Columns.Count; i++)
                {
                    var col = _table.Columns[i] as ITableColumn;
                    if (col != null && col.CellRow > 0)
                    {
                        var row = this.DataGridView.Rows[e.RowIndex];
                        var recipe = row.Index != -1 ? row.Cells[i].Value : null;

                        if (recipe != null)
                        {
                            sb[col.CellRow].Append(recipe + " ");
                        }

                    }
                }

                // Draw text for each sub row (Rows 1 and higher in the Table)
                for (var i = 1; i < _table.CellRowCount; i++)
                {
                    var text = sb[i].ToString().Trim();

                    if (string.IsNullOrEmpty(text) == false)
                    {
                        // Calculate the bounds for the content that spans multiple 
                        // columns, adjusting for the horizontal scrolling position 
                        // and the current row height, and displaying only whole
                        // lines of text.
                        var textArea = rowBounds;
                        textArea.X -= this.DataGridView.HorizontalScrollingOffset;
                        textArea.Width += this.DataGridView.HorizontalScrollingOffset;
                        textArea.Y += _rowHeight + (i - 1) * CELL_SUBROW_HEIGHT;
                        textArea.Height = CELL_SUBROW_HEIGHT;

                        // Calculate the portion of the text area that needs painting.
                        RectangleF clip = textArea;
                        var startX = this.DataGridView.RowHeadersVisible ? this.DataGridView.RowHeadersWidth : 0;
                        clip.Width -= startX + 1 - clip.X;
                        clip.X = startX + 1;
                        var oldClip = e.Graphics.ClipBounds;
                        e.Graphics.SetClip(clip);

                        var format = new StringFormat
                                     	{
                                     		FormatFlags = StringFormatFlags.NoWrap,
                                     		Trimming = StringTrimming.EllipsisWord
                                     	};

                    	// Draw the content that spans multiple columns.
                        e.Graphics.DrawString(text, _subRowFont ?? e.InheritedRowStyle.Font, forebrush, textArea, format);

                        e.Graphics.SetClip(oldClip);
                    }
                }
            }
            finally
            {
                if (forebrush != null)
                    forebrush.Dispose();
            }
        }

        // Make all the link column the same color as the fore color
        private void SetLinkColor(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var row = _dataGridView.Rows[e.RowIndex];
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell is DataGridViewLinkCell)
                {
                    var linkCell = (DataGridViewLinkCell)cell;
                    linkCell.ActiveLinkColor = linkCell.LinkColor = linkCell.VisitedLinkColor
                        = row.Selected ? cell.InheritedStyle.SelectionForeColor : cell.InheritedStyle.ForeColor;
                }
            }
        }

        private Rectangle GetAdjustedRowBounds(Rectangle rowBounds)
        {
            return new Rectangle(
                    (this.DataGridView.RowHeadersVisible ? this.DataGridView.RowHeadersWidth : 0) + rowBounds.Left,
                    rowBounds.Top,
                    this.DataGridView.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - this.DataGridView.HorizontalScrollingOffset,
                    rowBounds.Height);
        }

        private void _dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)    // rowindex == -1 represents a header click
            {
                // bug 8032: need to flush selection change prior to double click too (cf bug 386)
                FlushSelectionChangedNotification();
                EventsHelper.Fire(_itemDoubleClicked, this, new EventArgs());
            }
        }

        private void _contextMenu_Opening(object sender, CancelEventArgs e)
        {
            // if a context menu is being opened, need to flush any pending selection change notification immediately before showing menu (bug 386)
            FlushSelectionChangedNotification();
        }

        private void _dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            // fix Bug 386: rather than firing our own _selectionChanged event immediately, post delayed notification
            PostSelectionChangedNotification();
        }

        private void FlushSelectionChangedNotification()
        {
            _delayedSelectionChangedPublisher.PublishNow();
        }

        private void PostSelectionChangedNotification()
        {
            _delayedSelectionChangedPublisher.Publish(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handling this event is necessary to ensure that changes to checkbox cells are propagated
        /// immediately to the underlying <see cref="ITable"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // if the state of a checkbox cell has changed, commit the edit immediately
            if (_dataGridView.CurrentCell is DataGridViewCheckBoxCell)
            {
                _dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Handle the ItemDrag event of the internal control, so that this control can fire its own 
        /// event, using the current selection as the "item" that is being dragged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dataGridView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // if a drag is being initiated, need to flush any pending selection change notification immediately before
            // proceeding (bug 386)
            FlushSelectionChangedNotification();

            var args = new ItemDragEventArgs(e.Button, this.GetSelectionHelper());
            EventsHelper.Fire(_itemDrag, this, args);
        }

        private void NotifySelectionChanged()
        {
            if (SuppressSelectionChangedEvent)
                return;

            // notify clients of this class of a *real* selection change
            EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }

        protected DataGridView DataGridView
        {
            get { return _dataGridView; }
        }

        private void TableView_Load(object sender, EventArgs e)
        {
            InitializeMenu();
            InitializeToolStrip();

            _isLoaded = true;
        }

        private void InitializeSortButton()
        {
            if (_table == null || _table.Columns.Count == 0)
            {
                _sortButton.Enabled = false;
            }
            else
            {
                // Rebuild dropdown menu
                _sortButton.Enabled = true;
                _sortButton.DropDownItems.Clear();
                _sortButton.DropDownItems.Add(_sortAscendingButton);
                _sortButton.DropDownItems.Add(_sortDescendingButton);
                _sortButton.DropDownItems.Add(_sortSeparator);

				foreach (ITableColumn column in _table.Columns)
            	{
					ToolStripItem item = new ToolStripMenuItem(column.DisplayName, null, _sortButtonDropDownItem_Click, column.Name);
					if (_sortButton.DropDownItems.ContainsKey(column.Name) == false)
						_sortButton.DropDownItems.Add(item);
				}

                ResetSortButtonState();
            }
        }

        private void ResetSortButtonState()
        {
            if (_table == null || _table.SortParams == null)
                return;

			foreach (ToolStripItem item in _sortButton.DropDownItems)
        	{
				if (item == _sortAscendingButton)
					this.SortAscendingButtonCheck = _table.SortParams.Ascending;
				else if (item == _sortDescendingButton)
					this.SortDescendingButtonCheck = _table.SortParams.Ascending == false;
				else if (item == _sortSeparator)
					continue;
				else
				{
					if (item.Name.Equals(_table.SortParams.Column.Name))
					{
						item.Image = SR.CheckSmall;
						_sortButton.ToolTipText = string.IsNullOrEmpty(_sortButtonTooltipBase)
							? string.Format(SR.MessageSortBy, item.Name)
							: string.Format("{0}: {1}", _sortButtonTooltipBase, item.Name);
					}
					else
					{
						item.Image = null;
					}
				}
			}
        }

        private bool SortAscendingButtonCheck
        {
            get { return _sortAscendingButton.Image != null; }
            set { _sortAscendingButton.Image = value ? SR.CheckSmall : null; }
        }

        private bool SortDescendingButtonCheck
        {
            get { return _sortDescendingButton.Image != null; }
            set { _sortDescendingButton.Image = value ? SR.CheckSmall : null; }
        }

		private void _table_Items_TransactionStarted(object sender, EventArgs e)
		{
			// remember the selection and scroll position
			_rememberedSelection = this.Selection;
			_rememberedScrollPosition = this.FirstDisplayedScrollingRowIndex;
		}

		private void _table_Items_TransactionCompleted(object sender, EventArgs e)
		{
			// attempt to reset the selection and scroll position
			if (_rememberedSelection.Items.Length > 0)
				this.Selection = _rememberedSelection;
			if (_rememberedScrollPosition > 0 && _rememberedScrollPosition < _table.Items.Count)
				this.FirstDisplayedScrollingRowIndex = _rememberedScrollPosition;

			ResetSortButtonState();
		}

		private void _dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			ForceSelectionDisplay();
		}
		
		private void sortAscendingButton_Click(object sender, EventArgs e)
        {
            if (_table == null || _table.SortParams == null)
                return;

            _table.SortParams.Ascending = true;
            _table.Sort(_table.SortParams);
        }

        private void sortDescendingButton_Click(object sender, EventArgs e)
        {
            if (_table == null || _table.SortParams == null)
                return;

            _table.SortParams.Ascending = false;
            _table.Sort(_table.SortParams);
        }

        private void _sortButtonDropDownItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var sortColumn = CollectionUtils.SelectFirst(_table.Columns, (ITableColumn column) => column.Name.Equals(item.Name));

            if (sortColumn != null)
            {
                var direction = _table.SortParams == null ? true : _table.SortParams.Ascending;
                var sortParams = new TableSortParams(sortColumn, direction);
                _table.Sort(sortParams);
            }
        }

        private void IntializeFilter()
        {
            _filterTextBox.Enabled = (_table != null);

            if (_table != null && _table.FilterParams != null && _table.FilterParams.Value is string)
            {
                _filterTextBox.Text = (string)_table.FilterParams.Value;
            }
            else
                _filterTextBox.Text = "";
        }

        private void _clearFilterButton_Click(object sender, EventArgs e)
        {
            _filterTextBox.Text = "";
        }

        private void _filterText_TextChanged(object sender, EventArgs e)
        {
            if (_table == null)
                return;


            if (String.IsNullOrEmpty(_filterTextBox.Text))
            {
                _filterTextBox.ToolTipText = SR.MessageEmptyFilter;
                _clearFilterButton.Enabled = false;
                _table.RemoveFilter();
            }
            else
            {
                _filterTextBox.ToolTipText = String.Format(SR.MessageFilterBy, _filterTextBox.Text);
                _clearFilterButton.Enabled = true;
                var filterParams = new TableFilterParams(null, _filterTextBox.Text);
                _table.Filter(filterParams);
            }

            // Refresh the current table
            this.Table = _table;
        }

        private void _dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore header cells
            if (e.RowIndex == -1)
                return;

            var dgCol = _dataGridView.Columns[e.ColumnIndex];
            if (dgCol is DataGridViewLinkColumn)
            {
                var col = (ITableColumn)dgCol.Tag;
                col.ClickLink(_table.Items[e.RowIndex]);
            }
            else if (dgCol is DataGridViewButtonColumn)
            {
                throw new NotImplementedException();
            }
        }

		private void _dataGridView_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
		{
			if (e.RowIndex == -1 && string.IsNullOrEmpty(_columnHeaderTooltipBase))
				return;

			var column = (ITableColumn)_dataGridView.Columns[e.ColumnIndex].Tag;
			e.ToolTipText = e.RowIndex == -1 ? 
				string.Format("{0}: {1}", _columnHeaderTooltipBase, column.DisplayName)
				: column.GetTooltipText(_table.Items[e.RowIndex]);
		}

    	private void _dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			var column = (ITableColumn)_dataGridView.Columns[e.ColumnIndex].Tag;

			// Unless we know the type of e.Value can be handled by the DataGridView, we do not want to set e.FormattingApplied to true. Doing so will 
			// prevent the cell from formatting e.Value into type it can handle (eg. string), result in FormatException for value type like int, float, etc.			
			if (column.ColumnType == typeof(IconSet))
			{
				try
				{
					// try to create the icon
					var iconSet = (IconSet)e.Value;
					if (iconSet != null)
						e.Value = iconSet.CreateIcon(IconSize.Small, column.ResourceResolver);
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex);
				}
				return;
			}

			// special case: for a readonly boolean cell, we convert the value to a representative icon
			if (column.ColumnType == typeof(bool) && column.ReadOnly)
			{
				var b = (bool) e.Value;
				e.Value = b ? _checkmarkBitmap : null;
				return;
			}

    		e.Value = column.FormatValue(e.Value);
		}

		private void _dataGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
		{
			// if we do custom editing, the dgv sometimes tries to pass a string back and expects
			// the cell to "parse" it to obtain the actual value
			// therefore, we subscribe to this event so that we can retrieve the value from the 
			// underlying ITable
			var column = (ITableColumn)_dataGridView.Columns[e.ColumnIndex].Tag;

			// if no custom editor, then nothing needs to be done here
			if (column.GetCellEditor() == null)
				return;

			// retrieve value from table, and inform dgv that we have parsed the string successfully
			e.Value = column.GetValue(_table.Items[e.RowIndex]);
			e.ParsingApplied = true;
		}

		private void _dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			var column = (ITableColumn)_dataGridView.Columns[e.ColumnIndex].Tag;
			var item = _table.Items[e.RowIndex];

			// prevent editing on read-only columns, or if this particular item is not editable
			e.Cancel = column.ReadOnly || !column.IsEditable(item);
		}

		private void _dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			OnDataError(e);
		}
	}
}
