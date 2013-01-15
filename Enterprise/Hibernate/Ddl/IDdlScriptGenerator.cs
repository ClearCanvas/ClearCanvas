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
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Defines an interface for generating DDL scripts to create, upgrade or drop a relational database.
    /// </summary>
    public interface IDdlScriptGenerator
    {
        /// <summary>
        /// Returns a set of scripts that will be executed as part of creating the database.  The scripts
        /// will be executed in the order they are returned.
        /// </summary>
        /// <param name="config">The persistent store (database) that DDL should be generated for</param>
        /// <returns>A set of scripts</returns>
        string[] GenerateCreateScripts(Configuration config);

		/// <summary>
		/// Returns a set of scripts that will be executed to upgrade the database from a previous version.  The scripts
		/// will be executed in the order they are returned.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="baselineModel"></param>
		/// <returns></returns>
    	string[] GenerateUpgradeScripts(Configuration config, RelationalModelInfo baselineModel);

        /// <summary>
        /// Returns a set of scripts that will be executed as part of dropping the database.  The scripts
        /// will be executed in the order they are returned.
        /// </summary>
        /// <param name="config">The persistent store (database) that DDL should be generated for</param>
        /// <returns>A set of scripts</returns>
        string[] GenerateDropScripts(Configuration config);
    }
}
