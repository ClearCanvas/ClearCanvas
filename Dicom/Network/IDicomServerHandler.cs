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
	public interface IDicomFilestreamHandler
	{
		bool SaveStreamData(DicomMessage message, byte[] data, int offset, int count);
		void CancelStream();
		bool CompleteStream(DicomServer server, ServerAssociationParameters assoc, byte presentationId, DicomMessage message);
	}

    public interface IDicomServerHandler
    {
        void OnReceiveAssociateRequest(DicomServer server, ServerAssociationParameters association);
        void OnReceiveRequestMessage(DicomServer server, ServerAssociationParameters association, byte presentationId, DicomMessage message);
        void OnReceiveResponseMessage(DicomServer server, ServerAssociationParameters association, byte presentationId, DicomMessage message);
        void OnReceiveReleaseRequest(DicomServer server, ServerAssociationParameters association);
        void OnReceiveDimseCommand(DicomServer server, ServerAssociationParameters association, byte presentationId, DicomAttributeCollection command);

        IDicomFilestreamHandler OnStartFilestream(DicomServer server, ServerAssociationParameters association, byte presentationId, DicomMessage message);

        void OnReceiveAbort(DicomServer server, ServerAssociationParameters association, DicomAbortSource source, DicomAbortReason reason);
        void OnNetworkError(DicomServer server, ServerAssociationParameters association, Exception e);
        void OnDimseTimeout(DicomServer server, ServerAssociationParameters association);
    }
}
