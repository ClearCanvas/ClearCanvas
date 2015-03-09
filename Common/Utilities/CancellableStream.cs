using System;
using System.IO;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Allows regular reading to and writing from a <see cref="Stream"/> to be canceled via a <see cref="CancellationToken"/>.
	/// </summary>
	public class CancellableStream : Stream
	{
		private Stream _realStream;
		private readonly CancellationToken _cancellationToken;

		public CancellableStream(Stream realStream, CancellationToken cancellationToken)
		{
			_realStream = realStream;
			_cancellationToken = cancellationToken;
		}

		public override bool CanRead
		{
			get { return _realStream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return _realStream.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return _realStream.CanWrite; }
		}

		public override void Flush()
		{
			_cancellationToken.ThrowIfCancellationRequested();
			_realStream.Flush();
		}

		public override long Length
		{
			get { return _realStream.Length; }
		}

		public override long Position
		{
			get { return _realStream.Position; }
			set { _realStream.Position = value; }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			_cancellationToken.ThrowIfCancellationRequested();
			return _realStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			_cancellationToken.ThrowIfCancellationRequested();
			return _realStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			_cancellationToken.ThrowIfCancellationRequested();
			_realStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_cancellationToken.ThrowIfCancellationRequested();
			_realStream.Write(buffer, offset, count);
		}

		public override void Close()
		{
			if (_realStream != null)
				_realStream.Close();

			base.Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing) return;

			if (_realStream == null) return;
			_realStream.Dispose();
			_realStream = null;
		}
	}
}
