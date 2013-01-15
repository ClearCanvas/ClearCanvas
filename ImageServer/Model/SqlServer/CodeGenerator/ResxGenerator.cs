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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Resources;
using System.Text;

namespace ClearCanvas.ImageServer.Model.SqlServer.CodeGenerator
{

    class EnumRecord
    {
        public string Name { get; set; }
        public string Lookup { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
    }


    class ResxGenerator
    {
        public ResxGenerator()
        {
        }

        public void GenerateResxFile(TextWriter textWriter)
        {
            var enumTables = LoadEnumTables();
            var enumValues = LoadEnumValues(enumTables);

            using (ResXResourceWriter writer = new ResXResourceWriter(textWriter))
            {
                
                foreach (EnumRecord value in enumValues)
                {
                    writer.AddResource(new ResXDataNode(GetDescriptionResKey(value.Name, value.Lookup), value.Description));
                    writer.AddResource(new ResXDataNode(GetLongDescriptionResKey(value.Name, value.Lookup), value.LongDescription));
                }

                writer.Generate();
            }

        }

        private List<EnumRecord> LoadEnumValues(IEnumerable<string> enumTableNames)
        {
            List<EnumRecord> values = new List<EnumRecord>();
            foreach (string table in enumTableNames)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Working with table: {0}", table);
                Console.WriteLine("");

                if (table.EndsWith("Enum"))
                {
                    using (SqlConnection connection = new SqlConnection())
                    {
                        ConnectionStringSettings settings =
                            ConfigurationManager.ConnectionStrings[ConnectionStringName];
                        connection.ConnectionString = settings.ConnectionString;
                        connection.Open();
                        StringBuilder cmd = new StringBuilder();
                        cmd.AppendFormat("SELECT * FROM {0}", table);
                        SqlCommand cmdSelect = new SqlCommand(cmd.ToString(), connection);

                        SqlDataReader myReader = cmdSelect.ExecuteReader();
                        if (myReader != null)
                            while (myReader.Read())
                            {
                                var value = new EnumRecord()
                                {
                                    Name = table,
                                    Lookup = (string)myReader["Lookup"],
                                    Description = (string)myReader["Description"],
                                    LongDescription = (string)myReader["LongDescription"]
                                };

                                values.Add(value);
                            }
                    }

                }
            }
            return values;

        }

        public string ConnectionStringName { get; set; }

        public IEnumerable<string> LoadEnumTables()
        {
            List<string> tables = new List<string>();

            SqlConnection connection = new SqlConnection();

            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings[ConnectionStringName];

            connection.ConnectionString = settings.ConnectionString;
            connection.Open();


            DataTable dataTable = connection.GetSchema("Tables", new[] { null, "dbo", null, null });

            DataColumn colTableName = dataTable.Columns["TABLE_NAME"];
            if (colTableName != null)
                foreach (DataRow row in dataTable.Rows)
                {

                    String tableName = row[colTableName].ToString();
                    if (!tableName.StartsWith("sys") && tableName.EndsWith("Enum"))
                    {
                        try
                        {
                            tables.Add(tableName);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Unexpected exception when processing table: " + e.Message);
                        }
                    }
                }

            return tables;
        }


        static public string GetDescriptionResKey(string enumName, string enumLookupValue)
        {
            return string.Format("{0}_{1}_Description", enumName, enumLookupValue);
        }
        static public string GetLongDescriptionResKey(string enumName, string enumLookupValue)
        {
            return string.Format("{0}_{1}_LongDescription", enumName, enumLookupValue);
        }
        
    }
}