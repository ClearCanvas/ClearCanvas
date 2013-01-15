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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A strategy for determining what the <see cref="CursorToken"/> should be
	/// for a given <see cref="TargetGraphic"/>.
	/// </summary>
	[Cloneable(true)]
	public abstract class StretchCursorTokenStrategy : ICursorTokenProvider
	{
		[CloneIgnore]
		private IGraphic _targetGraphic;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected StretchCursorTokenStrategy()
		{
		}

		/// <summary>
		/// The target <see cref="Graphic"/> for which the <see cref="CursorToken"/>
		/// is to be determined.
		/// </summary>
		public IGraphic TargetGraphic
		{
			get { return _targetGraphic; }
			set { _targetGraphic = value; }
		}

		#region ICursorTokenProvider Members

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		public abstract CursorToken GetCursorToken(Point point);

		#endregion
	}
}
