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
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation;
using ClearCanvas.Ris.Application.Services;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Extended.Services.PatientReconciliation
{
	[ServiceImplementsContract(typeof(IPatientReconciliationService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class PatientReconciliationService : ApplicationServiceBase, IPatientReconciliationService
	{
		#region IPatientReconciliationService Members

		[ReadOperation]
		public ListPatientReconciliationMatchesResponse ListPatientReconciliationMatches(ListPatientReconciliationMatchesRequest request)
		{
			var targetProfile = this.PersistenceContext.GetBroker<IPatientProfileBroker>().Load(request.PatientProfileRef);

			var strategy = (IPatientReconciliationStrategy)(new PatientReconciliationStrategyExtensionPoint()).CreateExtension();
			var matches = strategy.FindReconciliationMatches(targetProfile, this.PersistenceContext);

			var profileAssembler = new PatientProfileAssembler();
			var rcAssembler = new ReconciliationCandidateAssembler();
			var response = new ListPatientReconciliationMatchesResponse
				{
					ReconciledProfiles =
						CollectionUtils.Map<PatientProfile, PatientProfileSummary, List<PatientProfileSummary>>(
						targetProfile.Patient.Profiles,
						profile => profileAssembler.CreatePatientProfileSummary(profile, this.PersistenceContext)),
					MatchCandidates =
						CollectionUtils.Map<PatientProfileMatch, ReconciliationCandidate, List<ReconciliationCandidate>>(
						matches,
						match => rcAssembler.CreateReconciliationCandidate(match, this.PersistenceContext))
				};

			return response;
		}

		[ReadOperation]
		public LoadPatientProfileDiffResponse LoadPatientProfileDiff(LoadPatientProfileDiffRequest request)
		{
			var broker = this.PersistenceContext.GetBroker<IPatientProfileBroker>();

			// load profiles to compare
			var leftProfile = broker.Load(request.LeftProfileRef);
			var rightProfile = broker.Load(request.RightProfileRef);

			// ask model to compute discrepancies
			var results = PatientProfileDiscrepancyTest.GetDiscrepancies(leftProfile, rightProfile, PatientProfileDiscrepancy.All);

			// build response
			var assembler = new PatientProfileDiffAssembler();
			var diff = assembler.CreatePatientProfileDiff(leftProfile, rightProfile, results);

			return new LoadPatientProfileDiffResponse(diff);
		}

		[ReadOperation]
		public ListProfilesForPatientsResponse ListProfilesForPatients(ListProfilesForPatientsRequest request)
		{
			var assembler = new PatientProfileAssembler();
			var summaries = new List<PatientProfileSummary>();
			foreach (var patientRef in request.PatientRefs)
			{
				var patient = this.PersistenceContext.Load<Patient>(patientRef);
				foreach (var profile in patient.Profiles)
				{
					summaries.Add(assembler.CreatePatientProfileSummary(profile, this.PersistenceContext));
				}
			}

			return new ListProfilesForPatientsResponse(summaries);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = Common.AuthorityTokens.Workflow.Patient.Reconcile)]
		public ReconcilePatientsResponse ReconcilePatients(ReconcilePatientsRequest request)
		{
			var patients = CollectionUtils.Map<EntityRef, Patient, List<Patient>>(
				request.PatientRefs,
				patientRef => this.PersistenceContext.Load<Patient>(patientRef, EntityLoadFlags.CheckVersion));

			if (patients.Count < 2)
				throw new RequestValidationException(SR.ExceptionReconciliationRequiresAtLeast2Patients);

			var op = new Operations.ReconcilePatient();
			op.Execute(patients, new PersistentWorkflow(this.PersistenceContext));

			return new ReconcilePatientsResponse();
		}

		#endregion
	}
}
