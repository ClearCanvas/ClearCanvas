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

using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Defines an extension point for contributing DDL script generators to the database definition.
	/// Note that extensions of this point must not have dependencies on the existence of other database objects,
	/// as the order in which each extension is processed is not deterministic.
	/// </summary>
	[ExtensionPoint]
	public class DdlScriptGeneratorExtensionPoint : ExtensionPoint<IDdlScriptGenerator>
	{
	}

	/// <summary>
	/// Utility class that generates a database creation, upgrade or drop script.
	/// </summary>
	public class ScriptWriter
	{
		private readonly Configuration _config;
		private readonly string _qualifier;
		private bool _qualifyNames;
		private RelationalSchemaOptions _options;
		private RelationalModelInfo _baselineModel;

		/// <summary>
		/// Constructs a script writer based on the specified configuration.
		/// </summary>
		public ScriptWriter(PersistentStore store)
		{
			_config = store.Configuration;

			_qualifier = _config.GetProperty(NHibernate.Cfg.Environment.DefaultSchema);
			if (!string.IsNullOrEmpty(_qualifier))
				_qualifier += ".";
		}

		/// <summary>
		/// Gets or sets a value indicating whether names will be qualified in the output script.
		/// </summary>
		public bool QualifyNames
		{
			get { return _qualifyNames; }
			set { _qualifyNames = value; }
		}

		/// <summary>
		/// Gets or sets options that control what is included in script generation.
		/// </summary>
		public RelationalSchemaOptions Options
		{
			get { return _options; }
			set { _options = value; }
		}

		/// <summary>
		/// Gets or sets the baseline model to upgrade from.
		/// </summary>
		/// <remarks>
		/// The default value is null, in which case scripts are generated to create the entire database from scratch.
		/// If this property is set, scripts will be generated to upgrade from this model to the current model.
		/// </remarks>
		public RelationalModelInfo BaselineModel
		{
			get { return _baselineModel; }
			set { _baselineModel = value; }
		}


		/// <summary>
		/// Writes a database creation script to the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="sw"></param>
		public void WriteCreateScript(TextWriter sw)
		{
			foreach (var gen in GetGenerators())
			{
				foreach (var script in gen.GenerateCreateScripts(_config))
				{
					sw.WriteLine(RewriteQualifiers(script));
				}
			}
		}

		/// <summary>
		/// Writes a database upgrade script to the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="sw"></param>
		public void WriteUpgradeScript(TextWriter sw)
		{
			foreach (var gen in GetGenerators())
			{
				foreach (var script in gen.GenerateUpgradeScripts(_config, _baselineModel))
				{
					sw.WriteLine(RewriteQualifiers(script));
				}
			}
		}

		/// <summary>
		/// Writes a database drop script to the specified <see cref="StreamWriter"/>
		/// </summary>
		/// <param name="sw"></param>
		public void WriteDropScript(StreamWriter sw)
		{
			foreach (var gen in GetGenerators())
			{
				foreach (var script in gen.GenerateDropScripts(_config))
				{
					sw.WriteLine(RewriteQualifiers(script));
				}
			}
		}

		private List<IDdlScriptGenerator> GetGenerators()
		{
			var generators = new List<IDdlScriptGenerator>();

			// the order of generator execution is important, so add the static generators first
			generators.Add(new RelationalSchemaGenerator(_options));

			// subsequently we can add extension generators, with uncontrolled ordering
			foreach (IDdlScriptGenerator generator in (new DdlScriptGeneratorExtensionPoint().CreateExtensions()))
			{
				if (_options.NamespaceFilter.Matches(generator.GetType().Namespace))
				{
					generators.Add(generator);
				}
			}
			return generators;
		}

		private string RewriteQualifiers(string script)
		{
			return _qualifyNames ? script : script.Replace(_qualifier, "");
		}
	}
}
