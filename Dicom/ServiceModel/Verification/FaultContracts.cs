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
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.ServiceModel.Verification
{
	/// <summary>
	/// The namespace for all the query data and service contracts.
	/// </summary>
	public class VerificationNamespace
	{
		/// <summary>
		/// The namespace for all the query data and service contracts.
		/// </summary>
		public const string Value = DicomNamespace.Value + "/verification";
	}

	/// <summary>
	/// Data contract for 'verification failed' faults.
	/// </summary>
	[DataContract(Namespace = VerificationNamespace.Value)]
	public class VerificationFailedFault
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public VerificationFailedFault()
		{
		}

		/// <summary>
		/// A textual description of the verification failure.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Description;

		/// <summary>
		/// An enumerated result.
		/// </summary>
		[DataMember(IsRequired = true)]
		public VerificationResult Result;
	}
}
