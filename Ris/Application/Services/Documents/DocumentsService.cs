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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Documents;

namespace ClearCanvas.Ris.Application.Services.Documents
{
	[ServiceImplementsContract(typeof(IDocumentsService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class DocumentsService : ApplicationServiceBase, IDocumentsService
	{
		[ReadOperation]
		public GetAttachedDocumentFormDataResponse GetAttachedDocumentFormData(GetAttachedDocumentFormDataRequest request)
		{
			return new GetAttachedDocumentFormDataResponse(
				EnumUtils.GetEnumValueList<PatientAttachmentCategoryEnum>(PersistenceContext),
				EnumUtils.GetEnumValueList<OrderAttachmentCategoryEnum>(PersistenceContext));
		}

		[UpdateOperation]
		public UploadResponse Upload(UploadRequest request)
		{
			var tempFile = Path.GetTempFileName();
			try
			{
				// write data to a temp file
				File.WriteAllBytes(tempFile, request.DataContent);

				// create the new document object, and put the remote file
				var args = new AttachedDocumentCreationArgs
				{
					MimeType = request.MimeType,
					FileExtension = request.FileExtension,
					LocalContentFilePath = tempFile
				};

				var document = AttachedDocument.Create(args, AttachmentStore.GetClient());
				PersistenceContext.Lock(document, DirtyState.New);

				PersistenceContext.SynchState();

				var assembler = new AttachedDocumentAssembler();
				return new UploadResponse(assembler.CreateAttachedDocumentSummary(document));

			}
			finally
			{
				File.Delete(tempFile);
			}
		}
	}
}
