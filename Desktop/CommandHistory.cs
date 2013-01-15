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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop 
{
	/// <summary>
	/// Class that facilitates undo/redo by storing incremental changes
	/// in the form of <see cref="UndoableCommand"/>s.
	/// </summary>
	/// <seealso cref="UndoableCommand"/>
	/// <seealso cref="IMemorable"/>
	public class CommandHistory 
	{
		// Private attributes
		private List<UndoableCommand> _history = new List<UndoableCommand>();
		private int _maxSize = 0;
		private int _currentCommandIndex = -1;
		private int _lastCommandIndex = -1;

		private event EventHandler _currentCommandChangingEvent;
		private event EventHandler _currentCommandChangedEvent;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="maxSize">The maximum number of <see cref="UndoableCommand"/>s to keep in memory.</param>
		public CommandHistory(int maxSize)
		{
			Platform.CheckPositive(maxSize, "maxSize");

			_maxSize = maxSize;
		}

		/// <summary>
		/// Gets the current number of <see cref="UndoableCommand"/>s stored.
		/// </summary>
		public int NumCommands 
		{
			get
			{
				return _history.Count;
			}
		}

		/// <summary>
		/// Gets the maximum number of <see cref="UndoableCommand"/>s that can be stored in memory by this object.
		/// </summary>
		public int MaxSize
		{
			get
			{
				return _maxSize;
			}
		}

		/// <summary>
		/// Gets the index of the current command in the command history.
		/// </summary>
		public int CurrentCommandIndex
		{
			get
			{
				return _currentCommandIndex;
			}
		}

		/// <summary>
		/// Gets the index of the last command in the command history.
		/// </summary>
		public int LastCommandIndex
		{
			get
			{
				return _lastCommandIndex;
			}
		}

		/// <summary>
		/// Indicates that the <see cref="CurrentCommandIndex"/> is about to change
		/// because of a call to <see cref="Undo"/>, <see cref="Redo"/> or <see cref="AddCommand"/>.
		/// </summary>
		public event EventHandler CurrentCommandChanging
		{
			add { _currentCommandChangingEvent += value; }	
			remove { _currentCommandChangingEvent -= value; }	
		}

		/// <summary>
		/// Indicates that the <see cref="CurrentCommandIndex"/> has changed
		/// because of a call to <see cref="Undo"/>, <see cref="Redo"/> or <see cref="AddCommand"/>.
		/// </summary>
		public event EventHandler CurrentCommandChanged
		{
			add { _currentCommandChangedEvent += value; }
			remove { _currentCommandChangedEvent -= value; }
		}

		/// <summary>
		/// Adds a command to the command history.
		/// </summary>
		/// <remarks>
		/// When a command is added, all commands after the <see cref="CurrentCommandIndex"/> will be removed
		/// in order to keep the state of the application consistent.  The added command will then become
		/// the last command (<see cref="LastCommandIndex"/>).
		/// </remarks>
		public void AddCommand(UndoableCommand command)
		{
			Platform.CheckForNullReference(command, "command");

			EventsHelper.Fire(_currentCommandChangingEvent, this, EventArgs.Empty);

			if (_currentCommandIndex < _lastCommandIndex)
			{
				int numCommandsToRemove = _lastCommandIndex - _currentCommandIndex;
				_history.RemoveRange(_currentCommandIndex + 1, numCommandsToRemove);
				_lastCommandIndex -= numCommandsToRemove;
			}

			_history.Add(command);

			if (NumCommands > _maxSize)
			{
				_history.RemoveAt(0);

				if (_currentCommandIndex == _lastCommandIndex)
					_currentCommandIndex--;

				_lastCommandIndex--;
			}

			_currentCommandIndex++;
			_lastCommandIndex++;

			EventsHelper.Fire(_currentCommandChangedEvent, this, EventArgs.Empty);
		}

		/// <summary>
		/// Calls <see cref="Command.Execute"/> in order to 'redo' the current command.
		/// </summary>
		/// <remarks>
		/// The <see cref="CurrentCommandIndex"/> will be incremented unless the <see cref="CurrentCommandIndex"/> is 
		/// the same as the <see cref="LastCommandIndex"/>, in which case only <see cref="Undo"/> operations can occur.
		/// </remarks>
		public void Redo()
		{
			if (NumCommands == 0)
				return;

			EventsHelper.Fire(_currentCommandChangingEvent, this, EventArgs.Empty);

			if (_currentCommandIndex == _lastCommandIndex)
				return;

			_currentCommandIndex++;
			UndoableCommand cmd = _history[_currentCommandIndex];

			try
			{
				cmd.Execute();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			EventsHelper.Fire(_currentCommandChangedEvent, this, EventArgs.Empty);
		}

		/// <summary>
		/// Calls <see cref="UndoableCommand.Unexecute"/> in order to 'undo' the current command.
		/// </summary>
		/// <remarks>
		/// Decrements the <see cref="CurrentCommandIndex"/>, unless it is already -1.  A <see cref="CurrentCommandIndex"/> of -1
		/// indicates that the entire command history has been undone and only <see cref="Redo"/> operations can occur.
		/// </remarks>
		public void Undo()
		{
			if (NumCommands == 0)
				return;

			if (_currentCommandIndex == -1)
				return;

			EventsHelper.Fire(_currentCommandChangingEvent, this, EventArgs.Empty);

			UndoableCommand cmd = _history[_currentCommandIndex];

			try
			{
				cmd.Unexecute();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			_currentCommandIndex--;
			EventsHelper.Fire(_currentCommandChangedEvent, this, EventArgs.Empty);
		}

        /// <summary>
        /// Clears the command history.
        /// </summary>
        public void Clear()
        {
            EventsHelper.Fire(_currentCommandChangingEvent, this, EventArgs.Empty);
            _history.Clear();
            _currentCommandIndex = -1;
            _lastCommandIndex = -1;
            EventsHelper.Fire(_currentCommandChangedEvent, this, EventArgs.Empty);
        }
    }
}
