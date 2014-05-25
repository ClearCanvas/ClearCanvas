#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.Threading;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Authentication;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Query;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
    /// <summary>
    /// Model used in order list grid control <see cref="Order"/>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class OrderSummary
    {
        #region Public Properties

        public ServerEntityKey Key { get; set; }

        public string PatientId { get; set; }

        public string PatientsName { get; set; }

        public DateTime InsertedTime { get; set; }

        public DateTime UpdatedTime { get; set; }

        public DateTime ScheduledDate { get; set; }

        public string AccessionNumber { get; set; }

        public string Priority { get; set; }

        public string PatientClass { get; set; }

        public string ReasonForStudy { get; set; }

        public string PointOfCare { get; set; }

        public string Room { get; set; }

        public string Bed { get; set; }

        public OrderStatusEnum OrderStatusEnum { get; set; }

        public string StudyStatusEnumString
        {
            get
            {
                return ServerEnumDescription.GetLocalizedDescription(OrderStatusEnum);
            }
        }

        public string EnteredByStaff { get; set; }

        public string ReferringStaff { get; set; }

        public Order TheOrder { get; set; }

        public ServerPartition ThePartition { get; set; }

        public Patient ThePatient { get; set; }

        public string StudyInstanceUid { get; set; }

        #endregion Public Properties

        public bool CanScheduleDelete(out string reason)
        {
            if (ThePartition.ServerPartitionTypeEnum.Equals(ServerPartitionTypeEnum.VFS))
            {
                reason = SR.ActionNotAllowed_ResearchPartition;
                return false;
            }

            reason = String.Empty;
            return true;
        }
    }

    public class OrderDataSource
    {
        #region Public Delegates
        public delegate void OrderFoundSetDelegate(IList<OrderSummary> list);

        public OrderFoundSetDelegate OrderFoundSet;
        #endregion

        #region Private Members
        private readonly OrderController _searchController = new OrderController();
        private IList<OrderSummary> _list = new List<OrderSummary>();
        private readonly string STUDYDATE_DATEFORMAT = "yyyyMMdd";

        #endregion

        #region Public Properties

        public string AccessionNumber { get; set; }

        public string PatientId { get; set; }

        public string PatientName { get; set; }

        public string StudyDescription { get; set; }

        public string ToStudyDate { get; set; }

        public string FromStudyDate { get; set; }

        public string ResponsiblePerson { get; set; }

        public string ResponsibleOrganization { get; set; }

        public ServerPartition Partition { get; set; }

        public string DateFormats { get; set; }

        public IList<OrderSummary> List
        {
            get { return _list; }
        }

        public int ResultCount { get; set; }

        public string[] Modalities { get; set; }

        public string ReferringPhysiciansName { get; set; }

        public string[] Statuses { get; set; }

        #endregion

        #region Private Methods
        private OrderSelectCriteria GetSelectCriteria()
        {
            var criteria = new OrderSelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(Partition.Key);

            if (!string.IsNullOrEmpty(PatientName) || !string.IsNullOrEmpty(PatientId))
            {
                var patientCriteria = new PatientSelectCriteria();
                QueryHelper.SetGuiStringCondition(patientCriteria.PatientId, PatientId);
                QueryHelper.SetGuiStringCondition(patientCriteria.PatientsName, PatientName);
                criteria.PatientRelatedEntityCondition.Exists(patientCriteria);
            }

            QueryHelper.SetGuiStringCondition(criteria.AccessionNumber, AccessionNumber);

            if (!String.IsNullOrEmpty(ToStudyDate) && !String.IsNullOrEmpty(FromStudyDate))
            {
                var toKey = DateTime.ParseExact(ToStudyDate, DateFormats, null);
                var fromKey = DateTime.ParseExact(FromStudyDate, DateFormats, null);
                criteria.ScheduledDateTime.Between(fromKey, toKey);
            }
            else if (!String.IsNullOrEmpty(ToStudyDate))
            {
                var toKey = DateTime.ParseExact(ToStudyDate, DateFormats, null);
                criteria.ScheduledDateTime.LessThanOrEqualTo(toKey);
            }
            else if (!String.IsNullOrEmpty(FromStudyDate))
            {
                var fromKey = DateTime.ParseExact(FromStudyDate, DateFormats, null);
                criteria.ScheduledDateTime.MoreThanOrEqualTo(fromKey);
            }


            return criteria;
        }

        #endregion

        #region Public Methods
        public IEnumerable<OrderSummary> Select(int startRowIndex, int maximumRows)
        {
            if (maximumRows == 0 || Partition == null) return new List<OrderSummary>();

            OrderSelectCriteria criteria = GetSelectCriteria();

            IList<Order> studyList = _searchController.GetRangeOrders(criteria, startRowIndex, maximumRows);

            _list = new List<OrderSummary>();

            foreach (Order study in studyList)
                _list.Add(OrderSummaryAssembler.CreateOrderSummary(HttpContextData.Current.ReadContext, study));

            if (OrderFoundSet != null)
                OrderFoundSet(_list);

            return _list;
        }

        public int SelectCount()
        {
            if (Partition == null) return 0;

            OrderSelectCriteria criteria = GetSelectCriteria();

            ResultCount = _searchController.GetOrderCount(criteria);

            return ResultCount;
        }


        #endregion
    }

    public class OrderSummaryAssembler
    {

        /// <summary>
        /// Returns an instance of <see cref="OrderSummary"/> based on a <see cref="Order"/> object.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="read"></param>
        /// <returns></returns>
        /// <remark>
        /// 
        /// </remark>
        static public OrderSummary CreateOrderSummary(IPersistenceContext read, Order order)
        {
            if (order == null)
            {
                return null;
            }

            var orderSummary = new OrderSummary();
            
            orderSummary.ThePatient = Patient.Load(read, order.PatientKey);
            orderSummary.TheOrder = order;

            orderSummary.Key = order.GetKey();
            orderSummary.AccessionNumber = order.AccessionNumber;
            orderSummary.Room = order.Room;
            orderSummary.OrderStatusEnum = order.OrderStatusEnum;
            orderSummary.ReasonForStudy = order.ReasonForStudy;
            orderSummary.Bed = order.Bed;
            orderSummary.InsertedTime = orderSummary.InsertedTime;
            orderSummary.UpdatedTime = orderSummary.UpdatedTime;
            orderSummary.Key = order.Key;
            orderSummary.PatientClass = order.PatientClass;
            orderSummary.PointOfCare = order.PointOfCare;
            orderSummary.Priority = order.Priority;

            var referStaff = Staff.Load(order.ReferringStaffKey);
            orderSummary.ReferringStaff = string.Format("{0}^{1}^{2}^{3}^{4}", referStaff.FamilyName,
                                                        referStaff.GivenName, referStaff.MiddleName, referStaff.Prefix,
                                                        referStaff.Suffix);
            
            var enteredByStaff = Staff.Load(order.EnteredByStaffKey);
            orderSummary.EnteredByStaff = string.Format("{0}^{1}^{2}^{3}^{4}", enteredByStaff.FamilyName,
                                                        enteredByStaff.GivenName, enteredByStaff.MiddleName,
                                                        enteredByStaff.Prefix, enteredByStaff.Suffix);
            
            orderSummary.PatientId = orderSummary.ThePatient.PatientId;
            orderSummary.PatientsName = orderSummary.ThePatient.PatientsName;
            orderSummary.ScheduledDate = order.ScheduledDateTime;
            orderSummary.StudyInstanceUid = order.StudyInstanceUid;
           
            orderSummary.ThePartition = ServerPartitionMonitor.Instance.FindPartition(order.ServerPartitionKey) ??
                                        ServerPartition.Load(read, order.ServerPartitionKey);

         
            return orderSummary;
        }
    }
}
