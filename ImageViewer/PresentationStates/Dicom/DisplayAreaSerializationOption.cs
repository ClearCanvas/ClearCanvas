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

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Enumerated values that specify how the displayed area of an image should be serialized in a presentation state.
	/// </summary>
	/// <remarks>
	/// <para>If the image with presentation state is being rendered in a screen rectangle of the same size as when the presentation
	/// state was created, then the displayed area is the same. However, this is the exception rather than the norm. The typical
	/// use case for presentation states would have the rendering of the image take place on a different workstation, typically
	/// with differing physical monitor size, screen resolution, window dimensions, window aspect ratio, etc. These enumerated
	/// values serve to specify which quality of the visible area should be preserved in such circumstances.</para>
	/// <para>An alternative description can be found in DICOM Standard 2008, PS 3.3 C.10.4.</para>
	/// </remarks>
	public enum DisplayAreaSerializationOption
	{
		/// <summary>
		/// Specifies that maintaining the same minimum visible area has higher precedence over maintaining the same magnification.
		/// </summary>
		/// <remarks>
		/// In cases where the aspect ratio of the screen rectangle where the image and presentation state is being rendered differs
		/// from that of the creator, then the magnification will be adjusted such that, at a minimum, the original visible area
		/// remains completely visible.
		/// </remarks>
		SerializeAsDisplayedArea,

		/// <summary>
		/// Specifies that maintaining the true physical size of the image has higher precedence over maintaining the same visible area.
		/// </summary>
		SerializeAsTrueSize,

		/// <summary>
		/// Specifies that maintaining the same magnification ratio has higher precedence over maintaining the same visible area.
		/// </summary>
		SerializeAsMagnification
	}
}