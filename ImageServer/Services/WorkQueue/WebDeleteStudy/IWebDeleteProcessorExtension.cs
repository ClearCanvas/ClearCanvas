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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy
{
    public class WebDeleteProcessorContext
    {
        private readonly WebDeleteStudyItemProcessor _processor;
        private readonly DeletionLevel _level;
        private readonly string _reason;
        private readonly string _userId;
        private readonly string _userName;
        private readonly StudyStorageLocation _storageLocation;

        public WebDeleteProcessorContext(WebDeleteStudyItemProcessor processor, DeletionLevel level, StudyStorageLocation storageLocation, string reason, string userId, string userName)
        {
            _processor = processor;
            _storageLocation = storageLocation;
            _userName = userName;
            _userId = userId;
            _reason = reason;
            _level = level;
        }

        public WebDeleteStudyItemProcessor Processor
        {
            get { return _processor; }
        }

        public DeletionLevel Level
        {
            get { return _level; }
        }

        public string Reason
        {
            get { return _reason; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public StudyStorageLocation StorageLocation
        {
            get { return _storageLocation; }
        }

        public string UserId
        {
            get { return _userId; }
        }
    }
    public interface IWebDeleteProcessorExtension
    {
        void OnSeriesDeleting(WebDeleteProcessorContext context, Series series);
        void OnSeriesDeleted(WebDeleteProcessorContext context, Series series);
        void OnCompleted(WebDeleteProcessorContext context, IList<Series> series);
    }

    public class WebDeleteProcessorExtensionPoint:ExtensionPoint<IWebDeleteProcessorExtension>{}
}