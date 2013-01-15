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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Common
{
    /// <summary>
    /// Setup application for the installer to set the AE Title and port of the DICOM Server.
    /// </summary>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    internal class ConfigureLocalServerApplication : IApplicationRoot
    {
        private class CommandLine : ClearCanvas.Common.Utilities.CommandLine
        {
            [CommandLineParameter("ae", "Sets the AE title of the local DICOM server.", Required = true)]
            public string AETitle { get; set;}

            [CommandLineParameter("host", "Sets the host name of the local DICOM server.", Required = false)]
            public string HostName { get; set; }

            [CommandLineParameter("port", "Sets the listening port of the local DICOM server.", Required = true)]
            public int Port { get; set; }

            [CommandLineParameter("filestore", "Sets the location of the file store.", Required = false)]
            public string FileStoreDirectory { get; set; }

            [CommandLineParameter("minspacepercent", "Sets the minimum used space required on the file store volume for the server to continue accepting studies.", Required = false)]
            public string MinimumFreeSpacePercent { get; set; }
        }

        #region Implementation of IApplicationRoot

        public void RunApplication(string[] args)
        {
            var commandLine = new CommandLine();
            try
            {
                commandLine.Parse(args);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Info, e);
                Console.WriteLine(e.Message);
                commandLine.PrintUsage(Console.Out);
                Environment.Exit(-1);
            }

            try
            {
                DicomServer.DicomServer.UpdateConfiguration(new DicomServerConfiguration
                                                                {
                                                                    HostName = commandLine.HostName,
                                                                    AETitle = commandLine.AETitle,
                                                                    Port = commandLine.Port
                                                                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); 
                Platform.Log(LogLevel.Warn, e);
                Environment.Exit(-1);
            }

            try
            {
                if (!String.IsNullOrEmpty(commandLine.FileStoreDirectory))
                    StudyStore.UpdateConfiguration(new StorageConfiguration
                                                   {
                                                       FileStoreDirectory = commandLine.FileStoreDirectory,
                                                       MinimumFreeSpacePercent =
                                                           commandLine.MinimumFreeSpacePercent != null
                                                               ? double.Parse(commandLine.MinimumFreeSpacePercent)
                                                               : StorageConfiguration.AutoMinimumFreeSpace
                                                   });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Platform.Log(LogLevel.Warn, e);
                Environment.Exit(-1);
            }
        }

        #endregion
    }
}
