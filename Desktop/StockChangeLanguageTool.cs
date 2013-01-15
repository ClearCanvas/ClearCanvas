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
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	partial class StockDesktopTools
	{
		[ExtensionOf(typeof (DesktopToolExtensionPoint), Enabled = false)]
		internal sealed class ChangeLanguageTool : Tool<IDesktopToolContext>
		{
			private LanguagePickerAction _action;
			private IActionSet _actionSet;

			public override IActionSet Actions
			{
				get
				{
					if (_actionSet == null)
					{
						_action = new LanguagePickerAction(@"changeLanguage", @"global-menus/MenuLanguage", new ResourceResolver(GetType(), false))
						          	{
						          		GroupHint = new GroupHint(@"Application.Languages"),
						          		SelectedLocale = InstalledLocales.Instance.Selected,
						          		Visible = InstalledLocales.Instance.Count > 1
						          	};
						_action.SelectedLocaleChanged += OnSelectedLocaleChanged;
						_actionSet = new ActionSet(new IAction[] {_action});
					}
					return _actionSet;
				}
			}

			private void OnSelectedLocaleChanged(object sender, EventArgs e)
			{
				try
				{
					var locale = _action.SelectedLocale;
					InstalledLocales.Instance.Selected = locale;
					Platform.Log(LogLevel.Debug, @"Locale changed to {0}", locale.Culture);

					if (Application.CurrentUICulture != locale.GetCultureInfo() && ConfirmLanguageChangeRestart(Context.DesktopWindow, locale))
					{
						Platform.Log(LogLevel.Debug, @"Restarting application from {0}", Assembly.GetEntryAssembly().Location);
						Process.Start(Assembly.GetEntryAssembly().Location);
						Application.Shutdown();
					}
				}
				catch (Exception ex)
				{
					ExceptionHandler.Report(ex, Context.DesktopWindow);
				}
			}

			private static bool ConfirmLanguageChangeRestart(IDesktopWindow desktopWindow, InstalledLocales.Locale locale)
			{
				var oldUiCulture = Thread.CurrentThread.CurrentUICulture;
				Thread.CurrentThread.CurrentUICulture = locale.GetCultureInfo();
				try
				{
					return desktopWindow.ShowMessageBox(SR.MessageDisplayLanguageChangedRestartRequired, MessageBoxActions.YesNo) == DialogBoxAction.Yes;
				}
				finally
				{
					Thread.CurrentThread.CurrentUICulture = oldUiCulture;
				}
			}
		}
	}
}