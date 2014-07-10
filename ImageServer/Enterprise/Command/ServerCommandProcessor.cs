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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Dicom.Utilities.Command;

namespace ClearCanvas.ImageServer.Enterprise.Command
{
    /// <summary>
	/// This class is used to execute and undo a series of <see cref="ICommand"/> instances.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The command pattern is used in the ImageServer whenever there are interactions while processing
	/// DICOM files.  The pattern allows undoing of the operations as files are being modified and
	/// data inserted into the database.  
	/// </para>
	/// <para>
	/// If <see cref="ServerDatabaseCommand"/> objects are included among the <see cref="CommandBase"/>
	/// instances, an <see cref="IUpdateContext"/> will be opened for the database, and will be used
	/// to execute each of the <see cref="ServerDatabaseCommand"/> instances.  If a failure occurs
	/// when executing the commands, the <see cref="IUpdateContext"/> will be rolled back.  If no
	/// failures occur, the context will be committed.
	/// </para>
	/// <para>
	/// When implementing <see cref="ServerDatabaseCommand"/> instances, it is recommended to group these
	/// together at the end of the list of commands, so that they are executed in sequence, and there
	/// are not any long running non-database related commands executing.  Having long running 
	/// non-database related commands being executed between database commands will cause a delay
	/// in committing transactions, and could cause database deadlocks and problems.
	/// </para>
	/// <para>
	/// The ServerCommandProcessor also supports executing commands that implement the 
	/// <see cref="IAggregateCommand"/> interface.  It is assumed that these commands 
	/// are an aggregate of several sub-commands.  When a <see cref="CommandProcessor.Rollback"/> occurs
	/// for an <see cref="IAggregateCommand"/> command, the base command is first
	/// rolled back, then the sub-commands for the <see cref="IAggregateCommand"/>
	/// are rolled back.  Note that classes implementing <see cref="IAggregateCommand"/>
	/// should use the <see cref="CommandProcessor.ExecuteSubCommand"/> method to execute sub-commands.
	/// </para>
	/// </remarks>
    public class ServerCommandProcessor : CommandProcessor
	{
		#region Private Members

        private readonly bool _ownsExecContext;
        private readonly string _contextId;

        #endregion

		#region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ServerCommandProcessor"/> using
        /// the current <see cref="ServerExecutionContext"/> for the thread.
        /// </summary>
        /// <param name="contextId"></param>
        /// <param name="description"></param>
        public ServerCommandProcessor(string contextId, string description) : base(description, new ServerExecutionContext(contextId))
        {
            Platform.CheckForEmptyString(contextId, "contextId");
            _contextId = contextId;
            _ownsExecContext = ServerExecutionContext.Current == null;
        }

        /// <summary>
        /// Creates an instance of <see cref="ServerCommandProcessor"/> using
        /// the current <see cref="ServerExecutionContext"/> for the thread.
        /// </summary>
        /// <param name="description"></param>
        public ServerCommandProcessor(string description)
            :this(Guid.NewGuid().ToString(), description)
        { }
		
	    #endregion

		/// <summary>
		/// Gets or sets the reference key of the server partition of the primary study being used in the process
		/// </summary>
		/// <remarks>
		/// Because command processor can be part of a bigger transaction, the information is stored in /retrieved from the <seealso cref="ServerExecutionContext"/>
		/// and can be accessed from any where using <seealso cref="ServerExecutionContext.Current"/>
		/// </remarks>
		public ServerEntityKey PrimaryServerPartitionKey
	    {
			get { return (ProcessorContext as ServerExecutionContext).PrimaryServerPartitionKey; }
			set { (ProcessorContext as ServerExecutionContext).PrimaryServerPartitionKey = value; }
	    }

		/// <summary>
		/// Gets or sets the reference key of the primary study being used in the process
		/// </summary>
		public ServerEntityKey PrimaryStudyKey
	    {
			get { return (ProcessorContext as ServerExecutionContext).PrimaryStudyKey; }
			set
			{
				(ProcessorContext as ServerExecutionContext).PrimaryStudyKey = value;
			}
	    }

		#region IDisposable Members

		public override void Dispose()
		{
		    base.Dispose();

            if (_ownsExecContext && ProcessorContext!=null)
            {
                ProcessorContext.Dispose();
            }
		}

		#endregion

	}
}