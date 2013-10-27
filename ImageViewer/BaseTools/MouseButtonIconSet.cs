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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using Action=ClearCanvas.Desktop.Actions.Action;

namespace ClearCanvas.ImageViewer.BaseTools
{
	partial class MouseImageViewerTool
	{
		private static void UpdateMouseButtonIconSet(IActionSet actions, XMouseButtons mouseButton)
		{
			foreach (IAction action in actions)
			{
				if (action is Action && action.IconSet is MouseButtonIconSet)
					((Action) action).IconSet = new MouseButtonIconSet(action.IconSet, mouseButton);
			}
		}

		/// <summary>
		/// Declares a set of icon resources to associate with an action that activates a mouse button tool.
		/// </summary>
		protected class MouseButtonIconSetAttribute : IconSetAttribute
		{
			/// <summary>
			/// Attribute constructor.
			/// </summary>
			/// <remarks>
			/// The use of icon schemes has been deprecated in favour of extensible application GUI themes.
			/// </remarks>
			/// <param name="actionId">The logical action identifier to which this attribute applies.</param>
			/// <param name="scheme">The scheme of this icon set.</param>
			/// <param name="smallIcon">The resource name of the icon to be used at small resolutions (around 24 x 24).</param>
			/// <param name="mediumIcon">The resource name of the icon to be used at medium resolutions (around 48 x 48).</param>
			/// <param name="largeIcon">The resource name of the icon to be used at large resolutions (around 64 x 64).</param>
			[Obsolete("The use of icon schemes has been deprecated in favour of extensible application GUI themes")]
			public MouseButtonIconSetAttribute(string actionId, IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon)
				: base(actionId, scheme, smallIcon, mediumIcon, largeIcon) {}

			/// <summary>
			/// Attribute constructor.
			/// </summary>
			/// <param name="actionId">The logical action identifier to which this attribute applies.</param>
			/// <param name="smallIcon">The resource name of the icon to be used at small resolutions (around 24 x 24).</param>
			/// <param name="mediumIcon">The resource name of the icon to be used at medium resolutions (around 48 x 48).</param>
			/// <param name="largeIcon">The resource name of the icon to be used at large resolutions (around 64 x 64).</param>
			public MouseButtonIconSetAttribute(string actionId, string smallIcon, string mediumIcon, string largeIcon)
				: base(actionId, smallIcon, mediumIcon, largeIcon) {}

			/// <summary>
			/// Attribute constructor.
			/// </summary>
			/// <param name="actionId">The logical action identifier to which this attribute applies.</param>
			/// <param name="icon">The resource name of the icon to be used at all resolutions.</param>
			public MouseButtonIconSetAttribute(string actionId, string icon)
				: base(actionId, icon) {}

			/// <summary>
			/// Sets the icon set for an <see cref="IAction"/>, via the specified <see cref="IActionBuildingContext"/>.
			/// </summary>
			public override void Apply(IActionBuildingContext builder)
			{
				//TODO (CR Mar 2010): Observe MouseButton changing and set the iconset.
				XMouseButtons assignedButton = XMouseButtons.None;
				if (builder.ActionTarget is MouseImageViewerTool)
					assignedButton = ((MouseImageViewerTool) builder.ActionTarget).MouseButton;
				builder.Action.IconSet = new MouseButtonIconSet(this.IconSet, assignedButton);
			}
		}

		//TODO (Web Viewer): make protected again and find a solution that works in the client (e.g. pixel shader).

		/// <summary>
		/// Represents a set of icon resources with an overlay to indicate mapped mouse button.
		/// </summary>
		public class MouseButtonIconSet : IconSet
		{
			private XMouseButtons _assignedButton;

			/// <summary>
			/// Initializes a new instance of <see cref="MouseButtonIconSet"/>.
			/// </summary>
			/// <param name="baseIconSet">A template <see cref="IconSet"/> from which to copy resource names.</param>
			/// <param name="assignedButton">The mouse button that is assigned to the associated tool.</param>
			public MouseButtonIconSet(IconSet baseIconSet, XMouseButtons assignedButton)
				: base(baseIconSet.SmallIcon, baseIconSet.MediumIcon, baseIconSet.LargeIcon)
			{
				_assignedButton = assignedButton;
			}

			/// <summary>
			/// Initializes a new instance of <see cref="MouseButtonIconSet"/>.
			/// </summary>
			/// <remarks>
			/// The use of icon schemes has been deprecated in favour of extensible application GUI themes.
			/// </remarks>
			/// <param name="scheme">The scheme of this icon set.</param>
			/// <param name="smallIcon">The resource name of the icon to be used at small resolutions (around 24 x 24).</param>
			/// <param name="mediumIcon">The resource name of the icon to be used at medium resolutions (around 48 x 48).</param>
			/// <param name="largeIcon">The resource name of the icon to be used at large resolutions (around 64 x 64).</param>
			/// <param name="assignedButton">The mouse button that is assigned to the associated tool.</param>
			[Obsolete("The use of icon schemes has been deprecated in favour of extensible application GUI themes")]
			public MouseButtonIconSet(IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon, XMouseButtons assignedButton)
				: base(scheme, smallIcon, mediumIcon, largeIcon) {}

			/// <summary>
			/// Initializes a new instance of <see cref="MouseButtonIconSet"/>.
			/// </summary>
			/// <param name="smallIcon">The resource name of the icon to be used at small resolutions (around 24 x 24).</param>
			/// <param name="mediumIcon">The resource name of the icon to be used at medium resolutions (around 48 x 48).</param>
			/// <param name="largeIcon">The resource name of the icon to be used at large resolutions (around 64 x 64).</param>
			/// <param name="assignedButton">The mouse button that is assigned to the associated tool.</param>
			public MouseButtonIconSet(string smallIcon, string mediumIcon, string largeIcon, XMouseButtons assignedButton)
				: base(smallIcon, mediumIcon, largeIcon)
			{
				_assignedButton = assignedButton;
			}

			/// <summary>
			/// Initializes a new instance of <see cref="MouseButtonIconSet"/>.
			/// </summary>
			/// <param name="icon">The resource name of the icon to be used at all resolutions.</param>
			/// <param name="assignedButton">The mouse button that is assigned to the associated tool.</param>
			public MouseButtonIconSet(string icon, XMouseButtons assignedButton)
				: base(icon)
			{
				_assignedButton = assignedButton;
			}

			/// <summary>
			/// Gets the mouse button that is assigned to the associated tool.
			/// </summary>
			public XMouseButtons AssignedButton
			{
				get { return _assignedButton; }
			}

			//TODO (Web Viewer): make private again and find a solution that works in the client (e.g. pixel shader).
			public bool ShowMouseButtonIconOverlay
			{
				get { return MouseToolSettings.DefaultInstance.ShowMouseButtonIconOverlay && !MouseToolSettingsProfile.Current.HideButtonsOverlay; }
			}

			/// <summary>
			/// Gets an appropriate icon overlay to indicate the mouse button assigned to the associated tool.
			/// </summary>
			/// <param name="iconSize">The desired version of the icon overlay.</param>
			/// <returns>The icon overlay as an <see cref="Image"/>.</returns>
			public Image GetButtonOverlay(IconSize iconSize)
			{
				string[] resourceNames;
				switch (_assignedButton)
				{
					case XMouseButtons.Left:
						resourceNames = new string[] {"BaseTools.LeftMouseButtonOverlaySmall.png", "BaseTools.LeftMouseButtonOverlayMedium.png", "BaseTools.LeftMouseButtonOverlayLarge.png"};
						break;
					case XMouseButtons.Right:
						resourceNames = new string[] {"BaseTools.RightMouseButtonOverlaySmall.png", "BaseTools.RightMouseButtonOverlayMedium.png", "BaseTools.RightMouseButtonOverlayLarge.png"};
						break;
					case XMouseButtons.Middle:
						resourceNames = new string[] {"BaseTools.MiddleMouseButtonOverlaySmall.png", "BaseTools.MiddleMouseButtonOverlayMedium.png", "BaseTools.MiddleMouseButtonOverlayLarge.png"};
						break;
					case XMouseButtons.XButton1:
						resourceNames = new string[] {"BaseTools.X1MouseButtonOverlaySmall.png", "BaseTools.X1MouseButtonOverlayMedium.png", "BaseTools.X1MouseButtonOverlayLarge.png"};
						break;
					case XMouseButtons.XButton2:
						resourceNames = new string[] {"BaseTools.X2MouseButtonOverlaySmall.png", "BaseTools.X2MouseButtonOverlayMedium.png", "BaseTools.X2MouseButtonOverlayLarge.png"};
						break;
					case XMouseButtons.None:
					default:
						return null;
				}

				IResourceResolver resourceResolver = new ActionResourceResolver(GetType());
				switch (iconSize)
				{
					case IconSize.Small:
						return new Bitmap(resourceResolver.OpenResource(resourceNames[0]));
					case IconSize.Medium:
						return new Bitmap(resourceResolver.OpenResource(resourceNames[1]));
					case IconSize.Large:
					default:
						return new Bitmap(resourceResolver.OpenResource(resourceNames[2]));
				}
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
				Image iconBase = base.CreateIcon(iconSize, resourceResolver);
				if (this.ShowMouseButtonIconOverlay)
				{
					Image iconOverlay = GetButtonOverlay(iconSize);
					if (iconOverlay != null)
					{
						using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(iconBase))
						{
							g.DrawImageUnscaledAndClipped(iconOverlay, new Rectangle(Point.Empty, iconBase.Size));
						}
						iconOverlay.Dispose();
					}
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
				string baseIconKey = base.GetIconKey(iconSize, resourceResolver);
				if (this.ShowMouseButtonIconOverlay)
					return string.Format("{0}:{1}", baseIconKey, _assignedButton);
				return baseIconKey;
			}
		}
	}
}