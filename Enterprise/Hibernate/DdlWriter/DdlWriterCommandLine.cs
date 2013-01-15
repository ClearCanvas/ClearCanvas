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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Ddl;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
	class DdlWriterCommandLine : CommandLine
	{
		public enum FormatOptions
		{
			sql,
			xml
		}

		public DdlWriterCommandLine()
		{
			QualifyNames = true;
			CreateUniqueKeys = true;
			CreateForeignKeys = true;
			CreateIndexes = true;
			AutoIndexForeignKeys = true;
			EnumOption = EnumOptions.All;
			Format = FormatOptions.sql;
		}

		[CommandLineParameter("fki", "Specifies whether to auto-index all foreign keys.  Ignored unless /ix is also specified.  Default is true.")]
		public bool AutoIndexForeignKeys { get; set; }

		[CommandLineParameter("index", "ix", "Specifies whether to generate database indexes. Default is true.")]
		public bool CreateIndexes { get; set; }

		[CommandLineParameter("fk", "Specifies whether to generate foreign keys. Default is true.")]
		public bool CreateForeignKeys { get; set; }

		[CommandLineParameter("uk", "Specifies whether to generate unique keys. Default is true.")]
		public bool CreateUniqueKeys { get; set; }

		[CommandLineParameter("q", "Specifies whether to qualify names of database objects. Default is true.")]
		public bool QualifyNames { get; set; }

		[CommandLineParameter("enums", "e", "Specifies whether to populate enumerations.  Possible values are 'all', 'hard' or 'none'.  If omitted, the default is 'all'")]
		public EnumOptions EnumOption { get; set; }

		[CommandLineParameter("out", "Specifies the name of the ouput file.  If omitted, output is written to stdout.")]
		public string OutputFile { get; set; }

		[CommandLineParameter("format", "f", "Specifies output format.  Possible values are 'sql' and 'xml'.  If omitted, the default is 'sql'")]
		public FormatOptions Format { get; set; }

		[CommandLineParameter("baseline", "b", "Specifies the name of a file that contains the model to upgrade from, in xml format.")]
		public string BaselineModelFile { get; set; }

		[CommandLineParameter("namespace", "ns", "Specifies the namespace of classes to include in the schema generation.  If omitted, all namespaces are included.  When using in combination with 'baseline' option, ensure that the same namespace option is used to generate both the baseline model and the current model.  Failing to do so will result in tables being dropped.")]
		public string Namespace { get; set; }
	}
}
