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

namespace ClearCanvas.Dicom.IO
{
    internal static class ByteConverter
    {
        /// <summary>
        /// Determines if this machine has the same byte
        /// order as endian.
        /// </summary>
        /// <param name="endian">endianness</param>
        /// <returns>true - byte swapping is required</returns>
        public static bool NeedToSwapBytes(Endian endian)
        {
            if (BitConverter.IsLittleEndian)
            {
                if (Endian.Little == endian)
                    return false;
                return true;
            }
            else
            {
                if (Endian.Big == endian)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Converts an array of ushorts to an array of bytes.
        /// </summary>
        /// <param name="words">Array of ushorts</param>
        /// <returns>Newly allocated byte array</returns>
        public static byte[] ToByteArray(ushort[] words)
        {
            int count = words.Length;
            byte[] bytes = new byte[words.Length * 2];
            for (int i = 0; i < count; i++)
            {
                // slow as shit, should be able to use Buffer.BlockCopy for this
                Array.Copy(BitConverter.GetBytes(words[i]), 0, bytes, i * 2, 2);
            }
            return bytes;
        }

        /// <summary>
        /// Converts an array of bytes to an array of ushorts.
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Newly allocated ushort array</returns>
        public static ushort[] ToUInt16Array(byte[] bytes)
        {
            int count = bytes.Length / 2;
            ushort[] words = new ushort[count];
            for (int i = 0, p = 0; i < count; i++, p += 2)
            {
                words[i] = BitConverter.ToUInt16(bytes, p);
            }
            return words;
        }

        /// <summary>
        /// Converts an array of bytes to an array of shorts.
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Newly allocated short array</returns>
        public static short[] ToInt16Array(byte[] bytes)
        {
            int count = bytes.Length / 2;
            short[] words = new short[count];
            for (int i = 0, p = 0; i < count; i++, p += 2)
            {
                words[i] = BitConverter.ToInt16(bytes, p);
            }
            return words;
        }

        /// <summary>
        /// Converts an array of bytes to an array of uints.
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Newly allocated uint array</returns>
        public static uint[] ToUInt32Array(byte[] bytes)
        {
            int count = bytes.Length / 4;
            uint[] dwords = new uint[count];
            for (int i = 0, p = 0; i < count; i++, p += 4)
            {
                dwords[i] = BitConverter.ToUInt32(bytes, p);
            }
            return dwords;
        }
        /// <summary>
        /// Converts an array of bytes to an array of uints.
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Newly allocated int array</returns>
        public static int[] ToInt32Array(byte[] bytes)
        {
            int count = bytes.Length / 4;
            int[] dwords = new int[count];
            for (int i = 0, p = 0; i < count; i++, p += 4)
            {
                dwords[i] = (int)BitConverter.ToUInt32(bytes, p);
            }
            return dwords;
        }

        /// <summary>
        /// Converts an array of bytes to an array of floats.
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Newly allocated float array</returns>
        public static float[] ToFloatArray(byte[] bytes)
        {
            int count = bytes.Length / 4;
            float[] floats = new float[count];
            for (int i = 0, p = 0; i < count; i++, p += 4)
            {
                floats[i] = BitConverter.ToSingle(bytes, p);
            }
            return floats;
        }

        /// <summary>
        /// Converts an array of bytes to an array of doubles.
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Newly allocated double array</returns>
        public static double[] ToDoubleArray(byte[] bytes)
        {
            int count = bytes.Length / 8;
            double[] doubles = new double[count];
            for (int i = 0, p = 0; i < count; i++, p += 8)
            {
                doubles[i] = BitConverter.ToDouble(bytes, p);
            }
            return doubles;
        }

        /// <summary>
        /// Swaps the bytes of an array of unsigned words.
        /// </summary>
        /// <param name="words">Array of ushorts</param>
        public static void SwapBytes(ushort[] words)
        {
            int count = words.Length;
            for (int i = 0; i < count; i++)
            {
                ushort u = words[i];
                words[i] = unchecked((ushort)((u >> 8) | (u << 8)));
            }
        }

        /// <summary>
        /// Swaps the bytes of an array of signed words.
        /// </summary>
        /// <param name="words">Array of shorts</param>
        public static void SwapBytes(short[] words)
        {
            int count = words.Length;
            for (int i = 0; i < count; i++)
            {
                short u = words[i];
                words[i] = unchecked((short)((u >> 8) | (u << 8)));
            }
        }
    }
}
