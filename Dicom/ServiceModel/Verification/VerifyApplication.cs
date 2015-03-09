#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.ServiceModel.Verification
{
	public class VerifyApplication : IVerifyApplication
	{
		public VerifyResponse Verify(VerifyRequest request)
		{
			try
			{
				var verify = new VerificationScu();

				var result = verify.Verify(request.LocalApplicationEntity, request.RemoteApplicationEntity.AETitle, request.RemoteApplicationEntity.ScpParameters.HostName, request.RemoteApplicationEntity.ScpParameters.Port);

				if (result == VerificationResult.AssociationRejected)
				{
					var fault = new VerificationFailedFault
						{
							Description = SR.VerifyAssociationRejected,
							Result = result
						};
					throw new FaultException<VerificationFailedFault>(fault, fault.Description);
				}
				if (result == VerificationResult.Failed)
				{
					var fault = new VerificationFailedFault
						{
							Description = SR.VerifyFailed,
							Result =  result
						};
					throw new FaultException<VerificationFailedFault>(fault, fault.Description);
				}
				if (result == VerificationResult.Canceled)
				{
					var fault = new VerificationFailedFault
						{
							Description = SR.VerifyCanceled,
							Result = result
						};
					throw new FaultException<VerificationFailedFault>(fault, fault.Description);
				}
				if (result == VerificationResult.TimeoutExpired)
				{
					var fault = new VerificationFailedFault
						{
							Description = SR.VerifyTimeoutExpired,
							Result = result
						};
					throw new FaultException<VerificationFailedFault>(fault, fault.Description);
				}

				return new VerifyResponse
					{
						Result = verify.Result
					};
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Info, "User DICOM Verification not understood: {0}", e.Message);
				var fault = new VerificationFailedFault
				{
					Description = e.Message,
					Result = VerificationResult.Failed
				};
				throw new FaultException<VerificationFailedFault>(fault, fault.Description);
			}
		}
	}
}
