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

using System.Collections.Generic;
using System.ComponentModel;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A <see cref="GalleryComponent"/> that supports drag-reordering of displayed items.
	/// </summary>
	/// <remarks>
	/// This class can be overriden to complete the gallery with drag and drop support for importing and exporting both <see cref="IGalleryItem"/>s
	/// and other objects. The default implemention only allowes reordering of items within itself.
	/// </remarks>
	public class ReorderableGalleryComponent : GalleryComponent
	{
		private IList<IGalleryItem> _draggedItems;

		/// <summary>
		/// Constructs an empty <see cref="ReorderableGalleryComponent"/> without any tool actions.
		/// </summary>
		public ReorderableGalleryComponent() : base() {}

		/// <summary>
		/// Constructs a <see cref="ReorderableGalleryComponent"/> with the specified data source and without any tool actions.
		/// </summary>
		/// <param name="dataSource">An <see cref="IBindingList"/> of <see cref="IGalleryItem"/>s.</param>
		public ReorderableGalleryComponent(IBindingList dataSource) : base(dataSource) {}

		/// <summary>
		/// Constructs an empty <see cref="ReorderableGalleryComponent"/>, automatically adding the actions of
		/// <see cref="GalleryToolExtensionPoint"/>s at the specified action sites.
		/// </summary>
		/// <param name="toolbarSite">The site for toolbar actions.</param>
		/// <param name="contextMenuSite">The site for context menu actions.</param>
		public ReorderableGalleryComponent(string toolbarSite, string contextMenuSite) : base(toolbarSite, contextMenuSite) {}

		/// <summary>
		/// Constructs a <see cref="ReorderableGalleryComponent"/> with the specified data source, automatically adding the actions of
		/// <see cref="GalleryToolExtensionPoint"/>s at the specified action sites.
		/// </summary>
		/// <param name="dataSource">An <see cref="IBindingList"/> of <see cref="IGalleryItem"/>s.</param>
		/// <param name="toolbarSite">The site for toolbar actions.</param>
		/// <param name="contextMenuSite">The site for context menu actions.</param>
		public ReorderableGalleryComponent(IBindingList dataSource, string toolbarSite, string contextMenuSite)
			: base(dataSource, toolbarSite, contextMenuSite) {}

		/// <summary>
		/// Gets or sets the list of items being dragged.
		/// </summary>
		protected IList<IGalleryItem> DraggedItems
		{
			get { return _draggedItems; }
			set { _draggedItems = value; }
		}

		/// <summary>
		/// Gets if the gallery supports any drag and drop interaction in between items.
		/// </summary>
		public override bool AllowsDropAtIndex
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a list of  <see cref="IGalleryItem"/>s from the given <see cref="IDragDropObject">data wrapper</see>.
		/// </summary>
		/// <param name="dataObject">The data wrapper object.</param>
		/// <returns>A <see cref="IList{T}"/> of <see cref="IGalleryItem"/>s, or null if the wrapper did not contain a list of 0 or more IGalleryItems.</returns>
		protected IList<IGalleryItem> ExtractGalleryItemList(IDragDropObject dataObject)
		{
			IList<IGalleryItem> itemlist = null;
			if (dataObject.HasData<IList<IGalleryItem>>())
			{
				itemlist = dataObject.GetData<IList<IGalleryItem>>();
			}

			foreach (string format in dataObject.GetFormats())
			{
				object obj = dataObject.GetData(format);
				if (obj is IList<IGalleryItem>)
					itemlist = obj as IList<IGalleryItem>;
			}

			if (itemlist != null && itemlist.Count == 0)
				return null;
			return itemlist;
		}

		/// <summary>
		/// Signals the component that a drag &amp; drop operation involving the specified
		/// <see cref="IGalleryItem"/>s has started on the associated view.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="draggedItems">The <see cref="IGalleryItem"/>s being dragged.</param>
		public override sealed DragDropOption BeginDrag(IList<IGalleryItem> draggedItems)
		{
			_draggedItems = draggedItems;
			return DragDropOption.Move | DragDropOption.Copy;
		}

		/// <summary>
		/// Signals the component that a drag &amp; drop operation involving the specified
		/// <see cref="IGalleryItem"/>s has ended with the given action being taken on the items by the drop target.
		/// </summary>
		/// <param name="draggedItems">The <see cref="IGalleryItem"/>s that were dragged.</param>
		/// <param name="action">The <see cref="DragDropOption"/> action that was taken on the items by the drop target.</param>
		public override sealed void EndDrag(IList<IGalleryItem> draggedItems, DragDropOption action)
		{
			if (_draggedItems != null && draggedItems == _draggedItems)
			{
				if ((action & DragDropOption.Move) == DragDropOption.Move)
				{
					foreach (IGalleryItem draggedItem in draggedItems)
					{
						base.DataSource.Remove(draggedItem);
					}
				}
				_draggedItems = null;
			}
		}

		/// <summary>
		/// Checks for allowed drag &amp; drop actions involving the specified foreign data and the given target on this component.
		/// </summary>
		/// <param name="droppingData">The <see cref="IDragDropObject"/> object that encapsulates all forms of the foreign data.</param>
		/// <param name="targetIndex">The target index that the user is trying to drop at.</param>
		/// <param name="actions"></param>
		/// <param name="modifiers">The modifier keys that are being held by the user.</param>
		/// <returns>The allowed <see cref="DragDropKind"/> actions for this attempted drag &amp; drop operation.</returns>
		public override sealed DragDropOption CheckDrop(IDragDropObject droppingData, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			DragDropOption allowedActions;
			IList<IGalleryItem> droppingItems = ExtractGalleryItemList(droppingData);
			if (droppingItems != null)
			{
				if (_draggedItems == droppingItems)
				{
					allowedActions = CheckDropLocalItems(droppingItems, targetIndex, actions, modifiers);
				}
				else
				{
					allowedActions = CheckDropForeignItems(droppingItems, targetIndex, actions, modifiers);
				}
			}
			else
			{
				allowedActions = CheckDropForeignObject(droppingData, targetIndex, actions, modifiers);
			}
			return actions & allowedActions;
		}

		/// <summary>
		/// Checks for allowed drag &amp; drop actions involving the specified foreign data and the given target on this component.
		/// </summary>
		/// <param name="droppingData">The <see cref="IDragDropObject"/> object that encapsulates all forms of the foreign data.</param>
		/// <param name="targetItem">The target item that the user is trying to drop on to.</param>
		/// <param name="actions"></param>
		/// <param name="modifiers">The modifier keys that are being held by the user.</param>
		/// <returns>The allowed <see cref="DragDropKind"/> action for this attempted drag &amp; drop operation.</returns>
		public override sealed DragDropOption CheckDrop(IDragDropObject droppingData, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			DragDropOption allowedActions;
			IList<IGalleryItem> droppingItems = ExtractGalleryItemList(droppingData);
			if (droppingItems != null)
			{
				if (_draggedItems == droppingItems)
				{
					allowedActions = CheckDropLocalItems(droppingItems, targetItem, actions, modifiers);
				}
				else
				{
					allowedActions = CheckDropForeignItems(droppingItems, targetItem, actions, modifiers);
				}
			}
			else
			{
				allowedActions = CheckDropForeignObject(droppingData, targetItem, actions, modifiers);
			}
			return actions & allowedActions;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This method or <see cref="GalleryComponent.PerformDrop(IDragDropObject,IGalleryItem,DragDropOption,ModifierFlags)"/> may be called
		/// additional times if the returned action is <see cref="DragDropOption.None"/> in order to attempt other ways to drop the item in
		/// an acceptable manner. It is thus very important that the result be set properly if the drop was accepted and no further attempts
		/// should be made.
		/// </remarks>
		/// <param name="droppedData"></param>
		/// <param name="targetIndex"></param>
		/// <param name="action"></param>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public override sealed DragDropOption PerformDrop(IDragDropObject droppedData, int targetIndex, DragDropOption action, ModifierFlags modifiers)
		{
			DragDropOption performedAction;
			IList<IGalleryItem> droppedItems = ExtractGalleryItemList(droppedData);
			if (droppedItems != null)
			{
				if (_draggedItems == droppedItems)
				{
					performedAction = PerformDropLocalItems(droppedItems, targetIndex, action, modifiers);
				}
				else
				{
					performedAction = PerformDropForeignItems(droppedItems, targetIndex, action, modifiers);
				}
			}
			else
			{
				performedAction = PerformDropForeignObject(droppedData, targetIndex, action, modifiers);
			}
			return performedAction;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="droppedData"></param>
		/// <param name="targetItem"></param>
		/// <param name="action"></param>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public override sealed DragDropOption PerformDrop(IDragDropObject droppedData, IGalleryItem targetItem, DragDropOption action, ModifierFlags modifiers)
		{
			DragDropOption performedAction;
			IList<IGalleryItem> droppedItems = ExtractGalleryItemList(droppedData);
			if (droppedItems != null)
			{
				if (_draggedItems == droppedItems)
				{
					performedAction = PerformDropLocalItems(droppedItems, targetItem, action, modifiers);
				}
				else
				{
					performedAction = PerformDropForeignItems(droppedItems, targetItem, action, modifiers);
				}
			}
			else
			{
				performedAction = PerformDropForeignObject(droppedData, targetItem, action, modifiers);
			}
			return performedAction;
		}

		#region Virtual Members

		/// <summary>
		/// Checks if drag-dropping <see cref="IGalleryItem"/>s from within this gallery to the specified target index is allowed.
		/// </summary>
		/// <param name="droppingItems">The list of <see cref="IGalleryItem"/>s to drop.</param>
		/// <param name="targetIndex">The target index at which the <paramref name="droppingItems"/> are being dropped.</param>
		/// <param name="actions">The interactions that are being allowed by the data source.</param>
		/// <param name="modifiers">The modifier keys that currently being pressed.</param>
		/// <returns>The allowed interactions for dropping the <paramref name="droppingItems"/> here.</returns>
		protected virtual DragDropOption CheckDropLocalItems(IList<IGalleryItem> droppingItems, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			DragDropOption allowedActions = DragDropOption.None;
			if (modifiers == ModifierFlags.None) {
				// check for null drops and drops to a point within the source data
				if (droppingItems.Count == 0)
					return DragDropOption.None;

				int draggedIndex = base.DataSource.IndexOf(droppingItems[0]);
				if (targetIndex >= draggedIndex && targetIndex < draggedIndex + droppingItems.Count)
					return DragDropOption.None;

				// we are dragging an item, and the item we want to drop is the same as that which we are dragging
				// then this is a reordering operation
				allowedActions = actions & DragDropOption.Move;
			}
			return allowedActions;
		}

		/// <summary>
		/// Checks if drag-dropping <see cref="IGalleryItem"/>s from outside this gallery to the specified target index is allowed.
		/// </summary>
		/// <param name="droppingItems">The list of <see cref="IGalleryItem"/>s to drop.</param>
		/// <param name="targetIndex">The target index at which the <paramref name="droppingItems"/> are being dropped.</param>
		/// <param name="actions">The interactions that are being allowed by the data source.</param>
		/// <param name="modifiers">The modifier keys that currently being pressed.</param>
		/// <returns>The allowed interactions for dropping the <paramref name="droppingItems"/> here.</returns>
		protected virtual DragDropOption CheckDropForeignItems(IList<IGalleryItem> droppingItems, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Checks if drag-dropping non-<see cref="IGalleryItem"/> objects from outside this gallery to the specified index is allowed.
		/// </summary>
		/// <param name="droppingData">The data object to drop.</param>
		/// <param name="targetIndex">The target index at which the <paramref name="droppingData"/> is being dropped.</param>
		/// <param name="actions">The interactions that are being allowed by the data source.</param>
		/// <param name="modifiers">The modifier keys that currently being pressed.</param>
		/// <returns>The allowed interactions for dropping the <paramref name="droppingData"/> here.</returns>
		protected virtual DragDropOption CheckDropForeignObject(IDragDropObject droppingData, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Checks if drag-dropping <see cref="IGalleryItem"/>s from within this gallery on top of the specified item is allowed.
		/// </summary>
		/// <param name="droppingItems">The list of <see cref="IGalleryItem"/>s to drop.</param>
		/// <param name="targetItem">The target <see cref="IGalleryItem"/> at which the <paramref name="droppingItems"/> are being dropped.</param>
		/// <param name="actions">The interactions that are being allowed by the data source.</param>
		/// <param name="modifiers">The modifier keys that currently being pressed.</param>
		/// <returns>The allowed interactions for dropping the <paramref name="droppingItems"/> here.</returns>
		protected virtual DragDropOption CheckDropLocalItems(IList<IGalleryItem> droppingItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Checks if drag-dropping <see cref="IGalleryItem"/>s from outside this gallery on top of the specified item is allowed.
		/// </summary>
		/// <param name="droppingItems">The list of <see cref="IGalleryItem"/>s to drop.</param>
		/// <param name="targetItem">The target <see cref="IGalleryItem"/> at which the <paramref name="droppingItems"/> are being dropped.</param>
		/// <param name="actions">The interactions that are being allowed by the data source.</param>
		/// <param name="modifiers">The modifier keys that currently being pressed.</param>
		/// <returns>The allowed interactions for dropping the <paramref name="droppingItems"/> here.</returns>
		protected virtual DragDropOption CheckDropForeignItems(IList<IGalleryItem> droppingItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Checks if drag-dropping non-<see cref="IGalleryItem"/> objects from outside this gallery on top of the specified item is allowed.
		/// </summary>
		/// <param name="droppingData">The data object to drop.</param>
		/// <param name="targetItem">The target <see cref="IGalleryItem"/> at which the <paramref name="droppingData"/> is being dropped.</param>
		/// <param name="actions">The interactions that are being allowed by the data source.</param>
		/// <param name="modifiers">The modifier keys that currently being pressed.</param>
		/// <returns>The allowed interactions for dropping the <paramref name="droppingData"/> here.</returns>
		protected virtual DragDropOption CheckDropForeignObject(IDragDropObject droppingData, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Performs a drag-drop of <see cref="IGalleryItem"/>s from within this gallery to the specified target index.
		/// </summary>
		/// <param name="droppedItems">The list of <see cref="IGalleryItem"/>s to drop.</param>
		/// <param name="targetIndex">The target index at which the <paramref name="droppedItems"/> are being dropped.</param>
		/// <param name="actions">The interaction to take.</param>
		/// <param name="modifiers">The modifier keys that were pressed at the time of the drop.</param>
		/// <returns>The actual interaction on the <paramref name="droppedItems"/> that was taken.</returns>
		protected virtual DragDropOption PerformDropLocalItems(IList<IGalleryItem> droppedItems, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			DragDropOption performedAction = DragDropOption.None;
			if (modifiers == ModifierFlags.None)
			{
				// check for null drops and drops to a point within the source data
				if (droppedItems.Count == 0)
					return DragDropOption.None;

				int draggedIndex = base.DataSource.IndexOf(droppedItems[0]);
				if (targetIndex >= draggedIndex && targetIndex < draggedIndex + droppedItems.Count)
					return DragDropOption.None;

				// we are dragging something, and the item we want to drop is the same as that which we are dragging
				// then this is a reordering operation
				if (droppedItems != this.DraggedItems)
					return DragDropOption.None;

				if (draggedIndex < targetIndex)
					targetIndex -= droppedItems.Count;

				Stack<IGalleryItem> stack = new Stack<IGalleryItem>();
				foreach (IGalleryItem droppedItem in droppedItems) {
					base.DataSource.Remove(droppedItem);
					stack.Push(droppedItem);
				}
				while (stack.Count > 0) {
					base.DataSource.Insert(targetIndex, stack.Pop());
				}
				this.DraggedItems = null;

				performedAction = DragDropOption.Move;
			}
			return performedAction;
		}

		/// <summary>
		/// Performs a drag-drop of <see cref="IGalleryItem"/>s from outside this gallery to the specified target index.
		/// </summary>
		/// <param name="droppedItems">The list of <see cref="IGalleryItem"/>s to drop.</param>
		/// <param name="targetIndex">The target index at which the <paramref name="droppedItems"/> are being dropped.</param>
		/// <param name="actions">The interaction to take.</param>
		/// <param name="modifiers">The modifier keys that were pressed at the time of the drop.</param>
		/// <returns>The actual interaction on the <paramref name="droppedItems"/> that was taken.</returns>
		protected virtual DragDropOption PerformDropForeignItems(IList<IGalleryItem> droppedItems, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Performs a drag-drop of a non-<see cref="IGalleryItem"/> object from outside this gallery to the specified index.
		/// </summary>
		/// <param name="droppedData">The data object to drop.</param>
		/// <param name="targetIndex">The target index at which the <paramref name="droppedData"/> is being dropped.</param>
		/// <param name="actions">The interaction to take.</param>
		/// <param name="modifiers">The modifier keys that were pressed at the time of the drop.</param>
		/// <returns>The actual interaction on the <paramref name="droppedData"/> that was taken.</returns>
		protected virtual DragDropOption PerformDropForeignObject(IDragDropObject droppedData, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Performs a drag-drop of <see cref="IGalleryItem"/>s from within this gallery on top of the specified item.
		/// </summary>
		/// <param name="droppedItems">The list of <see cref="IGalleryItem"/>s to drop.</param>
		/// <param name="targetItem">The target <see cref="IGalleryItem"/> at which the <paramref name="droppedItems"/> are being dropped.</param>
		/// <param name="actions">The interaction to take.</param>
		/// <param name="modifiers">The modifier keys that were pressed at the time of the drop.</param>
		/// <returns>The actual interaction on the <paramref name="droppedItems"/> that was taken.</returns>
		protected virtual DragDropOption PerformDropLocalItems(IList<IGalleryItem> droppedItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Performs a drag-drop of <see cref="IGalleryItem"/>s from outside this gallery on top of the specified item.
		/// </summary>
		/// <param name="droppedItems">The list of <see cref="IGalleryItem"/>s to drop.</param>
		/// <param name="targetItem">The target <see cref="IGalleryItem"/> at which the <paramref name="droppedItems"/> are being dropped.</param>
		/// <param name="actions">The interaction to take.</param>
		/// <param name="modifiers">The modifier keys that were pressed at the time of the drop.</param>
		/// <returns>The actual interaction on the <paramref name="droppedItems"/> that was taken.</returns>
		protected virtual DragDropOption PerformDropForeignItems(IList<IGalleryItem> droppedItems, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Performs a drag-drop of a non-<see cref="IGalleryItem"/> object from outside this gallery on top of the specified item.
		/// </summary>
		/// <param name="droppedData">The data object to drop.</param>
		/// <param name="targetItem">The target <see cref="IGalleryItem"/> at which the <paramref name="droppedData"/> is being dropped.</param>
		/// <param name="actions">The interaction to take.</param>
		/// <param name="modifiers">The modifier keys that were pressed at the time of the drop.</param>
		/// <returns>The actual interaction on the <paramref name="droppedData"/> that was taken.</returns>
		protected virtual DragDropOption PerformDropForeignObject(IDragDropObject droppedData, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		#endregion
	}
}