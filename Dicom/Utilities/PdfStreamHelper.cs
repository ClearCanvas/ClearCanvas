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

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Utilities
{
    internal static class PdfStreamHelper
    {
        #region Constants

        // Find %%EOF marker
        static readonly byte[] EOFMarkers = new byte[] { 0x25, 0x25, 0x45, 0x4F, 0x46 };

        #endregion

        /// <summary>
        /// Removes extra byte that may have been added to the PDF stream for DICOM compliance. See remarks for more details.
        /// </summary>
        /// <remarks>
        /// 
        /// 
        /// 
        /// Extra-Byte Stripping:
        /// 
        /// Per PDF 1.4:
        /// The trailer of an FDF ﬁle enables an application reading the ﬁle to ﬁnd signiﬁcant
        /// objects quickly within the body of the ﬁle. The last line of the ﬁle contains only
        /// the end-of-ﬁle marker, %%EOF
        /// 
        /// Also, PDF 1.4 defines each line as being "terminated by an end-of-line (EOL) marker, 
        /// which may be a carriage return (character code 13), a line feed (character code 10), or both."
        ///
        /// That means, a valid PDF will end with {0D 0A 25 25 45 4F 46 0D 0A} or {0D 25 25 45 4F 46 0D} or or {0A 25 25 45 4F 46 0A}.
        /// 
        /// Note: PDF exported using MS Word does not contain EOL marker(s).
        /// 
        /// To make the file DICOM-compliant, the DICOM encoder may have added an extra NULL byte at the end to make the length even.
        /// Although not necessary (because there's no additional "line" inserted and most populate PDF readers can handle extra bytes after the last EOF),
		/// when extracted, the extra byte should still be removed because it was not part of the original PDF content. 
        /// 
        /// </remarks>
        /// <param name="rawPDFBuffer">The buffer containing the entire PDF document</param>
        /// <returns>A buffer containing the entire PDF document with extra DICOM padding byte removed if it is present</returns>
        internal static byte[] StripDicomPaddingBytes(byte[] rawPDFBuffer)
        {
            if (rawPDFBuffer == null)
                return null;


            // Note: The %%EOF marker may appear more than once in the file. PDF standard allows incremental changes appended to file.
            var eofIndex = rawPDFBuffer.FindLastIndex(EOFMarkers);
            if (eofIndex < 0)
            {
                Platform.Log(LogLevel.Warn, "The PDF stream does not contain a PDF trailer");

                // Maybe it's just one of the chucks so don't do anything
                return rawPDFBuffer;
            }

            // There's %%EOF marker, now we need to check if there's an extra padding byte.
            // No Padding: {25 25 45 4F 46 0D} or {25 25 45 4F 46 0A} or {25 25 45 4F 46 0D 0A} or {25 25 45 4F 46 0A 0D}
            // Padded:      {25 25 45 4F 46 0D 00} or {25 25 45 4F 46 0A 00} or {25 25 45 4F 46 0D 0A 00} or {25 25 45 4F 46 0A 0D 00}

            int bytesRemain = rawPDFBuffer.Length - eofIndex - EOFMarkers.Length;
            if (bytesRemain <= 0)
            {
                // Note: Should not be <0. Otherwise, FindLastIndex() is wrong. Anyway, there's no EOL marker.
                // This happens when the pdf is created using MS Word. In any case, just return the original buffer.
                return rawPDFBuffer;
            }

            if (bytesRemain == 1)
            {
                // Should be the single-byte EOL marker or padding byte (MSWord case). In latter case, just remove the padding byte.
                if (rawPDFBuffer.ContainSequenceAt(eofIndex + EOFMarkers.Length, new byte[] { 0x0 }))
                {
                    return RemoveTrailingBytes(rawPDFBuffer, 1);
                }

                return rawPDFBuffer;
            }

            if (bytesRemain == 2)
            {
                // if the stream either ends with 1-byte EOL marker followed by a NULL...
                if (rawPDFBuffer.ContainSequenceAt(eofIndex + EOFMarkers.Length, new byte[] { 0x0A, 0x0 }) ||
                    rawPDFBuffer.ContainSequenceAt(eofIndex + EOFMarkers.Length, new byte[] { 0x0D, 0x0 })
                    )
                {
                    Platform.Log(LogLevel.Warn, "Detect a padding byte in the PDF stream");
                    return RemoveTrailingBytes(rawPDFBuffer, 1);
                }

            }
            else if (bytesRemain == 3)
            {

                // if the stream either ends with 2-byte EOL marker followed by a NULL ...
                if (rawPDFBuffer.ContainSequenceAt(eofIndex + EOFMarkers.Length, new byte[] { 0x0A, 0x0D, 0x0 }) ||
                    rawPDFBuffer.ContainSequenceAt(eofIndex + EOFMarkers.Length, new byte[] { 0x0D, 0x0A, 0x0 }))
                {
                    Platform.Log(LogLevel.Warn, "Detect a padding byte in the PDF stream");
                    return RemoveTrailingBytes(rawPDFBuffer, 1);
                }
            }

            return rawPDFBuffer;

        }

        #region Private Methods

        /// <summary>
        /// Return the index in the buffer where the sequence of bytes last occurs 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        internal static int FindLastIndex(this byte[] buffer, byte[] sequence)
        {
            if (sequence == null)
                return -1;

            int i = buffer.Length - sequence.Length;
            while (i >= 0)
            {
                if (buffer.ContainSequenceAt(i, sequence))
                {
                    return i;
                }
                i--;
            }

            return -1;
        }

        /// <summary>
        /// Checks if the sequence of byte array appears in the source at the specified index
        /// </summary>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private static bool ContainSequenceAt(this byte[] source, int index, byte[] sequence)
        {
            Platform.CheckNonNegative(index, "index");
            Platform.CheckForNullReference(sequence, "sequence");

            for (int i = 0; i < sequence.Length; i++)
            {
                if (index + i >= source.Length) // source is shorter than the sequence
                    return false;

                if (source[index + i] != sequence[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Remove a specified number of bytes at the end of the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static byte[] RemoveTrailingBytes(this byte[] source, int count)
        {
            var newBuffer = new byte[source.Length - count];
            Buffer.BlockCopy(source, 0, newBuffer, 0, newBuffer.Length);
            return newBuffer;
        }

        #endregion

    }
}

