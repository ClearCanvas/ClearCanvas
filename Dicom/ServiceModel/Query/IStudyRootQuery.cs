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
using System.ServiceModel;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// Service contract modelling a Dicom Hierarchical Study Root Query.
	/// </summary>
	[ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName="IStudyRootQuery" , Namespace = QueryNamespace.Value)]
	public interface IStudyRootQuery
	{
		/// <summary>
		/// Performs a STUDY level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria);

		/// <summary>
		/// Performs a SERIES level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria);

		/// <summary>
		/// Performs an IMAGE level query.
		/// </summary>
		/// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
		/// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria);
	}
}
