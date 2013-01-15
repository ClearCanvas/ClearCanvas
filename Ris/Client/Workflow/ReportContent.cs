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
using System.Text;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Defines the schema of the report content JSML.  This data-contract is never sent to the server.
	/// It is purely a local-contract for internal use.
	/// </summary>
	[DataContract]
	class ReportContent : DataContractBase
	{
		public ReportContent()
		{
		}

		public ReportContent(string reportText)
		{
			this.ReportText = reportText;
		}

		/// <summary>
		/// The free-text component of the report.
		/// </summary>
		[DataMember]
		public string ReportText;

		public string ToJsml()
		{
			return JsmlSerializer.Serialize(this, "Report");
		}
	}
}
