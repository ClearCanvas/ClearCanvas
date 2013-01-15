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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Xml;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class XmlReplaceValue : Task
	{
		private string _xPath;
		private string _value;
		private string _file;

		private bool _nodeMustExist;

		public bool NodeMustExist
		{
			get { return _nodeMustExist; }
			set { _nodeMustExist = value; }
		}
	
		[Required]
		public string XPath
		{
			get { return _xPath; }
			set { _xPath = value; }
		}

		[Required]
		public string File
		{
			get { return _file; }
			set { _file = value; }
		}

		[Required]
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public override bool Execute()
		{
			_value = _value ?? "";

			if (String.IsNullOrEmpty(_file))
			{
				base.Log.LogMessage("Invalid input file: {0}", _file);
				return false;
			}

			if (!System.IO.File.Exists(_file))
			{
				base.Log.LogMessage("File does not exist: {0}", _file);
				return false;
			}

			XmlDocument doc = new XmlDocument();
			doc.Load(File);

			XmlNodeList nodes = doc.SelectNodes(XPath);
			if (nodes != null && nodes.Count == 1)
			{
				XmlNode node = nodes[0];
				if (node is XmlElement)
				{
					node.InnerText = _value;
					base.Log.LogMessage("Replaced element value '{0}' with '{1}'", _xPath, _value);
				}
				else if (node is XmlAttribute)
				{
					node.Value = _value;
					base.Log.LogMessage("Replaced attribute value '{0}' with '{1}'", _xPath, _value);
				}

				doc.Save(File);
				return true;
			}
			else if (nodes == null || nodes.Count == 0)
			{
				base.Log.LogMessage("Xml node(s) not found: {0}", _xPath);
			}
			else
			{
				base.Log.LogMessage("More than one xml node found: {0}", _xPath);
			}

			if (NodeMustExist)
				return false;
			else
				return true;
		}
	}
}
