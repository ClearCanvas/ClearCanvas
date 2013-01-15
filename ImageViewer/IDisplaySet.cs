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
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for <see cref="IPresentationImage"/> objects.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>Disposal</b> of an <see cref="IDisplaySet"/> and it's contained <see cref="IPresentationImage"/>s
	/// is handled by the framework when the <see cref="IDisplaySet"/> is part of the <see cref="IImageViewer"/>
	/// hierarchy.  When an <see cref="IImageViewer"/> is disposed, all its contained objects are disposed
	/// automatically.
	/// </para>
	/// <para>
	/// You may create copies of <see cref="IDisplaySet"/>s and <see cref="IPresentationImage"/>s
	/// (via the <see cref="CreateFreshCopy"/> mechanism) without keeping track of the objects and
	/// disposing them, provided the following two conditions are met:
	///  <list type="bullet">
	/// <item>
	/// The 'source' <see cref="IDisplaySet"/> (e.g. the one being copied) must be owned by an 
	/// <see cref="IImageSet"/> at the time the copy is created; the owner <see cref="IImageSet"/> must
	/// at some point become (or already be) part of the <see cref="IImageViewer"/> hierarchy.
	/// </item>
	/// <item>
	/// Any <see cref="IPresentationImage"/>s that are copied directly must be added to an <see cref="IDisplaySet"/> 
	/// that at some point becomes (or is already) part of the <see cref="IImageViewer"/> hierarchy.
	/// </item>
	/// </list>
	/// </para>
	/// </remarks>
	public interface IDisplaySet : IDrawable, IMemorable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="IDisplaySet"/> is not part of the 
		/// logical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="IImageSet"/>.
		/// </summary>
		/// <value>The parent <see cref="ImageSet"/> or <b>null</b> if the 
		/// <see cref="IDisplaySet"/> has not been added to an 
		/// <see cref="IImageSet"/> yet.</value>
		IImageSet ParentImageSet { get; }

		/// <summary>
		/// Gets the collection of <see cref="IPresentationImage"/> objects belonging
		/// to this <see cref="IDisplaySet"/>.
		/// </summary>
		PresentationImageCollection PresentationImages { get; }

		/// <summary>
		/// Gets a collection of linked <see cref="IPresentationImage"/> objects.
		/// </summary>
		IEnumerable<IPresentationImage> LinkedPresentationImages { get; }

		/// <summary>
		/// Gets the <see cref="IImageBox"/> associated with this <see cref="IDisplaySet"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageBox "/> or <b>null</b> if the
		/// <see cref="IDisplaySet"/> is not currently visible.</value>
		IImageBox ImageBox { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IImageBox"/> is
		/// linked.
		/// </summary>
		/// <value><b>true</b> if linked; <b>false</b> otherwise.</value>
		/// <remarks>
		/// Multiple display sets may be linked, allowing tools that can operate on
		/// multiple display sets to operate on all linked display sets simultaneously.  
		/// Note that the concept of linkage is slightly different from selection:
		/// it is possible for an <see cref="IDisplaySet"/> to be 1) selected but not linked
		/// 2) linked but not selected and 3) selected and linked.
		/// </remarks>
		bool Linked { get; set; }

		/// <summary>
		/// Gets the <see cref="IDisplaySetDescriptor"/> that describes this <see cref="IDisplaySet"/>.
		/// </summary>
		IDisplaySetDescriptor Descriptor { get; }

		/// <summary>
		/// Gets or sets the name of the display set.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a text description for the display set.
		/// </summary>
		/// <remarks>
		/// This value will normally correspond to the series description of the contained images.
		/// </remarks>
		string Description { get; }

		/// <summary>
		/// Gets the numeric identifier for the display set.
		/// </summary>
		/// <remarks>
		/// This value will normally correspond to the series number of the contained DICOM images.
		/// </remarks>
		int Number { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="IDisplaySet"/> is selected.
		/// </summary>
		bool Selected { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="IDisplaySet"/> is visible.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Gets a unique identifier for this <see cref="IDisplaySet"/>.
		/// </summary>
		string Uid { get; }

		/// <summary>
		/// Creates a fresh copy of the <see cref="IDisplaySet"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="IDisplaySet"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		IDisplaySet CreateFreshCopy();

		/// <summary>
		/// Creates a deep copy of the <see cref="IDisplaySet"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IDisplaySet"/>s may not return null from this method.
		/// </remarks>
		IDisplaySet Clone();

        ExtensionData ExtensionData { get; }
	}
}
