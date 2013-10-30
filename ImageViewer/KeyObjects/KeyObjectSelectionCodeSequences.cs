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

using System.Globalization;
using ClearCanvas.Dicom.Iod.ContextGroups;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	/// <summary>
	/// Static class defining DICOM code sequences used in key object selections.
	/// </summary>
	/// <remarks>
	/// <para>Due to the relatively new nature of key object support in the ClearCanvas Framework, this API may be more prone to changes in the next release.</para>
	/// </remarks>
	public static class KeyObjectSelectionCodeSequences
	{
		/// <summary>
		/// Gets the code for DCM 113011 Document Title Modifier.
		/// </summary>
		public static readonly Code DocumentTitleModifier = new Code(113011, "Document Title Modifier");

		/// <summary>
		/// Gets the code for DCM 113012 Key Object Description.
		/// </summary>
		public static readonly Code KeyObjectDescription = new Code(113012, "Key Object Description");

		/// <summary>
		/// Gets the code for DCM 121005 Observer Type.
		/// </summary>
		public static readonly Code ObserverType = new Code(121005, "Observer Type");

		/// <summary>
		/// Gets the code for DCM 121008 Person Observer Name.
		/// </summary>
		public static readonly Code PersonObserverName = new Code(121008, "Person Observer Name");

		/// <summary>
		/// A DICOM code sequence used in key object selections.
		/// </summary>
		public sealed class Code : ContextGroupBase<Code>.ContextGroupItemBase
		{
			internal Code(int codeValue, string codeMeaning) : base("DCM", codeValue.ToString(CultureInfo.InvariantCulture), codeMeaning) {}
		}
	}
}