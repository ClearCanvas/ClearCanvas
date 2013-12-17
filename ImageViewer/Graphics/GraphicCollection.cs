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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A collection of <see cref="IGraphic"/> objects.
	/// </summary>
	public class GraphicCollection : ObservableList<IGraphic>
	{
		private readonly IGraphic _parentGraphic;

		/// <summary>
		/// Instantiates a new instance of <see cref="GraphicCollection"/>.
		/// </summary>
		internal GraphicCollection(IGraphic parentGraphic)
		{
			_parentGraphic = parentGraphic;
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not this <see cref="GraphicCollection"/> is visible.
		/// </summary>
		/// <remarks>
		/// This property is effectively the same as the <see cref="IGraphic.Visible"/> property on the collection's parent graphic.
		/// </remarks>
		public bool Visible
		{
			get { return _parentGraphic.Visible; }
			set { _parentGraphic.Visible = value; }
		}
	}
}