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

using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;

namespace ClearCanvas.Enterprise.Core.Msmq
{
	/// <summary>
	/// Encapsulates an OS handle to a native MSMQ queue.
	/// </summary>
	internal class QueueHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		sealed class InvalidQueueHandle : QueueHandle
		{
			protected override bool ReleaseHandle()
			{
				return true;
			}
		}

		public static readonly QueueHandle Invalid = new InvalidQueueHandle();

		public QueueHandle()
			: base(true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected override bool ReleaseHandle()
		{
			NativeMethods.MQCloseQueue(this.handle);
			return true;
		}
	}
}
