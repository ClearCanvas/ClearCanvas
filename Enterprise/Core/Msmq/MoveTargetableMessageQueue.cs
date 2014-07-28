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
	/// <summary>
	/// Extends <see cref="MessageQueue"/> to implement the <see cref="IMoveTargetableQueue"/> interface.
	/// </summary>
	/// <remarks>
	/// Application code should not create instances of this class. Instead, use <see cref="MsmqFactory"/> to
	/// obtain message queue objects.
	/// </remarks>
	internal class MoveTargetableMessageQueue : MessageQueue, IMoveTargetableQueue
	{
		private QueueHandle _moveHandle = QueueHandle.Invalid;

		public MoveTargetableMessageQueue(string path, bool sharedModeDenyReceive, bool enableCache)
			: base(path, sharedModeDenyReceive, enableCache)
		{
		}

		IntPtr IMoveTargetableQueue.MoveHandle
		{
			get
			{
				if (_moveHandle == QueueHandle.Invalid)
				{
					var handle = QueueHandle.Invalid;
					var error = NativeMethods.MQOpenQueue(this.FormatName, NativeMethods.MQ_MOVE_ACCESS, NativeMethods.MQ_DENY_NONE, ref handle);
					if (error != 0)
						throw new InvalidOperationException("Failed to open queue: " + this.FormatName, new Win32Exception(error));
					_moveHandle = handle;
				}
				return _moveHandle.DangerousGetHandle();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_moveHandle != QueueHandle.Invalid)
				{
					_moveHandle.Dispose();
					_moveHandle = QueueHandle.Invalid;
				}
			}
			base.Dispose(disposing);
		}
	}
}
