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

using ClearCanvas.Desktop;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// An <see cref="UndoableCommand"/> specifically for operations on objects
	/// that implement the Memento pattern using <see cref="IMemorable"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class is used in conjunction with <see cref="IUndoableOperation{T}"/>, which
	/// is intended to streamline application of the same operation to objects implementing
	/// the Memento pattern via <see cref="IMemorable"/>.  See the comments for 
	/// <see cref="IUndoableOperation{T}"/> for information on the pattern
	/// of applying the operation and creating the <see cref="MemorableUndoableCommand"/>.
	/// </para>
	/// <para>
	/// The <see cref="UndoableOperationCommand{T}"/> removes all the burden of properly
	/// applying the <see cref="IUndoableOperation{T}"/> and obtaining the mementos for
	/// any number of objects.  Given the <see cref="IUndoableOperation{T}"/> and a list
	/// of items to which the operation should be applied, the first call to <see cref="Execute"/>
	/// will automatically apply the <see cref="IUndoableOperation{T}"/> and populate
	/// itself with a <see cref="MemorableUndoableCommand"/> for each object that actually
	/// changed as a result of the operation.  Since the application of the operation
	/// is not guaranteed to result in any changes, you should check that the
	/// <see cref="CompositeUndoableCommand.Count"/> property is non-zero after first calling
	/// <see cref="Execute"/> before adding the <see cref="UndoableOperationCommand{T}"/>
	/// to the <see cref="CommandHistory"/>.
	/// </para>
	/// </remarks>
	/// <seealso cref="IMemorable"/>
	/// <seealso cref="IUndoableOperation{T}"/>
	/// <seealso cref="CompositeUndoableCommand"/>
	public class UndoableOperationCommand<T> : CompositeUndoableCommand where T : class 
	{
		private IUndoableOperation<T> _operation;
		private IEnumerable<T> _items;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="operation">The <see cref="IUndoableOperation{T}"/> that is to be applied to <paramref name="item"/>.</param>
		/// <param name="item">The item to which the <paramref name="operation"/> should be applied.</param>
		public UndoableOperationCommand(IUndoableOperation<T> operation, T item)
			: this(operation, ToEnumerable(item))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="operation">The <see cref="IUndoableOperation{T}"/> that is to be applied to <paramref name="items"/>.</param>
		/// <param name="items">The items to which the <paramref name="operation"/> should be applied.</param>
		public UndoableOperationCommand(IUndoableOperation<T> operation, IEnumerable<T> items)
		{
			_operation = operation;
			_items = items;
		}

		private static IEnumerable<U> ToEnumerable<U>(U item) where U : class
		{
			yield return item;
		}

		/// <summary>
		/// On the first call, the <see cref="IUndoableOperation{T}"/> is applied and this
		/// object is populated with the <see cref="MemorableUndoableCommand"/>s to undo
		/// the operation.
		/// </summary>
		/// <remarks>
		/// You should call <see cref="Execute"/> and check that the <see cref="CompositeUndoableCommand.Count"/>
		/// is non-zero before adding to the <see cref="CommandHistory"/>.  Otherwise, there will end up
		/// being 'no-op' commands in the <see cref="CommandHistory"/>.
		/// </remarks>
		public override void Execute()
		{
			if (_operation != null)
			{
				foreach (T item in _items)
				{
					UndoableCommand command = Apply(_operation, item);
					if (command != null)
						Enqueue(command);
				}

				_operation = null;
				_items = null;
			}
			else
			{
				base.Execute();
			}
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
		/// Inheritors can override this method to do any additional processing and/or to
		/// modify the resulting command, if necessary.
		/// </remarks>
		protected virtual UndoableCommand Apply(IUndoableOperation<T> operation, T item)
		{
			if (operation.AppliesTo(item))
			{
				IMemorable originator = operation.GetOriginator(item);
				if (originator != null)
				{
					object beginState = originator.CreateMemento();
					if (beginState != null)
					{
						operation.Apply(item);

						object endState = originator.CreateMemento();
						if (!Equals(beginState, endState))
						{
							MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(originator);
							memorableCommand.BeginState = beginState;
							memorableCommand.EndState = endState;

							return memorableCommand;
						}
					}
				}
			}

			return null;
		}
	}
}
