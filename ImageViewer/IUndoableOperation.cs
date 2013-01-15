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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// An interface for an operation on an object
	/// that implements undo/redo using the Memento pattern.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Memento design pattern allows client code to perform undo/redo operations on
	/// an object without understanding any of the internals of how the operation is undone.
	/// All that is needed is a 'memento' from before and after any modifications are made
	/// to the object in question.  These mementos can then be used to construct a 
	/// single <see cref="MemorableUndoableCommand"/> to be entered into the 
	/// <see cref="CommandHistory"/> and the changes to the object's state
	/// can then be 'undone' and 'redone', simply be passing the memento back to the object.
	/// </para>
	/// <para>
	/// This interface is merely an abstraction that allows client code to apply the Memento
	/// pattern to an arbitrary object without any understanding of the operation being applied
	/// or the participating objects.
	/// </para>
	/// </remarks>
	public interface IUndoableOperation<T> where T : class
	{
		/// <summary>
		/// In the memento pattern, the 'originator' is the object whose state is being
		/// captured and restored via a memento.
		/// </summary>
		/// <remarks>
		/// In this interface definition, the originator is purposely not of <typeparamref name="T">type T</typeparamref>
		/// because you may actually want to perform the operation on an object that is not itself
		/// <see cref="IMemorable">memorable</see>, but rather on some <see cref="IMemorable">memorable</see> property.
		/// </remarks>
		IMemorable GetOriginator(T item);

		/// <summary>
		/// Gets whether or not this operation applies to the given item.
		/// </summary>
		bool AppliesTo(T item);

		/// <summary>
		/// Applies the operation to the given item.
		/// </summary>
		/// <param name="item"></param>
		void Apply(T item);
	}
}