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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Defines a worklist item projection, which is a set of <see cref="WorklistItemField"/>s
	/// that shape the result set of a worklist item query.
	/// </summary>
	/// <remarks>
	/// Instances of this class are immutable.
	/// </remarks>
	public class WorklistItemProjection
	{
		#region Private Static

		private static readonly WorklistItemProjection _procedureStepBase;
		private static readonly WorklistItemProjection _reportingBase;

		/// <summary>
		/// Class constructor.
		/// </summary>
		static WorklistItemProjection()
		{
			// define the base procedure step projection (without the time field)
			_procedureStepBase = new WorklistItemProjection(
				new [] {
					WorklistItemField.ProcedureStep,
					WorklistItemField.ProcedureStepName,
					WorklistItemField.ProcedureStepState,
                    WorklistItemField.Procedure,
                    WorklistItemField.Order,
                    WorklistItemField.Patient,
                    WorklistItemField.PatientProfile,
                    WorklistItemField.Mrn,
                    WorklistItemField.PatientName,
                    WorklistItemField.AccessionNumber,
                    WorklistItemField.Priority,
                    WorklistItemField.PatientClass,
                    WorklistItemField.DiagnosticServiceName,
                    WorklistItemField.ProcedureTypeName,
					WorklistItemField.ProcedurePortable,
					WorklistItemField.ProcedureLaterality,
				});

			// define the base reporting procedure step projection
			_reportingBase = _procedureStepBase.AddFields(
				new [] {
					WorklistItemField.Report,
					WorklistItemField.ReportPartIndex
				});

			// initialize the "search" projections for each type of worklist

			ModalityWorklistSearch = GetDefaultProjection(WorklistItemField.ProcedureScheduledStartTime);

			// need to display the correct time field
			// ProcedureScheduledStartTime seems like a reasonable choice for registration homepage search,
			// as it gives a general sense of when the procedure occurs in time
			RegistrationWorklistSearch = GetDefaultProjection(WorklistItemField.ProcedureScheduledStartTime);

			ReportingWorklistSearch = GetReportingProjection(WorklistItemField.ProcedureStartTime);

			// TODO: this timefield is the value from when this broker was part of ReportingWorklistBroker,
			// but what should it really be?  
			ProtocolWorklistSearch = GetDefaultProjection(WorklistItemField.ProcedureStartTime);

		}

		#endregion

		/// <summary>
		/// Defines the projection for a modality worklist item search.
		/// </summary>
		public static readonly WorklistItemProjection ModalityWorklistSearch;

		/// <summary>
		/// Defines the projection for a registration worklist item search.
		/// </summary>
		public static readonly WorklistItemProjection RegistrationWorklistSearch;


		/// <summary>
		/// Defines the projection for a reporting worklist item search.
		/// </summary>
		public static readonly WorklistItemProjection ReportingWorklistSearch;

		/// <summary>
		/// Defines the projection for a protocol worklist item search.
		/// </summary>
		public static readonly WorklistItemProjection ProtocolWorklistSearch;


		/// <summary>
		/// Gets the default worklist item projection using the specified time field.
		/// </summary>
		/// <param name="timeField"></param>
		/// <returns></returns>
		public static WorklistItemProjection GetDefaultProjection(WorklistItemField timeField)
		{
			return _procedureStepBase.AddFields(new[] { timeField });
		}

		/// <summary>
		/// Gets the reporting worklist item projection using the specified time field.
		/// </summary>
		/// <param name="timeField"></param>
		/// <returns></returns>
		public static WorklistItemProjection GetReportingProjection(WorklistItemField timeField)
		{
			return _reportingBase.AddFields(new[] { timeField });
		}

		private readonly List<WorklistItemField> _fields;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fields"></param>
		public WorklistItemProjection(IEnumerable<WorklistItemField> fields)
		{
			_fields = new List<WorklistItemField>(fields);
		}

		/// <summary>
		/// Get a read-only list of the fields specified in this projection.
		/// </summary>
		public IList<WorklistItemField> Fields
		{
			get { return _fields.AsReadOnly(); }
		}

		/// <summary>
		/// Returns a new projection which contains the specified fields appended to the fields of this projection.
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public WorklistItemProjection AddFields(IList<WorklistItemField> fields)
		{
			var copy = new List<WorklistItemField>(_fields);
			copy.AddRange(fields);
			return new WorklistItemProjection(copy);
		}

		/// <summary>
		/// Returns a new projection which contains only the fields in this projection that satisfy the specified filter function.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public WorklistItemProjection Filter(Predicate<WorklistItemField> filter)
		{
			var filtered = CollectionUtils.Select(_fields, filter);
			return new WorklistItemProjection(filtered);
		}
		
	}
}
