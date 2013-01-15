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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration.ActionModel;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	public partial class ActionModelConfigurationComponentControl : UserControl
	{
		private IActionModelConfigurationComponent _component;

		public ActionModelConfigurationComponentControl(IActionModelConfigurationComponent component)
		{
			InitializeComponent();

			_component = component;
			_component.SelectedNodeChanged += OnComponentSelectedNodeChanged;

			_actionModelTree.ShowToolbar = true;
			_actionModelTree.Tree = component.ActionModelTreeRoot;
			_actionModelTree.ToolbarModel = component.ToolbarActionModel;
			_actionModelTree.MenuModel = component.ContextMenuActionModel;
			_actionModelTree.ShowLines = !component.EnforceFlatActionModel;
			_actionModelTree.ShowRootLines = !component.EnforceFlatActionModel;
			_actionModelTree.ShowPlusMinus = !component.EnforceFlatActionModel;

			this.OnComponentSelectedNodeChanged(null, null);
		}

		private void PerformDispose(bool disposing)
		{
			if (_component != null)
			{
				_component.SelectedNodeChanged -= OnComponentSelectedNodeChanged;
				_component = null;
			}
		}

		public bool ShowActionPropertiesPane
		{
			get { return !_pnlSplit.Panel2Collapsed; }
			set { _pnlSplit.Panel2Collapsed = !value; }
		}

		private void OnComponentSelectedNodeChanged(object sender, EventArgs e)
		{
			AbstractActionModelTreeNode selectedNode = _component.SelectedNode;
			_pnlNodeProperties.SuspendLayout();
			try
			{
				_pnlNodeProperties.Visible = selectedNode != null;
				if (selectedNode != null)
				{
					_lblLabel.Text = selectedNode.CanonicalLabel;

					string tooltip = selectedNode.Tooltip;
					if (String.IsNullOrEmpty(tooltip))
						tooltip = selectedNode.CanonicalLabel;

					_toolTip.SetToolTip(_lblLabel, tooltip);
					_toolTip.SetToolTip(_pnlIcon, tooltip);

					if (!string.IsNullOrEmpty(selectedNode.Description))
					{
						_lblDescription.Text = selectedNode.Description;
						_lblDescription.Height = _lblDescription.GetPreferredSize(new Size(_lblDescription.Width, 100)).Height;
					}
					else
					{
						_lblDescription.Text = string.Empty;
						_lblDescription.Height = 0;
					}

					// destroy old icon
					Image image = _pnlIcon.BackgroundImage;
					_pnlIcon.BackgroundImage = null;
					if (image != null)
					{
						image.Dispose();
						image = null;
					}

					// set new icon
					IconSet iconSet = selectedNode.IconSet;
					if (iconSet != null)
					{
						try
						{
							image = iconSet.CreateIcon(IconSize.Medium, selectedNode.ResourceResolver);
						}
						catch (Exception ex)
						{
							Platform.Log(LogLevel.Debug, ex, "Icon resolution failed.");
						}
						_pnlIcon.BackgroundImage = image;
					}
					_pnlIcon.Visible = _pnlIcon.BackgroundImage != null;

					// reload properties extensions
					_lyoNodePropertiesExtensions.SuspendLayout();
					try
					{
						ArrayList oldControls = new ArrayList(_lyoNodePropertiesExtensions.Controls);
						_lyoNodePropertiesExtensions.Controls.Clear();
						foreach (Control c in oldControls)
							c.Dispose();
						foreach (IApplicationComponentView componentView in _component.SelectedNodeProperties.ComponentViews)
						{
							try
							{
								_lyoNodePropertiesExtensions.Controls.Add((Control) componentView.GuiElement);
							}
							catch (Exception ex)
							{
								Platform.Log(LogLevel.Debug, ex, "Error encountered while loading a component extension");
							}
						}
					}
					finally
					{
						_lyoNodePropertiesExtensions.ResumeLayout();
					}
					this.OnLyoNodePropertiesExtensionsSizeChanged(null, null);
				}
			}
			finally
			{
				_pnlNodeProperties.ResumeLayout(true);
			}
		}

		private void OnActionModelTreeSelectionChanged(object sender, EventArgs e)
		{
			AbstractActionModelTreeNode selectedNode = null;
			if (_actionModelTree.Selection != null)
				selectedNode = (_actionModelTree.Selection.Item as AbstractActionModelTreeNode);
			_component.SelectedNode = selectedNode;
		}

		private void OnBindingTreeViewItemDrag(object sender, ItemDragEventArgs e)
		{
			BindingTreeView bindingTreeView = sender as BindingTreeView;
			if (bindingTreeView == null)
				return;

			ISelection selection = e.Item as ISelection;
			if (selection != null && selection.Item != null)
			{
				bindingTreeView.DoDragDrop(selection.Item, DragDropEffects.All);
			}
		}

		private void OnLyoNodePropertiesExtensionsSizeChanged(object sender, EventArgs e)
		{
			foreach (Control control in _lyoNodePropertiesExtensions.Controls)
			{
				// for some reason, the controls get cut off even using the client area width...
				control.Width = _lyoNodePropertiesExtensions.ClientSize.Width - 8;
			}
		}
	}
}