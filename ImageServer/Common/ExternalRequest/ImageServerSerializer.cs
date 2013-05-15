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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.ExternalRequest
{
    public class ImageServerSerializer
    {
        private static readonly IJsmlSerializerHook _externalRequestHook = new PolymorphicDataContractHook<ImageServerExternalRequestTypeAttribute>();
        private static readonly IJsmlSerializerHook _notificationHook = new PolymorphicDataContractHook<ImageServerNotificationTypeAttribute>();

        public static string SerializeExternalRequest(ImageServerExternalRequest data)
        {
            return JsmlSerializer.Serialize(data, "ImageServerExternalRequest",
                new JsmlSerializer.SerializeOptions { Hook = _externalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }

        public static ImageServerExternalRequest DeserializeExternalRequest(string data)
        {
            return JsmlSerializer.Deserialize<ImageServerExternalRequest>(data,
                new JsmlSerializer.DeserializeOptions { Hook = _externalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }

        public static string SerializeExternalRequestState(ImageServerExternalRequestState data)
        {
            return JsmlSerializer.Serialize(data, "ImageServerExternalRequestState",
                new JsmlSerializer.SerializeOptions { Hook = _externalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }

        public static ImageServerExternalRequestState DeserializeExternalRequestState(string data)
        {
            return JsmlSerializer.Deserialize<ImageServerExternalRequestState>(data,
                new JsmlSerializer.DeserializeOptions { Hook = _externalRequestHook, DataContractTest = IsImageServerExternalRequestContract });
        }

        public static string SerializeNotification(ImageServerNotification data)
        {
            return JsmlSerializer.Serialize(data, "ImageServerNotification",
                new JsmlSerializer.SerializeOptions { Hook = _notificationHook, DataContractTest = IsImageServerNotificationContract });
        }
        public static ImageServerNotification DeserializeNotification(string data)
        {
            return JsmlSerializer.Deserialize<ImageServerNotification>(data,
                new JsmlSerializer.DeserializeOptions { Hook = _notificationHook, DataContractTest = IsImageServerNotificationContract });
        }

        private static bool IsImageServerNotificationContract(Type t)
        {
            return AttributeUtils.HasAttribute<ImageServerNotificationTypeAttribute>(t);
        }

        private static bool IsImageServerExternalRequestContract(Type t)
        {
            return AttributeUtils.HasAttribute<ImageServerExternalRequestTypeAttribute>(t);
        }

    }
}
