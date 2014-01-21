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

using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.Command
{
	/// <summary>
	/// <see cref="CommandBase"/> derived class for implementing commands that interact with the database.
	/// </summary>
	public abstract class ServerDatabaseCommand : CommandBase
	{
		#region Private Members

	    #endregion

		#region Properties

        /// <summary>
        /// Update context to use by inherited commands for performing the update.
        /// </summary>
	    public IUpdateContext UpdateContext { get; set; }

	    #endregion

		#region Constructor

	    /// <summary>
	    /// Constructor for a ServerDatabaseCommand.
	    /// </summary>
	    /// <param name="description">A description of the command</param>
	    protected ServerDatabaseCommand(string description)
			: base(description, true)
		{
		}
		#endregion

		/// <summary>
		/// Execute the ServerDatabaseCommand with the specified <see cref="IUpdateContext"/>.
		/// </summary>
		protected abstract void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext);

		/// <summary>
		/// Execute the <see cref="CommandBase"/> 
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void OnExecute(CommandProcessor theProcessor)
		{
			if (UpdateContext != null)
			{
				OnExecute(theProcessor, UpdateContext);
				UpdateContext = null;
			}
			else
			{
				using (
					IUpdateContext updateContext =
						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					OnExecute(theProcessor, updateContext);
					updateContext.Commit();
				}
			}
		}

		/// <summary>
		/// Undo of database command, note that this is not called because the transaction is rolled back instead.
		/// </summary>
		protected override void OnUndo()
		{

		}
	}


    /// <summary>
    /// <see cref="CommandBase"/> derived class for implementing commands interact with the database
    /// and are called in a specific context.
    /// </summary>
    public abstract class ServerDatabaseCommand<TContext> : ServerDatabaseCommand
        where TContext : class
    {
        #region Private Fields
        private readonly TContext _context;
        #endregion

        /// <summary>
        /// Constructor for a ServerDatabaseCommand.
        /// </summary>
        /// <param name="description">A description of the command</param>
        /// <param name="context"></param>
        protected ServerDatabaseCommand(string description, TContext context)
            : base(description)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the context for this command.
        /// </summary>
        protected TContext Context
        {
            get { return _context; }
        }
    }

    /// <summary>
    /// <see cref="CommandBase"/> derived class for implementing commands interact with the database
    /// and are called in a specific context and required specific input.
    /// </summary>
    /// <remarks>
    /// <para>
    /// </para>
    /// </remarks>
    public abstract class ServerDatabaseCommand<TContext, TParameters> : ServerDatabaseCommand<TContext>
        where TContext : class
        where TParameters: class
    {
        #region Private Fields
        private readonly TParameters _parameters;
        #endregion

        /// <summary>
        /// Constructor for a ServerDatabaseCommand.
        /// </summary>
        /// <param name="description">A description of the command</param>
        /// <param name="context">The context for the command</param>
        /// <param name="parameters">The paramaters for the command</param>
        protected ServerDatabaseCommand(string description, TContext context, TParameters parameters)
            : base(description, context)
        {
            _parameters = parameters;
        }

        /// <summary>
        /// Gets the parameter object that was used to construct this command.
        /// </summary>
        protected TParameters Parameters
        {
            get { return _parameters; }
        }
    }
}