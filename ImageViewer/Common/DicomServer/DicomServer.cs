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
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    public abstract class DicomServer : IDicomServer
    {
        static DicomServer()
        {
            try
            {
                var service = Platform.GetService<IDicomServer>();
                IsSupported = service != null;
                var disposable = service as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            catch(EndpointNotFoundException)
            {
                //This doesn't mean it's not supported, it means it's not running.
                IsSupported = true;
            }
            catch (NotSupportedException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Local DICOM Server is not supported.");
            }
            catch (UnknownServiceException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Local DICOM Server is not supported.");
            }
            catch (Exception e)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, e, "Local DICOM Server is not supported.");
            }
        }

        public static bool IsSupported { get; private set; }

        public static bool IsListening
        {
            get
            {
                {
                    if (!IsSupported)
                        return false;

                    try
                    {
                        bool state = false;
                        Platform.GetService<IDicomServer>(s =>
                            state = s.GetListenerState(new GetListenerStateRequest()).State == ServiceStateEnum.Started);
                        return state;
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Debug, e, "The local DICOM Server could not be contacted.");
                        return false;
                    }
                }
            }
        }

        public static string AETitle
        {
            get { return GetConfiguration().AETitle; }
        }

        public static string HostName
        {
            get { return GetConfiguration().HostName; }
        }

        public static int Port
        {
            get { return GetConfiguration().Port; }
        }

        public static DicomServerConfiguration GetConfiguration()
        {
            DicomServerConfiguration configuration = null;
                Platform.GetService<IDicomServerConfiguration>(
                    s => configuration = s.GetConfiguration(new GetDicomServerConfigurationRequest()).Configuration);
            return configuration;
        }

        public static void UpdateConfiguration(DicomServerConfiguration configuration)
        {
            Platform.GetService<IDicomServerConfiguration>(
                s => s.UpdateConfiguration(new UpdateDicomServerConfigurationRequest{Configuration = configuration}));
        }

        public static DicomServerExtendedConfiguration GetExtendedConfiguration()
        {
            DicomServerExtendedConfiguration configuration = null;
            Platform.GetService<IDicomServerConfiguration>(
                s => configuration = s.GetExtendedConfiguration(new GetDicomServerExtendedConfigurationRequest()).ExtendedConfiguration);
            return configuration;
        }

        public static void UpdateExtendedConfiguration(DicomServerExtendedConfiguration configuration)
        {
            Platform.GetService<IDicomServerConfiguration>(
                s => s.UpdateExtendedConfiguration(new UpdateDicomServerExtendedConfigurationRequest { ExtendedConfiguration = configuration }));
        }

        public static void RestartListener()
        {
            Platform.GetService<IDicomServer>(s => s.RestartListener(new RestartListenerRequest()));
        }

        #region IDicomServer Members

        public abstract GetListenerStateResult GetListenerState(GetListenerStateRequest request);
        public abstract RestartListenerResult RestartListener(RestartListenerRequest request);

        #endregion
    }
}
