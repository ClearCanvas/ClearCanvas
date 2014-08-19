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
using System.Linq;
using System.Web;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Core;
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

        public string IssuerOfPatientId { get; set; }

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

		public int RelatedStudies { get; set; }

		public bool QCExpected { get; set; }

        public OrderStatusEnum OrderStatusEnum { get; set; }

        public string OrderStatusEnumString
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

        public ProcedureCode TheRequestedProcedure { get; set; }

        public string StudyInstanceUid { get; set; }

        public string RequestedProcedure { get; set; }

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

		public bool? QCExpected { get; set; }
        #endregion

        #region Private Methods
        private OrderSelectCriteria GetSelectCriteria()
        {
            var criteria = new OrderSelectCriteria();

            // only query for device in this partition
            criteria.ServerPartitionKey.EqualTo(Partition.Key);

            QueryHelper.SetGuiStringCondition(criteria.PatientId, PatientId);
			QueryHelper.SetGuiStringCondition(criteria.PatientsName, PatientName);

            QueryHelper.SetGuiStringCondition(criteria.AccessionNumber, AccessionNumber);

            if (!String.IsNullOrEmpty(ToStudyDate) && !String.IsNullOrEmpty(FromStudyDate))
            {
                var toKey = DateTime.ParseExact(ToStudyDate, DateFormats, null);
                var fromKey = DateTime.ParseExact(FromStudyDate, DateFormats, null);
				criteria.ScheduledDateTime.Between(fromKey, toKey.AddHours(24));
            }
            else if (!String.IsNullOrEmpty(ToStudyDate))
            {
                var toKey = DateTime.ParseExact(ToStudyDate, DateFormats, null);
				criteria.InsertTime.LessThanOrEqualTo(toKey.AddHours(24));
            }
            else if (!String.IsNullOrEmpty(FromStudyDate))
            {
                var fromKey = DateTime.ParseExact(FromStudyDate, DateFormats, null);
                criteria.ScheduledDateTime.MoreThanOrEqualTo(fromKey);
            }

	        if (!string.IsNullOrEmpty(ReferringPhysiciansName))
	        {
		        var staffCriteria = new StaffSelectCriteria();
		        QueryHelper.SetGuiStringCondition(staffCriteria.Name, ReferringPhysiciansName);
		        criteria.ReferringStaffRelatedEntityCondition.Exists(staffCriteria);
	        }

	        if (Statuses != null && Statuses.Length > 0)
            {
                if (Statuses.Length == 1)
                    criteria.OrderStatusEnum.EqualTo(OrderStatusEnum.GetEnum(Statuses[0]));
                else
                {
                    var statusList = Statuses.Select(OrderStatusEnum.GetEnum).ToList();
                    criteria.OrderStatusEnum.In(statusList);
                }
            }

            criteria.ScheduledDateTime.SortDesc(0);

	        if (QCExpected.HasValue)
		        criteria.QCExpected.EqualTo(QCExpected.Value);

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
				_list.Add(OrderSummaryAssembler.CreateOrderSummary(HttpContext.Current.GetSharedPersistentContext(), study));

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

            var orderSummary = new OrderSummary
                {
                    TheRequestedProcedure = ProcedureCode.Load(order.RequestedProcedureCodeKey),
                    TheOrder = order,
                    ScheduledDate = order.ScheduledDateTime,
                    StudyInstanceUid = order.StudyInstanceUid,
                    AccessionNumber = order.AccessionNumber,
                    Room = order.Room,
                    OrderStatusEnum = order.OrderStatusEnum,
                    ReasonForStudy = order.ReasonForStudy,
                    Bed = order.Bed,
                    InsertedTime = order.InsertTime,
                    UpdatedTime = order.UpdatedTime,
                    Key = order.Key,
                    PatientClass = order.PatientClass,
                    PointOfCare = order.PointOfCare,
                    Priority = order.Priority,
					QCExpected = order.QCExpected
                };

            if (order.Priority.Equals("S"))
                orderSummary.Priority = SR.Priority_Stat;
            else if (order.Priority.Equals("A"))
                orderSummary.Priority = SR.Priority_Asap;
            else if (order.Priority.Equals("R"))
                orderSummary.Priority = SR.Priority_Routine;
            else if (order.Priority.Equals("P"))
                orderSummary.Priority = SR.Priority_PreOp;
            else if (order.Priority.Equals("C"))
                orderSummary.Priority = SR.Priority_Callback;
            else if (order.Priority.Equals("T"))
                orderSummary.Priority = SR.Priority_Timing;

            orderSummary.Key = order.GetKey();

            orderSummary.PatientId = order.PatientId;
            orderSummary.PatientsName = order.PatientsName;
            orderSummary.IssuerOfPatientId = order.IssuerOfPatientId;
            orderSummary.RequestedProcedure = orderSummary.TheRequestedProcedure.Text;

            var referStaff = Staff.Load(order.ReferringStaffKey);
            orderSummary.ReferringStaff = referStaff.Name;
            
            var enteredByStaff = Staff.Load(order.EnteredByStaffKey);
            orderSummary.EnteredByStaff = enteredByStaff.Name;
            
           
            orderSummary.ThePartition = ServerPartitionMonitor.Instance.FindPartition(order.ServerPartitionKey) ??
                                        ServerPartition.Load(read, order.ServerPartitionKey);

			// Count of related studies
	        var broker = read.GetBroker<IStudyEntityBroker>();
	        var studySelect = new StudySelectCriteria();
	        studySelect.ServerPartitionKey.EqualTo(order.ServerPartitionKey);
	        studySelect.OrderKey.EqualTo(order.Key);
	        orderSummary.RelatedStudies = broker.Count(studySelect);

            return orderSummary;
        }
    }
}
