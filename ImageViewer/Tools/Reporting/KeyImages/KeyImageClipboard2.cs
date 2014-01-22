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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public partial class KeyImageClipboard : IDisposable, IKeyImageClipboard, IKeyObjectSelectionDocumentInformation, INotifyPropertyChanged
	{
		private readonly StudyTree _studyTree;
		private readonly List<KeyImageInformation> _availableContexts;
		private KeyImageInformation _currentContext;

		public KeyImageClipboard(StudyTree studyTree)
		{
			_studyTree = studyTree;

			_currentContext = new KeyImageInformation();
			_availableContexts = new List<KeyImageInformation> {_currentContext};
			_availableContexts.AddRange(_studyTree.Studies
			                            	.SelectMany(s => s.Series)
			                            	.SelectMany(s => s.Sops)
			                            	.Where(s => s.SopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid)
			                            	.Select(s => new KeyImageInformation(studyTree, s))
			                            	.OrderByDescending(s => s.ContentDateTime)
			                            	.ThenBy(s => s.SeriesNumber));
		}

		public IList<KeyImageInformation> AvailableContexts
		{
			get { return _availableContexts; }
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
					NotifyPropertyChanged("CurrentContext");
					NotifyPropertyChanged("DocumentTitle");
					NotifyPropertyChanged("Author");
					NotifyPropertyChanged("Description");
					NotifyPropertyChanged("SeriesDescription");
					NotifyDocumentInformationChanged();
				}
			}
		}

		public event EventHandler CurrentContextChanged;

		public KeyObjectSelectionDocumentTitle DocumentTitle
		{
			get { return CurrentContext.DocumentTitle; }
			set
			{
				if (CurrentContext.DocumentTitle != value)
				{
					CurrentContext.DocumentTitle = value;
					NotifyPropertyChanged("DocumentTitle");
					NotifyDocumentInformationChanged();
				}
			}
		}

		public string Author
		{
			get { return CurrentContext.Author; }
			set
			{
				if (CurrentContext.Author != value)
				{
					CurrentContext.Author = value;
					NotifyPropertyChanged("Author");
					NotifyDocumentInformationChanged();
				}
			}
		}

		public string Description
		{
			get { return CurrentContext.Description; }
			set
			{
				if (CurrentContext.Description != value)
				{
					CurrentContext.Description = value;
					NotifyPropertyChanged("Description");
					NotifyDocumentInformationChanged();
				}
			}
		}

		public string SeriesDescription
		{
			get { return CurrentContext.SeriesDescription; }
			set
			{
				if (CurrentContext.SeriesDescription != value)
				{
					CurrentContext.SeriesDescription = value;
					NotifyPropertyChanged("SeriesDescription");
					NotifyDocumentInformationChanged();
				}
			}
		}

		public IList<IClipboardItem> ClipboardItems
		{
			get { return CurrentContext.Items; }
		}

		public void Publish()
		{
			KeyImagePublisher.Publish(AvailableContexts.Where(c => c.HasChanges && c.Items.Count > 0));
		}

		public void Dispose()
		{
			foreach (var context in AvailableContexts)
				((IDisposable) context).Dispose();
			AvailableContexts.Clear();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		#region IKeyImageClipboard Implementation

		private event EventHandler _documentInformationChanged;

		private void NotifyDocumentInformationChanged()
		{
			EventsHelper.Fire(_documentInformationChanged, this, new EventArgs());
		}

		IKeyObjectSelectionDocumentInformation IKeyImageClipboard.DocumentInformation
		{
			get { return this; }
		}

		event EventHandler IKeyImageClipboard.DocumentInformationChanged
		{
			add { _documentInformationChanged += value; }
			remove { _documentInformationChanged -= value; }
		}

		IList<IClipboardItem> IKeyImageClipboard.Items
		{
			get { return CurrentContext.Items; }
		}

		#endregion
	}
}