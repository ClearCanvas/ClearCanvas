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
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	internal class Serializer
	{
		private static readonly DataContractSerializer _workItemRequestSerializer = CreateDataContractSerializer<WorkItemRequest, WorkItemRequestDataContractAttribute>();
		private static readonly DataContractSerializer _workItemProgressSerializer = CreateDataContractSerializer<WorkItemProgress, WorkItemProgressDataContractAttribute>();

		private static readonly IJsmlSerializerHook _workItemRequestHook = new PolymorphicDataContractHook<WorkItemRequestDataContractAttribute>();
		private static readonly IJsmlSerializerHook _workItemProgressHook = new PolymorphicDataContractHook<WorkItemProgressDataContractAttribute>();
		private static readonly IJsmlSerializerHook _serverExtensionDataHook = new PolymorphicDataContractHook<ServerDataContractAttribute>();

		public static string SerializeWorkItemRequest(WorkItemRequest data)
		{
			if (data == null) return null;

			var sb = new StringBuilder();
			using (var sw = XmlWriter.Create(sb))
			{
				_workItemRequestSerializer.WriteObject(sw, data);
			}
			return sb.ToString();
		}

		public static WorkItemRequest DeserializeWorkItemRequest(string data)
		{
			if (string.IsNullOrEmpty(data)) return null;

			try
			{
				using (var tr = new StringReader(data))
				using (var sr = XmlReader.Create(tr))
					return (WorkItemRequest) _workItemRequestSerializer.ReadObject(sr);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Unable to deserialize work item request, retrying with legacy JSML format");
			}

			return JsmlSerializer.Deserialize<WorkItemRequest>(data,
			                                                   new JsmlSerializer.DeserializeOptions {Hook = _workItemRequestHook, DataContractTest = IsWorkItemRequestContract});
		}

		public static string SerializeWorkItemProgress(WorkItemProgress data)
		{
			if (data == null) return null;

			var sb = new StringBuilder();
			using (var sw = XmlWriter.Create(sb))
			{
				_workItemProgressSerializer.WriteObject(sw, data);
			}
			return sb.ToString();
		}

		public static WorkItemProgress DeserializeWorkItemProgress(string data)
		{
			if (string.IsNullOrEmpty(data)) return null;

			try
			{
				using (var tr = new StringReader(data))
				using (var sr = XmlReader.Create(tr))
					return (WorkItemProgress) _workItemProgressSerializer.ReadObject(sr);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Unable to deserialize work item progress, retrying with legacy JSML format");
			}

			return JsmlSerializer.Deserialize<WorkItemProgress>(data,
			                                                    new JsmlSerializer.DeserializeOptions {Hook = _workItemProgressHook, DataContractTest = IsWorkItemProgressContract});
		}

		private static bool IsWorkItemProgressContract(Type t)
		{
			return AttributeUtils.HasAttribute<WorkItemProgressDataContractAttribute>(t);
		}

		private static bool IsWorkItemRequestContract(Type t)
		{
			return AttributeUtils.HasAttribute<WorkItemRequestDataContractAttribute>(t);
		}

		private static bool IsServerExtensionDataContract(Type t)
		{
			return AttributeUtils.HasAttribute<ServerDataContractAttribute>(t);
		}

		public static string SerializeServerExtensionData(Dictionary<string, object> serverExtensionData)
		{
			return JsmlSerializer.Serialize(serverExtensionData, "data",
			                                new JsmlSerializer.SerializeOptions {Hook = _serverExtensionDataHook, DataContractTest = IsServerExtensionDataContract});
		}

		public static Dictionary<string, object> DeserializeServerExtensionData(string data)
		{
			return JsmlSerializer.Deserialize<Dictionary<string, object>>(data,
			                                                              new JsmlSerializer.DeserializeOptions {Hook = _serverExtensionDataHook, DataContractTest = IsServerExtensionDataContract});
		}

		private static DataContractSerializer CreateDataContractSerializer<TObject, TContractAttribute>()
			where TContractAttribute : PolymorphicDataContractAttribute
		{
			return new DataContractSerializer(typeof (TObject), null, int.MaxValue, false, false, null, new PolymorphicDataContractResolver<TContractAttribute>());
		}
	}
}