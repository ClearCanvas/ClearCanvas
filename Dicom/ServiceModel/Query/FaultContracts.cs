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

using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// The namespace for all the query data and service contracts.
	/// </summary>
	public class QueryNamespace
	{
		/// <summary>
		/// The namespace for all the query data and service contracts.
		/// </summary>
		public const string Value = DicomNamespace.Value + "/query";
	}

	/// <summary>
	/// Data contract for 'query failed' faults.
	/// </summary>
	[DataContract(Namespace = QueryNamespace.Value)]
	public class QueryFailedFault
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public QueryFailedFault()
		{
		}

		/// <summary>
		/// A textual description of the query failure.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Description;
	}

	/// <summary>
	/// Data contract for 'retrieve failed' faults.
	/// </summary>
	[DataContract(Namespace = QueryNamespace.Value)]
	public class RetrieveFailedFault
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public RetrieveFailedFault()
		{
		}

		/// <summary>
		/// A textual description of the retrieve failure.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Description;
	}

	/// <summary>
	/// Data contract for data validation faults; when the data in the request is poorly formatted or incorrect.
	/// </summary>
	[DataContract(Namespace = QueryNamespace.Value)]
	public class DataValidationFault
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DataValidationFault()
		{
		}

		/// <summary>
		/// A textual description of the fault.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string Description;
	}
}
