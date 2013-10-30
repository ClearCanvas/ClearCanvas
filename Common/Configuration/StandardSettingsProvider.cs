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
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// This class is the standard settings provider that should be used by all settings classes that operate
	/// within the ClearCanvas framework.
	/// </summary>
	/// <remarks>
	/// Internally, this class will delegate the storage of settings between
	/// the local file system and an implemetation of <see cref="SettingsStoreExtensionPoint"/>,
	/// if an extension is found.  All methods on this class are thread-safe, as per MSDN guidelines.
	/// </remarks>
	public class StandardSettingsProvider : SettingsProvider, IApplicationSettingsProvider, ISharedApplicationSettingsProvider
	{
		private string _appName;
		private SettingsProvider _sourceProvider;
		private readonly object _syncLock = new object();

		/// <summary>
		/// Constructor.
		/// </summary>
		public StandardSettingsProvider()
		{
			// according to MSDN recommendation, use the name of the executing assembly here
			_appName = Assembly.GetExecutingAssembly().GetName().Name;
		}

		static StandardSettingsProvider()
		{
			IsLocal = new SettingsStoreExtensionPoint().ListExtensions().Length == 0;
		}

		internal static bool IsLocal { get; private set; }

		#region SettingsProvider overrides

		/// <summary>
		/// Gets the Application Name used to initialize the settings subsystem.
		/// </summary>
		public override string ApplicationName
		{
			get { return _appName; }
			set { _appName = value; }
		}

		///<summary>
		///Initializes the provider.
		///</summary>
		///
		///<param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		///<param name="name">The friendly name of the provider.</param>
		public override void Initialize(string name, NameValueCollection config)
		{
			lock (_syncLock)
			{
				// obtain a source provider
				if (SettingsStore.IsSupported)
				{
					try
					{
						ISettingsStore store = SettingsStore.Create();
						_sourceProvider = new SettingsStoreSettingsProvider(store);
					}
					catch (NotSupportedException ex)
					{
						// TODO: determine if we can safely treat other exceptions the same way here
						Platform.Log(LogLevel.Warn, ex, "Configuration store failed to initialize - defaulting to LocalFileSettingsProvider");

						// default to LocalFileSettingsProvider as a last resort
						_sourceProvider = new ExtendedLocalFileSettingsProvider(new LocalFileSettingsProvider());
					}
				}
				else
				{
					_sourceProvider = new ExtendedLocalFileSettingsProvider(new LocalFileSettingsProvider());
				}

				// init source provider
				// according to sample implementations, use the application name here
				_sourceProvider.Initialize(this.ApplicationName, config);
				base.Initialize(this.ApplicationName, config);
			}
		}

		///<summary>
		///Returns the collection of settings property values for the specified application instance and settings property group.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Configuration.SettingsPropertyValueCollection"></see> containing the values for the specified settings property group.
		///</returns>
		///
		///<param name="context">A <see cref="T:System.Configuration.SettingsContext"></see> describing the current application use.</param>
		///<param name="props">A <see cref="T:System.Configuration.SettingsPropertyCollection"></see> containing the settings property group whose values are to be retrieved.</param><filterpriority>2</filterpriority>
		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
		{
			lock (_syncLock)
			{
				Type settingsClass = (Type) context["SettingsClassType"];
				SettingsPropertyValueCollection values = _sourceProvider.GetPropertyValues(context, props);
				TranslateValues(settingsClass, values);
				return values;
			}
		}

		///<summary>
		///Sets the values of the specified group of property settings.
		///</summary>
		///
		///<param name="context">A <see cref="T:System.Configuration.SettingsContext"></see> describing the current application usage.</param>
		///<param name="settings">A <see cref="T:System.Configuration.SettingsPropertyValueCollection"></see> representing the group of property settings to set.</param><filterpriority>2</filterpriority>
		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection settings)
		{
			lock (_syncLock)
			{
				_sourceProvider.SetPropertyValues(context, settings);
			}
		}

		#endregion

		#region IApplicationSettingsProvider Members

		///<summary>
		///Returns the value of the specified settings property for the previous version of the same application.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Configuration.SettingsPropertyValue"></see> containing the value of the specified property setting as it was last set in the previous version of the application; or null if the setting cannot be found.
		///</returns>
		///
		///<param name="context">A <see cref="T:System.Configuration.SettingsContext"></see> describing the current application usage.</param>
		///<param name="property">The <see cref="T:System.Configuration.SettingsProperty"></see> whose value is to be returned.</param><filterpriority>2</filterpriority>
		public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
		{
			lock (_syncLock)
			{
				if (_sourceProvider is IApplicationSettingsProvider)
				{
					return ((IApplicationSettingsProvider) _sourceProvider).GetPreviousVersion(context, property);
				}
				else
				{
					// fail silently as per MSDN 
					return new SettingsPropertyValue(property);
				}
			}
		}

		///<summary>
		///Resets the application settings associated with the specified application to their default values.
		///</summary>
		///
		///<param name="context">A <see cref="T:System.Configuration.SettingsContext"></see> describing the current application usage.</param><filterpriority>2</filterpriority>
		public void Reset(SettingsContext context)
		{
			lock (_syncLock)
			{
				if (_sourceProvider is IApplicationSettingsProvider)
				{
					((IApplicationSettingsProvider) _sourceProvider).Reset(context);
				}
				else
				{
					// fail silently as per MSDN 
				}
			}
		}

		///<summary>
		///Indicates to the provider that the application has been upgraded. This offers the provider an opportunity to upgrade its stored settings as appropriate.
		///</summary>
		///
		///<param name="properties">A <see cref="T:System.Configuration.SettingsPropertyCollection"></see> containing the settings property group whose values are to be retrieved.</param>
		///<param name="context">A <see cref="T:System.Configuration.SettingsContext"></see> describing the current application usage.</param><filterpriority>2</filterpriority>
		public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			lock (_syncLock)
			{
				lock (_syncLock)
				{
					if (_sourceProvider is IApplicationSettingsProvider)
					{
						((IApplicationSettingsProvider) _sourceProvider).Upgrade(context, properties);
					}
					else
					{
						// fail silently as per MSDN 
					}
				}
			}
		}

		#endregion

		#region ISharedApplicationSettingsProvider Members

		public bool CanUpgradeSharedPropertyValues(SettingsContext context)
		{
			lock (_syncLock)
			{
				if (_sourceProvider is ISharedApplicationSettingsProvider)
					return ((ISharedApplicationSettingsProvider) _sourceProvider).CanUpgradeSharedPropertyValues(context);
			}

			return false;
		}

		public void UpgradeSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
		{
			lock (_syncLock)
			{
				if (_sourceProvider is ISharedApplicationSettingsProvider)
					((ISharedApplicationSettingsProvider) _sourceProvider).UpgradeSharedPropertyValues(context, properties, previousExeConfigFilename);
			}
		}

		public SettingsPropertyValueCollection GetPreviousSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
		{
			lock (_syncLock)
			{
				if (_sourceProvider is ISharedApplicationSettingsProvider)
				{
					var values = ((ISharedApplicationSettingsProvider) _sourceProvider).GetPreviousSharedPropertyValues(context, properties, previousExeConfigFilename);
					var settingsClass = (Type) context["SettingsClassType"];
					TranslateValues(settingsClass, values);
					return values;
				}

				return new SettingsPropertyValueCollection();
			}
		}

		public SettingsPropertyValueCollection GetSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
		{
			lock (_syncLock)
			{
				if (_sourceProvider is ISharedApplicationSettingsProvider)
				{
					var values = ((ISharedApplicationSettingsProvider) _sourceProvider).GetSharedPropertyValues(context, properties);
					var settingsClass = (Type) context["SettingsClassType"];
					TranslateValues(settingsClass, values);
					return values;
				}

				return new SettingsPropertyValueCollection();
			}
		}

		public void SetSharedPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
		{
			lock (_syncLock)
			{
				if (_sourceProvider is ISharedApplicationSettingsProvider)
					((ISharedApplicationSettingsProvider) _sourceProvider).SetSharedPropertyValues(context, values);
			}
		}

		#endregion

		private static void TranslateValues(Type settingsClass, SettingsPropertyValueCollection values)
		{
			foreach (SettingsPropertyValue value in values)
			{
				if (value.SerializedValue == null || (value.SerializedValue is string) && ((string) value.SerializedValue) == ((string) value.Property.DefaultValue))
					value.SerializedValue = SettingsClassMetaDataReader.TranslateDefaultValue(settingsClass, (string) value.Property.DefaultValue);
			}
		}
	}
}