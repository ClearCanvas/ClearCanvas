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
using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
    internal class UserSettingsUpgradeStepFactory : IUserUpgradeStepFactory
    {
        public ICollection<UserUpgradeStep> CreateSteps()
        {
            var upgradeSteps = new List<UserUpgradeStep>();

			if (UpgradeSettings.IsUserUpgradeEnabled())
			{
				foreach (var group in SettingsGroupDescriptor.ListInstalledSettingsGroups())
				{
					try
					{
						UserSettingsUpgradeStep step = UserSettingsUpgradeStep.Create(group);
						if (step != null)
							upgradeSteps.Add(step);
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Warn, e, "Unable to migrate user settings: {0}", group.Name);
					}
				}
			}

        	return upgradeSteps;
        }
    }

    internal class UserSettingsUpgradeStep : UserUpgradeStep
    {
        private UserSettingsUpgradeStep(ApplicationSettingsBase settings)
        {
            Platform.CheckForNullReference(settings, "settings");
            Settings = settings;
        }

        private ApplicationSettingsBase Settings { get; set; }

        public override string Identifier
        {
			get { return Settings.GetType().FullName; }
        }

        protected override bool PerformUpgrade()
        {
			ApplicationSettingsExtensions.MigrateUserSettings(Settings);
        	return true;
        }

		public static UserSettingsUpgradeStep Create(SettingsGroupDescriptor settingsGroup)
		{
			return Create(ApplicationSettingsHelper.GetSettingsClass(settingsGroup));
		}

    	public static UserSettingsUpgradeStep Create(Type settingsClass)
        {
			//Important: this line first so we can't get into infinite recursion issues with trying
			//to migrate the UpgradeSettings class on first access (SettingsStoreSettingsProvider does that).
			if (!ApplicationSettingsHelper.IsUserSettingsMigrationEnabled(settingsClass))
				return null;

			if (!new SettingsGroupDescriptor(settingsClass).HasUserScopedSettings)
				return null; //no point

			if (!UpgradeSettings.IsUserUpgradeEnabled())
				return null;

			if (UpgradeSettings.Default.IsUserUpgradeStepCompleted(settingsClass.FullName))
				return null;

			ApplicationSettingsBase settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
			return new UserSettingsUpgradeStep(settings);
		}
    }

    public static class SettingsMigrator
    {
		public static bool MigrateUserSettings(SettingsGroupDescriptor settingsGroup)
		{
			return MigrateUserSettings(ApplicationSettingsHelper.GetSettingsClass(settingsGroup));
		}

    	public static bool MigrateUserSettings(Type settingsClass)
        {
			UserSettingsUpgradeStep step = UserSettingsUpgradeStep.Create(settingsClass);
			return step != null && step.Run();
        }

		public static bool MigrateSharedSettings(SettingsGroupDescriptor settingsGroup, string previousExeConfigFilename)
		{
			return MigrateSharedSettings(ApplicationSettingsHelper.GetSettingsClass(settingsGroup), previousExeConfigFilename);
		}

    	public static bool MigrateSharedSettings(Type settingsClass, string previousExeConfigFilename)
        {
			if (!ApplicationSettingsHelper.IsSharedSettingsMigrationEnabled(settingsClass))
				return false;

			ApplicationSettingsBase settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
			ApplicationSettingsExtensions.MigrateSharedSettings(settings, previousExeConfigFilename);
    		return true;
        }
    }
}