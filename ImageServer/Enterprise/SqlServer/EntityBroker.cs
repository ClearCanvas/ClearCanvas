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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    /// <summary>
    /// Provides base implementation of <see cref="IEntityBroker{TServerEntity,TSearchCriteria,TUpdateColumns}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides a base implementation for doing a number of SQL related operations against
    /// a <see cref="ServerEntity"/> derived class.  The class implements the interface using
    /// SQL Server 2005 and ADO.NET.  
    /// </para>
    /// <para>
    /// It can generate dynamic SQL queries, updates, and inserts.
    /// It also allows for querying against a <see cref="ServerEntity"/> using a 
    /// <see cref="EntitySelectCriteria"/> derived class.
    /// </para>
    /// </remarks>
    /// <typeparam name="TServerEntity">The ServerEntity derived class to work against.</typeparam>
    /// <typeparam name="TSelectCriteria">The appropriate criteria for selecting against the entity.</typeparam>
    /// <typeparam name="TUpdateColumns">The columns for doing insert or updates.</typeparam>
    public abstract class EntityBroker<TServerEntity, TSelectCriteria, TUpdateColumns> : Broker,
                                                                                         IEntityBroker
                                                                                             <TServerEntity,
                                                                                             TSelectCriteria,
                                                                                             TUpdateColumns>
        where TServerEntity : ServerEntity, new()
        where TSelectCriteria : EntitySelectCriteria
        where TUpdateColumns : EntityUpdateColumns
    {
        #region Private Members

        private readonly String _entityName;

        #endregion

        #region Constructors

        protected EntityBroker(String entityName)
        {
            _entityName = entityName;
        }

        #endregion

        #region Private Static Members

        /// <summary>
        /// Gets ORDER BY clauses based on the input search condition.
        /// </summary>
        /// <returns>A string containing the ORDER BY clause.</returns>
        private static string GetSelectOrderBy(String entity, EntitySelectCriteria criteria)
        {
            var sb = new StringBuilder();

            // recurse on subCriteria
            foreach (SearchCriteria subCriteria in criteria.EnumerateSubCriteria())
            {
                string variable = string.Format("[{0}].{1}", entity, subCriteria.GetKey());
                var sc = subCriteria as SearchConditionBase;
                if (sc != null)
                {
                    if (sc.SortPosition != -1)
                    {
                        // With the Server, all primary keys end with "Key".  The database implementation itself
                        // names these columns with the name GUID instead of Key.
                        string sqlColumnName = variable.EndsWith("Key") 
                                                   ? variable.Replace("Key", "GUID") 
                                                   : variable;

                        if (sb.ToString().Length == 0)
                            sb.AppendFormat("ORDER BY {0} {1}", sqlColumnName, sc.SortDirection ? "ASC" : "DESC");
                        else
                            sb.AppendFormat(", {0} {1}", sqlColumnName, sc.SortDirection ? "ASC" : "DESC");
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets WHERE clauses based on the input search condition for a specific column.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="sc"></param>
        /// <param name="command">The SQL Command.</param>
        /// <returns>A string containing the WHERE clause for the column.</returns>
        private static string GetSelectWhereText(string variable, SearchConditionBase sc, SqlCommand command)
        {
            var sb = new StringBuilder();
            object[] values;             

            // With the Server, all primary keys end with "Key".  The database implementation itself
            // names these columns with the name GUID instead of Key.
            string sqlColumnName = variable.EndsWith("Key") 
                                       ? variable.Replace("Key", "GUID") : variable;

            // We use input parameters to the select statement.  We create a variable name for the 
            // input parameter based on the column name input.  Variable names can't have periods,
            // so we have to remove the "."
            int j = sqlColumnName.IndexOf(".");
            string sqlParmName = j != -1 
                                     ? sqlColumnName.Remove(j, 1) : sqlColumnName;
            if (sqlParmName.Contains("[")) sqlParmName = sqlParmName.Remove(sqlParmName.IndexOf("["), 1);
            if (sqlParmName.Contains("]")) sqlParmName = sqlParmName.Remove(sqlParmName.IndexOf("]"), 1);

            // Now go through the actual input parameters.  Replace references to ServerEntityKey with
            // the GUID itself for these parameters, and replace ServerEnum derived references with the 
            // value of the enum in the array so the input parameters work properly.

            if (sc.Values!=null && sc.Values.Length>0)
            {
                values = new object[sc.Values.Length];
                for (int i = 0; i < sc.Values.Length; i++)
                {
                    var key = sc.Values[i] as ServerEntityKey;
                    if (key != null)
                        values[i] = key.Key;
                    else
                    {
                        var e = sc.Values[i] as ServerEnum;
                        values[i] = e != null ? e.Enum : sc.Values[i];
                    }
                }
            }
            else
            {
                values = new object[1];
                values[0] = DBNull.Value;
            }
            

            // Generate the actual WHERE clauses based on the type of condition.
            switch (sc.Test)
            {
                case SearchConditionTest.Equal:
                    var doc = values[0] as XmlDocument;
                    if (doc != null)
                    {
                        var node = doc.SelectSingleNode("/Select/XPath/@path");
                        string xpath = node == null ? string.Empty : node.Value;

                        node = doc.SelectSingleNode("/Select/Select/@value");
                        string val = node == null ? string.Empty : node.Value;

                        if (!string.IsNullOrEmpty(xpath) && !string.IsNullOrEmpty(val))
                        {
                            sb.AppendFormat("{0}.value('{1}','text') = @{2}", sqlColumnName, xpath, sqlParmName);
                            command.Parameters.AddWithValue("@" + sqlParmName, val);
                        }
                    }
                    else
                    {
                        sb.AppendFormat("{0} = @{1}", sqlColumnName, sqlParmName);
                        command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    }
                    break;
                case SearchConditionTest.NotEqual:
                    sb.AppendFormat("{0} <> @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.Like:
                    doc = values[0] as XmlDocument;
                    if (doc != null)
                    {
                        var node = doc.SelectSingleNode("/Select/XPath/@path");
                        string xpath = node == null ? string.Empty : node.Value;

                        node = doc.SelectSingleNode("/Select/XPath/@value");
                        string val = node == null ? string.Empty : node.Value;

                        if (!string.IsNullOrEmpty(xpath) && !string.IsNullOrEmpty(val))
                        {
                            sb.AppendFormat("{0}.value('{1}','text') like @{2}", sqlColumnName, xpath, sqlParmName);
                            command.Parameters.AddWithValue("@" + sqlParmName, val);
                        }
                    }
                    else
                    {
                        // +'' in this statement was a total hack job to fix a performance issue with SQL.  See this:
                        // http://stackoverflow.com/questions/3495355/sql-server-performance-of-parameterized-queries-with-leading-wildcards
                        sb.AppendFormat("{0}+'' like @{1}", sqlColumnName, sqlParmName);
                        command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    }
                    break;
                case SearchConditionTest.NotLike:
                    sb.AppendFormat("{0}+'' not like @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.Between:
                    sb.AppendFormat("{0} between @{1}1 and @{1}2", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName + "1", values[0]);
                    command.Parameters.AddWithValue("@" + sqlParmName + "2", values[1]);
                    break;
                case SearchConditionTest.In:
                    sb.AppendFormat("{0} in (", sqlColumnName); // assume at least one param
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (i == 0)
                            sb.AppendFormat("@{0}{1}", sqlParmName, i + 1);
                        else
                            sb.AppendFormat(", @{0}{1}", sqlParmName, i + 1);
                        command.Parameters.AddWithValue(string.Format("@{0}{1}", sqlParmName, i + 1), values[i]);
                    }
                    sb.Append(")");
                    break;
                case SearchConditionTest.NotIn:
                    sb.AppendFormat("{0} not in (", sqlColumnName); // assume at least one param
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (i == 0)
                            sb.AppendFormat("@{0}{1}", sqlParmName, i + 1);
                        else
                            sb.AppendFormat(", @{0}{1}", sqlParmName, i + 1);
                        command.Parameters.AddWithValue(string.Format("@{0}{1}", sqlParmName, i + 1), values[i]);
                    }
                    sb.Append(")");
                    break;
                case SearchConditionTest.LessThan:
                    sb.AppendFormat("{0} < @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.LessThanOrEqual:
                    sb.AppendFormat("{0} <= @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.MoreThan:
                    sb.AppendFormat("{0} > @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.MoreThanOrEqual:
                    sb.AppendFormat("{0} >= @{1}", sqlColumnName, sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.NotNull:
                    sb.AppendFormat("{0} is not null", sqlColumnName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.Null:
                    sb.AppendFormat("{0} is null", sqlColumnName);
                    command.Parameters.AddWithValue("@" + sqlParmName, values[0]);
                    break;
                case SearchConditionTest.NotExists:
                    {
                        var rec = sc as RelatedEntityCondition<EntitySelectCriteria>;
                        if (rec == null) throw new PersistenceException("Casting error with RelatedEntityCondition", null);
                        var notExistsSubCriteria = (EntitySelectCriteria)values[0];

                        string baseTableColumn = rec.BaseTableColumn;
                        if (baseTableColumn.EndsWith("Key"))
                            baseTableColumn = baseTableColumn.Replace("Key", "GUID");
                        string relatedTableColumn = rec.RelatedTableColumn;
                        if (relatedTableColumn.EndsWith("Key"))
                            relatedTableColumn = relatedTableColumn.Replace("Key", "GUID");

                        string sql;
                        if (String.Format("{0}GUID", notExistsSubCriteria.GetKey()).Equals(relatedTableColumn))
                            sql = GetSelectSql(notExistsSubCriteria.GetKey(), command, notExistsSubCriteria, null, null,
                                               String.Format("[{0}].{2} = [{1}].GUID", variable, notExistsSubCriteria.GetKey(), baseTableColumn));
                        else
                            sql = GetSelectSql(notExistsSubCriteria.GetKey(), command, notExistsSubCriteria, null, null,
                                               String.Format("[{0}].{2} = [{1}].{3}", variable, notExistsSubCriteria.GetKey(), baseTableColumn, relatedTableColumn));

                        sb.AppendFormat("NOT EXISTS ({0})", sql);
                        break;
                    }
                    
                case SearchConditionTest.Exists:
                    {
                        var rec = sc as RelatedEntityCondition<EntitySelectCriteria>;
                        if (rec == null) throw new PersistenceException("Casting error with RelatedEntityCondition", null);
                        var existsSubCriteria = (EntitySelectCriteria)values[0];

                        string baseTableColumn = rec.BaseTableColumn;
                        if (baseTableColumn.EndsWith("Key"))
                            baseTableColumn = baseTableColumn.Replace("Key", "GUID");
                        string relatedTableColumn = rec.RelatedTableColumn;
                        if (relatedTableColumn.EndsWith("Key"))
                            relatedTableColumn = relatedTableColumn.Replace("Key", "GUID");

                        string existsSql;
                        if (String.Format("{0}GUID",existsSubCriteria.GetKey()).Equals(relatedTableColumn))
                            existsSql = GetSelectSql(existsSubCriteria.GetKey(), command, existsSubCriteria, null, null,
                                                     String.Format("[{0}].{2} = [{1}].GUID", variable, existsSubCriteria.GetKey(),
                                                                   baseTableColumn));
                        else
                            existsSql = GetSelectSql(existsSubCriteria.GetKey(), command, existsSubCriteria, null, null,
                                                     String.Format("[{0}].{2} = [{1}].{3}", variable, existsSubCriteria.GetKey(),
                                                                   baseTableColumn, relatedTableColumn));
                        sb.AppendFormat("EXISTS ({0})", existsSql);
                        break;
                    }

                default:
                    throw new ApplicationException(); // invalid
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get an array of WHERE clauses for all of the search criteria specified.
        /// </summary>
        /// <param name="qualifier">The table name to use for each of where clauses.</param>
        /// <param name="criteria">The actual Search Criteria specified.</param>
        /// <param name="command">The SQL command.</param>
        /// <returns>An array of WHERE clauses.</returns>
        private static String[] GetWhereSearchCriteria(string qualifier, SearchCriteria criteria, SqlCommand command)
        {
            var list = new List<string>();

            if (criteria is SearchConditionBase)
            {
                var sc = (SearchConditionBase) criteria;
                if (sc.Test != SearchConditionTest.None)
                {
                    String text = GetSelectWhereText(qualifier, sc, command);
                    list.Add(text);
                }
            }
            else
            {
                // recurse on subCriteria
                foreach (SearchCriteria subCriteria in criteria.EnumerateSubCriteria())
                {
                    // Note:  this is a bit ugly, but we don't do the <Table>.<Column>
                    // syntax for Subselect type criteria.  Subselects only need 
                    // the table name, and there isn't a real column associated with it.
                    // We could pass the entity down all the way, but decided to do it
                    // this way instead.
                    string subQualifier;
                    if (subCriteria is RelatedEntityCondition<EntitySelectCriteria>)
                    {
                        subQualifier = qualifier;
                    }
                    else
                        subQualifier = string.Format("[{0}].{1}", qualifier, subCriteria.GetKey());

                    list.AddRange(GetWhereSearchCriteria(subQualifier, subCriteria, command));
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Proves a SQL statement based on the supplied input criteria.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <param name="criteria">The criteria for the select</param>
        /// <param name="startIndex">The start index from within the result set</param>
        /// <param name="maxRows">The max rows.</param>
        /// <param name="subWhere">If this is being used to generate the SQL for a sub-select, additional where clauses are included here for the select.  Otherwise the parameter is null.</param>
        /// <returns>The SQL string.</returns>
        private static string GetSelectSql(string entityName, SqlCommand command, EntitySelectCriteria criteria, int? startIndex, int? maxRows, String subWhere)
        {
            /*
			 * Here's the general select format for querying a subset of rows
			   SELECT WorkQueueDetails.*
				FROM
				(SELECT WorkQueue.*, 
					ROW_NUMBER() OVER(ORDER BY ScheduledTime ASC) as RowNum
					FROM WorkQueue
				) AS WorkQueueDetails
				WHERE RowNum BETWEEN @startRowIndex AND (@startRowIndex + @maximumRows) - 1
			 */
            string orderBy = GetSelectOrderBy(entityName, criteria);

            var sb = new StringBuilder();
            if (maxRows == null || startIndex == null)
                sb.AppendFormat("SELECT * FROM [{0}]", entityName);
                // had a bug at the tail end of the 1.5 release where Web GUI queries w/ paging did not work properly
                // using the TOP syntax for the selects was not giving the same results as the ROW_NUMBER() syntax.
                // Since this code is also used extensively when we do a FindOne() call, made it so in that case we
                // still do the TOP, but use ROW_NUMBER syntax for other situations, which would include the web gui.
            else if (startIndex.Value == 0 && maxRows.Value == 1)
                sb.AppendFormat("SELECT TOP {0} * FROM [{1}]", maxRows, entityName);
            else
            {
                if (orderBy.Length > 0)
                    sb.AppendFormat("SELECT [{0}].*,ROW_NUMBER() OVER({1}) as RowNum FROM [{0}]", entityName, orderBy);
                else
                {
                    // no OrderBy clause to sort the list, just assign row number to 1 to keep their order as is
                    sb.AppendFormat("SELECT [{0}].*,ROW_NUMBER() OVER( ORDER BY [{0}].GUID ) as RowNum FROM [{0}]", entityName);
                }
            }
            // Generate an array of the WHERE clauses to be used.
            String[] where = GetWhereSearchCriteria(entityName, criteria, command);

            // Add the where clauses on.
            bool first = true;
            if (subWhere != null)
            {
                first = false;
                sb.AppendFormat(" WHERE {0}", subWhere);
            }

            foreach (String clause in where)
            {
                if (first)
                {
                    first = false;
                    sb.AppendFormat(" WHERE {0}", clause);
                }
                else
                    sb.AppendFormat(" AND {0}", clause);
            }

            if (maxRows == null || startIndex == null)
            {
                if (orderBy.Length > 0)
                    sb.AppendFormat(" {0}", orderBy);
            }
            else if (startIndex.Value == 0 && maxRows.Value == 1)
            {
                if (orderBy.Length > 0)
                    sb.AppendFormat(" {0}", orderBy);
            }
            else
            {
                StringBuilder sb2 = new StringBuilder();
                sb2.AppendFormat("SELECT {0}Details.* FROM ({1}) AS {0}Details ", entityName, sb);
                if(startIndex.Value == 0)
                    sb2.AppendFormat("WHERE RowNum BETWEEN @StartRowIndex AND @MaximumRows");
                else 
                    sb2.AppendFormat("WHERE RowNum BETWEEN @StartRowIndex AND (@StartRowIndex + @MaximumRows) - 1");
                command.Parameters.AddWithValue("@StartRowIndex", startIndex.Value);
                command.Parameters.AddWithValue("@MaximumRows", maxRows.Value);
                return sb2.ToString();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Proves a SELECT COUNT(*) SQL statement based on the supplied input criteria.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <param name="criteria">The criteria for the select count</param>
        /// <param name="subWhere">If this is being used to generate the SQL for a sub-select, additional where clauses are included here for the select.  Otherwise the parameter is null.</param>
        /// <returns>The SQL string.</returns>
        private static string GetSelectCountSql(string entityName, SqlCommand command, SearchCriteria criteria,
                                                String subWhere)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(*) FROM [{0}]", entityName);

            // Generate an array of the WHERE clauses to be used.
            String[] where = GetWhereSearchCriteria(entityName, criteria, command);

            // Add the where clauses on.
            bool first = true;
            if (subWhere != null)
            {
                first = false;
                sb.AppendFormat(" WHERE {0}", subWhere);
            }

            foreach (String clause in where)
            {
                if (first)
                {
                    first = false;
                    sb.AppendFormat(" WHERE {0}", clause);
                }
                else
                    sb.AppendFormat(" AND {0}", clause);
            }
            return sb.ToString();
        }


        /// <summary>
        /// Proves a WHERE clause based on the supplied input criteria.
        /// </summary>
        /// <param name="entityName">The entity that is being deleted from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <param name="criteria">The criteria for the delete statement</param>
        /// <param name="subWhere">If this is being used to generate the SQL for a sub-where, additional where clauses are included here for the delete.  Otherwise the parameter is null.</param>
        /// <returns>The SQL string.</returns>
        private static string GetDeleteWhereClause(string entityName, SqlCommand command, SearchCriteria criteria, String subWhere)
        {
            var sb = new StringBuilder();

            // Generate an array of the WHERE clauses to be used.
            String[] where = GetWhereSearchCriteria(entityName, criteria, command);

            // Add the where clauses on.
            bool first = true;
            if (subWhere != null)
            {
                first = false;
                sb.AppendFormat(" {0}", subWhere);
            }

            foreach (String clause in where)
            {
                if (first)
                {
                    first = false;
                    sb.AppendFormat(" {0}", clause);
                }
                else
                    sb.AppendFormat(" AND {0}", clause);
            }

            return sb.ToString();
        }
        /// <summary>
        /// Resolves the Database column name for a field name
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        private static String GetDbColumnName(EntityColumnBase parm)
        {
            String sqlColumnName;

            if (parm is EntityUpdateColumn<ServerEntity>)
            {
                if (parm.FieldName.EndsWith("Key"))
                    sqlColumnName = String.Format("{0}", parm.FieldName.Replace("Key", "GUID"));

                else if (parm.FieldName.EndsWith("GUID"))
                    sqlColumnName = String.Format("{0}", parm.FieldName);

                else
                    sqlColumnName = String.Format("{0}", parm.FieldName);
            }
            else if (parm is EntityUpdateColumn<ServerEntityKey>)
            {
                if (parm.FieldName.EndsWith("Key"))
                    sqlColumnName = String.Format("{0}", parm.FieldName.Replace("Key", "GUID"));

                else if (parm.FieldName.EndsWith("GUID"))
                    sqlColumnName = String.Format("{0}", parm.FieldName);

                else
                    sqlColumnName = String.Format("{0}", parm.FieldName);
            }
            else if (parm is EntityUpdateColumn<ServerEnum>)
            {
                sqlColumnName = parm.FieldName.EndsWith("Enum") 
                    ? String.Format("{0}", parm.FieldName) 
                    : String.Format("{0}Enum", parm.FieldName);
            }
            else
            {
                sqlColumnName = String.Format("{0}", parm.FieldName);
            }

            return sqlColumnName;
        }

        private static string GetUpdateWhereClause(SqlCommand command, ServerEntityKey key)
        {
            command.Parameters.AddWithValue("@PrimaryKey", key.Key);
            return String.Format("[GUID]=@PrimaryKey");
        }

        private static String GetUpdateSetClause(SqlCommand command, EntityUpdateColumns parameters)
        {
            Dictionary<string, string> columnMap = GetColumnMap(parameters.GetType());

            var setClause = new StringBuilder();
            bool first = true;
            foreach (EntityColumnBase parm in parameters.SubParameters.Values)
            {
                //String sqlParmName = GetDbColumnName(parm);
                String sqlParmName = columnMap[parm.FieldName];
                if (parm is EntityUpdateColumn<XmlDocument>)
                {
                    var p = parm as EntityUpdateColumn<XmlDocument>;

                    XmlDocument xml = p.Value;
                    var sw = new StringWriter();
                    var xmlSettings = new XmlWriterSettings
                                          {
                                              Encoding = Encoding.UTF8,
                                              ConformanceLevel = ConformanceLevel.Fragment,
                                              Indent = false,
                                              NewLineOnAttributes = false,
                                              CheckCharacters = true,
                                              IndentChars = ""
                                          };

                    XmlWriter xmlWriter = XmlWriter.Create(sw, xmlSettings);
                    xml.WriteTo(xmlWriter);
                    if (xmlWriter != null) xmlWriter.Close();

                    command.Parameters.AddWithValue("@" + sqlParmName, sw.ToString());
                }
                else if (parm is EntityUpdateColumn<ServerEnum>)
                {
                    var p = parm as EntityUpdateColumn<ServerEnum>;
                    ServerEnum v = p.Value;
                    command.Parameters.AddWithValue("@" + sqlParmName, v.Enum);
                }
                else if (parm is EntityUpdateColumn<ServerEntity>)
                {
                    var p = parm as EntityUpdateColumn<ServerEntity>;
                    ServerEntity v = p.Value;
                    command.Parameters.AddWithValue("@" + sqlParmName, v.GetKey().Key);
                }
                else if (parm is EntityUpdateColumn<ServerEntityKey>)
                {
                    var p = parm as EntityUpdateColumn<ServerEntityKey>;
                    ServerEntityKey key = p.Value;
                    command.Parameters.AddWithValue("@" + sqlParmName, key == null ? DBNull.Value : key.Key);
                }
                else if (parm is EntityUpdateColumn<DateTime?>)
                {
                    var p = parm as EntityUpdateColumn<DateTime?>;
                    DateTime? value = p.Value;
                    if (value.HasValue)
                        command.Parameters.AddWithValue("@" + sqlParmName, value.Value);
                    else
                        command.Parameters.AddWithValue("@" + sqlParmName, DBNull.Value);
                }
                else
                {
                    if (parm.Value is ServerEnum)
                    {
                        var v = (ServerEnum)parm.Value;
                        command.Parameters.AddWithValue("@" + sqlParmName, v.Enum);
                    }
                    else
                        command.Parameters.AddWithValue("@" + sqlParmName, parm.Value ?? DBNull.Value);
                }

                string text = String.Format("[{0}]=@{0}", sqlParmName);

                if (first)
                {
                    first = false;
                    setClause.AppendFormat(text);
                }
                else
                    setClause.AppendFormat(", {0}", text);
            }

            return setClause.ToString();
        }

        /// <summary>
        /// Proves a SQL statement based on the supplied input criteria.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <param name="key">The GUID of the table row to update</param>
        /// <param name="parameters">The columns to update.</param>
        /// <returns>The SQL string.</returns>
        private static string GetUpdateSql(string entityName, SqlCommand command, ServerEntityKey key,
                                           EntityUpdateColumns parameters)
        {
            // SET clause
            String setClause = GetUpdateSetClause(command, parameters);

            // WHERE clause
            String whereClause = GetUpdateWhereClause(command, key);

            return String.Format("UPDATE [{0}] SET {1} WHERE {2}", entityName, setClause, whereClause);
        }


        /// <summary>
        /// Proves an UPDATE SQL statement based on the supplied input entity.
        /// </summary>
        /// <param name="entity">The entity that is being selected from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <returns>The SQL string.</returns>
        private static string GetUpdateSql(TServerEntity entity, SqlCommand command)
        {
            Dictionary<string, string> columnMap = GetColumnMap(entity.GetType());

            var set = new StringBuilder();
            int fieldUpdated = 0;
			
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (columnMap.ContainsKey(prop.Name))
                {
                    object value = prop.GetValue(entity, null);

                    if (value is ServerEntityKey)
                    {
                        value = ((ServerEntityKey)value).Key;
                    }
                    else if (value is ServerEnum)
                    {
                        value = ((ServerEnum)value).Enum;
                    }
                    else if (value is ServerEntity)
                    {
                        value = ((ServerEntity)value).GetKey().Key;
                    }
                    else if (value is XmlDocument)
                    {
                        var xml = (XmlDocument) value;
                        var sw = new StringWriter();
                        var xmlSettings = new XmlWriterSettings
                                                            {
                                                                Encoding = Encoding.UTF8,
                                                                ConformanceLevel = ConformanceLevel.Fragment,
                                                                Indent = false,
                                                                NewLineOnAttributes = false,
                                                                CheckCharacters = true,
                                                                IndentChars = ""
                                                            };

                        XmlWriter xmlWriter = XmlWriter.Create(sw, xmlSettings);
                        xml.WriteTo(xmlWriter);
                        if (xmlWriter != null) xmlWriter.Close();

                        value = sw.ToString();
                    }

                    if (fieldUpdated == 0)
                    {

                        set.AppendFormat("{0}= @{1}", columnMap[prop.Name], prop.Name);
                        command.Parameters.AddWithValue("@" + prop.Name, value ?? DBNull.Value);
                    }
                    else
                    {
                        set.AppendFormat(", {0}=@{1}", columnMap[prop.Name], prop.Name);
                        command.Parameters.AddWithValue("@" + prop.Name, value ?? DBNull.Value);
                    }

                    fieldUpdated++;
                }
                
            }

            if (fieldUpdated == 0)
                return null;

            var where = new StringBuilder();
            where.AppendFormat("GUID=@{0}", "KEY");
            command.Parameters.AddWithValue("@KEY", entity.GetKey().Key);
            
            
            var sql = new StringBuilder();
            sql.AppendFormat("UPDATE [{0}] SET {1} WHERE {2}", entity.Name, set, where);

            return sql.ToString();
        }
        /// <summary>
        /// Proves a SQL statement based on the supplied input criteria.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="command">The SqlCommand to use.</param>
        /// <param name="criteria">The criteria for the update</param>
        /// <param name="parameters">The columns to update.</param>
        /// <returns>The SQL string.</returns>
        private static string GetUpdateSql(string entityName, SqlCommand command, TSelectCriteria criteria,
                                           EntityUpdateColumns parameters)
        {
            var sb = new StringBuilder();
            // SET clause
            String setClause = GetUpdateSetClause(command, parameters);

            // WHERE clause
            // Generate an array of the WHERE clauses to be used.
            String[] where = GetWhereSearchCriteria(entityName, criteria, command);

            // Add the where clauses on.
            bool first = true;
            foreach (String clause in where)
            {
                if (first)
                {
                    first = false;
                    sb.AppendFormat(" WHERE {0}", clause);
                }
                else
                    sb.AppendFormat(" AND {0}", clause);
            }

            return String.Format("UPDATE [{0}] SET {1} {2}", entityName, setClause, sb);
        }

        /// <summary>
        /// Generates a SQL statement based on the input.
        /// </summary>
        /// <param name="entityName">The entity that is being selected from.</param>
        /// <param name="parameters">The columns to insert.</param>
        /// <param name="command">The SQL Command for which the Insert SQL is being created.</param>
        /// <returns>The SQL string.</returns>
        private static string GetInsertSql(SqlCommand command, string entityName, EntityUpdateColumns parameters)
        {
            Guid guid = Guid.NewGuid();

            // Build the text after the INSERT INTO clause
            var intoText = new StringBuilder();
            intoText.Append("(");
            intoText.Append("[GUID]");

            // Build the text after the VALUES clause
            var valuesText = new StringBuilder();
            valuesText.Append("(");
            valuesText.AppendFormat("@PrimaryKey");
            command.Parameters.AddWithValue("@PrimaryKey", guid);

            foreach (EntityColumnBase parm in parameters.SubParameters.Values)
            {
                String sqlParmName = GetDbColumnName(parm);
                intoText.AppendFormat(", {0}", sqlParmName);

                if (parm.Value is ServerEnum)
                {
                    var v = (ServerEnum) parm.Value;
                    valuesText.AppendFormat(", @{0}", sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, v.Enum);
                }
                else if (parm is EntityUpdateColumn<ServerEntity>)
                {
                    var v = (ServerEntity) parm.Value;
                    valuesText.AppendFormat(", @{0}", sqlParmName);
                    command.Parameters.AddWithValue("@" + sqlParmName, v.GetKey().Key);
                }
                else if (parm is EntityUpdateColumn<ServerEntityKey>)
                {
                    var key = (ServerEntityKey) parm.Value;
                    valuesText.AppendFormat(", @{0}", sqlParmName);
                    if (key != null)
                    {
                        command.Parameters.AddWithValue("@" + sqlParmName, key.Key);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@" + sqlParmName, DBNull.Value);
                    }
                }
                else if (parm is EntityUpdateColumn<XmlDocument>)
                {
                    var xml = (XmlDocument) parm.Value;

                    if (xml!=null)
                    {
                        var sb = new StringBuilder();
                        var settings = new XmlWriterSettings
                                           {
                                               Indent = false
                                           };

                        using (XmlWriter writer = XmlWriter.Create(sb, settings))
                        {
                            xml.WriteTo(writer);
                            writer.Flush();
                        }
                        valuesText.AppendFormat(", @{0}", sqlParmName);
                        command.Parameters.AddWithValue("@" + sqlParmName, sb.ToString());
                    }
                    else
                    {
                        valuesText.AppendFormat(", @{0}", sqlParmName);
                        command.Parameters.AddWithValue("@" + sqlParmName, DBNull.Value);    
                    }
                    
                    
                }
                else
                {
                    valuesText.AppendFormat(", @{0}", sqlParmName);

                    command.Parameters.AddWithValue("@" + sqlParmName, parm.Value ?? DBNull.Value);
                }
            }
            intoText.Append(")");
            valuesText.Append(")");

            // Generate the INSERT statement
            var sql = new StringBuilder();
            sql.AppendFormat("INSERT INTO [{0}] {1} VALUES {2}\n", entityName, intoText, valuesText);

            // Add the SELECT statement. This allows us to popuplate the entity with the inserted values 
            // and return to the caller
            sql.AppendFormat("SELECT * FROM [{0}] WHERE [GUID]=@PrimaryKey", entityName);

            return sql.ToString();
        }

        #endregion

        #region IEntityBroker<TServerEntity,TSelectCriteria,TUpdateColumns> Members

        /// <summary>
        /// Load an entity based on the primary key.
        /// </summary>
        /// <param name="entityRef"></param>
        /// <returns></returns>
        public TServerEntity Load(ServerEntityKey entityRef)
        {
            TServerEntity row = null; // new TServerEntity();

            SqlDataReader myReader = null;
            SqlCommand command = null;

            try
            {
                command = new SqlCommand(String.Format("SELECT * FROM [{0}] WHERE GUID = @GUID",
                                                       _entityName), Context.Connection)
                              {
                                  CommandType = CommandType.Text,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };
                var update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                command.Parameters.AddWithValue("@GUID", entityRef.Key);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, command.CommandText);

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to select contents of '{0}'", _entityName);
                    command.Dispose();
                    return null;
                }
                else
                {
                    if (myReader.HasRows)
                    {
                        myReader.Read();

                        Dictionary<string, PropertyInfo> propMap = EntityMapDictionary.GetEntityMap(typeof(TServerEntity));

                        row = new TServerEntity();
                        PopulateEntity(myReader, row, propMap);

                        return row;
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when loading entity: {0}", _entityName);

                throw new PersistenceException(
                    String.Format("Unexpected problem when loading entity: {0}: {1}", _entityName,
                                  e.Message), e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.
                if (myReader != null)
                {
                    myReader.Close();
                    myReader.Dispose();
                }
                if (command != null)
                    command.Dispose();
            }

            return row;
        }

        public IList<TServerEntity> Find(TSelectCriteria criteria)
        {
            IList<TServerEntity> list = new List<TServerEntity>();

            _Find(criteria, null, null, list.Add);

            return list;
        }

        public void Find(TSelectCriteria criteria, SelectCallback<TServerEntity> callback)
        {
            _Find(criteria, null, null, callback);
        }

        public int Count(TSelectCriteria criteria)
        {
            SqlCommand command = null;
            string sql = "";

            try
            {
                command = new SqlCommand
                              {
                                  Connection = Context.Connection,
                                  CommandType = CommandType.Text,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };

                UpdateContext update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = sql = GetSelectCountSql(_entityName, command, criteria, null);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, command.CommandText);

                object result = command.ExecuteScalar();

                var count = (int) result;
                return count;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with select: {0}", sql);

                throw new PersistenceException(
                    String.Format("Unexpected problem with select count statement on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the command, or else we won't be able to do anything with the
                // connection the next time here.
                if (command != null)
                    command.Dispose();
            }
        }


        public bool Delete(ServerEntityKey key)
        {
            Platform.CheckForNullReference(key, "key");

            SqlCommand command = null;
            try
            {
                command = new SqlCommand
                              {
                                  Connection = Context.Connection,
                                  CommandType = CommandType.Text,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };
                UpdateContext update = Context as UpdateContext;

                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = String.Format("delete from [{0}] where GUID = '{1}'", _entityName, key.Key);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, command.CommandText);

                int rows = command.ExecuteNonQuery();

                return rows > 0;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}",
                             command != null ? command.CommandText : "");

                throw new PersistenceException(
                    String.Format("Unexpected problem with delete statement on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.

                if (command != null)
                    command.Dispose();
            }
        }

        public bool Update(ServerEntityKey key, TUpdateColumns parameters)
        {
            Platform.CheckForNullReference(key, "key");
            Platform.CheckForNullReference(parameters, "parameters");

            SqlCommand command = null;
            try
            {
                command = new SqlCommand
                              {
                                  Connection = Context.Connection,
                                  CommandType = CommandType.Text,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };
                UpdateContext update = Context as UpdateContext;

                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = GetUpdateSql(_entityName, command, key, parameters);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, command.CommandText);

                int rows = command.ExecuteNonQuery();

                return rows > 0;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}",
                             command != null ? command.CommandText : "");

                throw new PersistenceException(
                    String.Format("Unexpected problem with update statement on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.

                if (command != null)
                    command.Dispose();
            }
        }

        public bool Update(TServerEntity entity)
        {
            Platform.CheckForNullReference(entity, "entity");

            SqlCommand command = null;
            try
            {
                command = new SqlCommand
                              {
                                  Connection = Context.Connection,
                                  CommandType = CommandType.Text,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };

                UpdateContext update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = GetUpdateSql(entity, command);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, command.CommandText);

                if (command.CommandText == null)
                    return true; //ok. nothign to update

                int rows = command.ExecuteNonQuery();
                return rows > 0;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}",
                             command != null ? command.CommandText : "");

                throw new PersistenceException(
                    String.Format("Unexpected problem with update statement on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.

                if (command != null)
                    command.Dispose();
            }
                
        }

        public bool Update(TSelectCriteria criteria, TUpdateColumns parameters)
        {
            Platform.CheckForNullReference(criteria, "criteria");
            Platform.CheckForNullReference(parameters, "parameters");

            SqlCommand command = null;
            try
            {
                command = new SqlCommand
                              {
                                  Connection = Context.Connection,
                                  CommandType = CommandType.Text,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };

                UpdateContext update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = GetUpdateSql(_entityName, command, criteria, parameters);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, command.CommandText);

                int rows = command.ExecuteNonQuery();

                return rows > 0;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}",
                             command != null ? command.CommandText : "");

                throw new PersistenceException(
                    String.Format("Unexpected problem with update statement on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.

                if (command != null)
                    command.Dispose();
            }
        }

        public TServerEntity Insert(TUpdateColumns parameters)
        {
            Platform.CheckForNullReference(parameters, "parameters");
            Platform.CheckFalse(parameters.IsEmpty, "parameters must not be empty");


            SqlCommand command = null;
            try
            {
                command = new SqlCommand
                              {
                                  Connection = Context.Connection,
                                  CommandType = CommandType.Text,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };

                UpdateContext update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = GetInsertSql(command, _entityName, parameters);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, command.CommandText);

                TServerEntity entity = null;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Dictionary<string, PropertyInfo> propMap = EntityMapDictionary.GetEntityMap(typeof(TServerEntity));

                        while (reader.Read())
                        {
                            entity = new TServerEntity();
                            PopulateEntity(reader, entity, propMap);
                            break;
                        }
                    }
                }

                return entity;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with update: {0}",
                             command != null ? command.CommandText : "");

                throw new PersistenceException(
                    String.Format("Unexpected problem with insert statement on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup
                if (command != null)
                    command.Dispose();
            }
        }

        #endregion


        #region IEntityBroker<TServerEntity,TSelectCriteria,TUpdateColumns> Members

        public TServerEntity FindOne(TSelectCriteria criteria)
        {
            IList<TServerEntity> list = Find(criteria, 0, 1);
            if (list.Count == 0)
                return null;

            return list[0];
        }

        public IList<TServerEntity> Find(TSelectCriteria criteria, int startIndex, int maxRows)
        {
            IList<TServerEntity> list = new List<TServerEntity>();

            Find(criteria, startIndex, maxRows, list.Add);

            return list;
        }

        public void Find(TSelectCriteria criteria, int startIndex, int maxRows, SelectCallback<TServerEntity> callback)
        {
            _Find(criteria, startIndex, maxRows, callback);
        }

        private void _Find(TSelectCriteria criteria, int? startIndex, int? maxRows, SelectCallback<TServerEntity> callback)
        {
            SqlDataReader myReader = null;
            SqlCommand command = null;
            string sql = "";

            try
            {
                command = new SqlCommand
                              {
                                  Connection = Context.Connection,
                                  CommandType = CommandType.Text,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };

                var update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                command.CommandText = sql = GetSelectSql(_entityName, command, criteria, startIndex, maxRows, null);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, command.CommandText);

                myReader = command.ExecuteReader();
                if (myReader == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to select contents of '{0}'", _entityName);
                    Platform.Log(LogLevel.Error, "Select statement: {0}", sql);

                    command.Dispose();
                    return;
                }
                else
                {
                    if (myReader.HasRows)
                    {
                        Dictionary<string, PropertyInfo> propMap = EntityMapDictionary.GetEntityMap(typeof(TServerEntity));

                        while (myReader.Read())
                        {
                            var row = new TServerEntity();

                            PopulateEntity(myReader, row, propMap);

                            callback(row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with select: {0}", sql);

                throw new PersistenceException(
                    String.Format("Unexpected problem with select statement on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.
                if (myReader != null)
                {
                    myReader.Close();
                    myReader.Dispose();
                }
                if (command != null)
                    command.Dispose();
            }
        }

        #endregion

        #region IEntityBroker<TServerEntity,TSelectCriteria,TUpdateColumns> Members


        public int Delete(TSelectCriteria criteria)
        {
            Platform.CheckForNullReference(criteria, "criteria");

            SqlCommand command = null;
            try
            {
                command = new SqlCommand
                              {
                                  Connection = Context.Connection,
                                  CommandType = CommandType.Text,
                                  CommandTimeout = SqlServerSettings.Default.CommandTimeout
                              };

                var update = Context as UpdateContext;
                if (update != null)
                    command.Transaction = update.Transaction;

                string deleteWhereClause = GetDeleteWhereClause(_entityName, command, criteria, null);
                    
                command.CommandText = String.IsNullOrEmpty(deleteWhereClause) 
                    ? String.Format("DELETE FROM [{0}]", _entityName) 
                    : String.Format("DELETE FROM [{0}] WHERE {1}", _entityName, deleteWhereClause);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, command.CommandText);

                int rows = command.ExecuteNonQuery();

                return rows;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception with delete: {0}",
                             command != null ? command.CommandText : "");

                throw new PersistenceException(
                    String.Format("Unexpected problem with delete statment on table {0}: {1}", _entityName, e.Message),
                    e);
            }
            finally
            {
                // Cleanup the reader/command, or else we won't be able to do anything with the
                // connection the next time here.

                if (command != null)
                    command.Dispose();
            }
        }

        #endregion
    }
}