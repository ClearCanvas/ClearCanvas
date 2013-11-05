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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[MenuAction("previous", "global-menus/MenuTools/MenuStandard/MenuPreviousDisplaySet", "PreviousDisplaySet")]
	[ButtonAction("previous", "global-toolbars/ToolbarStandard/ToolbarPreviousDisplaySet", "PreviousDisplaySet", KeyStroke = XKeys.E)]
	[Tooltip("previous", "TooltipPreviousDisplaySet")]
	[IconSet("previous", "Icons.PreviousDisplaySetToolSmall.png", "Icons.PreviousDisplaySetToolMedium.png", "Icons.PreviousDisplaySetToolLarge.png")]
	[GroupHint("previous", "Tools.Navigation.DisplaySets.Previous")]
	//
	[MenuAction("next", "global-menus/MenuTools/MenuStandard/MenuNextDisplaySet", "NextDisplaySet")]
	[ButtonAction("next", "global-toolbars/ToolbarStandard/ToolbarNextDisplaySet", "NextDisplaySet", KeyStroke = XKeys.N)]
	[Tooltip("next", "TooltipNextDisplaySet")]
	[IconSet("next", "Icons.NextDisplaySetToolSmall.png", "Icons.NextDisplaySetToolMedium.png", "Icons.NextDisplaySetToolLarge.png")]
	[GroupHint("next", "Tools.Navigation.DisplaySets.Next")]
	//
	[EnabledStateObserver("next", "Enabled", "EnabledChanged")]
	[EnabledStateObserver("previous", "Enabled", "EnabledChanged")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class DisplaySetNavigationTool : Tool<IImageViewerToolContext>
	{
		// NOTE: this is purposely *not* derived from ImageViewerTool because that class sets Enabled differently than we want,
		// and we would have to override the methods and do nothing in order for it to work properly, which is a bit hacky.

		private bool _enabled = true;
		private event EventHandler _enabledChanged;

		public DisplaySetNavigationTool() {}

		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		private void OnStudyLoaded(object sender, StudyLoadedEventArgs e)
		{
			UpdateEnabled();
		}

		private void OnImageLoaded(object sender, ItemEventArgs<Sop> e)
		{
			UpdateEnabled();
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			UpdateEnabled();
		}

		private void OnImageBoxDrawing(object sender, ImageBoxDrawingEventArgs e)
		{
			UpdateEnabled();
		}

		private IDisplaySet GetSourceDisplaySet()
		{
			IImageBox imageBox = base.Context.Viewer.SelectedImageBox;
			if (imageBox == null)
				return null;

			IDisplaySet currentDisplaySet = imageBox.DisplaySet;

			if (currentDisplaySet == null)
				return null;

			if (currentDisplaySet.ParentImageSet == null)
			{
				// if the display set doesn't have a parent image set, fall back to using the logical workspace and the UID fo the display set
				// this situation usually arises from dynamically generated alternate views of a display set which is part of an image set
				return currentDisplaySet.ImageViewer != null ? currentDisplaySet.ImageViewer.LogicalWorkspace.ImageSets.SelectMany(ims => ims.DisplaySets).FirstOrDefault(ds => ds.Uid == currentDisplaySet.Uid) : null;
			}

			return CollectionUtils.SelectFirst(currentDisplaySet.ParentImageSet.DisplaySets, displaySet => displaySet.Uid == currentDisplaySet.Uid);
		}

		private void UpdateEnabled()
		{
			IImageBox imageBox = base.Context.Viewer.SelectedImageBox;
			if (imageBox == null || imageBox.DisplaySetLocked)
			{
				Enabled = false;
			}
			else
			{
				IDisplaySet sourceDisplaySet = GetSourceDisplaySet();
				Enabled = sourceDisplaySet != null && sourceDisplaySet.ParentImageSet.DisplaySets.Count > 1;
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			UpdateEnabled();

			base.Context.Viewer.EventBroker.ImageLoaded += OnImageLoaded;
			base.Context.Viewer.EventBroker.StudyLoaded += OnStudyLoaded;
			base.Context.Viewer.EventBroker.ImageBoxDrawing += OnImageBoxDrawing;
			base.Context.Viewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;
		}

		protected override void Dispose(bool disposing)
		{
			base.Context.Viewer.EventBroker.ImageLoaded -= OnImageLoaded;
			base.Context.Viewer.EventBroker.StudyLoaded -= OnStudyLoaded;
			base.Context.Viewer.EventBroker.ImageBoxDrawing -= OnImageBoxDrawing;
			base.Context.Viewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;

			base.Dispose(disposing);
		}

		public void NextDisplaySet()
		{
			AdvanceDisplaySet(+1);
		}

		public void PreviousDisplaySet()
		{
			AdvanceDisplaySet(-1);
		}

		public void AdvanceDisplaySet(int direction)
		{
			if (!Enabled)
				return;

			IDisplaySet sourceDisplaySet = GetSourceDisplaySet();
			if (sourceDisplaySet == null)
				return;

			IImageBox imageBox = base.Context.Viewer.SelectedImageBox;
			IImageSet parentImageSet = sourceDisplaySet.ParentImageSet;

			int sourceDisplaySetIndex = parentImageSet.DisplaySets.IndexOf(sourceDisplaySet);
			sourceDisplaySetIndex += direction;

			if (sourceDisplaySetIndex < 0)
				sourceDisplaySetIndex = parentImageSet.DisplaySets.Count - 1;
			else if (sourceDisplaySetIndex >= parentImageSet.DisplaySets.Count)
				sourceDisplaySetIndex = 0;

			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(imageBox);
			memorableCommand.BeginState = imageBox.CreateMemento();

			imageBox.DisplaySet = parentImageSet.DisplaySets[sourceDisplaySetIndex].CreateFreshCopy();
			imageBox.Draw();

			memorableCommand.EndState = imageBox.CreateMemento();

			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(imageBox);
			historyCommand.Enqueue(memorableCommand);
			base.Context.Viewer.CommandHistory.AddCommand(historyCommand);
		}
	}
}