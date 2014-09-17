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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;
using ClearCanvas.ImageServer.Services.Streaming.Shreds;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming
{
    /// <summary>
    /// Loads the compressed study header stream.
    /// </summary>
    internal class HeaderLoader
    {
        private readonly HeaderLoaderStatistics _statistics = new HeaderLoaderStatistics();
        private readonly string _partitionAE;
        private readonly string _studyInstanceUid;
        private StudyStorageLocation _studyLocation;
    	private string _faultDescription;
        
        private static bool _enableCache = ImageStreamingServerSettings.Default.EnableCache;
        private static TimeSpan _cacheRetentionTime = ImageStreamingServerSettings.Default.CacheRetentionWindow;

        #region Constructor

        public HeaderLoader(HeaderStreamingContext context)
        {
            _studyInstanceUid = context.Parameters.StudyInstanceUID;
            _partitionAE = context.Parameters.ServerAETitle;
			_statistics.FindStudyFolder.Start();
            string sessionId = context.CallerAE;
            
            ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(_partitionAE);

            StudyStorageLoader storageLoader = new StudyStorageLoader(sessionId);
            storageLoader.CacheEnabled = _enableCache;
            storageLoader.CacheRetentionTime = _cacheRetentionTime;
            StudyLocation = storageLoader.Find(_studyInstanceUid, partition);

            if (StudyLocation != null && StudyLocation.QueueStudyStateEnum != QueueStudyStateEnum.Idle)
            {
                Platform.Log(LogLevel.Warn, "Accessing to study {0} while its current state is {1}",
                             StudyLocation.StudyInstanceUid,  StudyLocation.QueueStudyStateEnum);
            }

            _statistics.FindStudyFolder.End();
        }

        #endregion

        #region Public Properties

        public HeaderLoaderStatistics Statistics
        {
            get { return _statistics; }
        }

        public bool StudyExists
        {
            get { return StudyLocation != null; }
        }

        public StudyStorageLocation StudyLocation
        {
            get { return _studyLocation; }
            set { _studyLocation = value; }
		}

    	public string FaultDescription
    	{
			get { return _faultDescription; }
			set { _faultDescription = value; }    		
    	}

		#endregion

		#region Public Methods
		/// <summary>
		/// Loads the compressed header stream for the study with the specified study instance uid
		/// </summary>
		/// <returns>
		/// The compressed study header stream or null if the study doesn't exist.
		/// </returns>
		/// <remarks>
		/// </remarks>
		public Stream Load()
		{
			if (!StudyExists)
				return null;

			_statistics.LoadHeaderStream.Start();
			String studyPath = StudyLocation.GetStudyPath();
			if (!Directory.Exists(studyPath))
			{
				// the study exist in the database but not on the filesystem.

				// TODO: If the study is migrated to another tier and the study folder is removed, 
				// we may want to do something here instead of throwing exception.
				_statistics.LoadHeaderStream.End();
				throw new ApplicationException(String.Format("Study Folder {0} doesn't exist", studyPath));
			}

			String compressedHeaderFile = Path.Combine(studyPath, _studyInstanceUid + ".xml.gz");
            Stream headerStream = null;
			Platform.Log(LogLevel.Debug, "Study Header Path={0}", compressedHeaderFile);
			try
			{
			    headerStream = FileStreamOpener.OpenForRead(compressedHeaderFile, FileMode.Open, 30000 /* try for 30 seconds */);
			}
            catch(FileNotFoundException)
            {
                throw;
            }
            catch(IOException ex)
            {
                // treated as sharing violation
                throw new StudyAccessException("Study header is not accessible at this time.", StudyLocation.QueueStudyStateEnum, ex);
            }
			_statistics.LoadHeaderStream.End();
            _statistics.Size = (ulong)headerStream.Length;
            return headerStream;
		}

    	#endregion Public Methods
    }
}