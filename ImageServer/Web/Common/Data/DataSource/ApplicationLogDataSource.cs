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
using ClearCanvas.ImageServer.Core.Query;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	public class ApplicationLogDataSource
	{
		#region Public Delegates
		public delegate void ApplicationLogFoundSetDelegate(IList<ApplicationLog> list);

		public ApplicationLogFoundSetDelegate ApplicationLogFoundSet;
		#endregion

		#region Private Members
		private readonly ApplicationLogController _searchController = new ApplicationLogController();
		private string _host;
		private string _logLevel;
		private string _thread;
		private string _message;
		private string _exception;
		private int _resultCount;
		private IList<ApplicationLog> _list = new List<ApplicationLog>();
		private DateTime? _endDate;
		private DateTime? _startDate;

		#endregion

		#region Public Properties
		public string Host
		{
			get { return _host; }
			set { _host = value; }
		}

		public string LogLevel
		{
			get { return _logLevel; }
			set { _logLevel = value; }
		}
		public string Thread
		{
			get { return _thread; }
			set { _thread = value; }
		}
		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}
		public string Exception
		{
			get { return _exception; }
			set { _exception = value; }
		}
		public DateTime? StartDate
		{
			get { return _startDate; }
			set { _startDate = value; }
		}
		public DateTime? EndDate
		{
			get { return _endDate; }
			set { _endDate = value; }
		}

		public IList<ApplicationLog> List
		{
			get { return _list; }
		}

		public int ResultCount
		{
			get { return _resultCount; }
			set { _resultCount = value; }
		}

		#endregion

		#region Private Methods
		private ApplicationLogSelectCriteria GetSelectCriteria()
		{
			var criteria = new ApplicationLogSelectCriteria();

			if (!String.IsNullOrEmpty(LogLevel))
			{
				string key = LogLevel.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				criteria.LogLevel.Like(key);
			}

            QueryHelper.SetGuiStringCondition(criteria.Thread, Thread);
            QueryHelper.SetGuiStringCondition(criteria.Host, Host);
            QueryHelper.SetGuiStringCondition(criteria.Message, Message);

			if (!String.IsNullOrEmpty(Exception))
			{
				string key = Exception.Replace("*", "%");
				key = key.Replace("?", "_");
				key = "%" + key + "%";
				criteria.Exception.Like(key);
			}

			// Sort with the latest timestamp first
			criteria.Timestamp.SortDesc(0);


			if (StartDate.HasValue && EndDate.HasValue)
			{
				criteria.Timestamp.Between(StartDate.Value, EndDate.Value);
			}
			else if (StartDate.HasValue)
			{
				criteria.Timestamp.MoreThanOrEqualTo(StartDate.Value);
			}
			else if (EndDate.HasValue)
			{
				criteria.Timestamp.LessThanOrEqualTo(EndDate.Value);
			}

			return criteria;
		}

		#endregion

		#region Public Methods
		public IEnumerable<ApplicationLog> Select(int startRowIndex, int maximumRows)
		{
			if (maximumRows == 0) return new List<ApplicationLog>();

			ApplicationLogSelectCriteria criteria = GetSelectCriteria();
			 _list = _searchController.GetRangeLogs(criteria, startRowIndex, maximumRows);

			return _list;
		}

		public int SelectCount()
		{

			ApplicationLogSelectCriteria criteria = GetSelectCriteria();

			ResultCount = _searchController.GetApplicationLogCount(criteria);

			return ResultCount;
		}


		#endregion
	}
}
