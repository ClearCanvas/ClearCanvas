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

using System.IO;
using System.Xml;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    /// <summary>
    /// Customized <see cref="XmlTextWriter"/> class that ensures the output xml
    /// is correctly encoded.
    /// </summary>
    /// <remarks>
    /// <see cref="CustomXmlTextWriter"/> escape character (0x1B) from the string value
    /// when serializing the data.
    /// </remarks>
    class CustomXmlTextWriter:XmlTextWriter
    {
        #region Constructors
        public CustomXmlTextWriter(TextWriter writer)
            : base(writer)
        {

        } 
        #endregion

        #region Overridden Public Methods
        public override void WriteString(string text)
        {
            string escapeChar = "\x1B";
            if (text!=null && text.Contains(escapeChar))
            {
                base.WriteString(text.Replace(escapeChar, string.Empty));
            }
            else
                base.WriteString(text);
        } 
        #endregion
    }
}