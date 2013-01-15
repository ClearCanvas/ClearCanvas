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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.DicomSend
{
    /// <summary>
    ///  Class for processing DICOM Sends.
    /// </summary>
    internal class DicomSendItemProcessor : BaseItemProcessor<DicomSendRequest, DicomSendProgress>
    {
        #region Private Members

        private ImageViewerStorageScu _scu;

        #endregion

        #region Public Properties

        public DicomSendStudyRequest SendStudy
        {
            get { return Request as DicomSendStudyRequest; }
        }

        public DicomSendSeriesRequest SendSeries
        {
            get { return Request as DicomSendSeriesRequest; }
        }

        public DicomSendSopRequest SendSops
        {
            get { return Request as DicomSendSopRequest; }
        }

        public DicomAutoRouteRequest AutoRoute
        {
            get { return Request as DicomAutoRouteRequest; }
        }

        public PublishFilesRequest PublishFiles
        {
            get { return Request as PublishFilesRequest; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Cleanup any failed items in the queue and delete the queue entry.
        /// </summary>
        public override void Delete()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemUidBroker();
                var uidBroker = context.GetWorkItemUidBroker();

                var list = broker.GetWorkItemUidsForWorkItem(Proxy.Item.Oid);
                foreach (WorkItemUid sop in list)
                {
                    uidBroker.Delete(sop);
                }
                context.Commit();
            }

            if (PublishFiles != null && (PublishFiles.DeletionBehaviour == DeletionBehaviour.DeleteAlways || PublishFiles.DeletionBehaviour == DeletionBehaviour.DeleteOnSuccess))
            {
                foreach (string file in PublishFiles.FilePaths)
                {
                    try
                    {
                        FileUtils.Delete(file);
                        DirectoryUtility.DeleteIfEmpty(Path.GetDirectoryName(file));
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Warn, "Unable to delete temporary publish file {0}: {1}", file, e.Message);
                    }
                }
            }

            Proxy.Delete();
        }

        /// <summary>
        /// Override of Cancel() routine.
        /// </summary>
        /// <remarks>
        /// The Cancel must be overriden to call the SCU's Cancel routine.
        /// </remarks>
        public override void Cancel()
        {
            base.Cancel();

            if (_scu != null)
                _scu.Cancel();
        }

        /// <summary>
        /// Override of Stop() routine.
        /// </summary>
        /// <remarks>
        /// The Stop must be override to call the SCU's Cancel routine.
        /// </remarks>
        public override void Stop()
        {
            if (_scu != null)
            {
                _scu.Cancel();
                _scu = null;
            }
            base.Stop();
        }

        public override void Process()
        {
            DicomServerConfiguration configuration = GetServerConfiguration();
            var remoteAE = ServerDirectory.GetRemoteServerByName(Request.DestinationServerName);
            if (remoteAE == null)
            {
                Proxy.Fail(string.Format("Unknown destination: {0}",Request.DestinationServerName),WorkItemFailureType.Fatal);
                return;

            }

            if (AutoRoute != null && Proxy.Item.Priority != WorkItemPriorityEnum.Stat)
            {
                DateTime now = Platform.Time;
                DateTime scheduledTime = AutoRoute.GetScheduledTime(now, 0);
                if (now != scheduledTime)
                {
                    Proxy.Postpone();
                    Platform.Log(LogLevel.Info, "Rescheduling AutoRoute WorkItem {0} back into the scheduled time window: {1}", Proxy.Item.Oid, Proxy.Item.ProcessTime);
                    return;
                }
            }

            _scu = new ImageViewerStorageScu(configuration.AETitle, remoteAE);
            
            LoadImagesToSend();

            if (Request.CompressionType != CompressionType.None)
            {
                _scu.LoadPreferredSyntaxes(Request);    
            }

            Progress.TotalImagesToSend = _scu.TotalSubOperations;            
            Progress.FailureSubOperations = 0;
            Progress.WarningSubOperations = 0;
            Progress.SuccessSubOperations = 0;
            Progress.IsCancelable = true;
            Proxy.UpdateProgress();

            _scu.ImageStoreCompleted += OnImageSent;

            _scu.DoSend();

            if (_scu.Canceled)
            {
                if (StopPending)
                {
                    Proxy.Postpone();
                }
                else
                {
                    Proxy.Cancel();
                }
            } 
            else if (_scu.Failed || _scu.FailureSubOperations > 0)
            {
                var settings = new DicomSendSettings();
                TimeSpan delay = settings.RetryDelayUnits == RetryDelayTimeUnit.Seconds
                                     ? TimeSpan.FromSeconds(settings.RetryDelay) 
                                     : TimeSpan.FromMinutes(settings.RetryDelay);

                Proxy.Fail(_scu.FailureDescription, WorkItemFailureType.NonFatal,
                           AutoRoute != null
                               ? AutoRoute.GetScheduledTime(Platform.Time, (int)delay.TotalSeconds)
                               : Platform.Time.Add(delay),settings.RetryCount);
            }
            else
            {
                Proxy.Complete();
            }
        }

        #endregion

        #region Private Methods

        private void LoadImagesToSend()
        {
            if (SendStudy != null)
            {
                var studyXml = Location.LoadStudyXml();

                _scu.LoadStudyFromStudyXml(Location, studyXml);
            }
            else if (SendSeries != null)
            {
                var studyXml = Location.LoadStudyXml();

                foreach (var seriesInstanceUid in SendSeries.SeriesInstanceUids)
                {
                    _scu.LoadSeriesFromStudyXml(Location, studyXml, seriesInstanceUid);
                }
            }
            else if (SendSops != null)
            {
                foreach (string sop in SendSops.SopInstanceUids)
                {
                    _scu.AddFile(Location.GetSopInstancePath(SendSops.SeriesInstanceUid, sop));
                }
            }
            else if (AutoRoute != null)
            {
                LoadUids();
                if (WorkQueueUidList.Count == 0)
                {
                    var studyXml = Location.LoadStudyXml();

                    _scu.LoadStudyFromStudyXml(Location, studyXml);
                }
                else
                {
                    foreach (var uid in WorkQueueUidList)
                    {
                        _scu.AddFile(Location.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid));
                    }
                }
            }
            else if (PublishFiles != null)
            {
                foreach (var path in PublishFiles.FilePaths)
                    _scu.AddFile(path);
            }
        }

        private void OnImageSent(object sender, StorageInstance storageInstance)
        {
            var scu = sender as ImageViewerStorageScu;
            Progress.TotalImagesToSend = _scu.TotalSubOperations;

            if (storageInstance.SendStatus.Status == DicomState.Success)
            {
                Progress.SuccessSubOperations++;
                Progress.StatusDetails = string.Empty;
            }
            else if (storageInstance.SendStatus.Status == DicomState.Failure)
            {
                Progress.FailureSubOperations++;
                Progress.StatusDetails = storageInstance.ExtendedFailureDescription;
                if (String.IsNullOrEmpty(Progress.StatusDetails))
                    Progress.StatusDetails = storageInstance.SendStatus.ToString();

                if (_scu != null) _scu.FailureDescription = Progress.StatusDetails;
            }
            else if (storageInstance.SendStatus.Status == DicomState.Warning)
            {
                Progress.WarningSubOperations++;
                Progress.StatusDetails = storageInstance.ExtendedFailureDescription;
                if (String.IsNullOrEmpty(Progress.StatusDetails))
                    Progress.StatusDetails = storageInstance.SendStatus.ToString();

                if (_scu != null) _scu.FailureDescription = Progress.StatusDetails;
            }

            Proxy.UpdateProgress();

            if (PublishFiles != null &&
                PublishFiles.DeletionBehaviour != DeletionBehaviour.None)
            {
                bool deleteFile = false;
                if (storageInstance.SendStatus.Status != DicomState.Failure)
                    deleteFile = true;
                else if (PublishFiles.DeletionBehaviour == DeletionBehaviour.DeleteAlways)
                    deleteFile = true;

                if (deleteFile)
                {
                    try
                    {
                        FileUtils.Delete(storageInstance.Filename);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Warn, e,
                                     "Failed to delete file after storage: {0}",
                                     storageInstance.Filename);
                    }
                }
            }
        }

        #endregion
    }
}
