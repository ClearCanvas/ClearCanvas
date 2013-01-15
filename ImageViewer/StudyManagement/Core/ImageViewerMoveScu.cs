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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
    /// <summary>
    /// ImageViewer specific class customizing <see cref="StudyRootMoveScu"/>. 
    /// </summary>
    public class ImageViewerMoveScu : StudyRootMoveScu
    {
        #region Private Fields

        private readonly IPatientRootData _patientToRetrieve;
        private readonly IStudyIdentifier _studyToRetrieve;
        private readonly IEnumerable<string> _seriesInstanceUids;
        private string _errorDescriptionDetails;

        #endregion

        #region Public Properties

        public string ErrorDescriptionDetails { get { return _errorDescriptionDetails; } }

        /// <summary>
        /// Gets a value indicating whether or not the operation as a whole (as opposed to an individual sub-operation) has failed.
        /// </summary>
        /// <remarks>
        /// Typically, this refers to exceptions being thrown on the connection socket.
        /// </remarks>
        public bool Failed { get; private set; }

        #endregion

        #region Public Constructor

        public ImageViewerMoveScu(string localAETitle, IDicomServiceNode remoteAEInfo, IPatientRootData patient, IStudyIdentifier studiesToRetrieve)
            : base(localAETitle, remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName, remoteAEInfo.ScpParameters.Port, localAETitle)
        {
            Platform.CheckForEmptyString(localAETitle, "localAETitle");
            Platform.CheckForEmptyString(remoteAEInfo.AETitle, "AETitle");
            Platform.CheckForEmptyString(remoteAEInfo.ScpParameters.HostName, "HostName");
            Platform.CheckForNullReference(studiesToRetrieve, "studiesToRetrieve");

            _studyToRetrieve = studiesToRetrieve;
            _patientToRetrieve = patient;
            _errorDescriptionDetails = string.Empty;
        }

        public ImageViewerMoveScu(string localAETitle, IDicomServiceNode remoteAEInfo, IPatientRootData patient, IStudyIdentifier studyInformation, IEnumerable<string> seriesInstanceUids)
            : this(localAETitle, remoteAEInfo, patient, studyInformation)
        {
            _seriesInstanceUids = seriesInstanceUids;
        }

        #endregion

        #region Public Methods

        public override void OnReceiveResponseMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, ClearCanvas.Dicom.DicomMessage message)
        {
            base.OnReceiveResponseMessage(client, association, presentationID, message);

            if (message.Status.Status == DicomState.Warning)
            {
                DicomStatus status = DicomStatuses.LookupQueryRetrieve(message.Status.Code);
                _errorDescriptionDetails = String.Format("Remote server returned a warning status ({0}: {1}).",
                     RemoteAE, status.Description);
            }
        }

        public void Retrieve()
        {
            AddStudyInstanceUid(_studyToRetrieve.StudyInstanceUid);

            if (_seriesInstanceUids != null)
            {
                foreach (string seriesInstanceUid in _seriesInstanceUids)
                    AddSeriesInstanceUid(seriesInstanceUid);
            }

            //do this rather than use BeginSend b/c it uses thread pool threads which can be exhausted.

            try
            {
                Failed = false;

                Move();

                if (Status == ScuOperationStatus.Canceled)
                    Join();
                else
                    Join(new TimeSpan(0, 0, 0, 0, 2000));

                if (Status == ScuOperationStatus.Canceled || (_cancelRequested && Status == ScuOperationStatus.TimeoutExpired))
                {
                    _errorDescriptionDetails = string.Format("The Retrieve was cancelled ({0}).", RemoteAE);
                    Status = ScuOperationStatus.Canceled;
                }
                else if (Status == ScuOperationStatus.ConnectFailed)
                {
                    _errorDescriptionDetails = String.Format("Unable to connect to remote server ({0}: {1}).",
                        RemoteAE, FailureDescription ?? "no failure description provided");
                    Failed = true;
                }
                else if (Status == ScuOperationStatus.AssociationRejected)
                {
                    _errorDescriptionDetails = String.Format("Association rejected ({0}: {1}).",
                        RemoteAE, FailureDescription ?? "no failure description provided");
                    Failed = true;
                }
                else if (Status == ScuOperationStatus.Failed)
                {
                    _errorDescriptionDetails = String.Format("The Retrieve failed ({0}: {1}).",
                        RemoteAE, FailureDescription ?? "no failure description provided");
                    Failed = true;
                }
                else if (Status == ScuOperationStatus.TimeoutExpired)
                {
                    //ignore, because this is the scu, we don't want users to think an error has occurred
                    //in retrieving.
                    Failed = true;
                }
                else if (Status == ScuOperationStatus.UnexpectedMessage)
                {
                    //ignore, because this is the scu, we don't want users to think an error has occurred
                    //in retrieving.
                    Failed = true;
                }
                else if (Status == ScuOperationStatus.NetworkError)
                {
                    _errorDescriptionDetails = String.Format("The Retrieve failed ({0}: {1}).",
                       RemoteAE, FailureDescription ?? "no failure description provided");
                    Failed = true;
                }             
            }
            catch (Exception e)
            {
                Failed = true;

                if (Status == ScuOperationStatus.ConnectFailed)
                {
                    _errorDescriptionDetails = String.Format("Unable to connect to remote server ({0}: {1}).",
                        RemoteAE, FailureDescription ?? "no failure description provided");
                }
                else
                {
                    _errorDescriptionDetails = String.Format("An unexpected error has occurred in the Move Scu: {0}:{1}:{2} -> {3}; {4}",
                                                  RemoteAE, RemoteHost, RemotePort, ClientAETitle, e.Message);
                }
            }
            finally
            {
                //OnRetrieveComplete(this);
            }
        }

        #endregion
    }
}
