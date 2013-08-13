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
using System.IO;
using System.Security.Cryptography;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Helper methods for computing various sized hashes for non-cryptographic purposes.
	/// </summary>
	/// <remarks>
	/// These methods provide a quick way to calculate hashes where the size of the output is the only thing that matters,
	/// and the actual algorithm details, cryptographic suitability and cross-version consistency do not matter.
	/// If you need such guarantees, invoke the various types in <see cref="System.Security.Cryptography"/> directly.
	/// The implementations provided here may change from version to version, so consider them suitable only for in-memory use.
	/// </remarks>
	public static class HashUtilities
	{
		#region 128-bit

		/// <summary>
		/// Computes a 128-bit (16-byte) hash from the specified input data.
		/// </summary>
		/// <remarks>
		/// This method provides no guarantees as to consistency over different versions of the framework,
		/// and should not be considered cryptographically secure.
		/// </remarks>
		/// <param name="bytes">The input data to be hashed.</param>
		/// <returns>A byte array containing the hash (16 bytes).</returns>
		public static byte[] ComputeHash128(byte[] bytes)
		{
			return ComputeHash128(bytes, 0, bytes.Length);
		}

		/// <summary>
		/// Computes a 128-bit (16-byte) hash from the specified input data.
		/// </summary>
		/// <remarks>
		/// This method provides no guarantees as to consistency over different versions of the framework,
		/// and should not be considered cryptographically secure.
		/// </remarks>
		/// <param name="bytes">The input data to be hashed.</param>
		/// <param name="offset">The byte offset in <paramref name="bytes"/> from which to start hashing.</param>
		/// <param name="length">The number of bytes in <paramref name="bytes"/> to hash.</param>
		/// <returns>A byte array containing the hash (16 bytes).</returns>
		public static byte[] ComputeHash128(byte[] bytes, int offset, int length)
		{
			// we don't simply use MD5 because it throws an exception if the OS has strict cryptographic policies in place (e.g. FIPS)
			// note: truncation of SHA256 seems to be an accepted method of producing a shorter hash
			// * RFC3874 describes the SHA224 algorithm, which is just a truncated SHA256 hash with a different initialization vector
			// * RFC4868 describes HMAC, a scheme for origin authentication and integrity verification which incorporates truncated SHA256 hashes
			// * Altman M. {A Fingerprint Method for Scientific Data Verification}. In: Sobh T Proceedings of the International Conference on Systems Computing Sciences and Software Engineering 2007. New York: Springer Netherlands; 2008. p. 311–316.
			// * a discussion of truncating SHA512 to 256 at http://crypto.stackexchange.com/questions/3153/sha-256-vs-any-256-bits-of-sha-512-which-is-more-secure
			using (var sha256 = new SHA256CryptoServiceProvider2())
			{
				var hash = sha256.ComputeHash(bytes, offset, length);
				var result = new byte[16];
				Buffer.BlockCopy(hash, 0, result, 0, 16);
				return result;
			}
		}

		/// <summary>
		/// Computes a 128-bit (16-byte) hash from the specified input data.
		/// </summary>
		/// <remarks>
		/// This method provides no guarantees as to consistency over different versions of the framework,
		/// and should not be considered cryptographically secure.
		/// </remarks>
		/// <param name="stream">The input stream to be hashed (starts from current stream position).</param>
		/// <returns>A byte array containing the hash (16 bytes).</returns>
		public static byte[] ComputeHash128(Stream stream)
		{
			// we don't simply use MD5 because it throws an exception if the OS has strict cryptographic policies in place (e.g. FIPS)
			// note: truncation of SHA256 seems to be an accepted method of producing a shorter hash - see other overload
			using (var sha256 = new SHA256CryptoServiceProvider2())
			{
				var hash = sha256.ComputeHash(stream);
				var result = new byte[16];
				Buffer.BlockCopy(hash, 0, result, 0, 16);
				return result;
			}
		}

		#endregion

		#region 256-bit

		/// <summary>
		/// Computes a 256-bit (32-byte) hash from the specified input data.
		/// </summary>
		/// <remarks>
		/// This method provides no guarantees as to consistency over different versions of the framework,
		/// and should not be considered cryptographically secure.
		/// </remarks>
		/// <param name="bytes">The input data to be hashed.</param>
		/// <returns>A byte array containing the hash (32 bytes).</returns>
		public static byte[] ComputeHash256(byte[] bytes)
		{
			return ComputeHash256(bytes, 0, bytes.Length);
		}

		/// <summary>
		/// Computes a 256-bit (32-byte) hash from the specified input data.
		/// </summary>
		/// <remarks>
		/// This method provides no guarantees as to consistency over different versions of the framework,
		/// and should not be considered cryptographically secure.
		/// </remarks>
		/// <param name="bytes">The input data to be hashed.</param>
		/// <param name="offset">The byte offset in <paramref name="bytes"/> from which to start hashing.</param>
		/// <param name="length">The number of bytes in <paramref name="bytes"/> to hash.</param>
		/// <returns>A byte array containing the hash (32 bytes).</returns>
		public static byte[] ComputeHash256(byte[] bytes, int offset, int length)
		{
			using (var sha256 = new SHA256CryptoServiceProvider2())
			{
				return sha256.ComputeHash(bytes, offset, length);
			}
		}

		/// <summary>
		/// Computes a 256-bit (32-byte) hash from the specified input data.
		/// </summary>
		/// <remarks>
		/// This method provides no guarantees as to consistency over different versions of the framework,
		/// and should not be considered cryptographically secure.
		/// </remarks>
		/// <param name="stream">The input stream to be hashed (starts from current stream position).</param>
		/// <returns>A byte array containing the hash (32 bytes).</returns>
		public static byte[] ComputeHash256(Stream stream)
		{
			using (var sha256 = new SHA256CryptoServiceProvider2())
			{
				return sha256.ComputeHash(stream);
			}
		}

		#endregion

		#region Guid

		/// <summary>
		/// Computes a 128-bit <see cref="Guid"/> hash from the specified input data.
		/// </summary>
		/// <remarks>
		/// This method provides no guarantees as to consistency over different versions of the framework,
		/// and should not be considered cryptographically secure.
		/// </remarks>
		/// <param name="bytes">The input data to be hashed.</param>
		/// <returns>A <see cref="Guid"/> containing the hash.</returns>
		public static Guid ComputeHashGuid(byte[] bytes)
		{
			return new Guid(ComputeHash128(bytes));
		}

		/// <summary>
		/// Computes a 128-bit <see cref="Guid"/> hash from the specified input data.
		/// </summary>
		/// <remarks>
		/// This method provides no guarantees as to consistency over different versions of the framework,
		/// and should not be considered cryptographically secure.
		/// </remarks>
		/// <param name="bytes">The input data to be hashed.</param>
		/// <param name="offset">The byte offset in <paramref name="bytes"/> from which to start hashing.</param>
		/// <param name="length">The number of bytes in <paramref name="bytes"/> to hash.</param>
		/// <returns>A <see cref="Guid"/> containing the hash.</returns>
		public static Guid ComputeHashGuid(byte[] bytes, int offset, int length)
		{
			return new Guid(ComputeHash128(bytes, offset, length));
		}

		/// <summary>
		/// Computes a 128-bit <see cref="Guid"/> hash from the specified input data.
		/// </summary>
		/// <remarks>
		/// This method provides no guarantees as to consistency over different versions of the framework,
		/// and should not be considered cryptographically secure.
		/// </remarks>
		/// <param name="stream">The input stream to be hashed (starts from current stream position).</param>
		/// <returns>A <see cref="Guid"/> containing the hash.</returns>
		public static Guid ComputeHashGuid(Stream stream)
		{
			return new Guid(ComputeHash128(stream));
		}

		#endregion
	}
}