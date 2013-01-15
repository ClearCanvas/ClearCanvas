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
using System.Text;
using System.Xml;

namespace ClearCanvas.ImageServer.Model
{
	public partial class Alert
	{
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			StringWriter sw = new StringWriter();
			XmlWriterSettings xmlSettings = new XmlWriterSettings();

			xmlSettings.Encoding = Encoding.UTF8;
			xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlSettings.Indent = true;
			xmlSettings.NewLineOnAttributes = false;
			xmlSettings.CheckCharacters = true;
			xmlSettings.IndentChars = "  ";

			XmlWriter xw = XmlWriter.Create(sw, xmlSettings);

			Content.WriteTo(xw);

			xw.Close();

			sb.AppendFormat("{0} {1} {2} {3} [{4}] - {5}",
			                InsertTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
			                Source,
			                AlertLevelEnum.Description,
			                AlertCategoryEnum.Description,
			                Component, sw);

			return sb.ToString();
		}
	}
}
