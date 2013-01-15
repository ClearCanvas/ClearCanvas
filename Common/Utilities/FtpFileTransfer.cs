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
using System.IO;
using System.Net;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Provide access to remote files using the FTP protocol.
	/// </summary>
	public class FtpFileTransfer : IRemoteFileTransfer
	{
		private readonly string _userId;
		private readonly string _password;
		private readonly bool _usePassive;

		// Keep track of the url created to reduce the number of FTP mkdir call
		private readonly List<string> _urlCreated;

		/// <summary>
		/// Get the base Uri for the FTP site.
		/// </summary>
		public Uri BaseUri { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public FtpFileTransfer(string userId, string password, string baseUri, bool usePassive)
		{
			_userId = userId;
			_password = password;
			_usePassive = usePassive;
			BaseUri = CreateProperBaseUri(baseUri);
			_urlCreated = new List<string>();
		}

		/// <summary>
		/// Upload one file from local to remote.
		/// </summary>
		/// <param name="request"></param>
		public void Upload(FileTransferRequest request)
		{
			try
			{
				CreateRemoteDirectoryForFile(request.RemoteFile);

				// upload the file
				var credentials = new NetworkCredential(_userId, _password);
				using (var ftpClient = new FtpClient {UsePassiveMode = _usePassive, Credentials = credentials})
				{
					ftpClient.UploadFile(request.RemoteFile, request.LocalFile);
				}
			}
			catch (Exception e)
			{
				//TODO (cr Oct 2009): we're not supposed to use SR for exception messages.
				//Throw a different type of exception and use an ExceptionHandler if it's supposed to be a user message.
				var message = string.Format(SR.ExceptionFailedToTransferFile, request.LocalFile, request.RemoteFile);
				throw new Exception(message, e);
			}
		}

		/// <summary>
		/// Download one file from remote to local
		/// </summary>
		/// <param name="request"></param>
		public void Download(FileTransferRequest request)
		{
			try
			{
				// Create download directory if not already exist
				var downloadDirectory = Path.GetDirectoryName(request.LocalFile);
				if (!Directory.Exists(downloadDirectory))
					Directory.CreateDirectory(downloadDirectory);

				// download the file
				var credentials = new NetworkCredential(_userId, _password);
				using (var ftpClient = new FtpClient {UsePassiveMode = _usePassive, Credentials = credentials})
				{
					ftpClient.DownloadFile(request.RemoteFile, request.LocalFile);
				}
			}
			catch (Exception e)
			{
				//TODO (cr Oct 2009): we're not supposed to use SR for exception messages.
				//Throw a different type of exception and use an ExceptionHandler if it's supposed to be a user message.
				var message = string.Format(SR.ExceptionFailedToTransferFile, request.RemoteFile, request.LocalFile);
				throw new Exception(message, e);
			}
		}

		private void CreateRemoteDirectoryForFile(Uri urlToCreate)
		{
			var uri = new Uri(this.BaseUri.ToString());

			// Continues to build uri and create them
			foreach (var segment in urlToCreate.Segments)
			{
				uri = new Uri(uri, segment);

				var isFileSegment = segment == urlToCreate.Segments[urlToCreate.Segments.Length - 1];

				if (_urlCreated.Contains(uri.ToString()) ||
				    Equals(this.BaseUri, uri) || // The base Uri should already exist
				    isFileSegment) // Skip the file segment, so we don't create a directory with the same name as the file
					continue;

				try
				{
					var ftpRequest = (FtpWebRequest) WebRequest.Create(uri);
					ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
					ftpRequest.UseBinary = true;
					ftpRequest.UsePassive = _usePassive;
					ftpRequest.Credentials = new NetworkCredential(_userId, _password);

					using (var ftpResponse = (FtpWebResponse) ftpRequest.GetResponse())
					{
						ftpResponse.Close();
					}
				}
				catch (Exception)
				{
					// Purposely swallowing exception here because the remote folders may already exist.
					// But this is okay because if there is a real problem creating folders, 
					// another exception will be thrown when transfering files
				}

				_urlCreated.Add(uri.ToString());
			}
		}

		private static Uri CreateProperBaseUri(string baseUri)
		{
			// Construct a temporary Uri object.  The first segment is always the slash
			var tempUri = new Uri(baseUri);
			var segmentDelimiter = tempUri.Segments[0];

			// Make sure the baseUri always ends with a trailing slash.
			return baseUri.EndsWith(segmentDelimiter)
			       	? new Uri(baseUri)
			       	: new Uri(string.Concat(baseUri, segmentDelimiter));
		}

		private class FtpClient : WebClient
		{
			public bool UsePassiveMode { get; set; }

			protected override WebRequest GetWebRequest(Uri address)
			{
				var request = base.GetWebRequest(address);
				var ftpWebRequest = request as FtpWebRequest;
				if (ftpWebRequest != null)
				{
					ftpWebRequest.UsePassive = UsePassiveMode;
					ftpWebRequest.UseBinary = true;
				}
				return request;
			}
		}
	}
}