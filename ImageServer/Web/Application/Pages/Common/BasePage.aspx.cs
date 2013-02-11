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
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.HtmlControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Web.Common;
using Resources;
using ClearCanvas.ImageServer.Web.Common.Extensions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Common
{
    /// <summary>
    /// Base class for all the pages.
    /// </summary>
    /// <remarks>
    /// Derive new page from this class to ensure consistent look across all pages.
    /// </remarks>
    public partial class BasePage : System.Web.UI.Page
    {
        private bool _extensionLoaded;
        protected List<object> Extensions = new List<object>();

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            LoadExtensions();

            ThemeManager.ApplyTheme(this);

            // This is necessary because Safari and Chrome browsers don't display the Menu control correctly.
            if (Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) != -1)
                Page.ClientTarget = "uplevel";
        
        }

        private void LoadExtensions()
        {
            lock (Extensions)
            {
                if (!_extensionLoaded)
                {
                    try
                    {
                        Extensions.Clear();

                        var attrs = ClearCanvas.Common.Utilities.AttributeUtils.GetAttributes<ExtensibleAttribute>(this.GetType(), true);
                        foreach (var attr in attrs)
                        {
                            var xp = Activator.CreateInstance(attr.ExtensionPoint);
                            Extensions.AddRange((xp as ExtensionPoint).CreateExtensions());
                        }
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Error, ex, "Unable to load page extension");
                    }
                }
                
            }
            
        }
        
        protected void SetPageTitle(string title)
        {
            SetPageTitle(title, true);
        }

        private static string GetProductInformation()
        {
            var tags = new List<string>();
            if (!string.IsNullOrEmpty(ProductInformation.Release))
                tags.Add(Titles.LabelNotForDiagnosticUse);
            if (!ServerPlatform.IsManifestVerified)
                tags.Add(ConstantResourceManager.ModifiedInstallation);

            var name = ProductInformation.GetName(false, true);
            if (tags.Count == 0)
                return name;

            var tagString = string.Join(" | ", tags.ToArray());
            return string.IsNullOrEmpty(name) ? tagString : string.Format("{0} - {1}", name, tagString);
        }

        protected void SetPageTitle(string title, bool includeProductInfo)
        {
            if (includeProductInfo)
            {
                Page.Title = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"])
                                 ? String.Format(title, GetProductInformation())
                                 : String.Format(title, GetProductInformation()) + " [" +
                                   ConfigurationManager.AppSettings["ServerName"] + "]";
            }
            else
                Page.Title = string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerName"])
                    ? title
                    : title + " [" + ConfigurationManager.AppSettings["ServerName"] + "]";
		}

        protected void ForeachExtension<T>(Action<T> action)
        {
            if (Extensions!=null)
            {
                foreach(var ex in Extensions)
                {
                    if (ex is T)
                    {
                        action((T) ex);
                    }
                }
            }
        }

        protected void ForceDocumentMode(string docMode)
        {
            if (base.Master == null)
                return;

            var meta = base.Master.Page.Header.Controls.OfType<HtmlMeta>().First();
            if (meta != null)
                meta.Content = docMode;
        }
    }
}