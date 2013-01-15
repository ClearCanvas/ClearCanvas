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

using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// Clipboard authority tokens.
	/// </summary>
	public class AuthorityTokens
	{
		/// <summary>
		/// Clipboard tokens
		/// </summary>
		public class Clipboard
		{
			/// <summary>
			/// Clipboard export tokens
			/// </summary>
			public class Export
			{
				/// <summary>
				/// Permission to export clipboard items into JPG files.
				/// </summary>
				[AuthorityToken(Description = "Permission to export clipboard items into JPG files.")]
				public const string JPG = "Viewer/Clipboard/Export/JPG";

				/// <summary>
				/// Permission to export clipboard items into AVI files.
				/// </summary>
				[AuthorityToken(Description = "Permission to export clipboard items into AVI files.")]
				public const string AVI = "Viewer/Clipboard/Export/AVI";
			}
		}
	}
}
