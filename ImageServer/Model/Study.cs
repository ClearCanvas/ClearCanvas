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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public partial class Study
    {
        #region Private Fields
        private IDictionary<string, Series> _series;
        private volatile Patient _patient;
        #endregion

        #region Public Properties

        public bool HasAttachment
        {
            get
            {
                if (Series==null)
                    return false;

                return CollectionUtils.Contains(Series.Values, 
                    series=> !String.IsNullOrEmpty(series.Modality) && (series.Modality.Equals("SR") || series.Modality.Equals("DOC")));
            }
        }

        /// <summary>
        /// Gets the <see cref="Series"/> related to this study.
        /// </summary>
        public IDictionary<string, Series> Series
        {
            get
            {
                if (_series == null)
                {
                    lock (SyncRoot)
                    {
                        using (var context = new ServerExecutionContext())
                        {
                            var broker = context.ReadContext.GetBroker<ISeriesEntityBroker>();
                            var criteria = new SeriesSelectCriteria();
                            criteria.StudyKey.EqualTo(Key);
                            IList<Series> list = broker.Find(criteria);

                            _series = new Dictionary<string, Series>();
                            foreach(Series theSeries in list)
                            {
                                _series.Add(theSeries.SeriesInstanceUid, theSeries);
                            }
                        }
                    }
                }

                return _series;
            }
        }

        /// <summary>
        /// Gets the <see cref="Patient"/> related to this study
        /// </summary>
        public Patient Patient
        {
            get
            {
                if (_patient==null)
                {
                    lock (SyncRoot)
                    {
                        _patient = Patient.Load(PatientKey);
                    }
                }
                return _patient;
            }
        }

        #endregion
		

        /// <summary>
        /// Find a <see cref="Study"/> with the specified study instance uid on the given partition.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="studyInstanceUid"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        static public Study Find(IPersistenceContext context, String studyInstanceUid, ServerPartition partition)
        {
            var broker = context.GetBroker<IStudyEntityBroker>();
            var criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(partition.GetKey());
            criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
            Study study = broker.FindOne(criteria);
            return study;
           
        }

		static public Study Find(IPersistenceContext context, String studyInstanceUid, string partitionAe)
		{
			var partitionBroker = context.GetBroker<IServerPartitionEntityBroker>();
			var serverPartitionSelectCriteria = new ServerPartitionSelectCriteria();
			serverPartitionSelectCriteria.AeTitle.EqualTo(partitionAe); //TODO: is EqualTo case sensitive?
			var partition = partitionBroker.FindOne(serverPartitionSelectCriteria);
			if (partition == null)
				return null;

			return Find(context, studyInstanceUid, partition);

		}

        public Patient LoadPatient(IPersistenceContext context)
        {
            if (_patient==null)
            {
                lock (SyncRoot)
                {
                    if (_patient == null)
                    {
                        _patient = Patient.Load(context, PatientKey);
                    }
                }
            }
            return _patient;
        }


        public StudyStorage LoadStudyStorage(IPersistenceContext context)
        {
            return StudyStorage.Load(StudyStorageKey);
        }

        static public Study Find(IPersistenceContext context, ServerEntityKey studyStorageKey)
        {
            var broker = context.GetBroker<IStudyEntityBroker>();
            var criteria = new StudySelectCriteria();
            criteria.StudyStorageKey.EqualTo( studyStorageKey);
            return broker.FindOne(criteria);
        }
    }
}
