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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Ris.Application.Common;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof (ConfigurationPageProviderExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class ConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.HomePage.View)
			    || LoginSession.Current == null
			    || !LoginSession.Current.IsStaff)
				yield break;

			yield return new ConfigurationPage<HomepageConfigurationComponent>("TitleHomepage");
		}

		#endregion
	}

	/// <summary>
	/// Extension point for views onto <see cref="HomepageConfigurationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class HomepageConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// HomepageConfigurationComponent class.
	/// </summary>
	[AssociateView(typeof (HomepageConfigurationComponentViewExtensionPoint))]
	public class HomepageConfigurationComponent : ConfigurationApplicationComponent
	{
		public override void Save()
		{
			HomePageSettings.Default.Save();
		}

		#region Presentation Model

		public bool ShowHomepageOnStartUp
		{
			get { return HomePageSettings.Default.ShowHomepageOnStartUp; }
			set
			{
				HomePageSettings.Default.ShowHomepageOnStartUp = value;
				if (!HomePageSettings.Default.ShowHomepageOnStartUp)
				{
					// it does not make sense to prevent the homepage from closing,
					// unless it is shown on startup
					HomePageSettings.Default.PreventHomepageFromClosing = false;

					NotifyPropertyChanged("PreventHomepageFromClosing");
				}
				NotifyPropertyChanged("PreventHomepageFromClosingEnabled");
				this.Modified = true;
			}
		}

		public bool PreventHomepageFromClosing
		{
			get { return HomePageSettings.Default.PreventHomepageFromClosing; }
			set
			{
				HomePageSettings.Default.PreventHomepageFromClosing = value;
				this.Modified = true;
			}
		}

		public bool PreventHomepageFromClosingEnabled
		{
			get
			{
				// it does not make sense to even have the option of preventing the homepage from closing,
				// unless it is shown on startup
				return HomePageSettings.Default.ShowHomepageOnStartUp;
			}
		}

		#endregion
	}
}