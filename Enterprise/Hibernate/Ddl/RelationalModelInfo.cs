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
using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;
using Iesi.Collections;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a relational database model.
	/// </summary>
    [DataContract]
    public class RelationalModelInfo : ElementInfo
    {
    	private List<TableInfo> _tables;
    	private List<EnumerationInfo> _enumerations;

		/// <summary>
		/// Constructor that creates an empty model.
		/// </summary>
        public RelationalModelInfo()
        {
			_tables = new List<TableInfo>();
			_enumerations = new List<EnumerationInfo>();
        }

		/// <summary>
		/// Constructor that creates a model from all NHibernate mappings and embedded enumeration information
		/// in the set of installed plugins.
		/// </summary>
		public RelationalModelInfo(PersistentStore store, RelationalSchemaOptions.NamespaceFilterOption namespaceFilter)
			: this(store.Configuration, namespaceFilter)
		{
		}

		/// <summary>
		/// Constructor that creates a model from all NHibernate mappings and embedded enumeration information
		/// in the set of installed plugins.
		/// </summary>
		/// <param name="config"></param>
		public RelationalModelInfo(Configuration config)
			:this(config, new RelationalSchemaOptions.NamespaceFilterOption())
		{
		}

		/// <summary>
		/// Constructor that creates a model from all NHibernate mappings and embedded enumeration information
		/// in the set of installed plugins.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="namespaceFilter"></param>
		public RelationalModelInfo(Configuration config, RelationalSchemaOptions.NamespaceFilterOption namespaceFilter)
		{
			_tables = CollectionUtils.Map(GetTables(config, namespaceFilter), (Table table) => BuildTableInfo(table, config));

			_enumerations = CollectionUtils.Select(
				new EnumMetadataReader().GetEnums(config),
				enumeration => namespaceFilter.Matches(enumeration.EnumerationClass));
		}

		/// <summary>
		/// Gets the set of tables.
		/// </summary>
    	[DataMember]
    	public List<TableInfo> Tables
    	{
			get { return _tables; }

			// for de-serialization
			private set { _tables = value; }
    	}

		/// <summary>
		/// Gets the set of enumerations.
		/// </summary>
		[DataMember]
		public List<EnumerationInfo> Enumerations
    	{
			get { return _enumerations; }
			private set { _enumerations = value; }
    	}

		/// <summary>
		/// Gets the table that matches the specified (unqualified) name, or null if no match.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public TableInfo GetTable(string table)
		{
			return CollectionUtils.SelectFirst(_tables, delegate(TableInfo t) { return t.Name == table; });
		}

		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
		public override string Identity
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

		#region Helpers

		/// <summary>
		/// Gets the set of NHibernate <see cref="Table"/> objects known to the specified configuration.
		/// </summary>
		/// <param name="cfg"></param>
		/// <param name="namespaceFilter"></param>
		/// <returns></returns>
		private static List<Table> GetTables(Configuration cfg, RelationalSchemaOptions.NamespaceFilterOption namespaceFilter)
		{
			// build set of all tables
			var tables = new HybridSet();
			var filteredClassMappings = CollectionUtils.Select(
				cfg.ClassMappings, 
				classMapping => namespaceFilter.Matches(classMapping.MappedClass.Namespace));
			foreach (var pc in filteredClassMappings)
			{
				foreach (var table in pc.TableClosureIterator)
				{
					tables.Add(table);
				}
			}

			var filteredCollectionMappings = CollectionUtils.Select(
				cfg.CollectionMappings, 
				collectionMapping => namespaceFilter.Matches(collectionMapping.Owner.MappedClass.Namespace));
			foreach (var collection in filteredCollectionMappings)
			{
				tables.Add(collection.CollectionTable);
			}

			return CollectionUtils.Sort(tables, (Table x, Table y) => x.Name.CompareTo(y.Name));
		}

		/// <summary>
		/// Converts an NHibernate <see cref="Table"/> object to a <see cref="TableInfo"/> object.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		private static TableInfo BuildTableInfo(Table table, Configuration config)
		{
			var dialect = Dialect.GetDialect(config.Properties);

			// map the set of additional unique constraints (not including individual unique columns)
			var uniqueKeys = CollectionUtils.Map(table.UniqueKeyIterator, (UniqueKey uk) => new ConstraintInfo(table, uk));

			// explicitly model any unique columns as unique constraints
			foreach (var col in table.ColumnIterator)
			{
				if(col.Unique)
				{
					uniqueKeys.Add(new ConstraintInfo(table, col));
				}
			}

			return new TableInfo(
				table.Name,
				table.Schema,
				CollectionUtils.Map(table.ColumnIterator, (Column column) => new ColumnInfo(column, config, dialect)),
				new ConstraintInfo(table, table.PrimaryKey),
				CollectionUtils.Map(table.IndexIterator, (Index index) => new IndexInfo(table, index)),
				CollectionUtils.Map(table.ForeignKeyIterator, (ForeignKey fk) => new ForeignKeyInfo(table, fk, config)),
				uniqueKeys
				);
		}

		#endregion
	}
}
