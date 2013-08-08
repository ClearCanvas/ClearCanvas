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
using System.Security.Cryptography;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// A very weak XOR implementation of a symmetric &quot;encryption&quot; scheme - i.e. do not use if you actually care about anyone breaking the cipher.
	/// </summary>
	/// <remarks>
	/// Block size is 64-bits, Key Sizes are 64 to 1024 bits in 8 bit increments. Also, algorithm pads upon encryption and length is not tracked, so
	/// length must be stored separately if you padding characters may be significant.
	/// </remarks>
	internal sealed class XorCryptoServiceProvider : SymmetricAlgorithm
	{
		public XorCryptoServiceProvider()
		{
			LegalBlockSizesValue = new[] {new KeySizes(64, 64, 8)};
			BlockSizeValue = 64;

			LegalKeySizesValue = new[] {new KeySizes(64, 1024, 8)};
			KeySizeValue = 64;

			PaddingValue = PaddingMode.Zeros;
			ModeValue = CipherMode.CBC;
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIv)
		{
			return new CryptoTransform(rgbKey, rgbIv);
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIv)
		{
			return new CryptoTransform(rgbKey, rgbIv);
		}

		public override void GenerateKey()
		{
			using (var rng = new RNGCryptoServiceProvider())
			{
				var key = new byte[KeySize/8];
				rng.GetBytes(key);
				Key = key;
			}
		}

		public override void GenerateIV()
		{
			using (var rng = new RNGCryptoServiceProvider())
			{
				var iv = new byte[BlockSize/8];
				rng.GetBytes(iv);
				IV = iv;
			}
		}

		private class CryptoTransform : ICryptoTransform
		{
			private byte[] _rgbKey;
			private byte[] _rgbIv;
			private int _round = 0;

			public CryptoTransform(byte[] rgbKey, byte[] rgbIv)
			{
				_rgbKey = rgbKey;
				_rgbIv = rgbIv;
			}

			public void Dispose()
			{
				_rgbKey = null;
				_rgbIv = null;
			}

			public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
			{
				Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);

				var ivLength = _rgbIv.Length;
				var keyLength = _rgbKey.Length;
				for (var n = 0; n < inputCount; ++n)
					outputBuffer[n + outputOffset] ^= (byte) (_rgbIv[n%ivLength] ^ _rgbKey[(_round*ivLength + n)%keyLength]);

				++_round;

				return inputCount;
			}

			public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
			{
				if (inputCount == 0) return new byte[0];

				var outputCount = OutputBlockSize;
				var outputBuffer = new byte[outputCount];
				Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, 0, inputCount);

				var ivLength = _rgbIv.Length;
				var keyLength = _rgbKey.Length;
				for (var n = 0; n < outputCount; ++n)
					outputBuffer[n] ^= (byte) (_rgbIv[n%ivLength] ^ _rgbKey[(_round*ivLength + n)%keyLength]);

				return outputBuffer;
			}

			public int InputBlockSize
			{
				get { return _rgbIv.Length; }
			}

			public int OutputBlockSize
			{
				get { return _rgbIv.Length; }
			}

			public bool CanTransformMultipleBlocks
			{
				get { return false; }
			}

			public bool CanReuseTransform
			{
				get { return false; }
			}
		}
	}
}