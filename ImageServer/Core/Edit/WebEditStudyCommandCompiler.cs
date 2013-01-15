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
using System.Xml;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Helper class to compile an XML specification into a list of <see cref="BaseImageLevelUpdateCommand"/>
	/// to be used by the <see cref="StudyEditor"/>
	/// </summary>
	public class WebEditStudyCommandCompiler
	{
		private readonly Dictionary<string, IWebEditStudyCommandCompiler> _commandCompilers = new Dictionary<string, IWebEditStudyCommandCompiler>();

		public WebEditStudyCommandCompiler()
		{
			LoadCommandCompilers();
		}

		private void LoadCommandCompilers()
		{
			// TODO: Make this plugin
			SetTagCommandCompiler compiler = new SetTagCommandCompiler();
			_commandCompilers.Add(compiler.CommandName, compiler);
		}

		public List<BaseImageLevelUpdateCommand> Compile(XmlNode node)
		{
			List<BaseImageLevelUpdateCommand> commands = new List<BaseImageLevelUpdateCommand>();

			foreach (XmlNode subNode in node.ChildNodes)
			{
				IList<BaseImageLevelUpdateCommand> list = Compile(new XmlNodeReader(subNode));
				if (list != null && list.Count > 0)
					commands.AddRange(list);
			}

			return commands;
		}

		public List<BaseImageLevelUpdateCommand> Compile(XmlReader reader)
		{
			List<BaseImageLevelUpdateCommand> commands = new List<BaseImageLevelUpdateCommand>();

			if (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						{
							if (_commandCompilers.ContainsKey(reader.Name))
							{
								IWebEditStudyCommandCompiler plugin = _commandCompilers[reader.Name];
								commands.Add(plugin.Compile(reader));
							}
							else
							{
								throw new NotSupportedException(reader.Name);
							}
						}
						break;
				}
			}


			return commands;
		}
	}

	/// <summary>
	/// Compile an XML node into a <see cref="SetTagCommand"/>.
	/// </summary>
	internal class SetTagCommandCompiler : IWebEditStudyCommandCompiler
	{
		#region IWebEditStudyCommandCompiler Members

		public string CommandName
		{
			get { return "SetTag"; }
		}


		#endregion

		#region IWebEditStudyCommandCompiler Members

		#endregion

		/// <summary>
		/// Compiles an XML specification into a <see cref="SetTagCommand"/> object.
		/// </summary>
		/// <param name="reader">Reference to a <see cref="XmlReader"/> to read the Xml node.</param>
		/// <returns>An instance of <see cref="SetTagCommand"/></returns>
		/// <remarks>
		/// The <cref="XmlReader"/> must be positioned at an Xml node named "SetTag".
		/// </remarks>
		public BaseImageLevelUpdateCommand Compile(XmlReader reader)
		{
			SetTagCommand command = XmlUtils.Deserialize<SetTagCommand>(reader);
			return command;
		}
	}
}