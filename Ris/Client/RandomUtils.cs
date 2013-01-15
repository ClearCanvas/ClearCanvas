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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Helper class for generating random patients, orders, visits, etc.
	/// </summary>
	/// <remarks>
	/// Static members of this class are safe for use by multiple threads.
	/// </remarks>
	public static class RandomUtils
	{
		// do not use this member directly even within this class - use the static methods instead
		private static readonly Random _random = new Random(Platform.Time.Millisecond);

		private static LoadPatientProfileEditorFormDataResponse _patientEditorFormData;
		private static LoadVisitEditorFormDataResponse _visitEditorFormData;
		private static GetOrderEntryFormDataResponse _orderEntryFormData;
		private static List<DiagnosticServiceSummary> _diagnosticServices;
		private static List<ExternalPractitionerSummary> _practitioners;
		private static bool _refDataCachedInitialized ;
		private static readonly object _syncLock = new object();

		#region Basic Utilities

		public static int GetRandomInteger(int min, int max)
		{
			// lock is required because Random.Next is not thread-safe
			lock(_random)
			{
				return _random.Next(min, max);
			}
		}

		public static char GetRandomAlphaChar()
		{
			return Convert.ToChar(Convert.ToInt32(GetRandomInteger(0, 25) + 65));
		}

		public static string GenerateRandomIntegerString(int length)
		{
			var builder = new StringBuilder();

			for (var i = 0; i < length; i++)
			{
				builder.Append(GetRandomInteger(0, 9).ToString());
			}

			return builder.ToString();
		}

		public static string GenerateRandomString(int stringLength)
		{
			var builder = new StringBuilder();

			for (var i = 0; i < stringLength; i++)
			{
				builder.Append(GetRandomAlphaChar());
			}

			return builder.ToString();
		}

		public static TItem ChooseRandom<TItem>(IList<TItem> target)
		{
			if (target.Count == 0)
				return default(TItem);

			if (target.Count == 1)
				return target[0];

			var randomIndex = GetRandomInteger(0, target.Count - 1);
			return target[randomIndex];
		}

		private static string GetRandomNameFromFile(string file)
		{
			var nameList = new List<string>();

			using (TextReader reader = new StringReader(file))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					nameList.Add(line);
				}
			}

			return ChooseRandom(nameList);
		}

		#endregion

		/// <summary>
		/// Return a list of cached diagnostic services.  This may not be the entire list in the database.
		/// </summary>
		public static List<DiagnosticServiceSummary> diagnosticServices
		{
			get
			{
				InitReferenceDataCacheOnce();
				return _diagnosticServices;
			}
		}

		// JR: this method won't compile anymore since the reconciliation service was moved to the Extended plugins... do we really need it?
		///// <summary>
		///// Creates a new patient with multiple profiles.
		///// </summary>
		///// <returns></returns>
		//public static PatientProfileSummary CreatePatient(String[] InfoAuth)
		//{
		//    var result=new List<PatientProfileSummary>();

		//    if (InfoAuth.Length==0)
		//        result.Add(CreatePatient());
		//    else
		//    {
		//        var timespan = new TimeSpan(GetRandomInteger(0,100 * 365 * 24), 0, 0);
		//        InitReferenceDataCacheOnce();
		//        var birthDate = Platform.Time - timespan;

		//        var profile = new PatientProfileDetail
		//            {
		//                Healthcard = new HealthcardDetail(
		//                    GenerateRandomIntegerString(10),
		//                    ChooseRandom(_patientEditorFormData.HealthcardAssigningAuthorityChoices),
		//                    "", null),
		//                DateOfBirth = birthDate,
		//                Sex = ChooseRandom(_patientEditorFormData.SexChoices),
		//                PrimaryLanguage = ChooseRandom(_patientEditorFormData.PrimaryLanguageChoices),
		//                Religion = ChooseRandom(_patientEditorFormData.ReligionChoices),
		//                DeathIndicator = false,
		//                TimeOfDeath = null
		//            };

		//        profile.Name = new PersonNameDetail
		//            {
		//                FamilyName = GetRandomNameFromFile(RandomUtilsSettings.Default.FamilyNameDictionary),
		//                GivenName = profile.Sex.Code == "F"
		//                    ? GetRandomNameFromFile(RandomUtilsSettings.Default.FemaleNameDictionary) + " Anonymous"
		//                    : GetRandomNameFromFile(RandomUtilsSettings.Default.MaleNameDictionary) + " Anonymous"
		//            };

		//        AddPatientResponse addResponse = null;

		//        for(var i=0; i<InfoAuth.Length; i++)
		//        {
		//            profile.Mrn = new CompositeIdentifierDetail(
		//                GenerateRandomIntegerString(10),
		//                CollectionUtils.SelectFirst(_patientEditorFormData.MrnAssigningAuthorityChoices, MAAC => MAAC.Code == InfoAuth[i]));

		//            Platform.GetService(
		//                delegate(IPatientAdminService service)
		//                {
		//                    addResponse = service.AddPatient(new AddPatientRequest(profile));
		//                });

		//            result.Add(addResponse.PatientProfile);
		//        }

		//        if (InfoAuth.Length > 1)
		//        {
		//            //reconcile patients
		//            var checkedPatients = new List<EntityRef>();
		//            foreach (var pps in result)
		//            {
		//                checkedPatients.Add(pps.PatientRef);
		//            }

		//            // reconcile
		//            Platform.GetService<IPatientReconciliationService>(
		//                service => service.ReconcilePatients(new ReconcilePatientsRequest(checkedPatients)));
		//        }
		//    }

		//    return result[0];
		//}

		/// <summary>
		/// Creates a new patient with a single profile.
		/// </summary>
		/// <returns></returns>
		public static PatientProfileSummary CreatePatient()
		{
			var timespan = new TimeSpan(GetRandomInteger(0,100 * 365 * 24), 0, 0);
			InitReferenceDataCacheOnce();
			var birthDate = Platform.Time - timespan;

			var profile = new PatientProfileDetail
				{
					Mrn = new CompositeIdentifierDetail(
						GenerateRandomIntegerString(10),
						ChooseRandom(_patientEditorFormData.MrnAssigningAuthorityChoices)),
					Healthcard = new HealthcardDetail(
						GenerateRandomIntegerString(10),
						ChooseRandom(_patientEditorFormData.HealthcardAssigningAuthorityChoices),
						"", null),
					DateOfBirth = birthDate,
					Sex = ChooseRandom(_patientEditorFormData.SexChoices),
					PrimaryLanguage = ChooseRandom(_patientEditorFormData.PrimaryLanguageChoices),
					Religion = ChooseRandom(_patientEditorFormData.ReligionChoices),
					DeathIndicator = false,
					TimeOfDeath = null
				};

			// Randomly create 0-2 allergies
			for (var i = 0; i < GetRandomInteger(0, 2); i++)
			{
				var allergy = new PatientAllergyDetail(
					ChooseRandom(_patientEditorFormData.AllergenTypeChoices),
					string.Format("description {0}", GenerateRandomString(3)),
					ChooseRandom(_patientEditorFormData.AllergySeverityChoices),
					string.Format("reaction {0}", GenerateRandomString(3)),
					ChooseRandom(_patientEditorFormData.AllergySensitivityTypeChoices),
					Platform.Time,
					Platform.Time,
					new PersonNameDetail
						{
							FamilyName = GetRandomNameFromFile(RandomUtilsSettings.Default.FamilyNameDictionary),
							GivenName = GetRandomNameFromFile(RandomUtilsSettings.Default.MaleNameDictionary) + " Anonymous"
						},
					ChooseRandom(_patientEditorFormData.PersonRelationshipTypeChoices));

				profile.Allergies.Add(allergy);
			}

			profile.Name = new PersonNameDetail
			{
				FamilyName = GetRandomNameFromFile(RandomUtilsSettings.Default.FamilyNameDictionary),
				GivenName = profile.Sex.Code == "F"
					? GetRandomNameFromFile(RandomUtilsSettings.Default.FemaleNameDictionary) + " Anonymous"
					: GetRandomNameFromFile(RandomUtilsSettings.Default.MaleNameDictionary) + " Anonymous"
			};

			AddPatientResponse addResponse = null;

			Platform.GetService(
				delegate(IPatientAdminService service)
				{
					addResponse = service.AddPatient(new AddPatientRequest(profile));
				});

			return addResponse.PatientProfile;
		}

		/// <summary>
		/// Creates a new visit for the specified patient.
		/// </summary>
		/// <param name="patientRef">Patient for which the visit is created.</param>
		/// <param name="informationAuthority">Information authority to use for the visit number.</param>
		/// <param name="admitOffsetDays">A positive or negative number of days from today.</param>
		/// <returns></returns>
		public static VisitSummary CreateVisit(PatientProfileSummary patientProfile, EnumValueInfo informationAuthority, int admitOffsetDays)
		{
			return CreateVisit(patientProfile, informationAuthority, admitOffsetDays, null);
		}

		/// <summary>
		/// Creates a new visit for the specified patient.
		/// </summary>
		/// <param name="patientRef">Patient for which the visit is created.</param>
		/// <param name="informationAuthority">Information authority to use for the visit number.</param>
		/// <param name="admitOffsetDays">A positive or negative number of days from today.</param>
		/// <param name="AdmissionType">Emergency or other types</param>
		/// <returns></returns>
		public static VisitSummary CreateVisit(PatientProfileSummary patientProfile, EnumValueInfo informationAuthority, int admitOffsetDays, EnumValueInfo AdmissionType)
		{
			InitReferenceDataCacheOnce();

			// Generate an active visit with randomize properties

			var now = Platform.Time;
			const string activeAdmittedVisitStatus = "AA";
			var visitDetail = new VisitDetail
				{
					Patient = patientProfile,
					VisitNumber = new CompositeIdentifierDetail(GenerateRandomIntegerString(10), informationAuthority),
					PatientClass = ChooseRandom(_visitEditorFormData.PatientClassChoices),
					PatientType = ChooseRandom(_visitEditorFormData.PatientTypeChoices),
					AdmissionType = AdmissionType ?? ChooseRandom(_visitEditorFormData.AdmissionTypeChoices),
					Status = CollectionUtils.SelectFirst(_visitEditorFormData.VisitStatusChoices, enumValue => enumValue.Code == activeAdmittedVisitStatus),
					AdmitTime = now + TimeSpan.FromDays(admitOffsetDays),
					Facility = ChooseRandom(_visitEditorFormData.FacilityChoices)
				};

			VisitSummary visit = null;
			Platform.GetService(
				delegate(IVisitAdminService service)
				{
					var addVisitResponse = service.AddVisit(new AddVisitRequest(visitDetail));
					visit = addVisitResponse.Visit;
				});

			return visit;
		}

		/// <summary>
		/// Create a random order on the specified visit.
		/// </summary>
		/// <param name="visit">Visit/patient for which the order is created.</param>
		/// <param name="informationAuthority">Performing facility will be selected to match this information authority.</param>
		/// <param name="diagnosticServiceName">Name of the diagnostic service to order.</param>
		/// <param name="schedulingOffsetDays">A positive or negative number of days from today.</param>
		/// <returns></returns>
		public static OrderSummary RandomOrder(VisitSummary visit, EnumValueInfo informationAuthority, string diagnosticServiceName, int schedulingOffsetDays)
		{
			return RandomOrder(visit, informationAuthority, diagnosticServiceName, schedulingOffsetDays,"", null, null);
		}

		/// <summary>
		/// Create a random order on the specified visit.
		/// </summary>
		/// <param name="visit">Visit/patient for which the order is created.</param>
		/// <param name="informationAuthority">Performing facility will be selected to match this information authority.</param>
		/// <param name="diagnosticServiceName">Name of the diagnostic service to order.</param>
		/// <param name="schedulingOffsetDays">A positive or negative number of days from today.</param>
		/// <param name="modalityName">The name of the modality.</param>
		/// <param name="facilityCode">The code of the ordering and performing facility.</param>
		/// <param name="laterality">The procedure laterality.</param>
		/// <returns></returns>
		public static OrderSummary RandomOrder(VisitSummary visit, EnumValueInfo informationAuthority, string diagnosticServiceName, int schedulingOffsetDays, string modalityName, string facilityCode, EnumValueInfo laterality)
		{
			InitReferenceDataCacheOnce();

			var scheduledTime = Platform.Time + TimeSpan.FromDays(schedulingOffsetDays);
			LoadDiagnosticServicePlanResponse dsResponse=null;
			OrderSummary orderSummary = null;
			Platform.GetService(
				delegate(IOrderEntryService service)
				{
					DiagnosticServiceSummary diagnosticService;
					if (String.IsNullOrEmpty(diagnosticServiceName) && String.IsNullOrEmpty(modalityName))
					{
						diagnosticService = ChooseRandom(_diagnosticServices);
						dsResponse = service.LoadDiagnosticServicePlan(new LoadDiagnosticServicePlanRequest(diagnosticService.DiagnosticServiceRef));
					}
					else
					{
						diagnosticService = CollectionUtils.SelectFirst(_diagnosticServices,
							delegate(DiagnosticServiceSummary ds)
							{
								dsResponse = service.LoadDiagnosticServicePlan(new LoadDiagnosticServicePlanRequest(ds.DiagnosticServiceRef));
								return (ds.Name == diagnosticServiceName) ||
									(!String.IsNullOrEmpty(modalityName) && (CollectionUtils.SelectFirst(dsResponse.DiagnosticServicePlan.ProcedureTypes, 
																				ptd => ptd.Name.IndexOf(modalityName) == 0) != null));
							});

						if (diagnosticService == null)
							throw new Exception(String.Format("Cannot find diagnostic service with name {0}", diagnosticServiceName));
					}

					var performingFacility = CollectionUtils.SelectFirst(_orderEntryFormData.FacilityChoices,
						delegate(FacilitySummary facility)
							{
								if (facilityCode == null)
									return facility.InformationAuthority.Code == informationAuthority.Code;
								
								return facility.Code == facilityCode;
							});

					var randomPhysician = ChooseRandom(_practitioners);
					var randomPriority = ChooseRandom(_orderEntryFormData.OrderPriorityChoices);

					var requisition = new OrderRequisition
						{
							Patient = visit.Patient,
							Visit = visit,
							DiagnosticService = diagnosticService,
							OrderingPractitioner = randomPhysician,
							OrderingFacility = performingFacility,
							Priority = randomPriority,
							ReasonForStudy = "Randomly generated test order",
							SchedulingRequestTime = scheduledTime,
							Procedures = new List<ProcedureRequisition>(),
							ResultRecipients = new List<ResultRecipientDetail>(),
							Attachments = new List<AttachmentSummary>(),
							Notes = new List<OrderNoteDetail>()
						};

					requisition.Procedures.AddRange(
						CollectionUtils.Map(dsResponse.DiagnosticServicePlan.ProcedureTypes,
							(ProcedureTypeSummary rpt) => new ProcedureRequisition(rpt, performingFacility)
															{
																ScheduledTime = scheduledTime, 
																Laterality = laterality,
																ScheduledDuration = 30 // default to a non-zero scheduled duration
															}));

					var response = service.PlaceOrder(new PlaceOrderRequest(requisition));

					orderSummary = response.Order;
				});

			return orderSummary;
		}

		private static void InitReferenceDataCacheOnce()
		{
			if (_refDataCachedInitialized)
				return;

			lock (_syncLock)
			{
				if (_refDataCachedInitialized)
					return;

				Platform.GetService(
					delegate(IPatientAdminService service)
					{
						_patientEditorFormData = service.LoadPatientProfileEditorFormData(new LoadPatientProfileEditorFormDataRequest());
					});

				Platform.GetService(
					delegate(IVisitAdminService service)
					{
						_visitEditorFormData = service.LoadVisitEditorFormData(new LoadVisitEditorFormDataRequest());
					});

				Platform.GetService(
					delegate(IOrderEntryService service)
					{
						_orderEntryFormData = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest());
					});

				// cache up to 1000 diagnostic services
				Platform.GetService(
					delegate(IDiagnosticServiceAdminService service)
					{
						var request = new ListDiagnosticServicesRequest { Page = { FirstRow = 0, MaxRows = 1000 } };
						_diagnosticServices = service.ListDiagnosticServices(request).DiagnosticServices;
					});

				// cache up to 500 practitioners
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new ListExternalPractitionersRequest { Page = { FirstRow = 0, MaxRows = 500 }, QueryItems = true };
						_practitioners = service.ListExternalPractitioners(request).Practitioners;
					});

				_refDataCachedInitialized = true;
			}
		}
	}
}
