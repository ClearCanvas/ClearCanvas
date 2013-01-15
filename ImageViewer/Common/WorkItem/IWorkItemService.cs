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
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public static class ImageViewerWorkItemNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/workitem";
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemInsertRequest : DataContractBase
    {
        [DataMember]
        public WorkItemRequest Request { get; set; }

        [DataMember]
        public WorkItemProgress Progress { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemInsertResponse : DataContractBase
    {
        [DataMember]
        public WorkItemData Item { get; set; }
    }

    /// TODO (CR Jun 2012): This seems overloaded and it can be ambiguous what the intent
    /// is if multiple fields are set. I wonder if we should just have a method on IWorkItemService for
    /// each kind of thing you can change (e.g. SetPriority, Cancel...).
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemUpdateRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public long Identifier { get; set; }

        [DataMember]
        public WorkItemPriorityEnum? Priority { get; set; }

        [DataMember]
        public WorkItemStatusEnum? Status { get; set; }

        [DataMember]
        public DateTime? ProcessTime { get; set; }

        [DataMember]
        public DateTime? ExpirationTime { get; set; }

        [DataMember]
        public bool? Cancel { get; set; }

        [DataMember]
        public bool? Delete { get; set; }

    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemUpdateResponse : DataContractBase
    {
        [DataMember]
        public WorkItemData Item { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemQueryRequest : DataContractBase
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public WorkItemStatusEnum? Status { get; set; }

        [DataMember]
        public string StudyInstanceUid { get; set; }

        [DataMember]
        public long? Identifier { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemQueryResponse : DataContractBase
    {
        [DataMember]
        public IEnumerable<WorkItemData> Items { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemSubscribeRequest : DataContractBase
    {
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemSubscribeResponse : DataContractBase
    {        
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemUnsubscribeRequest : DataContractBase
    {
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemUnsubscribeResponse : DataContractBase
    {
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemRefreshRequest : DataContractBase
    {
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemRefreshResponse : DataContractBase
    {
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemPublishRequest : DataContractBase
    {
        // TODO (CR Jun 2012): We can only publish changes to single items, but the callback accepts an array?
        [DataMember]
        public WorkItemData Item { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    public class WorkItemPublishResponse : DataContractBase
    {
    }

    [ServiceContract(SessionMode = SessionMode.Required,
                        CallbackContract = typeof(IWorkItemActivityCallback),
                        ConfigurationName = "IWorkItemActivityMonitorService",
                        Namespace = ImageViewerWorkItemNamespace.Value)]
    [ServiceKnownType("GetKnownTypes", typeof(WorkItemRequestTypeProvider))]
    public interface IWorkItemActivityMonitorService
    {
        [OperationContract]
        WorkItemSubscribeResponse Subscribe(WorkItemSubscribeRequest request);
        
        [OperationContract]
        WorkItemUnsubscribeResponse Unsubscribe(WorkItemUnsubscribeRequest request);

        [OperationContract(IsOneWay = true)]
        void Refresh(WorkItemRefreshRequest request);

        // TODO (CR Jun 2012): this should be renamed "PublishWorkedItemChanged". Still wish we could get rid of it.
        [OperationContract]
        WorkItemPublishResponse Publish(WorkItemPublishRequest request);
    }

    /// <summary>
    /// Service for the creation, manipulation, and monitoring of WorkItems.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, 
        ConfigurationName = "IWorkItemService",
        Namespace = ImageViewerWorkItemNamespace.Value)]
    [ServiceKnownType("GetKnownTypes", typeof(WorkItemRequestTypeProvider))]
    public interface IWorkItemService
    {
        [OperationContract]
        WorkItemInsertResponse Insert(WorkItemInsertRequest request);

        [OperationContract]
        WorkItemUpdateResponse Update(WorkItemUpdateRequest request);

        [OperationContract]
        WorkItemQueryResponse Query(WorkItemQueryRequest request);
    }
}
