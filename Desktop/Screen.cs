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
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// An interface for providers of screen information.
	/// </summary>
	public interface IScreenInfoProvider
	{
		/// <summary>
		/// Gets the virtual screen of the entire desktop (all display devices).
		/// </summary>
		Rectangle VirtualScreen { get; }

		/// <summary>
		/// Gets all the <see cref="Screen"/>s in the desktop.
		/// </summary>
		Screen[] GetScreens();
	}

	/// <summary>
	/// An extension point for <see cref="IScreenInfoProvider"/>s.
	/// </summary>
	public sealed class ScreenInfoProviderExtensionPoint : ExtensionPoint<IScreenInfoProvider>
	{
	}

	/// <summary>
	/// Abstract class representing a single screen in the desktop.
	/// </summary>
	public abstract class Screen : IEquatable<Screen>
	{
		private static readonly IScreenInfoProvider _screenInfoProvider;

		static Screen()
		{
			try
			{
				_screenInfoProvider = (IScreenInfoProvider)new ScreenInfoProviderExtensionPoint().CreateExtension();
			}
			catch(Exception e)
			{
			    _screenInfoProvider = null;
				Platform.Log(LogLevel.Debug, e, "A valid IScreenInfoProvider extension must exist.");
			}
		}

		#region Public Properties

		/// <summary>
		/// Gets the desktop's virtual screen.
		/// </summary>
		public static Rectangle VirtualScreen
		{
			get { return _screenInfoProvider == null ? Rectangle.Empty : _screenInfoProvider.VirtualScreen; }
		}

		/// <summary>
		/// Gets an array of all the <see cref="Screen"/>s in the desktop.
		/// </summary>
		public static Screen[] AllScreens
		{
            get { return _screenInfoProvider == null ? new Screen[0] : _screenInfoProvider.GetScreens(); }	
		}

		#region IEquatable<Screen> Members

		/// <summary>
		/// Gets whether or not this <see cref="Screen"/> object is equivalent to another.
		/// </summary>
		public abstract bool Equals(Screen other);

		#endregion
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the number of bits per pixel of the device.
		/// </summary>
		public abstract int BitsPerPixel { get; }

		/// <summary>
		/// Gets the bounds of the screen inside the <see cref="VirtualScreen"/>.
		/// </summary>
		public abstract Rectangle Bounds { get; }

		/// <summary>
		/// Gets the name of the device.
		/// </summary>
		public abstract string DeviceName { get; }

		/// <summary>
		/// Gets whether or not this is the primary screen.
		/// </summary>
		public abstract bool IsPrimary { get; }

		/// <summary>
		/// Gets the area of the <see cref="Screen"/> in which an <see cref="IDesktopWindow"/> can be maximized.
		/// </summary>
		public abstract Rectangle WorkingArea { get; }

		#endregion
	}
}
