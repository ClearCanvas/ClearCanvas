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
using System.Linq;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    public static class WebControlExtensions
    {
        public static void AddCssClass(this System.Web.UI.WebControls.WebControl ctrl, string cssClass)
        {
            if (string.IsNullOrEmpty(ctrl.CssClass))
                ctrl.CssClass = cssClass;

            var list = ctrl.CssClass.Split(' ');
            var alreadyIn = list.Any(c => c.Equals(cssClass, StringComparison.InvariantCultureIgnoreCase));
            if (!alreadyIn)
                ctrl.CssClass += " " + cssClass;
        }

        public static void RemoveCssClass(this System.Web.UI.WebControls.WebControl ctrl, string cssClass)
        {
            if (string.IsNullOrEmpty(ctrl.CssClass)) return;

            var list = ctrl.CssClass.Split(' ');
            var finalCss = string.Empty;
            foreach(var css in list)
            {
                if (css.Equals(cssClass, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (string.IsNullOrEmpty(finalCss))
                    finalCss += css;
                else
                    finalCss += " " + css;
            }

            ctrl.CssClass = finalCss;

        }
    }
}