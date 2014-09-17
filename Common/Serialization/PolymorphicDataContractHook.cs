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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Serialization
{
	public class PolymorphicDataContractHook<T> : IJsmlSerializerHook
		where T : PolymorphicDataContractAttribute
	{
// ReSharper disable StaticFieldInGenericType
		private static readonly Dictionary<string, Type> _contractMap = PolymorphicDataContractAttribute.GetContractMap(typeof (T));
// ReSharper restore StaticFieldInGenericType

		public static void RegisterKnownType(Type type)
		{
			var a = AttributeUtils.GetAttribute<T>(type);
			if (a == null)
				throw new ArgumentException(string.Format("Specified type must be decorated with {0}", typeof (T).FullName));
			_contractMap.Add(a.ContractId, type);
		}

		public static IEnumerable<Type> DataContracts
		{
			get { return _contractMap.Values; }
		}

		#region IJsmlSerializerHook

		bool IJsmlSerializerHook.Serialize(IJsmlSerializationContext context)
		{
			var data = context.Data;
			if (data != null)
			{
				// if we have an attribute, write out the contract ID as an XML attribute
				var a = AttributeUtils.GetAttribute<T>(data.GetType());
				if (a != null)
				{
					context.Attributes.Add("contract", a.ContractId);
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

		private static Type GetDataContract(string contractId)
		{
			Type contract;
			if (!_contractMap.TryGetValue(contractId, out contract))
				throw new ArgumentException("Invalid data contract ID.");

			return contract;
		}

		#region

#if UNIT_TESTS

		internal static void ClearKnownTypes()
		{
			_contractMap.Clear();
		}

#endif

		#endregion
	}
}