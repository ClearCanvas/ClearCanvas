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

using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    internal class TierMigrateDatabaseUpdateCommand : ServerDatabaseCommand
    {
        #region Private Memeber
        private TierMigrationContext _context = null;
        #endregion

        public TierMigrateDatabaseUpdateCommand(TierMigrationContext context)
            : base("TierMigrateDatabaseUpdateCommand")
        {
            Context = context;
        }

        public TierMigrationContext Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            // update FilesystemStudyStorage
            if (Context != null)
            {
                Platform.Log(LogLevel.Info, "Updating database...");
                IFilesystemStudyStorageEntityBroker broker = updateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();

                FilesystemStudyStorageSelectCriteria searchCriteria = new FilesystemStudyStorageSelectCriteria();
                searchCriteria.StudyStorageKey.EqualTo(Context.OriginalStudyLocation.GetKey());
                searchCriteria.FilesystemKey.EqualTo(Context.OriginalStudyLocation.FilesystemKey);
                FilesystemStudyStorage filesystemStudyStorage = broker.FindOne(searchCriteria);
                Debug.Assert(filesystemStudyStorage != null);

                // Update Filesystem for the StudyStorage entry
                filesystemStudyStorage.FilesystemKey = Context.Destination.Filesystem.GetKey();
                broker.Update(filesystemStudyStorage);


                // Update Filesystem for the remaining FilesystemQueue entries
                IFilesystemQueueEntityBroker fsQueueBroker = updateContext.GetBroker<IFilesystemQueueEntityBroker>();                
                FilesystemQueueSelectCriteria fsQueueSearchCriteria = new FilesystemQueueSelectCriteria();
                fsQueueSearchCriteria.StudyStorageKey.EqualTo(Context.OriginalStudyLocation.GetKey());
                fsQueueSearchCriteria.FilesystemKey.EqualTo(Context.OriginalStudyLocation.FilesystemKey);

                FilesystemQueueUpdateColumns fsQueueUpdateColumns = new FilesystemQueueUpdateColumns();
                fsQueueUpdateColumns.FilesystemKey = Context.Destination.Filesystem.GetKey();
                fsQueueBroker.Update(fsQueueSearchCriteria, fsQueueUpdateColumns);
                
                // Insert or update Filesystem Queue table.
                IInsertFilesystemQueue insertFilesystemQueueBroker = updateContext.GetBroker<IInsertFilesystemQueue>();
                FilesystemQueueInsertParameters parms = new FilesystemQueueInsertParameters();
                parms.FilesystemKey = Context.Destination.Filesystem.GetKey();
                parms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.TierMigrate;
                parms.ScheduledTime = Platform.Time;
                parms.StudyStorageKey = Context.OriginalStudyLocation.GetKey();
                insertFilesystemQueueBroker.Execute(parms);

                Platform.Log(LogLevel.Info, "Database is updated.");
                
            }
        }
    }
}
