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

using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	internal static class KeyImageItem
	{
		public static bool HasChanges(this IClipboardItem item)
		{
			return item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().HasChanges;
		}

		public static void SetHasChanges(this IClipboardItem item, bool value)
		{
			item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().HasChanges = value;
		}

		public static string GetSopInstanceUid(this IClipboardItem item)
		{
			return item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().SopInstanceUid;
		}

		public static string GetPresentationStateInstanceUid(this IClipboardItem item)
		{
			return item.ExtensionData.GetOrCreate<KeyImageItemMetaData>().PresentationStateInstanceUid;
		}

		public static void SetMetaData(this IClipboardItem item, string sopInstanceUid, string presentationStateInstanceUid)
		{
			var metadata = item.ExtensionData.GetOrCreate<KeyImageItemMetaData>();
			metadata.SopInstanceUid = sopInstanceUid;
			metadata.PresentationStateInstanceUid = presentationStateInstanceUid;
		}

		private class KeyImageItemMetaData
		{
			public bool HasChanges { get; set; }
			public string SopInstanceUid { get; set; }
			public string PresentationStateInstanceUid { get; set; }

			public KeyImageItemMetaData()
			{
				// assume that newly created items have changes by default
				HasChanges = true;
			}
		}
	}
}