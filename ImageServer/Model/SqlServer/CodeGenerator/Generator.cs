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
using System.Text;
using System.Xml;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.SqlServer.CodeGenerator
{
    public class Generator
    {
        private class Column
        {
            private readonly string _columnName;
            private readonly Type _columnType;
            private readonly string _variableName;
            private readonly string _databaseColumnName;
            
            public Column(string name, string type, bool isNullable=false)
            {
                _columnName = name.Replace("GUID", "Key");
                _variableName = String.Format("_{0}{1}", _columnName.Substring(0, 1).ToLower(), _columnName.Substring(1));
                _databaseColumnName = _columnName;
                if (_columnName.EndsWith("_"))
                    _columnName = _columnName.Substring(0, _columnName.Length - 1);

                if (type.Equals("nvarchar"))
                    _columnType = typeof(String);
                else if (type.Equals("varchar"))
                    _columnType = typeof(String);
                else if (type.Equals("smallint"))
                    _columnType = typeof(short);
                else if (type.Equals("uniqueidentifier"))
                    _columnType = typeof(ServerEntityKey);
                else if (type.Equals("bit"))
                    _columnType = typeof(bool);
                else if (type.Equals("int"))
                    _columnType = typeof(int);
                else if (type.Equals("datetime"))
					_columnType = isNullable? typeof(DateTime?):typeof(DateTime);
                else if (type.Equals("xml"))
                    _columnType = typeof(XmlDocument);
                else if (type.Equals("decimal"))
                    _columnType = typeof(Decimal);
                else
                    throw new ApplicationException("Unexpected SQL Server type: " + type);
            }

            public string DatabaseColumnName
            {
                get { return _databaseColumnName; }
            }

            public string ColumnName
            {
                get { return _columnName; }
            }

            public Type ColumnType
            {
                get { return _columnType; }
            }

	        public string DeclaringType
	        {
		        get
		        {
					return ColumnType.IsGenericType? ColumnType.GetGenericArguments()[0].Name + "?" : ColumnType.Name;
		        }
	        }

            public string VariableName
            {
                get { return _variableName; }
            }
        }

        private class Table
        {
            private readonly string _tableName;
            private readonly List<Column> _columnList = new List<Column>();
            private readonly string _databaseTableName;

            public Table(string name)
            {
                _tableName = name;
                _databaseTableName = name;
                if (_tableName.EndsWith("_"))
                    _tableName = _tableName.Substring(0, _tableName.Length - 1);
            }

            public IList<Column> Columns
            {
                get { return _columnList; }
            }

            public string TableName
            {
                get { return _tableName; }
            }

            public string DatabaseTableName
            {
                get { return _databaseTableName; }
            }
        }

        #region Private Members
        private readonly List<Table> _tableList = new List<Table>();
        private readonly string _selectCriteriaFolder = Path.Combine("Model", "Criteria");
        private readonly string _entityBrokerFolder = Path.Combine("Model", "EntityBrokers");

        #endregion

        #region Public Properties

        public string ModelNamespace { get; set; }

        private List<Table> TableList
        {
            get { return _tableList; }
        }

        public string SelectCriteriaFolder
        {
            get { return _selectCriteriaFolder; }
        }

        public string EntityBrokerFolder
        {
            get { return _entityBrokerFolder; }
        }
        
        public string ImageServerModelFolder { get; set; }

        public string EntityInterfaceFolder { get; set; }

        public string EntityInterfaceNamespace { get; set; }

        public string EntityImplementationNamespace { get; set; }

        public string EntityImplementationFolder { get; set; }

        public string ConnectionStringName
        {
            get; set;
        }

        public bool Proprietary { get; set; }

        public bool GenerateResxFile { get; set; }

        #endregion

        public void LoadTableInfo()
        {
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
                    if (!tableName.StartsWith("sys"))
                    {
                        try
                        {
                            Table table = new Table(tableName);

                            DataTable dt = connection.GetSchema("Columns", new[] { null, null, tableName });

                            DataColumn colColumnName = dt.Columns["COLUMN_NAME"];
                            DataColumn colColumnType = dt.Columns["DATA_TYPE"];
	                        var isNullable = dt.Columns["IS_NULLABLE"];
                            foreach (DataRow row2 in dt.Rows)
                            {
                                table.Columns.Add(
									new Column(row2[colColumnName].ToString(), row2[colColumnType].ToString(), row2[isNullable].ToString().ToUpper()=="YES"));
                            }

                            TableList.Add(table);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Unexpected exception when processing table: " + e.Message);
                        }
                    }
                }
        }

        private void WriterHeader(TextWriter writer, string nameSpace)
        {
            writer.WriteLine("#region License");
            writer.WriteLine("");
            if (Proprietary)
            {
                writer.WriteLine("// Copyright (c) 2013, ClearCanvas Inc.");
                writer.WriteLine("// All rights reserved.");
                writer.WriteLine("// http://www.clearcanvas.ca");
                writer.WriteLine("//");
                writer.WriteLine("// For information about the licensing and copyright of this software please");
                writer.WriteLine("// contact ClearCanvas, Inc. at info@clearcanvas.ca");
            }
            else
            {
                writer.WriteLine("// Copyright (c) 2013, ClearCanvas Inc.");
                writer.WriteLine("// All rights reserved.");
                writer.WriteLine("// http://www.clearcanvas.ca");
                writer.WriteLine("//");
                writer.WriteLine("// This file is part of the ClearCanvas RIS/PACS open source project.");
                writer.WriteLine("//");
                writer.WriteLine("// The ClearCanvas RIS/PACS open source project is free software: you can");
                writer.WriteLine("// redistribute it and/or modify it under the terms of the GNU General Public");
                writer.WriteLine("// License as published by the Free Software Foundation, either version 3 of the");
                writer.WriteLine("// License, or (at your option) any later version.");
                writer.WriteLine("//");
                writer.WriteLine("// The ClearCanvas RIS/PACS open source project is distributed in the hope that it");
                writer.WriteLine("// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of");
                writer.WriteLine("// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General");
                writer.WriteLine("// Public License for more details.");
                writer.WriteLine("//");
                writer.WriteLine("// You should have received a copy of the GNU General Public License along with");
                writer.WriteLine("// the ClearCanvas RIS/PACS open source project.  If not, see");
                writer.WriteLine("// <http://www.gnu.org/licenses/>.");
            }
            writer.WriteLine("");
            writer.WriteLine("#endregion");
            writer.WriteLine("");
            writer.WriteLine("// This file is auto-generated by the ClearCanvas.Model.SqlServer.CodeGenerator project.");
            writer.WriteLine("");
            writer.WriteLine("namespace {0}", nameSpace);
            writer.WriteLine("{");
        }

        private static void WriteFooter(TextWriter writer)
        {
            writer.WriteLine("}");
        }

        private void WriteEnumBrokerInterfaceFile(Table table)
        {
            String fileName = String.Format("I{0}Broker.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(EntityInterfaceFolder, fileName));

            WriterHeader(writer, EntityInterfaceNamespace);

            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("");

            writer.WriteLine("    public interface I{0}Broker: IEnumBroker<{0}>", table.TableName);
            writer.WriteLine("    { }");

            WriteFooter(writer);

            writer.Close();
        }

        private void WriteEnumBrokerImplementationFile(Table table)
        {
            String fileName = String.Format("{0}Broker.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(EntityImplementationFolder, fileName));

            WriterHeader(writer, EntityImplementationNamespace);

            writer.WriteLine("    using ClearCanvas.Common;");
            writer.WriteLine("    using {0};", EntityInterfaceNamespace);
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise.SqlServer;");
            writer.WriteLine("");

            writer.WriteLine("    [ExtensionOf(typeof(BrokerExtensionPoint))]");
            writer.WriteLine("    public class {0}Broker : EnumBroker<{0}>, I{0}Broker", table.TableName);
            writer.WriteLine("    { }");

            WriteFooter(writer);

            writer.Close();
        }

        private void WriteEnumFile(Table table)
        {
            String fileName = String.Format("{0}.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(ImageServerModelFolder, fileName));

            WriterHeader(writer, ModelNamespace);

            writer.WriteLine("    using System;");
            writer.WriteLine("    using System.Collections.Generic;");
            writer.WriteLine("    using {0};",EntityInterfaceNamespace);
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("    using System.Reflection;");
            writer.WriteLine("");

            writer.WriteLine("[Serializable]");
            writer.WriteLine("public partial class {0} : ServerEnum", table.TableName);
            writer.WriteLine("{");
           
            WritePredefinedEnums(table, writer);

            writer.WriteLine("      #region Constructors");
            writer.WriteLine("      public {0}():base(\"{0}\")", table.DatabaseTableName);
            writer.WriteLine("      {}");
            writer.WriteLine("      #endregion");
            writer.WriteLine("      #region Public Members");
            writer.WriteLine("      public override void SetEnum(short val)");
            writer.WriteLine("      {");
            writer.WriteLine("          ServerEnumHelper<{0}, I{0}Broker>.SetEnum(this, val);", table.TableName);
            writer.WriteLine("      }");

            writer.WriteLine("      static public List<{0}> GetAll()", table.TableName);
            writer.WriteLine("      {");
            writer.WriteLine("          return ServerEnumHelper<{0}, I{0}Broker>.GetAll();", table.TableName);
            writer.WriteLine("      }");

            writer.WriteLine("      static public {0} GetEnum(string lookup)", table.TableName);
            writer.WriteLine("      {");
            writer.WriteLine("          return ServerEnumHelper<{0}, I{0}Broker>.GetEnum(lookup);", table.TableName);
            writer.WriteLine("      }");

            writer.WriteLine("      #endregion");

            writer.WriteLine("}");


            WriteFooter(writer);

            writer.Close();
        }

        private void WritePredefinedEnums(Table table, TextWriter writer)
        {
            Dictionary<string, string> enumValues = new Dictionary<string, string>();

            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
                connection.ConnectionString = settings.ConnectionString;
                connection.Open();

                StringBuilder cmd = new StringBuilder();
                cmd.AppendFormat("SELECT * FROM {0}", table.TableName);
                SqlCommand cmdSelect = new SqlCommand(cmd.ToString(), connection);

                SqlDataReader myReader = cmdSelect.ExecuteReader();
                if (myReader != null)
                    while (myReader.Read())
                    {
                        string lookup = (string)myReader["Lookup"];
                        string longDescription = (string)myReader["LongDescription"];

                        enumValues.Add(lookup, longDescription);
                    }

                writer.WriteLine("      #region Private Static Members");
                foreach (string lookupValue in enumValues.Keys)
                {
                    string fieldName = lookupValue.Replace(" ", "");
                    writer.WriteLine("      private static readonly {0} _{1} = GetEnum(\"{2}\");", table.TableName, fieldName, lookupValue);
                }
                writer.WriteLine("      #endregion");
                writer.WriteLine("");

                writer.WriteLine("      #region Public Static Properties");
                foreach (string lookupValue in enumValues.Keys)
                {
                    string fieldName = lookupValue.Replace(" ", "");
                    string description = enumValues[lookupValue];

                    writer.WriteLine("      /// <summary>");
                    writer.WriteLine("      /// {0}", description);
                    writer.WriteLine("      /// </summary>");
                    writer.WriteLine("      public static {0} {1}", table.TableName, fieldName);
                    writer.WriteLine("      {");
                    writer.WriteLine("          get {{ return _{0}; }}", fieldName);
                    writer.WriteLine("      }");
                }
                
                writer.WriteLine("");
            }
            writer.WriteLine("      #endregion");
            writer.WriteLine("");
        }

        private void WriteEntityBrokerInterfaceFile(Table table)
        {
            String fileName = String.Format("I{0}EntityBroker.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(EntityInterfaceFolder, fileName));

            WriterHeader(writer, EntityInterfaceNamespace);

            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("    using {0};", EntityInterfaceNamespace);
            writer.WriteLine("");

            writer.WriteLine("public interface I{0}EntityBroker : IEntityBroker<{0}, {0}SelectCriteria, {0}UpdateColumns>", table.TableName);
            writer.WriteLine("{ }");

            WriteFooter(writer);

            writer.Close();
        }

        private void WriteEntityBrokerImplementationFile(Table table)
        {
            String fileName = String.Format("{0}EntityBroker.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(EntityImplementationFolder, fileName));

            WriterHeader(writer, EntityImplementationNamespace);

            writer.WriteLine("    using System;");
            writer.WriteLine("    using System.Xml;");
            writer.WriteLine("    using ClearCanvas.Common;");
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("    using {0};", EntityInterfaceNamespace);
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise.SqlServer;");
            writer.WriteLine("");

            writer.WriteLine("    [ExtensionOf(typeof(BrokerExtensionPoint))]");
            writer.WriteLine("    public class {0}Broker : EntityBroker<{0}, {0}SelectCriteria, {0}UpdateColumns>, I{0}EntityBroker", table.TableName);
            writer.WriteLine("    {");
            writer.WriteLine("        public {0}Broker() : base(\"{1}\")", table.TableName, table.DatabaseTableName);
            writer.WriteLine("        { }");
            writer.WriteLine("    }");

            WriteFooter(writer);

            writer.Close();
        }
        private void WriteModelFile(Table table)
        {
            String fileName = String.Format("{0}.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);
            StreamWriter writer = new StreamWriter(Path.Combine(ImageServerModelFolder, fileName));

            WriterHeader(writer, ModelNamespace);

            writer.WriteLine("    using System;");
            writer.WriteLine("    using System.Xml;");
            bool bDicomReference = false;
            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    DicomTag tag = DicomTagDictionary.GetDicomTag(col.ColumnName);
                    if (tag != null)
                    {
                        bDicomReference = true;
                        break;
                    }
                }
            }
            if (bDicomReference)
                writer.WriteLine("    using ClearCanvas.Dicom;");
            writer.WriteLine("    using ClearCanvas.Enterprise.Core;");
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise.Command;");
            writer.WriteLine("    using {0};", EntityInterfaceNamespace);
            writer.WriteLine("");

            writer.WriteLine("    [Serializable]");
            writer.WriteLine("    public partial class {0}: ServerEntity", table.TableName);
            writer.WriteLine("    {");
            writer.WriteLine("        #region Constructors");
            writer.WriteLine("        public {0}():base(\"{1}\")",table.TableName, table.DatabaseTableName);
            writer.WriteLine("        {}");
            writer.WriteLine("        public {0}(", table.TableName);
            char comma = ' ';
            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    if (col.ColumnName.EndsWith("Enum"))
                        writer.WriteLine("            {0}{1} {2}_", comma, col.ColumnName, col.VariableName);
                    else
						writer.WriteLine("            {0}{1} {2}_", comma, col.DeclaringType, col.VariableName);

                    comma = ',';
                }
            }
            writer.WriteLine("            ):base(\"{0}\")", table.DatabaseTableName);
            writer.WriteLine("        {");
            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    if (col.ColumnName.EndsWith("Enum"))
                        writer.WriteLine("            {0} = {1}_;", col.ColumnName, col.VariableName);
                    else
                        writer.WriteLine("            {0} = {1}_;", col.ColumnName, col.VariableName);
                }
            }
            writer.WriteLine("        }");
            writer.WriteLine("        #endregion");
            writer.WriteLine("");
            writer.WriteLine("        #region Public Properties");
            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    DicomTag tag = DicomTagDictionary.GetDicomTag(col.ColumnName);
                    if (tag != null)
                        writer.WriteLine("        [DicomField(DicomTags.{0}, DefaultValue = DicomFieldDefault.Null)]", col.ColumnName);

                    writer.WriteLine("        [EntityFieldDatabaseMappingAttribute(TableName=\"{0}\", ColumnName=\"{1}\")]",table.DatabaseTableName, col.DatabaseColumnName.Replace("Key", "GUID"));

                    if (col.ColumnName.EndsWith("Enum"))
                    {
                        writer.WriteLine("        public {0} {1}", col.ColumnName, col.ColumnName);
                        writer.WriteLine("        { get; set; }");
                    }
                    else if (col.ColumnType == typeof(XmlDocument))
                    {
						writer.WriteLine("        public {0} {1}", col.DeclaringType, col.ColumnName);
                        writer.WriteLine("        {{ get {{ return _{0}; }} set {{ _{0} = value; }} }}", col.ColumnName);
                        writer.WriteLine("        [NonSerialized]");
                        writer.WriteLine("        private XmlDocument _{0};", col.ColumnName);
                    }
                    else
                    {
						writer.WriteLine("        public {0} {1}", col.DeclaringType, col.ColumnName);
                        writer.WriteLine("        { get; set; }");
                    }
                }
            }
            writer.WriteLine("        #endregion");
            writer.WriteLine("");
            writer.WriteLine("        #region Static Methods");
            writer.WriteLine("        static public {0} Load(ServerEntityKey key)", table.TableName);
            writer.WriteLine("        {");
            writer.WriteLine("            using (var context = new ServerExecutionContext())");
            writer.WriteLine("            {");
            writer.WriteLine("                return Load(context.ReadContext, key);");
            writer.WriteLine("            }");
            writer.WriteLine("        }");
            writer.WriteLine("        static public {0} Load(IPersistenceContext read, ServerEntityKey key)", table.TableName);
            writer.WriteLine("        {");
            writer.WriteLine("            var broker = read.GetBroker<I{0}EntityBroker>();", table.TableName);
            writer.WriteLine("            {0} theObject = broker.Load(key);", table.TableName);
            writer.WriteLine("            return theObject;");
            writer.WriteLine("        }");
            writer.WriteLine("        static public {0} Insert({0} entity)", table.TableName);
            writer.WriteLine("        {");
            writer.WriteLine("            using (var update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))");
            writer.WriteLine("            {");
            writer.WriteLine("                {0} newEntity = Insert(update, entity);", table.TableName);
            writer.WriteLine("                update.Commit();");
            writer.WriteLine("                return newEntity;");
            writer.WriteLine("            }");
            writer.WriteLine("        }");
            writer.WriteLine("        static public {0} Insert(IUpdateContext update, {0} entity)", table.TableName);
            writer.WriteLine("        {");
            writer.WriteLine("            var broker = update.GetBroker<I{0}EntityBroker>();", table.TableName);
            writer.WriteLine("            var updateColumns = new {0}UpdateColumns();", table.TableName);
            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    writer.WriteLine("            updateColumns.{0} = entity.{0};", col.ColumnName);
                }
            }
            writer.WriteLine("            {0} newEntity = broker.Insert(updateColumns);", table.TableName);
            writer.WriteLine("            return newEntity;");
            writer.WriteLine("        }");
            writer.WriteLine("        #endregion");
            writer.WriteLine("    }");

            WriteFooter(writer);

            writer.Close();
        }

        private void WriteEntitySelectCriteriaFile(Table table)
        {
            String fileName = String.Format("{0}SelectCriteria.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(EntityInterfaceFolder, fileName));

            WriterHeader(writer, EntityInterfaceNamespace);

            writer.WriteLine("    using System;");
            writer.WriteLine("    using System.Xml;");
            writer.WriteLine("    using ClearCanvas.Enterprise.Core;");
            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("");

            writer.WriteLine("    public partial class {0}SelectCriteria : EntitySelectCriteria", table.TableName);
            writer.WriteLine("    {");
            writer.WriteLine("        public {0}SelectCriteria()", table.TableName);
            writer.WriteLine("        : base(\"{0}\")", table.DatabaseTableName);
            writer.WriteLine("        {}");
            writer.WriteLine("        public {0}SelectCriteria({0}SelectCriteria other)", table.TableName);
            writer.WriteLine("        : base(other)");
            writer.WriteLine("        {}");
            writer.WriteLine("        public override object Clone()");
            writer.WriteLine("        {");
            writer.WriteLine("            return new {0}SelectCriteria(this);", table.TableName);
            writer.WriteLine("        }");

            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
					string colType = col.ColumnName.EndsWith("Enum") ? col.ColumnName : col.DeclaringType;
                    string colName = col.DatabaseColumnName;
                    writer.WriteLine("        [EntityFieldDatabaseMappingAttribute(TableName=\"{0}\", ColumnName=\"{1}\")]", table.DatabaseTableName, col.DatabaseColumnName.Replace("Key", "GUID"));
                    writer.WriteLine("        public ISearchCondition<{0}> {1}", colType, col.ColumnName);
                    writer.WriteLine("        {");
                    writer.WriteLine("            get");
                    writer.WriteLine("            {");
                    writer.WriteLine("              if (!SubCriteria.ContainsKey(\"{0}\"))", colName);
                    writer.WriteLine("              {");
                    writer.WriteLine("                 SubCriteria[\"{0}\"] = new SearchCondition<{1}>(\"{0}\");", colName, colType);
                    writer.WriteLine("              }");
                    writer.WriteLine("              return (ISearchCondition<{0}>)SubCriteria[\"{1}\"];", colType, colName);
                    writer.WriteLine("            } ");
                    writer.WriteLine("        }");
                }
            }
            writer.WriteLine("    }");

            WriteFooter(writer);

            writer.Close();
        }

        private void WriteEntityUpdateColumnsFile(Table table)
        {
            String fileName = String.Format("{0}UpdateColumns.gen.cs", table.TableName);
            Console.WriteLine("Writing {0}", fileName);

            StreamWriter writer = new StreamWriter(Path.Combine(EntityInterfaceFolder, fileName));

            WriterHeader(writer, EntityInterfaceNamespace);

            bool bDicomReference = false;
            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
                    DicomTag tag = DicomTagDictionary.GetDicomTag(col.ColumnName);
                    if (tag != null)
                    {
                        bDicomReference = true;
                        break;
                    }
                }
            }

            writer.WriteLine("    using System;");
            writer.WriteLine("    using System.Xml;");
            if (bDicomReference)
                writer.WriteLine("    using ClearCanvas.Dicom;");

            writer.WriteLine("    using ClearCanvas.ImageServer.Enterprise;");
            writer.WriteLine("");

            writer.WriteLine("   public partial class {0}UpdateColumns : EntityUpdateColumns", table.TableName);
            writer.WriteLine("   {");
            writer.WriteLine("       public {0}UpdateColumns()", table.TableName);
            writer.WriteLine("       : base(\"{0}\")", table.DatabaseTableName);
            writer.WriteLine("       {}");

            foreach (Column col in table.Columns)
            {
                if (!col.ColumnName.Equals("Key"))
                {
					string colType = col.ColumnName.EndsWith("Enum") ? col.ColumnName : col.DeclaringType;
                    string colName = col.ColumnName;
                    DicomTag tag = DicomTagDictionary.GetDicomTag(col.ColumnName);
                    if (tag != null)
                        writer.WriteLine("       [DicomField(DicomTags.{0}, DefaultValue = DicomFieldDefault.Null)]", colName);
                    writer.WriteLine("        [EntityFieldDatabaseMappingAttribute(TableName=\"{0}\", ColumnName=\"{1}\")]", table.DatabaseTableName, col.DatabaseColumnName.Replace("Key", "GUID"));
                    writer.WriteLine("        public {0} {1}", colType, colName);
                    writer.WriteLine("        {");
                    writer.WriteLine(
                        "            set {{ SubParameters[\"{0}\"] = new EntityUpdateColumn<{1}>(\"{0}\", value); }}", colName,
                        colType);
                    writer.WriteLine("        }");
                }
            }
            writer.WriteLine("    }");

            WriteFooter(writer);

            writer.Close();
        }

        public void Generate()
        {
            LoadTableInfo();

            foreach (Table table in TableList)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Working with table: {0}", table.TableName);
                Console.WriteLine("");

                if (table.TableName.EndsWith("Enum"))
                {
                    WriteEnumBrokerInterfaceFile(table);
                    WriteEnumBrokerImplementationFile(table);
                    WriteEnumFile(table);
                }
                else
                {
                    WriteEntityBrokerInterfaceFile(table);
                    WriteEntitySelectCriteriaFile(table);
                    WriteEntityUpdateColumnsFile(table);
                    WriteEntityBrokerImplementationFile(table);

                    WriteModelFile(table);
                }
            }

            if (GenerateResxFile)
            {
                GenerateResx(Path.Combine(ImageServerModelFolder, "ServerEnumDescriptions"));
            }

            Console.WriteLine("Done!");
        }

        public void GenerateResx(string baseName)
        {
            var generator = new ResxGenerator(){ ConnectionStringName = ConnectionStringName };
            string filename = string.Format("{0}.resx", baseName);

            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                TextWriter writer = new StreamWriter(stream);
                generator.GenerateResxFile(writer);
            }
        }

    }

}