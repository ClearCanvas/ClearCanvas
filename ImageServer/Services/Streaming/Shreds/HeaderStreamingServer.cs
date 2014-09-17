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
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Services.Streaming.Shreds
{
	/// <summary>
	/// Plugin to handle streaming request for the ImageServer.
	/// </summary>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	[ShredIsolationAttribute(Level = ShredIsolationLevel.None)]
	public class HeaderStreamingServer : WcfShred
	{

		#region Private Members

		private readonly string _className;

		#endregion

		#region Constructors

		public HeaderStreamingServer()
		{
			_className = GetType().ToString();
		}

		#endregion

		#region IShred Implementation Shred Override

		public override void Start()
		{
			Platform.Log(LogLevel.Debug, "{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

			try
			{
				Platform.Log(LogLevel.Info, "Starting {0} using basic Http binding", GetDisplayName());
                StartBasicHttpHost<HeaderStreamingService, IHeaderStreamingService>("HeaderStreaming", GetDescription());
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Fatal, e, "Unexpected exception starting Streaming Server Shred");

			    ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, SR.HeaderStreamingServerDisplayName,
                                     AlertTypeCodes.UnableToStart, null, TimeSpan.Zero,
			                         SR.AlertUnableToStart, e.Message);
			}
		}

		public override void Stop()
		{
			Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
            StopHost("HeaderStreaming");
		}

		public override string GetDisplayName()
		{
			return SR.HeaderStreamingServerDisplayName;
		}

		public override string GetDescription()
		{
			return SR.HeaderStreamingServerDescription;
		}

		#endregion
	}
}