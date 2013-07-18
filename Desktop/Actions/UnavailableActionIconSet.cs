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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Represents a set of icon resources that specify the same logical icon in different sizes with an overlay to indicate that the action is unavailable.
	/// </summary>
	public sealed class UnavailableActionIconSet : IconSet
	{
	    private readonly IconSet _grayIconSet = new IconSet("Icons.UnavailableToolOverlayGraySmall.png", "Icons.UnavailableToolOverlayGrayMedium.png", "Icons.UnavailableToolOverlayGrayLarge.png");
	    private readonly IconSet _redIconSet = new IconSet("Icons.UnavailableToolOverlaySmall.png", "Icons.UnavailableToolOverlayMedium.png", "Icons.UnavailableToolOverlayLarge.png");
		
        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="baseIconSet">A template <see cref="IconSet"/> from which to copy resource names.</param>
		public UnavailableActionIconSet(IconSet baseIconSet)
			: base(baseIconSet.SmallIcon, baseIconSet.MediumIcon, baseIconSet.LargeIcon)
		{
		}

        /// <summary>
        /// Specifies whether or not to render the "unavailable" overlay gray, rather than the default red.
        /// </summary>
        public bool GrayMode { get; set; }

		private Image GetOverlayIcon(IconSize iconSize)
		{
			var resourceResolver = new ApplicationThemeResourceResolver(GetType().Assembly);
		    var overlayIconSet = GrayMode ? _grayIconSet : _redIconSet;
            return overlayIconSet.CreateIcon(iconSize, resourceResolver);
		}

		/// <summary>
		/// Creates an icon using the specified icon resource and resource resolver.
		/// </summary>
		/// <param name="iconSize">The size of the desired icon.</param>
		/// <param name="resourceResolver">The resource resolver with which to resolve the requested icon resource.</param>
		/// <returns>An <see cref="Image"/> constructed from the requested resource.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceResolver"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="resourceResolver"/> was unable to resolve the requested icon resource.</exception>
		public override Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
		{
			var iconBase = base.CreateIcon(iconSize, resourceResolver);
			var iconOverlay = GetOverlayIcon(iconSize);
			if (iconOverlay != null)
			{
				using (var g = Graphics.FromImage(iconBase))
				{
					g.DrawImageUnscaledAndClipped(iconOverlay, new Rectangle(Point.Empty, iconBase.Size));
				}
				iconOverlay.Dispose();
			}
			return iconBase;
		}

		/// <summary>
		/// Gets a string identifier that uniquely identifies the resolved icon, suitable for dictionary keying purposes.
		/// </summary>
		/// <param name="iconSize">The size of the desired icon.</param>
		/// <param name="resourceResolver">The resource resolver with which to resolve the requested icon resource.</param>
		/// <returns>A string identifier that uniquely identifies the resolved icon.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceResolver"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="resourceResolver"/> was unable to resolve the requested icon resource.</exception>
		public override string GetIconKey(IconSize iconSize, IResourceResolver resourceResolver)
		{
			var baseIconKey = base.GetIconKey(iconSize, resourceResolver);
			return string.Format("{0}:unavailable", baseIconKey);
		}
	}
};