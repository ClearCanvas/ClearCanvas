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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration
{
    /// <summary>
    /// An extension point for <see cref="IConfigurationPageProvider"/>s.
    /// </summary>
    [ExtensionPoint]
    public sealed class ConfigurationPageProviderExtensionPoint : ExtensionPoint<IConfigurationPageProvider>
    {
    }

	public static class ConfigurationDialog
	{
	    /// <summary>
        /// Shows all <see cref="IConfigurationPage"/>s returned by extensions of <see cref="ConfigurationPageProviderExtensionPoint"/>
		/// in a dialog, with a navigable tree to select the pages.
		/// </summary>
		public static ApplicationComponentExitCode Show(IDesktopWindow desktopWindow)
		{
			return Show(desktopWindow, null);
		}

		/// <summary>
        /// Shows all <see cref="IConfigurationPage"/>s returned by extensions of <see cref="ConfigurationPageProviderExtensionPoint"/>
		/// in a dialog, with a navigable tree to select the pages.
		/// </summary>
		public static ApplicationComponentExitCode Show(IDesktopWindow desktopWindow, string initialPageIdentifier)
		{
            var container = new ConfigurationDialogComponent(GetPages(), initialPageIdentifier);
			var exitCode = ApplicationComponent.LaunchAsDialog(desktopWindow, container, SR.TitleMenuOptions);

			return exitCode;
		}

        private static IEnumerable<IConfigurationPage> GetPages()
        {
            try
            {
                return new ConfigurationPageProviderExtensionPoint().CreateExtensions()
                    .Cast<IConfigurationPageProvider>().SelectMany(p => p.GetPages()).ToList();
            }
            catch (NotSupportedException)
            {
                return new IConfigurationPage[0];
            }
        }
	}
}
