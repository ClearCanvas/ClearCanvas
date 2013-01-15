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
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    public class ShredCommunicationServiceType : IShredCommunication
    {
        public ShredCommunicationServiceType()
        {
            _className = this.GetType().ToString();
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");
        }

        #region IShredCommunication Members

        public void Pause()
        {
            Platform.Log(LogLevel.Info,_className + "[" + AppDomain.CurrentDomain.FriendlyName + "] Pause invoked");
            System.Diagnostics.Trace.WriteLine(_className + ": Pause invoked");
        }

        public void Stop()
        {
            Platform.Log(LogLevel.Info, _className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Stop invoked");
            System.Diagnostics.Trace.WriteLine(_className + ": Stop invoked");

        }

        public void Unpause()
        {
            Platform.Log(LogLevel.Info, _className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Unpause invoked");
            System.Diagnostics.Trace.WriteLine(_className + ": Unpause invoked");
        }

        #endregion

        #region Private Members
        private string _className;
        #endregion
    }
}
