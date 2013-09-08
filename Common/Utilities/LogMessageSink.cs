using System;
using System.Collections.Generic;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Default Implementation of <see cref="ILogMessageSink"/>.
	/// </summary>
	public class DefaultLogMessageSink : ILogMessageSink
	{
		public void Write(LogMessage message)
		{
			if(message == null)
				throw new ArgumentNullException("message");

			Platform.Log(message.Level, message.Message, message.Arguments);
		}

		public void Flush()
		{
			// nothing to do
		}
	}

	/// <summary>
	/// Implementation of <see cref="ILogMessageSink"/> that defers writing messages to the log file
	/// until a <see cref="Flush"/> method is called.
	/// </summary>
	public class DeferredLogMessageSink : ILogMessageSink
	{
		private readonly Queue<LogMessage> _messages = new Queue<LogMessage>();

		public void Flush()
		{
			while (_messages.Count > 0)
			{
				var message = _messages.Dequeue();
				Platform.Log(message.Level, message.Message, message.Arguments);
			}
		}

		public void Write(LogMessage message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			_messages.Enqueue(message);
		}
	}

	/// <summary>
	/// Implementation of <see cref="ILogMessageSink"/> that allows for decorating a log message
	/// prior to writing it.
	/// </summary>
	public class DecoratingLogMessageSink : ILogMessageSink
	{
		private readonly ILogMessageSink _inner;
		private readonly Func<LogMessage, LogMessage> _decorator;

		public DecoratingLogMessageSink(ILogMessageSink inner, Func<LogMessage, LogMessage> decorator)
		{
			if (inner == null)
				throw new ArgumentNullException("inner");
			if (decorator == null)
				throw new ArgumentNullException("decorator");

			_inner = inner;
			_decorator = decorator;
		}

		public void Write(LogMessage message)
		{
			_inner.Write(_decorator(message));
		}

		public void Flush()
		{
			_inner.Flush();
		}
	}
}
