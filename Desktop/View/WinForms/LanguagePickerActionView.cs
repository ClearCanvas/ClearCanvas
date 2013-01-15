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
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// View implementation for the <see cref="LanguagePickerAction"/>.
	/// </summary>
	[ExtensionOf(typeof (LanguagePickerActionViewExtensionPoint))]
	internal sealed class LanguagePickerActionView : WinFormsView, IActionView
	{
		private LocalePickerToolStripItem _control;
		private IActionViewContext _context;

		public IActionViewContext Context
		{
			get { return _context; }
			set
			{
				if (_context != value)
				{
					if (_context != null) _context.IconSizeChanged -= OnIconSizeChanged;

					_context = value;

					if (_context != null) _context.IconSizeChanged += OnIconSizeChanged;
				}
			}
		}

		public override object GuiElement
		{
			get { return _control ?? (_control = new LocalePickerToolStripItem((LanguagePickerAction) Context.Action) {Alignment = ToolStripItemAlignment.Right}); }
		}

		private void OnIconSizeChanged(object sender, EventArgs e)
		{
			_control.IconSize = _context.IconSize;
		}

		private sealed class LocalePickerToolStripItem : ToolStripControlHost
		{
			private IconSize _iconSize;

			public LocalePickerToolStripItem(LanguagePickerAction action)
				: base(new LocalePicker())
			{
				Action = action;
				Action.AvailableChanged += OnActionAvailableChangedHandler;
				Action.EnabledChanged += OnActionEnabledChangedHandler;
				Action.IconSetChanged += OnActionIconSetChangedHandler;
				Action.LabelChanged += OnActionLabelChangedHandler;
				Action.TooltipChanged += OnActionTooltipChangedHandler;
				Action.VisibleChanged += OnActionVisibleChangedHandler;

				Control.SelectedLocale = Action.SelectedLocale;
				Control.SelectedLocaleChanged += OnControlSelectedLocaleChanged;

				UpdateVisibility();
				UpdateEnablement();
				UpdateLabel();
				UpdateTooltip();
				UpdateIcon();
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					Action.AvailableChanged -= OnActionAvailableChangedHandler;
					Action.EnabledChanged -= OnActionEnabledChangedHandler;
					Action.IconSetChanged -= OnActionIconSetChangedHandler;
					Action.LabelChanged -= OnActionLabelChangedHandler;
					Action.TooltipChanged -= OnActionTooltipChangedHandler;
					Action.VisibleChanged -= OnActionVisibleChangedHandler;
					Control.SelectedLocaleChanged -= OnControlSelectedLocaleChanged;
				}
				base.Dispose(disposing);
			}

			private LanguagePickerAction Action { get; set; }

			private new LocalePicker Control
			{
				get { return (LocalePicker) base.Control; }
			}

			public IconSize IconSize
			{
				get { return _iconSize; }
				set
				{
					if (_iconSize != value)
					{
						_iconSize = value;
						UpdateIcon();
					}
				}
			}

			private void OnActionIconSetChangedHandler(object sender, EventArgs e)
			{
				UpdateIcon();
			}

			private void OnActionLabelChangedHandler(object sender, EventArgs e)
			{
				UpdateLabel();
			}

			private void OnActionAvailableChangedHandler(object sender, EventArgs e)
			{
				UpdateEnablement();
				UpdateVisibility();
			}

			private void OnActionVisibleChangedHandler(object sender, EventArgs e)
			{
				UpdateVisibility();
			}

			private void OnActionTooltipChangedHandler(object sender, EventArgs e)
			{
				UpdateTooltip();
			}

			private void OnActionEnabledChangedHandler(object sender, EventArgs e)
			{
				UpdateEnablement();
			}

			private void OnControlSelectedLocaleChanged(object sender, EventArgs e)
			{
				Action.SelectedLocale = Control.SelectedLocale;
			}

			private void UpdateVisibility()
			{
				Available = Action.Available && Action.Visible && (Action.Permissible || DesktopViewSettings.Default.ShowNonPermissibleActions);
			}

			private void UpdateEnablement()
			{
				Enabled = Action.Available && Action.Enabled && (Action.Permissible || DesktopViewSettings.Default.EnableNonPermissibleActions);
			}

			private void UpdateLabel()
			{
				Text = Action.Label;
			}

			private void UpdateTooltip()
			{
				ToolTipText = Action.Tooltip;
			}

			private void UpdateIcon()
			{
				ActionViewUtils.SetIcon(this, Action, IconSize);
			}
		}
	}
}