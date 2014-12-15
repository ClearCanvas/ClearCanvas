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
using System.IO;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Common.Configuration;

namespace ClearCanvas.Enterprise.Common.Setup
{
	public static class SetupHelper
	{
		/// <summary>
		/// Import authority groups defined in XML files located at the specified path.
		/// </summary>
		/// <param name="dataFileOrFolderPath"></param>
		public static void ImportUsers(string dataFileOrFolderPath)
		{
			// determine list of source files to import
			var fileList = new List<string>();
			if (File.Exists(dataFileOrFolderPath))
			{
				fileList.Add(dataFileOrFolderPath);
			}
			else if (Directory.Exists(dataFileOrFolderPath))
			{
				fileList.AddRange(Directory.GetFiles(dataFileOrFolderPath, "*.xml"));
			}
			else
				throw new ArgumentException(string.Format("{0} is not a valid data file or directory.", dataFileOrFolderPath));

			Platform.Log(LogLevel.Info, "importing users from {0} files", fileList.Count);

			var users = from file in fileList
							 let xml = File.ReadAllText(file)
							 from user in JsmlSerializer.Deserialize<UserDefinition[]>(xml)
							 select user;

			ImportUsers(users, dataFileOrFolderPath);
		}

		private static void ImportUsers(IEnumerable<UserDefinition> users, string source)
		{
			var existingUsers = new List<UserSummary>();
			Platform.GetService<IUserReadService>(
				s => existingUsers = s.ListUsers(new ListUsersRequest()).Users);

			var authorityGroups = new List<AuthorityGroupSummary>();
			Platform.GetService<IAuthorityGroupReadService>(
				s => authorityGroups = s.ListAuthorityGroups(new ListAuthorityGroupsRequest()).AuthorityGroups);

			var service = Platform.GetService<IUserAdminService>();
			foreach (var user in users)
			{
				var userSummary = existingUsers.Find(u => u.UserName == user.UserName);
				if (userSummary == null)
				{
					var userDetail = new UserDetail
										{
											UserName = user.UserName,
											DisplayName = user.DisplayName,
											Enabled = user.Enabled,
											EmailAddress = user.EmailAddress,
											AuthorityGroups = user.AuthorityGroups.Select(
												newAGName => authorityGroups.Find(existingAG => newAGName == existingAG.Name)).ToList()
										};

					service.AddUser(new AddUserRequest(userDetail));
					LogImportedUsers(userDetail, source);
				}
				else
				{
					var userDetail = service.LoadUserForEdit(new LoadUserForEditRequest(user.UserName)).UserDetail;
					userDetail.DisplayName = user.DisplayName;
					userDetail.Enabled = user.Enabled;
					userDetail.EmailAddress = user.EmailAddress;
					userDetail.AuthorityGroups.Clear();
					userDetail.AuthorityGroups = user.AuthorityGroups.Select(
						newAGName => authorityGroups.Find(existingAG => newAGName == existingAG.Name)).ToList();

					service.UpdateUser(new UpdateUserRequest(userDetail));
					LogImportedUsers(userDetail, source);
				}
			}
		}

		/// <summary>
		/// Import authority tokens defined in local plugins.
		/// </summary>
		public static void ImportAuthorityTokens(string[] addToGroups)
		{
			var tokens = AuthorityGroupSetup.GetAuthorityTokens();
			var summaries = tokens.Select(t => new AuthorityTokenSummary(t.Token, t.DefiningAssembly, t.Description, t.FormerIdentities)).ToList();

			Platform.GetService<IAuthorityGroupAdminService>(
				service => service.ImportAuthorityTokens(new ImportAuthorityTokensRequest(summaries, new List<string>(addToGroups))));

			LogImportedTokens(tokens);
		}

		/// <summary>
		/// Import authority groups defined in local plugins.
		/// </summary>
		public static void ImportEmbeddedAuthorityGroups()
		{
			var groups = AuthorityGroupSetup.GetDefaultAuthorityGroups();
			ImportAuthorityGroups(groups, "plugins");
		}

		/// <summary>
		/// Import authority groups defined in XML files located at the specified path.
		/// </summary>
		/// <param name="dataFileOrFolderPath"></param>
		public static void ImportAuthorityGroups(string dataFileOrFolderPath)
		{
			// determine list of source files to import
			var fileList = new List<string>();
			if (File.Exists(dataFileOrFolderPath))
			{
				fileList.Add(dataFileOrFolderPath);
			}
			else if (Directory.Exists(dataFileOrFolderPath))
			{
				fileList.AddRange(Directory.GetFiles(dataFileOrFolderPath, "*.xml"));
			}
			else
				throw new ArgumentException(string.Format("{0} is not a valid data file or directory.", dataFileOrFolderPath));

			var authGroups = from file in fileList
							 let xml = File.ReadAllText(file)
							 from authGroup in JsmlSerializer.Deserialize<AuthorityGroupDefinition[]>(xml)
							 select authGroup;

			ImportAuthorityGroups(authGroups, dataFileOrFolderPath);
		}

		private static void ImportAuthorityGroups(IEnumerable<AuthorityGroupDefinition> groups, string source)
		{
			var groupDetails = groups.Select(g =>
											new AuthorityGroupDetail(
											null,
											g.Name,
											g.Description,
											g.BuiltIn,
											g.DataGroup,
											g.Tokens.Select(t => new AuthorityTokenSummary(t)).ToList()
										)).ToList();

			Platform.GetService<IAuthorityGroupAdminService>(
				service => service.ImportAuthorityGroups(new ImportAuthorityGroupsRequest(groupDetails)));

			LogImportedGroups(groups, source);
		}

		public static void ExportSettingsDefinition(ISettingsStore store, SettingDefinition request, string dataFile)
		{
			var version = new Version(request.Version);
			var allSettingsGroups = store.ListSettingsGroups();
			var group = allSettingsGroups.SingleOrDefault(g => g.Name.Equals(request.Group) && g.Version.Equals(version));
			if (group == null)
			{
				Platform.Log(LogLevel.Info, "Cannot find settings group: {0}/{1}", request.Group, request.Version);
				return;
			}

			var settings = store.GetSettingsValues(group, null, null);
			if (!settings.ContainsKey(request.Property))
			{
				Platform.Log(LogLevel.Info, "Cannot find settings property: {0}/{1}/{2}", request.Group, request.Version, request.Property);
				return;
			}

			var value = settings[request.Property];
			File.WriteAllText(dataFile, value);

			Platform.Log(LogLevel.Info, "Setting value written to {0}", value);
		}

		public static void ImportSettingsDefinition(ISettingsStore store, string dataFileOrFolderPath, bool overwrite)
		{
			// determine list of source files to import
			var fileList = new List<string>();
			if (File.Exists(dataFileOrFolderPath))
			{
				fileList.Add(dataFileOrFolderPath);
			}
			else if (Directory.Exists(dataFileOrFolderPath))
			{
				fileList.AddRange(Directory.GetFiles(dataFileOrFolderPath, "*.xml"));
			}
			else
				throw new ArgumentException(string.Format("{0} is not a valid data file or directory.", dataFileOrFolderPath));

			Platform.Log(LogLevel.Info, "Loading settings from {0}...", dataFileOrFolderPath);

			var configurations = from file in fileList
								 let xml = File.ReadAllText(file)
								 from config in JsmlSerializer.Deserialize<SettingDefinition[]>(xml)
								 select config;

			ImportSettingsDefinition(store, configurations, overwrite);
		}

		public static void ImportSettingsDefinition(ISettingsStore store, IEnumerable<SettingDefinition> settingDefinitions, bool overwrite)
		{
			var allSettingsGroups = store.ListSettingsGroups();
			const int maxCharDisplayed = 50;

			foreach (var s in settingDefinitions)
			{
				var found = false;
				var setting = s;
				var version = string.IsNullOrEmpty(setting.Version) ? null : new Version(setting.Version);
				var value = s.Value.Length <= maxCharDisplayed ? s.Value : s.Value.Substring(0, maxCharDisplayed) + "...";
				value = value.Replace('\r', ' ');
				value = value.Replace('\n', ' ');

				var groups = version == null
									? allSettingsGroups.Where(g => g.Name.Equals(setting.Group))
									: allSettingsGroups.Where(g => g.Name.Equals(setting.Group) && g.Version.Equals(version));

				foreach (var g in groups)
				{
					var property = store.ListSettingsProperties(g).SingleOrDefault(p => p.Name.Equals(s.Property));
					if (property == null)
						continue;

					found = true;
					var settings = store.GetSettingsValues(g, null, null);
					if (!overwrite && settings.ContainsKey(s.Property))
					{
						Platform.Log(LogLevel.Info, "Setting unchanged: {0}/{1}/{2} {3}", s.Group, s.Property, s.Version, value);
					}
					else 
					{
						settings[s.Property] = s.Value;
						store.PutSettingsValues(g, null, null, settings);

						Platform.Log(LogLevel.Info, "Setting updated: {0}/{1}/{2} {3}", s.Group, s.Property, s.Version, value);
					}
				}

				if (!found)
					Platform.Log(LogLevel.Error, "Setting not found: {0}/{1}/{2}", s.Group, s.Property, s.Version);
			}
		}

		public static void ImportConfigurationsDefinition(string dataFileOrFolderPath)
		{
			// determine list of source files to import
			var fileList = new List<string>();
			if (File.Exists(dataFileOrFolderPath))
			{
				fileList.Add(dataFileOrFolderPath);
			}
			else if (Directory.Exists(dataFileOrFolderPath))
			{
				fileList.AddRange(Directory.GetFiles(dataFileOrFolderPath, "*.xml"));
			}
			else
				throw new ArgumentException(string.Format("{0} is not a valid data file or directory.", dataFileOrFolderPath));

			var configurations = from file in fileList
								 let xml = File.ReadAllText(file)
								 from config in JsmlSerializer.Deserialize<ConfigurationDefinition[]>(xml)
								 select config;

			ImportConfigurationsDefinition(configurations, dataFileOrFolderPath);
		}

		private static void ImportConfigurationsDefinition(IEnumerable<ConfigurationDefinition> configurations, string source)
		{
			var requests = configurations.Select(c =>
				new SetConfigurationDocumentRequest(
					new ConfigurationDocumentKey(c.Name, Version.Parse(c.Version), null, null),
					c.Body
					)).ToList();

			foreach (var r in requests)
			{
				var request = r;
				Platform.GetService<IConfigurationService>(
					service => service.SetConfigurationDocument(request));
			}

			LogImportedConfiguration(configurations, source);
		}

		private static void LogImportedConfiguration(IEnumerable<ConfigurationDefinition> configurations, string source)
		{
			foreach (var c in configurations.Distinct())
			{
				Platform.Log(LogLevel.Info, "Imported configuration definition {0} from {1}", c.Name, source);
			}
		}

		private static void LogImportedGroups(IEnumerable<AuthorityGroupDefinition> groups, string source)
		{
			foreach (var g in groups.Distinct())
			{
				Platform.Log(LogLevel.Info, "Imported authority group definition {0} from {1}", g.Name, source);
			}
		}

		private static void LogImportedTokens(IEnumerable<AuthorityTokenDefinition> tokens)
		{
			foreach (var token in tokens)
			{
				Platform.Log(LogLevel.Info, "Imported authority token '{0}' from {1}", token.Token, token.DefiningAssembly);
			}
		}

		private static void LogImportedUsers(UserDetail u, string source)
		{
			Platform.Log(LogLevel.Info, "Imported user {0} from {1}", u.UserName, source);
		}

	}
}
