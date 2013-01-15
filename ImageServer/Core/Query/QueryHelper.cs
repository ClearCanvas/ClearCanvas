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
using System.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Core.Query
{
    public static class QueryHelper
    {
        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for an array of matching string values.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="vals"></param>
        public static void SetStringArrayCondition(ISearchCondition<string> cond, string[] vals)
        {
            if (vals == null || vals.Length == 0)
                return;

            if (vals.Length == 1)
                cond.EqualTo(vals[0]);
            else
                cond.In(vals);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for a DICOM range matching string value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        public static void SetRangeCondition(ISearchCondition<string> cond, string val)
        {
            if (val.Length == 0)
                return;

            if (val.Contains("-"))
            {
                string[] vals = val.Split(new[] { '-' });
                if (val.IndexOf('-') == 0)
                    cond.LessThanOrEqualTo(vals[1]);
                else if (val.IndexOf('-') == val.Length - 1)
                    cond.MoreThanOrEqualTo(vals[0]);
                else
                    cond.Between(vals[0], vals[1]);
            }
            else
                cond.EqualTo(val);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for DICOM string based (wildcard matching) value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        public static void SetStringCondition(ISearchCondition<string> cond, string val)
        {
            if (val.Length == 0 || SearchValueOnlyWildcard(val, false))
                return;

            if (val.Contains("*") || val.Contains("?"))
            {
                String value = val.Replace("%", "[%]").Replace("_", "[_]");
                value = value.Replace('*', '%');
                value = value.Replace('?', '_');
                cond.Like(value);
            }
            else
                cond.EqualTo(val);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for DICOM string based (wildcard matching) value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        public static void SetGuiStringCondition(ISearchCondition<string> cond, string val)
        {
            if ( string.IsNullOrEmpty(val) || SearchValueOnlyWildcard(val, true))
                return;

            String value = val.Replace('*', '%');
            value = value.Replace('?', '_');
            cond.Like(value);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for a <see cref="ServerEntityKey"/> reference.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="vals"></param>
        public static void SetKeyCondition(ISearchCondition<ServerEntityKey> cond, ServerEntityKey[] vals)
        {
            if (vals == null || vals.Length == 0)
                return;

            if (vals.Length == 1)
                cond.EqualTo(vals[0]);
            else
                cond.In(vals);
        }

        /// <summary>
        /// Set a xPath based <see cref="ISearchCondition{T}"/> for an <see cref="XmlDocument"/> column.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="xPath"></param>
        /// <param name="match"></param>
        public static void SetXmlStringCondition(ISearchCondition<XmlDocument> cond, string xPath, string match)
        {
            var doc = new XmlDocument();

            var xPathElem = doc.CreateElement("Select");
            doc.AppendChild(xPathElem);

            var equalElem = doc.CreateElement("XPath");
            xPathElem.AppendChild(equalElem);

            var attribElem = doc.CreateAttribute("path");
            attribElem.Value = xPath;
            xPathElem.Attributes.Append(attribElem);

            if (match.Contains("*") || match.Contains("?"))
            {
                var value = match.Replace("%", "[%]").Replace("_", "[_]");
                value = value.Replace('*', '%');
                value = value.Replace('?', '_');

                attribElem = doc.CreateAttribute("value");
                attribElem.Value = value;
                xPathElem.Attributes.Append(attribElem);

                cond.Like(doc);
            }
            else
            {
                attribElem = doc.CreateAttribute("value");
                attribElem.Value = match;
                xPathElem.Attributes.Append(attribElem);

                cond.EqualTo(doc);
            }
        }

        /// <summary>
        /// Check to see if the search value only contains wildcard charcters and can be ommited from a select.
        /// </summary>
        /// <param name="val">The value to check</param>
        /// <param name="sqlWildcards"></param>
        /// <returns>True if <see cref="val"/> only contains wildcard charcater(s).</returns>
        private static bool SearchValueOnlyWildcard(string val, bool sqlWildcards)
        {
            string val2 = val.Trim();

            if (sqlWildcards)
            {
                foreach (char c in val2)
                    if (c != '*' && c != '%')
                        return false;
            }
            else
            {
                foreach (char c in val2)
                    if (c != '*')
                        return false;
            }
            return true;
        }
    }
}
