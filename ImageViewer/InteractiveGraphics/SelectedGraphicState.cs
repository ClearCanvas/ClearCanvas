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
	/// Represents the 'selected' graphic state.
	/// </summary>
	/// <remarks>
	/// This state is different from <see cref="FocussedSelectedGraphicState"/> in 
	/// that it is entered when the
	/// <see cref="IStandardStatefulGraphic"/> is selected (i.e. clicked on)
	/// but <i>not</i> focussed (i.e. hovered over), whereas <see cref="FocussedSelectedGraphicState"/>
	/// is entered only when a graphic is selected <i>and</i> focussed.
	/// </remarks>
	public class SelectedGraphicState : StandardGraphicState
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SelectedGraphicState"/>.
		/// </summary>
		/// <param name="standardStatefulGraphic"></param>
		public SelectedGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{
		}

		/// <summary>
		/// Called by the framework when the user clicks away from the 
		/// associated <see cref="IStandardStatefulGraphic"/>
		/// and results in a transition to the <see cref="InactiveGraphicState"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			if (!this.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateInactiveState();
				return false;
			}

			return true;
		}

		/// <summary>
		/// Called by the framework when the mouse is moving and results in a transition 
		/// to the <see cref="FocussedGraphicState"/> when
		/// the mouse hovers over the associated 
		/// <see cref="IStandardStatefulGraphic"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns></returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			if (this.StatefulGraphic.HitTest(mouseInformation.Location))
			{
				this.StatefulGraphic.State = this.StatefulGraphic.CreateFocussedSelectedState();
				return true;
			}

			return false;
		}
	}
}
