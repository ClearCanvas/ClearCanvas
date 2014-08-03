﻿#region License

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

namespace ClearCanvas.Common.Serialization
{
	public class PolymorphicDataContractException : Exception
	{
		public PolymorphicDataContractException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}


	/// <summary>
	/// Assigns a GUID to a class to enable robust polymorphic de/serialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public abstract class PolymorphicDataContractAttribute : Attribute
	{
		// This is map of (Attribute type -> (contract ID -> contract type))
		private static readonly Dictionary<Type, Dictionary<string, Type>> _contractMaps;

		static PolymorphicDataContractAttribute()
		{
			try
			{
				// build the contract maps by finding all types having an attribute
				// that is a subclass of PolymorphicDataContractAttribute
				_contractMaps = (from p in Platform.PluginManager.Plugins
								 from t in p.Assembly.Resolve().GetTypes()
								 from a in t.GetCustomAttributes(false)
								 let pa = a as PolymorphicDataContractAttribute
								 where (pa != null)
								 group new { ContractType = t, Attr = pa } by pa.GetType() into g
								 select new
											{
												AttributeType = g.Key,
												ContractMap = g.ToDictionary(x => x.Attr.ContractId, x => x.ContractType)
											}
								)
					.ToDictionary(entry => entry.AttributeType, entry => entry.ContractMap);

			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		/// <summary>
		/// Gets the contract map for a given subclass of <see cref="PolymorphicDataContractAttribute"/>.
		/// </summary>
		/// <param name="polymorphicDataContractAttributeSubclass"></param>
		/// <returns></returns>
		public static Dictionary<string, Type> GetContractMap(Type polymorphicDataContractAttributeSubclass)
		{
			Dictionary<string, Type> contractMap;
			if(!_contractMaps.TryGetValue(polymorphicDataContractAttributeSubclass, out contractMap))
				throw new InvalidOperationException(string.Format("No contract map found for attribute '{0}'",
					polymorphicDataContractAttributeSubclass.FullName));

			return contractMap;
		}



		private readonly string _guidString;
		private Guid _guid;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dataContractGuid"></param>
		protected PolymorphicDataContractAttribute(string dataContractGuid)
		{
			_guidString = dataContractGuid;
		}

		/// <summary>
		/// Gets the ID that identifies the data-contract.
		/// </summary>
		public string ContractId
		{
			get
			{
				if (_guid == Guid.Empty)
				{
					try
					{
						_guid = new Guid(_guidString);
					}
					catch (FormatException e)
					{
						throw new PolymorphicDataContractException(string.Format("{0} is not a valid GUID.", _guidString), e);
					}
				}
				return _guid.ToString("N");
			}
		}
	}

}
