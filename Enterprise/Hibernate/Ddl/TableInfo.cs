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
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a table in a relational database model.
	/// </summary>
	[DataContract]
	public class TableInfo : ElementInfo
	{
		private string _name;
		private string _schema;
		private List<ColumnInfo> _columns;
		private ConstraintInfo _primaryKey;
		private List<IndexInfo> _indexes;
		private List<ForeignKeyInfo> _foreignKeys;
		private List<ConstraintInfo> _uniqueKeys;

		/// <summary>
		/// Constructor
		/// </summary>
		public TableInfo()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="schema"></param>
		/// <param name="columns"></param>
		/// <param name="primaryKey"></param>
		/// <param name="indexes"></param>
		/// <param name="foreignKeys"></param>
		/// <param name="uniqueKeys"></param>
		internal TableInfo(string name, string schema, List<ColumnInfo> columns, ConstraintInfo primaryKey, List<IndexInfo> indexes, List<ForeignKeyInfo> foreignKeys, List<ConstraintInfo> uniqueKeys)
		{
			Name = name;
			Schema = schema;
			Columns = columns;
			PrimaryKey = primaryKey;
			Indexes = indexes;
			ForeignKeys = foreignKeys;
			UniqueKeys = uniqueKeys;
		}

		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		[DataMember]
		public string Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		/// <summary>
		/// Gets the name of the schema to which the table belongs, if different from the default schema.
		/// </summary>
		[DataMember]
		public string Schema
		{
			get { return _schema; }
			private set { _schema = value; }
		}

		/// <summary>
		/// Gets the set of columns in the table.
		/// </summary>
		[DataMember]
		public List<ColumnInfo> Columns
		{
			get { return _columns; }
			private set { _columns = value; }
		}

		/// <summary>
		/// Gets the table's primary key.
		/// </summary>
		[DataMember]
		public ConstraintInfo PrimaryKey
		{
			get { return _primaryKey; }
			set { _primaryKey = value; }
		}

		/// <summary>
		/// Gets the set of indexes defined on columns in this table.
		/// </summary>
		[DataMember]
		public List<IndexInfo> Indexes
		{
			get { return _indexes; }
			private set { _indexes = value; }
		}

		/// <summary>
		/// Gets the set of foreign key relationships defined on columns in this table.
		/// </summary>
		[DataMember]
		public List<ForeignKeyInfo> ForeignKeys
		{
			get { return _foreignKeys; }
			private set { _foreignKeys = value; }
		}

		/// <summary>
		/// Gets the set of unique keys defined on columns in this table.
		/// </summary>
		[DataMember]
		public List<ConstraintInfo> UniqueKeys
		{
			get { return _uniqueKeys; }
			private set { _uniqueKeys = value; }
		}


		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
		public override string Identity
		{
			get { return Name; }
		}
	}
}
