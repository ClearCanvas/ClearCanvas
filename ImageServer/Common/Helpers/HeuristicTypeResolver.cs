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
using System.Reflection;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common.Helpers
{
    /// <summary>
    /// Helper class to resolve a type based on a name (either partial or Assembly fully-qualified). Unlike Type.GetType(), this class is more tolerant to assembly-version and sign-key changes.
    /// </summary>
    static public class HeuristicTypeResolver
    {
        private static readonly Dictionary<string, Type> _cache = new Dictionary<string, Type>();

        /// <summary>
        /// Returns a type that's best match for the specified typeName.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            Type type = null;
                
            lock (_cache)
            {
                if (_cache.ContainsKey(typeName))
                {
                    type = _cache[typeName];
                }
                else
                {
                    try
                    {
                        type = Type.GetType(typeName, false);
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Debug, ex);
                    }

                    if (type == null)
                    {
                        Platform.Log(LogLevel.Debug, "Could not find type {0}. Resolving type using heuristic method.", typeName);
                        try
                        {
                            type = Type.GetType(typeName, AssemblyResolver, TypeResolver);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Debug, ex);
                        }
                    }

                    if (type != null)
                    {
                        _cache.Add(typeName, type);
                    }
                }
                

                if (type != null)
                    Platform.Log(LogLevel.Debug, "Type {0} is resolved to {1}", typeName, type.AssemblyQualifiedName); 
                
                return type;
            }
            
        }

        private static Type TypeResolver(Assembly assembly, string simpleTypeName, bool ignoreCase)
        {
            // PER MSDN: The typeResolver method receives three arguments:
            //  The assembly to search or null if typeName does not contain an assembly name.
            //  The simple name of the type. In the case of a nested type, this is the outermost containing type. In the case of a generic type, this is the simple name of the generic type.
            //  A Boolean value that is true if the case of type names is to be ignored.
            if (assembly!=null)
                return assembly.GetType(simpleTypeName, false);
            else
            {
                return Type.GetType(simpleTypeName);
            }
        }

        private static Assembly AssemblyResolver(AssemblyName assemblyName)
        {
            // If the assemblyName refers to an assembly that is signed, the version and key must match. In this case, we resolve the assembly by matching on the name only
            Assembly assem = null;
            try
            {
                assem = Assembly.Load(assemblyName);
            }
            catch(Exception)
            {
                assem = Assembly.Load(assemblyName.Name);
            }

            
            return assem;
        }
    }

}