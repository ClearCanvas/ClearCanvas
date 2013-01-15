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

namespace ClearCanvas.Server.ShredHost
{
    /// <summary>
    /// The logic is that a program can be stopped, running, or in transition between stopped and running or vice versa.
    /// During transition, no other state changes are permitted. For example, Thread A attempts to start the ShredHost;
    /// the Start() routine checks to see whether the ShredHost is in a stopped state, because it doesn't make sense to
    /// start a running ShredHost. Once this checks out, the running state is changed to Transition. No methods should be 
    /// allowed to proceed with changing the state while it is in Transition. Once the ShredHost has started up, 
    /// the running state can then be set to Running. If Thread B had tried to stop the ShredHost while it was in Transition
    /// nothing would have happend. IF Thread B tried to stop ShredHost while it is in Running, it will be stopped
    /// successfully.
    /// </summary>
    internal enum RunningState { Stopped, Running, Transition };
}
