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

using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Static class used to verify if an association should be accepted.
    /// </summary>
    public static class AssociationVerifier
    {
        /// <summary>
        /// Do the actual verification if an association is acceptable.
        /// </summary>
        /// <remarks>
        /// This method primarily checks the remote AE title to see if it is a valid device that can 
        /// connect to the partition.
        /// </remarks>
        /// <param name="context">Generic parameter passed in, is a DicomScpParameters instance.</param>
        /// <param name="assocParms">The association parameters.</param>
        /// <param name="result">Output parameter with the DicomRejectResult for rejecting the association.</param>
        /// <param name="reason">Output parameter with the DicomRejectReason for rejecting the association.</param>
        /// <returns>true if the association should be accepted, false if it should be rejected.</returns>
        public static bool Verify(DicomScpContext context, ServerAssociationParameters assocParms, out DicomRejectResult result, out DicomRejectReason reason)
        {
            bool isNew;
            context.Device = DeviceManager.LookupDevice(context.Partition, assocParms, out isNew);

			if (context.Device == null)
            {
				if (context.Partition.AcceptAnyDevice)
				{
					reason = DicomRejectReason.NoReasonGiven;
					result = DicomRejectResult.Permanent;
					return true;
				}

            	reason = DicomRejectReason.CallingAENotRecognized;
                result = DicomRejectResult.Permanent;
                return false;
            }

			if (context.Device.Enabled == false)
            {
            
                Platform.Log(LogLevel.Error,
                             "Rejecting association from {0} to {1}.  Device is disabled.",
                             assocParms.CallingAE, assocParms.CalledAE);
                
                reason = DicomRejectReason.CallingAENotRecognized;
                result = DicomRejectResult.Permanent;
                return false;
                
            }

            reason = DicomRejectReason.NoReasonGiven;
            result = DicomRejectResult.Permanent;

            return true;
        }
    }
}