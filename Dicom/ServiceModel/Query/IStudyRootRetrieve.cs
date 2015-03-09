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

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	[DataContract(Namespace = QueryNamespace.Value)]
	public class RetrieveRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public string LocalApplicationEntity { get; set; }

		[DataMember(IsRequired = true)]
		public ApplicationEntity RemoteApplicationEntity { get; set; }

		[DataMember(IsRequired = true)]
		public string MoveDestination { get; set; }
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class StudyRetrieveRequest : RetrieveRequest
	{
		[DataMember(IsRequired = true)]
		public string[] StudyInstanceUid { get; set; }
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class StudyRetrieveResponse : DataContractBase
	{
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class SeriesRetrieveRequest : RetrieveRequest
	{
		[DataMember(IsRequired = true)]
		public string StudyInstanceUid { get; set; }
		
		[DataMember(IsRequired = true)]
		public string[] SeriesInstanceUid { get; set; }
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class SeriesRetrieveResponse : DataContractBase
	{
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class ImageRetrieveRequest : RetrieveRequest
	{
		[DataMember(IsRequired = true)]
		public string StudyInstanceUid { get; set; }
		
		[DataMember(IsRequired = true)]
		public string SeriesInstanceUid { get; set; }
		
		[DataMember(IsRequired = true)]
		public string[] SopInstanceUid { get; set; }
	}
	
	[DataContract(Namespace = QueryNamespace.Value)]
	public class ImageRetrieveResponse : DataContractBase
	{
	}

	[ServiceContract(SessionMode = SessionMode.Allowed, Namespace = QueryNamespace.Value)]
	public interface IStudyRootRetrieve
	{
		/// <summary>
		/// Performs a STUDY level retrieve.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the retrieve fails.</exception>
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(RetrieveFailedFault))]
		[OperationContract(IsOneWay = false)]
		StudyRetrieveResponse StudyRetrieve(StudyRetrieveRequest request);

		/// <summary>
		/// Performs a SERIES level retrieve.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the retrieve fails.</exception>
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(RetrieveFailedFault))]
		[OperationContract(IsOneWay = false)]
		SeriesRetrieveResponse SeriesRetrieve(SeriesRetrieveRequest request);

		/// <summary>
		/// Performs an IMAGE level retrieve.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the retrieve fails.</exception>
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(RetrieveFailedFault))]
		[OperationContract(IsOneWay = false)]
		ImageRetrieveResponse ImageRetrieve(ImageRetrieveRequest request);
	}
}
