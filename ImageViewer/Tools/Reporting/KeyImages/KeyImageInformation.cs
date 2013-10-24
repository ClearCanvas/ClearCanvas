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
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public sealed class KeyImageInformation : IKeyObjectSelectionDocumentInformation, IDisposable
	{
		private readonly BindingList<IClipboardItem> _clipboardItems;
		private readonly string _name;

		private KeyObjectSelectionDocumentTitle _documentTitle;
		private string _author;
		private string _description;
		private string _seriesDescription;

		private bool _hasChanges = false;

		internal KeyImageInformation()
		{
			_clipboardItems = new BindingList<IClipboardItem>();
			_clipboardItems.ListChanged += OnClipboardItemsListChanged;

			_description = string.Empty;
			_seriesDescription = SR.DefaultKeyObjectSelectionSeriesDescription;
			_documentTitle = KeyObjectSelectionDocumentTitleContextGroup.OfInterest;
			_author = GetUserName();
			_name = SR.LabelNewKeyImageSelection;
		}

		public KeyImageInformation(StudyTree studyTree, Sop keyObjectSelectionDocument)
		{
			Platform.CheckTrue(keyObjectSelectionDocument.SopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid, "SOP Class must be Key Object Selection Document Storage");

			_clipboardItems = new BindingList<IClipboardItem>();
			var factory = new PresentationImageFactory(studyTree);
			foreach (var image in factory.CreateImages(keyObjectSelectionDocument))
				_clipboardItems.Add(ClipboardComponent.CreatePresentationImageItem(image, true));

			// subscribe after having pre-populated the contents!
			_clipboardItems.ListChanged += OnClipboardItemsListChanged;

			var koDeserializer = new KeyImageDeserializer(keyObjectSelectionDocument);
			var description = koDeserializer.DeserializeDescriptions().OfType<KeyObjectDescriptionContentItem>().FirstOrDefault();
			var author = koDeserializer.DeserializeObserverContexts().OfType<PersonObserverContextContentItem>().FirstOrDefault();

			_author = author != null ? author.PersonObserverName : string.Empty;
			_description = description != null ? description.Description : string.Empty;
			_documentTitle = koDeserializer.DocumentTitle ?? KeyObjectSelectionDocumentTitleContextGroup.OfInterest;
			_seriesDescription = keyObjectSelectionDocument.SeriesDescription;
			_name = string.Format(SR.FormatOriginalKeyImageSelection, keyObjectSelectionDocument.SeriesNumber, keyObjectSelectionDocument.SeriesDescription);
		}

		public KeyObjectSelectionDocumentTitle DocumentTitle
		{
			get { return _documentTitle; }
			set
			{
				if (_documentTitle == value) return;
				_documentTitle = value;
				FlagChanges();
			}
		}

		public string Author
		{
			get { return _author; }
			set
			{
				if (_author == value) return;
				_author = value;
				FlagChanges();
			}
		}

		public string Description
		{
			get { return _description; }
			set
			{
				if (_description == value) return;
				_description = value;
				FlagChanges();
			}
		}

		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set
			{
				if (_seriesDescription == value) return;
				_seriesDescription = value;
				FlagChanges();
			}
		}

		public BindingList<IClipboardItem> ClipboardItems
		{
			get { return _clipboardItems; }
		}

		public bool HasChanges
		{
			get { return _hasChanges; }
		}

		public override string ToString()
		{
			return _name;
		}

		private void OnClipboardItemsListChanged(object sender, ListChangedEventArgs e)
		{
			FlagChanges();
		}

		private void FlagChanges()
		{
			_hasChanges = true;
		}

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
			foreach (IClipboardItem item in _clipboardItems)
				((IDisposable) item).Dispose();

			_clipboardItems.Clear();
		}

		#endregion
	}
}