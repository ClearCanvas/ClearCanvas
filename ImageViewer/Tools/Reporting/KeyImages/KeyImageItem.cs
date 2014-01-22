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
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	internal static class KeyImageItem
	{
		public static bool UpdateKeyImage(this IPresentationImage image, KeyImageInformation context)
		{
			if (context != null)
			{
				var presentationStateUid = GetPresentationStateSopInstanceUid(image);

				var clipboardItems = context.Items;
				var result = clipboardItems.Where(IsSerialized).Select((k, i) => new {k.Item, Index = i, MetaData = k.ExtensionData.GetOrCreate<KeyImageItemMetaData>()}).FirstOrDefault(c => GetPresentationStateSopInstanceUid((c.Item as IPresentationImage)) == presentationStateUid);
				if (result != null)
				{
					var keyImageItem = context.CreateKeyImageItem(image, hasChanges : true);

					var metadata = ((IClipboardItem) keyImageItem).ExtensionData.GetOrCreate<KeyImageItemMetaData>();
					metadata.Changes = true;
					metadata.Guid = result.MetaData.Guid;
					metadata.KoSopInstanceUid = result.MetaData.KoSopInstanceUid;
					metadata.OriginalItem = result.MetaData.OriginalItem ?? clipboardItems[result.Index];

					clipboardItems[result.Index] = keyImageItem;
					return true;
				}
			}
			return false;
		}

		public static bool RevertKeyImage(this IClipboardItem item, KeyImageInformation context)
		{
			var guid = GetGuid(item);
			if (context != null && IsSerialized(item))
			{
				var clipboardItems = context.Items;
				var result = clipboardItems.Where(k => IsSerialized(k) && GetGuid(k) == guid).Select((k, i) => new {k.Item, Index = i, MetaData = k.ExtensionData.GetOrCreate<KeyImageItemMetaData>()}).FirstOrDefault();
				if (result != null)
				{
					clipboardItems[result.Index] = result.MetaData.OriginalItem;
					return true;
				}
			}
			return false;
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

		public static string GetPresentationStateSopInstanceUid(this IPresentationImage image)
		{
			var dicomImage = image as IDicomPresentationImage;
			if (dicomImage != null && dicomImage.PresentationState is DicomSoftcopyPresentationState)
			{
				var presentationState = (DicomSoftcopyPresentationState) dicomImage.PresentationState;
				return presentationState.PresentationSopInstanceUid;
			}
			return null;
		}

		/// <summary>
		/// Gets a value indicating whether or not the item has unserialized changes.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool HasChanges(this IClipboardItem item)
		{
			return item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().Changes;
		}

		/// <summary>
		/// Flags an item as having unserialized changes.
		/// </summary>
		/// <param name="item"></param>
		public static void FlagHasChanges(this IClipboardItem item)
		{
			item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().Changes = true;
		}

		/// <summary>
		/// Assigns source identification details to a serialized item (i.e. that came from a KO document).
		/// </summary>
		/// <param name="item"></param>
		/// <param name="guid"></param>
		/// <param name="selectionDocumentInstanceUid"></param>
		public static void AssignSourceInfo(this IClipboardItem item, Guid guid, string selectionDocumentInstanceUid)
		{
			var metadata = item.ExtensionData.GetOrCreate<KeyImageItemMetaData>();
			metadata.Changes = false;
			metadata.Guid = guid;
			metadata.KoSopInstanceUid = selectionDocumentInstanceUid;
			metadata.OriginalItem = item;
		}

		/// <summary>
		/// Gets whether or not the item has previously been serialized to a KO document.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool IsSerialized(this IClipboardItem item)
		{
			return item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().Guid.HasValue;
		}

		/// <summary>
		/// Gets the identification Guid of a serialized item (i.e. that came from a KO document).
		/// Throws exception if item is not previously serialized.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static Guid GetGuid(this IClipboardItem item)
		{
			var guid = item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().Guid;
			if (!guid.HasValue)
				throw new InvalidOperationException("Item has not been serialized yet");
			return guid.Value;
		}

		/// <summary>
		/// Gets the selection document instance UID of a serialized item (i.e. the SOP instance UID of the source KO document).
		/// Throws exception if item is not previously serialized.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static string GetSelectionDocumentInstanceUid(this IClipboardItem item)
		{
			var sopInstanceUid = item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().KoSopInstanceUid;
			if (sopInstanceUid == null)
				throw new InvalidOperationException("Item has not been serialized yet");
			return sopInstanceUid;
		}

		private class KeyImageItemMetaData
		{
			public bool Changes { get; set; }
			public string KoSopInstanceUid { get; set; }
			public Guid? Guid { get; set; }
			public IClipboardItem OriginalItem { get; set; }

			public KeyImageItemMetaData()
			{
				// assume that newly created items have changes by default
				Changes = true;
			}
		}
	}
}