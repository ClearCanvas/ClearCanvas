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

using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Functional Group Macro
	/// </summary>
	public abstract class FunctionalGroupMacro : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalGroupMacro"/> class.
		/// </summary>
		protected FunctionalGroupMacro() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalGroupMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		protected FunctionalGroupMacro(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Enumerates the <see cref="DicomTag"/>s used by this functional group macro.
		/// </summary>
		public abstract IEnumerable<uint> DefinedTags { get; }

		/// <summary>
		/// Enumerates the nested <see cref="DicomTag"/>s used in the sequence attribute defined by this functional group macro.
		/// </summary>
		public abstract IEnumerable<uint> NestedTags { get; }

		/// <summary>
		/// Gets a value indicating whether or not the sequence attribute used by this functional group macro can potentially have multiple (more than one) sequence item.
		/// </summary>
		public virtual bool CanHaveMultipleItems
		{
			get { return false; }
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public abstract void InitializeAttributes();

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public virtual bool HasValues()
		{
			return DefinedTags.Any(t =>
			                       	{
			                       		DicomAttribute a;
			                       		return DicomAttributeProvider.TryGetAttribute(t, out a) && !a.IsEmpty;
			                       	});
		}

		/// <summary>
		/// Gets the singleton sequence item in the functional group sequence if the functional group cannot have multiple items. Returns NULL otherwise.
		/// </summary>
		public DicomSequenceItem SingleItem
		{
			get
			{
				DicomAttribute a;
				return !CanHaveMultipleItems && DicomAttributeProvider.TryGetAttribute(DefinedTags.Single(), out a) && a.Count > 0 ? ((DicomAttributeSQ) a)[0] : null;
			}
		}
	}
}