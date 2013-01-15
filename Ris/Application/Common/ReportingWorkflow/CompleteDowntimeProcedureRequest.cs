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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class CompleteDowntimeProcedureRequest : DataContractBase
	{
		public CompleteDowntimeProcedureRequest(
			EntityRef procedureRef,
			bool reportProvided,
			Dictionary<string, string> reportPartExtendedProperties,
			EntityRef interpreterRef,
			EntityRef transcriptionistRef)
		{
			ProcedureRef = procedureRef;
			ReportProvided = reportProvided;
			ReportPartExtendedProperties = reportPartExtendedProperties;
			TranscriptionistRef = transcriptionistRef;
			InterpreterRef = interpreterRef;
		}

		/// <summary>
		/// Reference to the procedure complete downtime for.
		/// </summary>
		[DataMember]
		public EntityRef ProcedureRef;

		/// <summary>
		/// Indicates whether a transcribed report has already been provided.
		/// </summary>
		[DataMember]
		public bool ReportProvided;

		/// <summary>
		/// Transcribed report data, if provided.
		/// </summary>
		[DataMember]
		public Dictionary<string, string> ReportPartExtendedProperties;

		/// <summary>
		/// Interpreter of the transcribed report, if provided.
		/// </summary>
		[DataMember]
		public EntityRef InterpreterRef;

		/// <summary>
		/// Transcriptionist that transcribed the report, if provided. Optional.
		/// </summary>
		[DataMember]
		public EntityRef TranscriptionistRef;

	}
}
