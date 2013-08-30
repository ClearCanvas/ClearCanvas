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
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    [SettingsGroupDescriptionAttribute("Configures the Address Editor component.")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class AddressEditorComponentSettings
    {

        private AddressEditorComponentSettings()
        {
        }

        public ICollection<string> CountryChoices
        {
            get
            {
                return CollectionUtils.Map<string, string>(this.Countries.Split(','),
                   delegate(string s) { return s.Trim(); });
            }
        }

        public ICollection<string> ProvinceChoices
        {
            get
            {
                return CollectionUtils.Map<string, string>(this.Provinces.Split(','),
                   delegate(string s) { return s.Trim(); });
            }
        }



    }
}
