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
	/// Provides data for the <see cref="EventBroker.GraphicSelectionChanged"/> event.
	/// </summary>
	public class GraphicSelectionChangedEventArgs : EventArgs
	{
		private readonly ISelectableGraphic _selectedGraphic;
		private readonly ISelectableGraphic _deselectedGraphic;

		/// <summary>
		/// Intializes a new instance of <see cref="GraphicSelectionChangedEventArgs"/>.
		/// </summary>
		/// <param name="selectedGraphic">The graphic that was selected. Can be <b>null</b> if there is no currently selected graphic.</param>
		/// <param name="deselectedGraphic">The graphic that was previously selected. Can be <b>null</b> if there was previously no selected graphic.</param>
		internal GraphicSelectionChangedEventArgs(ISelectableGraphic selectedGraphic, ISelectableGraphic deselectedGraphic)
		{
			_selectedGraphic = selectedGraphic;
			_deselectedGraphic = deselectedGraphic;
		}

		/// <summary>
		/// Gets the selected <see cref="IGraphic"/>. Can be <b>null</b> if there is no currently selected graphic.
		/// </summary>
		public ISelectableGraphic SelectedGraphic
		{
			get { return _selectedGraphic; }
		}

		/// <summary>
		/// Gets the deselected <see cref="IGraphic"/>. Can be <b>null</b> if there was previously no selected graphic.
		/// </summary>
		public ISelectableGraphic DeselectedGraphic
		{
			get { return _deselectedGraphic; }
		}
	}
}