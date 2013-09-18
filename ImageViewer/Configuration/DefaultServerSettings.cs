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

namespace ClearCanvas.ImageViewer.Configuration
{
    /// TODO (CR Jun 2012): Remove this, or keep around for upgrade? It would be going from a user setting to a shared setting, which maybe isn't so good.
    //TODO (Marmot): Migration, custom user upgrade step?

	[SettingsGroupDescription("Stores a list of default servers for the application.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class DefaultServerSettings : ApplicationSettingsBase
	{
        private static DefaultServerSettings _default = ((DefaultServerSettings)Synchronized(new DefaultServerSettings()));
        
        private DefaultServerSettings()
		{
		}

	    public static DefaultServerSettings Default
	    {
	        get { return _default; }
	    }

	    /// <summary>
        /// Server tree paths to the user&apos;s default servers.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Server tree paths to the user\'s default servers.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection DefaultServerPaths
        {
            get
            {
                return ((global::System.Collections.Specialized.StringCollection)(this["DefaultServerPaths"]));
            }
            set
            {
                this["DefaultServerPaths"] = value;
            }
        }
    }
}