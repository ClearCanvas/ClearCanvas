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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides methods for working with attached documents stored on the RIS server.
	/// </summary>
	public static class AttachedDocument
	{
		/// <summary>
		/// Downloads a document at a specified relativeUrl to a temporary file.
		/// </summary>
		/// <param name="documentSummary"></param>
		/// <returns>The location of the downloaded file.</returns>
		public static string DownloadFile(AttachedDocumentSummary documentSummary)
		{
			Platform.CheckForNullReference(documentSummary, "documentSummary");

			// if already cached locally, return local file name
			var tempFile = TempFileManager.Instance.GetFile(documentSummary.DocumentRef);
			if (!string.IsNullOrEmpty(tempFile))
				return tempFile;

			var ftpFileTransfer = new FtpFileTransfer(
				AttachedDocumentSettings.Default.FtpUserId,
				AttachedDocumentSettings.Default.FtpPassword,
				AttachedDocumentSettings.Default.FtpBaseUrl,
				AttachedDocumentSettings.Default.FtpPassiveMode);

			var fullUrl = new Uri(ftpFileTransfer.BaseUri, documentSummary.ContentUrl);
			var fileExtension = Path.GetExtension(fullUrl.LocalPath).Trim('.');
			var localFilePath = TempFileManager.Instance.CreateFile(documentSummary.DocumentRef, fileExtension,
			                                                        fn => ftpFileTransfer.Download(new FileTransferRequest(fullUrl, fn)),
			                                                        TimeSpan.FromSeconds(AttachedDocumentSettings.Default.DownloadCacheTimeToLive));

			return localFilePath;
		}
	}
}