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
using System.Runtime.Serialization;
using System.Xml;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using System.IO;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Serializes/de-serializes a <see cref="RelationalModelInfo"/> object to XML.
	/// </summary>
	public class RelationalModelSerializer
	{
		/// <summary>
		/// Writes the specified model.
		/// </summary>
		public void WriteModel(RelationalModelInfo model, TextWriter tw)
		{
			using (var writer = new XmlTextWriter(tw))
			{
				writer.Formatting = Formatting.Indented;
				Write(writer, model);
			}
 		}

		/// <summary>
		/// Reads a model from the specified reader.
		/// </summary>
		/// <param name="tr"></param>
		/// <returns></returns>
		public RelationalModelInfo ReadModel(TextReader tr)
		{
			using (var reader = new XmlTextReader(tr))
			{
				reader.WhitespaceHandling = WhitespaceHandling.None;
				var model = (RelationalModelInfo) Read(reader, typeof(RelationalModelInfo));

				// bug #5300: need to convert any unique flags to explicit unique constraints
				MakeUniqueConstraintsExplicit(model);
				return model;
			}
		}

		/// <summary>
		/// Writes the specified data to the specified xml writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="data"></param>
		private static void Write(XmlWriter writer, object data)
		{
			// bug #5300: do not write out the "Unique" flag anymore
			var options = new JsmlSerializer.SerializeOptions { DataMemberTest = (m => AttributeUtils.HasAttribute<DataMemberAttribute>(m) && m.Name != "Unique") };
			JsmlSerializer.Serialize(writer, data, data.GetType().Name, options);
		}

		/// <summary>
		/// Reads an object of the specified class from the xml reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="dataContractClass"></param>
		/// <returns></returns>
		private static object Read(XmlReader reader, Type dataContractClass)
		{
			return JsmlSerializer.Deserialize(reader, dataContractClass);
		}

		/// <summary>
		/// Adds an explicit unique constraint for each column that is marked as unique.
		/// </summary>
		/// <remarks>
		/// This is to support backwards compatability with prior versions, where
		/// the Unique flag was set to indicate that a column had a unique constraint.
		/// </remarks>
		/// <param name="model"></param>
		private static void MakeUniqueConstraintsExplicit(RelationalModelInfo model)
		{
			foreach (var table in model.Tables)
			{
				// explicitly model any unique columns as unique constraints
				foreach (var col in table.Columns)
				{
					if (col.Unique)
					{
						table.UniqueKeys.Add(new ConstraintInfo(table, col));
					}
				}
			}
		}

	}
}
