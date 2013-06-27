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

using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
    public class ExtendedLocalFileSettingsProvider : SettingsProvider, IApplicationSettingsProvider, ISharedApplicationSettingsProvider
    {
        private readonly LocalFileSettingsProvider _provider;
        private readonly bool _ownProvider;
        private string _appName;

        public ExtendedLocalFileSettingsProvider()
            : this(new LocalFileSettingsProvider())
        {
            _ownProvider = true;
        }

        public ExtendedLocalFileSettingsProvider(LocalFileSettingsProvider provider)
        {
            _provider = provider;
            _appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }

        /// <summary>
        /// Hack to allow shared settings migration to redirect to a different exe's config file.
        /// </summary>
        public static string ExeConfigFileName { get; set; }

        public override string ApplicationName
        {
            get { return _appName; }
            set { _appName = value; }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            return _provider.GetPropertyValues(context, collection);
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (string.IsNullOrEmpty(name))
                name = ApplicationName;

            if (_ownProvider)
            {
                //Otherwise it's already been initialized and we're just temporarily wrapping it.
                _provider.Initialize(null, null);
            }

            base.Initialize(name, config);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            _provider.SetPropertyValues(context, collection);
        }

        #region IApplicationSettingsProvider Members

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            return _provider.GetPreviousVersion(context, property);
        }

        public void Reset(SettingsContext context)
        {
            _provider.Reset(context);
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            _provider.Upgrade(context, properties);
        }

        #endregion

        #region ISharedApplicationSettingsProvider Members

		public bool CanUpgradeSharedPropertyValues(SettingsContext context)
		{
			return true; //just let them get overwritten.
		}

    	public void UpgradeSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
            LocalFileSettingsProviderExtensions.UpgradeSharedPropertyValues(_provider, context, properties, previousExeConfigFilename, ExeConfigFileName);
        }

        public SettingsPropertyValueCollection GetPreviousSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
            return LocalFileSettingsProviderExtensions.GetPreviousSharedPropertyValues(_provider, context, properties, previousExeConfigFilename);
        }

        public SettingsPropertyValueCollection GetSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            return LocalFileSettingsProviderExtensions.GetSharedPropertyValues(_provider, context, properties, ExeConfigFileName);
        }

        public void SetSharedPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            LocalFileSettingsProviderExtensions.SetSharedPropertyValues(_provider, context, values, ExeConfigFileName);
        }

        #endregion
    }
}
