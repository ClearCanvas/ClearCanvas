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
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Core.Events
{
    /// <summary>
    /// Class for firing events within the ImageServer.
    /// </summary>
    /// <remarks>
    /// The <see cref="EventManager"/> manages all plugins that have been registered to receive an event as defined
    /// by the <see cref="EventExtensionPoint{TEventArgs}"/> extension point.  
    /// </remarks>
    public static class EventManager
    {
        private interface IEventRecord
        {
            void Fire(object sender, ImageServerEventArgs e);
        }

        private class EventRecord<TEventArgs> : IEventRecord
            where TEventArgs : ImageServerEventArgs
        {
            private bool _warningLogged = false;
            private readonly EventExtensionPoint<TEventArgs> _extensionPoint;
            private readonly List<IEventHandler<TEventArgs>> _extensions;

            public Type EventArgsType { get; private set; }

            public EventRecord()
            {
                EventArgsType = typeof (TEventArgs);
                _extensionPoint = new EventExtensionPoint<TEventArgs>();

                var list = _extensionPoint.CreateExtensions();
                _extensions = new List<IEventHandler<TEventArgs>>(list.Length);

                foreach (IEventHandler<TEventArgs> extension in list)
                {
                    _extensions.Add(extension);
                }
            }

            public void Fire(object sender, ImageServerEventArgs e)
            {
                foreach (var eventHandler in _extensions)
                {
                    try
                    {
                        eventHandler.EventHandler(sender, (TEventArgs)e);
                    }
                    catch (Exception x)
                    {
                        if (!_warningLogged)
                        {
                            _warningLogged = true;
                            Platform.Log(LogLevel.Error, x, "Unexpected error firing {0} event to class {1}", EventArgsType.ToString(),eventHandler.GetType().ToString());
                        }
                    }
                }
            }            
        }

        private static Dictionary<Type, IEventRecord> _events = new Dictionary<Type, IEventRecord>(); 

        static EventManager()
        {
            // When a new EventType is added, it must be introduced here.
            var record = new EventRecord<NewSopEventArgs>();
            _events.Add(record.EventArgsType, record);

        }

        public static void FireEvent(object sender, ImageServerEventArgs e)
        {
            try
            {
                IEventRecord record;
                if (_events.TryGetValue(e.GetType(), out record))
                {
                    record.Fire(sender, e);
                }
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x, "Unexpected error firing {0}", e.GetType().ToString());
            }
        }
    }
}
