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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
	public class DropDownButtonToolbarItem : ToolStripSplitButton
	{
		#region Fake Button for Rendering 'Checked' drop-down button

		private class FakeToolstripButton : ToolStripButton
		{
			private readonly DropDownButtonToolbarItem _owner;

			public FakeToolstripButton(DropDownButtonToolbarItem owner)
			{
				_owner = owner;
			}

			public override ToolStripItemDisplayStyle DisplayStyle
			{
				get
				{
					return _owner.DisplayStyle;
				}
				set
				{
				}
			}

			public override bool Enabled
			{
				get
				{
					return _owner.Enabled;
				}
				set
				{
				}
			}

			public override Image Image
			{
				get
				{
					if ((_owner.DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
						return _owner.Image;

					return null;
				}
				set
				{
				}
			}

			public override Size Size
			{
				get
				{
					// Width+1 so that the black borders of the button and drop-down line up.  This has
					// only been tested with the 'Professional' renderer and may not work properly
					// with other renderers.
					return new Size(_owner.ButtonBounds.Width + 1, _owner.ButtonBounds.Height);
				}
				set
				{
				}
			}

			public override Padding Padding
			{
				get
				{
					return _owner.Padding;
				}
				set
				{
				}
			}

			public override bool Selected
			{
				get { return _owner.Selected; }
			}

			public override string Text
			{
				get
				{
					if ((_owner.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
						return _owner.Text;

					return null;
				}
				set
				{
				}
			}

			public override ToolStripTextDirection TextDirection
			{
				get
				{
					return _owner.TextDirection;
				}
			}
		}

		#endregion

		private ToolStripRenderer _currentRenderer;
		private FakeToolstripButton _fakeButton;

		private IClickAction _action;

		private EventHandler _actionEnabledChangedHandler;
		private EventHandler _actionVisibleChangedHandler;
		private EventHandler _actionAvailableChangedHandler;
		private EventHandler _actionLabelChangedHandler;
		private EventHandler _actionTooltipChangedHandler;
		private EventHandler _actionIconSetChangedHandler;
		private EventHandler _actionCheckedChangedHandler;

		private IconSize _iconSize;

		public DropDownButtonToolbarItem(IClickAction action)
			: this(action, IconSize.Medium)
		{
		}

		public DropDownButtonToolbarItem(IClickAction action, IconSize iconSize)
			: base()
		{
			IDropDownAction dropAction = (IDropDownAction) action;

			_fakeButton = new FakeToolstripButton(this);
			base.DropDownButtonWidth = 15;

			_action = action;

			_actionEnabledChangedHandler = new EventHandler(OnActionEnabledChanged);
			_actionVisibleChangedHandler = new EventHandler(OnActionVisibleChanged);
			_actionAvailableChangedHandler = new EventHandler(OnActionAvailableChanged);
			_actionLabelChangedHandler = new EventHandler(OnActionLabelChanged);
			_actionTooltipChangedHandler = new EventHandler(OnActionTooltipChanged);
			_actionIconSetChangedHandler = new EventHandler(OnActionIconSetChanged);
			_actionCheckedChangedHandler = new EventHandler(OnActionCheckedChanged);

			_action.EnabledChanged += _actionEnabledChangedHandler;
			_action.VisibleChanged += _actionVisibleChangedHandler;
			_action.AvailableChanged += _actionAvailableChangedHandler;
			_action.LabelChanged += _actionLabelChangedHandler;
			_action.TooltipChanged += _actionTooltipChangedHandler;
			_action.IconSetChanged += _actionIconSetChangedHandler;
			_action.CheckedChanged += _actionCheckedChangedHandler;

			_iconSize = iconSize;

			this.DropDown = new ContextMenuStrip();
			this.DropDown.ImageScalingSize = StandardIconSizes.Small;
			this.DropDownOpening += new EventHandler(OnDropDownOpening);

			this.Text = _action.Label;
			this.Enabled = _action.Enabled;
			this.Visible = _action.Visible;
			SetTooltipText();

			UpdateCheckedState();
			UpdateVisibility();
			UpdateEnablement();
			UpdateIcon();

			this.ButtonClick += delegate(object sender, EventArgs e)
			{
				_action.Click();
			};
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

		private void OnDropDownOpening(object sender, EventArgs e)
		{
			ToolStripBuilder.Clear(this.DropDownItems);
			
			ActionModelNode model = ((IDropDownAction)_action).DropDownMenuModel;
			if (model != null)
				ToolStripBuilder.BuildMenu(this.DropDownItems, model.ChildNodes);
		}

		protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
		{
			base.OnParentChanged(oldParent, newParent);

			if (oldParent != null)
			{
				oldParent.RendererChanged -= OnParentRendererChanged;
				UpdateRenderer(null);
			}

			if (newParent != null)
			{
				newParent.RendererChanged += OnParentRendererChanged;
				UpdateRenderer(newParent);
			}
		}

		private void OnParentRendererChanged(object sender, EventArgs e)
		{
			UpdateRenderer(this.Parent);
		}

		private void UpdateRenderer(ToolStrip newParent)
		{
			if (_currentRenderer != null && (newParent == null || _currentRenderer != newParent.Renderer))
			{
				_currentRenderer.RenderSplitButtonBackground -= OnRenderedSplitButtonBackground;
				_currentRenderer = null;
			}

			if (newParent != null)
			{
				_currentRenderer = newParent.Renderer;
				_currentRenderer.RenderSplitButtonBackground += OnRenderedSplitButtonBackground;
			}
		}

		private void OnRenderedSplitButtonBackground(object sender, ToolStripItemRenderEventArgs e)
		{
			// inject drawing of a checked 'button' into the rendering pipeline
			DropDownButtonToolbarItem clickItem = e.Item as DropDownButtonToolbarItem;
			if (clickItem != null && _fakeButton.Checked)
				base.Parent.Renderer.DrawButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, clickItem._fakeButton));
		}

		private void OnActionCheckedChanged(object sender, EventArgs e)
		{
			UpdateCheckedState();
		}

		private void OnActionEnabledChanged(object sender, EventArgs e)
		{
			UpdateEnablement();
		}

		private void OnActionVisibleChanged(object sender, EventArgs e)
		{
			UpdateVisibility();
		}

		private void OnActionAvailableChanged(object sender, EventArgs e)
		{
			UpdateEnablement();
			UpdateVisibility();
		}

		private void OnActionLabelChanged(object sender, EventArgs e)
		{
			this.Text = _action.Label;
		}

		private void OnActionTooltipChanged(object sender, EventArgs e)
		{
			SetTooltipText();
		}

		private void OnActionIconSetChanged(object sender, EventArgs e)
		{
			UpdateIcon();
		}

		private void UpdateCheckedState()
		{
			_fakeButton.Checked = _action.Checked;
			this.Invalidate();
		}

		private void UpdateVisibility()
		{
			this.Visible = _action.Available && _action.Visible && (_action.Permissible || DesktopViewSettings.Default.ShowNonPermissibleActions);
		}

		private void UpdateEnablement()
		{
			this.Enabled = _action.Available && _action.Enabled && (_action.Permissible || DesktopViewSettings.Default.EnableNonPermissibleActions);
		}

		private void UpdateIcon()
		{
			ActionViewUtils.SetIcon(this, _action, _iconSize);
		}

		private void SetTooltipText()
		{
			ActionViewUtils.SetTooltipText(this, _action);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _action != null)
			{
				OnParentChanged(this.Parent, null);

				ToolStripBuilder.Clear(this.DropDownItems);

				// VERY IMPORTANT: instances of this class will be created and discarded frequently
				// throughout the lifetime of the application
				// therefore is it extremely important that the event handlers are disconnected
				// from the underlying _action events
				// otherwise, this object will hang around for the entire lifetime of the _action object,
				// even though this object is no longer needed
				_action.EnabledChanged -= _actionEnabledChangedHandler;
				_action.CheckedChanged -= _actionCheckedChangedHandler;
				_action.VisibleChanged -= _actionVisibleChangedHandler;
				_action.AvailableChanged -= _actionAvailableChangedHandler;
				_action.LabelChanged -= _actionLabelChangedHandler;
				_action.TooltipChanged -= _actionTooltipChangedHandler;
				_action.IconSetChanged -= _actionIconSetChangedHandler;

				_action = null;
			}

			base.Dispose(disposing);
		}
	}
}