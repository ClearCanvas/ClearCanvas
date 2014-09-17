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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Enumerated value to differentiate between little and big endian.
	/// </summary>
	public enum Endian
	{
		Little,
		Big
	}

	/// <summary>
	/// This class contains transfer syntax definitions.
	/// </summary>
	public partial class TransferSyntax : IEquatable<TransferSyntax>
	{
		private static readonly object _updateLock = new object();
		private static readonly Dictionary<String, TransferSyntax> _transferSyntaxes = new Dictionary<String, TransferSyntax>();
		private static Dictionary<String, TransferSyntax> _privateTransferSyntaxes = new Dictionary<String, TransferSyntax>();

		private readonly bool _littleEndian;
		private readonly bool _encapsulated;
		private readonly bool _explicitVr;
		private readonly bool _deflate;
		private readonly bool _lossless;
		private readonly bool _lossy;
		private readonly String _name;
		private readonly String _uid;

		///<summary>
		/// Constructor for transfer syntax objects
		///</summary>
		public TransferSyntax(String name, String uid, bool bLittleEndian, bool bEncapsulated, bool bExplicitVr, bool bDeflate, bool bLossy, bool bLossless)
		{
			_uid = uid;
			_name = name;
			_littleEndian = bLittleEndian;
			_encapsulated = bEncapsulated;
			_explicitVr = bExplicitVr;
			_deflate = bDeflate;
			_lossy = bLossy;
			_lossless = bLossless;
		}

		public bool Equals(TransferSyntax other)
		{
			return !ReferenceEquals(other, null) && _uid == other._uid;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TransferSyntax);
		}

		public override int GetHashCode()
		{
			return _uid.GetHashCode();
		}

		///<summary>Override to the ToString() method, returns the name of the transfer syntax.</summary>
		public override String ToString()
		{
			return _name;
		}

		///<summary>Property representing the UID string of transfer syntax.</summary>
		public String UidString
		{
			get { return _uid; }
		}

		///<summary>Property representing the DicomUid of the transfer syntax.</summary>
		public DicomUid DicomUid
		{
			get { return new DicomUid(_uid, _name, UidType.TransferSyntax); }
		}

		///<summary>Property representing the name of the transfer syntax.</summary>
		public String Name
		{
			get { return _name; }
		}

		///<summary>Property representing if the transfer syntax is encoded as little endian.</summary>
		public bool LittleEndian
		{
			get { return _littleEndian; }
		}

		///<summary>Property representing the Endian enumerated value for the transfer syntax.</summary>
		public Endian Endian
		{
			get { return _littleEndian ? Endian.Little : Endian.Big; }
		}

		///<summary>Property representing if the transfer syntax is encoded as encapsulated.</summary>
		public bool Encapsulated
		{
			get { return _encapsulated; }
		}

		///<summary>Property representing if the transfer syntax is a lossy compression syntax.</summary>
		public bool LossyCompressed
		{
			get { return _lossy; }
		}

		///<summary>Property representing if the transfer syntax is a lossless compression syntax.</summary>
		public bool LosslessCompressed
		{
			get { return _lossless; }
		}

		///<summary>Property representing if the transfer syntax is encoded as explicit Value Representation.</summary>
		public bool ExplicitVr
		{
			get { return _explicitVr; }
		}

		///<summary>Property representing if the transfer syntax is encoded in deflate format.</summary>
		public bool Deflate
		{
			get { return _deflate; }
		}

		public static bool operator ==(TransferSyntax x, TransferSyntax y)
		{
			return Equals(x, y);
		}

		public static bool operator !=(TransferSyntax x, TransferSyntax y)
		{
			return !Equals(x, y);
		}

		/// <summary>
		/// Gets the <see cref="TransferSyntax"/> instance for a specific transfer syntax UID.
		/// </summary>
		public static TransferSyntax GetTransferSyntax(String uid)
		{
			TransferSyntax theSyntax;
			if (!_transferSyntaxes.TryGetValue(uid, out theSyntax) && !_privateTransferSyntaxes.TryGetValue(uid, out theSyntax))
				return null;
			return theSyntax;
		}

		/// <summary>
		/// Enumerates all registered transfer syntaxes.
		/// </summary>
		public static IEnumerable<TransferSyntax> TransferSyntaxes
		{
			get { return _transferSyntaxes.Values.Concat(_privateTransferSyntaxes.Values); }
		}

		/// <summary>
		/// Enumerates all registered transfer syntax UIDs.
		/// </summary>
		public static IEnumerable<string> TransferSyntaxUids
		{
			get { return _transferSyntaxes.Keys.Concat(_privateTransferSyntaxes.Keys); }
		}

		/// <summary>
		/// Registers a private transfer syntax.
		/// </summary>
		/// <param name="transferSyntax">The private transfer syntax to reigster.</param>
		public static void RegisterTransferSyntax(TransferSyntax transferSyntax)
		{
			Platform.CheckForNullReference(transferSyntax, "transferSyntax");
			Platform.CheckTrue(!_transferSyntaxes.ContainsKey(transferSyntax.UidString), "Cannot redefine a standard transfer syntax.");
			Platform.CheckTrue(!_privateTransferSyntaxes.ContainsKey(transferSyntax.UidString), "The specified private transfer syntax UID is already defined.");

			lock (_updateLock)
			{
				var transferSyntaxes = new Dictionary<string, TransferSyntax>(_privateTransferSyntaxes);
				transferSyntaxes.Add(transferSyntax.UidString, transferSyntax);
				Interlocked.Exchange(ref _privateTransferSyntaxes, transferSyntaxes);
			}
		}

		/// <summary>
		/// Unregisters a private transfer syntax.
		/// </summary>
		/// <param name="transferSyntax">The private transfer syntax to unreigster.</param>
		public static void UnregisterTransferSyntax(TransferSyntax transferSyntax)
		{
			Platform.CheckForNullReference(transferSyntax, "transferSyntax");

			lock (_updateLock)
			{
				var transferSyntaxes = new Dictionary<string, TransferSyntax>(_privateTransferSyntaxes);
				transferSyntaxes.Remove(transferSyntax.UidString);
				Interlocked.Exchange(ref _privateTransferSyntaxes, transferSyntaxes);
			}
		}
	}
}