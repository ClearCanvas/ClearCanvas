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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class WorklistItemTextQueryRequest : TextQueryRequest
	{
		[DataContract]
		public class AdvancedSearchFields : DataContractBase
		{
			[DataMember]
			public string FamilyName;

			[DataMember]
			public string GivenName;

			[DataMember]
			public string Mrn;

			[DataMember]
			public string HealthcardNumber;

			[DataMember]
			public string AccessionNumber;

			[DataMember]
			public EntityRef DiagnosticServiceRef;

			[DataMember]
			public EntityRef ProcedureTypeRef;

			[DataMember]
			public EntityRef OrderingPractitionerRef;

			[DataMember]
			public DateTime? FromDate;

			[DataMember]
			public DateTime? UntilDate;

			/// <summary>
			/// Checks if all search fields are empty.
			/// </summary>
			/// <returns></returns>
			public bool IsEmpty()
			{
				return IsEmpty(FamilyName)
					   && IsEmpty(GivenName)
					   && IsEmpty(Mrn)
					   && IsEmpty(HealthcardNumber)
					   && IsEmpty(AccessionNumber)
					   && DiagnosticServiceRef == null
					   && ProcedureTypeRef == null
					   && OrderingPractitionerRef == null
					   && FromDate == null
					   && UntilDate == null;
			}

			/// <summary>
			/// Checks if non-patient search fields are emtpy.
			/// </summary>
			/// <returns></returns>
			public bool IsNonPatientFieldsEmpty()
			{
				return IsEmpty(AccessionNumber)
					   && DiagnosticServiceRef == null
					   && ProcedureTypeRef == null
					   && OrderingPractitionerRef == null
					   && FromDate == null
					   && UntilDate == null;
			}

			private static bool IsEmpty(string s)
			{
				return s == null || s.Trim().Length == 0;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public WorklistItemTextQueryRequest()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textQuery"></param>
		/// <param name="specificityThreshold"></param>
		/// <param name="procedureStepClassName"></param>
		/// <param name="options"></param>
		public WorklistItemTextQueryRequest(string textQuery, int specificityThreshold, string procedureStepClassName, WorklistItemTextQueryOptions options)
			: base(textQuery, specificityThreshold)
		{
			ProcedureStepClassName = procedureStepClassName;
			Options = options;
		}

		/// <summary>
		/// Name of the procedure step class of interest.
		/// </summary>
		[DataMember]
		public string ProcedureStepClassName;

		/// <summary>
		/// Specifies options that affect how the search is executed.
		/// </summary>
		[DataMember]
		public WorklistItemTextQueryOptions Options;

		/// <summary>
		/// Specifies that "advanced" mode should be used, in which case the text query is ignored
		/// and the search is based on the content of the <see cref="SearchFields"/> member.
		/// </summary>
		[DataMember]
		public bool UseAdvancedSearch;

		/// <summary>
		/// Data used in the advanced search mode.
		/// </summary>
		[DataMember]
		public AdvancedSearchFields SearchFields;
	}
}
