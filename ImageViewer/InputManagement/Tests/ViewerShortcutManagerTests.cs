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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.InputManagement.Tests
{
	internal class TestTool : MouseImageViewerTool
	{
		public override IActionSet Actions
		{
			get { return new ActionSet(); }
		}

		public override string ToString()
		{
			return String.Format("{0}, Active={1}, InitiallyActive={2}", GetType().Name, Active, InitiallyActive);
		}
	}

	[MouseToolButton(XMouseButtons.Left, true)]
	[DefaultMouseToolButton(XMouseButtons.Right, ModifierFlags.Control)]
	internal class InitiallyActiveLeftMouse : TestTool {}

	[MouseToolButton(XMouseButtons.Left, true)]
	[DefaultMouseToolButton(XMouseButtons.Right)]
	internal class InitiallyActiveLeftMouse2 : TestTool {}

	[MouseToolButton(XMouseButtons.Left, true)]
	internal class InitiallyActiveLeftMouse3 : TestTool {}

	[MouseToolButton(XMouseButtons.Right, true)]
	[DefaultMouseToolButton(XMouseButtons.Middle, ModifierFlags.Shift)]
	internal class InitiallyActiveRightMouse : TestTool {}

	[MouseToolButton(XMouseButtons.Middle, true)]
	[DefaultMouseToolButton(XMouseButtons.Left)]
	internal class InitiallyActiveMiddleMouse : TestTool {}

	[MouseToolButton(XMouseButtons.Left, false)]
	[DefaultMouseToolButton(XMouseButtons.Right)]
	internal class InitiallyInactiveLeftMouse : TestTool {}

	[MouseToolButton(XMouseButtons.Right, false)]
	[DefaultMouseToolButton(XMouseButtons.Middle)]
	internal class InitiallyInactiveRightMouse : TestTool {}

	[MouseToolButton(XMouseButtons.Middle, false)]
	internal class InitiallyInactiveMiddleMouse : TestTool {}

	internal class TestToolContext : IImageViewerToolContext
	{
		public TestToolContext(IImageViewer viewer)
		{
			Viewer = viewer;
		}

		#region IImageViewerToolContext Members

		public IImageViewer Viewer { get; private set; }

		public IDesktopWindow DesktopWindow
		{
			get { return null; }
		}

		public event CancelEventHandler ViewerClosing
		{
			add { }
			remove { }
		}

		#endregion
	}

	[TestFixture]
	public class ViewerShortcutManagerTests
	{
		private ImageViewerComponent _viewer;
		private IImageViewerToolContext _context;
		private ToolSet _toolSet;

		[TestFixtureSetUp]
		public void Initialize()
		{
			Platform.SetExtensionFactory(new UnitTestExtensionFactory());
		}

		private IEnumerable<TestTool> CreateTools()
		{
			yield return new InitiallyInactiveLeftMouse();

			yield return new InitiallyActiveLeftMouse();
			yield return new InitiallyActiveRightMouse();

			yield return new InitiallyInactiveRightMouse();

			yield return new InitiallyActiveMiddleMouse();
			yield return new InitiallyActiveLeftMouse2();

			yield return new InitiallyInactiveMiddleMouse();

			yield return new InitiallyActiveLeftMouse3();
		}

		private void NewToolSet()
		{
			_viewer = new ImageViewerComponent();
			_context = new TestToolContext(_viewer);
			_toolSet = new ToolSet(CreateTools(), _context);

			MouseToolSettingsProfile.Reset();
		}

		private void RegisterTools()
		{
			//Just shortcut this.
			foreach (var tool in _toolSet.Tools)
				GetShortcutManager().RegisterImageViewerTool(tool);
		}

		private IEnumerable<TestTool> GetTools()
		{
			return _toolSet.Tools.Cast<TestTool>();
		}

		private ViewerShortcutManager GetShortcutManager()
		{
			return (ViewerShortcutManager) _viewer.ShortcutManager;
		}

		private TestTool GetActiveTool(XMouseButtons button)
		{
			return GetActiveTools(button).FirstOrDefault();
		}

		private IEnumerable<TestTool> GetActiveTools(XMouseButtons button)
		{
			return from tool in GetTools() where tool.Active && tool.MouseButton == button select tool;
		}

		private IEnumerable<TestTool> GetMouseButtonHandlers(MouseButtonShortcut shortcut)
		{
			return GetShortcutManager().GetMouseButtonHandlers(shortcut).Cast<TestTool>();
		}

		private IEnumerable<TestTool> GetMouseButtonHandlers(XMouseButtons button)
		{
			return GetMouseButtonHandlers(new MouseButtonShortcut(button)).Cast<TestTool>();
		}

		private IEnumerable<TestTool> GetMouseButtonHandlers(XMouseButtons button, bool ctrl, bool alt, bool shift)
		{
			return GetMouseButtonHandlers(new MouseButtonShortcut(button, ctrl, alt, shift)).Cast<TestTool>();
		}

		[Test]
		public void TestOneActivePerMouseButton()
		{
			NewToolSet();

			var activeLeft = GetActiveTools(XMouseButtons.Left);
			Assert.AreEqual(3, activeLeft.Count());

			var activeRight = GetActiveTools(XMouseButtons.Right);
			Assert.AreEqual(1, activeRight.Count());

			var activeMiddle = GetActiveTools(XMouseButtons.Middle);
			Assert.AreEqual(1, activeMiddle.Count());

			RegisterTools();

			Assert.AreEqual(1, activeLeft.Count());
			Assert.AreEqual(typeof (InitiallyActiveLeftMouse3), activeLeft.First().GetType());

			Assert.AreEqual(1, activeRight.Count());
			Assert.AreEqual(1, activeMiddle.Count());
		}

		[Test]
		public void TestChangeActiveTool()
		{
			NewToolSet();
			RegisterTools();

			var oldActiveTool = GetMouseButtonHandlers(XMouseButtons.Left).First();
			Assert.IsTrue(oldActiveTool.Active);

			var activeTool = GetTools().OfType<TestTool>().First();
			activeTool.Active = true;

			Assert.IsTrue(activeTool.Active);
			Assert.IsFalse(oldActiveTool.Active);

			oldActiveTool = GetMouseButtonHandlers(XMouseButtons.Right).First();
			Assert.IsTrue(oldActiveTool.Active);

			activeTool = GetTools().OfType<InitiallyInactiveRightMouse>().First();
			activeTool.Active = true;

			Assert.IsTrue(activeTool.Active);
			Assert.IsFalse(oldActiveTool.Active);

			//Active right and middle tools not affected.
			Assert.AreEqual(1, GetActiveTools(XMouseButtons.Right).Count());
			Assert.AreEqual(1, GetActiveTools(XMouseButtons.Middle).Count());
		}

		[Test]
		public void TestDeactivateDefaultActivated()
		{
			NewToolSet();
			RegisterTools();

			var oldActiveTool = GetMouseButtonHandlers(XMouseButtons.Left).First();
			Assert.IsTrue(oldActiveTool.Active);

			oldActiveTool.Active = false;

			Assert.IsFalse(oldActiveTool.Active);
			var newActiveTool = GetMouseButtonHandlers(XMouseButtons.Left).First();
			Assert.AreEqual(typeof (InitiallyActiveLeftMouse), newActiveTool.GetType());
			Assert.IsTrue(newActiveTool.Active);

			//Active right and middle tools not affected.
			Assert.AreEqual(1, GetActiveTools(XMouseButtons.Right).Count());
			Assert.AreEqual(1, GetActiveTools(XMouseButtons.Middle).Count());
		}

		[Test]
		public void TestDeactivateNoDefaultToActivate()
		{
			NewToolSet();
			RegisterTools();

			foreach (var tool in GetTools())
				tool.InitiallyActive = false;

			var oldActiveTool = GetMouseButtonHandlers(XMouseButtons.Left).First();
			Assert.IsTrue(oldActiveTool.Active);

			oldActiveTool.Active = false;

			Assert.IsFalse(oldActiveTool.Active);
			var newActiveTool = GetActiveTools(XMouseButtons.Left).FirstOrDefault();
			Assert.IsNull(newActiveTool);
		}

		[Test]
		public void TestActiveChangeMouseButton()
		{
			NewToolSet();
			RegisterTools();

			var activeLeft = GetActiveTool(XMouseButtons.Left);
			var activeRight = GetActiveTool(XMouseButtons.Right);

			Assert.IsNotNull(activeLeft);
			Assert.IsNotNull(activeRight);

			activeRight.MouseButton = XMouseButtons.Left;
			activeLeft = GetActiveTool(XMouseButtons.Left);

			Assert.AreEqual(activeLeft, activeRight);
			Assert.IsTrue(activeRight.Active);
			Assert.AreEqual(XMouseButtons.Left, activeRight.MouseButton);

			activeRight = GetActiveTool(XMouseButtons.Right);
			Assert.IsNull(activeRight);
		}

		[Test]
		public void TestActiveChangeMouseButton2()
		{
			NewToolSet();
			RegisterTools();

			var activeLeft = GetActiveTool(XMouseButtons.Left);
			var activeRight = GetActiveTool(XMouseButtons.Right);

			Assert.IsNotNull(activeLeft);
			Assert.IsNotNull(activeRight);

			activeLeft.MouseButton = XMouseButtons.Right;
			activeRight = GetActiveTool(XMouseButtons.Right);

			Assert.AreEqual(activeLeft, activeRight);
			Assert.IsTrue(activeRight.Active);
			Assert.AreEqual(XMouseButtons.Right, activeRight.MouseButton);

			//Because the shortcut manager doesn't know what the "old"
			//button assignment was, we can't try to activate the "initially active"
			//tool for the left mouse button.
			activeLeft = GetActiveTool(XMouseButtons.Left);
			Assert.IsNull(activeLeft);
		}

		[Test]
		public void TestGetMouseButtonHandlers()
		{
			NewToolSet();
			RegisterTools();

			var leftHandlers = GetMouseButtonHandlers(XMouseButtons.Left);
			Assert.AreEqual(2, leftHandlers.Count());
			Assert.IsTrue(leftHandlers.First().Active);
			Assert.IsTrue(leftHandlers.ElementAt(1).Active);
			Assert.AreEqual(XMouseButtons.Middle, leftHandlers.ElementAt(1).MouseButton);

			var rightHandlers = GetMouseButtonHandlers(XMouseButtons.Right);
			Assert.AreEqual(3, rightHandlers.Count());
			Assert.IsTrue(rightHandlers.First().Active);
			Assert.IsFalse(rightHandlers.ElementAt(1).Active);
			Assert.AreEqual(XMouseButtons.Left, rightHandlers.ElementAt(1).MouseButton);
			Assert.IsFalse(rightHandlers.ElementAt(2).Active);
			Assert.AreEqual(XMouseButtons.Left, rightHandlers.ElementAt(2).MouseButton);

			var middleHandlers = GetMouseButtonHandlers(XMouseButtons.Middle);
			Assert.AreEqual(2, middleHandlers.Count());
			Assert.IsTrue(middleHandlers.First().Active);
			Assert.IsFalse(middleHandlers.Skip(1).First().Active);
			Assert.AreEqual(XMouseButtons.Right, middleHandlers.Skip(1).First().MouseButton);
		}
	}
}

#endif