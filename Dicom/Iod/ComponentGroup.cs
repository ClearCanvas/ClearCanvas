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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Iod
{
	/// TODO: add functionality to create a component group from the individual components.
	/// 
	/// <summary>
	/// Represents one component group of a person name (VR PN).
	/// </summary>
	/// <remarks>
	/// This class assumes that the ComponentGroup has already been decoded 
	/// from any native character set into a Unicode string
	/// </remarks>
    public class ComponentGroup
    {
		#region Private Fields

		private readonly string _rawString;
		private string _familyName;
		private string _givenName;
		private string _middleName;
		private string _prefix;
		private string _suffix;
		
		#endregion

		/// <summary>
        /// Constructor.
        /// </summary>
		public ComponentGroup(string componentGroupString)
        {
            _rawString = componentGroupString;
            BreakApartIntoComponents();
        }

		#region Public Properties

		/// <summary>
		/// Gets whether or not this <see cref="ComponentGroup"/> is empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return _rawString == null || _rawString == String.Empty; }
		}

		public string Suffix
		{
			get { return _suffix; }
		}

		public string Prefix
		{
			get { return _prefix; }
		}

		public string MiddleName
		{
			get { return _middleName; }
		}

		public string GivenName
		{
			get { return _givenName; }
		}

		public string FamilyName
		{
			get { return _familyName; }
		}

		#endregion

		/// <summary>
		/// Creates and returns an empty <see cref="ComponentGroup"/>.
		/// </summary>
		/// <returns></returns>
		public static ComponentGroup GetEmptyComponentGroup()
		{
			return new ComponentGroup("");
		}

		/// <summary>
		/// Gets the entire component group as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _rawString;
		}

		/// <summary>
		/// Converts a <see cref="ComponentGroup"/> to a string.
		/// </summary>
		public static implicit operator String(ComponentGroup componentGroup)
		{
			return componentGroup.ToString();
		}

		private void BreakApartIntoComponents()
		{
			string[] components = _rawString.Split('^');

			if (components.GetUpperBound(0) >= 0 && components[0] != string.Empty)
			{
				_familyName = components[0];
			}

			if (components.GetUpperBound(0) > 0 && components[1] != string.Empty)
			{
				_givenName = components[1];
			}

			if (components.GetUpperBound(0) > 1 && components[2] != string.Empty)
			{
				_middleName = components[2];
			}

			if (components.GetUpperBound(0) > 2 && components[3] != string.Empty)
			{
				_prefix = components[3];
			}

			if (components.GetUpperBound(0) > 3 && components[4] != string.Empty)
			{
				_suffix = components[4];
			}
		}



        static private bool AreSame(string x, string y, PersonNameComparisonOptions options)
        {
            if (String.IsNullOrEmpty(x))
                return String.IsNullOrEmpty(y);
            else
            {
                switch (options)
                {
                    case PersonNameComparisonOptions.CaseSensitive:
                        return x.Equals(y);
                    case PersonNameComparisonOptions.CaseInsensitive:
                        return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
                }

                return false;
            }

        }

        #region IEquatable<ComponentGroup> Members

        /// <summary>
        /// Returns a value indicating whether two <see cref="ComponentGroup"/> are the same.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool AreSame(ComponentGroup other, PersonNameComparisonOptions options)
        {
            if (other == null)
                return false;

            if (IsEmpty)
                return other.IsEmpty;
            else
            {
                return AreSame(FamilyName, other.FamilyName, options) &&
                       AreSame(GivenName, other.GivenName, options) &&
                       AreSame(MiddleName, other.MiddleName, options) &&
                       AreSame(Prefix, other.Prefix, options) &&
                       AreSame(Suffix, other.Suffix, options);                
            }

        }


	    #endregion
    }
}
