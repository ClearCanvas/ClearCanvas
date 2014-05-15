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

using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Trees
{
	/// <summary>
	/// Defines the interface to a tree-item binding, which describes how attributes of the visible tree are obtained
	/// from the underlying item.
	/// </summary>
	public interface ITreeItemBinding
	{
		/// <summary>
		/// Gets the text to display for the node representing the specified item.
		/// </summary>
		[return : Localizable(true)]
		string GetNodeText(object item);

		/// <summary>
		/// Sets the text to display for the node representing the specified item.
		/// </summary>
		void SetNodeText(object item, [param : Localizable(true)] string text);

		/// <summary>
		/// Asks if the item text can be changed.
		/// </summary>
		bool CanSetNodeText(object item);

		/// <summary>
		/// Gets a value indicating the <see cref="CheckState"/> of the <paramref name="item"/>.
		/// </summary>
		CheckState GetCheckState(object item);

		/// <summary>
		/// Toggles the <see cref="CheckState"/> of the <paramref name="item"/>.
		/// </summary>
		CheckState ToggleCheckState(object item);

		/// <summary>
		/// Gets the tooltip to display for the specified item.
		/// </summary>
		[return : Localizable(true)]
		string GetTooltipText(object item);

		/// <summary>
		/// Gets the image iconset to display for the specified item.
		/// </summary>
		IconSet GetIconSet(object item);

		/// <summary>
		/// Gets whether the specified item should be highlighted.
		/// </summary>
		bool GetIsHighlighted(object item);

		/// <summary>
		/// Gets the resource resolver used to resolve the icon(s).
		/// </summary>
		IResourceResolver GetResourceResolver(object item);

		/// <summary>
		/// Asks if the item can have a subtree.
		/// </summary>
		/// <remarks>
		/// Note that this method should return true to inidicate that it
		/// is possible that the item might have a subtree.  This allows the view to determine whether to display
		/// a "plus" sign next to the node, without having to actually call <see cref="GetSubTree"/>.
		/// </remarks>
		bool CanHaveSubTree(object item);

		/// <summary>
		/// Gets the <see cref="ITree"/> that represents the subtree for the specified item,
		/// or null if the item does not have a subtree.
		/// </summary>
		/// <remarks>
		/// Note that <see cref="CanHaveSubTree"/> is called first,
		/// and this method will be called only if <see cref="CanHaveSubTree"/> returns true.
		/// </remarks>
		ITree GetSubTree(object item);

		/// <summary>
		/// Gets a value indicating if the item should be expanded when the tree is initially loaded.
		/// </summary>
		bool GetExpanded(object item);

		/// <summary>
		/// Sets a value indicating whether the specified item is currently expanded.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="expanded"></param>
		/// <returns></returns>
		void SetExpanded(object item, bool expanded);

		/// <summary>
		/// Asks the specified item if it can accept the specified drop data in a drag-drop operation.
		/// </summary>
		/// <param name="item">The tree item that is being dropped on.</param>
		/// <param name="dropData">The object being dropped.</param>
		/// <param name="kind">The drop kind being performed.</param>
		/// <param name="position">The position of the drop location relative to <paramref name="item"/>.</param>
		/// <returns>The drop kind that will be accepted.</returns>
		DragDropKind CanAcceptDrop(object item, object dropData, DragDropKind kind, DragDropPosition position);

		/// <summary>
		/// Informs the specified item that it should accept a drop of the specified data, completing a drag-drop operation.
		/// </summary>
		/// <param name="item">The tree item that is being dropped on.</param>
		/// <param name="dropData">The object being dropped.</param>
		/// <param name="kind">The drop kind being performed.</param>
		/// <param name="position">The position of the drop location relative to <paramref name="item"/>.</param>
		/// <returns>The drop kind that will be accepted.</returns>
		DragDropKind AcceptDrop(object item, object dropData, DragDropKind kind, DragDropPosition position);
	}
}