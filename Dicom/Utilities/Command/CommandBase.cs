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
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// Abstract class representing a command within the Command pattern.
    /// </summary>
    /// <remarks>
    /// <para>The Command pattern is used throughout the ImageServer and 
    /// Workstation when doing file and database operations to allow undoing 
    /// of the operations.  This abstract class is used as the interface for 
    /// the command.</para>
    /// </remarks>
    public abstract class CommandBase : ICommand
    {
        #region Private Members

        private EventHandler _executingEventHandlers;

        #endregion
        
        #region Constructor
        /// <summary>
        /// Constructor for a ServerCommand.
        /// </summary>
        /// <param name="description">A description of the command</param>
        /// <param name="requiresRollback">bool telling if the command requires a rollback of the operation if it fails</param>
        protected CommandBase(string description, bool requiresRollback)
        {
            Description = description;
            RequiresRollback = requiresRollback;
            Statistics = new CommandStatistics(this);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="CommandStatistics"/> of the command.
        /// </summary>
        [XmlIgnore]
        public CommandStatistics Statistics { get; private set; }

        /// <summary>
        /// Gets and sets a value describing what the command is doing.
        /// </summary>
        [XmlIgnore]
        public string Description { get; set; }

        /// <summary>
        /// Gets a value describing if the <see cref="CommandBase"/> requires a rollback of the operation its included in if it fails during execution.
        /// </summary>
        [XmlIgnore]
        public bool RequiresRollback { get; set; }

        /// <summary>
        /// Gets or sets the execution context for the command.
        /// </summary>
        /// <remarks>
        /// The execution context is managed by the owner.
        /// </remarks>
        [XmlIgnore]
        public ICommandProcessorContext ProcessorContext { get; set; }

        /// <summary>
        /// Simple flag that tells if a rollback / Undo has been requested for the <see cref="CommandBase"/>.
        /// </summary>
        [XmlIgnore]
        protected bool RollBackRequested { get; private set; }

        #endregion

        #region Events
        /// <summary>
        /// Occurs when <see cref="Execute"/> begins.
        /// </summary>
        public event EventHandler Executing
        {
            add { _executingEventHandlers += value; }
            remove { _executingEventHandlers -= value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Execute the <see cref="CommandBase"/>.
        /// </summary>
        /// <param name="theProcessor">The <see cref="CommandProcessor"/> executing the command.</param>
        public void Execute(CommandProcessor theProcessor)
        {
            try
            {
                Statistics.Start();
                EventsHelper.Fire(_executingEventHandlers, this, new EventArgs());
                OnExecute(theProcessor);
            }
            finally
            {
                Statistics.End();
            }

        }

        /// <summary>
        /// Undo the operation done by <see cref="Execute"/>.
        /// </summary>
        public void Undo()
        {
            RollBackRequested = true;
            OnUndo();
            Statistics.End();
        }
        #endregion

        #region Virtual methods

        protected abstract void OnExecute(CommandProcessor theProcessor);
        protected abstract void OnUndo();

        #endregion
    }

    /// <summary>
    /// Abstract class representing commands that tie to a specific context
    /// </summary>
    /// <remarks>
    /// <para> 
    /// </para>
    /// </remarks>
    public abstract class CommandBase<TContext> : CommandBase
        where TContext : class
    {
        #region Private Fields
        private readonly TContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="CommandBase"/>
        /// </summary>
        /// <param name="description"></param>
        /// <param name="requiresRollback"></param>
        /// <param name="context"></param>
        protected CommandBase(string description, bool requiresRollback, TContext context)
            : base(description, requiresRollback)
        {
            _context = context;
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets or sets the context of this command.
        /// </summary>
        protected TContext Context
        {
            get { return _context; }
        }
        #endregion
    }

    /// <summary>
    /// Abstract class representing commands that tie to a specific context
    /// and require specific type of input.
    /// </summary>
    /// <remarks>
    /// <para> 
    /// </para>
    /// </remarks>
    public abstract class CommandBase<TContext, TParameters> : CommandBase<TContext>
        where TContext : class
        where TParameters : class
    {
        #region Private Fields
        private readonly TParameters _parms;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="CommandBase"/>
        /// </summary>
        /// <param name="description"></param>
        /// <param name="requiresRollback"></param>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        protected CommandBase(string description, bool requiresRollback, TContext context, TParameters parameters)
            : base(description, requiresRollback, context)
        {
            _parms = parameters;
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets or sets the paramater object used for constructing this command.
        /// </summary>
        protected TParameters Parameters
        {
            get { return _parms; }
        }
        #endregion
    }
}