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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CleanupReconcile
{

    [ExtensionOf(typeof(WorkQueueFactoryExtensionPoint))]
    public class CleanupReconcileFactoryExtension : IWorkQueueProcessorFactory
    {
        #region Private Members
        #endregion

        #region Constructors
        public CleanupReconcileFactoryExtension()
        { }
        #endregion

        #region IWorkQueueProcessorFactory Members

        public WorkQueueTypeEnum GetWorkQueueType()
        {
            return WorkQueueTypeEnum.ReconcileCleanup ;
        }

        public IWorkQueueItemProcessor GetItemProcessor()
        {
            CleanupReconcileItemProcessor processor = new CleanupReconcileItemProcessor();
            processor.Name = GetWorkQueueType().ToString();
            return processor;
        }

        #endregion
    }
}
