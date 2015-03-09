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

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	[DataContract(Namespace = QueryNamespace.Value)]
	public abstract class QueryRequest : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public string LocalApplicationEntity { get; set; }

		[DataMember(IsRequired = true)]
		public ApplicationEntity RemoteApplicationEntity { get; set; }

		/// <summary>
		/// If set, the maximum number of return results expected.
		/// </summary>
		[DataMember(IsRequired = false)]
		public int? MaxResults { get; set; }

		/// <summary>
		/// Gets or sets whether or not failures are ignored if any results were returned.
		/// </summary>
		[DataMember(IsRequired = false)]
		public bool IgnoreFailureOnPartialResults { get; set; }
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public abstract class QueryResponse : DataContractBase
	{
		[DataMember(IsRequired = false)]
		public bool MaxResultsReached { get; set; }
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class StudyQueryRequest : QueryRequest
	{
		[DataMember(IsRequired = true)]
		public StudyRootStudyIdentifier Criteria { get; set; }
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class StudyQueryResponse : QueryResponse
	{
		[DataMember(IsRequired = true)]
		public List<StudyRootStudyIdentifier> Results { get; set; }
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class SeriesQueryRequest : QueryRequest
	{
		[DataMember(IsRequired = true)]
		public SeriesIdentifier Criteria { get; set; }
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class SeriesQueryResponse : QueryResponse
	{
		[DataMember(IsRequired = true)]
		public List<SeriesIdentifier> Results { get; set; }
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class ImageQueryRequest : QueryRequest
	{
		[DataMember(IsRequired = true)]
		public ImageIdentifier Criteria { get; set; }
	}

	[DataContract(Namespace = QueryNamespace.Value)]
	public class ImageQueryResponse : QueryResponse
	{
		[DataMember(IsRequired = true)]
		public List<ImageIdentifier> Results { get; set; }
	}

	/// <summary>
	/// A contract for performing Study Root DICOM Queries
	/// </summary>
	[ServiceContract(SessionMode = SessionMode.Allowed, Namespace = QueryNamespace.Value)]
	public interface IStudyRootQueryApplication
	{
		/// <summary>
		/// Performs a STUDY level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		[FaultContract(typeof (DataValidationFault))]
		[FaultContract(typeof (QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		StudyQueryResponse StudyQuery(StudyQueryRequest request);

		/// <summary>
		/// Performs a SERIES level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		[FaultContract(typeof (DataValidationFault))]
		[FaultContract(typeof (QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		SeriesQueryResponse SeriesQuery(SeriesQueryRequest request);

		/// <summary>
		/// Performs an IMAGE level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		[FaultContract(typeof (DataValidationFault))]
		[FaultContract(typeof (QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		ImageQueryResponse ImageQuery(ImageQueryRequest request);
	}
}