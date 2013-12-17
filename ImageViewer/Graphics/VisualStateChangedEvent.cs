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

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Represents the method that will handle the <see cref="IGraphic.VisualStateChanged"/> event raised
	/// when a property is changed on a graphic, resulting in a change in the graphic's visual state.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="VisualStateChangedEventArgs"/> that contains the event data. </param>
	public delegate void VisualStateChangedEventHandler(object sender, VisualStateChangedEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="IGraphic.VisualStateChanged"/> event. 
	/// </summary>
	public sealed class VisualStateChangedEventArgs : PropertyChangedEventArgs
	{
		private readonly VisualStatePropertyKind _propertyKind;
		private readonly IGraphic _graphic;

		/// <summary>
		/// Gets the graphic whose visual state changed.
		/// </summary>
		public IGraphic Graphic
		{
			get { return _graphic; }
		}

		/// <summary>
		/// Gets the kind of the property that changed.
		/// </summary>
		public VisualStatePropertyKind PropertyKind
		{
			get { return _propertyKind; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VisualStateChangedEventArgs"/> class. 
		/// </summary>
		/// <param name="graphic">The graphic whose visual state changed.</param>
		/// <param name="propertyName">The name of the property that changed. </param>
		/// <param name="propertyKind">The kind of the property that changed.</param>
		public VisualStateChangedEventArgs(IGraphic graphic, string propertyName, VisualStatePropertyKind propertyKind = VisualStatePropertyKind.Unspecified)
			: base(propertyName)
		{
			_graphic = graphic;
			_propertyKind = propertyKind;
		}
	}

	/// <summary>
	/// Indiciates the kind of visual state property.
	/// </summary>
	public enum VisualStatePropertyKind
	{
		/// <summary>
		/// Indicates that the property kind is unspecified.
		/// </summary>
		Unspecified,

		/// <summary>
		/// Indicates that the property affects the geometry (e.g. coordinates, angles, etc.) of the graphic.
		/// </summary>
		Geometry,

		/// <summary>
		/// Indicates that the property affects the appearance (e.g. line style, color, etc.) of the graphic.
		/// </summary>
		Appearance,

		/// <summary>
		/// Indicates that the property affects the textual content of the graphic.
		/// </summary>
		Text,

		/// <summary>
		/// Indicates that the property affects the font (e.g. font face, size, weight, etc.) of the textual content of the graphic.
		/// </summary>
		Font
	}
}