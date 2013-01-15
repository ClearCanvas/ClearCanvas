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
using System.IO;
using System.ServiceProcess;

namespace ClearCanvas.Server.ShredHostService
{
	public partial class ShredHostService : ServiceBase
	{
		private const int _serviceOperationTimeout = 3*60*1000;

		internal static void InternalStart()
		{
			// the default startup path is in the system folder
			// we need to change this to be able to scan for plugins and to log
			string startupPath = AppDomain.CurrentDomain.BaseDirectory;
			Directory.SetCurrentDirectory(startupPath);
			ShredHost.ShredHost.Start();
		}

		internal static void InternalStop()
		{
			ShredHost.ShredHost.Stop();
		}

		public ShredHostService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			RequestAdditionalTime(_serviceOperationTimeout);

			InternalStart();
		}

		protected override void OnStop()
		{
			RequestAdditionalTime(_serviceOperationTimeout);

			InternalStop();
		}
	}
}