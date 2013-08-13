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
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Manifest
{
	/// <summary>
	/// Represents a file contained in a <see cref="ClearCanvasManifest"/>.
	/// </summary>
	[XmlRoot("File")]
	public class ManifestFile
	{
		private static readonly string _checksumTypeMd5 = typeof (MD5CryptoServiceProvider).ToString();
		private const string _checksumTypeSha256 = "SHA256";

		/// <summary>
		/// The relative path of the file or directory in the manifest.
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// The Version of the file.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// The LegalCopyright of the file.
		/// </summary>
		public string Copyright { get; set; }

		/// <summary>
		/// The class used to generate the Checksum.
		/// </summary>
		public string ChecksumType { get; set; }

		/// <summary>
		/// The generated checksum.
		/// </summary>
		public string Checksum { get; set; }

		/// <summary>
		/// The CreatedDate of the file.
		/// </summary>
		[DefaultValue(null)]
		public DateTime? Timestamp { get; set; }

		/// <summary>
		/// An attribute telling if the file is optional.
		/// </summary>
		[XmlAttribute(AttributeName = "optional", DataType = "boolean")]
		[DefaultValue(false)]
		public Boolean Optional { get; set; }

		/// <summary>
		/// An attribute telling if the file should be ignored.
		/// </summary>
		[XmlAttribute(AttributeName = "ignore", DataType = "boolean")]
		[DefaultValue(false)]
		public Boolean Ignore { get; set; }

		[XmlIgnore]
		public Boolean IsDirectory { get; set; }

		/// <summary>
		/// Generate a checksum.
		/// </summary>
		/// <param name="fullPath">The full path of the file to generate a checksum for.</param>
		public void GenerateChecksum(string fullPath)
		{
			Checksum = GenerateChecksum<SHA256CryptoServiceProvider2>(fullPath);
			ChecksumType = _checksumTypeSha256;
		}

		private static string GenerateChecksum<T>(string fullPath)
			where T : HashAlgorithm, new()
		{
			using (var hash = new T())
			using (var file = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var hashBytes = hash.ComputeHash(file);
				file.Close();

				return StringUtilities.ToHexString(hashBytes, true);
			}
		}

		/// <summary>
		/// Verify a checksum.
		/// </summary>
		/// <param name="fullPath">The path to the file.</param>
		public void VerifyChecksum(string fullPath)
		{
			if (ChecksumType == _checksumTypeSha256)
			{
				// exception is thrown if checksum fails verification
				TryVerifyChecksum<SHA256CryptoServiceProvider2>(fullPath, Checksum);
			}
			else if (ChecksumType == _checksumTypeMd5)
			{
				// exception is thrown if checksum fails verification
				TryVerifyChecksum<MD5CryptoServiceProvider>(fullPath, Checksum);
			}
			else
			{
				throw new ApplicationException("Unknown checksum type: " + ChecksumType);
			}
		}

		private static void TryVerifyChecksum<T>(string fullPath, string checksum)
			where T : HashAlgorithm, new()
		{
			string hashString;

			try
			{
				using (var hash = new T())
				using (var file = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var hashBytes = hash.ComputeHash(file);
					file.Close();

					hashString = StringUtilities.ToHexString(hashBytes, true);
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Checksum could not be verified for file " + fullPath, ex);
			}

			if (!string.Equals(checksum, hashString, StringComparison.InvariantCultureIgnoreCase))
				throw new ApplicationException("Checksum does not match for file " + fullPath);
		}
	}
}