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
using System.Reflection;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common
{
    // TODO (CR Jun 2012): I think this should go back in Shreds or at least in a "ServiceUtilities" subnamespace or something.
    // Its presence in Common implies it's usable from anywhere, which technically isn't true.
    
    internal static class SubscriptionManager<T>
		where T : class 
	{
		private static readonly object _syncLock;
		private static readonly Dictionary<string, List<T>> _subscribers;

		static SubscriptionManager()
		{
			_syncLock = new object();

			Type operationContractType = typeof (OperationContractAttribute);
			Type callbackType = typeof (T);
			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

			_subscribers = new Dictionary<string, List<T>>();
			foreach (MethodInfo info in callbackType.GetMethods(bindingFlags))
			{
				if (info.IsDefined(operationContractType, false))
					_subscribers[info.Name] = new List<T>();
			}
		}

		public static IEnumerable<string> GetMethods()
		{
			foreach (string eventName in _subscribers.Keys)
				yield return eventName;
		}

		public static void Subscribe(string eventOperation)
		{
			Subscribe(OperationContext.Current.GetCallbackChannel<T>(), eventOperation);
		}

		public static void Subscribe(T callback, string eventOperation)
		{
			if (String.IsNullOrEmpty(eventOperation))
			{
				lock (_syncLock)
				{
					foreach (string subscribeMethod in _subscribers.Keys)
					{
						if (!_subscribers[subscribeMethod].Contains(callback))
							_subscribers[subscribeMethod].Add(callback);
					}
				}
			}
			else if (_subscribers.ContainsKey(eventOperation))
			{
				lock (_syncLock)
				{
					if (!_subscribers[eventOperation].Contains(callback))
						_subscribers[eventOperation].Add(callback);
				}
			}
			else
			{
				Debug.Assert(false);
			}
		}

		public static void Unsubscribe(string eventOperation)
		{
			Unsubscribe(OperationContext.Current.GetCallbackChannel<T>(), eventOperation);
		}

		public static void Unsubscribe(T callback, string eventOperation)
		{
			if (String.IsNullOrEmpty(eventOperation))
			{
				lock (_syncLock)
				{
					foreach (string method in _subscribers.Keys)
						_subscribers[method].Remove(callback);
				}
			}
			else if (_subscribers.ContainsKey(eventOperation))
			{
				lock (_syncLock)
				{
					_subscribers[eventOperation].Remove(callback);
				}
			}
		}

		public static T[] GetSubscribers(string eventOperation)
		{
			Debug.Assert(eventOperation != null && _subscribers.ContainsKey(eventOperation));

			lock (_syncLock)
			{
				return _subscribers[eventOperation].ToArray();
			}
		}
	}
}
