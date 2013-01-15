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
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredController : MarshalByRefObject
    {
        public ShredController(ShredStartupInfo startupInfo)
        {
            Platform.CheckForNullReference(startupInfo, "startupInfo");

            _startupInfo = startupInfo;
            _id = ShredController.NextId;
            _runningState = RunningState.Stopped;
        }

        static ShredController()
        {
            _nextId = 101;
        }

        public bool Start()
        {
            lock (_lockRunningState)
            {
                if (RunningState.Running == _runningState || RunningState.Transition == _runningState)
                    return (RunningState.Running == _runningState);

                _runningState = RunningState.Transition;
            }

            // create the AppDomain that the shred object will be instantiated in
            _domain = AppDomain.CreateDomain(_startupInfo.ShredTypeName);
            _shredObject = (IShred)_domain.CreateInstanceFromAndUnwrap(_startupInfo.AssemblyPath.LocalPath, _startupInfo.ShredTypeName);
            
            // cache the shred's details so that even if the shred is stopped and unloaded
            // we still have it's display name 
            _shredCacheObject = new ShredCacheObject(_shredObject.GetDisplayName(), _shredObject.GetDescription());
            
            // create the thread and start it
            _thread = new Thread(new ParameterizedThreadStart(StartupShred));
            _thread.Name = String.Format("{0}", _shredCacheObject.GetDisplayName());
            _thread.Start(this);

            lock (_lockRunningState)
            {
                _runningState = RunningState.Running;
            }

            return (RunningState.Running == _runningState);
        }


        public bool Stop()
        {
            lock (_lockRunningState)
            {
                if (RunningState.Stopped == _runningState || RunningState.Transition == _runningState)
                    return (RunningState.Running == _runningState);

                _runningState = RunningState.Transition;
            }

            _shredObject.Stop();
            _thread.Join();
            AppDomain.Unload(_domain);
            _domain = null;         // need to explicity set to null, otherwise any references to it in the future will throw an exception
            _shredObject = null;
            _thread = null;

            lock (_lockRunningState)
            {
                _runningState = RunningState.Stopped;
            }

            return (RunningState.Running == _runningState);
        }

        private void StartupShred(object threadData)
        {
            ShredController shredController = threadData as ShredController;
            IWcfShred wcfShred = shredController.Shred as IWcfShred;
            try
            {
                if (wcfShred != null)
                {
                    wcfShred.SharedHttpPort = ShredHostServiceSettings.Instance.SharedHttpPort;
                    wcfShred.SharedTcpPort = ShredHostServiceSettings.Instance.SharedTcpPort;
                    wcfShred.ServiceAddressBase = ShredHostServiceSettings.Instance.ServiceAddressBase;
                }
                shredController.Shred.Start();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when starting up Shred {0}",
                             shredController.Shred.GetDescription());
            }
        }

        private class ShredCacheObject : IShred
        {
            public ShredCacheObject(string displayName, string description)
            {
                _displayName = displayName;
                _description = description;
            }

            #region Private fields
            private string _displayName;
            private string _description;
            #endregion

            #region IShred Members

            public void Start()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void Stop()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public string GetDisplayName()
            {
                return _displayName;
            }

            public string GetDescription()
            {
                return _description;
            }

            #endregion
        }

        #region Private fields
        private static object _lockShredId = new object();
        private static object _lockRunningState = new object();
        private RunningState _runningState;
        #endregion

        #region Properties
        private Thread _thread;
        private IShred _shredObject;
        private ShredCacheObject _shredCacheObject;
        private AppDomain _domain;
        private ShredStartupInfo _startupInfo;
        private int _id;
        private static int _nextId;
        protected static int NextId
        {
            get { return _nextId++; }
        }

        public int Id
        {
            get { return _id; }
        }

        public ShredStartupInfo StartupInfo
        {
            get { return _startupInfo; }
        }

        public IShred Shred
        {
            get 
            {
                if (null == _shredObject)
                    return _shredCacheObject;
                else
                    return _shredObject; 
            }
        }

        public WcfDataShred WcfDataShred
        {
            get
            {
                return new WcfDataShred(this.Id, this.Shred.GetDisplayName(), this.Shred.GetDescription(), (RunningState.Running == _runningState) ? true : false);
            }
        }
	
        #endregion
    }
}
