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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
    /// <summary>
    /// Reapply Rules Utility class.
    /// </summary>
    public class ReapplyRulesUtility
    {
        #region Private members

        private event EventHandler<StudyEventArgs> _studyProcessedEvent;
        private readonly object _syncLock = new object();
        private readonly ReapplyRulesRequest _request;

        #endregion

        #region Public Events

        public class StudyEventArgs : EventArgs
        {
            public string StudyInstanceUid;
        }

        public event EventHandler<StudyEventArgs> StudyProcessedEvent
        {
            add
            {
                lock (_syncLock)
                {
                    _studyProcessedEvent += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studyProcessedEvent -= value;
                }
            }
        }

        #endregion

        #region Public Properties

        public int DatabaseStudiesToScan { get; private set; }
     
        public List<long> StudyOidList { get; private set; }

        #endregion

        #region Constructors

        public ReapplyRulesUtility(ReapplyRulesRequest request)
        {
            _request = request;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize the Reapply Rules.  Loast a list of studies.
        /// </summary>
        public void Initialize()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetStudyBroker();

                StudyOidList = broker.GetStudyOids();
            }

            DatabaseStudiesToScan = StudyOidList.Count;           
        }

        /// <summary>
        /// Reapply the rules.
        /// </summary>
        public void Process()
        {            
            ProcessStudiesInDatabase();            
        }

        #endregion

        #region Private Methods
    
        private void ProcessStudiesInDatabase()
        {
        	var rulesEngine = RulesEngine.Create();
 
            foreach (var oid in StudyOidList)
            {
                try
                {
                    // TODO (CR Jun 2012): We don't modify any work items - do we need the mutex?
                    using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                    {
                        var broker = context.GetStudyBroker();

                        var study = broker.GetStudy(oid);

                     
                        var studyEntry = study.ToStoreEntry();
                    	var rulesEngineOptions = new RulesEngineOptions
                    	                         	{
                    	                         		ApplyDeleteActions = _request.ApplyDeleteActions,
                    	                         		ApplyRouteActions = _request.ApplyRouteActions
                    	                         	};
						if(!string.IsNullOrEmpty(_request.RuleId))
						{
							rulesEngine.ApplyStudyRule(studyEntry, _request.RuleId, rulesEngineOptions);
						}
						else
						{
							rulesEngine.ApplyStudyRules(studyEntry, rulesEngineOptions);
						}

                        EventsHelper.Fire(_studyProcessedEvent, this, new StudyEventArgs { StudyInstanceUid = study.StudyInstanceUid });
                    }                    
                }
                catch (Exception x)
                {
                    Platform.Log(LogLevel.Warn, "Unexpected exception attempting to reapply rules for StudyOid {0}: {1}", oid, x.Message);
                }
            }
        }

        #endregion
    }
}
