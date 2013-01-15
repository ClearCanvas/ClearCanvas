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

using System.Collections;

namespace ClearCanvas.Common.Scripting
{
    /// <summary>
    /// Defines the interface to a script engine.  
    /// </summary>
    public interface IScriptEngine
    {
        /// <summary>
        /// Runs the specified script given the specified set of variables and their values.
        /// </summary>
        /// <remarks>
        /// The variables dictionary contains any number of named objects that the engine must make available to the script.
        /// It is left up to the implementation of the engine to decide how these objects are made available to the script.
        /// </remarks>
        /// <param name="script">The script to run.</param>
        /// <param name="variables">A set of named objects to which the script has access.</param>
        /// <returns>The return value of the script.</returns>
        object Run(string script, IDictionary variables);

        /// <summary>
        /// Asks the script engine to create an instance of a <see cref="IExecutableScript"/> based on the 
        /// specified string and variable names.
        /// </summary>
        /// <remarks>
        /// The variableNames array is an array of names of global variables whose values will be provided to the 
        /// <see cref="IExecutableScript.Run"/> method.  Use of this method may offer better performance than
        /// calling <see cref="IScriptEngine.Run"/> in the case where the same script is to be run multiple times,
        /// as the script engine may be able to pre-compile portions of the script.  However, this is entirely dependent
        /// upon the implementation of the script engine.
        /// </remarks>
        /// <param name="script">The script to create.</param>
        /// <param name="variableNames">The names of any global variables in the script that will be provided by the caller.</param>
        /// <returns>An executable script object that can be run multiple times.</returns>
        IExecutableScript CreateScript(string script, string[] variableNames);
    }
}
