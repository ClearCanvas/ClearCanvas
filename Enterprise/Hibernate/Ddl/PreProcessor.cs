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

using ClearCanvas.Common;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Defines an extension point for pre-processing the NHibernate configuration prior to generating DDL scripts.
	/// </summary>
	[ExtensionPoint]
	public class DdlPreProcessorExtensionPoint : ExtensionPoint<IDdlPreProcessor>
	{
	}

	/// <summary>
	/// Pre-processes the NHibernate configuration prior to generating output.
	/// </summary>
	public class PreProcessor
	{
		private readonly bool _createIndexes;
		private readonly bool _autoIndexForeignKeys;


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="createIndexes"></param>
		/// <param name="autoIndexForeignKeys"></param>
		public PreProcessor(bool createIndexes, bool autoIndexForeignKeys)
		{
			_createIndexes = createIndexes;
			_autoIndexForeignKeys = autoIndexForeignKeys;
		}

		/// <summary>
		/// Processes the specified persistent store.
		/// </summary>
		/// <param name="store"></param>
		public void Process(PersistentStore store)
		{
			Process(store.Configuration);
		}

		/// <summary>
		/// Processes the specified configuration.
		/// </summary>
		/// <param name="config"></param>
		public void Process(Configuration config)
		{
			// order in which processors are applied is important
			// because all FKs must be created prior to indexing

			// run the enum FK processor first
			Apply(config, new EnumForeignKeyProcessor());

			if (_createIndexes)
			{
				if (_autoIndexForeignKeys)
				{
					// run the fk index creator
					Apply(config, new ForeignKeyIndexProcessor());
				}

				// run the additional index creator
				Apply(config, new AdditionalIndexProcessor());
			}

			// run extension processors
			foreach (IDdlPreProcessor processor in new DdlPreProcessorExtensionPoint().CreateExtensions())
			{
				Apply(config, processor);
			}
		}

		private static void Apply(Configuration config, IDdlPreProcessor processor)
		{
			// it does not make sense to apply a given processor to the same configuration object
			// more than once
			// therefore we need to track whether a configuration object has been processed by a given processor
			// hopefully NH doesn't mind having extra keys stuck in the Configuration.Properties dictionary

			var key = "cc_custom_processor:" + processor.GetType().FullName;
			if(config.Properties.ContainsKey(key))
				return;

			// apply the processor
			processor.Process(config);

			// record that this processor has already been applied
			config.Properties[key] = "applied";
		}
	}
}
