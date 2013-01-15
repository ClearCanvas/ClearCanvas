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

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Defines a request to transfer a file between local and remote file systems.
	/// </summary>
	public class FileTransferRequest
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public FileTransferRequest(Uri remoteFile, string localFile)
		{
			RemoteFile = remoteFile;
			LocalFile = localFile;
		}

		#region Public Properties

		/// <summary>
		/// The url of the remote file.
		/// </summary>
		public Uri RemoteFile { get; private set; }

		/// <summary>
		/// The complete path of the local file.
		/// </summary>
		public string LocalFile { get; private set; }

		#endregion
	}

	/// <summary>
	/// Defines an interface for accessing remote files.
	/// </summary>
	public interface IRemoteFileTransfer
	{
		/// <summary>
		/// Upload one file from local to remote.
		/// </summary>
		/// <param name="request"></param>
		void Upload(FileTransferRequest request);

		/// <summary>
		/// Download one file from remote to local
		/// </summary>
		/// <param name="request"></param>
		void Download(FileTransferRequest request);
	}
}
