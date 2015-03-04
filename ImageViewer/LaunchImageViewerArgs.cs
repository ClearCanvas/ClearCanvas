#region License

// Copyright (c) 2014, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Arguments for launching an <see cref="ImageViewerComponent"/>.
	/// </summary>
	public class LaunchImageViewerArgs
	{
		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public LaunchImageViewerArgs(WindowBehaviour windowBehaviour)
		{
			WindowBehaviour = windowBehaviour;
		}

		/// <summary>
		/// Gets the <see cref="WindowBehaviour"/> to be used to launch the <see cref="ImageViewerComponent"/>.
		/// </summary>
		public readonly WindowBehaviour WindowBehaviour;

		/// <summary>
		/// Gets or sets the title to be used for the <see cref="ImageViewerComponent"/> when launched.
		/// </summary>
		/// <remarks>
		/// Leave this value null if you want the title to be automatically determined.
		/// </remarks>
		public string Title;
	}
}