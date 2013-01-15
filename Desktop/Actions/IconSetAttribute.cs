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

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Declares a set of icon resources to associate with an action.  
	/// </summary>
	/// <remarks>
	/// <para>
	/// The icon resources should be provided in several sizes so that different displays can be accomodated without
	/// having to scale the images:
	/// </para>
	/// <list type="table">
	/// <listheader><size><see cref="IconSize"/></size><res>Resolution</res></listheader>
	/// <item><size><see cref="IconSize.Small"/></size><res>24 x 24</res></item>
	/// <item><size><see cref="IconSize.Medium"/></size><res>48 x 48</res></item>
	/// <item><size><see cref="IconSize.Large"/></size><res>64 x 64</res></item>
	/// </list>
	/// </remarks>
	public class IconSetAttribute : ActionDecoratorAttribute
	{
		private readonly IconSet _iconSet;

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
		public IconSetAttribute(string actionId, IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon)
			: this(actionId, smallIcon, mediumIcon, largeIcon) {}

		/// <summary>
		/// Attribute constructor.
		/// </summary>
		/// <param name="actionId">The logical action identifier to which this attribute applies.</param>
		/// <param name="smallIcon">The resource name of the icon to be used at small resolutions (around 24 x 24).</param>
		/// <param name="mediumIcon">The resource name of the icon to be used at medium resolutions (around 48 x 48).</param>
		/// <param name="largeIcon">The resource name of the icon to be used at large resolutions (around 64 x 64).</param>
		public IconSetAttribute(string actionId, string smallIcon, string mediumIcon, string largeIcon)
			: base(actionId)
		{
			_iconSet = new IconSet(smallIcon, mediumIcon, largeIcon);
		}

		/// <summary>
		/// Attribute constructor.
		/// </summary>
		/// <param name="actionId">The logical action identifier to which this attribute applies.</param>
		/// <param name="icon">The resource name of the icon to be used at all resolutions.</param>
		public IconSetAttribute(string actionId, string icon)
			: base(actionId)
		{
			_iconSet = new IconSet(icon);
		}

		/// <summary>
		/// The <see cref="IconSet"/> defined by this attribute.
		/// </summary>
		public IconSet IconSet
		{
			get { return _iconSet; }
		}

		/// <summary>
		/// Sets the icon set for an <see cref="IAction"/>, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
		public override void Apply(IActionBuildingContext builder)
		{
			builder.Action.IconSet = IconSet;
		}
	}
}