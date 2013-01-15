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

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A subclass of <see cref="UndoableCommand"/> for <see cref="IDrawable"/> objects.
	/// </summary>
	/// <remarks>
	/// Often, when an <see cref="UndoableCommand"/> is <see cref="Execute"/>d or
	/// <see cref="Unexecute"/>d, it is necessary to refresh an <see cref="IDrawable"/>
	/// object, such as an <see cref="ITile"/>.  This class automatically calls
	/// <see cref="IDrawable.Draw"/> on the object passed to the constructor
	/// after <see cref="Execute"/> and <see cref="Unexecute"/>.
	/// </remarks>
	public class DrawableUndoableCommand : CompositeUndoableCommand
	{
		private readonly IDrawable _drawable;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="drawable">The object to redraw after <see cref="Execute"/> or <see cref="Unexecute"/>.</param>
		public DrawableUndoableCommand(IDrawable drawable)
		{
			Platform.CheckForNullReference(drawable, "drawable");

			_drawable = drawable;
		}

		/// <summary>
		/// Gets the <see cref="IDrawable"/> object that will be redrawn on <see cref="Unexecute"/>
		/// or <see cref="Execute"/>.
		/// </summary>
		protected IDrawable Drawable
		{
			get { return _drawable; }	
		}

		/// <summary>
		/// Performs a 'redo' and calls <see cref="IDrawable.Draw"/> on <see cref="Drawable"/> afterwards.
		/// </summary>
		public override void Execute()
		{
			base.Execute();
			_drawable.Draw();
		}

		/// <summary>
		/// Performs an 'undo' and calls <see cref="IDrawable.Draw"/> on <see cref="Drawable"/> afterwards.
		/// </summary>
		public override void Unexecute()
		{
			base.Unexecute();
			_drawable.Draw();
		}
	}
}
