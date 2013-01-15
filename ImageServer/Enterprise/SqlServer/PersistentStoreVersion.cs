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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    [Serializable]
    public class PersistentStoreVersion: ServerEntity
    {
        #region Constructors
        public PersistentStoreVersion():base("DatabaseVersion_")
        {}
        public PersistentStoreVersion(
            String _build__
            ,String _major__
            ,String _minor__
            ,String _revision__
            ):base("DatabaseVersion_")
        {
            _build_ = _build__;
            _major_ = _major__;
            _minor_ = _minor__;
            _revision_ = _revision__;
        }
        #endregion

        #region Private Members
        private String _build_;
        private String _major_;
        private String _minor_;
        private String _revision_;
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMapping(TableName="DatabaseVersion_", ColumnName="Build_")]
        public String Build
        {
            get { return _build_; }
            set { _build_ = value; }
        }
        [EntityFieldDatabaseMapping(TableName="DatabaseVersion_", ColumnName="Major_")]
        public String Major
        {
            get { return _major_; }
            set { _major_ = value; }
        }
        [EntityFieldDatabaseMapping(TableName="DatabaseVersion_", ColumnName="Minor_")]
        public String Minor
        {
            get { return _minor_; }
            set { _minor_ = value; }
        }
        [EntityFieldDatabaseMapping(TableName="DatabaseVersion_", ColumnName="Revision_")]
        public String Revision
        {
            get { return _revision_; }
            set { _revision_ = value; }
        }
        #endregion

        static public PersistentStoreVersion Insert(PersistentStoreVersion entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                PersistentStoreVersion newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public PersistentStoreVersion Insert(IUpdateContext update, PersistentStoreVersion entity)
        {
            IPersistentStoreVersionEntityBroker broker = update.GetBroker<IPersistentStoreVersionEntityBroker>();
            PersistentStoreVersionUpdateColumns updateColumns = new PersistentStoreVersionUpdateColumns();
            updateColumns.Build = entity.Build;
            updateColumns.Major = entity.Major;
            updateColumns.Minor = entity.Minor;
            updateColumns.Revision = entity.Revision;
            PersistentStoreVersion newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
    }
}