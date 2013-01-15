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

namespace ClearCanvas.Common
{
    /// <summary>
    /// Attribute used to mark an assembly as being a ClearCanvas Plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class PluginAttribute : Attribute
    {
        private string _name;
        private string _description;
    	private string _icon;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PluginAttribute()
		{
		}
		
    	/// <summary>
        /// A friendly name for the plugin.  
        /// </summary>
        /// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

		/// <summary>
		/// A friendly description for the plugin.  
		/// </summary>
		/// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
		public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

		/// <summary>
		/// The name of an icon resource to associate with the plugin.
		/// </summary>
    	public string Icon
    	{
			get { return _icon; }
			set { _icon = value; }
    	}
    }
}
