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
using System.ComponentModel;
using System.Messaging;

namespace ClearCanvas.Enterprise.Core.Msmq
{
	public static class MessageQueueExtensions
	{
		/// <summary>
		/// Moves the specified message from this queue to the specified destination queue.
		/// </summary>
		/// <param name="source">The queue that currently contains the message.</param>
		/// <param name="dest">The queue into which the message should be placed.</param>
		/// <param name="message">The message to move.</param>
		/// <remarks>
		/// The message to be moved must currently be in the source queue. The source and destination
		/// queues must have the same base queue, which implies that at least one of the two must be
		/// a subqueue.
		/// </remarks>
		public static void Move(this MessageQueue source, MessageQueue dest, Message message)
		{
			if(dest is IMoveTargetableQueue)
			{
				var targetableDest = (IMoveTargetableQueue)dest;
				MoveMessage(source.ReadHandle, targetableDest.MoveHandle, message);
			}
			else
				throw new ArgumentException("The specified destination queue does not support this operation.");
		}

		/// <summary>
		/// Moves all messages from this queue to the specified destination queue.
		/// </summary>
		/// <param name="source">The queue from which to move messages.</param>
		/// <param name="dest">The queue into which the messages should be placed.</param>
		/// <remarks>
		/// The source and destination queues must have the same base queue, which implies that at least one of the two must be
		/// a subqueue.
		/// </remarks>
		public static void MoveAll(this MessageQueue source, MessageQueue dest)
		{
			var sourceEnumerator = source.GetMessageEnumerator2();

			while (sourceEnumerator.MoveNext())
			{
				var message = sourceEnumerator.Current;
				if (message == null)
					throw new Exception("wtf?");
				source.Move(dest, message);
			}
		}

		private static void MoveMessage(IntPtr source, IntPtr dest, Message message)
		{
			var error = NativeMethods.MQMoveMessage(source, dest, message.LookupId, IntPtr.Zero);
			if (error != 0)
			{
				if (error == (int)MessageQueueErrorCode.MessageNotFound)
				{
					throw new Win32Exception(error, string.Format("Message with LookupId {0} not found", message.LookupId));
				}
				if (error == (int)MessageQueueErrorCode.AccessDenied)
				{
					throw new Win32Exception(error, "Failed to move message - access denied");
				}
				throw new Win32Exception(error, "Failed to move message");
			}
		}

	}
}
