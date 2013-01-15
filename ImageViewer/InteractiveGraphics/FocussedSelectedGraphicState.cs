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

using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Represents the 'focussed-selected' graphic state.
	/// </summary>
	/// <remarks>
	/// This state is different from <see cref="SelectedGraphicState"/> in 
	/// that it is entered when the
	/// <see cref="IStandardStatefulGraphic"/> is selected (i.e. clicked on)
	/// <i>and</i> focussed (i.e. hovered over), whereas <see cref="SelectedGraphicState"/>
	/// is entered only when a graphic is selected, but not focussed.
	/// </remarks>
	public class FocussedSelectedGraphicState : StandardGraphicState
	{
		/// <summary>
		/// Initializes a new instance of <see cref="FocussedSelectedGraphicState"/>.
		/// </summary>
		/// <param name="standardStatefulGraphic"></param>
		public FocussedSelectedGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		/// <summary>
		/// Called by the framework when the associated <see cref="IStandardStatefulGraphic"/>
		/// is clicked on.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			//We should never actually get to here, but if we did, this should happen.
			this.StatefulGraphic.State = this.StatefulGraphic.CreateSelectedState();
			return false;
		}

		/// <summary>
		/// Called by the framework when the mouse is moving and results in a transition 
		/// to the <see cref="SelectedGraphicState"/> when
		/// the mouse is no longer hovering over the associated 
		/// <see cref="IStandardStatefulGraphic"/>.
		/// </summary>
		public override bool Track(IMouseInformation mouseInformation)
		{
			if (!this.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateSelectedState();
				return false;
			}

			return true;
		}
	}
}
