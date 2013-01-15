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

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// Defines the interface of a command used by the <see cref="CommandProcessor"/>
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets and sets the execution context for the command.
        /// </summary>
        ICommandProcessorContext ProcessorContext { set; get; }

        /// <summary>
        /// Gets and sets a value describing what the command is doing.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets a value describing if the ServerCommand requires a rollback of the operation its included in if it fails during execution.
        /// </summary>
        bool RequiresRollback { get; set; }

        /// <summary>
        /// Execute the ServerCommand.
        /// </summary>
        void Execute(CommandProcessor theProcessor);

        /// <summary>
        /// Undo the operation done by <see cref="Execute"/>.
        /// </summary>
        void Undo();
    }
}