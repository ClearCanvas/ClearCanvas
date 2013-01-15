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
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Implements a simplified version of the HL7 XPN (Extended Person Name) data type
    /// </summary>
	public partial class PersonName : IFormattable
	{
        private void CustomInitialize()
        {
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            //TODO: honour format string

            StringBuilder sb = new StringBuilder();
            sb.Append(_familyName).Append(",");

            if (!string.IsNullOrEmpty(_prefix))
                sb.Append(" ").Append(_prefix);

            sb.Append(" ").Append(_givenName);

            if (!string.IsNullOrEmpty(_middleName))
                sb.Append(" ").Append(_middleName);

            if (!string.IsNullOrEmpty(_suffix))
                sb.Append(" ").Append(_suffix);

            return sb.ToString();
        }

        #endregion

        public override string ToString()
        {
            return this.ToString(null, null);
        }
    }
}