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
using ClearCanvas.Common.Utilities;
using NHibernate.Dialect;
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration.Renderers;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	/// <summary>
	/// Base implementation of <see cref="IRenderer"/>.
	/// </summary>
	class Renderer : IRenderer
	{
		/// <summary>
		/// Gets the renderer implementation based on the dialect specified in the configuration.
		/// </summary>
		/// <returns></returns>
		public static IRenderer GetRenderer(Configuration config)
		{
			// TODO use config to choose renderer
			return new MsSqlRenderer(config);
		}

        private readonly Configuration _config;
		private readonly Dialect _dialect;
        private readonly string _defaultSchema;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="config"></param>
        protected Renderer(Configuration config)
		{
            _config = config;
			_dialect = Dialect.GetDialect(config.Properties);
            _defaultSchema = config.GetProperty(NHibernate.Cfg.Environment.DefaultSchema);

		}

		#region IRenderer Members

        public virtual IEnumerable<RelationalModelChange> PreFilter(IEnumerable<RelationalModelChange> changes)
        {
            // don't filter any changes
            return changes;
        }

		public virtual Statement[] Render(AddTableChange change)
		{
			TableInfo table = change.Table;

			StringBuilder sql = new StringBuilder("create table ")
				.Append(GetQualifiedName(table))
				.Append(" (");

			string columns = StringUtilities.Combine(
				CollectionUtils.Map<ColumnInfo, string>(table.Columns,
					delegate(ColumnInfo col)
					{
						return GetColumnDefinitionString(col);
					}), ", ");

			sql.Append(columns);

			if (table.PrimaryKey != null)
			{
				sql.Append(',').Append(GetPrimaryKeyString(table.PrimaryKey));
			}

			sql.Append(")");

			return new Statement[] { new Statement(sql.ToString()) };
		}

		public virtual Statement[] Render(DropTableChange change)
		{
			return new Statement[] { new Statement(_dialect.GetDropTableString(GetQualifiedName(change.Table))), };
		}

		public virtual Statement[] Render(AddColumnChange change)
		{
			string sql = string.Format("alter table {0} add {1}", GetQualifiedName(change.Table), GetColumnDefinitionString(change.Column));
			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(DropColumnChange change)
		{
			string sql = string.Format("alter table {0} drop column {1}", GetQualifiedName(change.Table), change.Column.Name);
			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(AddIndexChange change)
		{
			IndexInfo index = change.Index;
			StringBuilder buf = new StringBuilder("create index ")
				.Append(_dialect.QualifyIndexName ? index.Name : NHibernate.Util.StringHelper.Unqualify(index.Name))
				.Append(" on ")
				.Append(GetQualifiedName(change.Table))
				.Append(" (");

			buf.Append(StringUtilities.Combine(index.Columns, ", "));
			buf.Append(")");

			return new Statement[] { new Statement(buf.ToString()) };
		}

		public virtual Statement[] Render(DropIndexChange change)
		{
			IndexInfo index = change.Index;
			StringBuilder buf = new StringBuilder("drop index ")
				.Append(this.Dialect.QualifyIndexName ? index.Name : NHibernate.Util.StringHelper.Unqualify(index.Name))
				.Append(" on ")
				.Append(GetQualifiedName(change.Table));

			return new Statement[] { new Statement(buf.ToString()) };
		}

        public virtual Statement[] Render(AddPrimaryKeyChange change)
		{
			string sql = string.Format("alter table {0} add {1}",
				GetQualifiedName(change.Table),
				GetPrimaryKeyString(change.PrimaryKey));

			return new Statement[] { new Statement(sql) };
		}

        public virtual Statement[] Render(DropPrimaryKeyChange change)
		{
			string sql = string.Format("alter table {0} drop primary key", GetQualifiedName(change.Table));
			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(AddForeignKeyChange change)
		{
			ForeignKeyInfo fk = change.ForeignKey;
			string[] cols = fk.Columns.ToArray();
			string[] refcols = fk.ReferencedColumns.ToArray();

			string sql = string.Format("alter table {0} {1}",
				GetQualifiedName(change.Table),
				_dialect.GetAddForeignKeyConstraintString(fk.Name, cols, GetQualifiedName(change.Table.Schema, fk.ReferencedTable), refcols, true));

			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(DropForeignKeyChange change)
		{
			string sql = string.Format("alter table {0} {1}", GetQualifiedName(change.Table),
							  _dialect.GetDropForeignKeyConstraintString(change.ForeignKey.Name));

			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(AddUniqueConstraintChange change)
		{
			string sql = string.Format("alter table {0} add constraint {1} {2}",
				GetQualifiedName(change.Table),
				change.Constraint.Name,
				GetUniqueConstraintString(change.Constraint));

			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(DropUniqueConstraintChange change)
		{
			string sql = "alter table " + GetQualifiedName(change.Table) + _dialect.GetDropIndexConstraintString(change.Constraint.Name);
			return new Statement[] { new Statement(sql) };
		}

		public virtual Statement[] Render(ModifyColumnChange change)
		{
			string sql = string.Format("alter table {0} alter column {1}", GetQualifiedName(change.Table), GetColumnDefinitionString(change.Column));
			return new Statement[] { new Statement(sql) };
		}

        public virtual Statement[] Render(AddEnumValueChange change)
		{
			EnumerationMemberInfo e = change.Value;
			string sql = string.Format("insert into {0} (Code_, Value_, Description_, DisplayOrder_, Deactivated_) values ({1}, {2}, {3}, {4}, {5})",
				GetQualifiedName(change.Table),
				FormatValue(e.Code),
				FormatValue(e.Value),
				FormatValue(e.Description),
				e.DisplayOrder,
				FormatValue(false.ToString()));
			return new Statement[] { new Statement(sql) };
		}

        public virtual Statement[] Render(DropEnumValueChange change)
		{
			string sql = string.Format("delete from {0} where Code_ = {1}",
				GetQualifiedName(change.Table),
				FormatValue(change.Value.Code));
			return new Statement[] { new Statement(sql) };
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Gets the configuration.
		/// </summary>
        protected Configuration Config
        {
            get { return _config; }
        }

		/// <summary>
		/// Gets the default schema defined in the configuration.
		/// </summary>
        protected string DefaultSchema
        {
            get { return _defaultSchema; }
        }

		/// <summary>
		/// Gets the dialect specified in the configuration.
		/// </summary>
		protected Dialect Dialect
		{
			get { return _dialect; }
		}

		/// <summary>
		/// Formats the specified string for SQL.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
        protected static string FormatValue(string str)
		{
			// todo: can we use dialect here?
			if (str == null)
				return "NULL";

			// make sure to escape ' to ''
			return string.Format("'{0}'", str.Replace("'", "''"));
		}

		/// <summary>
		/// Gets the schema qualified name of the Table.
		/// </summary>
		protected string GetQualifiedName(TableInfo table)
		{
			return GetQualifiedName(table.Schema, table.Name);
		}

		/// <summary>
		/// Gets the schema qualified name of the table.
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		protected string GetQualifiedName(string schema, string table)
		{
            string qualifier = schema ?? _defaultSchema;
            return qualifier == null ? table : qualifier + "." + table;
		}

		/// <summary>
		/// Gets the primary key definition string.
		/// </summary>
		/// <param name="pk"></param>
		/// <returns></returns>
		protected string GetPrimaryKeyString(ConstraintInfo pk)
		{
			return string.Format(" primary key ({0})", StringUtilities.Combine(pk.Columns, ", "));
		}

		/// <summary>
		/// Gets the unique constraint definition string.
		/// </summary>
		/// <param name="uk"></param>
		/// <returns></returns>
		protected string GetUniqueConstraintString(ConstraintInfo uk)
		{
			return string.Format(" unique ({0})", StringUtilities.Combine(uk.Columns, ", "));
		}

		/// <summary>
		/// Gets the column definition string.
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		protected string GetColumnDefinitionString(ColumnInfo col)
		{
			StringBuilder colStr = new StringBuilder();
			colStr.Append(col.Name).Append(' ').Append(col.SqlType);

			if (col.Nullable)
			{
				colStr.Append(_dialect.NullColumnString);
			}
			else
			{
				colStr.Append(" not null");

				switch(col.SqlType)
				{
					case "REAL": // single
					case "SMALLINT": // Int16
					case "INT": // integer or Int32
					case "BIGINT": // Int64
						colStr.Append(" default 0");
						break;
					case "BIT": // boolean
						colStr.Append(" default 0");
						break;
					default:
						// do nothing.  For other SqlType, whoever running the upgrade script will have to manually fill in the default values
						break; 
				}
			}

			return colStr.ToString();
		}

		#endregion
	}
}
