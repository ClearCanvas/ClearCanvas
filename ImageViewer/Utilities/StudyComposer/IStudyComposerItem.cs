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

using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	/// <summary>
	/// An interface for items in the <see cref="StudyComposerComponent"/> tree.
	/// </summary>
	public interface IStudyComposerItem
	{
		/// <summary>
		/// Indicates that a property on the node has changed, and that any views should refresh its display of the item.
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Gets or sets the name label of this item.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets a short, multi-line description of the item that contains ancillary information.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the <see cref="StudyBuilderNode"/> node that is encapsulated by this <see cref="StudyComposerItemBase{T}"/>.
		/// </summary>
		StudyBuilderNode Node { get; }

		/// <summary>
		/// Gets an <see cref="Image"/> icon that can be used to represent the item in thumbnail views.
		/// </summary>
		Image Icon { get; }

		/// <summary>
		/// Regenerates the icon for a specific icon size.
		/// </summary>
		/// <param name="iconSize">The <see cref="Size"/> of the icon to generate.</param>
		void UpdateIcon(Size iconSize);

		/// <summary>
		/// Regenerates the icon for the default icon size (64x64).
		/// </summary>
		void UpdateIcon();

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		IStudyComposerItem Clone();
	}
}