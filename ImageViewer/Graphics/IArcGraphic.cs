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

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an arc graphic.
	/// </summary>
	public interface IArcGraphic : IBoundableGraphic
	{
		/// <summary>
		/// Gets or sets the angle at which the arc begins.
		/// </summary>
		/// <remarks>
		/// It is good practice to set the <see cref="StartAngle"/> before the <see cref="SweepAngle"/>
		/// because in the case where a graphic is scaled differently in x than in y, the conversion
		/// of the <see cref="SweepAngle"/> from <see cref="CoordinateSystem.Source"/> to
		/// <see cref="CoordinateSystem.Destination"/> coordinates is dependent upon the <see cref="StartAngle"/>.
		/// However, under normal circumstances, where the scale in x and y are the same, the <see cref="StartAngle"/>
		/// and <see cref="SweepAngle"/> can be set independently.
		/// </remarks>
		float StartAngle { get; set; }

		/// <summary>
		/// Gets or sets the angle that the arc sweeps out.
		/// </summary>
		/// <remarks>
		/// See <see cref="StartAngle"/> for information on setting the <see cref="SweepAngle"/>.
		/// </remarks>
		float SweepAngle { get; set; }
	}
}