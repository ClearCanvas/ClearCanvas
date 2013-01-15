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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.PatientAdmin
{
	class PatientAdminServiceRecorder
	{
		static class Operations
		{
			public const string New = "Patient:New";
			public const string OpenForUpdate = "PatientProfile:OpenForUpdate";
			public const string Update = "PatientProfile:Update";
		}

		internal class AddPatient : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var response = (AddPatientResponse)recorderContext.Response;
				var patientProfile = persistenceContext.Load<PatientProfile>(response.PatientProfile.PatientProfileRef, EntityLoadFlags.None);

				return new OperationData(Operations.New, patientProfile);
			}
		}

		internal class LoadPatientProfileForEdit : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var response = (LoadPatientProfileForEditResponse)recorderContext.Response;
				var patientProfile = persistenceContext.Load<PatientProfile>(response.PatientProfileRef, EntityLoadFlags.None);

				return new OperationData(Operations.OpenForUpdate, patientProfile);
			}
		}

		internal class UpdatePatientProfile : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var response = (UpdatePatientProfileResponse)recorderContext.Response;
				var patientProfile = persistenceContext.Load<PatientProfile>(response.PatientProfile.PatientProfileRef, EntityLoadFlags.None);

				IncludeChangeSetFor(patientProfile);
				IncludeChangeSetFor(patientProfile.Patient);

				return new OperationData(Operations.Update, patientProfile);
			}
		}
	}
}
