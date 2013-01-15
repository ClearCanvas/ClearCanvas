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
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="ITool"/>.
    /// </summary>
    /// <remarks>
	/// Tool classes may inherit this class, but inheriting 
	/// from <see cref="Tool{TContextInterface}"/> is recommended.
	/// </remarks>
    public abstract class ToolBase : ITool
    {
        private IToolContext _context;
        private IActionSet _actions;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ToolBase()
		{
		}

        /// <summary>
        /// Provides an untyped reference to the context in which the tool is operating.
        /// </summary>
        /// <remarks>
		/// Attempting to access this property before <see cref="SetContext"/> 
		/// has been called (e.g in the constructor of this tool) will return null.
		/// </remarks>
        protected IToolContext ContextBase
        {
            get { return _context; }
        }

        #region ITool members

    	/// <summary>
    	/// Called by the framework to set the tool context.
    	/// </summary>
    	public void SetContext(IToolContext context)
        {
            _context = context;
        }

    	/// <summary>
    	/// Called by the framework to allow the tool to initialize itself.
    	/// </summary>
    	/// <remarks>
    	/// This method will be called after <see cref="SetContext"/> has been called, 
    	/// which guarantees that the tool will have access to its context when this method is called.
    	/// </remarks>
    	public virtual void Initialize()
        {
            // nothing to do
        }

    	/// <summary>
    	/// Gets the set of actions that act on this tool.
    	/// </summary>
    	/// <remarks>
    	/// <see cref="ITool.Actions"/> mentions that this property should not be considered dynamic.
    	/// This implementation assumes that the actions are <b>not</b> dynamic by lazily initializing
    	/// the actions and storing them.  If you wish to return actions dynamically, you must override
    	/// this property.
    	/// </remarks>
    	public virtual IActionSet Actions
        {
            get { return _actions ?? (_actions = new ActionSet(ActionAttributeProcessor.Process(this))); }
            protected set { _actions = value; }
        }

        #endregion

        /// <summary>
		/// Disposes of this object; override this method to do any necessary cleanup.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context = null;
			}
		}
		
		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                // shouldn't throw anything from inside Dispose()
                Platform.Log(LogLevel.Error, e);
            }
        }

        #endregion
    }
}
