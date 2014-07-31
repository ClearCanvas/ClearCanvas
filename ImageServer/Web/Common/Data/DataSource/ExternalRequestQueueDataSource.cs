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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.ExternalRequest;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
    public class ExternalRequestQueueSummary
    {
        #region Private members

        private ServerPartition _thePartition;
        private ExternalRequestQueue _theExternalRequestQueue;
        private ImageServerExternalRequest _request;

        private string _notes;
        
        #endregion Private members

        #region Public Properties

        public DateTime InsertDateTime
        {
            get { return _theExternalRequestQueue.InsertTime; }
        }

		public DateTime ScheduledDateTime
		{
			get { return _theExternalRequestQueue.ScheduledTime; }
		}

        public string StatusString
        {
            get { return ServerEnumDescription.GetLocalizedDescription(_theExternalRequestQueue.ExternalRequestQueueStatusEnum); }
        }
        
        public ServerEntityKey Key
        {
            get { return _theExternalRequestQueue.Key; }
        }

        public ExternalRequestQueue TheExternalRequestQueue
        {
            get { return _theExternalRequestQueue; }
            set { _theExternalRequestQueue = value; }
        }
        public ServerPartition ThePartition
        {
            get { return _thePartition; }
            set { _thePartition = value; }
        }
        public String Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public string OperationToken
        {
            get { return _request.OperationToken; }
        }

        public string RequestType
        {
            get { return _request.RequestTypeString; }
        }

        public string RequestDescription
        {
            get { return _request.RequestDescription; }
        }

        public ImageServerExternalRequest Request
        {
            get { return _request; }
            set { _request = value; }
        }
        #endregion Public Properties
    }

    public class ExternalRequestQueueDataSource
    {
        #region Public Delegates
        public delegate void ExternalRequestQueueFoundSetDelegate(IList<ExternalRequestQueueSummary> list);
        public ExternalRequestQueueFoundSetDelegate ExternalRequestQueueFoundSet;
        #endregion

        #region Private Members
        private readonly ExternalRequestQueueController _searchController = new ExternalRequestQueueController();
        private string _requestType;
        private int _resultCount;
        private ExternalRequestQueueStatusEnum _statusEnum;
        private IList<ExternalRequestQueueSummary> _list = new List<ExternalRequestQueueSummary>();
        private IList<ServerEntityKey> _searchKeys;
        #endregion

        #region Public Properties
        public string RequestType
        {
            get { return _requestType; }
            set { _requestType = value; }
        }

        public ServerPartition Partition { get; set; }

        public ExternalRequestQueueStatusEnum StatusEnum
        {
            get { return _statusEnum; }
            set { _statusEnum = value; }
        }
        public IList<ExternalRequestQueueSummary> List
        {
            get { return _list; }
        }
        public int ResultCount
        {
            get { return _resultCount; }
            set { _resultCount = value; }
        }
        public IList<ServerEntityKey> SearchKeys
        {
            get { return _searchKeys; }
            set { _searchKeys = value; }
        }

        #endregion

        #region Private Methods
        private IList<ExternalRequestQueue> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
        {
            resultCount = 0;

            if (maximumRows == 0) return new List<ExternalRequestQueue>();

            if (SearchKeys != null)
            {
                IList<ExternalRequestQueue> archiveQueueList = new List<ExternalRequestQueue>();
                foreach (ServerEntityKey key in SearchKeys)
                    archiveQueueList.Add(ExternalRequestQueue.Load(key));

                resultCount = archiveQueueList.Count;

                return archiveQueueList;
            }

            var criteria = new ExternalRequestQueueSelectCriteria();
            if (!string.IsNullOrEmpty(RequestType))
                criteria.RequestType.EqualTo(RequestType);

            if (StatusEnum != null)
                criteria.ExternalRequestQueueStatusEnum.EqualTo(StatusEnum);

            criteria.InsertTime.SortDesc(0);

            IList<ExternalRequestQueue> list = _searchController.FindExternalRequestQueue(criteria, startRowIndex,
                                                                                          maximumRows);

            resultCount = _searchController.Count(criteria);

            return list;
        }
        #endregion

        #region Public Methods
        public IEnumerable<ExternalRequestQueueSummary> Select(int startRowIndex, int maximumRows)
        {
            IList<ExternalRequestQueue> list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

            _list = new List<ExternalRequestQueueSummary>();
            foreach (ExternalRequestQueue item in list)
                _list.Add(CreateExternalRequestQueueSummary(item));

            if (ExternalRequestQueueFoundSet != null)
                ExternalRequestQueueFoundSet(_list);

            return _list;
        }

        public int SelectCount()
        {
            if (ResultCount != 0) return ResultCount;

            // Ignore the search results
            InternalSelect(0, 1, out _resultCount);

            return ResultCount;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Constructs an instance of <see cref="ExternalRequestQueueSummary"/> based on a <see cref="ExternalRequestQueue"/> object.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <remark>
        /// </remark>
        private ExternalRequestQueueSummary CreateExternalRequestQueueSummary(ExternalRequestQueue item)
        {
            var summary = new ExternalRequestQueueSummary
                {
                    TheExternalRequestQueue = item, 
                    ThePartition = Partition,
                    Request = ImageServerSerializer.DeserializeExternalRequest(item.RequestXml)
                };
            
            return summary;
        }
        #endregion
    }
}
