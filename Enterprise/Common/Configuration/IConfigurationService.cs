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
	/// Defines a service for saving/retrieving configuration data to/from a persistent store.
	/// </summary>
	[EnterpriseCoreService]
	[ServiceContract]
	[Authentication(true)]
	public interface IConfigurationService : IApplicationConfigurationReadService
	{
		/// <summary>
		/// Imports meta-data for a settings group.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ImportSettingsGroupResponse ImportSettingsGroup(ImportSettingsGroupRequest request);

		/// <summary>
		/// Sets the content for the specified document, version, user and instance key.
		/// The user and instance key may be null.
		/// </summary>
		[OperationContract]
		SetConfigurationDocumentResponse SetConfigurationDocument(SetConfigurationDocumentRequest request);

		/// <summary>
		/// Removes any stored settings values for the specified group, version, user and instance key.
		/// The user and instance key may be null.
		/// </summary>
		[OperationContract]
		RemoveConfigurationDocumentResponse RemoveConfigurationDocument(RemoveConfigurationDocumentRequest request);

	}
}
