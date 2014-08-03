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
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredStartupInfo : MarshalByRefObject
    {
        public ShredStartupInfo(Uri assemblyPath, string shredName, string shredTypeName, ShredIsolationLevel isolationLevel)
        {
            Platform.CheckForNullReference(assemblyPath, "assemblyPath");
            Platform.CheckForEmptyString(shredName, "shredName");
            Platform.CheckForEmptyString(shredTypeName, "shredTypeName");

            _assemblyPath = assemblyPath;
            _shredName = shredName;
            _shredTypeName = shredTypeName;
        	_isolationLevel = isolationLevel;
        }

        #region Properties
		private readonly Uri _assemblyPath;
		private readonly string _shredName;
		private readonly string _shredTypeName;
    	private readonly ShredIsolationLevel _isolationLevel;

    	public string ShredTypeName
        {
            get { return _shredTypeName; }
        }
	

        public string ShredName
        {
            get { return _shredName; }
        }
	
        public Uri AssemblyPath
        {
            get { return _assemblyPath; }
        }

    	public ShredIsolationLevel IsolationLevel
    	{
			get { return _isolationLevel; }
    	}

    	#endregion
    }
}
