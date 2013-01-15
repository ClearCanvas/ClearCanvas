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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// Defines a container for <see cref="IImageSet"/> objects in an <see cref="MprViewerComponent"/>.
	/// </summary>
	public interface IMprWorkspace : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="MprViewerComponent"/>.
		/// </summary>
		MprViewerComponent MprViewer { get; }

		/// <summary>
		/// Gets a collection of <see cref="IImageSet"/> objects that belong to
		/// this <see cref="IMprWorkspace"/>.
		/// </summary>
		ObservableList<IImageSet> ImageSets { get; }
	}
}