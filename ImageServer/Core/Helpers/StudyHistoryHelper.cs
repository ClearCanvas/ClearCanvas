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
using System.Linq;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Helpers
{
    public static class StudyHistoryHelper
    {
        /// <summary>
        /// Finds a list of <see cref="StudyHistory"/> records of the specified <see cref="StudyHistoryTypeEnum"/> 
        /// for the specified <see cref="StudyStorage"/>.
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <returns></returns>
        /// <param name="types"></param>
        static public IList<StudyHistory> FindStudyHistories(StudyStorage studyStorage, IEnumerable<StudyHistoryTypeEnum> types)
        {
            // Use of ExecutionContext to re-use db connection if possible
            using (ServerExecutionContext scope = new ServerExecutionContext())
            {
                IStudyHistoryEntityBroker broker = scope.PersistenceContext.GetBroker<IStudyHistoryEntityBroker>();
                StudyHistorySelectCriteria criteria = new StudyHistorySelectCriteria();
                criteria.StudyStorageKey.EqualTo(studyStorage.Key);
                criteria.StudyHistoryTypeEnum.EqualTo(StudyHistoryTypeEnum.StudyReconciled);

                if (types != null)
                {
                    criteria.StudyHistoryTypeEnum.In(types);
                }

                criteria.InsertTime.SortAsc(0);
                IList<StudyHistory> historyList = broker.Find(criteria);
                return historyList;
            }
        }

        /// <summary>
        /// Finds all <see cref="StudyHistory"/> records for the specified <see cref="StudyStorage"/>.
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <returns></returns>
        static public IList<StudyHistory> FindStudyHistories(StudyStorage studyStorage)
        {
            return FindStudyHistories(studyStorage, null);
        }

        public static StudyHistory CreateStudyHistoryRecord(IUpdateContext updateContext,
                                                            StudyStorageLocation primaryStudyLocation,
                                                            StudyStorageLocation secondaryStudyLocation,
                                                            StudyHistoryTypeEnum type, object entryInfo,
                                                            object changeLog)
        {
            var columns = new StudyHistoryUpdateColumns
                {
                    InsertTime = Platform.Time,
                    StudyHistoryTypeEnum = type,
                    StudyStorageKey = primaryStudyLocation.GetKey(),
                    DestStudyStorageKey =
                        secondaryStudyLocation != null
                            ? secondaryStudyLocation.GetKey()
                            : primaryStudyLocation.GetKey(),
                    StudyData = XmlUtils.SerializeAsXmlDoc(entryInfo) ?? new XmlDocument(),
                    ChangeDescription = XmlUtils.SerializeAsXmlDoc(changeLog) ?? new XmlDocument()
                };

            var broker = updateContext.GetBroker<IStudyHistoryEntityBroker>();
            return broker.Insert(columns);
        }
    }
}
