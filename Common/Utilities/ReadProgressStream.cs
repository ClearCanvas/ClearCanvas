using System;
using System.IO;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Notifies of progress reading a stream.
	/// </summary>
	/// <remarks>
	/// The <see cref="Stream.Length"/>, <see cref="Stream.Position"/> properties of the underlying stream must be readable.
	/// </remarks>
	public class ReadProgressStream : Stream
	{
		private Stream _realStream;

		public ReadProgressStream(Stream realStream)
		{
			Platform.CheckTrue(realStream.CanRead, "realStream readable");
			_realStream = realStream;
		}

		public double ProgressPercent
		{
			get { return Math.Min(_realStream.Position/(double) _realStream.Length*100, 100); }
		}

		public event EventHandler ProgressChanged;

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
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
				throw new NotSupportedException("Cannot set position of ReadProgressStream.");
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var returnValue = _realStream.Read(buffer, offset, count);
			OnProgressChanged();
			return returnValue;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Cannot seek ReadProgressStream.");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("Cannot set length of ReadProgressStream.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("Cannot write to ReadProgressStream.");
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

		protected virtual void OnProgressChanged()
		{
			EventHandler handler = ProgressChanged;
			if (handler != null) handler(this, EventArgs.Empty);
		}
	}
}
