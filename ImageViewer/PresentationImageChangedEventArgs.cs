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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="Tile.PresentationImageChanged"/> event.
	/// </summary>
	public class PresentationImageChangedEventArgs : EventArgs
	{
		private IPresentationImage _oldPresentationImage;
		private IPresentationImage _newPresentationImage;

		/// <summary>
		/// Initializes a new instance of <see cref="PresentationImageChangedEventArgs"/>.
		/// </summary>
		/// <param name="oldPresentationImage"></param>
		/// <param name="newPresentationImage"></param>
		public PresentationImageChangedEventArgs(
			IPresentationImage oldPresentationImage,
			IPresentationImage newPresentationImage)
		{
			_oldPresentationImage = oldPresentationImage;
			_newPresentationImage = newPresentationImage;
		}

		/// <summary>
		/// Gets the old <see cref="IPresentationImage"/>.
		/// </summary>
		public IPresentationImage OldPresentationImage
		{
			get { return _oldPresentationImage; }
		}

		/// <summary>
		/// Gets the new <see cref="IPresentationImage"/>.
		/// </summary>
		public IPresentationImage NewPresentationImage
		{
			get { return _newPresentationImage; }
		}
	}
}
