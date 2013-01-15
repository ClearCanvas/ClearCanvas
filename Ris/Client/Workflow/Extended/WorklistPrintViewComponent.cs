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
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public class WorklistPrintViewComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class PrintContext : DataContractBase
		{
			public PrintContext(string folderSystemName, string folderName, string folderDescription, int totalItemCount, List<object> items)
			{
				this.PrintedBy = LoginSession.Current.FullName;
				this.PrintedTime = Platform.Time;

				this.FolderSystemName = folderSystemName;
				this.FolderName = folderName;
				this.FolderDescription = folderDescription;
				this.TotalItemCount = totalItemCount;
				this.Items = items;
			}

			[DataMember]
			public PersonNameDetail PrintedBy;

			[DataMember]
			public DateTime PrintedTime;

			[DataMember]
			public string FolderSystemName;

			[DataMember]
			public string FolderName;

			[DataMember]
			public string FolderDescription;

			[DataMember]
			public int TotalItemCount;

			[DataMember]
			public List<object> Items;
		}

		private readonly PrintContext _context;

		public WorklistPrintViewComponent(PrintContext context)
		{
			_context = context;
		}

		public override void Start()
		{
			SetUrl(this.PageUrl);
			base.Start();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}

		protected string PageUrl
		{
			get { return WebResourcesSettings.Default.WorklistPrintPreviewPageUrl; }
		}
	}
}
