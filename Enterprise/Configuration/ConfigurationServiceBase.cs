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
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Configuration
{
	/// <summary>
	/// Contains code shared by different configuration service implementations.
	/// </summary>
	public abstract class ConfigurationServiceBase : CoreServiceLayer
	{
		/// <summary>
		/// Gets the specified configuration document.
		/// </summary>
		/// <param name="documentKey"></param>
		/// <returns></returns>
		protected GetConfigurationDocumentResponse GetConfigurationDocumentHelper(ConfigurationDocumentKey documentKey)
		{
			CheckReadAccess(documentKey);

			var broker = PersistenceContext.GetBroker<IConfigurationDocumentBroker>();
			var criteria = BuildDocumentKeyCriteria(documentKey);
			var documents = broker.Find(criteria, new SearchResultPage(0, 1), new EntityFindOptions { Cache = true });

			var document = CollectionUtils.FirstElement(documents);
			return document == null
					? new GetConfigurationDocumentResponse(documentKey, null, null, null)
					: new GetConfigurationDocumentResponse(documentKey, document.CreationTime, document.Body.ModifiedTime, document.Body.DocumentText);
		}

		/// <summary>
		/// Lists documents matching the specified query.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected ListConfigurationDocumentsResponse ListConfigurationDocumentsHelper(ConfigurationDocumentQuery query)
		{
			Platform.CheckNonNegative(query.FirstResult, "FirstResult");
			Platform.CheckArgumentRange(query.MaxResults, 0, 500, "MaxResults");

			var whereDocument = new ConfigurationDocumentSearchCriteria();
			var whereBody = new ConfigurationDocumentBodySearchCriteria();

			BuildDocumentQueryCriteria(query, whereDocument, whereBody);

			var broker = PersistenceContext.GetBroker<IConfigurationDocumentBroker>();
			var page = new SearchResultPage(query.FirstResult, query.MaxResults);
			var documents = broker.Find(whereDocument, whereBody, page);

			return new ListConfigurationDocumentsResponse(
				CollectionUtils.Map(documents, (ConfigurationDocument doc) => GetDocumentHeader(doc)));
		}

		#region Caching Directives

		/// <summary>
		/// This method is called automatically by response caching framework
		/// to provide caching directive for configuration documents.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected ResponseCachingDirective GetDocumentCachingDirective(GetConfigurationDocumentRequest request)
		{
			// if the request is for ConfigurationStoreSettings, we cannot try to load 
			// these settings to read the values, or we'll get into an infinite recursion
			// therefore, we assume ConfigurationStoreSettings are simply never cached.
			// a better solution would be to allow each settings group to specify its own
			// cacheability, and store this in the db with the settings meta-data
			// but this is not currently implemented
			if (request.DocumentKey.DocumentName == typeof(ConfigurationStoreSettings).FullName)
			{
				return ResponseCachingDirective.DoNotCacheDirective;
			}

			var settings = new ConfigurationStoreSettings();
			return new ResponseCachingDirective(
				settings.ConfigurationCachingEnabled,
				TimeSpan.FromSeconds(settings.ConfigurationCachingTimeToLiveSeconds),
				ResponseCachingSite.Client);
		}

		/// <summary>
		/// This method is called automatically by response caching framework
		/// to provide caching directive for settings meta-data.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected ResponseCachingDirective GetSettingsMetadataCachingDirective(object request)
		{
			var settings = new ConfigurationStoreSettings();
			return new ResponseCachingDirective(
				settings.SettingsMetadataCachingEnabled,
				TimeSpan.FromSeconds(settings.SettingsMetadataCachingTimeToLiveSeconds),
				ResponseCachingSite.Client);
		}

		#endregion

		#region Criteria builders

		/// <summary>
		/// Builds the criteria for retrieving a document by key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected ConfigurationDocumentSearchCriteria BuildDocumentKeyCriteria(ConfigurationDocumentKey key)
		{
			var criteria = new ConfigurationDocumentSearchCriteria();
			criteria.DocumentName.EqualTo(key.DocumentName);

			if (!string.IsNullOrEmpty(key.InstanceKey))
			{
				criteria.InstanceKey.EqualTo(key.InstanceKey);
			}
			else
			{
				criteria.InstanceKey.IsNull();
			}

			if (!string.IsNullOrEmpty(key.User))
			{
				criteria.User.EqualTo(key.User);
			}
			else
			{
				criteria.User.IsNull();
			}
			criteria.DocumentVersionString.EqualTo(VersionUtils.ToPaddedVersionString(key.Version, false, false));
			return criteria;
		}

		/// <summary>
		/// Builds criteria for querying documents.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="whereDocument"></param>
		/// <param name="whereBody"></param>
		private static void BuildDocumentQueryCriteria(ConfigurationDocumentQuery query, ConfigurationDocumentSearchCriteria whereDocument, ConfigurationDocumentBodySearchCriteria whereBody)
		{
			var currentUser = Thread.CurrentPrincipal == null ? null : Thread.CurrentPrincipal.Identity.Name;

			// user type
			if (query.UserType == ConfigurationDocumentQuery.DocumentUserType.User)
			{
				// to query for user documents, we must have an authenticated user
				if (string.IsNullOrEmpty(currentUser))
					ThrowNotAuthorized();

				// current users documents only
				whereDocument.User.EqualTo(currentUser);
			}
			else
			{
				// shared documents only!
				// (authentication is irrelevant - shared documents can be queried anonymously)
				whereDocument.User.IsNull();
			}

			// document name
			if (query.DocumentName.IsSet && !string.IsNullOrEmpty(query.DocumentName.Value))
			{
				if (query.DocumentName.Operator == ConfigurationDocumentQuery.StringOperator.StartsWith)
				{
					whereDocument.DocumentName.StartsWith(query.DocumentName.Value);
				}
				else if (query.DocumentName.Operator == ConfigurationDocumentQuery.StringOperator.Exact)
				{
					whereDocument.DocumentName.EqualTo(query.DocumentName.Value);
				}
			}

			// document version
			if (query.Version.IsSet)
			{
				whereDocument.DocumentVersionString.EqualTo(VersionUtils.ToPaddedVersionString(query.Version.Value));
			}

			// instance key
			if (query.InstanceKey.IsSet)
			{
				if (query.InstanceKey.Value == null)
				{
					whereDocument.InstanceKey.IsNull(); // default instances only!
				}
				else
				{
					whereDocument.InstanceKey.EqualTo(query.InstanceKey.Value);
				}
			}

			// creation time
			if (query.CreationTime.IsSet)
			{
				if (query.CreationTime.Operator == ConfigurationDocumentQuery.DateTimeOperator.After)
				{
					whereDocument.CreationTime.MoreThan(query.CreationTime.Value);
				}
				else if (query.CreationTime.Operator == ConfigurationDocumentQuery.DateTimeOperator.Before)
				{
					whereDocument.CreationTime.LessThan(query.CreationTime.Value);
				}
			}

			// modified time
			if (query.ModifiedTime.IsSet)
			{
				if (query.ModifiedTime.Operator == ConfigurationDocumentQuery.DateTimeOperator.After)
				{
					whereBody.ModifiedTime.MoreThan(query.ModifiedTime.Value);
				}
				else if (query.ModifiedTime.Operator == ConfigurationDocumentQuery.DateTimeOperator.Before)
				{
					whereBody.ModifiedTime.LessThan(query.ModifiedTime.Value);
				}
			}
		}

		#endregion

		#region Access control

		protected static void CheckReadAccess(ConfigurationDocumentKey key)
		{
			var user = key.User;
			if (string.IsNullOrEmpty(user))
			{
				// all users can read application configuration docs
			}
			else
			{
				// user can only read their own configuration docs
				if (user != Thread.CurrentPrincipal.Identity.Name)
					ThrowNotAuthorized();
			}
		}

		protected static void CheckWriteAccess(ConfigurationDocumentKey key)
		{
			var user = key.User;
			if (string.IsNullOrEmpty(user))
			{
				// this is an application configuration doc - need admin permission
				if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.System.Configuration))
					ThrowNotAuthorized();
			}
			else
			{
				// user can only save their own configuration docs
				if (user != Thread.CurrentPrincipal.Identity.Name)
					ThrowNotAuthorized();
			}
		}

		private static void ThrowNotAuthorized()
		{
			throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
		}

		#endregion

		#region Misc

		private static ConfigurationDocumentHeader GetDocumentHeader(ConfigurationDocument document)
		{
			var key = new ConfigurationDocumentKey(
				document.DocumentName,
				VersionUtils.FromPaddedVersionString(document.DocumentVersionString),
				document.User,
				document.InstanceKey);

			return new ConfigurationDocumentHeader(key, document.CreationTime, document.Body.ModifiedTime);
		}

		#endregion

	}
}
