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
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.ExternalRequest;
using ClearCanvas.ImageServer.Common.WorkQueue;

namespace ClearCanvas.ImageServer.Common
{
    public class ImageServerSerializer
    {
        #region Private Static Members
        private static readonly IJsmlSerializerHook ExternalRequestHook = new PolymorphicDataContractHook<ImageServerExternalRequestTypeAttribute>();
        private static readonly IJsmlSerializerHook NotificationHook = new PolymorphicDataContractHook<ImageServerNotificationTypeAttribute>();
        private static readonly IJsmlSerializerHook WorkItemDataHook = new PolymorphicDataContractHook<WorkQueueDataTypeAttribute>();
        #endregion

        #region ImageServerExternalRequest
        public static string SerializeExternalRequest(ImageServerExternalRequest data)
        {
            return JsmlSerializer.Serialize(data, "ImageServerExternalRequest",
                new JsmlSerializer.SerializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }

        public static XmlDocument SerializeExternalRequestToXmlDocument(ImageServerExternalRequest data)
        {
            var s = JsmlSerializer.Serialize(data, "ImageServerExternalRequest",
                new JsmlSerializer.SerializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });

            var d = new XmlDocument();
            d.LoadXml(s);
            return d;
        }

        public static ImageServerExternalRequest DeserializeExternalRequest(string data)
        {
            return JsmlSerializer.Deserialize<ImageServerExternalRequest>(data,
                new JsmlSerializer.DeserializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }

        public static ImageServerExternalRequest DeserializeExternalRequest(XmlDocument data)
        {
            return JsmlSerializer.Deserialize<ImageServerExternalRequest>(data,
                new JsmlSerializer.DeserializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }
        #endregion

        #region ImageServerExternalRequestState
        public static string SerializeExternalRequestState(ImageServerExternalRequestState data)
        {
            return JsmlSerializer.Serialize(data, "ImageServerExternalRequestState",
                new JsmlSerializer.SerializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }
        public static XmlDocument SerializeExternalRequestStateToXmlDocument(ImageServerExternalRequestState data)
        {
            var s = JsmlSerializer.Serialize(data, "ImageServerExternalRequestState",
                new JsmlSerializer.SerializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });

            var d = new XmlDocument();
            d.LoadXml(s);
            return d;
        }

        public static ImageServerExternalRequestState DeserializeExternalRequestState(string data)
        {
            return JsmlSerializer.Deserialize<ImageServerExternalRequestState>(data,
                new JsmlSerializer.DeserializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }

        public static ImageServerExternalRequestState DeserializeExternalRequestState(XmlDocument data)
        {
            return JsmlSerializer.Deserialize<ImageServerExternalRequestState>(data,
                new JsmlSerializer.DeserializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }
        #endregion

        #region ImageServerNotification
        public static string SerializeNotification(ImageServerNotification data)
        {
            return JsmlSerializer.Serialize(data, "ImageServerNotification",
                new JsmlSerializer.SerializeOptions { Hook = NotificationHook, DataContractTest = IsImageServerNotificationContract });
        }
        public static XmlDocument SerializeNotificationToXmlDocument(ImageServerNotification data)
        {
            var s = JsmlSerializer.Serialize(data, "ImageServerNotification",
                new JsmlSerializer.SerializeOptions { Hook = NotificationHook, DataContractTest = IsImageServerNotificationContract });

            var d = new XmlDocument();
            d.LoadXml(s);
            return d;
        }

        public static ImageServerNotification DeserializeNotification(string data)
        {
            return JsmlSerializer.Deserialize<ImageServerNotification>(data,
                new JsmlSerializer.DeserializeOptions { Hook = NotificationHook, DataContractTest = IsImageServerNotificationContract });
        }

        public static ImageServerNotification DeserializeNotification(XmlDocument data)
        {
            return JsmlSerializer.Deserialize<ImageServerNotification>(data,
                new JsmlSerializer.DeserializeOptions { Hook = NotificationHook, DataContractTest = IsImageServerNotificationContract });
        }
        #endregion

        #region WorkQueueData
        public static string SerializeWorkQueueData(WorkQueueData data)
        {
            return JsmlSerializer.Serialize(data, "WorkQueueData",
                new JsmlSerializer.SerializeOptions { Hook = WorkItemDataHook, DataContractTest = IsWorkQueueDataContract, DataMemberTest = IsXmlSerializationDataMember });
        }

        public static XmlDocument SerializeWorkQueueDataToXmlDocument(WorkQueueData data)
        {
            var s = JsmlSerializer.Serialize(data, "WorkQueueData",
                      new JsmlSerializer.SerializeOptions { Hook = WorkItemDataHook, DataContractTest = IsWorkQueueDataContract, DataMemberTest = IsXmlSerializationDataMember });
            var d = new XmlDocument();
            d.LoadXml(s);
            return d;
        }

        public static WorkQueueData DeserializeWorkQueueData(string data)
        {
            return JsmlSerializer.Deserialize<WorkQueueData>(data,
                new JsmlSerializer.DeserializeOptions { Hook = WorkItemDataHook, DataContractTest = IsWorkQueueDataContract, DataMemberTest = IsXmlSerializationDataMember });
        }

        public static WorkQueueData DeserializeWorkQueueData(XmlDocument data)
        {
            return JsmlSerializer.Deserialize<WorkQueueData>(data,
                new JsmlSerializer.DeserializeOptions { Hook = WorkItemDataHook, DataContractTest = IsWorkQueueDataContract, DataMemberTest = IsXmlSerializationDataMember });
        }
        #endregion

        #region Private Static Methods
        private static bool IsImageServerNotificationContract(Type t)
        {
            return AttributeUtils.HasAttribute<ImageServerNotificationTypeAttribute>(t);
        }

        private static bool IsImageServerExternalRequestContract(Type t)
        {
            return AttributeUtils.HasAttribute<ImageServerExternalRequestTypeAttribute>(t);
        }

        private static bool IsWorkQueueDataContract(Type t)
        {
            return AttributeUtils.HasAttribute<WorkQueueDataTypeAttribute>(t);
        }

        private static bool IsXmlSerializationDataMember(MemberInfo t)
        {
            if (t.IsDefined(typeof(XmlIgnoreAttribute), false))
                return false;

            if (t.MemberType != MemberTypes.Property)
                return false;            

            var pi = t as PropertyInfo;
            if (pi != null && (!pi.CanRead || !pi.CanWrite))
                return false;

            return true;
        }
        #endregion
    }
}
