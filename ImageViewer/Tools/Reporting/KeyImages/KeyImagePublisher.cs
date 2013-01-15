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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	internal class KeyImagePublisher
	{
		private readonly KeyImageInformation _sourceInformation;
		private readonly List<DicomFile> _keyObjectDocuments;

		private List<KeyValuePair<Frame, DicomSoftcopyPresentationState>> _framePresentationStates;
		private Dictionary<string, SeriesInfo> _seriesIndex;

		public KeyImagePublisher(KeyImageInformation information)
		{
			_sourceInformation = information;
			_keyObjectDocuments = new List<DicomFile>();
		}

		private List<KeyValuePair<Frame, DicomSoftcopyPresentationState>> SourceFrames
		{
			get
			{
				if (_framePresentationStates == null)
				{
					_framePresentationStates = new List<KeyValuePair<Frame, DicomSoftcopyPresentationState>>();
					_seriesIndex = new Dictionary<string, SeriesInfo>();
					foreach (IClipboardItem item in _sourceInformation.ClipboardItems)
					{
						var image = item.Item as IPresentationImage;
						if (image == null)
							continue;

						if (!DicomSoftcopyPresentationState.IsSupported(image))
							continue;

						var provider = image as IImageSopProvider;
						if (provider == null)
							continue;

						SeriesInfo seriesInfo;
						string key = provider.ImageSop.StudyInstanceUid;
						if (!_seriesIndex.TryGetValue(key, out seriesInfo))
							_seriesIndex.Add(key, seriesInfo = new SeriesInfo(provider));

						var presentationState = DicomSoftcopyPresentationState.Create
							(image, delegate(DicomSoftcopyPresentationState ps)
							        	{
							        		ps.PresentationSeriesInstanceUid = seriesInfo.PresentationSeriesUid;
							        		ps.PresentationSeriesNumber = seriesInfo.PresentationSeriesNumber;
							        		ps.PresentationSeriesDateTime = seriesInfo.PresentationSeriesDateTime;
							        		ps.PresentationInstanceNumber = seriesInfo.GetNextPresentationInstanceNumber();
							        	    ps.SourceAETitle = DicomServer.AETitle;
							        	});

						_framePresentationStates.Add(new KeyValuePair<Frame, DicomSoftcopyPresentationState>(provider.Frame, presentationState));
					}
				}

				return _framePresentationStates;
			}
		}

		/// <remarks>
		/// The current implementation of <see cref="KeyImagePublisher"/> supports only locally stored images that are <see cref="IImageSopProvider"/>s and supports <see cref="DicomSoftcopyPresentationState"/>s.
		/// </remarks>
		public static bool IsSupportedImage(IPresentationImage image)
		{
			var imageSopProvider = image as IImageSopProvider;
			if (imageSopProvider == null)
				return false;
			return imageSopProvider.ImageSop.DataSource.IsStored && DicomSoftcopyPresentationState.IsSupported(image);
		}

		private void CreateKeyObjectDocuments()
		{
			KeyImageSerializer serializer = new KeyImageSerializer();
			serializer.Description = _sourceInformation.Description;
			serializer.DocumentTitle = _sourceInformation.DocumentTitle;
			serializer.SeriesDescription = _sourceInformation.SeriesDescription;
		    serializer.SourceAETitle = DicomServer.AETitle;

			foreach (KeyValuePair<Frame, DicomSoftcopyPresentationState> presentationFrame in SourceFrames)
				serializer.AddImage(presentationFrame.Key, presentationFrame.Value);

			_keyObjectDocuments.AddRange(serializer.Serialize(
			                             	delegate(KeyImageSerializer.KeyObjectDocumentSeries keyObjectDocumentSeries)
			                             		{
			                             			string key = keyObjectDocumentSeries.StudyInstanceUid;
			                             			if (_seriesIndex.ContainsKey(key))
			                             			{
			                             				keyObjectDocumentSeries.SeriesDateTime = _seriesIndex[key].KeyObjectSeriesDateTime;
			                             				keyObjectDocumentSeries.SeriesNumber = _seriesIndex[key].KeyObjectSeriesNumber;
			                             				keyObjectDocumentSeries.SeriesInstanceUid = _seriesIndex[key].KeyObjectSeriesUid;
			                             			}
			                             		}
			                             	));
		}

		public void Publish()
		{
			if (_sourceInformation.ClipboardItems.Count == 0)
				return;

            var service = Platform.GetService<IPublishFiles>();
            while (!service.CanPublish())
            {
                // TODO CR (Sep 12): convert this to a desktop alert
                DialogBoxAction result = Application.ActiveDesktopWindow.ShowMessageBox(
                    SR.MessageCannotPublishKeyImagesServersNotRunning, MessageBoxActions.OkCancel);

                if (result == DialogBoxAction.Cancel)
                    return;
            }                    
    
		    var anyFailed = false;
		    try
		    {
                CreateKeyObjectDocuments();

                var publishers = new Dictionary<string, DicomPublishingHelper>();

                // add each KO document to a publisher by study
                foreach (var koDocument in _keyObjectDocuments)
                {
                    var publisher = GetValue(publishers, koDocument.DataSet[DicomTags.StudyInstanceUid].ToString());
                    publisher.Files.Add(koDocument);
                }

                // add each PR state to a publisher by study
                foreach (var presentationFrame in SourceFrames)
                {
                    var sourceFrame = presentationFrame.Key;
                    var publisher = GetValue(publishers, sourceFrame.StudyInstanceUid);

                    if (presentationFrame.Value != null)
                        publisher.Files.Add(presentationFrame.Value.DicomFile);

                    var sopDataSource = sourceFrame.ParentImageSop.DataSource;
                    publisher.OriginServer = ServerDirectory.GetRemoteServersByAETitle(sopDataSource[DicomTags.SourceApplicationEntityTitle].ToString()).FirstOrDefault();
                    publisher.SourceServer = sopDataSource.Server;
                }

                // publish all files now
		        foreach (var publisher in publishers.Values)
                {
                    if (!publisher.Publish())
                        anyFailed = true;
                }
		    }
		    catch (Exception e)
		    {
		        anyFailed = true;
		        Platform.Log(LogLevel.Error, e, "An unexpected error occurred while trying to publish key images.");
		    }

			// TODO CR (Sep 12): convert this to a desktop alert
			if (anyFailed)
				Application.ActiveDesktopWindow.ShowMessageBox(SR.MessageKeyImagePublishingFailed, MessageBoxActions.Ok);
		}

		private static T GetValue<T>(IDictionary<string, T> dictionary, string key)
			where T : new()
		{
			if (!dictionary.ContainsKey(key))
				dictionary.Add(key, new T());
			return dictionary[key];
		}

		private class SeriesInfo
		{
			public readonly string PresentationSeriesUid;
			public readonly int PresentationSeriesNumber;
			public readonly DateTime PresentationSeriesDateTime;
			public readonly string KeyObjectSeriesUid;
			public readonly int KeyObjectSeriesNumber;
			public readonly DateTime KeyObjectSeriesDateTime;

			private int _presentationNextInstanceNumber;

			public SeriesInfo(IImageSopProvider provider)
			{
				KeyObjectSeriesUid = DicomUid.GenerateUid().UID;
				KeyObjectSeriesNumber = CalculateSeriesNumber(provider.Frame);
				KeyObjectSeriesDateTime = Platform.Time;
				PresentationSeriesUid = DicomUid.GenerateUid().UID;
				PresentationSeriesNumber = KeyObjectSeriesNumber + 1;
				PresentationSeriesDateTime = Platform.Time;
				_presentationNextInstanceNumber = 1;
			}

			public int GetNextPresentationInstanceNumber()
			{
				return _presentationNextInstanceNumber++;
			}

			private static int CalculateSeriesNumber(Frame frame)
			{
				if (frame.ParentImageSop == null || frame.ParentImageSop.ParentSeries == null || frame.ParentImageSop.ParentSeries.ParentStudy == null)
					return 1;

				int maxValue = 0;
				foreach (Series series in frame.ParentImageSop.ParentSeries.ParentStudy.Series)
				{
					if (series.SeriesNumber > maxValue)
						maxValue = series.SeriesNumber;
				}

				return maxValue + 1;
			}
		}
	}
}