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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	public class StudyFilterTableView : TableView
	{
		public delegate ActionModelNode ActionModelGetterDelegate(int row, int column);

		private readonly IContainer _components;
		private readonly ContextMenuStrip _contextMenuStrip;
		private ActionModelGetterDelegate _contextActionModelDelegate;

		private readonly ContextMenuStrip _columnFilterMenuStrip;
		private event EventHandler _columnFilterMenuStripClosed;
		private ActionModelGetterDelegate _columnFilterActionModelDelegate;

		public StudyFilterTableView() : base()
		{
			base.SmartColumnSizing = true;
			base.DataGridView.ColumnAdded += DataGridView_ColumnAdded;
			base.DataGridView.ColumnRemoved += DataGridView_ColumnRemoved;
			base.DataGridView.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
			base.DataGridView.CellMouseClick += DataGridView_CellMouseClick;

			base.DataGridView.AllowUserToResizeRows = false;

			_components = new Container();
			_contextMenuStrip = new ContextMenuStrip(_components);
			_contextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);

			_columnFilterMenuStrip = new ContextMenuStrip(_components);
			_columnFilterMenuStrip.Closing += ColumnFilterMenuStrip_Closing;
			_columnFilterMenuStrip.Closed += ColumnFilterMenuStrip_Closed;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _components != null)
			{
				_components.Dispose();
			}

			base.DataGridView.CellMouseClick -= DataGridView_CellMouseClick;
			base.DataGridView.ColumnHeaderMouseClick -= DataGridView_ColumnHeaderMouseClick;
			base.DataGridView.ColumnRemoved -= DataGridView_ColumnRemoved;
			base.DataGridView.ColumnAdded -= DataGridView_ColumnAdded;
			base.Dispose(disposing);
		}

		private static void ShowContextMenu(ContextMenuStrip contextMenuStrip, ActionModelNode actionModel, Point screenPoint, int minWidth, bool alignRight)
		{
			ToolStripBuilder.Clear(contextMenuStrip.Items);
			if (actionModel != null)
			{
				ToolStripBuilder.BuildMenu(contextMenuStrip.Items, actionModel.ChildNodes);
				if (alignRight)
					screenPoint.Offset(-contextMenuStrip.Width, 0);
				contextMenuStrip.Show(screenPoint);
			}
		}

		#region Context Menu Handling

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelGetterDelegate ContextActionModelDelegate
		{
			get { return _contextActionModelDelegate; }
			set { _contextActionModelDelegate = value; }
		}

		private void DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (_contextActionModelDelegate != null)
				{
					ActionModelNode actionModel = _contextActionModelDelegate(e.RowIndex, e.ColumnIndex);
					Rectangle r = base.DataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
					ShowContextMenu(_contextMenuStrip, actionModel,
					                base.DataGridView.PointToScreen(new Point(e.Location.X + r.Left, e.Location.Y + r.Top)),
					                0, false);
				}
			}
		}

		private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (_contextActionModelDelegate != null)
				{
					ActionModelNode actionModel = _contextActionModelDelegate(-1, e.ColumnIndex);
					Rectangle r = base.DataGridView.GetColumnDisplayRectangle(e.ColumnIndex, true);
					ShowContextMenu(_contextMenuStrip, actionModel,
					                base.DataGridView.PointToScreen(new Point(e.Location.X + r.Left, e.Location.Y + r.Top)),
					                0, false);
				}
			}
		}

		#endregion

		#region Column Filter Handling

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelGetterDelegate ColumnFilterActionModelDelegate
		{
			get { return _columnFilterActionModelDelegate; }
			set { _columnFilterActionModelDelegate = value; }
		}

		public void SetColumnFilteringActive(string columnKey, bool isActive)
		{
			StudyFilterColumnHeaderCell header = base.DataGridView.Columns[columnKey].HeaderCell as StudyFilterColumnHeaderCell;
			if(header != null)
			{
				header.Filtered = isActive;
			}
		}

		public void SetColumnFilteringActive(int columnIndex, bool isActive)
		{
			StudyFilterColumnHeaderCell header = base.DataGridView.Columns[columnIndex].HeaderCell as StudyFilterColumnHeaderCell;
			if (header != null)
			{
				header.Filtered = isActive;
			}
		}

		public event EventHandler ColumnFilterMenuStripClosed
		{
			add { _columnFilterMenuStripClosed += value; }
			remove { _columnFilterMenuStripClosed -= value; }
		}

		private void DataGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
		{
			if (!(e.Column.HeaderCell is StudyFilterColumnHeaderCell))
			{
				StudyFilterColumnHeaderCell header = new StudyFilterColumnHeaderCell(e.Column.HeaderCell);
				header.DropDownShown += HeaderCell_DropDownShown;
				header.DropDownHidden += HeaderCell_DropDownHidden;
				e.Column.SortMode = DataGridViewColumnSortMode.Programmatic;
				e.Column.HeaderCell = header;
			}
		}

		private void DataGridView_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
		{
			if (e.Column.HeaderCell is StudyFilterColumnHeaderCell)
			{
				StudyFilterColumnHeaderCell header = (StudyFilterColumnHeaderCell) e.Column.HeaderCell;
				header.DropDownHidden -= HeaderCell_DropDownHidden;
				header.DropDownShown -= HeaderCell_DropDownShown;
			}
		}

		private void HeaderCell_DropDownShown(object sender, EventArgs e)
		{
			if (_columnFilterActionModelDelegate != null)
			{
				StudyFilterColumnHeaderCell header = sender as StudyFilterColumnHeaderCell;
				if (header != null)
				{
					Rectangle headerBounds = this.DataGridView.GetCellDisplayRectangle(header.ColumnIndex, -1, false);
					_columnFilterMenuStrip.Tag = header;
					ShowContextMenu(_columnFilterMenuStrip, _columnFilterActionModelDelegate(-1, header.ColumnIndex),
					                base.DataGridView.PointToScreen(new Point(headerBounds.Right, headerBounds.Bottom)),
					                headerBounds.Width, true);
				}
			}
		}

		private void HeaderCell_DropDownHidden(object sender, EventArgs e)
		{
			_columnFilterMenuStrip.Close();
		}

		private void ColumnFilterMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			switch (e.CloseReason)
			{
				case ToolStripDropDownCloseReason.CloseCalled:
					break;
				case ToolStripDropDownCloseReason.AppClicked:
				case ToolStripDropDownCloseReason.AppFocusChange:
				case ToolStripDropDownCloseReason.Keyboard:
				case ToolStripDropDownCloseReason.ItemClicked:
				default:
					StudyFilterColumnHeaderCell header = _columnFilterMenuStrip.Tag as StudyFilterColumnHeaderCell;
					if (header != null)
					{
						Platform.Log(LogLevel.Debug, "{0} ColumnFilterMenuStrip_Closing because of {1}", DateTime.Now.TimeOfDay, e.CloseReason);
						header.ResetDropDown();
					}
					break;
			}
		}

		private void ColumnFilterMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			EventsHelper.Fire(_columnFilterMenuStripClosed, this, EventArgs.Empty);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if (base.ParentForm != null)
				base.ParentForm.Activated += ParentForm_Activated;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (base.ParentForm != null)
				base.ParentForm.Activated -= ParentForm_Activated;
			base.OnHandleDestroyed(e);
		}

		private void ParentForm_Activated(object sender, EventArgs e)
		{
			// if the parent form 
			if (!_columnFilterMenuStrip.Visible)
			{
				StudyFilterColumnHeaderCell header = _columnFilterMenuStrip.Tag as StudyFilterColumnHeaderCell;
				if (header != null)
				{
					header.ResetDropDown();
				}
			}
		}

		#endregion
	}
}