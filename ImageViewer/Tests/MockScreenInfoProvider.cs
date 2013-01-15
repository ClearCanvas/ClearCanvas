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

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tests
{
	/// <summary>
	/// A mock implementation of <see cref="IScreenInfoProvider"/> suitable for unit testing contexts.
	/// </summary>
	/// <remarks>
	/// In order to use this mock extension, the class must be explicitly installed as an extension
	/// of the <see cref="ScreenInfoProviderExtensionPoint"/> extension point before the unit test is
	/// executed. This can be accomplised by installing a custom <see cref="IExtensionFactory"/>, such
	/// as <see cref="UnitTestExtensionFactory"/>.
	/// </remarks>
	/// <example lang="CS">
	/// <code><![CDATA[
	/// UnitTestExtensionFactory extensionFactory = new UnitTestExtensionFactory();
	/// extensionFactory.Define(typeof(ScreenInfoProviderExtensionPoint), typeof(MockScreenInfoProvider));
	/// Platform.SetExtensionFactory(extensionFactory);
	/// ]]></code>
	/// </example>
	public sealed class MockScreenInfoProvider : IScreenInfoProvider
	{
		/// <summary>
		/// Gets the virtual screen of the entire desktop (all display devices).
		/// </summary>
		public Rectangle VirtualScreen
		{
			get { return new Rectangle(0, 0, 640, 480); }
		}

		/// <summary>
		/// Gets all the <see cref="Screen"/>s in the desktop.
		/// </summary>
		public Screen[] GetScreens()
		{
			return new[] {new MockScreen()};
		}

		private sealed class MockScreen : Screen
		{
			public override bool Equals(Screen other)
			{
				return other is MockScreen;
			}

			public override int BitsPerPixel
			{
				get { return 24; }
			}

			public override Rectangle Bounds
			{
				get { return new Rectangle(0, 0, 640, 480); }
			}

			public override string DeviceName
			{
				get { return "V'GER"; }
			}

			public override bool IsPrimary
			{
				get { return true; }
			}

			public override Rectangle WorkingArea
			{
				get { return new Rectangle(0, 0, 640, 480); }
			}
		}
	}
}

#endif