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

			if(message.Exception != null)
			{
				Platform.Log(message.Level, message.Exception, message.Message, message.Arguments);
			}
			else
			{
				Platform.Log(message.Level, message.Message, message.Arguments);
			}
		}

		public void Flush()
		{
			// nothing to do
		}

		public void Purge()
		{
			// nothing to do
		}
	}

	/// <summary>
	/// Implementation of <see cref="ILogMessageSink"/> that defers writing messages
	/// until the <see cref="Flush"/> method is called.
	/// </summary>
	public class DeferredLogMessageSink : ILogMessageSink
	{
		private readonly Queue<LogMessage> _messages = new Queue<LogMessage>();
		private readonly ILogMessageSink _inner;

		public DeferredLogMessageSink()
			:this(new DefaultLogMessageSink())
		{
		}

		public DeferredLogMessageSink(ILogMessageSink inner)
		{
			_inner = inner;
		}

		public void Flush()
		{
			while (_messages.Count > 0)
			{
				var message = _messages.Dequeue();
				_inner.Write(message);
			}
			_inner.Flush();
		}

		public void Purge()
		{
			_messages.Clear();
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

		public void Purge()
		{
			_inner.Purge();
		}
	}
}
