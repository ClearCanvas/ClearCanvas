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

using System;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.GraphicFocusChanged"/> event.
	/// </summary>
	public class GraphicFocusChangedEventArgs : EventArgs
	{
		private readonly IFocussableGraphic _focusedGraphic;
		private readonly IFocussableGraphic _unfocusedGraphic;

		/// <summary>
		/// Intializes a new instance of <see cref="GraphicFocusChangedEventArgs"/>.
		/// </summary>
		/// <param name="focusedGraphic">The graphic that was focused. Can be <b>null</b> if there is no currently focused graphic.</param>
		/// <param name="unfocusedGraphic">The graphic that was previously focused. Can be <b>null</b> if there was previously no focused graphic.</param>
		internal GraphicFocusChangedEventArgs(IFocussableGraphic focusedGraphic, IFocussableGraphic unfocusedGraphic)
		{
			_focusedGraphic = focusedGraphic;
			_unfocusedGraphic = unfocusedGraphic;
		}

		/// <summary>
		/// Gets the focused <see cref="IGraphic"/>. Can be <b>null</b> if there is no currently focused graphic.
		/// </summary>
		public IFocussableGraphic FocusedGraphic
		{
			get { return _focusedGraphic; }
		}

		/// <summary>
		/// Gets the deselected <see cref="IGraphic"/>. Can be <b>null</b> if there was previously no focused graphic.
		/// </summary>
		public IFocussableGraphic UnfocusedGraphic
		{
			get { return _unfocusedGraphic; }
		}
	}
}