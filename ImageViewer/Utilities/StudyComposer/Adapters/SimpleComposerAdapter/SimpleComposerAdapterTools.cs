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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter
{
	[ButtonAction("addnew", "studyComposer-toolbar/AddNew", "AddNew")]
	[MenuAction("addnew", "studyComposer-context/AddNew", "AddNew")]
	[VisibleStateObserver("addnew", "IsNotImageItemGallery", "IsNotImageItemGalleryChanged")]
	[IconSet("addnew", IconScheme.Colour, "AddToolSmall.png", "AddToolSmall.png", "AddToolSmall.png")]
	// Action: Delete
	[ButtonAction("delete", "studyComposer-toolbar/Delete", "Delete")]
	[EnabledStateObserver("delete", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[IconSet("delete", IconScheme.Colour, "DeleteToolSmall.png", "DeleteToolSmall.png", "DeleteToolSmall.png")]
	[MenuAction("cxt_delete", "studyComposer-context/Delete", "Delete")]
	[VisibleStateObserver("cxt_delete", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[IconSet("cxt_delete", IconScheme.Colour, "DeleteToolSmall.png", "DeleteToolSmall.png", "DeleteToolSmall.png")]
	// Action: Clear
	[ButtonAction("clear", "studyComposer-toolbar/Clear", "Clear")]
	[IconSet("clear", IconScheme.Colour, "DeleteAllToolSmall.png", "DeleteAllToolSmall.png", "DeleteAllToolSmall.png")]
	// Action: Edit Props
	[ButtonAction("editprops", "studyComposer-toolbar/Properties", "ShowProperties")]
	[EnabledStateObserver("editprops", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[IconSet("editprops", IconScheme.Colour, "EditToolSmall.png", "EditToolSmall.png", "EditToolSmall.png")]
	[MenuAction("cxt_editprops", "studyComposer-context/Properties", "ShowProperties")]
	[VisibleStateObserver("cxt_editprops", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[IconSet("cxt_editprops", IconScheme.Colour, "EditToolSmall.png", "EditToolSmall.png", "EditToolSmall.png")]
	// End Actions
	[ExtensionOf(typeof (GalleryToolExtensionPoint))]
	public class SimpleComposerAdapterTools : Tool<IGalleryToolContext>
	{
		public void Delete()
		{
			try
			{
				List<object> list = new List<object>();
				foreach (object o in base.Context.Selection.Items)
				{
					list.Add(o);
				}

				base.Context.Select(new List<IGalleryItem>(0)); // deselect all

				foreach (object o in list)
				{
					base.Context.DataSource.Remove(o);
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.Context.DesktopWindow);
			}
		}

		public void Clear()
		{
			try
			{
				if (base.Context.DataSource.Count > 0)
				{
					if (base.Context.DesktopWindow.ShowMessageBox(SR.MessageConfirmDeleteAllItems, MessageBoxActions.YesNo) == DialogBoxAction.Yes) {

						base.Context.Select(new List<IGalleryItem>(0)); // deselect all
						base.Context.DataSource.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.Context.DesktopWindow);
			}
		}

		public void AddNew()
		{
			if (!IsNotImageItemGallery)
				return;

			try
			{
				object o = base.Context.DataSource.AddNew();
				IStudyComposerItem item = (IStudyComposerItem) o;
				item.UpdateIcon();
				base.Context.DataSource.Add(o);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.Context.DesktopWindow);
			}
		}

		public void ShowProperties()
		{
			try
			{
				if (base.Context.SelectedData.Items.Length > 0)
				{
					ShowProperties((IStudyComposerItem) base.Context.Selection.Items[0]);
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.Context.DesktopWindow);
			}
		}

		public void ShowProperties(IStudyComposerItem item)
		{
			if (item != null)
			{
				StudyComposerItemEditorComponent component = new StudyComposerItemEditorComponent(item);
				base.Context.DesktopWindow.ShowDialogBox(component, component.Name);
			}
		}

		#region Tool<T> Overrides and Internals

		public override void Initialize()
		{
			base.Initialize();

			base.Context.SelectionChanged += Context_SelectionChanged;
			base.Context.ItemActivated += Context_ItemActivated;
		}

		protected override void Dispose(bool disposing)
		{
			base.Context.SelectionChanged -= Context_SelectionChanged;
			base.Context.ItemActivated -= Context_ItemActivated;

			base.Dispose(disposing);
		}

		private void Context_SelectionChanged(object sender, EventArgs e)
		{
			this.AtLeastOneSelected = (base.Context.Selection.Items.Length > 0);
		}

		private void Context_ItemActivated(object sender, GalleryItemEventArgs e)
		{
			ShowProperties((IStudyComposerItem) e.Item);
		}

		#endregion

		#region Tool State Properties and Events

		private bool _atLeastOneSelected = false;
		public event EventHandler AtLeastOneSelectedChanged;

		public bool AtLeastOneSelected
		{
			get { return _atLeastOneSelected; }
			private set
			{
				if (_atLeastOneSelected != value)
				{
					_atLeastOneSelected = value;
					if (AtLeastOneSelectedChanged != null)
						AtLeastOneSelectedChanged(this, new EventArgs());
				}
			}
		}

		public bool IsNotImageItemGallery
		{
			get { return !(base.Context.DataSource is BindingListWrapper<ImageItem>); }
		}

		/// <summary>
		/// Fired when the value of <see cref="IsNotImageItemGallery"/> changes.
		/// </summary>
		/// <remarks>However, since the value of <see cref="IsNotImageItemGallery"/> NEVER changes, this event never fires. Its presence is only to
		/// allow the VisibleStateObserver attribute to have a dummy event to listen on.</remarks>
		public event EventHandler IsNotImageItemGalleryChanged;

		#endregion
	}
}