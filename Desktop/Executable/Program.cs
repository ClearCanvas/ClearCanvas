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
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.Executable
{
	class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//Always at least try to let our application code handle the exception.
			//Setting this to "catch" means the Application.ThreadException event
			//will fire first, essentially causing the app to crash right away and shut down.
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

#if !MONO
			SplashScreenManager.DisplaySplashScreen();
#endif
			Platform.PluginManager.PluginLoaded += new EventHandler<PluginLoadedEventArgs>(OnPluginProgress);

			// check for command line arguments
            if (args.Length > 0)
            {
                // for the sake of simplicity, this is a naive implementation (probably needs to change in future)
                // if there is > 0 arguments, assume the first argument is a class name
                // and bundle the subsequent arguments into a secondary array which is 
                // forwarded to the application root class
                string[] args1 = new string[args.Length - 1];
                Array.Copy(args, 1, args1, 0, args1.Length);

                Platform.StartApp(args[0], args1);
            }
            else
            {
                Platform.StartApp(@"ClearCanvas.Desktop.Application", new string[0]);
            }
		}

		private static void OnPluginProgress(object sender, PluginLoadedEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
#if !MONO
			SplashScreenManager.SetStatus(e.Message);

			if (e.PluginAssembly != null)
				SplashScreenManager.AddAssemblyIcon(e.PluginAssembly);
#endif
        }
	}
}
