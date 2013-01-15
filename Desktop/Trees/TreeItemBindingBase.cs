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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Base implementation of <see cref="ITreeItemBinding"/>.
    /// </summary>
    /// <remarks>
	/// Provides null default implementations of most methods.
	/// </remarks>
    public abstract class TreeItemBindingBase : ITreeItemBinding
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		protected TreeItemBindingBase()
		{
		}

    	#region ITreeItemBinding members

    	/// <summary>
    	/// Gets the text to display for the node representing the specified item.
    	/// </summary>
    	public abstract string GetNodeText(object item);

		/// <summary>
		/// Sets the text to display for the node representing the specified item.
		/// </summary>
		public virtual void SetNodeText(object item, string text)
		{
			return;
		}

    	/// <summary>
    	/// Asks if the item text can be changed.
    	/// </summary>
    	public virtual bool CanSetNodeText(object item)
		{
			return false;
		}

		/// <summary>
    	/// Gets whether or not <paramref name="item"/> is checked.
    	/// </summary>
    	public virtual bool GetIsChecked(object item)
        {
            return false;
        }

    	/// <summary>
    	/// Sets whether or not <paramref name="item"/> is checked.
    	/// </summary>
    	public virtual void SetIsChecked(object item, bool value)
        {
            return;
        }

    	/// <summary>
    	/// Gets a value indicating the <see cref="CheckState"/> of the <paramref name="item"/>.
    	/// </summary>
    	public virtual CheckState GetCheckState(object item)
    	{
    		return this.GetIsChecked(item) ? CheckState.Checked : CheckState.Unchecked;
    	}

    	/// <summary>
    	/// Toggles the <see cref="CheckState"/> of the <paramref name="item"/>.
    	/// </summary>
    	public virtual CheckState ToggleCheckState(object item)
    	{
    		bool value = !this.GetIsChecked(item);
    		this.SetIsChecked(item, value);
    		return value ? CheckState.Checked : CheckState.Unchecked;
    	}

    	/// <summary>
    	/// Gets the tooltip to display for the specified item.
    	/// </summary>
    	public virtual string GetTooltipText(object item)
        {
            return null;
        }

    	/// <summary>
    	/// Gets the image iconset to display for the specified item.
    	/// </summary>
    	public virtual IconSet GetIconSet(object item)
        {
            return null;
        }

		/// <summary>
		/// Gets whether the specified item should be highlighted.
		/// </summary>
		public virtual bool GetIsHighlighted(object item)
		{
			return false;
		}

    	/// <summary>
    	/// Gets the resource resolver used to resolve the icon(s).
    	/// </summary>
    	public virtual IResourceResolver GetResourceResolver(object item)
        {
            return null;
        }

    	/// <summary>
    	/// Asks if the item can have a subtree.
    	/// </summary>
    	/// <remarks>
    	/// Note that this method should return true to inidicate that it
    	/// is possible that the item might have a subtree.  This allows the view to determine whether to display
    	/// a "plus" sign next to the node, without having to actually call <see cref="GetSubTree"/>.
    	/// </remarks>
    	public virtual bool CanHaveSubTree(object item)
        {
            return true;
        }

    	/// <summary>
    	/// Gets a value indicating if the item should be expanded when the tree is initially loaded.
    	/// </summary>
    	public virtual bool GetExpanded(object item)
        {
            return false;
        }

    	/// <summary>
    	/// Sets a value indicating whether the specified item is currently expanded.
    	/// </summary>
    	/// <param name="item"></param>
    	/// <param name="expanded"></param>
    	/// <returns></returns>
    	public virtual void SetExpanded(object item, bool expanded)
		{
			
		}

    	/// <summary>
    	/// Gets the <see cref="ITree"/> that represents the subtree for the specified item,
    	/// or null if the item does not have a subtree.
    	/// </summary>
    	/// <remarks>
    	/// Note that <see cref="CanHaveSubTree"/> is called first,
    	/// and this method will be called only if <see cref="CanHaveSubTree"/> returns true.
    	/// </remarks>
    	public virtual ITree GetSubTree(object item)
        {
            return null;
        }

    	/// <summary>
    	/// Asks the specified item if it can accept the specified drop data in a drag-drop operation.
    	/// </summary>
    	/// <param name="item">The item being drag-dropped.</param>
    	/// <param name="dropData">Information about the item drag-dropped.</param>
    	/// <param name="kind">The drop kind being performed.</param>
    	/// <returns>The drop kind that will be accepted.</returns>
    	public virtual DragDropKind CanAcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return DragDropKind.None;
        }

    	/// <summary>
    	/// Informs the specified item that it should accept a drop of the specified data, completing a drag-drop operation.
    	/// </summary>
    	/// <param name="item">The item being drag-dropped.</param>
    	/// <param name="dropData">Information about the item being drag-dropped.</param>
    	/// <param name="kind">The drop kind being performed.</param>
    	/// <returns>The drop kind that will be accepted.</returns>
    	public virtual DragDropKind AcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return DragDropKind.None;
        }

    	/// <summary>
    	/// Asks the specified item if it can accept the specified drop data in a drag-drop operation.
    	/// </summary>
    	/// <param name="item">The item being drag-dropped.</param>
    	/// <param name="dropData">Information about the item drag-dropped.</param>
		/// <param name="kind">The drop kind being performed.</param>
		/// <param name="position">The position of the drop location relative to <paramref name="item"/>.</param>
    	/// <returns>The drop kind that will be accepted.</returns>
    	public virtual DragDropKind CanAcceptDrop(object item, object dropData, DragDropKind kind, DragDropPosition position)
    	{
    		return this.CanAcceptDrop(item, dropData, kind);
    	}

    	/// <summary>
    	/// Informs the specified item that it should accept a drop of the specified data, completing a drag-drop operation.
    	/// </summary>
    	/// <param name="item">The item being drag-dropped.</param>
    	/// <param name="dropData">Information about the item being drag-dropped.</param>
		/// <param name="kind">The drop kind being performed.</param>
		/// <param name="position">The position of the drop location relative to <paramref name="item"/>.</param>
    	/// <returns>The drop kind that will be accepted.</returns>
		public virtual DragDropKind AcceptDrop(object item, object dropData, DragDropKind kind, DragDropPosition position)
    	{
			return this.AcceptDrop(item, dropData, kind);
    	}

        #endregion
    }
}
