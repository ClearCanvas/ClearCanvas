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
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Editing.Common.Commands;
using ClearCanvas.ImageServer.Common.ExternalRequest;
using ClearCanvas.ImageServer.Common.WorkQueue;

namespace ClearCanvas.ImageServer.Common
{
    public class ImageServerSerializer
    {
        private class CustomHook : IJsmlSerializerHook
        {
            private readonly Dictionary<string, Type> _contractMap;

            public CustomHook()
            {
                _contractMap = (from p in Platform.PluginManager.Plugins
                                from t in p.Assembly.Resolve().GetTypes()
                                let a = AttributeUtils.GetAttribute<ImageServerExternalRequestTypeAttribute>(t)
                                where (a != null)
                                select new { a.ContractId, Contract = t })
                    .ToDictionary(entry => entry.ContractId, entry => entry.Contract);

                EditTypeAttribute attribute;
                foreach (var type in Condition.GetConditionTypes().Where(AttributeUtils.HasAttribute<EditTypeAttribute>))
                {
                    attribute = AttributeUtils.GetAttribute<EditTypeAttribute>(type);
                    _contractMap.Add(attribute.ContractId, type);
                }
                foreach (var type in Edit.GetEditTypes().Where(AttributeUtils.HasAttribute<EditTypeAttribute>))
                {
                    attribute = AttributeUtils.GetAttribute<EditTypeAttribute>(type);
                    _contractMap.Add(attribute.ContractId, type);
                }

                var pathAndVr = typeof (PathAndVr);
                attribute = AttributeUtils.GetAttribute<EditTypeAttribute>(pathAndVr);
                _contractMap.Add(attribute.ContractId, pathAndVr);

                var dateTimePair = typeof(DateTimePair);
                attribute = AttributeUtils.GetAttribute<EditTypeAttribute>(dateTimePair);
                _contractMap.Add(attribute.ContractId, dateTimePair);
            }

            #region IJsmlSerializerHook

            bool IJsmlSerializerHook.Serialize(IJsmlSerializationContext context)
            {
                var data = context.Data;
                if (data != null)
                {
                    // if we have an attribute, write out the contract ID as an XML attribute
                    var a = AttributeUtils.GetAttribute<ImageServerExternalRequestTypeAttribute>(data.GetType());
                    if (a != null)
                    {
                        context.Attributes.Add("contract", a.ContractId);
                    }
                    else
                    {
                        var b = AttributeUtils.GetAttribute<EditTypeAttribute>(data.GetType());
                        if (b != null)
                        {
                            context.Attributes.Add("contract", b.ContractId);
                        }
                    }
                }

                // always return false - we don't handle serialization ourselves
                return false;
            }

            bool IJsmlSerializerHook.Deserialize(IJsmlDeserializationContext context)
            {
                // if we have an XML attribute for the contract ID, change the data type to use the correct contract
                var contract = context.XmlElement.GetAttribute("contract");
                if (!string.IsNullOrEmpty(contract))
                {
                    // constrain the data type by the contract id
                    context.DataType = GetDataContract(contract);
                }

                // always return false - we don't handle serialization ourselves
                return false;
            }

            #endregion

            private Type GetDataContract(string contractId)
            {
                Type contract;
                if (!_contractMap.TryGetValue(contractId, out contract))
                    throw new ArgumentException("Invalid data contract ID.");

                return contract;
            }
        }

        #region Private Static Members

        private static readonly IJsmlSerializerHook ExternalRequestHook;
        private static readonly IJsmlSerializerHook WorkItemDataHook;

        static ImageServerSerializer()
        {
            ExternalRequestHook = new CustomHook();
            WorkItemDataHook = new PolymorphicDataContractHook<WorkQueueDataTypeAttribute>();
        }

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
                new JsmlSerializer.SerializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }
        public static XmlDocument SerializeNotificationToXmlDocument(ImageServerNotification data)
        {
            var s = JsmlSerializer.Serialize(data, "ImageServerNotification",
                new JsmlSerializer.SerializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });

            var d = new XmlDocument();
            d.LoadXml(s);
            return d;
        }

        public static ImageServerNotification DeserializeNotification(string data)
        {
            return JsmlSerializer.Deserialize<ImageServerNotification>(data,
                new JsmlSerializer.DeserializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }

        public static ImageServerNotification DeserializeNotification(XmlDocument data)
        {
            return JsmlSerializer.Deserialize<ImageServerNotification>(data,
                new JsmlSerializer.DeserializeOptions { Hook = ExternalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
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
     
        private static bool IsImageServerExternalRequestContract(Type t)
        {
            return AttributeUtils.HasAttribute<ImageServerExternalRequestTypeAttribute>(t) ||
                   AttributeUtils.HasAttribute<EditTypeAttribute>(t);
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
