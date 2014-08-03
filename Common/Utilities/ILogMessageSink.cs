using System;

namespace ClearCanvas.Common.Utilities
{
	public class LogMessage
	{
		public LogMessage()
		{
			
		}

		public LogMessage(LogLevel level, string message, params object[] arguments)
		{
			Level = level;
			Message = message;
			Arguments = arguments;
		}

		public LogLevel Level { get; set; }
		public string Message { get; set; }
		public object[] Arguments { get; set; }
		public Exception Exception { get; set; }
	}

	/// <summary>
	/// Defines an interface to an object that acts as a log message sink.
	/// </summary>
	public interface ILogMessageSink
	{
		/// <summary>
		/// Writes the specified message to the sink.
		/// </summary>
		/// <param name="message"></param>
		void Write(LogMessage message);

		/// <summary>
		/// Ensures that all messages written to the sink have been committed to the underlying storage.
		/// </summary>
		void Flush();

		/// <summary>
		/// Purges any messages that have not been flushed.
		/// </summary>
		void Purge();
	}

	public static class LogMessageSinkExtensions
	{
		public static void Log(this ILogMessageSink sink, LogLevel level, string message, params object[] args)
		{
			sink.Write(new LogMessage(level, message, args));
		}

		public static void Log(this ILogMessageSink sink, LogLevel level, Exception ex, string message, params object[] args)
		{
			sink.Write(new LogMessage(level, message, args) { Exception = ex });
		}
	}
}
