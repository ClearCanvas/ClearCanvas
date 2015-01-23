using System;
using System.IO;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Notifies of progress writing to a stream.
	/// </summary>
	/// <remarks>
	/// The <see cref="Stream.Length"/>, <see cref="Stream.Position"/> and <see cref="Stream."/>
	/// </remarks>
	public class WriteProgressStream : Stream
	{
		private readonly Stream _realStream;
		private long _bytesWritten;
		private long _bytesToWrite;

		public WriteProgressStream(Stream realStream, long bytesToWrite)
		{
			Platform.CheckTrue(realStream.CanWrite, "realStream writeable");
			_bytesToWrite = bytesToWrite;
			_realStream = realStream;
		}

		public double ProgressPercent
		{
			get { return Math.Min(_bytesWritten/(double) _bytesToWrite*100, 100); }
		}

		public event EventHandler ProgressChanged;

		public override bool CanRead
		{
			get { return false; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void Flush()
		{
			_realStream.Flush();
		}

		public override long Length
		{
			get { return _realStream.Length; }
		}

		public override long Position
		{
			get { return _realStream.Position; }
			set
			{
				throw new NotSupportedException("Cannot set position of WriteProgressStream.");
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("Cannot read WriteProgressStream.");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Cannot seek WriteProgressStream.");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("Cannot set length of WriteProgressStream.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_realStream.Write(buffer, offset, count);
			_bytesWritten += count;
			if (_bytesWritten > _bytesToWrite)
				_bytesToWrite = _bytesWritten;
			OnProgressChanged();
		}

		protected virtual void OnProgressChanged()
		{
			EventHandler handler = ProgressChanged;
			if (handler != null) handler(this, EventArgs.Empty);
		}
	}
}
