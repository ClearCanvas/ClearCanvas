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

// This file is auto-generated by the ClearCanvas.Model.SqlServer.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using System.Xml;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Enterprise.Command;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class Alert: ServerEntity
    {
        #region Constructors
        public Alert():base("Alert")
        {}
        public Alert(
             DateTime _insertTime_
            ,String _component_
            ,Int32 _typeCode_
            ,String _source_
            ,AlertLevelEnum _alertLevelEnum_
            ,AlertCategoryEnum _alertCategoryEnum_
            ,XmlDocument _content_
            ):base("Alert")
        {
            InsertTime = _insertTime_;
            Component = _component_;
            TypeCode = _typeCode_;
            Source = _source_;
            AlertLevelEnum = _alertLevelEnum_;
            AlertCategoryEnum = _alertCategoryEnum_;
            Content = _content_;
        }
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="Alert", ColumnName="InsertTime")]
        public DateTime InsertTime
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Alert", ColumnName="Component")]
        public String Component
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Alert", ColumnName="TypeCode")]
        public Int32 TypeCode
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Alert", ColumnName="Source")]
        public String Source
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Alert", ColumnName="AlertLevelEnum")]
        public AlertLevelEnum AlertLevelEnum
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Alert", ColumnName="AlertCategoryEnum")]
        public AlertCategoryEnum AlertCategoryEnum
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Alert", ColumnName="Content")]
        public XmlDocument Content
        { get { return _Content; } set { _Content = value; } }
        [NonSerialized]
        private XmlDocument _Content;
        #endregion

        #region Static Methods
        static public Alert Load(ServerEntityKey key)
        {
            using (var context = new ServerExecutionContext())
            {
                return Load(context.ReadContext, key);
            }
        }
        static public Alert Load(IPersistenceContext read, ServerEntityKey key)
        {
            var broker = read.GetBroker<IAlertEntityBroker>();
            Alert theObject = broker.Load(key);
            return theObject;
        }
        static public Alert Insert(Alert entity)
        {
            using (var update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                Alert newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public Alert Insert(IUpdateContext update, Alert entity)
        {
            var broker = update.GetBroker<IAlertEntityBroker>();
            var updateColumns = new AlertUpdateColumns();
            updateColumns.InsertTime = entity.InsertTime;
            updateColumns.Component = entity.Component;
            updateColumns.TypeCode = entity.TypeCode;
            updateColumns.Source = entity.Source;
            updateColumns.AlertLevelEnum = entity.AlertLevelEnum;
            updateColumns.AlertCategoryEnum = entity.AlertCategoryEnum;
            updateColumns.Content = entity.Content;
            Alert newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
