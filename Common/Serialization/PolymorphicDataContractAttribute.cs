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
				if(_guid == Guid.Empty)
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
