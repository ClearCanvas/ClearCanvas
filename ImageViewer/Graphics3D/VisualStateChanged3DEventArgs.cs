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

namespace ClearCanvas.ImageViewer.Graphics3D
{
	/// <summary>
	/// Represents the method that will handle the <see cref="IGraphic3D.VisualStateChanged"/> event raised
	/// when a property is changed on a graphic, resulting in a change in the graphic's visual state.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="VisualStateChanged3DEventArgs"/> that contains the event data. </param>
	public delegate void VisualStateChanged3DEventHandler(object sender, VisualStateChanged3DEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="IGraphic3D.VisualStateChanged"/> event. 
	/// </summary>
	public sealed class VisualStateChanged3DEventArgs : PropertyChangedEventArgs
	{
		/// <summary>
		/// Gets the graphic whose visual state changed.
		/// </summary>
		public readonly IGraphic3D Graphic;

		/// <summary>
		/// Initializes a new instance of the <see cref="VisualStateChanged3DEventArgs"/> class. 
		/// </summary>
		/// <param name="graphic">The graphic whose visual state changed.</param>
		/// <param name="propertyName">The name of the property that changed. </param>
		public VisualStateChanged3DEventArgs(IGraphic3D graphic, string propertyName)
			: base(propertyName)
		{
			Graphic = graphic;
		}
	}
}