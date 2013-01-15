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
using TimeoutException = System.ServiceProcess.TimeoutException;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Services
{
    public static class LocalServiceProcess
    {
        private const int TimeoutSeconds = 30;

        public static string Name
        {
            get { return ServiceControlSettings.Default.ServiceName; }
        }

        public static ServiceControllerStatus GetStatus()
        {
            CheckEnabled();
            using (ServiceController controller = CreateServiceController())
            {
                return controller.Status;
            }
        }

        public static void Start()
        {
            CheckEnabled();
            using (var controller = CreateServiceController())
            {
                if (controller.Status != ServiceControllerStatus.Running)
                {
                    //Note: don't remove the surrounding if because it tells us if the service name is wrong.
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(TimeoutSeconds));
                }

                if (controller.Status != ServiceControllerStatus.Running)
                    throw new TimeoutException();
            }
        }

        public static void Stop()
        {
            CheckEnabled();
            using (var controller = CreateServiceController())
            {
                if (controller.Status != ServiceControllerStatus.Stopped)
                {
                    //Note: don't remove the surrounding if because it tells us if the service name is wrong.
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(TimeoutSeconds));
                }

                if (controller.Status != ServiceControllerStatus.Stopped)
                    throw new TimeoutException();
            }
        }

        public static void Restart()
        {
            CheckEnabled();
            using (var controller = CreateServiceController())
            {
                if (controller.Status == ServiceControllerStatus.Running)
                {
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(TimeoutSeconds));
                    if (controller.Status != ServiceControllerStatus.Stopped)
                        throw new System.TimeoutException();
                }

                if (controller.Status == ServiceControllerStatus.Stopped)
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(TimeoutSeconds));
                    if (controller.Status != ServiceControllerStatus.Running)
                        throw new System.TimeoutException();
                }
                else
                {
                    throw new InvalidOperationException("The service is not in the correct state to be restarted.");
                }
            }
        }

        internal static bool IsAccessDeniedError(Exception e)
        {
            const int win32ErrorAccessDenied = 0x5;
            return e.InnerException != null && e.InnerException is Win32Exception && ((Win32Exception)e.InnerException).NativeErrorCode == win32ErrorAccessDenied;
        }

        private static void CheckEnabled()
        {
            if (!ServiceControlSettings.Default.Enabled)
                throw new InvalidOperationException("Local service process functionality is disabled, likely because the service does not exist.");
        }

        private static ServiceController CreateServiceController()
		{
			return new ServiceController(Name);
		}
    }
}
