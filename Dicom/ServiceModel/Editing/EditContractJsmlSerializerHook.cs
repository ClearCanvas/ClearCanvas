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
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.ServiceModel.Editing
{
	public class EditContractJsmlSerializerHook : IJsmlSerializerHook
	{
		private readonly Dictionary<string, Type> _contractMap;

		public EditContractJsmlSerializerHook()
		{
			_contractMap = (from p in Platform.PluginManager.Plugins
			                from t in p.Assembly.Resolve().GetTypes()
			                let a = AttributeUtils.GetAttribute<EditTypeAttribute>(t)
			                where (a != null)
			                select new {a.ContractId, Contract = t})
				.ToDictionary(entry => entry.ContractId, entry => entry.Contract);
		}

		#region IJsmlSerializerHook

		bool IJsmlSerializerHook.Serialize(IJsmlSerializationContext context)
		{
			SetDataType(context);
			return false;
		}

		bool IJsmlSerializerHook.Deserialize(IJsmlDeserializationContext context)
		{
			var type = GetDataType(context);
			if (type != null)
				context.DataType = type;

			return false;
		}

		#endregion

		public Type GetDataType(IJsmlDeserializationContext context)
		{
			// if we have an XML attribute for the contract ID, change the data type to use the correct contract
			var contract = context.XmlElement.GetAttribute("contract");
			if (!string.IsNullOrEmpty(contract))
			{
				// constrain the data type by the contract id
				return GetDataContract(contract);
			}

			contract = context.XmlElement.GetAttribute("attributeType");
			if (!String.IsNullOrEmpty(contract))
			{
				var type = TypeHelper.GetKnownAttributeValueTypes().FirstOrDefault(t => t.Name == contract);
				if (type != null)
					return type;
			}

			return null;
		}

		public bool SetDataType(IJsmlSerializationContext context)
		{
			var data = context.Data;
			if (data != null)
			{
				var dataType = data.GetType();
				// if we have an attribute, write out the contract ID as an XML attribute
				var b = AttributeUtils.GetAttribute<EditTypeAttribute>(dataType);
				if (b != null)
				{
					context.Attributes.Add("contract", b.ContractId);
					return true;
				}
				
				if (TypeHelper.GetKnownAttributeValueTypes().Contains(dataType))
				{
					context.Attributes.Add("attributeType", dataType.Name);
					return true;
				}
			}

			return false;
		}

		private Type GetDataContract(string contractId)
		{
			Type contract;
			if (!_contractMap.TryGetValue(contractId, out contract))
				throw new ArgumentException("Invalid data contract ID.");

			return contract;
		}
	}
}
