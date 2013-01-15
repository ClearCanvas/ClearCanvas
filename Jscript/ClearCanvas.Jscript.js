// License

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

// End License

import System;
import System.Collections;

import ClearCanvas.Common;
import ClearCanvas.Common.Scripting;

package ClearCanvas.Jscript
{
    /// This class provides an implementation of ClearCanvas.Common.Scripting.ScriptEngineExtensionPoint
    /// for JScript
    ///
    /// The values in the context IDictionary are accessible to the script as 
    /// both globals (actually locals in the context of the script function) and as properties of "this".
    /// For example, if the dictionary contains "name" => "JoJo", then from the script,
    ///
    ///     name => "JoJo"          (entries are accessible as "global" variables)
    ///     or
    ///     this.name => "JoJo"     (the entries are directly accessible as properties of this)
    ///
    /// The script must use the "return" keyword to return an object to the caller. This is because
    /// the Run(...) method packages the script into its own function before executing it.
	
	/// The ScriptEngineOptions attribute is used to mark this engine as being a singleton, and requiring
	/// thread synchronization.  This is because the JScript.NET runtime is not designed to be thread-safe,
	/// as noted here: http://msdn.microsoft.com/en-us/library/ye921ye4%28VS.71%29.aspx.
	/// The intended effect of setting these options is that all calls into the jscript runtime are externally
	/// synchronized - that is, multiple threads will never execute code concurrently that calls into the jscript runtime
    public 
    ExtensionOf(ScriptEngineExtensionPoint)
    LanguageSupport("jscript")
	ScriptEngineOptions(Singleton = true, SynchronizeAccess = true)
    class Engine implements IScriptEngine
    {
	    function Run(script: String, context: IDictionary)
	    {
			var variableNames = new String[context.Count];

			var i = 0;
 	        for(var entry in context)
	        {
	            variableNames[i++] = entry.Key;
	        }
	        var es = CreateScriptPrivate(script, variableNames);
	        return es.Run(context);
	    }
	    
        function CreateScript(script: String, variableNames: String[]) : IExecutableScript
        {
			return CreateScriptPrivate(script, variableNames);
        }
        
        private function CreateScriptPrivate(script, variableNames) : ExecutableScript
        {
			var header = "";
			
			for(var i=0; i < variableNames.length; i++)
			{
				header += "var " + variableNames[i] + " = this." + variableNames[i] + ";";
			}
 	        var ctx = {};
	        ctx.__scriptFunction__ = new Function(header + script);
	        return new ExecutableScript(ctx);
        }
    }
    
    internal class ExecutableScript implements IExecutableScript
    {
        private var _ctx;
        
        // constructor 
        function ExecutableScript(ctx)
        {
            _ctx = ctx;
        }
        
        function Run(context: IDictionary)
        {
 	        for(var entry in context)
	        {
	            _ctx[entry.Key] = entry.Value;
	        }
	        return _ctx.__scriptFunction__();
        }
    }
}