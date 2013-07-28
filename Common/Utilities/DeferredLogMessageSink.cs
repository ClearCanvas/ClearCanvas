using System;
using System.Collections.Generic;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Implementation of <see cref="ILogMessageSink"/> that defers writing messages to the log file
	/// until a <see cref="Flush"/> method is called.
	/// </summary>
	public class DeferredLogMessageSink : ILogMessageSink
	{
		private readonly Queue<LogMessage> _messages = new Queue<LogMessage>();

		public void Flush()
		{
			while(_messages.Count > 0)
			{
				var message = _messages.Dequeue();
				Platform.Log(message.Level, message.Message, message.Arguments);
			}
		}

		public void Write(LogMessage message)
		{
			if(message == null)
				throw new ArgumentNullException("message");

			_messages.Enqueue(message);
		}
	}
}
