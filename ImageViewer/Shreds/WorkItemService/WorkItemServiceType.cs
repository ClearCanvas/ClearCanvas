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
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    //TODO (Marmot): Shouldn't throw real exceptions across service boundaries.
    [Serializable]
    internal class WorkItemServiceException : Exception
    {
        public WorkItemServiceException(string message)
            : base(message)
        {
        }

        protected WorkItemServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WorkItemServiceType : IWorkItemService
    {
        public WorkItemInsertResponse Insert(WorkItemInsertRequest request)
        {
            try
            {
                return WorkItemService.Instance.Insert(request);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                var message = SR.ExceptionErrorProcessingInsert;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public WorkItemUpdateResponse Update(WorkItemUpdateRequest request)
        {
            try
            {
                return WorkItemService.Instance.Update(request);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                var message = SR.ExceptionErrorProcessingUpdate;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public WorkItemQueryResponse Query(WorkItemQueryRequest request)
        {
            try
            {
                return WorkItemService.Instance.Query(request);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                var message = SR.ExceptionErrorProcessingQuery;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }
    }
}
