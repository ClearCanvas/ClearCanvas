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

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// This class is used to execute and undo a series of <see cref="ICommand"/> instances.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The command pattern is used whenever there are interactions while processing
    /// DICOM files that require undoing, such as interacting with a database.  The 
    /// pattern allows undoing of the operations as files are being modified and
    /// data inserted into the database.  
    /// </para>
    /// <para>
    /// An implemetation specific <see cref="ICommandProcessorContext"/> is used to implement an 
    /// context specific database interactions.  The interface is passed each command before it is
    /// executed such that a database context can be passed to the command.  If a failure occurs
    /// when executing the commands, the context will be notifed to roll back.
    /// </para>
    /// <para>
    /// If database interaction is required, it is recommended to group <see cref="CommandBase"/>s
    /// together at the end of the list of commands, so that they are executed in sequence, and there
    /// are not any long running non-database related commands in between.  Having long running 
    /// non-database related commands being executed between database commands will cause a delay
    /// in committing transactions, and could cause database deadlocks and problems.
    /// </para>
    /// <para>
    /// The CommandProcessor also supports executing commands that implement the 
    /// <see cref="IAggregateCommand"/> interface.  It is assumed that these commands 
    /// are an aggregate of several sub-commands.  The aggregate commands can be created when
    /// the command is executing.  When a <see cref="Rollback"/> occurs for 
    /// an <see cref="IAggregateCommand"/> command, the base command is first
    /// rolled back, then the sub-commands for the <see cref="IAggregateCommand"/>
    /// are rolled back.  Note that classes implementing <see cref="IAggregateCommand"/>
    /// should use the <see cref="ExecuteSubCommand"/> method to execute sub-commands.
    /// </para>
    /// </remarks>
    public abstract class CommandProcessor: IDisposable        
    {
        #region Private Members

        private readonly Stack<ICommand> _stack = new Stack<ICommand>();
        private readonly Queue<ICommand> _queue = new Queue<ICommand>();
        private readonly List<ICommand> _list = new List<ICommand>();
        private bool _disposed;
        private bool _contextRolledback;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="description">Description of the CommandProcessor</param>
        /// <param name="context">Context for the CommandProcessor</param>
        protected CommandProcessor(string description, ICommandProcessorContext context)
        {
            Description = description;
            ProcessorContext = context;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Description for the processor.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Exception that caused the failure.
        /// </summary>
        public Exception FailureException { get; protected set; }

        /// <summary>
        /// Number of commands stored in the processor queue.
        /// </summary>
        public int CommandCount
        {
            get { return _queue.Count; }
        }

        /// <summary>
        /// Context for the <see cref="CommandProcessor"/>.
        /// </summary>
        public ICommandProcessorContext ProcessorContext { get; private set; }

        /// <summary>
        /// Reason for a failure, if it occurs.
        /// </summary>
        public string FailureReason { get; private set; }

        #endregion

        #region Public Methods
        /// <summary>
        /// Add a command to the processor.
        /// </summary>
        /// <param name="command">The command to add.</param>
        public void AddCommand(ICommand command)
        {
            _queue.Enqueue(command);
            _list.Add(command);
        }

        /// <summary>
        /// Execute the commands passed to the processor.
        /// </summary>
        /// <returns>false on failure, true on success</returns>
        public bool Execute()
        {
            while (_queue.Count > 0)
            {
                ICommand command = _queue.Dequeue();
                command.ProcessorContext = ProcessorContext;

                _stack.Push(command);
                try
                {
                    ProcessorContext.PreExecute(command);
                    command.Execute(this);
                }
                catch (Exception e)
                {
                    if (command.RequiresRollback)
                    {
                        if (!_contextRolledback)
                        {
                            ProcessorContext.Rollback();
                            _contextRolledback = true;
                        }

                        FailureReason = String.Format("{0}: {1}", e.GetType().Name, e.Message);
                        Platform.Log(LogLevel.Error, e, "Unexpected error when executing command: {0}", command.Description);
                        Rollback();
                        FailureException = e;
                        return false;
                    }

                    Platform.Log(LogLevel.Warn, e,
                                 "Unexpected exception on command {0} that doesn't require rollback", command.Description);
                    _stack.Pop(); // Pop it off the stack, since it failed.
                }
            }

            try
            {
                ProcessorContext.Commit();  
            }
            catch (Exception e)
            {
                if (!_contextRolledback)
                {
                    ProcessorContext.Rollback();
                    _contextRolledback = true;
                }

                FailureReason = String.Format("{0}: {1}", e.GetType().Name, e.Message);
                Platform.Log(LogLevel.Error, e, "Unexpected error when committing updates to context.  Rolling back.");
                Rollback();
                FailureException = e;
                return false;               
            }
            
            return true;
        }

        /// <summary>
        /// Execute a Sub-Command of the current command being executed.
        /// </summary>
        /// <param name="baseCommand"></param>
        /// <param name="subCommand"></param>
        /// <returns></returns>
        public bool ExecuteSubCommand(IAggregateCommand baseCommand, ICommand subCommand)
        {
            subCommand.ProcessorContext = ProcessorContext;
            baseCommand.AggregateCommands.Push(subCommand);

            try
            {
                ProcessorContext.PreExecute(subCommand);

                subCommand.Execute(this);
            }
            catch (Exception e)
            {
                if (subCommand.RequiresRollback)
                {
                    if (!_contextRolledback)
                    {
                        ProcessorContext.Rollback();
                        _contextRolledback = true;
                    }

                    // Real rollback happens when the Execute fails
                    FailureReason = String.Format("{0}: {1}", e.GetType().Name, e.Message);
                    Platform.Log(LogLevel.Error, e, "Unexpected error when executing command: {0}", subCommand.Description);
                    FailureException = e;
                    return false;
                }

                Platform.Log(LogLevel.Warn, e,
                             "Unexpected exception on command {0} that doesn't require rollback", subCommand.Description);
                baseCommand.AggregateCommands.Pop(); // Pop it off the stack, since it failed.
            }
            return true;
        }

        /// <summary>
        /// Rollback the commands that have been executed already.
        /// </summary>
        public void Rollback()
        {
            if (!_contextRolledback)
            {
                ProcessorContext.Rollback();
                _contextRolledback = true;
            }

            while (_stack.Count > 0)
            {
                ICommand command = _stack.Pop();

                try
                {
                    command.Undo();
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception rolling back command {0}", command.Description);
                }

                var aggregateCommand = command as IAggregateCommand;
                if (aggregateCommand != null)
                {
                    RollbackAggregateCommand(aggregateCommand);
                }
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public virtual void Dispose()
        {
            if (_disposed)
                throw new InvalidOperationException("Already disposed.");

            _disposed = true;

            DisposeCommandList(_list);
            if (ProcessorContext != null)
            {
                ProcessorContext.Dispose();
                ProcessorContext = null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Static method for rolling back a command that implements <see cref="IAggregateCommand"/>
        /// </summary>
        /// <param name="command">The aggregate command to rollback sub-commands for.</param>
        protected static void RollbackAggregateCommand(IAggregateCommand command)
        {
            while (command.AggregateCommands.Count > 0)
            {
                ICommand subCommand = command.AggregateCommands.Pop();

                try
                {
                    subCommand.Undo();
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception rolling back sub command {0}", subCommand.Description);
                }

                var aggregateCommand = subCommand as IAggregateCommand;
                if (aggregateCommand != null)
                {
                    RollbackAggregateCommand(aggregateCommand);
                }
            }
        }

        /// <summary>
        /// Static method for disposing a list of commands.  Will recursively dispose <see cref="IAggregateCommand"/>s.
        /// </summary>
        /// <param name="commandList"></param>
        protected static void DisposeCommandList(IEnumerable<ICommand> commandList)
        {
            foreach (ICommand command in commandList)
            {
                if (command is IAggregateCommand)
                {
                    DisposeCommandList((command as IAggregateCommand).AggregateCommands);
                }
                if (command is IDisposable)
                {
                    try
                    {
                        (command as IDisposable).Dispose();
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Error, ex);
                    }
                }
            }
        }
        #endregion
    }
}