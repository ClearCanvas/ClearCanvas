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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    /// <summary>
    /// Provides unified methods to convert a person name in dicom format into a human readable format suitable for display on a web page.
    /// </summary>
    /// <remarks>
    /// Use <see cref="NameFormatter"/> to ensure consistent name format is generated in the entire applications. By default, the person name
    /// is rendered in "Last, First Middle" format. The caller can specify an alternative format when calling <see cref="Format(string, string)"/>. 
    /// The following placeholders for the name fields can be used in the format string: {Last}, {First}, {Middle}, {Given}, {Prefix}, {Suffix}
    /// 
    /// 
    /// </remarks>
    /// <example>
    /// The following example illustrates how to convert a dicom name to a human readable string representation in "First Middle Last" format.
    /// 
    /// <code>
    /// 
    /// string patientName = "Adams^John Robert Quincy^^Rev.^B.A. M.Div";
    /// Console.WriteLine("Formatted name= " + NameFormatter.Format(patientName, "{First} {Middle} {Last}"));
    /// 
    /// // output: Formatted name=John Robert Qunicy Adams 
    /// 
    /// </code>
    /// 
    /// </example>
    public class NameFormatter : IFormatProvider, ICustomFormatter
    {
        private enum Component
        {
            LastName,
            FirstName,
            MiddleName,
            GivenName,
            Prefix,
            Suffix
        }

        #region Constants

        public static string DEFAULT_NAME_FORMAT = String.IsNullOrEmpty(UISettings.Default.NameFormat) ? "{Last}, {First} {Middle}" : UISettings.Default.NameFormat;

        #endregion Constants

        #region Constructor
       
        #endregion Constructor

        #region Private members

        private bool _formatEmptyName = false;


        #endregion Private members

        #region Private Methods
        
        static private string GetComponent(PersonName name, Component part)
        {
            if (name == null)
                return null;

            if (name.IsEmpty)
                return "";

            switch (part)
            {
                case Component.FirstName:
                    return name.FirstName;

                case Component.GivenName:
                    return name.FirstName;

                case Component.LastName:
                    return name.LastName;

                case Component.MiddleName:
                    if (name.Phonetic.IsEmpty)
                    {
                        return "";
                    }
                    else
                        return name.Phonetic.MiddleName;

                case Component.Prefix:
                    if (name.Phonetic.IsEmpty)
                    {
                        return "";
                    }
                    else
                        return name.Phonetic.Prefix;


                case Component.Suffix:
                    if (name.Phonetic.IsEmpty)
                    {
                        return "";
                    }
                    else
                        return name.Phonetic.Suffix;

                default:
                    return null;
            }
        }

        #endregion Private Methods

        #region Public methods
        static public string Format(string dicomName)
        {
            return Format(new PersonName(dicomName), DEFAULT_NAME_FORMAT);
        }
        
        static public string Format(PersonName personName)
        {
            return Format(personName, DEFAULT_NAME_FORMAT);
        }

        static public string Format(string dicomName, string format)
        {
            return Format(new PersonName(dicomName), format);
        }

        static public string Format(PersonName name, string format)
        {
            NameFormatter formatter = new NameFormatter();
            return formatter.Format(format, name, formatter);
        }

        #endregion Public methods

        #region Public Properties
        public bool FormatEmptyName
        {
            get { return _formatEmptyName; }
            set { _formatEmptyName = value; }
        }

        #endregion Public Properties

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(PersonName)) // can only format PersonName
                return this;
            else
                return null;
        }

        #endregion

        #region ICustomFormatter Members

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg!=null && arg is PersonName)
            {
                PersonName pn = arg as PersonName;

                if (pn.IsEmpty && !FormatEmptyName)
                {
                    return "";
                }

                if (String.IsNullOrEmpty(format))
                    format = DEFAULT_NAME_FORMAT;

                string output = format;
                output = output.Replace("{Last}", GetComponent(pn, Component.LastName));
                output = output.Replace("{First}", GetComponent(pn, Component.FirstName));
                output = output.Replace("{Given}", GetComponent(pn, Component.GivenName));
                output = output.Replace("{Middle}", GetComponent(pn, Component.MiddleName));
                output = output.Replace("{Suffix}", GetComponent(pn, Component.Suffix));
                output = output.Replace("{Prefix}", GetComponent(pn, Component.Prefix));

                //Preserve the ampersand entity "&amp;"
                output = output.Replace("&amp;", "&amp;amp;");


                return output;
            }

            return null;
        }

        #endregion
    }
}
