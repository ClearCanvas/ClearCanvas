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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	/// <summary>
	/// Provides access to the attached document store that is configured by <see cref="AttachedDocumentSettings"/>.
	/// </summary>
	public static class AttachmentStore
	{
		/// <summary>
		/// Gets a new client to the attached document store, safe for use by a single-thread.
		/// </summary>
		/// <returns></returns>
		public static IAttachedDocumentStore GetClient()
		{
			// always construct a new instance of each object here, so we don't have to worry about thread-safety
			var ftpSettings = new AttachedDocumentSettings();
			var ftp = new FtpFileTransfer(
				ftpSettings.FtpUserId,
				ftpSettings.FtpPassword,
				ftpSettings.FtpBaseUrl,
				ftpSettings.FtpPassiveMode);
			return new FtpAttachedDocumentStore(ftp, Path.GetTempPath());
		}
	}
}
