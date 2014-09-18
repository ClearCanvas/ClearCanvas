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
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    [ShredIsolation(Level = ShredIsolationLevel.None)]
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class DicomServerExtension : WcfShred
    {
		private readonly string _dicomServerEndpointName = "DicomServer";
		private bool _dicomServerWcfInitialized;

        public DicomServerExtension()
        {
			_dicomServerWcfInitialized = false;

			LicenseInformation.LicenseChanged += OnLicenseInformationChanged;
		}

        public override void Start()
        {
			try
			{                
				StartNetPipeHost<DicomServerServiceType, IDicomServer>(_dicomServerEndpointName, SR.DicomServer);
				_dicomServerWcfInitialized = true;
				string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.DicomServer);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);			    
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.DicomServer));
			}

            //NOTE: in a lot of cases, we start all the internal services before the WCF service,
            //but in this case, the (shared/offline) DICOM service configuration will call RestartListener
            //right after a change is made in the database, so we want to start the internal services
            //after the WCF service us up and running. That way, although unlikely, if the server configuration
            //were changed just as this service were starting up, we will always start up the listener
            //with the right AE title and Port, even if the listener starts then restarts in quick succession.
            //These internal services all nicely handle the possibility of a service calling into them when
            //they're not running yet, anyway.
            try
            {
                DicomServerManager.Instance.Start();

                string message = String.Format(SR.FormatServiceStartedSuccessfully, SR.DicomServer);
                Platform.Log(LogLevel.Info, message);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                Console.WriteLine(String.Format(SR.FormatServiceFailedToStart, SR.DicomServer));
            }
        }

        public override void Stop()
        {
			if (_dicomServerWcfInitialized)
        	{
        		try
        		{
        			StopHost(_dicomServerEndpointName);
					Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.DicomServer));
        		}
        		catch (Exception e)
        		{
        			Platform.Log(LogLevel.Error, e);
        		}
        	}

			try
			{
				DicomServerManager.Instance.Stop();
				Platform.Log(LogLevel.Info, String.Format(SR.FormatServiceStoppedSuccessfully, SR.DicomServer));
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

        public override string GetDisplayName()
        {
			return SR.DicomServer;
        }

        public override string GetDescription()
        {
			return SR.DicomServerDescription;
        }

    	private void OnLicenseInformationChanged(object sender, EventArgs e)
    	{
    		Platform.Log(LogLevel.Info, @"Restarting {0} due to application licensing status change.", SR.DicomServer);
    		DicomServerManager.Instance.Restart();
    	}
   }
}