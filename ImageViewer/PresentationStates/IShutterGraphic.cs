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

using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// Defines properties to control the presentation display of a graphic which may be used as a display shutter.
	/// </summary>
	public interface IShutterGraphic : IGraphic
	{
		/// <summary>
		/// Gets or sets the 16-bit grayscale presentation value which should replace the shuttered pixels.
		/// </summary>
		/// <remarks>
		/// The display of shuttered pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="PresentationValue"/> and <see cref="PresentationColor"/>
		/// properties allows this display to be customized by client code.
		/// </remarks>
		ushort PresentationValue { get; set; }

		/// <summary>
		/// Gets or sets the presentation color which should replace the shuttered pixels.
		/// </summary>
		/// <remarks>
		/// The display of shuttered pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="PresentationValue"/> and <see cref="PresentationColor"/>
		/// properties allows this display to be customized by client code.
		/// </remarks>
		Color PresentationColor { get; set; }
	}
}