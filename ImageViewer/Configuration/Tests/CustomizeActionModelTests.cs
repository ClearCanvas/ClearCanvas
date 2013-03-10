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

#if UNIT_TESTS

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

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Configuration.Tests
{
    [TestFixture]
    public class CustomizeActionModelTests
    {
        private IAction _mainMenu;
        private IAction _contextMenu;

        [TestFixtureSetUp]
        public void Initialize()
        {
            Platform.SetExtensionFactory(new NullExtensionFactory());
        }

        private void RefreshTool()
        {
            var tool = new CustomizeViewerActionModelTool();
            var actions = tool.Actions;

            _mainMenu = actions.Select(a => a.ActionID.EndsWith(CustomizeViewerActionModelTool._mainMenuCustomizeId)).First();
            _contextMenu = actions.Select(a => a.ActionID.EndsWith(CustomizeViewerActionModelTool._contextMenuCustomizeId)).First();
        }

        [Test]
        public void TestDefault()
        {
            RefreshTool();
            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(false, _contextMenu.Available);
        }

        [Test]
        public void TestUserShowsBoth()
        {
            RefreshTool();
            _contextMenu.Available = true;

            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            RefreshTool();
            _mainMenu.Available = false;
            _contextMenu.Available = false;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            _mainMenu.Available = true;
            _contextMenu.Available = true;

            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            RefreshTool();
            _mainMenu.Available = false;
            _contextMenu.Available = false;

            _contextMenu.Available = true;
            _mainMenu.Available = true;

            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);
        }

        [Test]
        public void TestUserHidesBoth()
        {
            RefreshTool();
            _contextMenu.Available = true;

            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            _mainMenu.Available = false;
            _contextMenu.Available = false;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            RefreshTool();
            _contextMenu.Available = false;
            _mainMenu.Available = false;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);
        }

        [Test]
        public void TestUserShowsContextMenu()
        {
            RefreshTool();
            _mainMenu.Available = false;
            _contextMenu.Available = true;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            RefreshTool();
            _mainMenu.Available = false;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            _contextMenu.Available = true;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);
        }

        [Test]
        public void TestUserShowsMainMenu()
        {
            RefreshTool();
            _mainMenu.Available = false;
            _contextMenu.Available = true;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            _mainMenu.Available = true;

            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);
        }
    }
}

#endif