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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
	/// <summary>
	/// Provides dynamic-dispatch functionality for web-services.
	/// </summary>
	public static class ShimUtil
	{
		/// <summary>
		/// Returns the set of operations defined by the service contract.
		/// </summary>
		/// <param name="serviceContractTypeName"></param>
		/// <returns></returns>
		public static string[] GetOperationNames(string serviceContractTypeName)
		{
			Type contract = Type.GetType(serviceContractTypeName, true);

			return CollectionUtils.Map<MethodInfo, string>(
				CollectionUtils.Select(contract.GetMethods(), IsServiceOperation),
				delegate(MethodInfo m) { return m.Name; }).ToArray(); 
		}

		/// <summary>
		/// Invokes the specified operation on the specified service contract, passing the specified JSML-encoded request object.
		/// </summary>
		/// <param name="serviceContractTypeName"></param>
		/// <param name="operationName"></param>
		/// <param name="jsmlRequest"></param>
		/// <returns></returns>
		public static string InvokeOperation(string serviceContractTypeName, string operationName, string jsmlRequest)
		{
			Type contract = Type.GetType(serviceContractTypeName);
			MethodInfo operation = contract.GetMethod(operationName);
			ParameterInfo[] parameters = operation.GetParameters();
			if (parameters.Length != 1)
				throw new InvalidOperationException("Can only invoke methods with exactly one input parameter.");

			object service = null;
			try
			{
				service = Platform.GetService(contract);

				object innerRequest = JsmlSerializer.Deserialize(parameters[0].ParameterType, jsmlRequest);
				object innerResponse = operation.Invoke(service, new object[] { innerRequest });

				return JsmlSerializer.Serialize(innerResponse, "responseData");
			}
			finally
			{
				if (service != null && service is IDisposable)
					(service as IDisposable).Dispose();
			}
		}

		private static bool IsServiceOperation(MethodInfo method)
		{
			return AttributeUtils.HasAttribute<OperationContractAttribute>(method, false);
		}
	}
}
