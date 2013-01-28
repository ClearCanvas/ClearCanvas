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
	/// Defined terms for the <see cref="ClearCanvas.Dicom.DicomTags.DecayCorrection"/> attribute 
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.9.1 (Table C.8-60)</remarks>
	public struct DecayCorrection
	{
		/// <summary>
		/// Represents the empty value.
		/// </summary>
		public static readonly DecayCorrection Empty = new DecayCorrection();

		public static readonly DecayCorrection None = new DecayCorrection("NONE", "no decay correction");
		public static readonly DecayCorrection Start = new DecayCorrection("START", "acquisition start time");
		public static readonly DecayCorrection Admin = new DecayCorrection("ADMIN", "radiopharmaceutical administration time");

		/// <summary>
		/// Enumeration of defined terms.
		/// </summary>
		public static IEnumerable<DecayCorrection> DefinedTerms
		{
			get
			{
				yield return None;
				yield return Start;
				yield return Admin;
			}
		}

		private readonly string _code;
		private readonly string _description;

		/// <summary>
		/// Initializes a standard defined term.
		/// </summary>
		private DecayCorrection(string code, string description)
		{
			_code = code;
			_description = description;
		}

		/// <summary>
		/// Initializes a defined term.
		/// </summary>
		/// <param name="code"></param>
		public DecayCorrection(string code)
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
			return obj is DecayCorrection && Equals((DecayCorrection) obj);
		}

		/// <summary>
		/// Checks whether or not this defined term is the same as <paramref name="other"/>.
		/// </summary>
		public bool Equals(DecayCorrection other)
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

		public static bool operator ==(DecayCorrection x, DecayCorrection y)
		{
			return Equals(x, y);
		}

		public static bool operator !=(DecayCorrection x, DecayCorrection y)
		{
			return !Equals(x, y);
		}

		public static implicit operator string(DecayCorrection decayCorrection)
		{
			return decayCorrection.ToString();
		}

		public static explicit operator DecayCorrection(string decayCorrection)
		{
			return new DecayCorrection(decayCorrection);
		}
	}
}