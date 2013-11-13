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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.ImageServer.Services.Dicom;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Plugin for handling DICOM Verification (C-ECHO) requests.
    /// </summary>
    [ExtensionOf(typeof(DicomScpExtensionPoint<DicomScpContext>))]
    public class VerificationScpExtension : BaseScp, IDicomScp<DicomScpContext>
    {
        #region Private members
        private List<SupportedSop> _list = new List<SupportedSop>();
        #endregion

        #region Contructors
        /// <summary>
        /// Public default constructor.  Implements the Verification SOP Class.
        /// </summary>
        public VerificationScpExtension()
        {
            SupportedSop sop = new SupportedSop();
            sop.SopClass = SopClass.VerificationSopClass;
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            _list.Add(sop);
        }
        #endregion

        #region IDicomScp Members

        public override bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationId, DicomMessage message)
        {
            server.SendCEchoResponse(presentationId,message.MessageId,DicomStatuses.Success);
            return true;
        }

        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            return _list;
        }

        #endregion

        #region Overridden BaseSCP methods

        protected override DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid)
        {
            return DicomPresContextResult.Accept;
        }

        #endregion Overridden BaseSCP methods
    }
}