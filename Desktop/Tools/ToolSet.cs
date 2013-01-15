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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Default implementation of <see cref="IToolSet"/>.
    /// </summary>
    public class ToolSet : IToolSet
    {
        private readonly List<ITool> _tools;

        /// <summary>
        /// This contructs a tool set containing the specified tools.  The <see cref="IToolContext"/>
        /// is set on each tool and each tool's Initialize method is called.
        /// </summary>
        /// <param name="context">The tool context to pass to each tool.</param>
        /// <param name="tools">A set of tools to group in this ToolSet and be initialized and 
        /// set with the same tool context.</param>
        public ToolSet(IEnumerable tools, IToolContext context)
        {
            _tools = new List<ITool>();

            foreach (ITool tool in tools)
            {
                try
                {
                    tool.SetContext(context);
                    tool.Initialize();
                    _tools.Add(tool);
                }
                catch (Exception e)
                {
                    // a tool failed to initialize - log and continue
                    // (this tool will not be included in the set)
                    Platform.Log(LogLevel.Error, e);
                }
            }
        }

        /// <summary>
        /// Constructs a toolset based on the specified extension point and context.
        /// </summary>
        /// <remarks>
		/// The toolset will attempt to instantiate and initialize all 
		/// extensions of the specified tool extension point.
		/// </remarks>
        /// <param name="toolExtensionPoint">The tool extension point that provides the tools.</param>
        /// <param name="context">The tool context to pass to each tool.</param>
        public ToolSet(IExtensionPoint toolExtensionPoint, IToolContext context)
            :this(toolExtensionPoint, context, null)
        {
        }

        /// <summary>
        /// Constructs a toolset based on the specified extension point and context.
        /// </summary>
        /// <remarks>
        /// The toolset will attempt to instantiate and initialize all 
        /// extensions of the specified tool extension point that pass the 
        /// specified filter.
        /// </remarks>
        /// <param name="toolExtensionPoint">The tool extension point that provides the tools.</param>
        /// <param name="context">The tool context to pass to each tool.</param>
        /// <param name="filter">Only tools that match the specified extension filter are loaded into the 
        /// tool set.  If null, all tools extending the extension point are loaded.</param>
        public ToolSet(IExtensionPoint toolExtensionPoint, IToolContext context, ExtensionFilter filter)
            :this(toolExtensionPoint.CreateExtensions(filter), context)
        {
        }

        /// <summary>
        /// Disposes of all the <see cref="ITool"/>s in the tool set.
        /// </summary>
        /// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (ITool tool in _tools)
                {
                    try
                    {
                        tool.Dispose();
                    }
                    catch (Exception e)
                    {
                        // log and continue disposing of other tools
                        Platform.Log(LogLevel.Error, e);
                    }
                }
            }
        }


        #region IToolSet members

    	/// <summary>
    	/// Gets the tools contained in this tool set.
    	/// </summary>
    	public ITool[] Tools
        {
            get { return _tools.ToArray(); }
        }

    	/// <summary>
    	/// Finds the tool of the specified type.
    	/// </summary>
    	/// <typeparam name="TTool"></typeparam>
    	/// <returns>The instance of the tool of the specified type, or null if no such exists.</returns>
    	public TTool Find<TTool>() where TTool : ITool
    	{
    		return (TTool)_tools.FirstOrDefault(t => t is TTool);
    	}

    	/// <summary>
    	/// Returns the union of all actions defined by all tools in this tool set.
    	/// </summary>
    	public IActionSet Actions
        {
            get
            {
                var actionList = new List<IAction>();
                foreach (ITool tool in _tools)
                {
                    actionList.AddRange(tool.Actions);
                }
                return new ActionSet(actionList);
            }
        }

        #endregion

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
