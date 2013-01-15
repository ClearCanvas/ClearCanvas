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
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate.Ddl;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class DdlWriterApplication : IApplicationRoot
	{
		public void RunApplication(string[] args)
		{
			var cmdLine = new DdlWriterCommandLine();
			try
			{
				cmdLine.Parse(args);

				// if a file name was supplied, write to the file
				if (!string.IsNullOrEmpty(cmdLine.OutputFile))
				{
					using (var sw = File.CreateText(cmdLine.OutputFile))
					{
						WriteOutput(sw, cmdLine);
					}
				}
				else
				{
					// by default write to stdout
					WriteOutput(Console.Out, cmdLine);
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
				cmdLine.PrintUsage(Console.Out);
			}
		}

		private static void WriteOutput(TextWriter writer, DdlWriterCommandLine cmdLine)
		{
			try
			{
				// load the persistent store defined by the current set of binaries
				var store = (PersistentStore)PersistentStoreRegistry.GetDefaultStore();

				// get config

				// run pre-processors
				var preProcessor = new PreProcessor(cmdLine.CreateIndexes, cmdLine.AutoIndexForeignKeys);
				preProcessor.Process(store);


				// if this is an upgrade, load the baseline model file
				RelationalModelInfo baselineModel = null;
				if (!string.IsNullOrEmpty(cmdLine.BaselineModelFile))
				{
					var serializer = new RelationalModelSerializer();
					baselineModel = serializer.ReadModel(File.OpenText(cmdLine.BaselineModelFile));
				}

				switch (cmdLine.Format)
				{
					case DdlWriterCommandLine.FormatOptions.sql:

						// create script writer and set properties based on command line 
						var scriptWriter = new ScriptWriter(store)
											{
												Options = new RelationalSchemaOptions
															{
																EnumOption = cmdLine.EnumOption,
																SuppressForeignKeys = !cmdLine.CreateForeignKeys,
																SuppressUniqueConstraints = !cmdLine.CreateUniqueKeys,
																NamespaceFilter = new RelationalSchemaOptions.NamespaceFilterOption(cmdLine.Namespace)
															},
												QualifyNames = cmdLine.QualifyNames,
												BaselineModel = baselineModel
											};

						// decide whether to write a creation or upgrade script, depending on if a baseline was supplied
						if (baselineModel == null)
							scriptWriter.WriteCreateScript(writer);
						else
							scriptWriter.WriteUpgradeScript(writer);

						break;

					case DdlWriterCommandLine.FormatOptions.xml:

						// we don't currently support outputting upgrades in XML format
						if (baselineModel != null)
							throw new NotSupportedException("Upgrade is not compatible with XML output format.");

						var serializer = new RelationalModelSerializer();
						var relationalModelInfo = new RelationalModelInfo(store, new RelationalSchemaOptions.NamespaceFilterOption(cmdLine.Namespace));
						serializer.WriteModel(relationalModelInfo, writer);
						break;

					default:
						throw new NotSupportedException(string.Format("{0} is not a valid output format.", cmdLine.Format));
				}
			}
			catch (Exception e)
			{
				Log(e, LogLevel.Error);
			}
		}

		private static void Log(object obj, LogLevel level)
		{
			Platform.Log(level, obj);
			Console.WriteLine(obj);
		}
	}
}
