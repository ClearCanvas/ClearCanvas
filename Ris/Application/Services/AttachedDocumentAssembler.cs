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
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class AttachedDocumentAssembler
	{
		public AttachedDocumentSummary CreateAttachedDocumentSummary(AttachedDocument doc)
		{
			var summary = new AttachedDocumentSummary();

			UpdateAttachedDocumentSummary(doc, summary);

			return summary;
		}

		public void UpdateAttachedDocumentSummary(AttachedDocument doc, AttachedDocumentSummary summary)
		{
			summary.DocumentRef = doc.GetRef();
			summary.CreationTime = doc.CreationTime;
			summary.ReceivedTime = doc.DocumentReceivedTime;
			summary.MimeType = doc.MimeType;
			summary.ContentUrl = doc.ContentUrl;
			summary.FileExtension = doc.FileExtension;
			summary.DocumentHeaders = new Dictionary<string, string>(doc.DocumentHeaders);
			summary.DocumentTypeName = doc.DocumentTypeName;
		}
	}
}
