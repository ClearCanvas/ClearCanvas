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
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Manifest
{
	/// <summary>
	/// Class for Encryption/Decription of the ProductSettings in the app.config.
	/// </summary>
	internal static class ProductSettingsEncryption
	{
		/// <summary>
		/// Decrypt a Product Settings string.
		/// </summary>
		/// <param name="string">The string to decrypt.</param>
		/// <returns>The decrypted string.</returns>
		public static string Decrypt(string @string)
		{
			if (String.IsNullOrEmpty(@string))
				return @string;

			string result;
			try
			{
				using (var dataStream = new MemoryStream(Convert.FromBase64String(@string)))
				using (var cryptoService = new XorCryptoServiceProvider {Key = Encoding.UTF8.GetBytes(@"ClearCanvas"), IV = Encoding.UTF8.GetBytes(@"IsSoCool")})
				using (var cryptoStream = new CryptoStream(dataStream, cryptoService.CreateDecryptor(), CryptoStreamMode.Read))
				using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
				{
					result = reader.ReadToEnd().TrimEnd('\0');
				}
			}
			catch (Exception)
			{
				result = string.Empty;
			}
			return result;
		}
	}
}