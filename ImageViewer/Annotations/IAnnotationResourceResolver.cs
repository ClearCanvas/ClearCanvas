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
	/// Resolves the display name and label from an <see cref="IAnnotationItem"/>'s 
	/// unique identifier (<see cref="IAnnotationItem.GetIdentifier"/>, usually by looking the values
	/// up in assembly resources.
	/// </summary>
	public interface IAnnotationResourceResolver
	{
		/// <summary>
		/// Resolves the <see cref="IAnnotationItem"/>'s label (see <see cref="IAnnotationItem.GetLabel()"/>).
		/// </summary>
		string ResolveDisplayName(string annotationIdentifier);

		/// <summary>
		/// Resolves the <see cref="IAnnotationItem"/>'s (or <see cref="IAnnotationItemProvider"/>'s) display name 
		/// (see <see cref="IAnnotationItem.GetDisplayName"/> and <see cref="IAnnotationItemProvider.GetDisplayName"/>).
		/// </summary>
		string ResolveLabel(string annotationIdentifier);
	}
}
