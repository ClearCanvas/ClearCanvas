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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	[ExtensionPoint]
	public sealed class LanguagePickerActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	[AssociateView(typeof (LanguagePickerActionViewExtensionPoint))]
	public class LanguagePickerAction : Action
	{
		private InstalledLocales.Locale _selectedLocale;
		private event EventHandler _selectedLocaleChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionId">The logical action ID.</param>
		/// <param name="actionPath">The action path.</param>
		/// <param name="resourceResolver">A resource resolver that will be used to resolve icons associated with this action.</param>
		public LanguagePickerAction(string actionId, string actionPath, IResourceResolver resourceResolver)
			: base(actionId, new ActionPath(actionPath, resourceResolver), resourceResolver) {}

		public InstalledLocales.Locale SelectedLocale
		{
			get { return _selectedLocale; }
			set
			{
				if (_selectedLocale != value)
				{
					_selectedLocale = value;
					EventsHelper.Fire(_selectedLocaleChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedLocaleChanged
		{
			add { _selectedLocaleChanged += value; }
			remove { _selectedLocaleChanged -= value; }
		}
	}
}