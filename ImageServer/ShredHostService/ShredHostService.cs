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
using System.Reflection;
using System.ServiceProcess;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.ShredHostService
{
    public partial class ShredHostService : ServiceBase
    {
        private static Assembly _assembly;
        private static Type _shredHostType;

        internal static void InternalStart()
        {
            Platform.Log(LogLevel.Info, "Starting Server Shred Host Service...");

            ServerPlatform.Alert(AlertCategory.System, AlertLevel.Informational,
                                 SR.AlertComponentName, AlertTypeCodes.Starting,
                                 null, TimeSpan.Zero,
                                 SR.AlertShredHostServiceStarting);

            // the default startup path is in the system folder
            // we need to change this to be able to scan for plugins and to log
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            System.IO.Directory.SetCurrentDirectory(startupPath);

            // we choose to dynamically load the ShredHost dll so that we can bypass
            // the requirement that the ShredHost dll be Strong Name signed, i.e.
            // if we were to reference it directly in the the project at design time
            // ClearCanvas.Server.ShredHost.dll would also need to be Strong Name signed
            _assembly = Assembly.Load("ClearCanvas.Server.ShredHost");
            _shredHostType = _assembly.GetType("ClearCanvas.Server.ShredHost.ShredHost");
            _shredHostType.InvokeMember("Start", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,null, null, new object[] { });
        }

        internal static void InternalStop()
        {
            _shredHostType.InvokeMember("Stop", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
                null, null, new object[] { });


            ServerPlatform.Alert(AlertCategory.System, AlertLevel.Informational,
                                 SR.AlertComponentName, AlertTypeCodes.Stopped,
                                 null, TimeSpan.Zero,
                                 SR.AlertShredHostServiceStopped);
        }

        public ShredHostService()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            InternalStart();
        }

        protected override void OnStop()
        {
            InternalStop();
        }
    }
}
