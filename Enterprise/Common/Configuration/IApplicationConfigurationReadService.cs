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

using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	/// <summary>
	/// Defines a service for allowing un-authenticated processes to read application settings.
	/// </summary>
	[EnterpriseCoreService]
	[ServiceContract]
	[Authentication(false)]
	public interface IApplicationConfigurationReadService
	{
		/// <summary>
		/// Lists settings groups installed in the local plugin base.
		/// </summary>
		[OperationContract]
		ListSettingsGroupsResponse ListSettingsGroups(ListSettingsGroupsRequest request);

		/// <summary>
		/// Lists the settings properties for the specified settings group.
		/// </summary>
		[OperationContract]
		ListSettingsPropertiesResponse ListSettingsProperties(ListSettingsPropertiesRequest request);

		/// <summary>
		/// Lists configuration documents matching specified criteria.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ListConfigurationDocumentsResponse ListConfigurationDocuments(ListConfigurationDocumentsRequest request);


		/// <summary>
		/// Gets the document specified by the name, version, user and instance key.
		/// The user and instance key may be null.
		/// </summary>
		[OperationContract]
		GetConfigurationDocumentResponse GetConfigurationDocument(GetConfigurationDocumentRequest request);
	}
}
