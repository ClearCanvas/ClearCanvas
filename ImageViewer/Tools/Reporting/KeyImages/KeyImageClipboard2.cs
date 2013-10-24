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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public partial class KeyImageClipboard : IDisposable, IKeyObjectSelectionDocumentInformation
	{
		private KeyImageInformation _currentContext;

		public KeyImageClipboard()
		{
			_currentContext = new KeyImageInformation();
		}

		public KeyImageInformation CurrentContext
		{
			get { return _currentContext; }
			set
			{
				Platform.CheckForNullReference(value, "CurrentContext");
				if (_currentContext != value)
				{
					_currentContext = value;
					EventsHelper.Fire(CurrentContextChanged, this, new EventArgs());
				}
			}
		}

		public event EventHandler CurrentContextChanged;

		public IList<IClipboardItem> ClipboardItems
		{
			get { return CurrentContext.ClipboardItems; }
		}

		public void Publish()
		{
			var publisher = new KeyImagePublisher(_currentContext);
			publisher.Publish();
		}

		public void Dispose()
		{
			((IDisposable) _currentContext).Dispose();
		}

		public KeyObjectSelectionDocumentTitle DocumentTitle { get; set; }

		public string Author { get; set; }

		public string Description { get; set; }

		public string SeriesDescription { get; set; }
	}
}