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
using System.Web;
using ClearCanvas.Common;
using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Common
{
    public interface IThemeManager
    {
        string GetTheme(Page page);
        string GetDefaultTheme();
    }

    [ExtensionPoint]
    public class ThemeManagerExtensionPoint : ExtensionPoint<IThemeManager>
    {
    }

    public static class ThemeManager
    {
        private static readonly IThemeManager _manager;
        static ThemeManager()
        {
            try
            {
                var xp = new ThemeManagerExtensionPoint();
                _manager = xp.CreateExtension() as IThemeManager;
            }
            catch(Exception ex)
            {
                Platform.Log(LogLevel.Debug, "Unable to find theme manager. {0}", ex.Message);
            }
        }

        public static string CurrentTheme
        {
            get
            {
                var existingTheme = HttpContext.Current.Items["Theme"] as string;
                if (!string.IsNullOrEmpty(existingTheme))
                {
                    return existingTheme;
                }
                else
                {
                    var theme = _manager != null ? _manager.GetTheme(HttpContext.Current.Handler as Page) : ImageServerConstants.Default;
                    HttpContext.Current.Items["Theme"] = theme;
                    return theme;
                }

            }
        }

        public static void ApplyTheme(Page page)
        {
            if (page!=null)
            {
                var existingTheme = HttpContext.Current.Items["Theme"] as string;
                if (!string.IsNullOrEmpty(existingTheme))
                {
                    page.Theme = existingTheme;
                }
                else
                {
                    page.Theme = _manager != null ? _manager.GetTheme(page) : ImageServerConstants.Default;
                    HttpContext.Current.Items["Theme"] = page.Theme;
                }
                
            }
        }

        public static string GetDefaultTheme()
        {
            return _manager != null ? _manager.GetDefaultTheme() : ImageServerConstants.Default;
        }
    }
}