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
using System.Messaging;

namespace ClearCanvas.Enterprise.Core.Msmq
{
	/// <summary>
	/// Factory class for instantiating <see cref="MessageQueue"/> instances.
	/// </summary>
	public class MsmqFactory
	{
		private static readonly MsmqFactory _default = new MsmqFactory();

		/// <summary>
		/// Gets the default factory instance.
		/// </summary>
		public static MsmqFactory Default
		{
			get { return _default; }
		}

		/// <summary>
		/// Gets a value indicating whether a queue with the specified name exists.
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		public bool QueueExists(MsmqName queueName)
		{
			if (queueName.IsSubQueue)
				throw new InvalidOperationException("A subqueue cannot be checked for existence.");

			return MessageQueue.Exists(queueName.Path);
		}

		/// <summary>
		/// Creates a queue with the specified name, and returns a <see cref="MessageQueue"/> instance for it.
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		/// <remarks>
		/// The specified queue must not already exist. Also, the supplied <see cref="MsmqName"/> must not refer to
		/// a subqueue, because subqueues cannot be explicitly created.
		/// </remarks>
		public MessageQueue CreateQueue(MsmqName queueName)
		{
			if (queueName.IsSubQueue)
				throw new InvalidOperationException("A subqueue cannot be explicitly created.");

			MessageQueue.Create(queueName.Path);
			return GetQueue(queueName);
		}

		/// <summary>
		/// Returns a new <see cref="MessageQueue"/> instance for the queue with the specified name.
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		/// <remarks>
		/// The specified queue must already exist.
		/// </remarks>
		public MessageQueue GetQueue(MsmqName queueName)
		{
			var path = string.Format("FormatName:{0}", queueName.FormatName);
			return new MoveTargetableMessageQueue(path, false, true);
		}
	}
}
