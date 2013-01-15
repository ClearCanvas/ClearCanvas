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

using System.Drawing;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Utility class for working with <see cref="DialogSizeHint"/>.
	/// </summary>
	public static class SizeHintHelper
	{
		/// <summary>
		/// Translates the specified size hint into an absolute size.
		/// </summary>
		/// <param name="hint"></param>
		/// <param name="defaultSize"></param>
		/// <returns></returns>
		public static Size TranslateHint(DialogSizeHint hint, Size defaultSize)
		{
			switch (hint)
			{
				case DialogSizeHint.Small:
					return new Size(320, 240);
				case DialogSizeHint.Medium:
					return new Size(640, 480);
				case DialogSizeHint.Large:
					return new Size(800, 600);
				default:
					return defaultSize;
			}
		}
	}
}
