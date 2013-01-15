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

#if	UNIT_TESTS
#pragma warning disable 1591

using System.Collections.Generic;
using System.Configuration;

namespace ClearCanvas.Common.Configuration.Tests
{
	internal class SimpleSettingsStore
	{
		private enum Store
		{
			CurrentUser,
			PreviousUser,
			CurrentShared,
			PreviousShared
		}
		public static SimpleSettingsStore Instance  = new SimpleSettingsStore();

		private readonly Dictionary<Store, SettingsPropertyValueCollection> _stores;

		private SimpleSettingsStore()
		{
			_stores = new Dictionary<Store, SettingsPropertyValueCollection>();
			_stores[Store.CurrentUser] = CurrentUserValues = new SettingsPropertyValueCollection();
			_stores[Store.PreviousUser] = PreviousUserValues = new SettingsPropertyValueCollection();
			_stores[Store.CurrentShared] = CurrentSharedValues = new SettingsPropertyValueCollection();
			_stores[Store.PreviousShared] = PreviousSharedValues = new SettingsPropertyValueCollection();
		}

		public SettingsPropertyValueCollection PreviousUserValues { get; set; }
		public SettingsPropertyValueCollection CurrentUserValues { get; set; }
		public SettingsPropertyValueCollection PreviousSharedValues { get; set; }
		public SettingsPropertyValueCollection CurrentSharedValues { get; set; }

		public void Reset()
		{
			PreviousUserValues.Clear();
			CurrentUserValues.Clear();
			PreviousSharedValues.Clear();
			CurrentSharedValues.Clear();
		}
	}
}

#endif