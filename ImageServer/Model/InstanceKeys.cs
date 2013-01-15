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

using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    public class InstanceKeys : ServerEntity
    {
        #region Constructors
        public InstanceKeys()
            : base("InstanceKeys")
        {
        }
        #endregion

        #region Private Members
        private ServerEntityKey _serverPartitionKey;
		private ServerEntityKey _studyStorageKey;
		private ServerEntityKey _patientKey;
        private ServerEntityKey _studyKey;
        private ServerEntityKey _seriesKey;
        private bool _insertPatient;
        private bool _insertStudy;
        private bool _insertSeries;
        #endregion

        #region Public Properties
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "ServerPartitionGUID")]
		public ServerEntityKey ServerPartitionKey
		{
			get { return _serverPartitionKey; }
			set { _serverPartitionKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "StudyStorageGUID")]
		public ServerEntityKey StudyStorageKey
		{
			get { return _studyStorageKey; }
			set { _studyStorageKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "PatientGUID")]
		public ServerEntityKey PatientKey
		{
			get { return _patientKey; }
			set { _patientKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "StudyGUID")]
		public ServerEntityKey StudyKey
		{
			get { return _studyKey; }
			set { _studyKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "SeriesGUID")]
		public ServerEntityKey SeriesKey
		{
			get { return _seriesKey; }
			set { _seriesKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "InsertPatient")]
		public bool InsertPatient
		{
			get { return _insertPatient; }
			set { _insertPatient = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "InsertStudy")]
		public bool InsertStudy
		{
			get { return _insertStudy; }
			set { _insertStudy = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "InsertSeries")]
		public bool InsertSeries
		{
			get { return _insertSeries; }
			set { _insertSeries = value; }
		}
        #endregion
    }
}
