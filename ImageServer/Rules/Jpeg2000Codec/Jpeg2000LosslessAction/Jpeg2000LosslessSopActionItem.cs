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

using System.Xml;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Rules.Jpeg2000Codec.Jpeg2000LosslessAction
{
	/// <summary>
	/// JPEG 2000 Lossless SOP Compress action item for <see cref="ServerRulesEngine"/>
	/// </summary>
	public class Jpeg2000LosslessSopActionItem : ServerActionItemBase
	{
		public Jpeg2000LosslessSopActionItem()
			: base("JPEG 2000 Lossless SOP compression action")
		{
		}

		protected override bool OnExecute(ServerActionContext context)
		{
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("compress");
			doc.AppendChild(element);
			XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
			syntaxAttribute.Value = TransferSyntax.Jpeg2000ImageCompressionLosslessOnlyUid;
			element.Attributes.Append(syntaxAttribute);

			context.CommandProcessor.AddCommand(
				new DicomCompressCommand(context.Message, doc));

			return true;
		}
	}
}
