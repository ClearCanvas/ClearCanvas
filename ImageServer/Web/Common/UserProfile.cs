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

using System.Web.Security;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Web.Common.Security;
using ImageServerAuthorityTokens = ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens;
namespace ClearCanvas.ImageServer.Web.Common
{
    public static class UserProfile
    {
        /// <summary>
        /// Returns the default url for the current user
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultUrl()
        {
            //TODO: Use a mapping file similar to SiteMap to specify the default home page based on the authority tokens that user has.
            
            if (SessionManager.Current == null)
            {
                // user has not logged in
                FormsAuthentication.RedirectToLoginPage();
				// Need Response.End here, per this link:  http://www.neilpullinger.co.uk/2007/07/always-use-responseend-after.html
            	return null;
            }

			if (SessionManager.Current.User == null)
				return ImageServerConstants.PageURLs.SearchPage;

            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.Admin.Dashboard.View))
                return ImageServerConstants.PageURLs.DashboardPage;

            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.Study.Search))
                return ImageServerConstants.PageURLs.SearchPage;

            if (SessionManager.Current.User.IsInRole(AuthorityTokens.Admin.Security.User))
                return ImageServerConstants.PageURLs.AdminUserPage;
            
            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.WorkQueue.Search))
                return ImageServerConstants.PageURLs.WorkQueuePage;
           
            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.StudyIntegrityQueue.Search))
                return ImageServerConstants.PageURLs.StudyIntegrityQueuePage;
                    
            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.ArchiveQueue.Search))
                return ImageServerConstants.PageURLs.ArchiveQueuePage;

            if (SessionManager.Current.User.IsInRole(ImageServerAuthorityTokens.RestoreQueue.Search))
                return ImageServerConstants.PageURLs.RestoreQueuePage;

            return ImageServerConstants.PageURLs.AboutPage;
            
        }
    }
}
