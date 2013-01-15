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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Allows object state to be captured and restored.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="IMemorable"/> can be implemented by classes that require support
	/// for the <i>Memento</i> design pattern--<see cref="IMemorable"/> acts as the
	/// <i>Originator</i>.  Typically, the <see cref="IMemorable"/>
	/// interface is used in conjunction with <see cref="MemorableUndoableCommand"/> 
	/// to provide undo/redo support.
	/// </para>
	/// <para>
	///	It is common in the framework to use a 'begin state' and 'end state' memento
	/// that comprise an <see cref="MemorableUndoableCommand"/>.  It is also common to check
	/// the equality of the two mementos in order to decide whether or not an
	/// <see cref="MemorableUndoableCommand"/> should be added to a <see cref="CommandHistory"/>
	/// object.  Therefore, it is good practice to override and implement the
	/// <see cref="object.Equals(object)"/> method on memento classes.
	/// </para>
	/// </remarks>
	/// <seealso cref="MemorableUndoableCommand"/>
	/// <seealso cref="CommandHistory"/>
	public interface IMemorable
	{
		/// <summary>
		/// Captures the state of an object.
		/// </summary>
		/// <remarks>
		/// The implementation of <see cref="CreateMemento"/> should return an
		/// object containing enough state information so that
		/// when <see cref="SetMemento"/> is called, the object can be restored
		/// to the original state.
		/// </remarks>
		object CreateMemento();

		/// <summary>
		/// Restores the state of an object.
		/// </summary>
		/// <param name="memento">The object that was
		/// originally created with <see cref="CreateMemento"/>.</param>
		/// <remarks>
		/// The implementation of <see cref="SetMemento"/> should return the 
		/// object to the original state captured by <see cref="CreateMemento"/>.
		/// </remarks>
		void SetMemento(object memento);
	}
}
