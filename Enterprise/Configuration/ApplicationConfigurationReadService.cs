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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Configuration
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IApplicationConfigurationReadService))]
	public class ApplicationConfigurationReadService : ConfigurationServiceBase, IApplicationConfigurationReadService
	{
		#region IApplicationConfigurationReadService Members

		// because this service is invoked by the framework, rather than by the application,
		// it is safest to use a new persistence scope
		[ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
		[ResponseCaching("GetSettingsMetadataCachingDirective")]
		public ListSettingsGroupsResponse ListSettingsGroups(ListSettingsGroupsRequest request)
		{
			var broker = PersistenceContext.GetBroker<IConfigurationSettingsGroupBroker>();
			return new ListSettingsGroupsResponse(
				CollectionUtils.Map(broker.FindAll(), (ConfigurationSettingsGroup g) => g.GetDescriptor()));
		}

		// because this service is invoked by the framework, rather than by the application,
		// it is safest to use a new persistence scope
		[ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
		[ResponseCaching("GetSettingsMetadataCachingDirective")]
		public ListSettingsPropertiesResponse ListSettingsProperties(ListSettingsPropertiesRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Group, "Group");

			var where = ConfigurationSettingsGroup.GetCriteria(request.Group);

			var broker = PersistenceContext.GetBroker<IConfigurationSettingsGroupBroker>();
			var group = broker.FindOne(where);

			return new ListSettingsPropertiesResponse(
				CollectionUtils.Map(group.SettingsProperties, (ConfigurationSettingsProperty p) => p.GetDescriptor()));
		}

		// because this service is invoked by the framework, rather than by the application,
		// it is safest to use a new persistence scope
		[ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
		public ListConfigurationDocumentsResponse ListConfigurationDocuments(ListConfigurationDocumentsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckForNullReference(request.Query, "Query");

			return ListConfigurationDocumentsHelper(request.Query);
		}

		// because this service is invoked by the framework, rather than by the application,
		// it is safest to use a new persistence scope
		[ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
		[ResponseCaching("GetDocumentCachingDirective")]
		public GetConfigurationDocumentResponse GetConfigurationDocument(GetConfigurationDocumentRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.DocumentKey, "DocumentKey");

			return GetConfigurationDocumentHelper(request.DocumentKey);
		}

		#endregion
	}
}
