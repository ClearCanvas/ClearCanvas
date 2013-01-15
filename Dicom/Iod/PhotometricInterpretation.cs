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

namespace ClearCanvas.Dicom.Iod
{
	public class PhotometricInterpretation
	{
		public static PhotometricInterpretation Unknown = new PhotometricInterpretation("Unknown", "", false);

		public static PhotometricInterpretation Monochrome1 = new PhotometricInterpretation("Monochrome1", "MONOCHROME1", false);
		public static PhotometricInterpretation Monochrome2 = new PhotometricInterpretation("Monochrome2", "MONOCHROME2", false);
		public static PhotometricInterpretation PaletteColor = new PhotometricInterpretation("Palette Color", "PALETTE COLOR", true);
		public static PhotometricInterpretation Rgb = new PhotometricInterpretation("Rgb", "RGB", true);
		public static PhotometricInterpretation YbrFull = new PhotometricInterpretation("Ybr Full", "YBR_FULL", true);
		public static PhotometricInterpretation YbrFull422 = new PhotometricInterpretation("Ybr (Full 4-2-2)", "YBR_FULL_422", true);
		public static PhotometricInterpretation YbrIct = new PhotometricInterpretation("Ybr (Ict)", "YBR_ICT", true);
		public static PhotometricInterpretation YbrPartial422 = new PhotometricInterpretation("Ybr (Partial 4-2-2)", "YBR_PARTIAL_422", true);
		public static PhotometricInterpretation YbrRct = new PhotometricInterpretation("Ybr (Rct)", "YBR_RCT", true);

		private static readonly Dictionary<string, PhotometricInterpretation> _photometricInterpretations = new Dictionary<string, PhotometricInterpretation>();

		private readonly string _name;
		private readonly string _code;
		private readonly bool _isColor;

		internal PhotometricInterpretation(string name, string code, bool isColor)
		{
			_name = name;
			_code = code;
			_isColor = isColor;
		}

		static PhotometricInterpretation()
		{
			Add(Monochrome1);
			Add(Monochrome2);
			Add(PaletteColor);
			Add(Rgb);
			Add(YbrFull);
			Add(YbrFull422);
			Add(YbrIct);
			Add(YbrPartial422);
			Add(YbrRct);
		}

		private static void Add(PhotometricInterpretation photometricInterpretation)
		{
			_photometricInterpretations.Add(photometricInterpretation.Code, photometricInterpretation);
		}

		public string Name
		{
			get { return _name;	}
		}

		public string Code
		{
			get { return _code; }	
		}

		public bool IsColor
		{
			get { return _isColor; }
		}

		public override int GetHashCode()
		{
			return _code.GetHashCode();
		}

		public override bool Equals(object obj)
		{
 			 if (obj is PhotometricInterpretation)
 			 	return ((PhotometricInterpretation) obj).Code == this.Code;

			return base.Equals(obj);
		}

		public override string ToString()
		{
			return _name;
		}
				
		public static PhotometricInterpretation FromCodeString(string codeString)
		{
			PhotometricInterpretation theInterpretation;
			if (!_photometricInterpretations.TryGetValue(codeString ?? string.Empty, out theInterpretation))
				return Unknown;
			return theInterpretation;
		}
	}
}
