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

namespace ClearCanvas.Dicom.Network
{
    public interface IDicomClientHandler
    {
        void OnReceiveAssociateAccept(DicomClient client, ClientAssociationParameters association);
        void OnReceiveAssociateReject(DicomClient client, ClientAssociationParameters association, DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason);
        void OnReceiveRequestMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message);
        void OnReceiveResponseMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message);
        void OnReceiveReleaseResponse(DicomClient client, ClientAssociationParameters association);
        void OnReceiveAbort(DicomClient client, ClientAssociationParameters association, DicomAbortSource source, DicomAbortReason reason);
        void OnNetworkError(DicomClient client, ClientAssociationParameters association, Exception e);
        void OnDimseTimeout(DicomClient server, ClientAssociationParameters association);
    }
}
