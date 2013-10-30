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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	internal static class KeyImageItem
	{
		public static IPresentationImage BeginEditKeyImage(this IClipboardItem item, KeyImageInformation parentContext)
		{
			var image = item.Item as IDicomPresentationImage;
			if (image != null)
			{
				var editableImage = image.Clone();
				var metadata = editableImage.ExtensionData.GetOrCreate<KeyPresentationImageMetaData>();
				metadata.Guid = item.GetGuid();
				metadata.ParentContext = parentContext;
				return editableImage;
			}
			return null;
		}

		public static bool EndEditKeyImage(this IPresentationImage image, out KeyImageInformation context)
		{
			context = null;
			var metadata = image.ExtensionData.Get<KeyPresentationImageMetaData>();
			if (metadata != null)
			{
				context = metadata.ParentContext;
				var clipboardItems = metadata.ParentContext.ClipboardItems;
				var item = clipboardItems.Select((k, i) => new {Guid = k.GetGuid(), Index = i}).FirstOrDefault(x => x.Guid == metadata.Guid);
				if (item != null)
				{
					var keyImageItem = ClipboardComponent.CreatePresentationImageItem(image, false);
					keyImageItem.SetHasChanges(true);
					keyImageItem.SetGuid(item.Guid);
					clipboardItems[item.Index] = keyImageItem;
				}
				return true;
			}
			return false;
		}

		public static bool IsEdittingKeyImage(this IPresentationImage image)
		{
			return image.ExtensionData.Get<KeyPresentationImageMetaData>() != null;
		}

		public static bool IsKeyImage(this IPresentationImage image)
		{
			return FindParentKeyObjectDocument(image) != null;
		}

		public static KeyObjectSelectionDocumentIod FindParentKeyObjectDocument(this IPresentationImage image)
		{
			if (image != null && image.ParentDisplaySet != null)
			{
				var viewer = image.ImageViewer;
				if (viewer != null)
				{
					var descriptor = image.ParentDisplaySet.Descriptor as IDicomDisplaySetDescriptor;
					if (descriptor != null && descriptor.SourceSeries != null)
					{
						var uid = descriptor.SourceSeries.SeriesInstanceUid;
						if (!string.IsNullOrEmpty(uid))
						{
							var keyObjectSeries = viewer.StudyTree.GetSeries(uid);
							if (keyObjectSeries != null)
							{
								var keyObjectSop = keyObjectSeries.Sops.FirstOrDefault();
								if (keyObjectSop != null && keyObjectSop.SopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid)
								{
									return new KeyObjectSelectionDocumentIod(keyObjectSop);
								}
							}
						}
					}
				}
			}
			return null;
		}

		public static bool HasChanges(this IClipboardItem item)
		{
			return item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().HasChanges;
		}

		public static void SetHasChanges(this IClipboardItem item, bool value)
		{
			item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().HasChanges = value;
		}

		public static Guid GetGuid(this IClipboardItem item)
		{
			return item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().Guid;
		}

		public static void SetGuid(this IClipboardItem item, Guid guid)
		{
			var metadata = item.ExtensionData.GetOrCreate<KeyImageItemMetaData>();
			metadata.Guid = guid;
		}

		private class KeyImageItemMetaData
		{
			public bool HasChanges { get; set; }
			public Guid Guid { get; set; }

			public KeyImageItemMetaData()
			{
				// assume that newly created items have changes by default
				HasChanges = true;
			}
		}

		private class KeyPresentationImageMetaData
		{
			public Guid Guid { get; set; }
			public KeyImageInformation ParentContext { get; set; }
		}
	}
}