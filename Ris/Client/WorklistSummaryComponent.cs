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

using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
	public class WorklistSummaryComponent : DHtmlComponent
	{
		[DataContract]
		internal class WorklistSummaryContext : DataContractBase
		{
			private readonly WorklistAdminDetail _worklist;
			private readonly bool _isAdmin;

			private bool _hasMultipleWorklists;
			private List<string> _worklistNames;
			private List<string> _worklistDescriptions;
			private List<WorklistClassSummary> _worklistClasses;

			public WorklistSummaryContext(WorklistAdminDetail worklist, bool isAdmin)
			{
				_worklist = worklist;
				_isAdmin = isAdmin;
			}

			[DataMember]
			public bool IsAdmin
			{
				get { return _isAdmin; }
			}

			[DataMember]
			public WorklistAdminDetail Worklist
			{
				get { return _worklist; }
			}

			[DataMember]
			public bool HasMultipleWorklists
			{
				get { return _hasMultipleWorklists; }
				set { _hasMultipleWorklists = value; }
			}

			[DataMember]
			public List<string> WorklistNames
			{
				get { return _worklistNames; }
				set { _worklistNames = value; }
			}

			[DataMember]
			public List<string> WorklistDescriptions
			{
				get { return _worklistDescriptions; }
				set { _worklistDescriptions = value; }
			}

			[DataMember]
			public List<WorklistClassSummary> WorklistClasses
			{
				get { return _worklistClasses; }
				set { _worklistClasses = value; }
			}
		}

		private readonly WorklistSummaryContext _context;

		public WorklistSummaryComponent(WorklistAdminDetail worklist, bool isAdmin)
		{
			_context = new WorklistSummaryContext(worklist, isAdmin);
		}

		public override void Start()
		{
			this.SetUrl(WebResourcesSettings.Default.WorklistSummaryPageUrl);
			base.Start();
		}

		public void SetMultipleWorklistInfo(List<string> names, List<string> descriptions, List<WorklistClassSummary> classes)
		{
			_context.HasMultipleWorklists = true;
			_context.WorklistNames = names;
			_context.WorklistDescriptions = descriptions;
			_context.WorklistClasses = classes;
		}

		public void Refresh()
		{
			NotifyAllPropertiesChanged();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}
	}
}
