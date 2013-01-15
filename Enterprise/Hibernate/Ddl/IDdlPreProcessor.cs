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

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Defines an interface to a DDL pre-processor.
	/// </summary>
	/// <remarks>
	/// Pre-processors are executed prior to generating any DDL output, for the purpose of modifying the <see cref="Configuration"/>.
	/// </remarks>
    public interface IDdlPreProcessor
    {
		/// <summary>
		/// Processes the specified configuration.
		/// </summary>
		/// <remarks>
		/// The pre-processor may modify the configuration object.
		/// </remarks>
		/// <param name="config"></param>
        void Process(Configuration config);
    }
}
