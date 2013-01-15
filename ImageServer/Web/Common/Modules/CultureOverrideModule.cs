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
using System.Globalization;
using System.Threading;
using System.Web;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common.Modules
{
    /// <summary>
    /// This class is intented for fixing the localization issue when browser only sends the language (and not the culture) in the header
    /// .NET 4 support neutral culture but we are using .NET 3.5
    /// </summary>
    class CultureOverrideModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += PreRequestHandlerExecute;
        }

        void PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (CultureInfo.CurrentUICulture.IsNeutralCulture)
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = GetDefaultCulture(CultureInfo.CurrentUICulture.Name);
            }
        }

        public void Dispose()
        {
            
        }

        static CultureInfo GetDefaultCulture(string language)
        {
            try
            {
                // Use CreateSpecificCulture to create culture-specific CultureInfo
                // CultureInfo.CreateSpecificCulture("en") will return an "en-US" CultureInfo
                //
                // CreateSpecificCulture does have a few caveats. The first is that it will return an ARBITRARY culture that has the requested language.  For en it will return en-US even if your user's in Australia or the Canada.
                // 
                // The second issue is that Chinese (zh) will throw an exception instead of returning a specific culture because its not possible to pick a geopolitically correct culture for Chinese
                //
                // Note: .NET 4 does not have problem with neutral culture
                // It picks the culture which has majority population for the language (eg, Spain for Spanish "es")
                return CultureInfo.CreateSpecificCulture(language);
            }
            catch(Exception ex)
            {
                // fallback
                Platform.Log(LogLevel.Debug, ex, "Unable to determine the culture for '{0}' language. Fallback to English.", language);
                return new CultureInfo("en-US");
            }
        }
    }
}
