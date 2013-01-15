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
using System.Reflection;
using System.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    public static class HtmlUtility
    {
        public static string ConditionalString(bool condition, string s1, string s2)
        {
            return condition ? s1 : s2;
        }

    	///
    	/// Encode a string so that it is suitable for rendering in an Html page.
    	/// Also ensure all Xml escape characters are encoded properly.
        public static string Encode(string text)
        {
            if (text == null) return string.Empty;
            String encodedText = new SecurityElement("dummy", SecurityElement.Escape(text)).Text; //decode any escaped xml characters.
            return HttpUtility.HtmlEncode(encodedText).Replace(Environment.NewLine, "<BR/>");
            
        }

        /// <summary>
        /// Returns the <see cref="EnumInfoAttribute"/> of an enum value, if it's defined.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EnumInfoAttribute GetEnumInfo<TEnum>(TEnum value)
            where TEnum:struct
        {
            FieldInfo field = typeof (TEnum).GetField(value.ToString());
            if (field != null)
            {
                return AttributeUtils.GetAttribute<EnumInfoAttribute>(field);
            }
            else
                return null;
        }

        public static string GetEvalValue(object item, string itemName, string defaultValue)
        {
            string value = DataBinder.Eval(item, itemName, "");

            if (value == null || value.Equals("")) return defaultValue;
            else return value;
        }

         public static void AddCssClass(WebControl control, string cssClass)
         {
             control.CssClass += " " + cssClass;
         }
         
        public static void RemoveCssClass(WebControl control, string cssClass)
        {
            control.CssClass = control.CssClass.Replace(" " + cssClass, "");
        }

        public static String ResolveStudyDetailsUrl(Page page, String serverAE, String studyUid)
        {
            return String.Format("{0}?serverae={1}&siuid={2}",
                page.ResolveClientUrl(ImageServerConstants.PageURLs.StudyDetailsPage),
                serverAE, studyUid);
        }

        public static String ResolveWorkQueueDetailsUrl(Page page, String workQueueKey)
        {
            return String.Format("{0}?uid={1}",
                page.ResolveClientUrl(ImageServerConstants.PageURLs.WorkQueueItemDetailsPage), workQueueKey);
        }
    }


}
