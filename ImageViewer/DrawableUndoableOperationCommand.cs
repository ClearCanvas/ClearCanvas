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

using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A specialization of <see cref="UndoableOperationCommand{T}"/> especially for
	/// objects that implement <see cref="IDrawable"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Once an <see cref="UndoableCommand"/> is in the <see cref="CommandHistory"/>, a mechanism
	/// is needed to cause visible <see cref="IDrawable"/> objects, such as <see cref="IPresentationImage"/>s,
	/// to draw when an operation is undone or redone.  For those objects that implement undo/redo using the 
	/// Memento pattern (and <see cref="IUndoableOperation{T}"/>), and are also <see cref="IDrawable"/>,
	/// this class will automatically redraw the object after the <see cref="IUndoableOperation{T}"/> has been undone or redone.
	/// </para>
	/// <para>
	/// See <see cref="UndoableOperationCommand{T}"/> for a detailed explanation.
	/// </para>
	/// </remarks>
	public class DrawableUndoableOperationCommand<T> : UndoableOperationCommand<T> where T : class, IDrawable
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="operation">The <see cref="IUndoableOperation{T}"/> that is to be applied to <paramref name="item"/>.</param>
		/// <param name="item">The item to which the <paramref name="operation"/> should be applied.</param>
		public DrawableUndoableOperationCommand(IUndoableOperation<T> operation, T item)
			: base(operation, item)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="operation">The <see cref="IUndoableOperation{T}"/> that is to be applied to <paramref name="items"/>.</param>
		/// <param name="items">The items to which the <paramref name="operation"/> should be applied.</param>
		public DrawableUndoableOperationCommand(IUndoableOperation<T> operation, IEnumerable<T> items)
			: base(operation, items)
		{
		}

		/// <summary>
		/// On the first call, the <see cref="IUndoableOperation{T}"/> is applied and this
		/// object is populated with a <see cref="MemorableUndoableCommand"/>s wrapped in a
		/// <see cref="DrawableUndoableCommand"/> to first undo (or redo) the operation and
		/// then draw the object.
		/// </summary>
		/// <remarks>
		/// You should call <see cref="Execute"/> and check that the <see cref="CompositeUndoableCommand.Count"/>
		/// is non-zero before adding to the <see cref="CommandHistory"/>.  Otherwise, there will end up
		/// being 'no-op' commands in the <see cref="CommandHistory"/>.
		/// </remarks>
		public sealed override void Execute()
		{
			base.Execute();
		}

		/// <summary>
		/// <see cref="CompositeUndoableCommand.Unexecute"/>s each command, from the end to the beginning.
		/// </summary>
		public sealed override void Unexecute()
		{
			base.Unexecute();
		}

		/// <summary>
		/// Applies <paramref name="operation"/> to <paramref name="item"/> and returns
		/// an <see cref="UndoableCommand"/> that can undo and redo the operation.
		/// </summary>
		/// <returns>
		/// An <see cref="UndoableCommand"/> if application of <paramref name="operation"/>
		/// resulted in a change to the internal state of <paramref name="item"/>, otherwise null.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation takes the <see cref="MemorableUndoableCommand"/> returned
		/// by the base method and wraps it in a <see cref="DrawableUndoableCommand"/>.
		/// </para>
		/// <para>
		/// Inheritors can override this method to do any additional processing and/or to
		/// modify the resulting command, if necessary.
		/// </para>
		/// </remarks>
		protected override UndoableCommand Apply(IUndoableOperation<T> operation, T item)
		{
			UndoableCommand command = base.Apply(operation, item);

			if (command != null)
			{
				item.Draw();

				DrawableUndoableCommand drawableCommand = new DrawableUndoableCommand(item);
				drawableCommand.Enqueue(command);
				command = drawableCommand;
			}

			return command;
		}
	}
}
