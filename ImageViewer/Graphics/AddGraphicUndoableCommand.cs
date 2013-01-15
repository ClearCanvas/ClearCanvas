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
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An <see cref="UndoableCommand"/> for adding a graphic to a <see cref="GraphicCollection"/>.
	/// </summary>
	public class AddGraphicUndoableCommand : UndoableCommand
	{
		private Command _command;

		/// <summary>
		/// Constructor.
		/// </summary>
		public AddGraphicUndoableCommand(IGraphic graphic, GraphicCollection parentCollection)
		{
			_command = new AddGraphicCommand(graphic, parentCollection);
		}

		/// <summary>
		/// On first call, adds the graphic to the collection.  Subsequent calls perform an insert.
		/// </summary>
		public override void Execute()
		{
			if (_command is AddGraphicCommand)
			{
				_command.Execute();
				_command = ((AddGraphicCommand)_command).GetUndoCommand();
			}
			else if (_command is InsertGraphicCommand)
			{
				//after the initial add has executed (and has been undone) it's an insert from then on.
				_command.Execute();
				_command = ((InsertGraphicCommand)_command).GetUndoCommand();
			}
		}

		/// <summary>
		/// <see cref="Unexecute"/>s the add or insert command (e.g. removes the graphic).
		/// </summary>
		public override void Unexecute()
		{
			if (_command is RemoveGraphicCommand)
			{
				_command.Execute();
				_command = ((RemoveGraphicCommand)_command).GetUndoCommand();
			}
		}
	}

	internal class AddGraphicCommand : Command
	{
		private GraphicCollection _parentCollection;
		private IGraphic _graphic;

		private RemoveGraphicCommand _undoCommand;

		public AddGraphicCommand(IGraphic graphic, GraphicCollection parentCollection)
		{
			Platform.CheckForNullReference(graphic, "graphic");
			Platform.CheckForNullReference(parentCollection, "parentCollection");

			_parentCollection = parentCollection;
			_graphic = graphic;
		}

		public override void Execute()
		{
			if (_undoCommand != null)
				throw new InvalidOperationException("The command has already been executed.");

			_parentCollection.Add(_graphic);
			_undoCommand = new RemoveGraphicCommand(_graphic);

			_parentCollection = null;
			_graphic = null;
		}

		internal Command GetUndoCommand()
		{
			if (_undoCommand == null)
				throw new InvalidOperationException("The command must be executed first.");

			return _undoCommand;
		}
	}
}