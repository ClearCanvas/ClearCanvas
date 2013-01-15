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

using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.Standard
{
	/// <summary>
	/// Provides common <see cref="IConfigurationPage"/>s for settings defined in the framework.
	/// </summary>
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	internal class StandardOptionsConfigurationPageProvider : IConfigurationPageProvider
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public StandardOptionsConfigurationPageProvider()
		{ 
		}

		#region IConfigurationPageProvider Members

		/// <summary>
		/// Gets all the <see cref="IConfigurationPage"/>s for this provider.
		/// </summary>
		public IEnumerable<IConfigurationPage> GetPages()
		{
			var listPages = new List<IConfigurationPage>();

			if(CheckPermission(AuthorityTokens.Desktop.CustomizeDateTimeFormat))
			{
				listPages.Add(new ConfigurationPage<DateFormatApplicationComponent>("TitleDateFormat"));
			}

			listPages.Add(new ConfigurationPage<ToolbarConfigurationComponent>("TitleToolbar"));
		
			return listPages.AsReadOnly();
		}

		#endregion

		private static bool CheckPermission(string authorityToken)
		{
			// if the thread is running in a non-authenticated mode, then we have no choice but to allow.
			// this seems a little counter-intuitive, but basically we're counting on the fact that if
			// the desktop is running in an enterprise environment, then the thread *will* be authenticated,
			// and that this is enforced by some mechanism outside the scope of this class.  The only
			// scenario in which the thread would ever be unauthenticated is the stand-alone scenario.
			return Thread.CurrentPrincipal == null || Thread.CurrentPrincipal.Identity.IsAuthenticated == false
			       || Thread.CurrentPrincipal.IsInRole(authorityToken);
		}
	}
}
