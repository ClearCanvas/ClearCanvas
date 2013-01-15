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
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[Serializable]
	internal class DicomServerException : Exception
	{
		public DicomServerException(string message)
			: base(message)
		{ 
		}

		protected DicomServerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ 
		}
	}

    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall)]
    public class DicomServerServiceType : IDicomServer
    {
    	#region IDicomServer Members

        public GetListenerStateResult GetListenerState(GetListenerStateRequest request)
        {
            return new GetListenerStateResult {State = DicomServerManager.Instance.State};
        }

        public RestartListenerResult RestartListener(RestartListenerRequest request)
        {
            try
            {
                DicomServerManager.Instance.Restart();
                return new RestartListenerResult();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                //we throw a serializable, non-FaultException-derived exception so that the 
                //client channel *does* get closed.
                string message = SR.ExceptionErrorRestartingDICOMServer;
                message += "\nDetail: " + e.Message;
                throw new DicomServerException(message);
            }
        }

        #endregion
    }
}
