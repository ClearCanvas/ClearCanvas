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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
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

		#region Key Image Serialization

		/// <summary>
		/// Creates all the SOP instances associated with the key object selection and the content presentation states.
		/// </summary>
		public IDictionary<IStudySource, List<DicomFile>> CreateSopInstances(NextSeriesNumberDelegate nextSeriesNumberDelegate = null)
		{
			if (!_hasChanges || !ClipboardItems.Any()) return new Dictionary<IStudySource, List<DicomFile>>(0);

			// the series index ensures consistent series level data because we only create one KO series and one PR series per study
			var studyIndex = new Dictionary<string, StudyInfo>();

			var framePresentationStates = new List<KeyValuePair<KeyImageReference, PresentationStateReference>>();

			// create presentation states for the images in the clipboard
			var presentationStates = new List<DicomSoftcopyPresentationState>();
			foreach (var image in ClipboardItems.Select(x => x.Item).OfType<IPresentationImage>())
			{
				if (image is KeyObjectPlaceholderImage)
				{
					var ko = (KeyObjectPlaceholderImage) image;
					framePresentationStates.Add(new KeyValuePair<KeyImageReference, PresentationStateReference>(ko.KeyImageReference, ko.PresentationStateReference));
					continue;
				}

				var provider = image as IImageSopProvider;
				if (provider == null) continue;

				StudyInfo studyInfo;
				var studyInstanceUid = provider.ImageSop.StudyInstanceUid;
				if (!studyIndex.TryGetValue(studyInstanceUid, out studyInfo))
					studyIndex.Add(studyInstanceUid, studyInfo = new StudyInfo(provider));

				var presentationState = DicomSoftcopyPresentationState.IsSupported(image)
				                        	? DicomSoftcopyPresentationState.Create
				                        	  	(image, ps =>
				                        	  	        	{
				                        	  	        		ps.PresentationSeriesInstanceUid = studyInfo.PresentationSeriesUid;
				                        	  	        		ps.PresentationSeriesNumber = studyInfo.PresentationSeriesNumber;
				                        	  	        		ps.PresentationSeriesDateTime = studyInfo.PresentationSeriesDateTime;
				                        	  	        		ps.PresentationInstanceNumber = studyInfo.GetNextPresentationInstanceNumber();
				                        	  	        		ps.SourceAETitle = provider.ImageSop.DataSource[DicomTags.SourceApplicationEntityTitle].ToString();
				                        	  	        	}) : null;
				if (presentationState != null) presentationStates.Add(presentationState);
				framePresentationStates.Add(new KeyValuePair<KeyImageReference, PresentationStateReference>(provider.Frame, presentationState));
			}

			// serialize the key image document
			var serializer = new KeyImageSerializer();
			serializer.Author = Author;
			serializer.Description = Description;
			serializer.DocumentTitle = DocumentTitle;
			serializer.SeriesDescription = SeriesDescription;
			foreach (var presentationFrame in framePresentationStates)
				serializer.AddImage(presentationFrame.Key, presentationFrame.Value);

			// collect all the SOP instances that were created (both PR and KO)
			var documents = new List<DicomFile>();
			documents.AddRange(serializer.Serialize(koSeries =>
			                                        	{
			                                        		var uid = koSeries.StudyInstanceUid;
			                                        		if (studyIndex.ContainsKey(uid))
			                                        		{
			                                        			koSeries.SeriesDateTime = studyIndex[uid].KeyObjectSeriesDateTime;
			                                        			koSeries.SeriesNumber = studyIndex[uid].KeyObjectSeriesNumber;
			                                        			koSeries.SeriesInstanceUid = studyIndex[uid].KeyObjectSeriesUid;
			                                        			return studyIndex[uid].DataSource;
			                                        		}
			                                        		return null;
			                                        	}
			                   	));
			documents.AddRange(presentationStates.Select(ps => ps.DicomFile));

			// return the created instances grouped by study (and thus study origin/source)
			return documents.GroupBy(f => (IStudySource) studyIndex[f.DataSet[DicomTags.StudyInstanceUid].ToString()]).ToDictionary(g => g.Key, g => g.ToList());
		}

		private class StudyInfo : IStudySource
		{
			public readonly string PresentationSeriesUid;
			public readonly int PresentationSeriesNumber;
			public readonly DateTime PresentationSeriesDateTime;

			public readonly string KeyObjectSeriesUid;
			public readonly int KeyObjectSeriesNumber;
			public readonly DateTime KeyObjectSeriesDateTime;

			private readonly IImageSopProvider _provider;
			private readonly string _studyInstanceUid;
			private readonly IDicomServiceNode _originServer;
			private readonly IDicomServiceNode _sourceServer;
			private int _presentationNextInstanceNumber = 1;

			public StudyInfo(IImageSopProvider provider, NextSeriesNumberDelegate nextSeriesNumberDelegate = null)
			{
				_provider = provider;
				_studyInstanceUid = provider.Sop.StudyInstanceUid;
				_originServer = ServerDirectory.GetRemoteServersByAETitle(provider.Sop[DicomTags.SourceApplicationEntityTitle].ToString()).FirstOrDefault();
				_sourceServer = provider.Sop.DataSource.Server;

				KeyObjectSeriesUid = DicomUid.GenerateUid().UID;
				KeyObjectSeriesDateTime = Platform.Time;
				PresentationSeriesUid = DicomUid.GenerateUid().UID;
				PresentationSeriesDateTime = Platform.Time;

				if (nextSeriesNumberDelegate == null)
				{
					KeyObjectSeriesNumber = KeyImagePublisher.GetMaxSeriesNumber(provider.Frame) + 1;
					PresentationSeriesNumber = KeyObjectSeriesNumber + 1;
				}
				else
				{
					KeyObjectSeriesNumber = nextSeriesNumberDelegate.Invoke(provider.Frame);
					PresentationSeriesNumber = nextSeriesNumberDelegate.Invoke(provider.Frame);
				}
			}

			public IDicomAttributeProvider DataSource
			{
				get { return _provider.Sop.DataSource; }
			}

			public string StudyInstanceUid
			{
				get { return _studyInstanceUid; }
			}

			public IDicomServiceNode OriginServer
			{
				get { return _originServer; }
			}

			public IDicomServiceNode SourceServer
			{
				get { return _sourceServer; }
			}

			public override int GetHashCode()
			{
				return _studyInstanceUid.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				return obj is IStudySource && ((IStudySource) obj).StudyInstanceUid == _studyInstanceUid;
			}

			public override string ToString()
			{
				return _studyInstanceUid;
			}

			public int GetNextPresentationInstanceNumber()
			{
				return _presentationNextInstanceNumber++;
			}
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			foreach (IClipboardItem item in _clipboardItems)
				((IDisposable) item).Dispose();

			_clipboardItems.Clear();
		}

		#endregion
	}

	public delegate int NextSeriesNumberDelegate(Frame frame);

	public interface IStudySource
	{
		string StudyInstanceUid { get; }
		IDicomServiceNode OriginServer { get; }
		IDicomServiceNode SourceServer { get; }
	}
}