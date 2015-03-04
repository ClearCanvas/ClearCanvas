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
using System.Configuration;
using System.Xml;
using System.Threading;

namespace ClearCanvas.Common.Configuration
{
	[SettingsGroupDescription("Settings related to application upgrades.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	[UserSettingsMigrationDisabled]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class UpgradeSettings
	{
		private UpgradeSettings()
		{
		}

		public bool UnitTesting = false;

		internal static void CheckUserUpgradeEnabled()
		{
			if (!IsUserUpgradeEnabled())
				throw new InvalidOperationException("User upgrade is currently disabled.");
		}

		internal static bool IsUserUpgradeEnabled()
		{
            //This will not work on Mono out of the box.
		    if (Platform.IsMono)
		    {
                Platform.Log(LogLevel.Debug, "User settings upgrade will not work on Mono as-is.");
		        return false;
		    }

			if (Default.UnitTesting)
				return true;

			if (SettingsStore.IsSupported && !SettingsStore.IsStoreOnline)
				return false;

			return Default.UserUpgradeEnabled;
		}

		public override void Upgrade()
		{
			//Disable user settings upgrade.
		}

		private XmlElement CompletedUserUpgradeSteps
		{
			get
			{
				const string documentElementName = "completed-user-upgrade-steps";

				if (CompletedUserUpgradeStepsXml == null || CompletedUserUpgradeStepsXml.DocumentElement == null || CompletedUserUpgradeStepsXml.DocumentElement.Name != documentElementName)
				{
					CompletedUserUpgradeStepsXml = new XmlDocument();
					XmlElement documentElement = CompletedUserUpgradeStepsXml.CreateElement(documentElementName);
					CompletedUserUpgradeStepsXml.AppendChild(documentElement);
					return documentElement;
				}

				return CompletedUserUpgradeStepsXml.DocumentElement;
			}
		}

		private XmlElement GetUserUpgradeStepElement(string identifier, bool create)
		{
			const string userUpgradeStepElementName = "user-upgrade-step";
			const string identifierAttributeName = "identifier";

			string xPath = String.Format("{0}[@{1}='{2}']", userUpgradeStepElementName, identifierAttributeName, identifier);
			XmlElement element = CompletedUserUpgradeSteps.SelectSingleNode(xPath) as XmlElement;

			if (element == null && create)
			{
				element = CompletedUserUpgradeStepsXml.CreateElement(userUpgradeStepElementName);
				element.SetAttribute(identifierAttributeName, identifier);
				CompletedUserUpgradeSteps.AppendChild(element);
			}

			return element;
		}

		public bool IsUserUpgradeStepCompleted(string identifier)
		{
			if (IsSynchronized)
				Monitor.Enter(this);
			
			try
			{
				return null != GetUserUpgradeStepElement(identifier, false);
			}
			finally
			{
				if (IsSynchronized)
					Monitor.Exit(this);
			}
		}

		public void OnUserUpgradeStepCompleted(string identifier)
		{
			if (IsSynchronized)
				Monitor.Enter(this);

			try
			{
				GetUserUpgradeStepElement(identifier, true);
				if (UnitTesting) //don't save
					return;

				//TODO: on application stop?
				Save();
			}
			finally
			{
				if (IsSynchronized)
					Monitor.Exit(this);
			}
		}
	}
}
