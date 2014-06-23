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
using System.Data.Linq;
using System.Linq;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Configuration
{
	/// <summary>
	/// Internal configuration document broker.
	/// </summary>
	internal class ConfigurationDocumentBroker : Broker
	{
		internal ConfigurationDocumentBroker(ConfigurationDataContext context)
			: base(context) {}

		/// <summary>
		/// Get the specified ConfigurationDocument or null if not found
		/// </summary>
		/// <param name="documentKey"></param>
		/// <returns></returns>
		public ConfigurationDocument GetConfigurationDocument(ConfigurationDocumentKey documentKey)
		{
			return GetConfigurationDocument(documentKey, false);
		}

		/// <summary>
		/// Get the specified ConfigurationDocument or null if not found
		/// </summary>
		/// <param name="documentKey"></param>
		/// <returns></returns>
		public ConfigurationDocument GetPriorConfigurationDocument(ConfigurationDocumentKey documentKey)
		{
			return GetConfigurationDocument(documentKey, true);
		}

		private static readonly Func<ConfigurationDataContext, string, string, string, string, IQueryable<ConfigurationDocument>> _queryPriorVersion =
			CompiledQuery.Compile<ConfigurationDataContext, string, string, string, string, IQueryable<ConfigurationDocument>>(
				(context, documentName, instanceKey, user, paddedVersionString) =>
				(from d in context.ConfigurationDocuments
				 where ((instanceKey == null && d.InstanceKey == null) || d.InstanceKey == instanceKey)
				       && ((user == null && d.User == null) || d.User == user)
				       && d.DocumentName == documentName
				       && string.Compare(d.DocumentVersionString, paddedVersionString, StringComparison.InvariantCulture) < 0
				 orderby d.DocumentVersionString descending
				 select d
				).Take(1));

		private static readonly Func<ConfigurationDataContext, string, string, string, string, IQueryable<ConfigurationDocument>> _queryCurrentVersion =
			CompiledQuery.Compile<ConfigurationDataContext, string, string, string, string, IQueryable<ConfigurationDocument>>(
				(context, documentName, instanceKey, user, paddedVersionString) =>
				(from d in context.ConfigurationDocuments
				 where ((instanceKey == null && d.InstanceKey == null) || d.InstanceKey == instanceKey)
				       && ((user == null && d.User == null) || d.User == user)
				       && d.DocumentName == documentName
				       && d.DocumentVersionString == paddedVersionString
				 select d
				).Take(1));

		private ConfigurationDocument GetConfigurationDocument(ConfigurationDocumentKey documentKey, bool prior)
		{
			var query = prior ? _queryPriorVersion : _queryCurrentVersion;

			return query(Context,
			             documentKey.DocumentName,
			             !string.IsNullOrEmpty(documentKey.InstanceKey) ? documentKey.InstanceKey : null,
			             !string.IsNullOrEmpty(documentKey.User) ? documentKey.User : null,
			             VersionUtils.ToPaddedVersionString(documentKey.Version, false, false)).FirstOrDefault();
		}

		/// <summary>
		/// Insert a ConfigurationDocument
		/// </summary>
		/// <param name="entity"></param>
		public void AddConfigurationDocument(ConfigurationDocument entity)
		{
			Context.ConfigurationDocuments.InsertOnSubmit(entity);
		}

		internal void DeleteAllDocuments()
		{
			Context.ConfigurationDocuments.DeleteAllOnSubmit(Context.ConfigurationDocuments);
		}
	}
}