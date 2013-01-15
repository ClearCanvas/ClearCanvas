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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration
{
	public class PriorsServersConfigurationComponent : ServerTreeConfigurationComponent
	{
		public PriorsServersConfigurationComponent()
			:base(SR.DescriptionPriorsServers, GetPriorsServers())
		{
		}

        private static DicomServiceNodeList GetPriorsServers()
        {
            try
            {
                var priorsServers = ServerDirectory.GetPriorsServers(false);
                return new DicomServiceNodeList(priorsServers);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Error initializing priors servers from directory.");
                return new DicomServiceNodeList();
            }
        }

		public override void Save()
		{
		    try
		    {
		        List<ServerDirectoryEntry> allEntries = null;
		        Platform.GetService<IServerDirectory>(s => allEntries = s.GetServers(new GetServersRequest()).ServerEntries);

                var changedEntries = new List<ServerDirectoryEntry>();

                foreach (var existingEntry in allEntries)
                {
                    var isChecked = CheckedServers.Any(s => s.Name == existingEntry.Server.Name);
                    if (existingEntry.IsPriorsServer == isChecked)
                        continue;

                    existingEntry.IsPriorsServer = isChecked;
                    changedEntries.Add(existingEntry);
                }
                
                Platform.GetService(delegate(IServerDirectory service)
		                                {
                                            foreach (var changedEntry in changedEntries)
                                                service.UpdateServer(new UpdateServerRequest{ServerEntry = changedEntry});
		                                });

		    }
		    catch (Exception e)
		    {
		        ExceptionHandler.Report(e, SR.MessageFailedToSavePriorsServers, Host.DesktopWindow);
		    }
		}
	}
}
