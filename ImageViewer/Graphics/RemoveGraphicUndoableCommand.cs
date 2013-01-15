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
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An <see cref="UndoableCommand"/> for removing an <see cref="IGraphic"/> from a <see cref="GraphicCollection"/>.
	/// </summary>
	public class RemoveGraphicUndoableCommand : UndoableCommand
	{
		private Command _command;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// The <paramref name="graphic"/>'s <see cref="IGraphic.ParentGraphic"/> must not be null.
		/// </remarks>
		public RemoveGraphicUndoableCommand(IGraphic graphic)
		{
			_command = new RemoveGraphicCommand(graphic);
		}

		/// <summary>
		/// <see cref="Execute"/>s the remove command.
		/// </summary>
		public override void Execute()
		{
			if (_command is RemoveGraphicCommand)
			{
				_command.Execute();
				_command = ((RemoveGraphicCommand)_command).GetUndoCommand();
			}
		}

		/// <summary>
		/// <see cref="Unexecute"/>s the remove command (e.g. re-inserts the graphic).
		/// </summary>
		public override void Unexecute()
		{
			if (_command is InsertGraphicCommand)
			{
				_command.Execute();
				_command = ((InsertGraphicCommand)_command).GetUndoCommand();
			}
		}
	}
	
	internal class RemoveGraphicCommand : Command
	{
		private IGraphic _graphic;
		private InsertGraphicCommand _undoCommand;

		public RemoveGraphicCommand(IGraphic graphic)
		{
			Platform.CheckForNullReference(graphic, "graphic");

			_graphic = graphic;
			Validate();
		}

		private void Validate()
		{
			CompositeGraphic parentGraphic = _graphic.ParentGraphic as CompositeGraphic;
			if (parentGraphic == null)
				throw new InvalidOperationException("The graphic must have a parent.");
		}

		public InsertGraphicCommand GetUndoCommand()
		{
			if (_undoCommand == null)
				throw new InvalidOperationException("The command must be executed first.");

			return _undoCommand;
		}

		public override void Execute()
		{
			if (_undoCommand != null)
				throw new InvalidOperationException("The command has already been executed.");

			Validate();

			GraphicCollection parentCollection = ((CompositeGraphic)_graphic.ParentGraphic).Graphics;
			int restoreIndex = parentCollection.IndexOf(_graphic);
			parentCollection.Remove(_graphic);

			_undoCommand = new InsertGraphicCommand(_graphic, parentCollection, restoreIndex);
			_graphic = null;
		}
	}
}
