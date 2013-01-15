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

using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class WorkflowServiceRecorder
	{
		static class Operations
		{
			public const string WorklistSearch = "Worklist:Search";
			public const string PatientProfileSearch = "PatientProfile:Search";
		}

		internal abstract class SearchOperationRecorderBase : RisServiceOperationRecorderBase
		{
			[DataContract]
			protected class SearchOperationData : OperationData
			{
				public SearchOperationData(string operation, string queryString)
					: base(operation)
				{
					this.SearchString = queryString;
				}

				public SearchOperationData(string operation, object queryParameters)
					: base(operation)
				{
					this.SearchParameters = queryParameters;
				}

				[DataMember]
				public string SearchString;

				[DataMember]
				public object SearchParameters;
			}
		}

		internal class SearchWorklists : SearchOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (WorklistItemTextQueryRequest) recorderContext.Request;
				return request.UseAdvancedSearch ?
					new SearchOperationData(Operations.WorklistSearch, request.SearchFields)
					: new SearchOperationData(Operations.WorklistSearch, request.TextQuery);
			}
		}

		internal class SearchPatientProfiles : SearchOperationRecorderBase
		{

			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				return new SearchOperationData(Operations.PatientProfileSearch, ((TextQueryRequest)recorderContext.Request).TextQuery);
			}
		}
	}
}
