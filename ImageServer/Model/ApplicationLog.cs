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
using System.Text;

namespace ClearCanvas.ImageServer.Model
{
	public partial class ApplicationLog
	{
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (Exception != null && Exception.Length > 0)
			{
				sb.AppendFormat("{0} {1} [{2}]  {3} - Exception thrown",
				              Host,
				              Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				              Thread,
				              LogLevel);
				sb.AppendLine();
				sb.AppendLine(Message);
				sb.AppendLine();
				sb.AppendLine(Exception);
			}
			else
				sb.AppendFormat("{0} {1} [{2}]  {3} - {4}",
				              Host,
				              Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				              Thread,
				              LogLevel,
				              Message);

			return sb.ToString();
		}

		public string MessageException
		{
			get
			{
				if (string.IsNullOrEmpty(Exception))
				{
					return Message
						.Replace("<","&lt;")
						.Replace(">","&gt;")
						.Replace(Environment.NewLine, "<br/>");
				}

				StringBuilder sb = new StringBuilder();
				sb.AppendLine(Message);
				sb.AppendLine(Exception);
				return sb.ToString()
						.Replace("<", "&lt;")
						.Replace(">", "&gt;")
						.Replace(Environment.NewLine, "<br/>");
			}
		}
	}
}
