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
	/// Abstract base implementation of <see cref="IDdlScriptGenerator"/>.
	/// </summary>
	public abstract class DdlScriptGenerator : IDdlScriptGenerator
	{
		#region IDdlScriptGenerator Members

		public abstract string[] GenerateCreateScripts(Configuration config);

		public abstract string[] GenerateUpgradeScripts(Configuration config, RelationalModelInfo baselineModel);

		public abstract string[] GenerateDropScripts(Configuration config);

		#endregion

		/// <summary>
		/// Gets the dialect object specified by the configuration.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static Dialect GetDialect(Configuration config)
		{
			return Dialect.GetDialect(config.Properties);
		}
	}
}
