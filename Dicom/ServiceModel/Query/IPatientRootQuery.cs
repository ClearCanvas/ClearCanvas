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
	//NOTE: internal for now because we don't actually implement it anywhere.

	[ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = QueryNamespace.Value)]
	internal interface IPatientRootQuery
	{
		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<PatientRootPatientIdentifier> PatientQuery(PatientRootPatientIdentifier queryCriteria);

		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<PatientRootStudyIdentifier> StudyQuery(PatientRootStudyIdentifier queryCriteria);

		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria);

		[FaultContract(typeof(DataValidationFault))]
		[FaultContract(typeof(QueryFailedFault))]
		[OperationContract(IsOneWay = false)]
		IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria);
	}
}
