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
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace ClearCanvas.Enterprise.Core.Msmq
{
	internal static class NativeMethods
	{
		public const int MQ_MOVE_ACCESS = 4;
		public const int MQ_DENY_NONE = 0;

		[DllImport("mqrt.dll", CharSet = CharSet.Unicode)]
		public static extern int MQOpenQueue(string formatName, int access, int shareMode, ref QueueHandle hQueue);

		[DllImport("mqrt.dll")]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static extern int MQCloseQueue(IntPtr queue);

		[DllImport("mqrt.dll")]
		public static extern int MQMoveMessage(IntPtr sourceQueue, IntPtr targetQueue, long lookupID, IntPtr transaction);

		[DllImport("mqrt.dll")]
		internal static extern int MQMgmtGetInfo([MarshalAs(UnmanagedType.BStr)]string computerName, [MarshalAs(UnmanagedType.BStr)]string objectName, ref MQMGMTPROPS mgmtProps);

		public const byte VT_NULL = 1;
		public const byte VT_UI4 = 19;
		public const int PROPID_MGMT_QUEUE_MESSAGE_COUNT = 7;

		//size must be 16 in x86 and 28 in x64
		[StructLayout(LayoutKind.Sequential)]
		internal struct MQMGMTPROPS
		{
			public uint cProp;
			public IntPtr aPropID;
			public IntPtr aPropVar;
			public IntPtr status;
		}
	}
}
