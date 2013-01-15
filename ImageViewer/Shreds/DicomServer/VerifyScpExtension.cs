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

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class VerifyScpExtension : ScpExtension
	{
		public VerifyScpExtension()
			: base(GetSupportedSops())
		{
		}

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
		    var sop = new SupportedSop
		                  {
		                      SopClass = SopClass.VerificationSopClass
		                  };
		    sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
			sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
			yield return sop;
		}

        public override bool OnReceiveRequest(ClearCanvas.Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
		{
			server.SendCEchoResponse(presentationID, message.MessageId, DicomStatuses.Success);
			return true;
		}
	}
}
