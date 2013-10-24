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

using System;
using System.Collections.Generic;
using System.Configuration;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class SettingsGroupTests
	{
		[SettingsProvider(typeof (StandardSettingsProvider))]
		private class TestEnterpriseSetting : ApplicationSettingsBase {}

		[SettingsProvider(typeof (LocalFileSettingsProvider))]
		private class TestLocalSetting : ApplicationSettingsBase {}

		[SettingsProvider(typeof (ExtendedLocalFileSettingsProvider))]
		private class TestExtendedLocalSetting : ApplicationSettingsBase {}

		[SettingsProvider(typeof (ApplicationCriticalSettingsProvider))]
		private class TestCriticalSetting : ApplicationSettingsBase {}

		[TestFixtureSetUp]
		public void Initialize()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test]
		public void TestApplyFilter()
		{
			SettingsStore.SetIsSupported(true);

			Type enterpriseType = typeof (TestEnterpriseSetting);
			Type criticalSetting = typeof (TestCriticalSetting);
			Type localType = typeof (TestLocalSetting);
			Type extendedLocalType = typeof (TestExtendedLocalSetting);

			AssertSettingsType(enterpriseType,
			                   new Dictionary<SettingsGroupFilter, bool>
			                   	{
			                   		{SettingsGroupFilter.All, true},
			                   		{SettingsGroupFilter.SupportEnterpriseStorage, true},
			                   		{SettingsGroupFilter.SupportsEditingOfSharedProfile, true},
			                   		{SettingsGroupFilter.LocalStorage, false}
			                   	});

			AssertSettingsType(criticalSetting,
			                   new Dictionary<SettingsGroupFilter, bool>
			                   	{
			                   		{SettingsGroupFilter.All, true},
			                   		{SettingsGroupFilter.SupportEnterpriseStorage, false},
			                   		{SettingsGroupFilter.SupportsEditingOfSharedProfile, false},
			                   		{SettingsGroupFilter.LocalStorage, true}
			                   	});
			AssertSettingsType(extendedLocalType,
			                   new Dictionary<SettingsGroupFilter, bool>
			                   	{
			                   		{SettingsGroupFilter.All, true},
			                   		{SettingsGroupFilter.SupportEnterpriseStorage, false},
			                   		{SettingsGroupFilter.SupportsEditingOfSharedProfile, true},
			                   		{SettingsGroupFilter.LocalStorage, true}
			                   	});
			AssertSettingsType(localType,
			                   new Dictionary<SettingsGroupFilter, bool>
			                   	{
			                   		{SettingsGroupFilter.All, true},
			                   		{SettingsGroupFilter.SupportEnterpriseStorage, false},
			                   		{SettingsGroupFilter.SupportsEditingOfSharedProfile, false},
			                   		{SettingsGroupFilter.LocalStorage, true}
			                   	});
		}

		[Test]
		public void TestApplyFilterNoSettingsStore()
		{
			SettingsStore.SetIsSupported(false);

			Type enterpriseType = typeof (TestEnterpriseSetting);
			Type criticalSetting = typeof (TestCriticalSetting);
			Type localType = typeof (TestLocalSetting);
			Type extendedLocalType = typeof (TestExtendedLocalSetting);

			AssertSettingsType(enterpriseType,
			                   new Dictionary<SettingsGroupFilter, bool>
			                   	{
			                   		{SettingsGroupFilter.All, true},
			                   		{SettingsGroupFilter.SupportEnterpriseStorage, true},
			                   		{SettingsGroupFilter.SupportsEditingOfSharedProfile, true},
			                   		{SettingsGroupFilter.LocalStorage, true}
			                   	});

			AssertSettingsType(criticalSetting,
			                   new Dictionary<SettingsGroupFilter, bool>
			                   	{
			                   		{SettingsGroupFilter.All, true},
			                   		{SettingsGroupFilter.SupportEnterpriseStorage, false},
			                   		{SettingsGroupFilter.SupportsEditingOfSharedProfile, false},
			                   		{SettingsGroupFilter.LocalStorage, true}
			                   	});
			AssertSettingsType(extendedLocalType,
			                   new Dictionary<SettingsGroupFilter, bool>
			                   	{
			                   		{SettingsGroupFilter.All, true},
			                   		{SettingsGroupFilter.SupportEnterpriseStorage, false},
			                   		{SettingsGroupFilter.SupportsEditingOfSharedProfile, true},
			                   		{SettingsGroupFilter.LocalStorage, true}
			                   	});
			AssertSettingsType(localType,
			                   new Dictionary<SettingsGroupFilter, bool>
			                   	{
			                   		{SettingsGroupFilter.All, true},
			                   		{SettingsGroupFilter.SupportEnterpriseStorage, false},
			                   		{SettingsGroupFilter.SupportsEditingOfSharedProfile, false},
			                   		{SettingsGroupFilter.LocalStorage, true}
			                   	});
		}

		private void AssertSettingsType(Type type, IEnumerable<KeyValuePair<SettingsGroupFilter, bool>> expectedResults)
		{
			foreach (var expectedResult in expectedResults)
				Assert.AreEqual(expectedResult.Value, SettingsGroupDescriptor.ApplyFilter(expectedResult.Key, type), "(Filter={0}) expected {1} for {2}", expectedResult.Key, expectedResult.Value, type);
		}
	}
}

#endif