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
	/// Categorizations for special types of SOP classes.
	/// </summary>
	[Flags]
	public enum SopClassCategory
	{
		Uncategorized = 0,
		Meta = 0x1,
		Storage = 0x2,
		Image = 0x4 | Storage
	}

	/// <summary>
	/// This class contains defines for all DICOM SOP Classes.
	/// </summary>
	public partial class SopClass : IEquatable<SopClass>
	{
		private static readonly object _updateLock = new object();
		private static Dictionary<String, SopClass> _sopList = new Dictionary<String, SopClass>();

		private readonly String _sopName;
		private readonly String _sopUid;
		private readonly SopClassCategory _category;

		/// <summary> Property that represents the Name of the SOP Class. </summary>
		public String Name
		{
			get { return _sopName; }
		}

		/// <summary> Property that represents the Uid for the SOP Class. </summary>
		public String Uid
		{
			get { return _sopUid; }
		}

		/// <summary> Property that returns a DicomUid that represents the SOP Class. </summary>
		public DicomUid DicomUid
		{
			get { return new DicomUid(_sopUid, _sopName, Meta ? UidType.MetaSOPClass : UidType.SOPClass); }
		}

		/// <summary> Indicates whether or not this is a meta SOP Class. </summary>
		public bool Meta
		{
			get { return (_category & SopClassCategory.Meta) == SopClassCategory.Meta; }
		}

		/// <summary> Indicates whether or not this is a storage SOP Class. </summary>
		public bool IsStorage
		{
			get { return (_category & SopClassCategory.Storage) == SopClassCategory.Storage; }
		}

		/// <summary> Indicates whether or not this SOP Class is an image. </summary>
		/// <remarks> At least where this property is concerned, an image is one that makes use of the Pixel Data (7FE0,0010)
		/// attribute to encode one or more 2-D image frames that can be rendered without special processing.
		/// Although some SOP Classes, like waveforms (ECG, audio), are usually represented as images or videos,
		/// they are not actually images from an encoding perspective. </remarks>
		public bool IsImage
		{
			get { return (_category & SopClassCategory.Image) == SopClassCategory.Image; }
		}

		/// <summary> Constructor to create SopClass object. </summary>
		[Obsolete("Use the constructor that takes a SopClassCategory instead.")]
		public SopClass(String name,
		                String uid,
		                bool isMeta)
			: this(name, uid, isMeta ? SopClassCategory.Meta : SopClassCategory.Uncategorized) {}

		/// <summary> Constructor to create SopClass object. </summary>
		public SopClass(String name,
		                String uid,
		                SopClassCategory category)
		{
			_sopName = name;
			_sopUid = uid;
			_category = category;
		}

		public bool Equals(SopClass other)
		{
			return !ReferenceEquals(other, null) && _sopUid == other._sopUid;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SopClass);
		}

		public override int GetHashCode()
		{
			return _sopUid.GetHashCode();
		}

		/// <summary>Override that displays the name of the SOP Class.</summary>
		public override string ToString()
		{
			return Name;
		}

		public static bool operator ==(SopClass x, SopClass y)
		{
			return Equals(x, y);
		}

		public static bool operator !=(SopClass x, SopClass y)
		{
			return !Equals(x, y);
		}

		/// <summary>
		/// Gets a list of all the registered SOP Classes.
		/// </summary>
		/// <returns></returns>
		public static IList<SopClass> GetRegisteredSopClasses()
		{
			return _sopList.Values.ToList();
		}

		/// <summary>Retrieve a SopClass object associated with the Uid.</summary>
		public static SopClass GetSopClass(String uid)
		{
			SopClass theSop;
			if (!_sopList.TryGetValue(uid, out theSop))
				theSop = new SopClass(string.Format("Generated: '{0}'", uid), uid, SopClassCategory.Uncategorized);
			return theSop;
		}

		/// <summary>
		/// Registers a private SOP Class.
		/// </summary>
		/// <param name="sopClass">The private SOP Class to register.</param>
		/// <returns></returns>
		public static SopClass RegisterSopClass(SopClass sopClass)
		{
			Platform.CheckForNullReference("sopClass", "sopClass");

			SopClass theSop;
			if (!_sopList.TryGetValue(sopClass.Uid, out theSop))
			{
				lock (_updateLock)
				{
					var sopClasses = new Dictionary<string, SopClass>(_sopList);
					sopClasses.Add(sopClass.Uid, theSop = sopClass);
					Interlocked.Exchange(ref _sopList, sopClasses);
				}
			}
			return theSop;
		}

		/// <summary>
		/// Unregisters a private SOP Class.
		/// </summary>
		/// <param name="sopClass">The private SOP Class to unregister.</param>
		/// <returns></returns>
		public static SopClass UnregisterSopClass(SopClass sopClass)
		{
			Platform.CheckForNullReference("sopClass", "sopClass");

			SopClass theSop;
			if (_sopList.TryGetValue(sopClass.Uid, out theSop))
			{
				lock (_updateLock)
				{
					var sopClasses = new Dictionary<string, SopClass>(_sopList);
					sopClasses.Remove(sopClass.Uid);
					Interlocked.Exchange(ref _sopList, sopClasses);
				}
			}
			return theSop;
		}
	}
}