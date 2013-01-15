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

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// Defines the interface of a command used by the <see cref="CommandProcessor"/>
    /// which includes several internal additional commands that must be rolled back.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an <see cref="ICommand"/> also is defined as an IAggregateServerCommand,
    /// it is assumed that the command will execute its sub-commands through the 
    /// <see cref="CommandProcessor.ExecuteSubCommand"/> method.  This method will 
    /// automatically add the commands as they are executed to the <see cref="AggregateCommands"/>
    /// property to ensure proper later rollback.
    /// </para>
    /// <para>
    /// If an error occurs that causes a Rollback, the <see cref="CommandProcessor"/> will 
    /// automatically also rollback the commands associated with the IAggregateServerCommand
    /// by looking at the <see cref="AggregateCommands"/> property.
    /// </para>
    /// </remarks>
    public interface IAggregateCommand : ICommand
    {
        Stack<ICommand> AggregateCommands { get; }
    }
}
