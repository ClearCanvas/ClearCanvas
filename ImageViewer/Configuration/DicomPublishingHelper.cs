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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Configuration
{
	// TODO (CR Jan 2013): Badly needs to be refactored.

	/// <summary>
	/// Helper class for publishing DICOM files to update existing studies using the configured publishing settings.
	/// </summary>
	/// <remarks>
	/// This class identifies potential destination servers depending on the current configuration
	/// and source/origin servers identified by calling code. Instances are only published to servers
	/// that possess the parent study.
	/// </remarks>
	public class DicomPublishingHelper
	{
		private readonly List<DicomFile> _files = new List<DicomFile>();
		private bool _hasErrors = false;

		/// <summary>
		/// Gets the list of files to be published.
		/// </summary>
		public List<DicomFile> Files
		{
			get { return _files; }
		}

		/// <summary>
		/// Gets or sets the source server AE from which the study was opened.
		/// </summary>
		/// <remarks>
		/// If the subject study is streamed from an ImageServer, the source AE would be the streaming server.
		/// If the subject study is on a remote PACS server, the source AE would be the remote server.
		/// </remarks>
		public IDicomServiceNode SourceServer { get; set; }

		/// <summary>
		/// Gets or sets the remote server AE from which the study originated.
		/// </summary>
		/// <remarks>
		/// In most cases, this is the server identified by the <see cref="DicomTags.SourceApplicationEntityTitle"/> attribute in the file's metadata.
		/// </remarks>
		public IDicomServiceNode OriginServer { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not the previous call to <see cref="Publish"/> files resulted in one or more errors.
		/// </summary>
		public bool HasErrors
		{
			get { return _hasErrors; }
		}

		/// <summary>
		/// Publishes the specified files.
		/// </summary>
		/// <remarks>
		/// Files are published to one or more of the servers identified by the <see cref="SourceServer"/> and <see cref="OriginServer"/> properties,
		/// and the <see cref="DefaultServers"/> depending on the current user configuration.
		/// </remarks>
		/// <returns>True if the files were published to all relevant destinations successfully; False if publishing to one or more destinations failed.</returns>
		public bool Publish()
		{
			// TODO (CR Mar 2012): Method's getting a bit long now.

			var filesByStudyUid = Files.GroupBy(f => f.DataSet[DicomTags.StudyInstanceUid].ToString());
			var hasErrors = false;

			// generate a list of files to be published
			var localFiles = new List<DicomFile>();
			foreach (var studyFiles in filesByStudyUid)
			{
				try
				{
					// files are only published to the local store if it possesses the parent study
					if (StudyExistsOnLocal(studyFiles.Key))
						localFiles.AddRange(studyFiles);
				}
				catch (Exception e)
				{
					//Consider failure to query an error because we don't know whether or not the study exists.
					Platform.Log(LogLevel.Error, e, "Unable to determine if study '{0}' exists locally.", studyFiles.Key);
					hasErrors = true;
				}
			}

			// publish local files now
			if (localFiles.Count > 0 && !PublishFilesToLocal(localFiles))
				hasErrors = true;

			// check that user has permissions to publish to remote servers
			if (PermissionsHelper.IsInRole(AuthorityTokens.Publishing))
			{
				var servers = new List<IDicomServiceNode>();

				// TODO (CR Jan 2013 - Low): How files are published and to where should be an independent strategy. Will enter an SDK ticket.

				// if configured to publish to the original server, resolve the origin and add to list of destinations
				if (DicomPublishingSettings.Default.SendLocalToStudySourceAE)
				{
					if (OriginServer != null && !OriginServer.IsLocal && !ContainsServer(servers, OriginServer))
						servers.Add(OriginServer);
				}

				// resolve the source server and add to list of destinations
				if (SourceServer != null && !SourceServer.IsLocal && !ContainsServer(servers, SourceServer))
					servers.Add(SourceServer);

				// enumerate the list of destination servers
				foreach (var server in servers)
				{
					// generate a list of files to be published
					var remoteFiles = new List<DicomFile>();
					foreach (var studyFiles in filesByStudyUid)
					{
						try
						{
							// files are only published to servers which possess the parent study
							if (StudyExistsOnRemote(server, studyFiles.Key))
								remoteFiles.AddRange(studyFiles);
						}
						catch (Exception e)
						{
							//Consider failure to query an error because we don't know whether or not the study exists.
							Platform.Log(LogLevel.Error, e, "Unable to determine if study '{0}' exists on server '{1}'.", studyFiles.Key, server.AETitle);
							hasErrors = true;
						}
					}

					// publish remote files now
					if (remoteFiles.Count > 0 && !PublishFilesToRemote(server, remoteFiles))
						hasErrors = true;
				}
			}
			else
			{
				Platform.Log(LogLevel.Debug, "Skipping remote publishing step; user does not have Publishing permissions.");
			}

			_hasErrors = hasErrors;
			return !hasErrors;
		}

		private static bool ContainsServer(IEnumerable<IDicomServiceNode> servers, IDicomServiceNode server)
		{
			return server != null && servers.Any(s => s.Name == server.Name);
		}

		private static bool StudyExistsOnLocal(string studyInstanceUid)
		{
			using (var bridge = new StudyStoreBridge())
				return bridge.QueryByStudyInstanceUid(studyInstanceUid).Count > 0;
		}

		private static bool StudyExistsOnRemote(IDicomServiceNode server, string studyInstanceUid)
		{
			IList<StudyRootStudyIdentifier> result = null;
			server.GetService<IStudyRootQuery>(s => result =
			                                        s.StudyQuery(new StudyRootStudyIdentifier {StudyInstanceUid = studyInstanceUid}));
			return result.Count > 0;
		}

		private static bool PublishFilesToLocal(ICollection<DicomFile> files)
		{
			try
			{
				Platform.GetService((IPublishFiles service) => service.PublishLocal(files));
				return true;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "An error occurred while attempting to publish files locally.");
			}
			return false;
		}

		private static bool PublishFilesToRemote(IDicomServiceNode destination, ICollection<DicomFile> files)
		{
			try
			{
				Platform.GetService((IPublishFiles service) => service.PublishRemote(files, destination));
				return true;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "An error occurred while attempting to publish files to server {0}.", destination.AETitle);
			}
			return false;
		}
	}
}