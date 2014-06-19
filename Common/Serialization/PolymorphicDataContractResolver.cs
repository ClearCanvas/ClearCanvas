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
using System.Runtime.Serialization;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Serialization
{
	public class PolymorphicDataContractResolver<T> : DataContractResolver
		where T : PolymorphicDataContractAttribute
	{
// ReSharper disable StaticFieldInGenericType
		private static readonly Dictionary<string, Type> _contractMap = PolymorphicDataContractAttribute.GetContractMap(typeof (T));
// ReSharper restore StaticFieldInGenericType

		private const string _contractNamespace = "http://www.clearcanvas.ca";
		private const string _contractNamePrefix = "c_";
		private readonly int _contractNamePrefixLength = _contractNamePrefix.Length;

		public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
		{
			var a = AttributeUtils.GetAttribute<T>(type);
			if (a != null)
			{
				var dictionary = new XmlDictionary();
				typeName = dictionary.Add(_contractNamePrefix + a.ContractId);
				typeNamespace = dictionary.Add(_contractNamespace);
				return true;
			}
			return knownTypeResolver.TryResolveType(type, declaredType, knownTypeResolver, out typeName, out typeNamespace);
		}

		public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
		{
			if (typeNamespace == _contractNamespace && typeName.StartsWith(_contractNamePrefix))
			{
				var contract = typeName.Substring(_contractNamePrefixLength);
				if (!string.IsNullOrEmpty(contract))
				{
					// get the data type by the contract id
					return GetDataContract(contract);
				}
			}
			return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, knownTypeResolver);
		}

		private static Type GetDataContract(string contractId)
		{
			Type contract;
			if (!_contractMap.TryGetValue(contractId, out contract))
				throw new ArgumentException("Invalid data contract ID.");

			return contract;
		}
	}
}