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
using System.ComponentModel;
using System.Threading;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public sealed class KeyImageInformation : IKeyObjectSelectionDocumentInformation, IDisposable
	{
		internal readonly BindingList<IClipboardItem> ClipboardItems;

		internal KeyImageInformation()
		{
			Description = string.Empty;
			SeriesDescription = SR.DefaultKeyObjectSelectionSeriesDescription;
			DocumentTitle = KeyObjectSelectionDocumentTitleContextGroup.OfInterest;
			Author = GetUserName();
			ClipboardItems = new BindingList<IClipboardItem>();
		}

		public KeyObjectSelectionDocumentTitle DocumentTitle { get; set; }

		public string Author { get; set; }

		public string Description { get; set; }

		public string SeriesDescription { get; set; }

		private static string GetUserName()
		{
			var p = Thread.CurrentPrincipal;
			if (p == null || string.IsNullOrEmpty(p.Identity.Name))
				return Environment.UserName;
			return p.Identity.Name;
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			foreach (IClipboardItem item in ClipboardItems)
				((IDisposable) item).Dispose();

			ClipboardItems.Clear();
		}

		#endregion
	}
}