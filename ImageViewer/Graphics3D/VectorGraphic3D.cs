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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics3D
{
	/// <summary>
	/// An vector <see cref="Graphic3D"/>.
	/// </summary>
	[Cloneable(true)]
	public abstract class VectorGraphic3D : Graphic3D
	{
		/// <summary>
		/// The hit test distance in destination pixels.
		/// </summary>
		public static readonly int HitTestDistance = 10;

		private Color _color = Color.Yellow;

		/// <summary>
		/// Initializes a new instance of <see cref="VectorGraphic3D"/>.
		/// </summary>
		protected VectorGraphic3D() {}

		/// <summary>
		/// Gets or sets the colour.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
				{
					_color = value;
					NotifyVisualStateChanged("Color");
				}
			}
		}
	}
}