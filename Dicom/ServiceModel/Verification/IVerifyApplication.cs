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

using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.Dicom.ServiceModel.Verification
{
	[DataContract(Namespace = VerificationNamespace.Value)]
	public class VerifyRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public string LocalApplicationEntity { get; set; }

		[DataMember(IsRequired = true)]
		public ApplicationEntity RemoteApplicationEntity { get; set; }
	}

	[DataContract(Namespace = VerificationNamespace.Value)]
	public class VerifyResponse : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public VerificationResult Result { get; set; }
	}

	[ServiceContract(SessionMode = SessionMode.Allowed, Namespace = VerificationNamespace.Value)]
	public interface IVerifyApplication
	{
		/// <summary>
		/// Performs a C-ECHO-RQ against a remote application.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{VerificationFailedFault}">Thrown when the verification fails.</exception>
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(VerificationFailedFault))]
		[OperationContract(IsOneWay = false)]
		VerifyResponse Verify(VerifyRequest request);
	}
}
