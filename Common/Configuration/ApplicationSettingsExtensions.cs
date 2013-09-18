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
using System.Linq.Expressions;
using System.Reflection;
using SystemConfiguration = System.Configuration.Configuration;
using System.Xml;

namespace ClearCanvas.Common.Configuration
{
	public static class ApplicationSettingsExtensions
	{
		#region Public API

		public static object GetPreviousSharedPropertyValue(this ApplicationSettingsBase settings, string propertyName, string previousExeConfigFilename)
		{
			SettingsProperty property = settings.Properties[propertyName];
			if (property == null)
				throw new ArgumentException(String.Format("The specified property does not exist: {0}", propertyName), "propertyName");

			ISharedApplicationSettingsProvider provider = GetSharedSettingsProvider(property.Provider);
			if (provider == null)
				return null;

			SettingsPropertyValueCollection values = provider.GetPreviousSharedPropertyValues(settings.Context,
									new SettingsPropertyCollection { property }, previousExeConfigFilename);

			SettingsPropertyValue value = values[propertyName];
			return value == null ? null : value.PropertyValue;
		}

		public static TProperty GetSharedPropertyValue<TSettings, TProperty>(this TSettings settings, Expression<Func<TSettings, TProperty>> propertyExpr, TProperty value)
			where TSettings : ApplicationSettingsBase
		{
			return (TProperty)GetSharedPropertyValue(settings, ConvertPropertyExpressionToMember(propertyExpr).Name);
		}

		public static object GetSharedPropertyValue(this ApplicationSettingsBase settings, string propertyName)
		{
			SettingsProperty property = settings.Properties[propertyName];
			if (property == null)
				throw new ArgumentException(String.Format("The specified property does not exist: {0}", propertyName), "propertyName");

			ISharedApplicationSettingsProvider sharedSettingsProvider = GetSharedSettingsProvider(property.Provider);
			if (sharedSettingsProvider == null)
				return null;

			var values = sharedSettingsProvider.GetSharedPropertyValues(settings.Context, new SettingsPropertyCollection { property });
			SettingsPropertyValue value = values[propertyName];
			return value == null ? null : value.PropertyValue;
		}

		public static void SetSharedPropertyValue<TSettings, TProperty>(this TSettings settings, Expression<Func<TSettings, TProperty>> propertyExpr, TProperty value)
			where TSettings : ApplicationSettingsBase
		{
			SetSharedPropertyValue(settings, ConvertPropertyExpressionToMember(propertyExpr).Name, value);
		}

		public static void SetSharedPropertyValue(this ApplicationSettingsBase settings, string propertyName, object value)
		{
			SettingsProperty property = settings.Properties[propertyName];
			if (property == null)
				throw new ArgumentException(String.Format("The specified property does not exist: {0}", propertyName), "propertyName");

			ISharedApplicationSettingsProvider sharedSettingsProvider = GetSharedSettingsProvider(property.Provider);
			if (sharedSettingsProvider == null)
				throw new NotSupportedException("Setting shared values is not supported.");

			SettingsPropertyValue settingsValue = new SettingsPropertyValue(property) { PropertyValue = value };
			sharedSettingsProvider.SetSharedPropertyValues(settings.Context, new SettingsPropertyValueCollection { settingsValue });

			SaveIfDirty(settings);
			//Need to call Reload because changes to shared settings aren't automatically realized by the .NET settings framework.
			settings.Reload();
		}

		public static void ImportSharedSettings(this ApplicationSettingsBase settings, string configurationFilename)
		{
			SystemConfiguration configuration = SystemConfigurationHelper.GetExeConfiguration(configurationFilename);
			var values = SystemConfigurationHelper.GetSettingsValues(configuration, settings.GetType());
			SetSharedPropertyValues(settings, values);
		}

		#endregion

		#region Helpers

		private static SettingsPropertyCollection GetPropertiesForProvider(ApplicationSettingsBase settings, SettingsProvider provider)
        {
            SettingsPropertyCollection properties = new SettingsPropertyCollection();
            
            foreach (SettingsProperty property in settings.Properties)
            {
                if (property.Provider == provider)
                    properties.Add(property);
            }
            
            return properties;
        }

        private static ISharedApplicationSettingsProvider GetSharedSettingsProvider(SettingsProvider provider)
        {
            var shared = provider as ISharedApplicationSettingsProvider;
            if (shared != null)
                return shared;

            var local = provider as LocalFileSettingsProvider;
            if (local != null)
                return new ExtendedLocalFileSettingsProvider(local);

            return null;
        }

        private static void SaveIfDirty(ApplicationSettingsBase settings)
        {
            foreach (SettingsPropertyValue property in settings.PropertyValues)
            {
            	if (!property.IsDirty)
					continue;
            	
				settings.Save();
            	return;
            }
        }

		private static void SetSharedPropertyValues(ApplicationSettingsBase settings, Dictionary<string, string> values)
		{
			foreach (SettingsProvider provider in settings.Providers)
			{
				ISharedApplicationSettingsProvider sharedSettingsProvider = GetSharedSettingsProvider(provider);
				if (sharedSettingsProvider == null)
					throw new NotSupportedException("Setting shared values is not supported.");

				var properties = GetPropertiesForProvider(settings, provider);
				SettingsPropertyValueCollection settingsValues = new SettingsPropertyValueCollection();

				foreach (var value in values)
				{
					SettingsProperty property = properties[value.Key];
					if (property == null)
						continue;

					settingsValues.Add(new SettingsPropertyValue(property) { SerializedValue = value.Value, IsDirty = true });
				}

				sharedSettingsProvider.SetSharedPropertyValues(settings.Context, settingsValues);
			}

			SaveIfDirty(settings);
			settings.Reload();
		}

		private static void MigrateProperty(IMigrateSettings customMigrator, MigrationScope migrationScope, 
		                                    SettingsPropertyValue currentValue, object previousValue)
		{
			var migrationValues = new SettingsPropertyMigrationValues(
				currentValue.Property.Name, migrationScope, 
				currentValue.PropertyValue, previousValue);

			customMigrator.MigrateSettingsProperty(migrationValues);
			if (!Equals(migrationValues.CurrentValue, currentValue.PropertyValue))
				currentValue.PropertyValue = migrationValues.CurrentValue;
		}

		internal static void MigrateUserSettings(ApplicationSettingsBase settings)
		{
			if (settings is IMigrateSettings)
			{
				IMigrateSettings customMigrator = (IMigrateSettings)settings;
				foreach (SettingsProperty property in settings.Properties)
				{
					if (!SettingsPropertyExtensions.IsUserScoped(property))
						continue;

					object previousValue = settings.GetPreviousVersion(property.Name);

					//need to do this to force the values to load before accessing the PropertyValues in order to migrate,
					//otherwise the SettingsPropertyValue will always be null.
					var iForceSettingsPropertyValuesToLoad = settings[property.Name];
					var currentValue = settings.PropertyValues[property.Name];
					MigrateProperty(customMigrator, MigrationScope.User, currentValue, previousValue);
				}
			}
			else
			{
				settings.Upgrade();
			}

			//Don't need to reload because the user settings will be current.
			SaveIfDirty(settings);
		}

		internal static void MigrateSharedSettings(ApplicationSettingsBase settings, string previousExeConfigFilename)
		{
			if (settings is IMigrateSettings)
			{
				IMigrateSettings customMigrator = (IMigrateSettings) settings;
				foreach (SettingsProvider settingsProvider in settings.Providers)
				{
					ISharedApplicationSettingsProvider sharedSettingsProvider = GetSharedSettingsProvider(settingsProvider);
					if (sharedSettingsProvider == null || !sharedSettingsProvider.CanUpgradeSharedPropertyValues(settings.Context))
						continue;

					var properties = GetPropertiesForProvider(settings, settingsProvider);
					var previousValues = sharedSettingsProvider.GetPreviousSharedPropertyValues(settings.Context,
					                                                                            properties, previousExeConfigFilename);
					if (previousValues == null || previousValues.Count == 0)
						continue;

					var currentValues = sharedSettingsProvider.GetSharedPropertyValues(settings.Context, properties);
					foreach (SettingsPropertyValue previousValue in previousValues)
					{
						SettingsPropertyValue currentValue = currentValues[previousValue.Name];
						if (currentValue == null)
							continue;

						MigrateProperty(customMigrator, MigrationScope.Shared, currentValue, previousValue.PropertyValue);
					}

					foreach (SettingsPropertyValue property in currentValues)
					{
						if (!property.IsDirty)
							continue;

						sharedSettingsProvider.SetSharedPropertyValues(settings.Context, currentValues);
						break;
					}
				}
			}
			else
			{
				foreach (SettingsProvider settingsProvider in settings.Providers)
				{
					ISharedApplicationSettingsProvider sharedSettingsProvider = GetSharedSettingsProvider(settingsProvider);
					if (sharedSettingsProvider == null)
						continue;

					var properties = GetPropertiesForProvider(settings, settingsProvider);
					sharedSettingsProvider.UpgradeSharedPropertyValues(settings.Context, properties, previousExeConfigFilename);
				}
			}

			SaveIfDirty(settings);
			//Need to call Reload because changes to shared settings aren't automatically realized by the .NET settings framework.
			settings.Reload();
		}

		/// <summary>
		/// Converts a lambda property de-referencing expression to a <see cref="MemberInfo"/>.
		/// </summary>
		/// <remarks>
		/// Given a lambda expression of the form <code>a => a.B</code>, this method returns the <see cref="MemberInfo"/>
		/// that represents the property B on the object a.  Only one level of de-referencing is supported.
		/// </remarks>
		/// <typeparam name="TItem"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="lambda"></param>
		/// <returns></returns>
		private static MemberInfo ConvertPropertyExpressionToMember<TItem, TProperty>(Expression<Func<TItem, TProperty>> lambda)
		{
			// stole this code from .net framework (see system.data.linq.DataLoadOptions, method LoadWith)
			var body = lambda.Body;
			if (body.NodeType == ExpressionType.Convert || body.NodeType == ExpressionType.ConvertChecked)
			{
				body = ((UnaryExpression)body).Operand;
			}
			var memberExpr = body as MemberExpression;
			if (memberExpr == null || memberExpr.Expression.NodeType != ExpressionType.Parameter)
			{
				throw new InvalidOperationException(string.Format("{0} is not a valid property expression.", lambda));
			}
			return memberExpr.Member;
		}

		#endregion
	}
}