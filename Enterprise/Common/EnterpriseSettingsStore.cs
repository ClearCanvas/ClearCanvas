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
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common.Ping;

namespace ClearCanvas.Enterprise.Common
{
	[ExtensionPoint]
	public class EnterpriseSettingsStoreOfflineCacheExtensionPoint : ExtensionPoint<IOfflineCache<ConfigurationDocumentKey, string>>
	{
	}

	/// <summary>
	/// This class is an implementation of <see cref="ISettingsStore"/> that uses a <see cref="IConfigurationService"/>
	/// as a back-end storage.
	/// </summary>
	[ExtensionOf(typeof(SettingsStoreExtensionPoint))]
	public class EnterpriseSettingsStore : ISettingsStore
	{
		private readonly IOfflineCache<ConfigurationDocumentKey, string> _offlineCache;

		public EnterpriseSettingsStore()
		{
			try
			{
				_offlineCache = (IOfflineCache<ConfigurationDocumentKey, string>)(new EnterpriseSettingsStoreOfflineCacheExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, SR.ExceptionOfflineCacheNotFound);
				_offlineCache = new NullOfflineCache<ConfigurationDocumentKey, string>();
			}
		}

		#region ISettingsStore Members

		public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey)
		{
			var serviceContract = !String.IsNullOrEmpty(user) ?
				typeof(IConfigurationService) : typeof(IApplicationConfigurationReadService);
			var service = (IApplicationConfigurationReadService)Platform.GetService(serviceContract);

			using (service as IDisposable)
			{
				var documentKey = new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey);
				var document = GetConfigurationDocument(service, documentKey);

				var values = new Dictionary<string, string>();
				var parser = new SettingsParser();
				parser.FromXml(document, values);
				return values;
			}
		}

		public void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
		{
			// note: if user == null, we are saving shared settings, if user is valued, we are saving user settings
			// but both are never edited as a single operation

			// the approach taken here is to create an XML document that represents a diff between
			// the default settings (as specified by the settings group meta-data) and the modified settings,
			// and store that document in the configuration store

			var service = Platform.GetService<IConfigurationService>();
			using(service as IDisposable)
			using (var offlineCacheClient = _offlineCache.CreateClient())
			{
				// first obtain the meta-data for the settings group properties
				var properties = ListSettingsProperties(group, service);

				// next we obtain any previously stored configuration document for this settings group
				var documentKey = new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey);
				var document = GetConfigurationDocument(service, documentKey);

				// parse document
				var parser = new SettingsParser();
				var values = new Dictionary<string, string>();
				parser.FromXml(document, values);

				// update the values that have changed
				foreach (var kvp in dirtyValues)
					values[kvp.Key] = kvp.Value;

				// now remove any values that are identical to the default values
				foreach (var property in properties)
				{
					string value;
					if (values.TryGetValue(property.Name, out value) && Equals(value, property.DefaultValue))
						values.Remove(property.Name);
				}

				try
				{
					if (values.Count > 0)
					{
						// generate the document, update local cache and server
						document = parser.ToXml(values);
						offlineCacheClient.Put(documentKey, document);
						service.SetConfigurationDocument(new SetConfigurationDocumentRequest(documentKey, document));
					}
					else
					{
						// every value is the same as the default, so the document can be removed
						// update local cache and server
						offlineCacheClient.Remove(documentKey);
						service.RemoveConfigurationDocument(new RemoveConfigurationDocumentRequest(documentKey));
					}

				}
				catch (EndpointNotFoundException e)
				{
					Platform.Log(LogLevel.Debug, e, "Unable to save settings to configuration service.");
				}
			}
		}

		public void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
		{
			Platform.CheckForNullReference(user, "user");

			var documentKey = new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey);
			
			// remove from offline cache
			using(var offlineCacheClient = _offlineCache.CreateClient())
			{
				offlineCacheClient.Remove(documentKey);
			}

			// remove from server
			Platform.GetService((IConfigurationService service) => service.RemoveConfigurationDocument(new RemoveConfigurationDocumentRequest(documentKey)));
		}

		public IList<SettingsGroupDescriptor> ListSettingsGroups()
		{
			List<SettingsGroupDescriptor> groups = null;

			// obtain the list of settings groups
			Platform.GetService((IApplicationConfigurationReadService service) => groups = ListSettingsGroups(service));

			return groups;
		}

		public SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor group)
		{
			List<SettingsGroupDescriptor> groups = null;
			var request = new ListSettingsGroupsRequest();

			//TODO/NOTE: I had changed the service interface so the server would return the previous group, but in order to
			//not have to release enterprise for Summer, I just did this.
			Platform.GetService((IApplicationConfigurationReadService service) => groups = service.ListSettingsGroups(request).Groups);

			if (groups == null || groups.Count == 0)
				return null;

			var sameGroup = CollectionUtils.Select(groups, other => other.AssemblyQualifiedTypeName == group.AssemblyQualifiedTypeName);
			var lessEqualGroups = CollectionUtils.Select(sameGroup, other => other.Version <= group.Version);
			if (lessEqualGroups.Count < 2)
				return null;

			//Sort ascending.
			lessEqualGroups.Sort((other1, other2) => other1.Version.CompareTo(other2.Version));
			//The current version is last, the previous is second last.
			return lessEqualGroups[lessEqualGroups.Count - 2];
		}

		public IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
		{
			// use the configuration service to obtain the properties
			IList<SettingsPropertyDescriptor> properties = null;
			Platform.GetService((IApplicationConfigurationReadService service) => properties = ListSettingsProperties(group, service));
			return properties;
		}

		public bool IsOnline
		{
			get { return RemoteCoreServiceProvider.IsOnline; }	
		}

		public bool SupportsImport
		{
			get { return true; }
		}

		public void ImportSettingsGroup(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties)
		{
			Platform.GetService((IConfigurationService service) =>
									service.ImportSettingsGroup(new ImportSettingsGroupRequest(group, properties)));
		}

		#endregion

		private static List<SettingsGroupDescriptor> ListSettingsGroups(IApplicationConfigurationReadService service)
		{
			return service.ListSettingsGroups(new ListSettingsGroupsRequest()).Groups;
		}
		private static IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group, IApplicationConfigurationReadService service)
		{
			try
			{
				// try to get the information from the local plugins
				return SettingsPropertyDescriptor.ListSettingsProperties(group);
			}
			catch (SettingsException)
			{
				// guess it's not a local group, so use the configuration service to obtain the properties
				return service.ListSettingsProperties(new ListSettingsPropertiesRequest(group)).Properties;
			}
		}

		private string GetConfigurationDocument(IApplicationConfigurationReadService service, ConfigurationDocumentKey documentKey)
		{
			using(var offlineCacheClient = _offlineCache.CreateClient())
			{
				try
				{
					var document = service.GetConfigurationDocument(new GetConfigurationDocumentRequest(documentKey)).Content;

					// keep offline cache up to date
					offlineCacheClient.Put(documentKey, document);

					return document;
				}
				catch (EndpointNotFoundException e)
				{
					Platform.Log(LogLevel.Debug, e, "Unable to retreive settings from configuration service.");

					// get most recent version from offline cache
					return offlineCacheClient.Get(documentKey);
				}
			}
		}

	}
}
