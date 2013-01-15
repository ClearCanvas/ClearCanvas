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
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
	/// A custom <see cref="ToolStripItem"/> control that hosts a <see cref="TableDimensionsPicker"/>.
	/// </summary>
	internal class LayoutChangerToolStripItem : ToolStripControlHost
	{
		private readonly Color CLEARCANVAS_BLUE = Color.FromArgb(0, 164, 228);
		private readonly TableDimensionsPicker _picker;
		private readonly LayoutChangerAction _action;
		private readonly CustomLabel _label;
		private readonly Panel _panel;
		private readonly Panel _spacerL;
		private readonly Panel _spacerR;
		private readonly Size _defaultSize;
		private ToolStrip _owner;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action">The action to which this view is bound.</param>
		public LayoutChangerToolStripItem(LayoutChangerAction action) : base(new Panel())
		{
			const int cellWidth = 25;
			int idealPickerWidth = (action.MaxColumns - 1)*cellWidth*6/5 + cellWidth;
			int idealPickerHeight = (action.MaxRows - 1)*cellWidth*6/5 + cellWidth;

			_action = action;

			const int borderWidth = 1;
			_picker = new TableDimensionsPicker(_action.MaxRows, _action.MaxColumns);
			_picker.Dock = DockStyle.Left;
			_picker.BackColor = Color.Transparent;
			_picker.CellSpacing = new TableDimensionsCellSpacing(cellWidth/5, cellWidth/5);
			_picker.CellStyle = new TableDimensionsCellStyle(Color.FromArgb(0, 71, 98), borderWidth);
			_picker.HotCellStyle = new TableDimensionsCellStyle(CLEARCANVAS_BLUE, CLEARCANVAS_BLUE, borderWidth);
			_picker.SelectedCellStyle = new TableDimensionsCellStyle();
			_picker.Size = new Size(idealPickerWidth, idealPickerHeight);
			_picker.DimensionsSelected += OnDimensionsSelected;
			_picker.HotDimensionsChanged += OnHotTrackingDimensionsChanged;
			_label = new CustomLabel();
			_label.AutoSize = false;
			_label.BackColor = Color.Transparent;
			_label.Click += OnCancel;
			_label.Dock = DockStyle.Top;
			_label.Size = new Size(idealPickerWidth, 21);
			_label.Text = _action.Label;
			_spacerL = new Panel();
			_spacerL.BackColor = Color.Transparent;
			_spacerL.Dock = DockStyle.Left;
			_spacerL.Size = new Size(0, idealPickerHeight);
			_spacerR = new Panel();
			_spacerR.BackColor = Color.Transparent;
			_spacerR.Dock = DockStyle.Fill;
			_spacerR.Size = new Size(0, idealPickerHeight);
			_panel = (Panel) base.Control;
			_panel.Size = _defaultSize = new Size(Math.Max(base.Width, idealPickerWidth), idealPickerHeight + _label.Height);
			_panel.Controls.Add(_spacerR);
			_panel.Controls.Add(_picker);
			_panel.Controls.Add(_spacerL);
			_panel.Controls.Add(_label);
			_panel.Resize += OnContentPanelResize;

			base.AutoSize = false;
			base.BackColor = Color.Transparent;
			base.ControlAlign = ContentAlignment.TopCenter;
			this.MyOwner = base.Owner;
			base.Size = _defaultSize = new Size(Math.Max(base.Width, idealPickerWidth), idealPickerHeight + _label.Height);
		}

		protected override Size DefaultSize
		{
			get { return _defaultSize; }
		}

		public override Size GetPreferredSize(Size constrainingSize)
		{
			return _defaultSize;
		}

		protected override bool DismissWhenClicked
		{
			get { return true; }
		}

		private void OnContentPanelResize(object sender, EventArgs e)
		{
			_spacerL.Size = new Size((_panel.Width - _picker.Width)/2, _spacerL.Height);
		}

		/// <summary>
		/// Fired when the hot-tracked cell changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnHotTrackingDimensionsChanged(object sender, EventArgs e)
		{
			if (_picker.HotDimensions.IsEmpty)
				_label.Text = _action.Label;
			else
				_label.Text = string.Format(SR.FormatKeyValue, _action.Label, string.Format(SR.FormatRowsColumns, _picker.HotDimensions.Height, _picker.HotDimensions.Width));
		}

		/// <summary>
		/// Fired when the user selects a layout.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDimensionsSelected(object sender, TableDimensionsEventArgs e)
		{
			_action.SetLayout(e.Rows, e.Columns);
			CloseDropDown();
		}

		/// <summary>
		/// Fired when the user clicks on the cancel label bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancel(object sender, EventArgs e)
		{
			CloseDropDown();
		}

		/// <summary>
		/// Closes the dropdown, if this ToolStripItem is on a dropdown.
		/// </summary>
		private void CloseDropDown()
		{
			if (base.IsOnDropDown)
			{
				base.PerformClick();
			}
		}

		/// <remarks>
		/// Yes, this is an incredibly convoluted way to determine max width of toolstripitems in the same menu at runtime
		/// However, it is the only one that seems to work.
		/// </remarks>
		private ToolStrip MyOwner
		{
			get { return _owner; }
			set
			{
				if (_owner != value)
				{
					if (_owner != null)
						_owner.Resize -= OnOwnerResize;

					_owner = value;

					if (_owner != null)
						_owner.Resize += OnOwnerResize;
				}
			}
		}

		protected override void OnOwnerChanged(EventArgs e)
		{
			this.MyOwner = base.Owner;
			base.OnOwnerChanged(e);
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				MyOwner = null;	

			base.Dispose(disposing);
		}

		private void OnOwnerResize(object sender, EventArgs e)
		{
			int maxWidth = _panel.Width;
			foreach (ToolStripItem item in this.MyOwner.Items)
			{
				maxWidth = Math.Max(item.Width, maxWidth);
			}
			_panel.Size = new Size(maxWidth, _panel.Height);
		}

		/// <summary>
		/// Draw our own label because for some reason the regular Label control identifies a smaller region to double buffer the draw
		/// and as a result, part of the label will flicker (double buffer region width seems to be linked with the actual size of the picker control)
		/// </summary>
		private class CustomLabel : Control
		{
			public CustomLabel()
			{
				base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			}

			protected override void OnTextChanged(EventArgs e)
			{
				base.OnTextChanged(e);
				base.Invalidate();
			}

			protected override bool DoubleBuffered
			{
				get { return true; }
				set { }
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				base.OnPaint(e);
				using (StringFormat sf = new StringFormat())
				{
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;
					e.Graphics.DrawString(base.Text, base.Font, Brushes.Black, base.ClientRectangle, sf);
				}
			}
		}
	}
}
