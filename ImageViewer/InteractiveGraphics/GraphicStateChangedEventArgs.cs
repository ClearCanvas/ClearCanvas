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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Provides data for the <see cref="StatefulCompositeGraphic.StateChanged"/>
	/// event.
	/// </summary>
	public class GraphicStateChangedEventArgs : EventArgs
	{
		private IStatefulGraphic _statefulGraphic;
		private GraphicState _oldState;
		private GraphicState _newState;
		private IMouseInformation _mouseInformation;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GraphicStateChangedEventArgs()
		{

		}

		/// <summary>
		/// Gets the <see cref="IStatefulGraphic"/>.
		/// </summary>
		public IStatefulGraphic StatefulGraphic
		{
			get { return _statefulGraphic; }
			set { _statefulGraphic = value; }
		}

		/// <summary>
		/// Gets the old <see cref="GraphicState"/>.
		/// </summary>
		public GraphicState OldState
		{
			get { return _oldState; }
			set { _oldState = value; }
		}

		/// <summary>
		/// Gets the new <see cref="GraphicState"/>.
		/// </summary>
		public GraphicState NewState
		{
			get { return _newState; }
			set 
			{
				Platform.CheckForNullReference(value, "NewState");
				_newState = value; 
			}
		}

		/// <summary>
		/// Gets the <see cref="IMouseInformation"/>.
		/// </summary>
		public IMouseInformation MouseInformation
		{
			get { return _mouseInformation; }
			set { _mouseInformation = value; }
		}
	}
}
