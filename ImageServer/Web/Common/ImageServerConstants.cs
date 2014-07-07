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

using System.Threading;
using ClearCanvas.ImageServer.Web.Common;

public class ImageServerConstants
    {
        #region GridViewPagerPosition enum

        public enum GridViewPagerPosition
        {
            Top = 0,
            Bottom = 1,
        } ;

        #endregion

        public const string CookieDateTimeFormat = "yyyy-M-d H:m:s"; //TODO: get rid of this, use epoch instead
        public const string DefaultApplicationName = "ImageServer";
        public const string Default = "Default";
        public const string DefaultConfigurationXml = "<HsmArchive><RootDir>e:\\Archive</RootDir></HsmArchive>";
        public const string First = "first";
        public const string High = "high";
        public const string ImagePng = "image/png";
        public const string Last = "last";
        public const string Low = "low";
        public const string Next = "next";
        public const string PagerItemCount = "ItemCount";
        public const string Pct = "pct";
        public const string Prev = "prev";

        public static string[] ReasonCommentSeparator = {"::"};
    
        /// <summary>
        /// Sets the current theme.
        /// </summary>
        /// TODO: This is not static. Consider moving out of this class.
        public static string Theme
        {
            get
            {
                return ThemeManager.CurrentTheme;
            }
        }

        #region Nested type: ContextKeys

        public class ContextKeys
        {
            public const string ErrorDescription = "ERROR_DESCRIPTION";
            public const string ErrorMessage = "ERROR_MESSAGE";
            public const string StackTrace = "STACK_TRACE";
        }

        #endregion

        #region Nested type: ImageURLs

        /// TODO: Not static. Consider moving out of this class.
        public class ImageURLs
        {
            private static string LocalizedRootImageUrl
            {
                get
                {
                    var culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.StartsWith("en")
                        ? "" 
                        : Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                    return string.Format("~/App_Themes/{0}/{1}/images/", Theme, culture);
                }
            }

            public static string IdeographyName
            {
                get { return LocalizedRootImageUrl + "Indicators/IdeographicName.gif"; }
            }

            public static string PhoneticName
            {
                get { return LocalizedRootImageUrl + "Indicators/PhoneticName.gif"; }
            }

            public const string AddButtonDisabled = "images/Buttons/AddDisabled.png";
            public const string AddButtonEnabled = "images/Buttons/AddEnabled.png";
            public const string AddButtonHover = "images/Buttons/AddHover.png";
            public const string UpdateButtonDisabled = "images/Buttons/UpdateDisabled.png";
            public const string UpdateButtonEnabled = "images/Buttons/UpdateEnabled.png";
            public const string UpdateButtonHover = "images/Buttons/UpdateHover.png";

            public static readonly string AcceptKOPRFeature =
                string.Format("~/App_Themes/{0}/images/Indicators/AcceptKOPRFeature.png", Theme);

            public static readonly string AutoRouteFeature =
                string.Format("~/App_Themes/{0}/images/Indicators/AutoRouteFeature.png", Theme);

            public static readonly string Blank = string.Format("~/App_Themes/{0}/images/blank.gif", Theme);

            public static readonly string CalendarIcon =
                string.Format("~/App_Themes/{0}/images/Buttons/CalendarIcon.png", Theme);

            public static readonly string Checked = string.Format("~/App_Themes/{0}/images/Indicators/checked.png",
                                                                  Theme);

            public static readonly string GridPagerFirstDisabled =
                string.Format("~/App_Themes/{0}/images/Controls/GridView/GridViewPagerFirstDisabled.png", Theme);

            public static readonly string GridPagerFirstEnabled =
                string.Format("~/App_Themes/{0}/images/Controls/GridView/GridViewPagerFirstEnabled.png", Theme);

            public static readonly string GridPagerLastDisabled =
                string.Format("~/App_Themes/{0}/images/Controls/GridView/GridViewPagerLastDisabled.png", Theme);

            public static readonly string GridPagerLastEnabled =
                string.Format("~/App_Themes/{0}/images/Controls/GridView/GridViewPagerLastEnabled.png", Theme);

            public static readonly string GridPagerNextDisabled =
                string.Format("~/App_Themes/{0}/images/Controls/GridView/GridViewPagerNextDisabled.png", Theme);

            public static readonly string GridPagerNextEnabled =
                string.Format("~/App_Themes/{0}/images/Controls/GridView/GridViewPagerNextEnabled.png", Theme);

            public static readonly string GridPagerPreviousDisabled =
                string.Format("~/App_Themes/{0}/images/Controls/GridView/GridViewPagerPreviousDisabled.png", Theme);

            public static readonly string GridPagerPreviousEnabled =
                string.Format("~/App_Themes/{0}/images/Controls/GridView/GridViewPagerPreviousEnabled.png", Theme);

            public static readonly string WebViewerPagerFirstDisabled =
                string.Format("~/Pages/WebViewer/Images/WebViewerPagerFirstDisabled.png", Theme);

            public static readonly string WebViewerPagerFirstEnabled =
                string.Format("~/Pages/WebViewer/Images/WebViewerPagerFirstEnabled.png", Theme);

            public static readonly string WebViewerPagerLastDisabled =
                string.Format("~/Pages/WebViewer/Images/WebViewerPagerLastDisabled.png", Theme);

            public static readonly string WebViewerPagerLastEnabled =
                string.Format("~/Pages/WebViewer/Images/WebViewerPagerLastEnabled.png", Theme);

            public static readonly string WebViewerPagerNextDisabled =
                string.Format("~/Pages/WebViewer/Images/WebViewerPagerNextDisabled.png", Theme);

            public static readonly string WebViewerPagerNextEnabled =
                string.Format("~/Pages/WebViewer/Images/WebViewerPagerNextEnabled.png", Theme);

            public static readonly string WebViewerPagerPreviousDisabled =
                string.Format("~/Pages/WebViewer/Images/WebViewerPagerPreviousDisabled.png", Theme);

            public static readonly string WebViewerPagerPreviousEnabled =
                string.Format("~/Pages/WebViewer/Images/WebViewerPagerPreviousEnabled.png", Theme);

            public static readonly string QueryFeature =
                string.Format("~/App_Themes/{0}/images/Indicators/QueryFeature.png", Theme);

            public static readonly string RetrieveFeature =
                string.Format("~/App_Themes/{0}/images/Indicators/RetrieveFeature.png", Theme);

            public static readonly string StoreFeature =
                string.Format("~/App_Themes/{0}/images/Indicators/StoreFeature.png", Theme);

            public static readonly string Unchecked = string.Format("~/App_Themes/{0}/images/Indicators/unchecked.png",
                                                                    Theme);

            public static readonly string UsageBar = string.Format("~/App_Themes/{0}/images/Indicators/usage.png", Theme);

            public static readonly string Watermark = string.Format("~/App_Themes/{0}/images/Indicators/Watermark.gif",
                                                                    Theme);
        }

        #endregion

        #region Nested type: PageURLs

        public class PageURLs
        {
            public const string AboutPage = "~/Pages/Help/About.aspx";
            public const string AdminUserPage = "~/Pages/Admin/UserManagement/Users/Default.aspx";
            public const string ApplicationLog = "~/Pages/Admin/ApplicationLog/Default.aspx";
            public const string ArchiveQueuePage = "~/Pages/Queues/ArchiveQueue/Default.aspx";
            public const string AuthorizationErrorPage = "~/Pages/Error/AuthorizationErrorPage.aspx";
            public const string BarChartPage = "~/Pages/Common/BarChart.aspx?pct={0}&high={1}&low={2}";
            public const string CookiesErrorPage = "~/Pages/Error/CookiesRequired.aspx";
            public const string DashboardPage = "~/Pages/Admin/Dashboard/Default.aspx";
            public const string ErrorPage = "~/Pages/Error/ErrorPage.aspx";
            public const string JavascriptErrorPage = "~/Pages/Error/JavascriptRequired.aspx";
			public const string InvalidRequestErrorPage = "~/Pages/Error/InvalidRequestErrorPage.aspx";
            public const string LoginPage = "~/Pages/Login/Default.aspx";
			public const string LogoutPage = "~/Pages/Login/Logout.aspx?ReturnUrl={0}";
            public const string MoveSeriesPage = "~/Pages/Studies/MoveSeries/Default.aspx";
            public const string MoveStudyPage = "~/Pages/Studies/Move/Default.aspx";
            public const string NumberFormatScript = "~/Scripts/NumberFormat154.js";
            public const string RestoreQueuePage = "~/Pages/Queues/RestoreQueue/Default.aspx";
            public const string SearchPage = "~/Pages/Studies/Default.aspx";
            public const string SeriesDetailsPage = "~/Pages/Studies/SeriesDetails/Default.aspx";
            public const string StudiesPage = "~/Pages/Studies/Default.aspx";
            public const string StudyDetailsPage = "~/Pages/Studies/StudyDetails/Default.aspx";
            public const string StudyIntegrityQueuePage = "~/Pages/Queues/StudyIntegrityQueue/Default.aspx";
            public const string WebServices = "~/Services/webservice.htc";
            public const string WorkQueueItemDeletedPage = "~/Pages/Queues/WorkQueue/Edit/WorkQueueItemDeleted.aspx";
            public const string WorkQueueItemDetailsPage = "~/Pages/Queues/WorkQueue/Edit/Default.aspx";
            public const string WorkQueuePage = "~/Pages/Queues/WorkQueue/Default.aspx";
            
            public const string DefaultTimeoutPage = "~/Pages/Error/TimeoutErrorPage.aspx";
            public const string WebViewerTimeoutErrorPage = "~/Pages/Error/WebViewerTimeoutErrorPage.aspx";
            public const string WebViewerAuthorizationErrorPage = "~/Pages/Error/WebViewerAuthorizationErrorPage.aspx";
            public const string WebViewerErrorPage = "~/Pages/Error/WebViewerErrorPage.aspx";
            
            public const string WebViewerStandaloneSilverlightDefault = "~/Pages/WebViewer/Default.aspx";
            public const string WebViewerStandaloneHtml5Embedded = "~/Pages/WebViewer/ViewHtml/EmbeddedDefault.aspx";
            public const string WebViewerStandaloneHTML5Default = "~/Pages/WebViewer/ViewHtml/Default.aspx";
            public const string WebViewerStandaloneStudiesPage = "~/Pages/WebViewer/StudyList.aspx";

            public const string WebViewerIntegratedSilverlightDefault = "~/Pages/Studies/View/Default.aspx";
            public const string WebViewerIntegratedHtml5Embedded = "~/Pages/Studies/ViewHtml/EmbeddedDefault.aspx";
            public const string WebViewerIntegratedHTML5Default = "~/Pages/Studies/ViewHtml/Default.aspx";
        }

        #endregion

        #region Nested type: QueryStrings

        public class QueryStrings
        {
            public const string SeriesUID = "seriesuid";
            public const string ServerAE = "serverae";
            public const string StudyInstanceUID = "siuid";
            public const string StudyUID = "studyuid";
        }

        public class WebViewerQueryStrings
        {
            public const string ApplicationName = "application";
            public const string Username = "username";
            public const string Password = "password";
            public const string Aetitle = "aetitle";
            public const string Study = "study";
            public const string Session = "session";
            public const string WebViewerInitParams = "WebViewerInitParams";
            public const string ListStudies = "liststudies";
        }

        #endregion

		#region

		/// <summary>
        /// Names of the paramters passed to the viewer.
        /// </summary>
        public class WebViewerStartupParameters
        {
            public const string LANMode= "LANMode";
            public const string Username = "username";
            public const string Password = "password";
            public const string Session = "session";
			public const string IsSessionShared = "sessionshared";
			public const string TimeoutUrl = "TimeoutUrl";
            public const string Port = "Port";
            public const string InactivityTimeout = "InactivityTimeout";
            public const string LocalIPAddress = "LocalIPAddress";
		    public const string PatientID = "patientid";
		    public const string AccessionNumber = "accession";
            public const string Study = "study";
            public const string ListStudies = "liststudies";
            public const string AeTitle = "aetitle";
            public const string ApplicationName = "application";
            public const string Language = "Language";
        }
        	
        #endregion
    }