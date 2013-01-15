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
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
	public static class ToolStripBuilder
	{
		public class ItemTag
		{
			private readonly ActionModelNode _node;
			private readonly IActionView _view;

			public ItemTag(ActionModelNode node, IActionView view)
			{
				_node = node;
				_view = view;
			}

			public ActionModelNode Node
			{
				get { return _node; }
			}

			public IActionView View
			{
				get { return _view; }	
			}
		}

		#region ToolStripKind

		public enum ToolStripKind
        {
            Menu,
            Toolbar
		}

		#endregion

		#region ToolStripBuilderStyle

		/// <summary>
		/// Specifies style charateristics for a tool strip.
		/// </summary>
		public class ToolStripBuilderStyle
        {
			/// <summary>
			/// Gets an object representing the default style defined by <see cref="DesktopViewSettings"/>.
			/// </summary>
        	public static ToolStripBuilderStyle GetDefault()
        	{
				return new ToolStripBuilderStyle(ToolStripItemDisplayStyle.Image,
											  DesktopViewSettings.Default.LocalToolStripItemAlignment,
											  DesktopViewSettings.Default.LocalToolStripItemTextImageRelation);
        	}


            private readonly ToolStripItemDisplayStyle _toolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
            private readonly ToolStripItemAlignment _toolStripItemAlignment = ToolStripItemAlignment.Left;
            private readonly TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;

            public ToolStripBuilderStyle(ToolStripItemDisplayStyle toolStripItemDisplayStyle, ToolStripItemAlignment toolStripItemAlignment, TextImageRelation textImageRelation)
            {
                _toolStripItemAlignment = toolStripItemAlignment;
                _toolStripItemDisplayStyle = toolStripItemDisplayStyle;
                _textImageRelation = textImageRelation;
            }

			[Obsolete("This overload will be removed in a future version of the framework.")]
			public ToolStripBuilderStyle(ToolStripItemDisplayStyle toolStripItemDisplayStyle)
            {
                _toolStripItemDisplayStyle = toolStripItemDisplayStyle;
            }

            public ToolStripBuilderStyle()
            {
            }

            public ToolStripItemAlignment ToolStripAlignment
            {
                get { return _toolStripItemAlignment; }
            }

            public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
            {
                get { return _toolStripItemDisplayStyle; }
            }

            public TextImageRelation TextImageRelation
            {
                get { return _textImageRelation; }
            }
		}

		#endregion

		#region Public API

		public static void ChangeIconSize(ToolStrip toolStrip, IconSize iconSize)
		{
			toolStrip.SuspendLayout();
			toolStrip.ImageScalingSize = StandardIconSizes.GetSize(iconSize);
			ChangeIconSize(toolStrip.Items, iconSize);
			toolStrip.ResumeLayout(false);
			toolStrip.PerformLayout();
		}

		/// <summary>
		/// Builds a toolstrip of the specified kind, from the specified action model nodes, using the default style and size.
		/// </summary>
		/// <param name="kind"></param>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
		public static void BuildToolStrip(ToolStripKind kind, ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes)
        {
			BuildToolStrip(kind, parentItemCollection, nodes, ToolStripBuilderStyle.GetDefault());
        }

		/// <summary>
		/// Builds a toolstrip of the specified kind, from the specified action model nodes, using the specified style and default size.
		/// </summary>
		/// <param name="kind"></param>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
		/// <param name="builderStyle"></param>
		public static void BuildToolStrip(ToolStripKind kind, ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripBuilderStyle builderStyle)
		{
			BuildToolStrip(kind, parentItemCollection, nodes, builderStyle, IconSize.Medium);
		}

		/// <summary>
		/// Builds a toolstrip of the specified kind, from the specified action model nodes, using the specified style and size.
		/// </summary>
		/// <param name="kind"></param>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
		/// <param name="builderStyle"></param>
		/// <param name="iconSize"></param>
		public static void BuildToolStrip(ToolStripKind kind, ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripBuilderStyle builderStyle, IconSize iconSize)
        {
            switch (kind)
            {
                case ToolStripKind.Menu:
                    BuildMenu(parentItemCollection, nodes);
                    break;
                case ToolStripKind.Toolbar:
					BuildToolbar(parentItemCollection, nodes, builderStyle, iconSize);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

		/// <summary>
		/// Builds a toolbar from the specified action model nodes, using the default style and size.
		/// </summary>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
        public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes)
        {
			BuildToolbar(parentItemCollection, nodes, ToolStripBuilderStyle.GetDefault());
        }

		/// <summary>
		/// Builds a toolbar from the specified action model nodes, using the specified style and the default size.
		/// </summary>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
		/// <param name="builderStyle"></param>
		public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripBuilderStyle builderStyle)
		{
			BuildToolbar(parentItemCollection, nodes, builderStyle, IconSize.Medium);
		}

		/// <summary>
		/// Builds a toolbar from the specified action model nodes, using the specified style and size.
		/// </summary>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
		/// <param name="builderStyle"></param>
		/// <param name="iconSize"></param>
		public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripBuilderStyle builderStyle, IconSize iconSize)
        {
			List<ActionModelNode> nodeList = CombineAdjacentSeparators(new List<ActionModelNode>(nodes));
			
			// reverse nodes if alignment is right
			if (builderStyle.ToolStripAlignment == ToolStripItemAlignment.Right)
				nodeList.Reverse();

			foreach (ActionModelNode node in nodeList)
            {
                if (node is ActionNode)
                {
                    IAction action = ((ActionNode)node).Action;
					IActionView view = CreateActionView(ToolStripKind.Toolbar, action, iconSize);
					ToolStripItem button = (ToolStripItem)view.GuiElement;
                    button.Tag = new ItemTag(node, view);

                    // By default, only display the image on the button
                    button.DisplayStyle = builderStyle.ToolStripItemDisplayStyle;
                    button.Alignment = builderStyle.ToolStripAlignment;
                    button.TextImageRelation = builderStyle.TextImageRelation;

                    parentItemCollection.Add(button);
                }
				else if(node is SeparatorNode)
				{
					ToolStripSeparator separator = new ToolStripSeparator();
					separator.Tag = new ItemTag(node, null);
					parentItemCollection.Add(separator);
				}
                else
                {
					BuildToolbar(parentItemCollection, node.ChildNodes, builderStyle, iconSize);
                }
            }
        }

		[Obsolete("This overload will be removed in a future version of the framework.")]
		public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripItemDisplayStyle toolStripItemDisplayStyle)
        {
			BuildToolbar(parentItemCollection, nodes, new ToolStripBuilderStyle(toolStripItemDisplayStyle));
        }

		/// <summary>
		/// Builds a menu from the specified action model nodes.
		/// </summary>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
        public static void BuildMenu(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes)
        {
			List<ActionModelNode> nodeList = CombineAdjacentSeparators(new List<ActionModelNode>(nodes));
			foreach (ActionModelNode node in nodeList)
            {
                ToolStripItem toolstripItem;

                if (node is ActionNode)
                {
                    // this is a leaf node (terminal menu item)
                	ActionNode actionNode = (ActionNode) node;
					IAction action = actionNode.Action;
					IActionView view = CreateActionView(ToolStripKind.Menu, action, IconSize.Medium);
					toolstripItem = (ToolStripItem)view.GuiElement;
					toolstripItem.Tag = new ItemTag(node, view);
                    parentItemCollection.Add(toolstripItem);

                    // Determine whether we should check the parent menu items too
					IClickAction clickAction = actionNode.Action as IClickAction;

                    if (clickAction != null && clickAction.CheckParents && clickAction.Checked)
                        CheckParentItems(toolstripItem);
                }
				else if (node is SeparatorNode)
				{
					toolstripItem = new ToolStripSeparator();
					toolstripItem.Tag = new ItemTag(node, null);
					parentItemCollection.Add(toolstripItem);
				}
				else
                {
                    // this menu item has a sub menu
                    toolstripItem = new ToolStripMenuItem(node.PathSegment.LocalizedText);

					toolstripItem.Tag = new ItemTag(node, null);
                    parentItemCollection.Add(toolstripItem);

                    BuildMenu(((ToolStripMenuItem)toolstripItem).DropDownItems, node.ChildNodes);
                }

                // When you get Visible, it refers to whether the object is really visible, as opposed to whether it _can_ be visible. 
                // When you _set_ Visible, it affects whether it _can_ be visible.
                // For example, an item is really invisible but _can_ be visible before it is actually drawn.
                // This is why we use the Available property, which give us the information when we are interested in "_Could_ this be Visible?"
                ToolStripMenuItem parent = toolstripItem.OwnerItem as ToolStripMenuItem;
                if (parent != null)
                {
                    SetParentAvailability(parent);
                    toolstripItem.AvailableChanged += delegate { SetParentAvailability(parent); };
                }
            }
        }


        public static void Clear(ToolStripItemCollection parentItemCollection)
        {
            // this is kinda dumb, but we can't just Dispose() of the items directly
            // because calling Dispose() alters the parent collection causing the
            // enumeration to fail - hence the temp array
            ToolStripItem[] temp = new ToolStripItem[parentItemCollection.Count];
            for (int i = 0; i < parentItemCollection.Count; i++)
            {
                temp[i] = parentItemCollection[i];
            }

            // item seems that calling Dispose() on the item will automatically recurse
            // to all it's children, so no need to recurse here
            foreach (ToolStripItem item in temp)
            {
                // the system may have added other items to the toolstrip,
                // so make sure we only delete our own
                if (item.Tag is ItemTag)
                {
                    item.Dispose();
                }
            }
		}

		#endregion

		#region Private Helpers

		private static void SetParentAvailability(ToolStripMenuItem parent)
		{
			bool parentIsAvailable = false;
			foreach (ToolStripItem item in parent.DropDownItems)
			{
				if (item.Available)
					parentIsAvailable = true;
			}

			if (parent.Available != parentIsAvailable)
				parent.Available = parentIsAvailable;
		}

		private static void CheckParentItems(ToolStripItem menuItem)
		{
			ToolStripMenuItem parentItem = menuItem.OwnerItem as ToolStripMenuItem;

			if (parentItem != null)
			{
				parentItem.Checked = true;
				CheckParentItems(parentItem);
			}

			return;
		}

		private static void ChangeIconSize(ToolStripItemCollection toolStripItems, IconSize iconSize)
		{
			foreach (ToolStripItem toolStripItem in toolStripItems)
			{
				ItemTag tag = toolStripItem.Tag as ItemTag;
				if (tag != null && tag.View != null)
					tag.View.Context.IconSize = iconSize;

				if (toolStripItem is ToolStripMenuItem)
				{
					ToolStripMenuItem item = (ToolStripMenuItem)toolStripItem;
					if (item.HasDropDownItems)
						ChangeIconSize(item.DropDownItems, iconSize);
				}
			}
		}

		private static List<ActionModelNode> CombineAdjacentSeparators(List<ActionModelNode> nodes)
		{
			// nothing to do if less than 2 items
			if(nodes.Count < 2)
				return nodes;

			List<ActionModelNode> result = new List<ActionModelNode>();
			result.Add(nodes[0]);
			for(int i = 1; i < nodes.Count; i++)
			{
				// if both this node and the previous node are separators, do not add this node to the result
				if(nodes[i] is SeparatorNode && nodes[i-1] is SeparatorNode)
					continue;

				result.Add(nodes[i]);
			}
			return result;
		}

		private static IActionView CreateActionView(ToolStripKind kind, IAction action, IconSize iconSize)
        {
			IActionView view = null;

            // optimization: for framework-provided actions, we can just create the controls
            // directly rather than use the associated view, which is slower;
            // however, an AssociateViewAttribute should always take precedence.
            if (action.GetType().GetCustomAttributes(typeof(AssociateViewAttribute), true).Length == 0)
            {
				if (kind == ToolStripKind.Toolbar)
				{
					if (action is IDropDownAction)
					{
						if (action is IClickAction)
							view = StandardWinFormsActionView.CreateDropDownButtonActionView();
						else
							view = StandardWinFormsActionView.CreateDropDownActionView();
					}
					else if(action is ITextBoxAction)
						view = StandardWinFormsActionView.CreateTextBoxActionView();
					else if (action is IClickAction)
						view = StandardWinFormsActionView.CreateButtonActionView();
				}
				else
				{
					if (action is IClickAction)
						view = StandardWinFormsActionView.CreateMenuActionView();
				}
            }
			if (view == null)
				view = (IActionView)ViewFactory.CreateAssociatedView(action.GetType());

			view.Context = new ActionViewContext(action, iconSize);
			return view;
		}

		#endregion
	}
}
