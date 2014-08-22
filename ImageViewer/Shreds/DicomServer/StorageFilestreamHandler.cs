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
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	class StorageFilestreamHandler : IDicomFilestreamHandler, IDisposable
	{
		private readonly IDicomServerContext _context;
		private readonly ImportFilesContext _importContext;
		private FileStream _fileStream;
		private string _sourceFolder;
		private string _sourceFilename;
		private bool _rejectFile = false;

		public StorageFilestreamHandler(IDicomServerContext context, DicomReceiveImportContext importContext)
		{
			_context = context;
			_importContext = importContext;

			if (LocalStorageMonitor.IsMaxUsedSpaceExceeded)
			{
				//The input to this method is a VALID DICOM file, and we know we should have stored it if it weren't for
				//the fact that we're out of disk space. So, we insert the work item UID anyway, knowing that it'll cause
				//the work item to fail. In fact, that's why we're doing it.
				_rejectFile = true;
			}

			if (!File.Exists(_context.StorageConfiguration.FileStoreIncomingFolder))
				Directory.CreateDirectory(_context.StorageConfiguration.FileStoreIncomingFolder);
		}

		public bool SaveStreamData(DicomMessage message, byte[] data, int offset, int length)
		{
			if (_rejectFile) return true;

			if (_fileStream == null)
			{
				_sourceFolder = _context.StorageConfiguration.FileStoreIncomingFolder;

				_sourceFilename = Path.Combine(_sourceFolder, Guid.NewGuid().ToString() + ".cc");

				try
				{
					_fileStream = new FileStream(_sourceFilename, FileMode.Create, FileAccess.Write, FileShare.None, 65536, FileOptions.SequentialScan);
				}
				catch (Exception x)
				{
					Platform.Log(LogLevel.Warn, x, "Unable to open file for saving filestream: {0}", _sourceFilename);
					return false;
				}
			}

			_fileStream.Write(data, offset, length);

			return true;
		}

		public void CancelStream()
		{
			if (_fileStream != null)
			{
				try
				{
					_fileStream.Close();
					_fileStream = null;
				}
				catch (Exception)
				{ }
			}

			if (!string.IsNullOrEmpty(_sourceFilename))
			{
				try
				{
					if (File.Exists(_sourceFilename))
					{
						FileUtils.Delete(_sourceFilename);
						_sourceFilename = null;
					}
				}
				catch (Exception)
				{ }
			}

			CleanupFile();
		}

		public bool CompleteStream(Dicom.Network.DicomServer server, ServerAssociationParameters assoc, byte presentationId, DicomMessage message)
		{
			DicomProcessingResult result;
			var importer = new ImportFilesUtility(_importContext);

			if (_rejectFile)
			{
				result = new DicomProcessingResult();
				result.SetError(DicomStatuses.StorageStorageOutOfResources, string.Format("Import failed, disk space usage exceeded"));

				string studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
				WorkItem workItem;
				lock (_importContext.StudyWorkItemsSyncLock)
					_importContext.StudyWorkItems.TryGetValue(studyInstanceUid, out workItem);

				importer.InsertFailedWorkItemUid(workItem, message, result);

				_importContext.FatalError = true;
				importer.AuditFailure(result);

				Platform.Log(LogLevel.Warn, "Failure receiving sop, out of disk space: {0}", message.AffectedSopInstanceUid);
				server.SendCStoreResponse(presentationId, message.MessageId, message.AffectedSopInstanceUid, result.DicomStatus);
				return true;
			}

			try
			{
				if (_fileStream != null)
				{
					_fileStream.Flush(true);
					_fileStream.Close();
					_fileStream.Dispose();
					_fileStream = null;
				}

				// Convert to file to pass in the source filename
				var theFile = new DicomFile(message,_sourceFilename);

				result = importer.Import(theFile, BadFileBehaviourEnum.Delete, FileImportBehaviourEnum.Move);

				if (result.Successful)
				{
					if (!String.IsNullOrEmpty(result.AccessionNumber))
						Platform.Log(LogLevel.Info, "Received SOP Instance {0} from {1} to {2} (A#:{3} StudyUid:{4})",
									 result.SopInstanceUid, assoc.CallingAE, assoc.CalledAE, result.AccessionNumber,
									 result.StudyInstanceUid);
					else
						Platform.Log(LogLevel.Info, "Received SOP Instance {0} from {1} to {2} (StudyUid:{3})",
									 result.SopInstanceUid, assoc.CallingAE, assoc.CalledAE,
									 result.StudyInstanceUid);
				}
			}
			catch (Exception e)
			{
				result = new DicomProcessingResult { DicomStatus = DicomStatuses.ProcessingFailure, ErrorMessage = e.Message };
			}

			if (!result.Successful)
			{
				Platform.Log(LogLevel.Warn, "Failure importing sop: {0}", result.ErrorMessage);
			}

			CleanupFile();

			server.SendCStoreResponse(presentationId, message.MessageId, message.AffectedSopInstanceUid, result.DicomStatus);
			return true;
		}

		private void CleanupFile()
		{
			if (!string.IsNullOrEmpty(_sourceFilename))
				try
				{
					if (File.Exists(_sourceFilename))
						FileUtils.Delete(_sourceFilename);
					_sourceFilename = null;
				}
				catch (Exception x)
				{
					Platform.Log(LogLevel.Warn, x, "Unexpected error cleaning up streaming SOP Instance file.");
				}
		}

		public void Dispose()
		{
			if (_fileStream != null)
			{
				try
				{
					_fileStream.Flush(true);
					_fileStream.Close();
					_fileStream.Dispose();
					_fileStream = null;
				}
				catch (Exception)
				{
				}
			}

			CleanupFile();
		}


		private static string GetRemoteHostName(AssociationParameters association)
		{
			string remoteHostName = null;
			try
			{
				if (association.RemoteEndPoint != null)
				{
					try
					{
						IPHostEntry entry = Dns.GetHostEntry(association.RemoteEndPoint.Address);
						remoteHostName = entry.HostName;
					}
					catch
					{
						remoteHostName = association.RemoteEndPoint.Address.ToString();
					}
				}
			}
			catch (Exception e)
			{
				remoteHostName = null;
				Platform.Log(LogLevel.Warn, e, "Unable to resolve remote host name.");
			}

			return remoteHostName;
		}
	}
}
