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
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Represents the institution where the equipment that produced the composite instance is located.
	/// </summary>
	public struct Institution : IEquatable<Institution>
	{
		private string _name;
		private string _address;
		private string _departmentName;

		/// <summary>
		/// Initializes a new instance of <see cref="Institution"/>.
		/// </summary>
		/// <param name="name">The name of the institution.</param>
		/// <param name="address">The mailing address of the institution.</param>
		public Institution(string name, string address)
		{
			_name = name ?? string.Empty;
			_address = address ?? string.Empty;
			_departmentName = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Institution"/>.
		/// </summary>
		/// <param name="name">The name of the institution.</param>
		/// <param name="address">The mailing address of the institution.</param>
		/// <param name="departmentName">The name of the department in the institution.</param>
		public Institution(string name, string address, string departmentName)
		{
			_name = name ?? string.Empty;
			_address = address ?? string.Empty;
			_departmentName = departmentName ?? string.Empty;
		}

		/// <summary>
		/// Gets or sets the name of the institution.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the mailing address of the institution.
		/// </summary>
		public string Address
		{
			get { return _address; }
			set { _address = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the name of the department in the institution.
		/// </summary>
		public string DepartmentName
		{
			get { return _departmentName; }
			set { _departmentName = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets a value indicating whether or not this <see cref="Institution"/> is the empty value.
		/// </summary>
		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(Name) & string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(DepartmentName); }
		}

		public bool Equals(Institution other)
		{
			return Name == other.Name && Address == other.Address && DepartmentName == other.DepartmentName;
		}

		public override bool Equals(object obj)
		{
			return obj is Institution && Equals((Institution) obj);
		}

		public override int GetHashCode()
		{
			return -0x502B77AC ^ Name.GetHashCode() ^ Address.GetHashCode() ^ DepartmentName.GetHashCode();
		}

		/// <summary>
		/// Gets the empty institution.
		/// </summary>
		public static readonly Institution Empty = new Institution();

		/// <summary>
		/// Gets the institution defined in the General Equipment Module of the given <paramref name="dcm">DICOM file</paramref>.
		/// </summary>
		public static Institution GetInstitution(DicomFile dcm)
		{
			return GetInstitution(dcm.DataSet);
		}

		/// <summary>
		/// Gets the institution defined in the General Equipment Module of the given <paramref name="dicomAttributeProvider">dataset</paramref>.
		/// </summary>
		public static Institution GetInstitution(IDicomAttributeProvider dicomAttributeProvider)
		{
			var institution = new Institution();
			var iod = new GeneralEquipmentModuleIod(dicomAttributeProvider);
			institution.Name = iod.InstitutionName ?? string.Empty;
			institution.Address = iod.InstitutionAddress ?? string.Empty;
			institution.DepartmentName = iod.InstitutionalDepartmentName ?? string.Empty;
			return institution;
		}
	}
}