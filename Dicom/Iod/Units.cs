#region License

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

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Defined terms for the <see cref="ClearCanvas.Dicom.DicomTags.Units"/> attribute.
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.9.1 (Table C.8-60)</remarks>
	public struct Units
	{
		/// <summary>
		/// Represents the empty value (indicating that units are not specified, as opposed to <see cref="None"/> which explicitly indicates that values have no units).
		/// </summary>
		public static readonly Units Empty = new Units();

		public static readonly Units Cnts = new Units("CNTS", "counts");

		/// <summary>
		/// Represents the NONE defined term (explicitly indicating that values have no units, as opposed to <see cref="Empty"/> which indicates that units are not specified).
		/// </summary>
		public static readonly Units None = new Units("NONE", "none");

		public static readonly Units Cm2 = new Units("CM2", "cm^2");
		public static readonly Units Cm2Ml = new Units("CM2ML", "cm^2/ml");
		public static readonly Units Pcnt = new Units("PCNT", "percent");
		public static readonly Units Cps = new Units("CPS", "counts/s");
		public static readonly Units Bqml = new Units("BQML", "Bq/ml");
		public static readonly Units MgMinMl = new Units("MGMINML", "mg/min/ml");
		public static readonly Units UmolMinMl = new Units("UMOLMINML", "umol/min/ml");
		public static readonly Units MlMinG = new Units("MLMING", "ml/min/g");
		public static readonly Units MlG = new Units("MLG", "ml/g");
		public static readonly Units _1Cm = new Units("1CM", "1/cm");
		public static readonly Units UmolMl = new Units("UMOLML", "umol/ml");
		public static readonly Units PropCnts = new Units("PROPCNTS", "proportional to counts");
		public static readonly Units PropCps = new Units("PROPCPS", "proportional to counts/s");
		public static readonly Units MlMinMl = new Units("MLMINML", "ml/min/ml");
		public static readonly Units MlMl = new Units("ML", "ml/ml");
		public static readonly Units GMl = new Units("GML", "g/ml");
		public static readonly Units StdDev = new Units("STDDEV", "standard deviations");

		/// <summary>
		/// Enumeration of defined terms.
		/// </summary>
		public static IEnumerable<Units> DefinedTerms
		{
			get
			{
				yield return Cnts;
				yield return None;
				yield return Cm2;
				yield return Cm2Ml;
				yield return Pcnt;
				yield return Cps;
				yield return Bqml;
				yield return MgMinMl;
				yield return UmolMinMl;
				yield return MlMinG;
				yield return MlG;
				yield return _1Cm;
				yield return UmolMl;
				yield return PropCnts;
				yield return PropCps;
				yield return MlMinMl;
				yield return MlMl;
				yield return GMl;
				yield return StdDev;
			}
		}

		private readonly string _code;
		private readonly string _description;

		/// <summary>
		/// Initializes a standard defined term.
		/// </summary>
		private Units(string code, string description)
		{
			_code = code;
			_description = description;
		}

		/// <summary>
		/// Initializes a defined term.
		/// </summary>
		/// <param name="code"></param>
		public Units(string code)
		{
			_description = _code = (code ?? string.Empty).ToUpperInvariant();
		}

		/// <summary>
		/// Gets the value of the defined term.
		/// </summary>
		public string Code
		{
			get { return _code ?? string.Empty; }
		}

		/// <summary>
		/// Gets a textual description of the defined term.
		/// </summary>
		public string Description
		{
			get { return _description ?? string.Empty; }
		}

		/// <summary>
		/// Gets a value indicating whether or not this represents the empty value.
		/// </summary>
		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(_code); }
		}

		/// <summary>
		/// Checks whether or not <paramref name="obj"/> represents the same defined term.
		/// </summary>
		public override bool Equals(object obj)
		{
			return obj is Units && Equals((Units) obj);
		}

		/// <summary>
		/// Checks whether or not this defined term is the same as <paramref name="other"/>.
		/// </summary>
		public bool Equals(Units other)
		{
			return Code == other.Code;
		}

		public override int GetHashCode()
		{
			return -0x0A4F34E4 ^ Code.GetHashCode();
		}

		public override string ToString()
		{
			return Code;
		}

		public static bool operator ==(Units x, Units y)
		{
			return Equals(x, y);
		}

		public static bool operator !=(Units x, Units y)
		{
			return !Equals(x, y);
		}

		public static implicit operator string(Units units)
		{
			return units.ToString();
		}

		public static explicit operator Units(string units)
		{
			return new Units(units);
		}
	}
}