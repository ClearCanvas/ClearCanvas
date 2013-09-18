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

#if UNIT_TESTS

// ReSharper disable LocalizableElement

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
	[TestFixture]
	internal class XorCryptoServiceProviderTests
	{
		[Test]
		public void TestRoundtripEncryption()
		{
			const string key = "asdfasdsfhddshsdf";
			const string iv = "12345678";

			var encoding = Encoding.UTF8;

			TestRoundtripEncryption("1", key, iv, encoding);
			TestRoundtripEncryption("123", key, iv, encoding);
			TestRoundtripEncryption("1234567890123", key, iv, encoding);
			TestRoundtripEncryption("Test Test Test Test Test Test a    ", key, iv, encoding);
			TestRoundtripEncryption("Test Test Test Test Test Test hsjkfhsdfkhdskdsjshkf \0\0\0", key, iv, encoding);
		}

		private static void TestRoundtripEncryption(string inputText, string key, string iv, Encoding encoding)
		{
			inputText = inputText.TrimEnd('\0');

			var keyBytes = encoding.GetBytes(key);
			var ivBytes = encoding.GetBytes(iv);
			var inputBytes = encoding.GetBytes(inputText);

			Console.WriteLine("=======================================");
			Console.WriteLine("{0,12}: {1}", "Input", Convert.ToBase64String(inputBytes));

			using (var cryptoServiceProvider = new XorCryptoServiceProvider {Key = keyBytes, IV = ivBytes})
			{
				byte[] cipherBytes;

				using (var memoryStream = new MemoryStream())
				using (var cryptoStream = new CryptoStream(memoryStream, cryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
				{
					cryptoStream.Write(inputBytes, 0, inputBytes.Length);
					cryptoStream.FlushFinalBlock();
					cipherBytes = memoryStream.ToArray();

					Console.WriteLine("{0,12}: {1}", "Encrypted", Convert.ToBase64String(cipherBytes));

					Assert.AreNotEqual(inputText, encoding.GetString(cipherBytes), "cipher form should not be same as plaintext form");
					Assert.LessOrEqual(inputBytes.Length*8/3, GetBitDelta(inputBytes, cipherBytes), "cipher form should not be similar to plaintext form");
				}

				using (var memoryStream = new MemoryStream(cipherBytes))
				using (var cryptoStream = new CryptoStream(memoryStream, cryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read))
				using (var streamReader = new StreamReader(cryptoStream, encoding))
				{
					var outputText = streamReader.ReadToEnd().TrimEnd('\0');
					var outputBytes = encoding.GetBytes(outputText);

					Console.WriteLine("{0,12}: {1}", "Decrypted", Convert.ToBase64String(outputBytes));

					Assert.AreEqual(inputText, outputText, "decrypted plaintext should be equal to original plaintext");
				}
			}
		}

		private static int GetBitDelta(byte[] a, byte[] b)
		{
			// basically just counts all the bits that are different (every bit that exists in one but not the other shall count as a difference)
			var l = Math.Min(a.Length, b.Length);
			int d = Math.Max(a.Length, b.Length) - l;
			for (var n = 0; n < l; ++n)
			{
				var v = (byte) (a[n] ^ b[n]);
				for (; v != 0; v >>= 1)
					d += v & 1;
			}
			return d;
		}
	}
}

// ReSharper restore LocalizableElement

#endif