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
using System.Resources;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents a set of icon resources.
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
	public class IconSet : IEquatable<IconSet>
	{
		private readonly string _small;
		private readonly string _medium;
		private readonly string _large;

		/// <summary>
		/// Initializes a new instance of <see cref="IconSet"/>.
		/// </summary>
		/// <remarks>
		/// The use of icon schemes has been deprecated in favour of extensible application GUI themes.
		/// </remarks>
		/// <param name="scheme">The scheme of this icon set.</param>
		/// <param name="smallIcon">The resource name of the icon to be used at small resolutions (around 24 x 24).</param>
		/// <param name="mediumIcon">The resource name of the icon to be used at medium resolutions (around 48 x 48).</param>
		/// <param name="largeIcon">The resource name of the icon to be used at large resolutions (around 64 x 64).</param>
		[Obsolete("The use of icon schemes has been deprecated in favour of extensible application GUI themes")]
		public IconSet(IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon)
			: this(smallIcon, mediumIcon, largeIcon) {}

		/// <summary>
		/// Initializes a new instance of <see cref="IconSet"/>.
		/// </summary>
		/// <param name="smallIcon">The resource name of the icon to be used at small resolutions (around 24 x 24).</param>
		/// <param name="mediumIcon">The resource name of the icon to be used at medium resolutions (around 48 x 48).</param>
		/// <param name="largeIcon">The resource name of the icon to be used at large resolutions (around 64 x 64).</param>
		public IconSet(string smallIcon, string mediumIcon, string largeIcon)
		{
			_small = smallIcon ?? string.Empty;
			_medium = mediumIcon ?? string.Empty;
			_large = largeIcon ?? string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IconSet"/>.
		/// </summary>
		/// <param name="icon">The resource name of the icon to be used at all resolutions.</param>
		public IconSet(string icon)
			: this(icon, icon, icon) {}

		/// <summary>
		/// Gets the scheme of this icon set.
		/// </summary>
		/// <remarks>
		/// The use of icon schemes has been deprecated in favour of extensible application GUI themes.
		/// </remarks>
		[Obsolete("The use of icon schemes has been deprecated in favour of extensible application GUI themes")]
		public IconScheme Scheme
		{
			get { return IconScheme.Colour; }
		}

		/// <summary>
		/// Gets the name of the resource for the specified <see cref="IconSize"/>.
		/// </summary>
		public string this[IconSize iconSize]
		{
			get
			{
				if (iconSize == IconSize.Small)
					return SmallIcon;
				if (iconSize == IconSize.Medium)
					return MediumIcon;

				return LargeIcon;
			}
		}

		/// <summary>
		/// Gets the resource name of the icon to be used at small resolutions (around 24 x 24).
		/// </summary>
		public string SmallIcon
		{
			get { return _small; }
		}

		/// <summary>
		/// Gets the resource name of the icon to be used at medium resolutions (around 48 x 48).
		/// </summary>
		public string MediumIcon
		{
			get { return _medium; }
		}

		/// <summary>
		/// Gets the resource name of the icon to be used at large resolutions (around 64 x 64).
		/// </summary>
		public string LargeIcon
		{
			get { return _large; }
		}

		/// <summary>
		/// Creates an icon using the specified icon resource and resource resolver.
		/// </summary>
		/// <remarks>
		/// The base implementation resolves the specified image resource using the provided
		/// <paramref name="resourceResolver"/> and deserializes the resource stream into a <see cref="Bitmap"/>.
		/// </remarks>
		/// <param name="iconSize">The size of the desired icon.</param>
		/// <param name="resourceResolver">The resource resolver with which to resolve the requested icon resource.</param>
		/// <returns>An <see cref="Image"/> constructed from the requested resource.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceResolver"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="resourceResolver"/> was unable to resolve the requested icon resource.</exception>
		public virtual Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
		{
			Platform.CheckForNullReference(resourceResolver, "resourceResolver");
			try
			{
				return new Bitmap(resourceResolver.OpenResource(this[iconSize]));
			}
			catch (MissingManifestResourceException ex)
			{
				throw new ArgumentException("The provided resource resolver was unable to resolve the requested icon resource.", ex);
			}
		}

		/// <summary>
		/// Gets a string identifier that uniquely identifies the resolved icon, suitable for dictionary keying purposes.
		/// </summary>
		/// <remarks>
		/// The base implementation resolves the specified image resource using the provided
		/// <paramref name="resourceResolver"/> and returns the resource's fully qualified resource name.
		/// </remarks>
		/// <param name="iconSize">The size of the desired icon.</param>
		/// <param name="resourceResolver">The resource resolver with which to resolve the requested icon resource.</param>
		/// <returns>A string identifier that uniquely identifies the resolved icon.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceResolver"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="resourceResolver"/> was unable to resolve the requested icon resource.</exception>
		public virtual string GetIconKey(IconSize iconSize, IResourceResolver resourceResolver)
		{
			Platform.CheckForNullReference(resourceResolver, "resourceResolver");
			try
			{
				return resourceResolver.ResolveResource(this[iconSize]);
			}
			catch (MissingManifestResourceException ex)
			{
				throw new ArgumentException("The provided resource resolver was unable to resolve the requested icon resource.", ex);
			}
		}

		public override string ToString()
		{
			return string.Format(@"IconSet{{S={0}; M={1}; L={2}}}", SmallIcon, MediumIcon, LargeIcon);
		}

		public override int GetHashCode()
		{
		    return 0x68377975
		           //It's pretty likely another type will override CreateIcon
		           ^ GetType().GetHashCode()
		           ^ SmallIcon.GetHashCode()
		           ^ MediumIcon.GetHashCode()
		           ^ LargeIcon.GetHashCode();
		}

		public virtual bool Equals(IconSet other)
		{
			return other != null
                //It's pretty likely another type will override CreateIcon
                   && other.GetType() == GetType()
			       && other.SmallIcon == SmallIcon
			       && other.MediumIcon == MediumIcon
			       && other.LargeIcon == LargeIcon;
		}

		public override bool Equals(object obj)
		{
			return obj is IconSet && Equals((IconSet) obj);
		}
	}
}