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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	[ButtonAction("activate", "servertree-toolbar/ToolbarVerify", "VerifyServer")]
	[MenuAction("activate", "servertree-contextmenu/MenuVerify", "VerifyServer")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipVerify")]
	[IconSet("activate", "Icons.VerifyServerToolSmall.png", "Icons.VerifyServerToolMedium.png", "Icons.VerifyServerToolLarge.png")]
	[ExtensionOf(typeof(ServerTreeToolExtensionPoint))]
	public class VerifyServerTool : ServerTreeTool
	{
		private bool NoServersSelected()
		{
			return this.Context.SelectedServers == null || this.Context.SelectedServers.Count == 0;
		}

		private void VerifyServer()
		{
			BlockingOperation.Run(this.InternalVerifyServer);
		}

		private void InternalVerifyServer()
		{
			if (this.NoServersSelected())
			{
				//should never get here because the verify button should be disabled.
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageNoServersSelected, MessageBoxActions.Ok);
				return;
			}

		    try
		    {
		        var localServer = ServerDirectory.GetLocalServer();

                var msgText = new StringBuilder();
                msgText.AppendFormat(SR.MessageCEchoVerificationPrefix + "\r\n\r\n");
                foreach (var server in this.Context.SelectedServers)
                {
                    using (var scu = new VerificationScu())
                    {
                        VerificationResult result = scu.Verify(localServer.AETitle, server.AETitle, server.ScpParameters.HostName, server.ScpParameters.Port);
                        if (result == VerificationResult.Success)
                            msgText.AppendFormat(SR.MessageCEchoVerificationSingleServerResultSuccess + "\r\n", server.Name);
                        else
                            msgText.AppendFormat(SR.MessageCEchoVerificationSingleServerResultFail + "\r\n", server.Name);

                        // must wait for the SCU thread to release the connection properly before disposal, otherwise we might end up aborting the connection instead
                        scu.Join(new TimeSpan(0, 0, 2));
                    }
                }

                msgText.AppendFormat("\r\n");
                this.Context.DesktopWindow.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
		    }
		    catch (Exception e)
		    {
                ExceptionHandler.Report(e, base.Context.DesktopWindow);
		    }
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			this.Enabled = !this.Context.SelectedServers.IsLocalServer && !this.NoServersSelected();                 
		}
	}
}