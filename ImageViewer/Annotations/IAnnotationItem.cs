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

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Provides text to be rendered to the overlay by an <see cref="ClearCanvas.ImageViewer.Rendering.IRenderer"/>.
	/// </summary>
	/// <seealso cref="AnnotationBox"/>
	/// <seealso cref="AnnotationItemConfigurationOptions"/>
	/// <seealso cref="IAnnotationItemProvider"/>
	/// <seealso cref="IAnnotationLayout"/>
	/// <seealso cref="IAnnotationLayoutProvider"/>
	public interface IAnnotationItem
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
		/// Gets the label that can be shown on the overlay depending on the <see cref="AnnotationBox"/>'s 
		/// configuration (<see cref="AnnotationItemConfigurationOptions"/>).
		/// </summary>
		string GetLabel();

		/// <summary>
		/// Gets the annotation text to display on the overlay for <paramref name="presentationImage"/>.
		/// </summary>
		string GetAnnotationText(IPresentationImage presentationImage);
	}
}
