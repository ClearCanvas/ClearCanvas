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

//2006 IDesign Inc. 
//Questions? Comments? go to 
//http://www.idesign.net

/// I have since rewritten the classes from scratch, but the design idea
/// is based on Juval Lowy's publish-subscribe framework.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
    // TODO (CR Jun 2012): I think this should go back in Shreds or at least in a "ServiceUtilities" subnamespace or something.
    // Its presence in Common implies it's usable from anywhere, which technically isn't true.

	internal static class PublishManager<T>
		where T : class
	{
		private static readonly object _syncLock;
		private static readonly Dictionary<string, Dictionary<T, Queue<object[]>>> _pendingPublishItems;

		static PublishManager()
		{
			_syncLock = new object();
			_pendingPublishItems = new Dictionary<string, Dictionary<T, Queue<object[]>>>();
			foreach (string eventName in SubscriptionManager<T>.GetMethods())
				_pendingPublishItems.Add(eventName, new Dictionary<T, Queue<object[]>>());
		}

		public static void Publish(string eventName, params object[] args)
		{
			Debug.Assert(eventName != null && _pendingPublishItems.ContainsKey(eventName));

			Dictionary<T, Queue<object[]>> eventDictionary = _pendingPublishItems[eventName];
			T[] subscribers = SubscriptionManager<T>.GetSubscribers(eventName);

			lock (_syncLock)
			{
				foreach (T subscriber in subscribers)
				{
					//for each event, we need to ensure that only a single thread is publishing data
					//per subscriber at a time.  Otherwise, occasionally, you can end up with events 
					// reaching the subscribers in the wrong order.
					if (eventDictionary.ContainsKey(subscriber))
					{
						eventDictionary[subscriber].Enqueue(args);
					}
					else
					{
						Queue<object[]> queue = new Queue<object[]>();
						queue.Enqueue(args);
						eventDictionary.Add(subscriber, queue);
						//The Publish delegate called by the thread pool will keep re-adding itself to the thread pool 
						//with the same KeyValuePair until all data for the KeyValuePair (event-subscriber) has been published.
						ThreadPool.QueueUserWorkItem(Publish, new KeyValuePair<string, T>(eventName, subscriber));
					}
				}
			}
		}

		private static void Publish(object obj)
		{
			KeyValuePair<string, T> kvp = (KeyValuePair<string, T>)obj;
			object[] args;
			bool anyLeft = true;

			Dictionary<T, Queue<object[]>> eventDictionary = _pendingPublishItems[kvp.Key];

			lock (_syncLock)
			{
				args = eventDictionary[kvp.Value].Dequeue();

				if (eventDictionary[kvp.Value].Count == 0)
				{
					eventDictionary.Remove(kvp.Value);
					anyLeft = false;
				}
			}

			try
			{
				typeof(T).GetMethod(kvp.Key).Invoke(kvp.Value, args);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			if (anyLeft)
				ThreadPool.QueueUserWorkItem(Publish, kvp);
		}
	}
}
