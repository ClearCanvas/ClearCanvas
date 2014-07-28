using System;
using System.ComponentModel;
using System.Messaging;

namespace ClearCanvas.Enterprise.Core.Msmq
{
	/// <summary>
	/// Extends <see cref="MessageQueue"/> to implement the <see cref="IMoveTargetableQueue"/> interface.
	/// </summary>
	internal class MoveTargetableMessageQueue : MessageQueue, IMoveTargetableQueue
	{
		//todo: clean this up - use SafeHandle
		class HandleManager : IDisposable
		{
			private readonly string _formatName;
			private IntPtr _queueHandle = IntPtr.Zero;

			public HandleManager(string formatName)
			{
				_formatName = formatName;
			}

			public string FormatName
			{
				get { return _formatName; }
			}

			public IntPtr Handle
			{
				get
				{
					if (_queueHandle == IntPtr.Zero)
					{
						Open();
					}
					return _queueHandle;
				}
			}

			public void Dispose()
			{
				try
				{
					Close();
				}
				catch (Exception)
				{
				}
			}

			private void Open()
			{
				var queueHandle = IntPtr.Zero;
				var error = NativeMethods.MQOpenQueue(_formatName, NativeMethods.MQ_MOVE_ACCESS, NativeMethods.MQ_DENY_NONE, ref queueHandle);
				if (error != 0)
					throw new InvalidOperationException("Failed to open queue: " + _formatName, new Win32Exception(error));
				_queueHandle = queueHandle;
			}

			private void Close()
			{
				if (_queueHandle == IntPtr.Zero)
					return;

				var error = NativeMethods.MQCloseQueue(_queueHandle);
				if (error != 0)
					throw new InvalidOperationException("Failed to close queue: " + _formatName, new Win32Exception(error));

				_queueHandle = IntPtr.Zero;
			}
		}

		private HandleManager _handleManager;

		public MoveTargetableMessageQueue(string path, bool sharedModeDenyReceive, bool enableCache)
			: base(path, sharedModeDenyReceive, enableCache)
		{
		}

		public IntPtr MoveHandle
		{
			get
			{
				if (_handleManager == null)
				{
					_handleManager = new HandleManager(this.FormatName);
				}
				return _handleManager.Handle;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(_handleManager != null)
			{
				_handleManager.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
