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
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.DicomRetrieve
{
    /// <summary>
    ///  Class for procesing DICOM Retrieves.
    /// </summary>
    internal class DicomRetrieveItemProcessor : BaseItemProcessor<DicomRetrieveRequest, DicomRetrieveProgress>
    {
        #region Private Members

        private ImageViewerMoveScu _scu;
        private bool _cancelDueToDiskSpace = false;
        #endregion

        #region Public Properties

        public DicomRetrieveStudyRequest RetrieveStudy
        {
            get { return Request as DicomRetrieveStudyRequest; }
        }

        public DicomRetrieveSeriesRequest RetrieveSeries
        {
            get { return Request as DicomRetrieveSeriesRequest; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Override of Cancel() routine.
        /// </summary>
        /// <remarks>
        /// The Cancel must be overriden to call the SCU's Cancel routine.
        /// </remarks>
        public override void Cancel()
        {
            base.Cancel();
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
            _scu.Cancel();
            base.Stop();
        }

        public override void Process()
        {
            EnsureMaxUsedSpaceNotExceeded();

            DicomServerConfiguration configuration = GetServerConfiguration();
            var remoteAE = ServerDirectory.GetRemoteServerByName(Request.ServerName);
            if (remoteAE == null)
            {
                Proxy.Fail(string.Format("Unknown destination: {0}", Request.ServerName), WorkItemFailureType.Fatal);
                return;
            }

            if (RetrieveStudy != null)
                _scu = new ImageViewerMoveScu(configuration.AETitle, remoteAE, RetrieveStudy.Patient, RetrieveStudy.Study);
            else if (RetrieveSeries != null)
                _scu = new ImageViewerMoveScu(configuration.AETitle, remoteAE, RetrieveSeries.Patient, RetrieveSeries.Study, RetrieveSeries.SeriesInstanceUids);
            else
            {
                Proxy.Fail("Invalid request type.", WorkItemFailureType.Fatal);
                return;
            }
            
            Progress.ImagesToRetrieve = _scu.TotalSubOperations;
            Progress.FailureSubOperations = 0;
            Progress.WarningSubOperations = 0;
            Progress.SuccessSubOperations = 0;
            Progress.IsCancelable = false;
            Proxy.UpdateProgress();

            _scu.ImageMoveCompleted += OnMoveImage;
            
            _scu.Retrieve();

            Progress.ImagesToRetrieve = _scu.TotalSubOperations;
            Progress.SuccessSubOperations = _scu.SuccessSubOperations;
            Progress.FailureSubOperations = _scu.FailureSubOperations;
            Progress.WarningSubOperations = _scu.WarningSubOperations;
            Progress.StatusDetails = !string.IsNullOrEmpty(_scu.ErrorDescriptionDetails) ? _scu.ErrorDescriptionDetails : _scu.FailureDescription;            

            if (_scu.Canceled)
            {
                if (_cancelDueToDiskSpace)
                {
                    var study = RetrieveStudy.Study ?? RetrieveSeries.Study;

                    Platform.Log(LogLevel.Info, "Dicom Retrieve for {0} from {1} was cancelled because disk space has been exceeded", study, remoteAE.AETitle);
                    Progress.IsCancelable = true;
                    throw new NotEnoughStorageException(); // item will be failed
                }
                else if (StopPending)
                {
                    Progress.IsCancelable = true;
                    Proxy.Postpone();
                }
                else
                {
                    Proxy.Cancel();
                }
            }
            else if (_scu.FailureSubOperations > 0 || _scu.Failed)
            {
                Progress.IsCancelable = true;
                Proxy.Fail(!string.IsNullOrEmpty(_scu.ErrorDescriptionDetails) ? _scu.ErrorDescriptionDetails : _scu.FailureDescription, WorkItemFailureType.NonFatal);
            }
            else
            {
                Proxy.Complete();
            }
        }

        #endregion

        #region Private Methods

        private void OnMoveImage(object sender, EventArgs storageInstance)
        {
            Progress.ImagesToRetrieve = _scu.TotalSubOperations;
            Progress.SuccessSubOperations = _scu.SuccessSubOperations;
            Progress.FailureSubOperations = _scu.FailureSubOperations;
            Progress.WarningSubOperations = _scu.WarningSubOperations;
            Progress.StatusDetails = !string.IsNullOrEmpty(_scu.ErrorDescriptionDetails) ? _scu.ErrorDescriptionDetails : _scu.FailureDescription;
            Proxy.UpdateProgress();

            if (LocalStorageMonitor.IsMaxUsedSpaceExceeded)
            {
                _cancelDueToDiskSpace = true;
                _scu.Cancel();
            }
        }

        #endregion
    }
}
