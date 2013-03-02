﻿#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	internal static class DicomAttributeMethods
	{
		public static bool? GetBoolean(this DicomAttribute attribute, int i, string trueString, string falseString)
		{
			var value = attribute.GetString(i, string.Empty);
			if (string.Equals(value, trueString, StringComparison.InvariantCultureIgnoreCase)) return true;
			else if (string.Equals(value, falseString, StringComparison.InvariantCultureIgnoreCase)) return false;
			return null;
		}

		public static void SetBoolean(this DicomAttribute attribute, int i, bool? value, string trueString, string falseString)
		{
			if (!value.HasValue) attribute.SetStringValue(string.Empty);
			else attribute.SetStringValue(value.Value ? trueString : falseString);
		}
	}
}