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
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.RleCodec.RleCompressAction
{
	/// <summary>
	/// RLE compression action item for <see cref="ServerRulesEngine"/>
	/// </summary>
	public class RleActionItem : ActionItemBase<ServerActionContext>
	{
		private static readonly FilesystemQueueTypeEnum _queueType = FilesystemQueueTypeEnum.LosslessCompress;
		private readonly Expression _exprScheduledTime;
		private readonly int _offsetTime;
		private readonly TimeUnit _units;
		private readonly bool _convertFromPalette;

		public RleActionItem(int time, TimeUnit unit, bool convertFromPalette)
			: this(time, unit, null, convertFromPalette)
		{
		}

		public RleActionItem(int time, TimeUnit unit, Expression exprScheduledTime, bool convertFromPalette)
			: base("RLE compression action")
		{
			_offsetTime = time;
			_units = unit;
			_exprScheduledTime = exprScheduledTime;
			_convertFromPalette = convertFromPalette;
		}

		protected override bool OnExecute(ServerActionContext context)
		{
			DateTime scheduledTime = Platform.Time;

			if (_exprScheduledTime != null)
			{
				scheduledTime = Evaluate(_exprScheduledTime, context, scheduledTime);
			}

			scheduledTime = CalculateOffsetTime(scheduledTime, _offsetTime, _units);
			
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("compress");
			doc.AppendChild(element);
			XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
			syntaxAttribute.Value = TransferSyntax.RleLosslessUid;
			element.Attributes.Append(syntaxAttribute);

			syntaxAttribute = doc.CreateAttribute("convertFromPalette");
			syntaxAttribute.Value = _convertFromPalette.ToString();
			element.Attributes.Append(syntaxAttribute);

			context.CommandProcessor.AddCommand(
				new InsertFilesystemQueueCommand(_queueType, context.FilesystemKey, context.StudyLocationKey,
				                                 scheduledTime, doc));

			return true;
		}
	}
}