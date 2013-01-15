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
using System.Linq;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Validation rules for <see cref="Order"/> and <see cref="Procedure"/> entities.
	/// </summary>
	internal static class OrderRules
	{
		internal static TestResult VisitAndPerformingFacilitiesHaveSameInformationAuthority(Order order)
		{
			// all non-defunct procedures for the order must have the same performing information authority as the visit
			var hasSameInformationAuthority = CollectionUtils.TrueForAll(NonDefunctProcedures(order),
				p => Equals(p.PerformingFacility.InformationAuthority, p.Order.Visit.VisitNumber.AssigningAuthority));
			return new TestResult(hasSameInformationAuthority, SR.MessageValidateInformationAuthorityForVisitAndPerformingFacilities);
		}

		internal static TestResult AllNonDefunctProceduresHaveSamePerformingFacility(Order order)
		{
			// all non-defunct procedures for the order must have the same performing facility
			var procedures = NonDefunctProcedures(order);
			var facility = CollectionUtils.FirstElement(CollectionUtils.Map(procedures, (Procedure p) => p.PerformingFacility));
			var hasSameFacility = CollectionUtils.TrueForAll(procedures, p => Equals(p.PerformingFacility, facility));
			return new TestResult(hasSameFacility, SR.MessageValidateOrderPerformingFacilities);
		}

		internal static TestResult AllNonDefunctProceduresHaveSamePerformingDepartment(Order order)
		{
			// all non-defunct procedures for the order must have the same performing department
			var procedures = NonDefunctProcedures(order);
			var department = CollectionUtils.FirstElement(CollectionUtils.Map(procedures, (Procedure p) => p.PerformingDepartment));
			var hasSameDepartment = CollectionUtils.TrueForAll(procedures, p => Equals(p.PerformingDepartment, department));
			return new TestResult(hasSameDepartment, SR.MessageValidateOrderPerformingDepartments);
		}

		internal static TestResult PerformingDepartmentAlignsWithPerformingFacility(Procedure procedure)
		{
			// performing department must be associated with performing facility
			var performingDepartmentIsInPerformingFacility = procedure.PerformingDepartment == null
				|| procedure.PerformingFacility.Equals(procedure.PerformingDepartment.Facility);
			return new TestResult(performingDepartmentIsInPerformingFacility, SR.MessageValidateProcedurePerformingFacilityAndDepartment);
		}

		internal static TestResult ModalitiesAlignWithPerformingFacility(Procedure procedure)
		{
			// modality facilities must match performing facility
			var valid = procedure.ModalityProcedureSteps.All(
				mps => ModalityAlignsWithPerformingFacility(mps).Success);

			return new TestResult(valid, SR.MessageValidateProcedurePerformingFacilityAndModalities);
		}

		internal static TestResult ModalityAlignsWithPerformingFacility(ModalityProcedureStep mps)
		{
			// modality facility must match performing facility
			var valid = mps.Modality.Facility == null || mps.Modality.Facility.Equals(mps.Procedure.PerformingFacility);

			return new TestResult(valid, SR.MessageValidateProcedurePerformingFacilityAndModalities);
		}

		internal static TestResult PatientProfileExistsForPerformingFacility(Procedure procedure)
		{
			// patient must have a profile at the performing facility
			var patientProfileExists = procedure.PatientProfile != null;
			return new TestResult(patientProfileExists, SR.MessageValidateProcedurePatientProfile);
		}

		private static List<Procedure> NonDefunctProcedures(Order order)
		{
			return order.GetProcedures(p => !p.IsDefunct);
		}
	}
}
