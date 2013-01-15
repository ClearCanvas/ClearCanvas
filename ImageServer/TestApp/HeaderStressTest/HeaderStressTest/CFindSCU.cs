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
using System.Net;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace HeaderStressTest
{
    class CFindSCU : IDicomClientHandler
    {
        private string _aeTitle;
        private DicomClient _dicomClient;

        public string AETitle
        {
            get { return _aeTitle; }
            set { _aeTitle = value; }
        }

        public delegate void ResultReceivedHandler(DicomAttributeCollection ds);
        public delegate void QueryCompletedHandler();


        public event ResultReceivedHandler OnResultReceive;
        public event QueryCompletedHandler OnQueryCompleted;

        public void Query(string remoteAE, string remoteHost, int remotePort)
        {

            IPAddress addr = Dns.GetHostAddresses(remoteHost)[0];
            ClientAssociationParameters _assocParams = new ClientAssociationParameters(AETitle, remoteAE, new IPEndPoint(addr, remotePort));

            byte pcid = _assocParams.AddPresentationContext(SopClass.StudyRootQueryRetrieveInformationModelFind);
            _assocParams.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            _assocParams.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            _dicomClient = DicomClient.Connect(_assocParams, this);

        }

        private void SendCFind()
        {
            DicomMessage msg = new DicomMessage();
            DicomAttributeCollection cFindDataset = msg.DataSet;

            // set the Query Retrieve Level
            cFindDataset[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");

            // set the other tags we want to retrieve
            cFindDataset[DicomTags.StudyInstanceUid].SetStringValue("");
            cFindDataset[DicomTags.PatientsName].SetStringValue("");
            cFindDataset[DicomTags.PatientId].SetStringValue("");
            cFindDataset[DicomTags.ModalitiesInStudy].SetStringValue("");
            cFindDataset[DicomTags.StudyDescription].SetStringValue("");


            byte pcid = _dicomClient.AssociationParams.FindAbstractSyntax(SopClass.StudyRootQueryRetrieveInformationModelFind);
            _dicomClient.SendCFindRequest(pcid, _dicomClient.NextMessageID(), msg);

        }

        #region IDicomClientHandler Members

        public void OnReceiveAssociateAccept(DicomClient client, ClientAssociationParameters association)
        {
            SendCFind();
        }

        public void OnReceiveAssociateReject(DicomClient client, ClientAssociationParameters association, DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason)
        {

        }

        public void OnReceiveRequestMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message)
        {

        }

        public void OnReceiveResponseMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message)
        {
            if (message.Status.Status == DicomState.Pending)
            {
                string studyinstanceuid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
                if (OnResultReceive != null)
                    OnResultReceive(message.DataSet);
            }
            else
            {
                _dicomClient.SendReleaseRequest();
            }
        }

        public void OnReceiveReleaseResponse(DicomClient client, ClientAssociationParameters association)
        {
            if (OnQueryCompleted != null)
            {
                OnQueryCompleted();
                _dicomClient.Dispose();
            }
        }

        public void OnReceiveAbort(DicomClient client, ClientAssociationParameters association, DicomAbortSource source, DicomAbortReason reason)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void OnNetworkError(DicomClient client, ClientAssociationParameters association, Exception e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void OnDimseTimeout(DicomClient server, ClientAssociationParameters association)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
