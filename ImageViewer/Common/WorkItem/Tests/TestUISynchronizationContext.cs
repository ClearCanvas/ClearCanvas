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

#if UNIT_TESTS

using System.Threading;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    //TODO (Marmot): should really test this scenario, too.
    internal class TestUISynchronizationContext : SynchronizationContext
    {
        //private readonly object _syncLock = new object();
        //private Thread _thread;
        //private readonly Queue<SendOrPostCallback> _callbacks = new Queue<SendOrPostCallback>();

        //public TestUISynchronizationContext()
        //{
        //    _thread = new Thread(RunThread);
        //    _thread.Name = String.Format("Simulated UI Thread [{0}]", _thread.ManagedThreadId);
        //    _thread.Start();
        //    SynchronizationContext.Current.Post()
        //}

        //private void RunThread(object ignore)
        //{

        //}

        //public void RunPump()
        //{
        //    lock(_syncLock)
        //    {

        //    }

        //}
    }
}

#endif