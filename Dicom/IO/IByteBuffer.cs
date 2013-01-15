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

using System.IO;
using System.Text;

namespace ClearCanvas.Dicom.IO
{
	/// <summary>
	/// Interface for accessing a byte buffer.
	/// </summary>
	internal interface IByteBuffer
	{
		/// <summary>
		/// Gets the byte buffer as a <see cref="Stream"/>.
		/// </summary>
		Stream Stream { get; }

		/// <summary>
		/// Gets a <see cref="BinaryReader"/> view on the buffer.
		/// </summary>
		BinaryReader Reader { get; }

		/// <summary>
		/// Gets a <see cref="BinaryWriter"/> view on the buffer.
		/// </summary>
		BinaryWriter Writer { get; }

		/// <summary>
		/// Gets or sets the endianess of the buffer.
		/// </summary>
		Endian Endian { get; set; }

		/// <summary>
		/// Gets or sets the character encoding of the buffer.
		/// </summary>
		Encoding Encoding { get; set; }

		/// <summary>
		/// Gets or sets the specific character set used by the buffer.
		/// </summary>
		string SpecificCharacterSet { get; set; }

		/// <summary>
		/// Gets the length of the buffer.
		/// </summary>
		int Length { get; }

		/// <summary>
		/// Clears the contents of the buffer.
		/// </summary>
		void Clear();

		/// <summary>
		/// Truncates the buffer by removing the first <paramref name="count"/> bytes.
		/// </summary>
		/// <param name="count">Number of bytes to be truncated.</param>
		void Chop(int count);

		/// <summary>
		/// Appends bytes to the buffer.
		/// </summary>
		/// <param name="buffer">Bytes to be appended.</param>
		/// <param name="offset">Index in <paramref name="buffer"/> from which to start appending.</param>
		/// <param name="count">Number of bytes in <paramref name="buffer"/> to append.</param>
		void Append(byte[] buffer, int offset, int count);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		int CopyFrom(Stream s, int count);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bw"></param>
		void CopyTo(BinaryWriter bw);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		void CopyTo(Stream s, int offset, int count);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		void CopyTo(byte[] buffer, int offset, int count);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="srcOffset"></param>
		/// <param name="dstOffset"></param>
		/// <param name="count"></param>
		void CopyTo(byte[] buffer, int srcOffset, int dstOffset, int count);

		byte[] GetChunk(int offset, int count);
		void FromBytes(byte[] bytes);
		byte[] ToBytes();
		byte[] ToBytes(int offset, int count);
		ushort[] ToUInt16s();
		short[] ToInt16s();
		uint[] ToUInt32s();
		int[] ToInt32s();
		float[] ToFloats();
		double[] ToDoubles();
		string GetString();
		void SetString(string stringValue);
		void SetString(string stringValue, byte paddingByte);
		void Swap(int bytesToSwap);
		void Swap2();
		void Swap4();
		void Swap8();

		#region Unit Test Support

#if UNIT_TESTS

		byte[] GetTestData();

		Stream GetTestStream();

#endif

		#endregion
	}
}