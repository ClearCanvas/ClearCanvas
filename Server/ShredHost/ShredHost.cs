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
using ClearCanvas.Common;


namespace ClearCanvas.Server.ShredHost
{
    public static class ShredHost
    {
		#region Private Members
		private static ShredControllerList _shredInfoList;
		private static ServiceEndpointDescription _sed;
		private static RunningState _runningState;
		private static object _lockObject = new object();
    	private static bool _shredHostWCFInitialized = false;
		#endregion

		/// <summary>
        /// Starts the ShredHost routine.
        /// </summary>
        /// <returns>true - if the ShredHost is currently running, false - if ShredHost is stopped.</returns>
        public static bool Start()
        {
			// install the unhandled exception event handler
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += MyUnhandledExceptionEventHandler;

            lock (_lockObject)
            {
                if (RunningState.Running == _runningState || RunningState.Transition == _runningState)
                    return (RunningState.Running == _runningState);

                _runningState = RunningState.Transition;
            }

            Platform.Log(LogLevel.Info, "Starting up in AppDomain [" + AppDomain.CurrentDomain.FriendlyName + "]");

            // the ShredList and shreds objects are proxy objects that actually exist
            // in the secondary AppDomain
			//AppDomain stagingDomain = AppDomain.CreateDomain("StagingDomain");
            ExtensionScanner scanner = (ExtensionScanner)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, "ClearCanvas.Server.ShredHost.ExtensionScanner");
            ShredStartupInfoList shredStartupInfoList = null;

            try
            {
                shredStartupInfoList = scanner.ScanExtensions();
            }
            catch (PluginException pluginException)
            {
                // There was a problem loading the plugins, including if there were no plugins found
                // This is an innocuous problem, and just means that there are no shreds to run
                Platform.Log(LogLevel.Warn, pluginException);
            }

            StartShreds(shredStartupInfoList);

            // all the shreds have been created, so we can dismantle the secondary domain that was used 
            // for scanning for all Extensions that are shreds
			//AppDomain.Unload(stagingDomain);

			//try
			//{
			//    _sed = WcfHelper.StartHttpHost<ShredHostServiceType, IShredHost>(
			//        "ShredHost", "Host program of multiple independent service-like sub-programs", ShredHostServiceSettings.Instance.ShredHostHttpPort);
			//    _shredHostWCFInitialized = true;
			//    string message = String.Format("The ShredHost WCF service has started on port {0}.", ShredHostServiceSettings.Instance.ShredHostHttpPort);
			//    Platform.Log(LogLevel.Info, message);
			//    Console.WriteLine(message);
			//}
			//catch(Exception	e)
			//{
			//    Platform.Log(LogLevel.Error, e);
			//    Console.WriteLine("The ShredHost WCF service has failed to start.  Please check the log for more details.");
			//}

        	lock (_lockObject)
			{
				_runningState = RunningState.Running;
			}
			
			return (RunningState.Running == _runningState);
        }

        /// <summary>
        /// Stops the running ShredHost.
        /// </summary>
        /// <returns>true - if the ShredHost is running, false - if the ShredHost is stopped.</returns>
        public static bool Stop()
        {
            lock (_lockObject)
            {
                if (RunningState.Stopped == _runningState || RunningState.Transition == _runningState)
                    return (RunningState.Running == _runningState);

                _runningState = RunningState.Transition;
            }

            // correct sequence should be to stop the WCF host so that we don't
            // receive any more incoming requests
			Platform.Log(LogLevel.Info, "ShredHost stop request received");

			if (_shredHostWCFInitialized)
			{
				try
				{
					WcfHelper.StopHost(_sed);
					Platform.Log(LogLevel.Info, "The ShredHost WCF service has stopped.");
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

        	StopShreds();
            Platform.Log(LogLevel.Info, "Completing ShredHost stop.");

            _shredInfoList.Clear();
            lock (_lockObject)
            {
                _runningState = RunningState.Stopped;
            }

            return (RunningState.Running == _runningState);
        }

        static public bool IsShredHostRunning
        {
            get
            {
                bool isRunning;
                lock (_lockObject)
                {
                    isRunning = (RunningState.Running == _runningState);
                }

                return isRunning;
            }
        }

        static public bool StartShred(WcfDataShred shred)
        {
            Platform.Log(LogLevel.Info, "Attempting to start shred: " + shred.Name);
            return ShredControllerList[shred.Id].Start();
        }

        static public bool StopShred(WcfDataShred shred)
        {
            Platform.Log(LogLevel.Info, "Attempting to stop shred: " + shred.Name);
            return ShredControllerList[shred.Id].Stop();
        }

        static ShredHost()
        {
            _shredInfoList = new ShredControllerList();
            _sed = null;
            _runningState = RunningState.Stopped;
        }

        private static void StartShreds(ShredStartupInfoList shredStartupInfoList)
        {
            if (null != shredStartupInfoList)
            {
                // create the data structure that will hold the shreds and their thread, etc. related objects
                foreach (ShredStartupInfo shredStartupInfo in shredStartupInfoList)
                {
                    if (null != shredStartupInfo)
                    {
                        // clone the shredStartupInfo structure into the current AppDomain, otherwise, once the StagingDomain 
                        // has been unloaded, the shredStartupInfo structure will be destroyed
                        ShredStartupInfo newShredStartupInfo = new ShredStartupInfo(shredStartupInfo.AssemblyPath, shredStartupInfo.ShredName, shredStartupInfo.ShredTypeName);
                        
                        // create the controller that will allow us to start and stop the shred
                        ShredController shredController = new ShredController(newShredStartupInfo);
                        _shredInfoList.Add(shredController);
                    }

                }
            }

            foreach (ShredController shredController in _shredInfoList)
            {
                shredController.Start();
            }
        }

        private static void StopShreds()
        {
            Exception savedException = null;

            foreach (ShredController shredController in _shredInfoList)
            {
                try
                {
                    string displayName = shredController.Shred.GetDisplayName();
                    Platform.Log(LogLevel.Info, displayName + ": Signalling stop");
                    shredController.Stop();
                    Platform.Log(LogLevel.Info, displayName + ": Stopped");
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Fatal, e, "Unexepected exception stopping Shred (shred still running): {0}", shredController.Shred.GetDisplayName());
                    savedException = e;
                }
            }

            if (savedException != null) throw savedException;
        }

        #region Print asms in AD helper f(x)
        public static void PrintAllAssembliesInAppDomain(AppDomain ad)
        {
            Assembly[] loadedAssemblies = ad.GetAssemblies();
            Console.WriteLine(@"***** Here are the assemblies loaded in {0} *****\n",
                ad.FriendlyName);
            foreach (Assembly a in loadedAssemblies)
            {
                Console.WriteLine(@"-> Name: {0}", a.GetName().Name);
                Console.WriteLine(@"-> Version: {0}", a.GetName().Version);
            }
        }
        #endregion

		internal static void MyUnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs args)
		{
			Exception e = (Exception)args.ExceptionObject;
			Platform.Log(LogLevel.Fatal, e, "Fatal error - unhandled exception in running Shred; ShredHost must terminate");
		}

        internal static ShredControllerList ShredControllerList
        {
            get { return _shredInfoList; }
        }
    }    
}
