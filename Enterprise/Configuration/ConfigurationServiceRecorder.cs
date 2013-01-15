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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Configuration
{
	/// <summary>
	/// Records custom information about operations on <see cref="IConfigurationService"/>.
	/// </summary>
	public class ConfigurationServiceRecorder : IServiceOperationRecorder
	{
		[DataContract]
		class OperationData
		{
			[DataMember]
			public string Operation;
			[DataMember]
			public string DocumentName;
			[DataMember]
			public string DocumentVersion;
			[DataMember]
			public string DocumentUser;
			[DataMember]
			public string DocumentInstanceKey;
		}

		string IServiceOperationRecorder.Category
		{
			get { return "Configuration"; }
		}

		public void PreCommit(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContent)
		{
		}

		public void PostCommit(IServiceOperationRecorderContext recorderContext)
		{
			var request = (ConfigurationDocumentRequestBase)recorderContext.Request;

			var data = new OperationData
						{
							Operation = "SetConfigurationDocument",
							DocumentName = request.DocumentKey.DocumentName,
							DocumentVersion = request.DocumentKey.Version.ToString(),
							DocumentUser = request.DocumentKey.User ?? "{application}",
							DocumentInstanceKey = StringUtilities.NullIfEmpty(request.DocumentKey.InstanceKey) ?? "{default}"
						};


			var xml = JsmlSerializer.Serialize(data, "Audit");
			recorderContext.Write(data.Operation, xml);
		}
	}
}
