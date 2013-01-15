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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Base class for server rule samples that read samples from the embedded resource.
    /// </summary>
    public class SampleRuleBase : ISampleRule
    {
        #region Private members

        private readonly IList<ServerRuleApplyTimeEnum> _applyTime = new List<ServerRuleApplyTimeEnum>();
        private readonly string _embeddedXmlName;
        private string _description;
        private string _name;
        private ServerRuleTypeEnum _type;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SampleRuleBase"/>
        /// </summary>
        /// <param name="name">Name of the sample rule</param>
        /// <param name="description">Description of the sample rule</param>
        /// <param name="type">Type of the sample rule</param>
        /// <param name="embeddedXmlName">Name of the resource file containing the sample rule xml</param>
        public SampleRuleBase(string name, string description, ServerRuleTypeEnum type, string embeddedXmlName)
        {
            _name = name;
            _description = description;
            _type = type;
            _embeddedXmlName = embeddedXmlName;
        }

        #endregion

        #region ISampleRule Members

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ServerRuleTypeEnum Type
        {
            get { return _type; }
            set { _type = value; }
        }


        public XmlDocument Rule
        {
            get
            {
                Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), _embeddedXmlName);
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                stream.Close();
                return doc;
            }
        }

        public IList<ServerRuleApplyTimeEnum> ApplyTimeList
        {
            get { return _applyTime; }
        }

        #endregion
    }
}