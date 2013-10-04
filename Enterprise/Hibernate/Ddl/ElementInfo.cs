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
using System.Security.Cryptography;
using System.Text;
using ClearCanvas.Common.Utilities;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Defines an element in a relational database model.
	/// </summary>
    public abstract class ElementInfo
	{
		#region Utilities

		/// <summary>
		/// Creates a unique name for the element by combining the prefix, table, and element strings
		/// along with a generated hash that is a function of the table and element.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="table"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		protected static string MakeName(string prefix, string table, string element)
		{
			// NOTE: under some scenarios, the use of MD5 here may cause a FIPS-related exception - see #11283 for details

			// use MD5 to obtain a 32-character hex string that is a unique function of the table and element
			var bytes = Encoding.UTF8.GetBytes(table + element);
			var hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
			var code = BitConverter.ToString(hash).Replace("-", "");

			// return a value that is at most 64 chars long
			return prefix + Truncate(element, 64 - prefix.Length - code.Length) + code;
		}

		/// <summary>
		/// Creates a unique name for the element by combining the prefix, table, and element strings
		/// along with a generated hash that is a function of the table and element.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="table"></param>
		/// <param name="columns"></param>
		/// <returns></returns>
		protected static string MakeName(string prefix, string table, IEnumerable<Column> columns)
		{
			// concat column names into single string
			var columnNames = StringUtilities.Combine(columns, "", c => c.Name);

			return MakeName(prefix, table, columnNames);
		}

		/// <summary>
		/// Creates a unique name for the element by combining the prefix, table, and element strings
		/// along with a generated hash that is a function of the table and element.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="table"></param>
		/// <param name="columns"></param>
		/// <returns></returns>
		protected static string MakeName(string prefix, string table, IEnumerable<ColumnInfo> columns)
		{
			// concat column names into single string
			var columnNames = StringUtilities.Combine(columns, "", c => c.Name);

			return MakeName(prefix, table, columnNames);
		}

		#endregion


		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
        public abstract string Identity { get; }

		/// <summary>
		/// Compares elements by identity.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
        public override bool Equals(object obj)
        {
            var that = obj as ElementInfo;
            if (that == null)
                return false;
            return this.GetType() == that.GetType() && this.Identity == that.Identity;
        }

		/// <summary>
		/// Gets a hash code based on element identity.
		/// </summary>
		/// <returns></returns>
        public override int GetHashCode()
        {
            return this.Identity.GetHashCode();
        }

		private static string Truncate(string input, int len)
		{
			return (len >= input.Length) ? input : input.Substring(0, len);
		}
	}
}
