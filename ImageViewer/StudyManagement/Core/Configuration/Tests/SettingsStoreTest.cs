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
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Configuration.Tests
{
    [TestFixture]
    internal class SettingsStoreTest
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            Platform.SetExtensionFactory(new UnitTestExtensionFactory(
                new Dictionary<Type, Type>
                    {
                        {typeof(ServiceProviderExtensionPoint), typeof(SystemConfigurationServiceProvider)}
                    }));

            using (var context = new DataAccessContext())
            {
                var broker = context.GetConfigurationDocumentBroker();
                broker.DeleteAllDocuments();
            }
        }

        [Test]
        public void SettingsStorePutGet()
        {
            const string testVal = "A man, a plan, a canal, panama";

            var store = new SystemConfigurationSettingsStore();

            var settings = new AppSettings();

            var group = new SettingsGroupDescriptor(typeof (AppSettings));

            settings.App = testVal;
            var dic = new Dictionary<string, string>();
            dic.Add(AppSettings.PropertyApp, settings.App);

            store.PutSettingsValues(group, null, null, dic);

            var resultDic = store.GetSettingsValues(group, null, null);

            string val;

            Assert.IsTrue(resultDic.TryGetValue(AppSettings.PropertyApp, out val));

            Assert.AreEqual(val, testVal);
        }

        [Test]
        public void SettingsPrior()
        {
            const string testVal = "A man, a plan, a canal, panama 2";
            const string testValOld = "A man, a plan, a canal, panama 3";

            var store = new SystemConfigurationSettingsStore();

            var settings = new AppSettings();

            var group = new SettingsGroupDescriptor(typeof(AppSettings));

            settings.App = testVal;
            var dic = new Dictionary<string, string>();
            dic.Add(AppSettings.PropertyApp, settings.App);

            store.PutSettingsValues(group, null, null, dic);

            var oldGroup = new SettingsGroupDescriptor(group.Name, new Version(group.Version.Major -1,group.Version.Minor), group.Description,
                                                       group.AssemblyQualifiedTypeName, group.HasUserScopedSettings);
            dic[AppSettings.PropertyApp] = testValOld;

            store.PutSettingsValues(oldGroup, null, null, dic);

            var resultDic = store.GetSettingsValues(group, null, null);

            string val;

            Assert.IsTrue(resultDic.TryGetValue(AppSettings.PropertyApp, out val));

            Assert.AreEqual(val, testVal);


            resultDic = store.GetSettingsValues(oldGroup, null, null);

            Assert.IsTrue(resultDic.TryGetValue(AppSettings.PropertyApp, out val));

            Assert.AreEqual(val, testValOld);

            var returnedOldGroup = store.GetPreviousSettingsGroup(group);
            Assert.IsNotNull(returnedOldGroup);
            Assert.AreEqual(returnedOldGroup.Version.Major, oldGroup.Version.Major);
            Assert.AreEqual(returnedOldGroup.Version.Minor, oldGroup.Version.Minor);
            Assert.AreEqual(returnedOldGroup.Description, oldGroup.Description);
            Assert.AreEqual(returnedOldGroup.Name, oldGroup.Name);
            Assert.AreEqual(returnedOldGroup.AssemblyQualifiedTypeName, oldGroup.AssemblyQualifiedTypeName);


            returnedOldGroup = store.GetPreviousSettingsGroup(oldGroup);
            Assert.IsNull(returnedOldGroup);

        }
    }
}

#endif