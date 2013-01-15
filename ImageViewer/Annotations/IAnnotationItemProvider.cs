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

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// An <see cref="ExtensionPoint"/> for <see cref="IAnnotationItemProvider"/>s.
	/// </summary>
	[ExtensionPoint()]
	public sealed class AnnotationItemProviderExtensionPoint : ExtensionPoint<IAnnotationItemProvider>
	{
	}

	/// <summary>
	/// An <see cref="IAnnotationItemProvider"/> provides a logical grouping of 
	/// <see cref="IAnnotationItem"/>s simply because there can be so many of them.
	/// </summary>
	/// <seealso cref="AnnotationBox"/>
	/// <seealso cref="IAnnotationItem"/>
	/// <seealso cref="AnnotationItemConfigurationOptions"/>
	/// <seealso cref="IAnnotationLayout"/>
	/// <seealso cref="IAnnotationLayoutProvider"/>
	/// <seealso cref="AnnotationItemProviderExtensionPoint"/>
	public interface IAnnotationItemProvider
	{
		/// <summary>
		/// Gets a unique identifier.
		/// </summary>
		string GetIdentifier();

		/// <summary>
		/// Gets a user friendly display name.
		/// </summary>
		string GetDisplayName();

		/// <summary>
		/// Gets the logical group of <see cref="IAnnotationItem"/>s.
		/// </summary>
		IEnumerable<IAnnotationItem> GetAnnotationItems();
	}
}
