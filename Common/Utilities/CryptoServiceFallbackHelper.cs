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
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;

// ReSharper disable InconsistentNaming

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Provides access to <see cref="SHA256CryptoServiceProvider"/>, falling back to <see cref="SHA256Managed"/> if the platform does not support the native, FIPS-compliant implementation.
	/// </summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class SHA256CryptoServiceProvider2 : SHA256
	{
		private SHA256 _implementation;

		[SecurityCritical]
		public SHA256CryptoServiceProvider2()
		{
			_implementation = CryptoServiceFallbackHelper.CreateImplementation<SHA256, SHA256CryptoServiceProvider, SHA256Managed>();
		}

		[SecurityCritical]
		protected override void Dispose(bool disposing)
		{
			if (disposing && _implementation != null)
			{
				_implementation.Dispose();
				_implementation = null;
			}
			base.Dispose(disposing);
		}

		[SecurityCritical]
		public override void Initialize()
		{
			_implementation.Initialize();
		}

		[SecurityCritical]
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			_implementation.HashCore(array, ibStart, cbSize);
		}

		[SecurityCritical]
		protected override byte[] HashFinal()
		{
			return _implementation.HashFinal();
		}
	}

	/// <summary>
	/// Provides access to <see cref="SHA384CryptoServiceProvider"/>, falling back to <see cref="SHA384Managed"/> if the platform does not support the native, FIPS-compliant implementation.
	/// </summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class SHA384CryptoServiceProvider2 : SHA384
	{
		private SHA384 _implementation;

		[SecurityCritical]
		public SHA384CryptoServiceProvider2()
		{
			_implementation = CryptoServiceFallbackHelper.CreateImplementation<SHA384, SHA384CryptoServiceProvider, SHA384Managed>();
		}

		[SecurityCritical]
		protected override void Dispose(bool disposing)
		{
			if (disposing && _implementation != null)
			{
				_implementation.Dispose();
				_implementation = null;
			}
			base.Dispose(disposing);
		}

		[SecurityCritical]
		public override void Initialize()
		{
			_implementation.Initialize();
		}

		[SecurityCritical]
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			_implementation.HashCore(array, ibStart, cbSize);
		}

		[SecurityCritical]
		protected override byte[] HashFinal()
		{
			return _implementation.HashFinal();
		}
	}

	/// <summary>
	/// Provides access to <see cref="SHA512CryptoServiceProvider"/>, falling back to <see cref="SHA512Managed"/> if the platform does not support the native, FIPS-compliant implementation.
	/// </summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class SHA512CryptoServiceProvider2 : SHA512
	{
		private SHA512 _implementation;

		[SecurityCritical]
		public SHA512CryptoServiceProvider2()
		{
			_implementation = CryptoServiceFallbackHelper.CreateImplementation<SHA512, SHA512CryptoServiceProvider, SHA512Managed>();
		}

		[SecurityCritical]
		protected override void Dispose(bool disposing)
		{
			if (disposing && _implementation != null)
			{
				_implementation.Dispose();
				_implementation = null;
			}
			base.Dispose(disposing);
		}

		[SecurityCritical]
		public override void Initialize()
		{
			_implementation.Initialize();
		}

		[SecurityCritical]
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			_implementation.HashCore(array, ibStart, cbSize);
		}

		[SecurityCritical]
		protected override byte[] HashFinal()
		{
			return _implementation.HashFinal();
		}
	}

	internal static class CryptoServiceFallbackHelper
	{
		private static readonly MethodInfo _hashCore = typeof (HashAlgorithm).GetMethod("HashCore", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new[] {typeof (byte[]), typeof (int), typeof (int)}, null);
		private static readonly MethodInfo _hashFinal = typeof (HashAlgorithm).GetMethod("HashFinal", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

		private static bool _infoLogged = false;

		internal static TBase CreateImplementation<TBase, TPrime, TFallback>()
			where TPrime : TBase, new()
			where TFallback : TBase, new()
		{
			try
			{
				return new TPrime();
			}
			catch (PlatformNotSupportedException)
			{
				return CreateFallbackImplementation<TFallback>();
			}
			catch (TargetInvocationException ex)
			{
				// looks silly, but necessary because the PlatformNotSupportedException might be wrapped in a TargetInvocationException
				if (ex.InnerException is PlatformNotSupportedException)
					return CreateFallbackImplementation<TFallback>();
				throw;
			}
		}

		private static TFallback CreateFallbackImplementation<TFallback>()
			where TFallback : new()
		{
			try
			{
				return new TFallback();
			}
			catch (Exception)
			{
				var osVersion = Environment.OSVersion;
				if (!_infoLogged && osVersion.Platform == PlatformID.Win32NT && osVersion.Version.Major <= 5)
				{
					// the workaround for Windows XP and Server 2003 is to copy all contents of the following registry key
					// SOURCE: HKLM\SOFTWARE\Microsoft\Cryptography\Defaults\Provider\Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)
					// TARGET: HKLM\SOFTWARE\Microsoft\Cryptography\Defaults\Provider\Microsoft Enhanced RSA and AES Cryptographic Provider
					// if the source key is missing, user doesn't have the latest Service Pack, or is on wrong OS
					// older/other OS versions may simply not support either requested implementation
					// Ref: http://support.microsoft.com/kb/2541382/ja
					//      http://tsells.wordpress.com/2012/06/09/issues-with-sha-256-encryption-and-fips-compliance-with-windows-xp/
					const string message = "Failed to initialize the requested cryptographic algorithm. "
					                       + "If your system requires FIPS-compliant implementations, you may need to apply "
					                       + "the workaround specified in Microsoft KB2541382 (http://support.microsoft.com/kb/2541382/ja)";
					Platform.Log(LogLevel.Info, message);
					_infoLogged = true;
				}
				throw;
			}
		}

		public static void HashCore(this HashAlgorithm hashAlgorithm, byte[] array, int ibStart, int cbSize)
		{
			_hashCore.Invoke(hashAlgorithm, new object[] {array, ibStart, cbSize});
		}

		public static byte[] HashFinal(this HashAlgorithm hashAlgorithm)
		{
			return _hashFinal.Invoke(hashAlgorithm, null) as byte[];
		}
	}
}

// ReSharper restore InconsistentNaming