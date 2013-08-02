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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public class SelectOverlaysActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	[AssociateView(typeof (SelectOverlaysActionViewExtensionPoint))]
	public sealed class SelectOverlaysAction : Action
	{
		public class Item
		{
			public Item(IImageOverlay source)
			{
				Name = source.Name;
				DisplayName = source.DisplayName;
				IsSelected = source.IsSelected;
				IconSet = source.IconSet;
				ResourceResolver = source.ResourceResolver ?? new ResourceResolver(GetType(), false);
			}

			public IconSet IconSet { get; private set; }
			public string Name { get; private set; }
			public string DisplayName { get; private set; }
			public IResourceResolver ResourceResolver { get; private set; }
			public bool IsSelected { get; set; }
		}

		private readonly ShowHideOverlaysTool _parent;

		public SelectOverlaysAction(ShowHideOverlaysTool parent, string actionID, ActionPath path, IResourceResolver resourceResolver)
			: base(actionID, path, resourceResolver)
		{
			_parent = parent;
			var image = GetImage(_parent.Viewer.SelectedImageBox);

			Persistent = false;

			//ApplicableModality = image.GetModality();
			Enabled = image != null && _parent.SelectedOverlaysVisible;

			var overlays = image.GetOverlays();
			Items = overlays.Select(o => new Item(o)).ToList();
		}

		public bool Enabled { get; private set; }
		public IList<Item> Items { get; private set; }

		//private string ApplicableModality { get; set; }

		public void Apply()
		{
			//Apply to all images in current display set
			Apply(_parent.Viewer.SelectedImageBox);
		}

		public void ApplyEverywhere()
		{
			//Apply to all images in all visible display sets
			foreach (var imageBox in _parent.Viewer.PhysicalWorkspace.ImageBoxes)
				Apply(imageBox);
		}

		private void Apply(IImageBox imageBox)
		{
			if (imageBox == null || imageBox.DisplaySet == null)
				return;

			//foreach (var presentationImage in imageBox.DisplaySet.PresentationImages.Where(i => i.GetModality() == ApplicableModality))

			foreach (var presentationImage in imageBox.DisplaySet.PresentationImages)
			{
				var imageOverlays = presentationImage.GetOverlays();
				foreach (var imageOverlay in imageOverlays)
				{
					var source = Items.FirstOrDefault(i => i.Name == imageOverlay.Name);
					if (source != null)
						imageOverlay.IsSelected = source.IsSelected;

					if (_parent.SelectedOverlaysVisible)
						imageOverlay.ShowIfSelected();
					else
						imageOverlay.Hide();
				}
			}

			imageBox.Draw();
		}

		private static IPresentationImage GetImage(IImageBox imageBox)
		{
			if (imageBox == null)
				return null;

			IPresentationImage image;
			if (imageBox.SelectedTile != null && imageBox.SelectedTile.PresentationImage != null)
			{
				image = imageBox.SelectedTile.PresentationImage;
			}
			else if (imageBox.Tiles.Count > 0 && imageBox.Tiles[0].PresentationImage != null)
			{
				image = imageBox.Tiles[0].PresentationImage;
			}
			else if (imageBox.DisplaySet != null && imageBox.DisplaySet.PresentationImages.Count > 0)
				image = imageBox.DisplaySet.PresentationImages[0];
			else
			{
				image = null;
			}

			return image;
		}
	}
}