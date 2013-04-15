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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Core.Query;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	public class AlertSummary
	{
		#region Private members

		private Alert _theAlertItem;

		#endregion Private members

		#region Public Properties

		public DateTime InsertTime
		{
			get { return _theAlertItem.InsertTime; }
		}

		public bool LevelIsErrorOrCritical
		{
			get { return _theAlertItem.AlertLevelEnum == AlertLevelEnum.Critical || _theAlertItem.AlertLevelEnum == AlertLevelEnum.Error; }
		}

		public string Level
		{
			get { return ServerEnumDescription.GetLocalizedDescription(_theAlertItem.AlertLevelEnum); }
		}

		public string Category
		{
			get { return ServerEnumDescription.GetLocalizedDescription(_theAlertItem.AlertCategoryEnum); }
		}

		public string Message
		{
			get
			{
                StringBuilder sb = new StringBuilder();
			    sb.AppendLine(_theAlertItem.Content.GetElementsByTagName("Message").Item(0).InnerText);
                return sb.ToString();
			}
		}

	    public object ContextData
	    {
	        get
	        {
                XmlNode contextNode = _theAlertItem.Content.SelectSingleNode("//Context");

                if (contextNode != null)
                {
                    XmlNode node = contextNode.FirstChild;
                    // Cast the Data back from the Abstract Type.
                    if (node != null && node.Attributes!=null && node.Attributes["type"] != null)
                    {
                        Type type = HeuristicTypeResolver.GetType(node.Attributes["type"].Value);
                        if (type == null)
                            return contextNode; // don't know how to decode it

                        XmlSerializer serializer = new XmlSerializer(type);
                        return serializer.Deserialize(new XmlNodeReader(node));
                    }
                    else
                        return node; // don't know how to decode it

                }

	            return null;//
	        }
	    }

		public string Component
		{
			get { return _theAlertItem.Component; }	        
		}

		public string Source
		{
			get { return _theAlertItem.Source; }
		}

		public int TypeCode
		{
			get { return _theAlertItem.TypeCode; }
		}

		public ServerEntityKey Key
		{
			get { return _theAlertItem.Key; }
		}

		public Alert TheAlertItem
		{
			get { return _theAlertItem; }
			set { _theAlertItem = value; }
		}

		#endregion Public Properties
	}

	public class AlertDataSource
	{
		#region Public Delegates
		public delegate void AlertFoundSetDelegate(IList<AlertSummary> list);
		public AlertFoundSetDelegate AlertFoundSet;
		#endregion

		#region Private Members
		private readonly AlertController _alertController = new AlertController();
		private string _insertTime;
		private string _component;
		private AlertLevelEnum _level;
		private AlertCategoryEnum _category;
		private string _dateFormats;
		private int _resultCount;
		private IList<AlertSummary> _list = new List<AlertSummary>();
		private IList<ServerEntityKey> _searchKeys;
		#endregion

		#region Public Properties
		public string InsertTime
		{
			get { return _insertTime; }
			set { _insertTime = value; }
		}
		public string Component
		{
			get { return _component; }
			set { _component = value; }
		}
		public AlertLevelEnum Level
		{
			get { return _level; }
			set { _level = value; }
		}

		public AlertCategoryEnum Category
		{
			get { return _category; }
			set { _category = value; }
		}
	
		public IList<AlertSummary> List
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

		public string DateFormats
		{
			get { return _dateFormats; }
			set { _dateFormats = value; }
		}

		#endregion

		#region Private Methods
		private AlertSelectCriteria GetSelectCriteria()
		{
			var criteria = new AlertSelectCriteria();

            QueryHelper.SetGuiStringCondition(criteria.Component, Component);
          
			if (!String.IsNullOrEmpty(InsertTime))
			{
				DateTime lowerDate = DateTime.ParseExact(InsertTime, DateFormats, null);
				DateTime upperDate = DateTime.ParseExact(InsertTime, DateFormats, null).Add(new TimeSpan(23, 59, 59));
				criteria.InsertTime.Between(lowerDate, upperDate);
			}

			if (Level != null)
				criteria.AlertLevelEnum.EqualTo(Level);
			if (Category != null)
				criteria.AlertCategoryEnum.EqualTo(Category);
			return criteria;
		}

		private IList<Alert> InternalSelect(int startRowIndex, int maximumRows)
		{
			if (maximumRows == 0) return new List<Alert>();

			if (SearchKeys != null)
			{
				IList<Alert> alertList = new List<Alert>();
				foreach (ServerEntityKey key in SearchKeys)
					alertList.Add(Alert.Load(key));

				return alertList;
			}

			AlertSelectCriteria criteria = GetSelectCriteria();

			criteria.InsertTime.SortDesc(0);

			IList<Alert> list = _alertController.GetRangeAlerts(criteria, startRowIndex, maximumRows);

			return list;
		}
		#endregion

		#region Public Methods
		public IEnumerable<AlertSummary> Select(int startRowIndex, int maximumRows)
		{
			IList<Alert> list = InternalSelect(startRowIndex, maximumRows);

			_list = new List<AlertSummary>();
			foreach (Alert item in list)
				_list.Add(CreateAlertSummary(item));

			if (AlertFoundSet != null)
				AlertFoundSet(_list);

			return _list;
		}

		public int SelectCount()
		{
			AlertSelectCriteria criteria = GetSelectCriteria();

			ResultCount = _alertController.GetAlertsCount(criteria);

			return ResultCount;
		}
		#endregion

		#region Private Methods

		private AlertSummary CreateAlertSummary(Alert item)
		{
			AlertSummary summary = new AlertSummary();
			summary.TheAlertItem = item;

			return summary;
		}
		#endregion
	}
}