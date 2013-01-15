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
using System.Data.SqlTypes;
using System.Reflection;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    public abstract class Broker : IPersistenceBroker
    {
        private PersistenceContext _context;

        /// <summary>
        /// Returns the persistence context associated with this broker instance.
        /// </summary>
        protected PersistenceContext Context
        {
            get { return _context; }
        }

        public void SetContext(IPersistenceContext context)
        {
            _context = (PersistenceContext)context;
        }

        protected static void SetParameters(SqlCommand command, ProcedureParameters parms)
        {
            foreach (SearchCriteria parm in parms.EnumerateSubCriteria())
            {
                String sqlParmName = "@" + parm.GetKey();
                
                if (parm is ProcedureParameter<DateTime?>)
                {
                    ProcedureParameter<DateTime?> parm2 = (ProcedureParameter<DateTime?>)parm;
                    if (!parm2.Output)
                        command.Parameters.AddWithValue(sqlParmName, parm2.Value);
                    else
                    {
                        SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.DateTime);
                        sqlParm.IsNullable = true;
                        sqlParm.Direction = ParameterDirection.Output;
                    }
                }
                else if (parm is ProcedureParameter<DateTime>)
                {
                    ProcedureParameter<DateTime> parm2 = (ProcedureParameter<DateTime>)parm;
                    if (!parm2.Output)
                        command.Parameters.AddWithValue(sqlParmName, parm2.Value);
                    else
                    {
                        SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.DateTime);
                        sqlParm.Direction = ParameterDirection.Output;
                    }
                }
                else if (parm is ProcedureParameter<int>)
                {
                    ProcedureParameter<int> parm2 = (ProcedureParameter<int>)parm;

                    if (!parm2.Output)
                        command.Parameters.AddWithValue(sqlParmName, parm2.Value);
                    else
                    {
                        SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.Int);
                        sqlParm.Direction = ParameterDirection.Output;
                    }
                }
                else if (parm is ProcedureParameter<ServerEntityKey>)
                {
                    sqlParmName = sqlParmName.Replace("Key", "GUID");
                    ProcedureParameter<ServerEntityKey> parm2 = (ProcedureParameter<ServerEntityKey>)parm;

                    if (!parm2.Output)
                    {
                        if (parm2.Value!=null)
                            command.Parameters.AddWithValue(sqlParmName, parm2.Value.Key);   
                        else
                        {
                            command.Parameters.AddWithValue(sqlParmName, DBNull.Value);   
                        }
                    }
                    else
                    {
                        SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.UniqueIdentifier);
                        sqlParm.Direction = ParameterDirection.Output;
                    }
                }
                else if (parm is ProcedureParameter<bool>)
                {
                    ProcedureParameter<bool> parm2 = (ProcedureParameter<bool>)parm;
                    if (!parm2.Output)
                        command.Parameters.AddWithValue(sqlParmName, parm2.Value);
                    else
                    {
                        SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.Bit);
                        sqlParm.Direction = ParameterDirection.Output;
                    }
                }
                else if (parm is ProcedureParameter<string>)
                {
                    ProcedureParameter<string> parm2 = (ProcedureParameter<string>)parm;

                    if (!parm2.Output)
                        command.Parameters.AddWithValue(sqlParmName, parm2.Value);
                    else
                    {
                        SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.NVarChar, 1024);
                        sqlParm.Direction = ParameterDirection.Output;
                    }
                }
                else if (parm is ProcedureParameter<ServerEnum>)
                {
                    ProcedureParameter<ServerEnum> parm2 = (ProcedureParameter<ServerEnum>)parm;
                    if (parm2.Value == null)
                        command.Parameters.AddWithValue(sqlParmName, null);
                    else
                    {
                        if (parm2.Output)												
                            throw new PersistenceException("Unsupported output parameter type: ServerEnum",null);

                        command.Parameters.AddWithValue(sqlParmName, parm2.Value.Enum);
                    }
                }
                else if (parm is ProcedureParameter<Decimal>)
                {
                    ProcedureParameter<Decimal> parm2 = (ProcedureParameter<Decimal>)parm;

                    if (!parm2.Output)
                        command.Parameters.AddWithValue(sqlParmName, parm2.Value);
                    else
                    {
                        SqlParameter sqlParm = command.Parameters.Add(sqlParmName, SqlDbType.Decimal);
                        sqlParm.Direction = ParameterDirection.Output;
                    }
                }
                else if (parm is ProcedureParameter<XmlDocument>)
                {
                    ProcedureParameter<XmlDocument> parm2 = (ProcedureParameter<XmlDocument>)parm;
                    if (parm2.Value == null)
                        command.Parameters.AddWithValue(sqlParmName, null);
                    else
                    {
                        if (parm2.Output)
                            throw new PersistenceException("Unsupported output parameter type: XmlDocument", null);

                        if (parm2.Value.DocumentElement!=null)
                        {
                            XmlNodeReader reader = new XmlNodeReader(parm2.Value.DocumentElement);
                            SqlXml xml = new SqlXml(reader);
                            command.Parameters.AddWithValue(sqlParmName, xml);
                        }
                        else
                        {
                            SqlXml xml = new SqlXml();
                            command.Parameters.AddWithValue(sqlParmName, xml);
                        }
                        
                    }
                }
                else
                    throw new PersistenceException("Unknown procedure parameter type: " + parm.GetType(), null);

            }

        }

        protected static void GetOutputParameters(SqlCommand command, ProcedureParameters parms)
        {
            foreach (SearchCriteria parm in parms.EnumerateSubCriteria())
            {
                String sqlParmName = "@" + parm.GetKey();

                if (parm is ProcedureParameter<DateTime?>)
                {
                    ProcedureParameter<DateTime?> parm2 = (ProcedureParameter<DateTime?>)parm;
                    if (!parm2.Output)
                        continue;
                    SqlParameter sqlParm = command.Parameters[sqlParmName];
                    parm2.Value = (DateTime?)sqlParm.Value;
                }
                else if (parm is ProcedureParameter<DateTime>)
                {
                    ProcedureParameter<DateTime> parm2 = (ProcedureParameter<DateTime>)parm;
                    if (!parm2.Output)
                        continue;
                    SqlParameter sqlParm = command.Parameters[sqlParmName];
                    parm2.Value = (DateTime)sqlParm.Value;
                }
                else if (parm is ProcedureParameter<int>)
                {
                    ProcedureParameter<int> parm2 = (ProcedureParameter<int>)parm;

                    if (!parm2.Output)
                        continue;
                    SqlParameter sqlParm = command.Parameters[sqlParmName];
                    //object o = command.Connection.Get
                    if (sqlParm.Value != null)
                        parm2.Value = (int)sqlParm.Value;
                }
                else if (parm is ProcedureParameter<ServerEntityKey>)
                {
                    sqlParmName = sqlParmName.Replace("Key", "GUID");
                    ProcedureParameter<ServerEntityKey> parm2 = (ProcedureParameter<ServerEntityKey>)parm;

                    if (!parm2.Output)
                        continue;
                    SqlParameter sqlParm = command.Parameters[sqlParmName];
                    parm2.Value = new ServerEntityKey("", sqlParm.Value);
                }
                else if (parm is ProcedureParameter<bool>)
                {
                    ProcedureParameter<bool> parm2 = (ProcedureParameter<bool>)parm;
                    if (!parm2.Output)
                        continue;
                    SqlParameter sqlParm = command.Parameters[sqlParmName];
                    parm2.Value = (bool)sqlParm.Value;
                }
                else if (parm is ProcedureParameter<Decimal>)
                {
                    ProcedureParameter<Decimal> parm2 = (ProcedureParameter<Decimal>)parm;

                    if (!parm2.Output)
                        continue;
                    SqlParameter sqlParm = command.Parameters[sqlParmName];
                    parm2.Value = (Decimal)sqlParm.Value;
                }
                else if (parm is ProcedureParameter<string>)
                {
                    ProcedureParameter<string> parm2 = (ProcedureParameter<string>)parm;

                    if (!parm2.Output)
                        continue;
                    SqlParameter sqlParm = command.Parameters[sqlParmName];
                    if (sqlParm.Value != DBNull.Value)
                        parm2.Value = (string)sqlParm.Value;
                }
            }
        }

        protected static Dictionary<string, string> GetColumnMap(Type entityType)
        {
            ObjectWalker walker = new ObjectWalker();
            Dictionary<string, string> propMap = new Dictionary<string, string>();

            foreach (IObjectMemberContext member in walker.Walk(entityType))
            {
                EntityFieldDatabaseMappingAttribute map =
                    AttributeUtils.GetAttribute<EntityFieldDatabaseMappingAttribute>(member.Member);
                if (map != null)
                {
                    propMap.Add(member.Member.Name, map.ColumnName);
                }
            }

            return propMap;
        }

        protected static void PopulateEntity(SqlDataReader reader, ServerEntity entity, Dictionary<string, PropertyInfo> propMap)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                String columnName = reader.GetName(i);

                // Special case for when we select a range of values with an EntityBroker, just ignore
                if (columnName.Equals("RowNum"))
                    continue;

                if (columnName.Equals("GUID"))
                {
                    Guid uid = reader.GetGuid(i);
                    entity.SetKey(new ServerEntityKey(entity.Name, uid));
                    continue;
                }

                if (columnName.Equals(entity.Name) && columnName.Contains("Enum"))
                    columnName = "Enum";

                PropertyInfo prop;
				
                if (!propMap.TryGetValue(columnName, out prop))
                    throw new EntityNotFoundException("Unable to match column to property: " + columnName, null);

                if (columnName.Contains("GUID"))
                    columnName = columnName.Replace("GUID", "Key");

                if (reader.IsDBNull(i))
                {
                    prop.SetValue(entity, null, null);
                    continue;
                }

                Type propType = prop.PropertyType;
                if (propType == typeof(String))
                    prop.SetValue(entity, reader.GetString(i), null);
                else if (propType == typeof(ServerEntityKey))
                {
                    Guid uid = reader.GetGuid(i);
                    prop.SetValue(entity, new ServerEntityKey(columnName.Replace("Key", String.Empty), uid), null);
                }
                else if (propType == typeof(DateTime))
                    prop.SetValue(entity, reader.GetDateTime(i), null);
                else if (propType == typeof(DateTime?))
                {
                    if (reader.IsDBNull(i))
                    {
                        prop.SetValue(entity, null, null);
                    }
                    else
                    {
                        prop.SetValue(entity, reader.GetDateTime(i), null);
                    }
                    
                }
                else if (propType == typeof(bool))
                    prop.SetValue(entity, reader.GetBoolean(i), null);
                else if (propType == typeof(Int32))
                    prop.SetValue(entity, reader.GetInt32(i), null);
                else if (propType == typeof(Int16))
                    prop.SetValue(entity, reader.GetInt16(i), null);
                else if (propType == typeof(double))
                    prop.SetValue(entity, reader.GetDouble(i), null);
                else if (propType == typeof(Decimal))
                    prop.SetValue(entity, reader.GetDecimal(i), null);
                else if (propType == typeof(float))
                    prop.SetValue(entity, reader.GetFloat(i), null);
                else if (propType == typeof(XmlDocument))
                {
                    SqlXml xml = reader.GetSqlXml(i);
                    if (xml!=null && !xml.IsNull)
                    {
                        XmlReader xmlReader = xml.CreateReader();
                        if (xmlReader != null)
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(xmlReader);
                            prop.SetValue(entity, xmlDoc, null);
                        }
                        else
                            prop.SetValue(entity, null, null);
                    }
                    else
                    {
                        prop.SetValue(entity, null, null);
                    }
                }
                else if (typeof(ServerEnum).IsAssignableFrom(propType))
                {
                    short enumVal = reader.GetInt16(i);
                    ConstructorInfo construct = prop.PropertyType.GetConstructor(new Type[0]);
                    ServerEnum val = (ServerEnum)construct.Invoke(null);
                    val.SetEnum(enumVal);
                    prop.SetValue(entity, val, null);
                }
                else
                    throw new EntityNotFoundException("Unsupported property type: " + propType, null);
            }
        }
    }
}