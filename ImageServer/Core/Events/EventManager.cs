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
						eventHandler.EventHandler(sender, (TEventArgs) e);
					}
					catch (Exception x)
					{
						if (!_warningLogged)
						{
							_warningLogged = true;
							Platform.Log(LogLevel.Error, x, "Unexpected error firing {0} event to class {1}", EventArgsType.ToString(),
							             eventHandler.GetType().ToString());
						}
					}
				}
			}
		}

		private static readonly Dictionary<Type, IEventRecord> Events = new Dictionary<Type, IEventRecord>();

		static EventManager()
		{
			// Generically create EventRecord instances for each of the types marked with [ImageServerEvent]
			var types = ImageServerEventTypeProvider.GetTypeArray();
			foreach (var t in types)
			{
				try
				{
					Type[] typeArgs = {t};
					Type ert = typeof (EventRecord<>);
					Type genericType = ert.MakeGenericType(typeArgs);

					var o = Activator.CreateInstance(genericType);
					Events.Add(t, (IEventRecord) o);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e,
					             "Unable to create event handler for: {0}, check if event has the [ImageServerEvent] attribute set for it and inherits from ImageServerEventArgs.",
					             t.ToString());
				}

			}
		}

		public static void FireEvent(object sender, ImageServerEventArgs e)
		{
			try
			{
				IEventRecord record;
				if (Events.TryGetValue(e.GetType(), out record))
				{
					record.Fire(sender, e);
				}
				else
				{
					Platform.Log(LogLevel.Error,
					             "Invalid Event Type: {0}, check if event has the [ImageServerEvent] attribute set for it.",
					             e.GetType().ToString());
				}
			}
			catch (Exception x)
			{
				Platform.Log(LogLevel.Error, x, "Unexpected error firing {0}", e.GetType().ToString());
			}
		}
	}
}
