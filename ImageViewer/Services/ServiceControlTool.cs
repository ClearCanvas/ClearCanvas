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
using System.ServiceProcess;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using TimeoutException = System.TimeoutException;

namespace ClearCanvas.ImageViewer.Services
{
    [MenuAction("start", "global-menus/MenuTools/MenuService/MenuStart", "StartService")]
	[EnabledStateObserver("start", "StartEnabled", "EnabledChanged")]
	[IconSet("start", "Icons.StartServiceToolSmall.png", "Icons.StartServiceToolMedium.png", "Icons.StartServiceToolLarge.png")]
	[ViewerActionPermission("start", ImageViewer.AuthorityTokens.Administration.Services)]

    [MenuAction("stop", "global-menus/MenuTools/MenuService/MenuStop", "StopService")]
	[EnabledStateObserver("stop", "StopEnabled", "EnabledChanged")]
	[IconSet("stop", "Icons.StopServiceToolSmall.png", "Icons.StopServiceToolMedium.png", "Icons.StopServiceToolLarge.png")]
	[ViewerActionPermission("stop", ImageViewer.AuthorityTokens.Administration.Services)]

    [MenuAction("restart", "global-menus/MenuTools/MenuService/MenuRestart", "RestartService")]
	[EnabledStateObserver("restart", "StopEnabled", "EnabledChanged")]
	[IconSet("restart", "Icons.RestartServiceToolSmall.png", "Icons.RestartServiceToolMedium.png", "Icons.RestartServiceToolLarge.png")]
	[ViewerActionPermission("restart", ImageViewer.AuthorityTokens.Administration.Services)]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	internal class ServiceControlTool : Tool<IDesktopToolContext>
	{
        private enum ServiceControlOperation
        {
            Start,
            Stop,
            Restart
        }
        
        private Timer _timer;
		private bool _startEnabled;
		private bool _stopEnabled;

		public bool StartEnabled
		{
			get { return _startEnabled; }
			set
			{
				if (_startEnabled == value)
					return;

				_startEnabled = value;
				EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
			}
		}

        public override IActionSet Actions
        {
            get
            {
                if (!ServiceControlSettings.Default.Enabled)
                    return new ActionSet();

                return base.Actions;
            }
        }
		public bool StopEnabled
		{
			get { return _stopEnabled; }
			set
			{
				if (_stopEnabled == value)
					return;

				_stopEnabled = value;
				EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler EnabledChanged;

		public override void Initialize()
		{
			base.Initialize();

            if (ServiceControlSettings.Default.Enabled)
    			Refresh();
		}

		protected override void Dispose(bool disposing)
		{
            if (ServiceControlSettings.Default.Enabled)
                StopTimer();
			
            base.Dispose(disposing);
		}

		#region Refresh

		private void StartTimer()
		{
			if (_timer != null)
				return;

			_timer = new Timer(OnTimer) { IntervalMilliseconds = 10000 };
			_timer.Start();
		}

		private void StopTimer()
		{
			if (_timer == null)
				return;
			
			_timer.Stop();
			_timer = null;
		}

		private void OnTimer(object nothing)
		{
			if (_timer != null)
				Refresh();
		}

		private void OnUnknownService()
		{
			Platform.Log(LogLevel.Debug, "Failed to determine state of service '{0}' because it doesn't exist.", LocalServiceProcess.Name);
			
			StopTimer();
			StartEnabled = true; //Leave one button enabled so the user can try to refresh status.
			StopEnabled = false;
		}

		private void OnUnknownError(Exception e)
		{
            Platform.Log(LogLevel.Debug, e, "Failed to determine state of service '{0}'.", LocalServiceProcess.Name);

			StopTimer();
			StartEnabled = true; //Leave one button enabled so the user can try to refresh status.
			StopEnabled = false;
		}

		private void Refresh()
		{
			try
			{
			    var status = LocalServiceProcess.GetStatus();
				StartEnabled = status == ServiceControllerStatus.Stopped;
                StopEnabled = status == ServiceControllerStatus.Running;

				StartTimer();
			}
			catch (InvalidOperationException)
			{
				OnUnknownService();
			}
			catch (Exception e)
			{
				OnUnknownError(e);
			}
		}

		#endregion


		#region Service Control

		public void StopService()
		{
			BlockingOperation.Run(() => ControlService(ServiceControlOperation.Stop));
		}

		public void StartService()
		{
			BlockingOperation.Run(() => ControlService(ServiceControlOperation.Start));
		}

		public void RestartService()
		{
			BlockingOperation.Run(() => ControlService(ServiceControlOperation.Restart));
		}


        private void ControlService(ServiceControlOperation operation)
        {
            try
            {
                switch (operation)
                {
                    case ServiceControlOperation.Start:
                        LocalServiceProcess.Start();
                        break;
                    case ServiceControlOperation.Stop:
                        LocalServiceProcess.Stop();
                        break;
                    case ServiceControlOperation.Restart:
                        LocalServiceProcess.Restart();
                        break;
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Info, e, "Failed to {0} service '{1}'.", operation.ToString().ToLower(), LocalServiceProcess.Name);

                if (LocalServiceProcess.IsAccessDeniedError(e))
                    Context.DesktopWindow.ShowMessageBox(SR.MessageControlServiceAccessDenied, MessageBoxActions.Ok);
                else if (e is TimeoutException)
                    Context.DesktopWindow.ShowMessageBox(SR.MessageControlServiceTimeout, MessageBoxActions.Ok);
                else
                    Context.DesktopWindow.ShowMessageBox(SR.MessageFailedToStartService, MessageBoxActions.Ok);
            }

            Refresh();
        }

	    #endregion
	}
}
