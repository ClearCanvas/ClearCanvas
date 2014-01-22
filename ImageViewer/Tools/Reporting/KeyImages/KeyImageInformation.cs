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
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public sealed class KeyImageInformation : Clipboard.Clipboard, IKeyObjectSelectionDocumentInformation, IDisposable
	{
		private readonly string _name;

		private readonly string _parentStudyInstanceUid;
		private readonly string _documentInstanceUid;
		private KeyObjectSelectionDocumentTitle _documentTitle;
		private string _author;
		private string _description;
		private string _seriesDescription;
		private int? _seriesNumber;
		private DateTime? _contentDateTime;

		private bool _hasChanges = false;
		private bool _creatingItemHasChanges = false;

		internal KeyImageInformation()
		{
			_parentStudyInstanceUid = string.Empty;
			_documentInstanceUid = string.Empty;
			_description = string.Empty;
			_seriesDescription = SR.DefaultKeyObjectSelectionSeriesDescription;
			_documentTitle = KeyObjectSelectionDocumentTitleContextGroup.OfInterest;
			_author = GetUserName();
			_name = SR.LabelNewKeyImageSelection;
			_seriesNumber = null;
			_contentDateTime = null;
			if (!string.IsNullOrEmpty(_author))
				_seriesDescription = string.Format("{0} ({1})", _seriesDescription, _author);
		}

		private static IEnumerable<IClipboardItem> CreateClipboardItems(StudyTree studyTree, Sop keyObjectSelectionDocument)
		{
			Platform.CheckTrue(keyObjectSelectionDocument.SopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid, "SOP Class must be Key Object Selection Document Storage");

			var dummyContext = new KeyImageInformation(); // just need an instance of creating items
			var factory = new PresentationImageFactory(studyTree);
			foreach (var image in factory.CreateImages(keyObjectSelectionDocument))
			{
				// set the deserialize interactive flag on the presentation state
				var dicomPresentationImage = image as IDicomPresentationImage;
				if (dicomPresentationImage != null)
				{
					var presentationState = dicomPresentationImage.PresentationState as DicomSoftcopyPresentationState;
					if (presentationState != null) presentationState.DeserializeInteractiveAnnotations = true;
				}

				var item = dummyContext.CreateKeyImageItem(image, true);
				item.AssignSourceInfo(Guid.NewGuid(), keyObjectSelectionDocument.SopInstanceUid);
				yield return item;
			}
		}

		public KeyImageInformation(StudyTree studyTree, Sop keyObjectSelectionDocument)
			: base(CreateClipboardItems(studyTree, keyObjectSelectionDocument))
		{
			var koDeserializer = new KeyImageDeserializer(keyObjectSelectionDocument);
			var description = koDeserializer.DeserializeDescriptions().OfType<KeyObjectDescriptionContentItem>().FirstOrDefault();
			var author = koDeserializer.DeserializeObserverContexts().OfType<PersonObserverContextContentItem>().FirstOrDefault();

			_parentStudyInstanceUid = keyObjectSelectionDocument.StudyInstanceUid;
			_documentInstanceUid = keyObjectSelectionDocument.SopInstanceUid;
			_author = author != null ? author.PersonObserverName : string.Empty;
			_description = description != null ? description.Description : string.Empty;
			_documentTitle = koDeserializer.DocumentTitle ?? KeyObjectSelectionDocumentTitleContextGroup.OfInterest;
			_seriesDescription = keyObjectSelectionDocument.SeriesDescription;
			_seriesNumber = keyObjectSelectionDocument.SeriesNumber;
			_contentDateTime = DateTimeParser.ParseDateAndTime(null, keyObjectSelectionDocument.ContentDate, keyObjectSelectionDocument.ContentTime);
			_name = string.Format(SR.FormatOriginalKeyImageSelection, keyObjectSelectionDocument.SeriesNumber, keyObjectSelectionDocument.SeriesDescription, Format.DateTime(_contentDateTime));
		}

		public string DocumentInstanceUid
		{
			get { return _documentInstanceUid; }
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

		public int? SeriesNumber
		{
			get { return _seriesNumber; }
		}

		public DateTime? ContentDateTime
		{
			get { return _contentDateTime; }
		}

		public bool HasChanges
		{
			get { return _hasChanges || Items.Any(k => k.HasChanges()); }
		}

		public override string ToString()
		{
			return _name;
		}

		protected override void OnItemsListChanged(ListChangedEventArgs e)
		{
			// ignore in-place item change events - they are evaluated when HasChanges is called
			if (e.ListChangedType != ListChangedType.ItemChanged)
				FlagChanges();
		}

		private void FlagChanges()
		{
			_hasChanges = true;
		}

		protected override Bitmap CreateIcon(IPresentationImage presentationImage, Rectangle clientRectangle)
		{
			var icon = base.CreateIcon(presentationImage, clientRectangle);
			if (_creatingItemHasChanges)
			{
				using (var g = System.Drawing.Graphics.FromImage(icon))
				using (var f = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold, GraphicsUnit.Pixel))
					g.DrawString("*", f, Brushes.Yellow, new PointF(0, 0));
			}
			return icon;
		}

		public ClipboardItem CreateKeyImageItem(IPresentationImage image, bool ownReference = false, bool hasChanges = false)
		{
			_creatingItemHasChanges = hasChanges;
			try
			{
				return base.CreatePresentationImageItem(image, ownReference);
			}
			finally
			{
				_creatingItemHasChanges = false;
			}
		}

		[Obsolete("Use CreateKeyImageItem", true)]
		public override ClipboardItem CreatePresentationImageItem(IPresentationImage image, bool ownReference = false)
		{
			return CreateKeyImageItem(image, ownReference);
		}

		[Obsolete("Use CreateKeyImageItem", true)]
		public override ClipboardItem CreateDisplaySetItem(IDisplaySet displaySet, IImageSelectionStrategy selectionStrategy = null)
		{
			throw new InvalidOperationException("Display set items are not supported by Key Images");
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
			if (!HasChanges || !Items.Any()) return new Dictionary<IStudySource, List<DicomFile>>(0);

			// the series index ensures consistent series level data because we only create one KO series and one PR series per study
			var studyIndex = new Dictionary<string, StudyInfo>();

			var framePresentationStates = new List<KeyValuePair<KeyImageReference, PresentationStateReference>>();

			// create presentation states for the images in the clipboard
			var presentationStates = new List<DicomSoftcopyPresentationState>();
			foreach (var item in Items.Where(i => i.Item is IPresentationImage))
			{
				var image = (IPresentationImage) item.Item;

				// if the item is a placeholder image (e.g. because source study wasn't available), simply reserialize the original sop references
				if (image is KeyObjectPlaceholderImage)
				{
					// because source study wasn't available, we don't have enough information to create the identical KO for that study
					// and thus an entry in the study index table is not needed
					var ko = (KeyObjectPlaceholderImage) image;
					framePresentationStates.Add(new KeyValuePair<KeyImageReference, PresentationStateReference>(ko.KeyImageReference, ko.PresentationStateReference));
					continue;
				}

				var provider = image as IImageSopProvider;
				if (provider == null) continue;

				StudyInfo studyInfo;
				var studyInstanceUid = provider.ImageSop.StudyInstanceUid;
				if (!studyIndex.TryGetValue(studyInstanceUid, out studyInfo))
				{
					studyIndex.Add(studyInstanceUid, studyInfo = new StudyInfo(provider, nextSeriesNumberDelegate));

					// keep the previous series number if the one we know about is the same study as this new document
					if (_parentStudyInstanceUid == studyInstanceUid && _seriesNumber.HasValue)
						studyInfo.KeyObjectSeriesNumber = _seriesNumber.Value;
				}

				// if the item doesn't have changes and the presentation state is DICOM, simply reserialize the original sop references
				if (!item.HasChanges() && image is IDicomPresentationImage)
				{
					var dicomPresentationState = ((IDicomPresentationImage) image).PresentationState as DicomSoftcopyPresentationState;
					framePresentationStates.Add(new KeyValuePair<KeyImageReference, PresentationStateReference>(provider.Frame, dicomPresentationState));
					continue;
				}

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
			public readonly DateTime PresentationSeriesDateTime;

			public readonly string KeyObjectSeriesUid;
			public readonly DateTime KeyObjectSeriesDateTime;

			private readonly IImageSopProvider _provider;
			private readonly NextSeriesNumberDelegate _nextSeriesNumberDelegate;

			private readonly string _studyInstanceUid;
			private readonly IDicomServiceNode _originServer;
			private readonly IDicomServiceNode _sourceServer;
			private int _presentationNextInstanceNumber = 1;
			private int? _presentationSeriesNumber;
			private int? _keyObjectSeriesNumber;

			public StudyInfo(IImageSopProvider provider, NextSeriesNumberDelegate nextSeriesNumberDelegate = null)
			{
				_provider = provider;
				_nextSeriesNumberDelegate = nextSeriesNumberDelegate ?? new DefaultNextSeriesNumberGetter().GetNextSeriesNumber;
				_studyInstanceUid = provider.Sop.StudyInstanceUid;
				_originServer = ServerDirectory.GetRemoteServersByAETitle(provider.Sop[DicomTags.SourceApplicationEntityTitle].ToString()).FirstOrDefault();
				_sourceServer = provider.Sop.DataSource.Server;

				KeyObjectSeriesUid = DicomUid.GenerateUid().UID;
				KeyObjectSeriesDateTime = Platform.Time;
				PresentationSeriesUid = DicomUid.GenerateUid().UID;
				PresentationSeriesDateTime = Platform.Time;
			}

			public IDicomAttributeProvider DataSource
			{
				get { return _provider.Sop.DataSource; }
			}

			public int PresentationSeriesNumber
			{
				get { return _presentationSeriesNumber ?? (_presentationSeriesNumber = _nextSeriesNumberDelegate(_provider.Frame)).Value; }
			}

			public int KeyObjectSeriesNumber
			{
				get { return _keyObjectSeriesNumber ?? (_keyObjectSeriesNumber = _nextSeriesNumberDelegate(_provider.Frame)).Value; }
				set { _keyObjectSeriesNumber = value; }
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

			private class DefaultNextSeriesNumberGetter
			{
				private int? _maxSeriesNumber;

				public int GetNextSeriesNumber(Frame f)
				{
					if (!_maxSeriesNumber.HasValue)
						_maxSeriesNumber = KeyImagePublisher.GetMaxSeriesNumber(f);
					return (_maxSeriesNumber = _maxSeriesNumber.Value + 1).Value;
				}
			}
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			Items.RaiseListChangedEvents = false;

			foreach (var item in Items.OfType<IDisposable>())
				item.Dispose();

			Items.Clear();
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