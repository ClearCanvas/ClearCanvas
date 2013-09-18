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
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Provides application settings for banner.
    /// </summary>
    /// <remarks>
    /// This code is adapted from the Visual Studio generated template code;  the generated code has been removed from the project.  Additional 
    /// settings need to be manually added to this class.
    /// </remarks>
    [SettingsGroupDescription("Settings that configure the display of the patient banner.")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    public sealed class BannerSettings : global::System.Configuration.ApplicationSettingsBase
    {
        private static BannerSettings defaultInstance = ((BannerSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new BannerSettings())));

        private BannerSettings()
        {
        }

        public static BannerSettings Default {
            get {
                return defaultInstance;
            }
        }

        /// <summary>
        /// Defined the height of banner in pixels.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Defined the height of banner in pixels.")]
        [global::System.Configuration.DefaultSettingValueAttribute("95")]
        public int BannerHeight {
            get {
                return ((int)(this["BannerHeight"]));
            }
        }
    }
}
