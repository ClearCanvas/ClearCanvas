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
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	internal static class KeyImagePublisher
	{
		public static void Publish(IEnumerable<KeyImageInformation> keyImageInformations, StudyTree studyTree)
		{
			var keyImageContexts = keyImageInformations != null ? keyImageInformations.ToList() : null;
			if (keyImageContexts == null || !keyImageContexts.Any()) return;

			if (!AssertIsPublishingServiceAvailable()) return;

			var anyFailed = false;
			try
			{
				var publishers = new Dictionary<string, DicomPublishingHelper>();

				var seriesNumberIndex = new Dictionary<string, int>();
				var nextSeriesNumberDelegate = new NextSeriesNumberDelegate(studyInstanceUid =>
				                                                            {
					                                                            int nextSeriesNumber;
					                                                            if (!seriesNumberIndex.TryGetValue(studyInstanceUid, out nextSeriesNumber))
					                                                            {
						                                                            var study = studyTree.GetStudy(studyInstanceUid);
						                                                            nextSeriesNumber = study != null ? study.Series.Select(series => series.SeriesNumber).Concat(new[] {0}).Max() : 0;
					                                                            }
					                                                            seriesNumberIndex[studyInstanceUid] = ++nextSeriesNumber;
					                                                            return nextSeriesNumber;
				                                                            });

				foreach (var kod in keyImageContexts)
				{
					// add each created instance to a publisher by study
					foreach (var studyInstances in kod.CreateSopInstances(nextSeriesNumberDelegate))
					{
						var studyInstanceUid = studyInstances.Key.StudyInstanceUid;
						var publisher = GetValue(publishers, studyInstanceUid);
						publisher.OriginServer = studyInstances.Key.OriginServer;
						publisher.SourceServer = FindStudySourceServer(studyTree, studyInstanceUid);
						foreach (var f in studyInstances.Value)
							publisher.Files.Add(f);
					}
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
			{
				// ActiveDesktopWindow may be null when the study is opened from Webstation
				// By the time viewer closes, there is no browser window to display the error, so log to file only.
				if (Application.ActiveDesktopWindow == null)
					Platform.Log(LogLevel.Error, SR.MessageKeyImagePublishingFailed);
				else
					Application.ActiveDesktopWindow.ShowMessageBox(SR.MessageKeyImagePublishingFailed, MessageBoxActions.Ok);
			}
		}

		private static IDicomServiceNode FindStudySourceServer(StudyTree studyTree, string studyInstanceUid)
		{
			try
			{
				return studyTree.GetStudy(studyInstanceUid).Series.SelectMany(s => s.Sops).Select(s => s.DataSource.Server).FirstOrDefault(source => source != null);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, "Failed to identify source server for study {0}", studyInstanceUid);
				return null;
			}
		}

		private static bool AssertIsPublishingServiceAvailable()
		{
			var service = Platform.GetService<IPublishFiles>();
			while (!service.CanPublish())
			{
				// ActiveDesktopWindow may be null when the study is opened from Webstation
				// By the time viewer closes, there is no browser window to display the error
				if (Application.ActiveDesktopWindow == null)
				{
					// Log to file only and return immediately
					Platform.Log(LogLevel.Error, SR.MessageKeyImagePublishingFailed);
					return false;
				}
				else
				{
					var result = Application.ActiveDesktopWindow.ShowMessageBox(SR.MessageCannotPublishKeyImagesServersNotRunning, MessageBoxActions.OkCancel);
					if (result == DialogBoxAction.Cancel)
						return false;
				}
			}
			return true;
		}

		private static T GetValue<T>(IDictionary<string, T> dictionary, string key)
			where T : new()
		{
			if (!dictionary.ContainsKey(key))
				dictionary.Add(key, new T());
			return dictionary[key];
		}
	}
}