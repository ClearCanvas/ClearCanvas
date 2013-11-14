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
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Annotations.Utilities;
using ClearCanvas.ImageViewer.Clipboard.ImageExport;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// Represents an image and display set clipboard.
	/// </summary>
	/// <remarks>
	/// The clipboard can be thought of as a "holding area" for images the user has deemed to
	/// be of interest. Clipboard tools can then operate on those images.
	/// </remarks>
	public class Clipboard
	{
		/// <summary>
		/// Constant defining the action model site for the toolbar on the <see cref="ClipboardComponent"/>.
		/// </summary>
		public const string ClipboardSiteToolbar = "clipboard-toolbar";

		/// <summary>
		/// Constant defining the action model site for the context menu of the <see cref="ClipboardComponent"/>.
		/// </summary>
		public const string ClipboardSiteMenu = "clipboard-contextmenu";

		[ThreadStatic]
		private static Clipboard _default;

		/// <summary>
		/// Gets the global <see cref="Clipboard"/> instance.
		/// </summary>
		/// <remarks>
		/// The clipboard provided by this property is thread static (that is, each thread has its own independent clipboard instance).
		/// However, in practice, the clipboard should only be accessed from the UI thread.
		/// </remarks>
		public static Clipboard Default
		{
			get { return _default ?? (_default = new Clipboard()); }
		}

		/// <summary>
		/// Adds an <see cref="IPresentationImage"/> to the clipboard.
		/// </summary>
		/// <param name="image"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IPresentationImage"/> is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IPresentationImage"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");

			var clipboard = Default;
			clipboard._items.Add(clipboard.CreatePresentationImageItem(image));
		}

		/// <summary>
		/// Adds an <see cref="IDisplaySet"/> to the clipboard.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IDisplaySet"/> is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IDisplaySet"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IDisplaySet displaySet)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");

			var clipboard = Default;
			clipboard._items.Add(clipboard.CreateDisplaySetItem(displaySet));
		}

		/// <summary>
		/// Adds an <see cref="IDisplaySet"/> to the clipboard.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <param name="selectionStrategy"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IPresentationImage"/>s
		/// (as determined by the <paramref name="selectionStrategy"/>) is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IPresentationImage"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IDisplaySet displaySet, IImageSelectionStrategy selectionStrategy)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");
			Platform.CheckForNullReference(selectionStrategy, "selectionStrategy");

			var clipboard = Default;
			clipboard._items.Add(clipboard.CreateDisplaySetItem(displaySet, selectionStrategy));
		}

		#region Instance Implementation

		private readonly BindingList<IClipboardItem> _items;

		/// <summary>
		/// Initializes a new instance of <see cref="Clipboard"/>.
		/// </summary>
		public Clipboard() : this(null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="Clipboard"/>.
		/// </summary>
		/// <param name="items"></param>
		public Clipboard(IEnumerable<IClipboardItem> items)
		{
			_items = new BindingList<IClipboardItem>();
			if (items != null) foreach (var item in items) _items.Add(item);
			_items.ListChanged += (s, e) => OnItemsListChanged(e);
		}

		/// <summary>
		/// Gets the list of items on the clipboard.
		/// </summary>
		public BindingList<IClipboardItem> Items
		{
			get { return _items; }
		}

		/// <summary>
		/// Called when the <see cref="BindingList{T}.ListChanged"/> event is raised.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnItemsListChanged(ListChangedEventArgs e) {}

		/// <summary>
		/// Called to create an icon image for the specified contents.
		/// </summary>
		/// <param name="presentationImage"></param>
		/// <param name="clientRectangle"></param>
		/// <returns></returns>
		protected virtual Bitmap CreateIcon(IPresentationImage presentationImage, Rectangle clientRectangle)
		{
			return IconCreator.CreatePresentationImageIcon(presentationImage, clientRectangle);
		}

		/// <summary>
		/// Called to create an icon image for the specified contents.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <param name="clientRectangle"></param>
		/// <returns></returns>
		protected virtual Bitmap CreateIcon(IDisplaySet displaySet, Rectangle clientRectangle)
		{
			return IconCreator.CreateDisplaySetIcon(displaySet, clientRectangle);
		}

		/// <summary>
		/// Called to create a clipboard item representing a presentation image.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="ownReference"></param>
		/// <returns></returns>
		public virtual ClipboardItem CreatePresentationImageItem(IPresentationImage image, bool ownReference = false)
		{
			Rectangle clientRectangle = image.ClientRectangle;
			if (clientRectangle.IsEmpty) clientRectangle = new Rectangle(new Point(), image.SceneSize);

			// Must build description from the source image because the ParentDisplaySet info is lost in the cloned image.
			var name = BuildClipboardItemName(image);
			var description = BuildClipboardItemDescription(image);

			image = !ownReference ? ImageExporter.ClonePresentationImage(image) : image;
			var bmp = CreateIcon(image, clientRectangle);

			return new ClipboardItem(image, bmp, name, description, clientRectangle);
		}

		/// <summary>
		/// Called to create a clipboard item representing a display set.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <param name="selectionStrategy"></param>
		/// <returns></returns>
		public virtual ClipboardItem CreateDisplaySetItem(IDisplaySet displaySet, IImageSelectionStrategy selectionStrategy = null)
		{
			if (displaySet.PresentationImages.Count == 0)
				throw new ArgumentException("DisplaySet must have at least one image.");

			var presentationImage = displaySet.ImageBox != null
			                        && displaySet.ImageBox.SelectedTile != null
			                        && displaySet.ImageBox.SelectedTile.PresentationImage != null
			                        	? displaySet.ImageBox.SelectedTile.PresentationImage
			                        	: displaySet.PresentationImages[displaySet.PresentationImages.Count/2];

			var clientRectangle = presentationImage.ClientRectangle;
			if (clientRectangle.IsEmpty) clientRectangle = new Rectangle(new Point(), presentationImage.SceneSize);

			if (selectionStrategy == null)
			{
				if (displaySet.PresentationImages.Count == 1)
				{
					// Add as a single image.
					return CreatePresentationImageItem(displaySet.PresentationImages[0]);
				}
				else
				{
					return CreateDisplaySetItem(displaySet.Clone(), clientRectangle);
				}
			}
			else
			{
				List<IPresentationImage> images = new List<IPresentationImage>(selectionStrategy.GetImages(displaySet));
				if (images.Count == 1)
				{
					// Add as a single image.
					return CreatePresentationImageItem(images[0]);
				}
				else
				{
					string name = String.Format("{0} - {1}", selectionStrategy.Description, displaySet.Name);
					displaySet = new DisplaySet(name, displaySet.Uid) {Description = displaySet.Description, Number = displaySet.Number};
					images.ForEach(image => displaySet.PresentationImages.Add(image.Clone()));
					return CreateDisplaySetItem(displaySet, clientRectangle);
				}
			}
		}

		private ClipboardItem CreateDisplaySetItem(IDisplaySet displaySet, Rectangle clientRectangle)
		{
			var bmp = CreateIcon(displaySet, clientRectangle);
			return new ClipboardItem(displaySet, bmp, displaySet.Name, BuildClipboardItemDescription(displaySet), clientRectangle);
		}

		private static string BuildClipboardItemName(IPresentationImage image)
		{
			if (!(image is IImageSopProvider))
				return string.Empty;

			var imageSopProvider = (IImageSopProvider) image;

			// This is unlikely to happen
			if (image.ParentDisplaySet == null)
				return string.Format(SR.MessageClipboardNameSingleImage, imageSopProvider.ImageSop.SeriesDescription, imageSopProvider.ImageSop.InstanceNumber);

			// Multi-frame image, display image and frame number
			if (imageSopProvider.ImageSop.NumberOfFrames > 1)
				return string.Format(SR.MessageClipboardNameMultiframeImage, image.ParentDisplaySet.Name, imageSopProvider.ImageSop.InstanceNumber, imageSopProvider.Frame.FrameNumber);

			// Single frame image in a display set of multiple images, display image number
			if (image.ParentDisplaySet.PresentationImages.Count > 1)
				return string.Format(SR.MessageClipboardNameSingleImage, image.ParentDisplaySet.Name, imageSopProvider.ImageSop.InstanceNumber);

			// Only one image in the displayset, no need for image number
			return image.ParentDisplaySet.Name;
		}

		private static string BuildClipboardItemDescription(IPresentationImage image)
		{
			if (!(image is IImageSopProvider))
				return string.Empty;

			var imageSopProvider = (IImageSopProvider) image;
			var sop = imageSopProvider.ImageSop;

			return string.Format(SR.MessageClipboardDescription
			                     , sop.PatientId
			                     , sop.PatientsName == null ? null : sop.PatientsName.FormattedName
			                     , Format.Date(DateParser.Parse(sop.StudyDate))
			                     , sop.StudyDescription
			                     , sop.AccessionNumber
			                     , sop.Modality
			                     , TextOverlayVisibilityHelper.IsVisible(image, true) ? SR.LabelOn : SR.LabelOff);
		}

		private static string BuildClipboardItemDescription(IDisplaySet displaySet)
		{
			return displaySet.PresentationImages.Count == 0
			       	? string.Empty
			       	: BuildClipboardItemDescription(displaySet.PresentationImages[0]);
		}

		#endregion
	}
}