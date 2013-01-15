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
using System.Linq;
using System.Text;
using System.Web;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    public static class UsersGuideLinkHelper
    {
        public static string HomeUrl
        {
            get
            {
                return VirtualPathUtility.ToAbsolute(string.Format("~/Pages/Help/{0}", ClearCanvas.ImageServer.Web.Common.PageSettings.AboutPage.Default.UsersGuideUrl));
            }
        }

        public static string GetUrlTo(string topicID)
        {
            return string.Format("{0}?{1}.htm", HomeUrl, topicID);
        }

        public static string GetUrlTo(string topicID, string anchorID)
        {
            return string.Format("{0}#{1}", GetUrlTo(topicID), anchorID);
        }

    }
}
