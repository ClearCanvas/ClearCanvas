#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.IO;

namespace ClearCanvas.Dicom.IO
{
	internal static class StreamMethods
	{
		/// <summary>
		/// Sets the position within the current stream, but if the stream is unseekable and it is a forward seek, it will just read bytes until it gets to the desired location.
		/// </summary>
		public static long SeekEx(this Stream stream, long offset, SeekOrigin origin)
		{
			if (!stream.CanSeek)
			{
				var bytesToSkip = 0L;

				switch (origin)
				{
					case SeekOrigin.Current:
						bytesToSkip = offset;
						break;
					case SeekOrigin.Begin:
						bytesToSkip = offset - stream.Position;
						break;
					case SeekOrigin.End:
						bytesToSkip = stream.Length + offset - stream.Position;
						break;
				}

				// as long it is a forward seek, we can accomplish it on an unseekable stream by simply reading
				if (bytesToSkip > 0)
				{
					var buffer = new byte[(int) Math.Min(4096, bytesToSkip)]; // max buffer size is 4096
					var bytesSkipped = 0;
					while (bytesSkipped < bytesToSkip)
					{
						// read either the full buffer, or the number of bytes left, whichever is less
						// then add the actual bytes read to skipped bytes
						var bytesRead = stream.Read(buffer, 0, (int) Math.Min(buffer.Length, bytesToSkip - bytesSkipped));
						if (bytesRead == 0) break; // if we reached EOF, just stop and return current position (behaviour for streams that do not support seeking beyond end)
						bytesSkipped += bytesRead;
					}
					return stream.Position;
				}
			}
			return stream.Seek(offset, origin);
		}
	}
}