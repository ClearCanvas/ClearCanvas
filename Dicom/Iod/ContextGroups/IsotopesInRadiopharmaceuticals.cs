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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.ContextGroups
{
	public sealed class IsotopesInRadiopharmaceuticalsContextGroup : ContextGroupBase<IsotopesInRadiopharmaceuticals>
	{
		private IsotopesInRadiopharmaceuticalsContextGroup() : base(18, "Isotopes in Radiopharmaceuticals", true, new DateTime(2007, 06, 25)) {}

		// ReSharper disable InconsistentNaming
		public static readonly IsotopesInRadiopharmaceuticals Fluorine18 = new IsotopesInRadiopharmaceuticals("C-111A1", "Fluorine", "F", 18, false);
		public static readonly IsotopesInRadiopharmaceuticals Iodine123 = new IsotopesInRadiopharmaceuticals("C-114A4", "Iodine", "I", 123, false);
		public static readonly IsotopesInRadiopharmaceuticals Iodine125 = new IsotopesInRadiopharmaceuticals("C-114A6", "Iodine", "I", 125, false);
		public static readonly IsotopesInRadiopharmaceuticals Iodine131 = new IsotopesInRadiopharmaceuticals("C-114B1", "Iodine", "I", 131, false);
		public static readonly IsotopesInRadiopharmaceuticals Barium133 = new IsotopesInRadiopharmaceuticals("C-122A5", "Barium", "Ba", 133, false);
		public static readonly IsotopesInRadiopharmaceuticals Gallium67 = new IsotopesInRadiopharmaceuticals("C-131A2", "Gallium", "Ga", 67, false);
		public static readonly IsotopesInRadiopharmaceuticals Thallium201 = new IsotopesInRadiopharmaceuticals("C-138A9", "Thallium", "Tl", 201, false);
		public static readonly IsotopesInRadiopharmaceuticals Cobalt57 = new IsotopesInRadiopharmaceuticals("C-144A3", "Cobalt", "Co", 57, false);
		public static readonly IsotopesInRadiopharmaceuticals Indium111 = new IsotopesInRadiopharmaceuticals("C-145A4", "Indium", "In", 111, false);
		public static readonly IsotopesInRadiopharmaceuticals Technetium99m = new IsotopesInRadiopharmaceuticals("C-163A8", "Technetium", "Tc", 99, true);
		public static readonly IsotopesInRadiopharmaceuticals Xenon133 = new IsotopesInRadiopharmaceuticals("C-172A8", "Xenon", "Xe", 133, false);
		public static readonly IsotopesInRadiopharmaceuticals Krypton85 = new IsotopesInRadiopharmaceuticals("C-173A7", "Krypton", "K", 85, false);
		public static readonly IsotopesInRadiopharmaceuticals Gadolinium153 = new IsotopesInRadiopharmaceuticals("C-178A8", "Gadolinium", "Gd", 153, false);
		public static readonly IsotopesInRadiopharmaceuticals Carbon14 = new IsotopesInRadiopharmaceuticals("C-105A2", "Carbon", "C", 14, false);
		public static readonly IsotopesInRadiopharmaceuticals Phosphorus32 = new IsotopesInRadiopharmaceuticals("C-106A1", "Phosphorus", "P", 32, false);
		public static readonly IsotopesInRadiopharmaceuticals Chromium51 = new IsotopesInRadiopharmaceuticals("C-129A2", "Chromium", "Cr", 51, false);
		public static readonly IsotopesInRadiopharmaceuticals Gold198 = new IsotopesInRadiopharmaceuticals("C-146A9", "Gold", "Au", 198, false);
		public static readonly IsotopesInRadiopharmaceuticals Copper64 = new IsotopesInRadiopharmaceuticals("C-127A2", "Copper", "Cu", 64, false);
		public static readonly IsotopesInRadiopharmaceuticals Copper67 = new IsotopesInRadiopharmaceuticals("C-127A3", "Copper", "Cu", 67, false);
		public static readonly IsotopesInRadiopharmaceuticals Cobalt58 = new IsotopesInRadiopharmaceuticals("C-144A4", "Cobalt", "Co", 58, false);
		public static readonly IsotopesInRadiopharmaceuticals Cobalt60 = new IsotopesInRadiopharmaceuticals("C-144A6", "Cobalt", "Co", 60, false);
		public static readonly IsotopesInRadiopharmaceuticals Iron59 = new IsotopesInRadiopharmaceuticals("C-130A3", "Iron", "Fe", 59, false);
		public static readonly IsotopesInRadiopharmaceuticals Indium133m = new IsotopesInRadiopharmaceuticals("C-145A5", "Indium", "In", 133, true);
		public static readonly IsotopesInRadiopharmaceuticals Ytterbium169 = new IsotopesInRadiopharmaceuticals("C-181A3", "Ytterbium", "Y", 169, false);
		public static readonly IsotopesInRadiopharmaceuticals Potassium42 = new IsotopesInRadiopharmaceuticals("C-135A2", "Potassium", "K", 42, false);
		public static readonly IsotopesInRadiopharmaceuticals Potassium43 = new IsotopesInRadiopharmaceuticals("C-135A3", "Potassium", "K", 43, false);
		public static readonly IsotopesInRadiopharmaceuticals Rhenium186 = new IsotopesInRadiopharmaceuticals("C-11906", "Rhenium", "Re", 186, false);
		public static readonly IsotopesInRadiopharmaceuticals Rhenium188 = new IsotopesInRadiopharmaceuticals("C-1018D", "Rhenium", "Re", 188, false);
		public static readonly IsotopesInRadiopharmaceuticals Samarium153 = new IsotopesInRadiopharmaceuticals("C-B1134", "Samarium", "Sm", 153, false);
		public static readonly IsotopesInRadiopharmaceuticals Selenium75 = new IsotopesInRadiopharmaceuticals("C-116A3", "Selenium", "Se", 75, false);
		public static readonly IsotopesInRadiopharmaceuticals Sodium22 = new IsotopesInRadiopharmaceuticals("C-155A1", "Sodium", "Na", 22, false);
		public static readonly IsotopesInRadiopharmaceuticals Sodium24 = new IsotopesInRadiopharmaceuticals("C-155A2", "Sodium", "Na", 24, false);
		public static readonly IsotopesInRadiopharmaceuticals Strontium85 = new IsotopesInRadiopharmaceuticals("C-158A3", "Strontium", "Sr", 85, false);
		public static readonly IsotopesInRadiopharmaceuticals Strontium87m = new IsotopesInRadiopharmaceuticals("C-158A5", "Strontium", "Sr", 87, true);
		public static readonly IsotopesInRadiopharmaceuticals Strontium89 = new IsotopesInRadiopharmaceuticals("C-158A6", "Strontium", "Sr", 89, false);
		// ReSharper restore InconsistentNaming

		public static IEnumerable<IsotopesInRadiopharmaceuticals> Values
		{
			get
			{
				yield return Fluorine18;
				yield return Iodine123;
				yield return Iodine125;
				yield return Iodine131;
				yield return Barium133;
				yield return Gallium67;
				yield return Thallium201;
				yield return Cobalt57;
				yield return Indium111;
				yield return Technetium99m;
				yield return Xenon133;
				yield return Krypton85;
				yield return Gadolinium153;
				yield return Carbon14;
				yield return Phosphorus32;
				yield return Chromium51;
				yield return Gold198;
				yield return Copper64;
				yield return Copper67;
				yield return Cobalt58;
				yield return Cobalt60;
				yield return Iron59;
				yield return Indium133m;
				yield return Ytterbium169;
				yield return Potassium42;
				yield return Potassium43;
				yield return Rhenium186;
				yield return Rhenium188;
				yield return Samarium153;
				yield return Selenium75;
				yield return Sodium22;
				yield return Sodium24;
				yield return Strontium85;
				yield return Strontium87m;
				yield return Strontium89;
			}
		}

		public override IEnumerator<IsotopesInRadiopharmaceuticals> GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		protected override IsotopesInRadiopharmaceuticals CreateContextGroupItem(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
		{
			return new IsotopesInRadiopharmaceuticals(codingSchemeDesignator, codingSchemeVersion, codeValue, codeMeaning);
		}

		public static IsotopesInRadiopharmaceuticals LookupIsotope(CodeSequenceMacro codeSequence)
		{
			return Instance.Lookup(codeSequence);
		}

		#region Singleton Instancing

		private static readonly IsotopesInRadiopharmaceuticalsContextGroup _contextGroup = new IsotopesInRadiopharmaceuticalsContextGroup();

		public static IsotopesInRadiopharmaceuticalsContextGroup Instance
		{
			get { return _contextGroup; }
		}

		#endregion
	}

	/// <summary>
	/// A code sequence specifying the isotopes in radiopharmaceuticals.
	/// </summary>
	public sealed class IsotopesInRadiopharmaceuticals : ContextGroupBase<IsotopesInRadiopharmaceuticals>.ContextGroupItemBase
	{
		public readonly string ElementName = null;
		public readonly string ElementSymbol = null;
		public readonly string Isotope = null;
		public readonly string IsotopeName = null;
		public readonly string IsotopeSymbol = null;

		/// <summary>
		/// Constructor for isotopes defined in DICOM 2008, Part 16, Annex B, CID 18.
		/// </summary>
		internal IsotopesInRadiopharmaceuticals(string value, string elementName, string elementSymbol, int nucleonCount, bool metastable)
			: base("SRT", value, BuildCodeMeaning(elementName, nucleonCount, metastable))
		{
			Platform.CheckPositive(nucleonCount, "nucleonCount");

			ElementName = elementName;
			ElementSymbol = elementSymbol;
			Isotope = string.Format("{0}{1}", nucleonCount, metastable ? "m" : string.Empty);
			IsotopeName = string.Format("{0}-{1}", elementName, Isotope);
			IsotopeSymbol = string.Format("{0}-{1}", elementSymbol, Isotope);
		}

		/// <summary>
		/// Constructs a new isotope.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public IsotopesInRadiopharmaceuticals(string codingSchemeDesignator, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new isotope.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codingSchemeVersion">The version of the coding scheme in which this code is defined, if known. Should be <code>null</code> if not explicitly specified.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public IsotopesInRadiopharmaceuticals(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codingSchemeVersion, codeValue, codeMeaning) {}

		public override int GetHashCode()
		{
			string modifiedCodingSchemeDesignator = this.CodingSchemeDesignator;
			if (modifiedCodingSchemeDesignator.Equals("99SDM", StringComparison.InvariantCultureIgnoreCase))
				modifiedCodingSchemeDesignator = "SRT";
			return -0x06EA5B76 ^ modifiedCodingSchemeDesignator.GetHashCode() ^ this.CodeValue.GetHashCode();
		}

		public override bool Equals(string codingSchemeDesignator, string codeValue, string codeMeaning, string codingSchemeVersion, bool compareCodingSchemeVersion)
		{
			string modifiedCodingSchemeDesignator = codingSchemeDesignator;
			if (modifiedCodingSchemeDesignator.Equals("99SDM", StringComparison.InvariantCultureIgnoreCase))
				modifiedCodingSchemeDesignator = "SRT";
			StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
			bool result = comparer.Equals(this.CodeValue, codeValue);
			result = result && comparer.Equals(this.CodingSchemeDesignator, modifiedCodingSchemeDesignator);
			if (compareCodingSchemeVersion)
				result = result && comparer.Equals(this.CodingSchemeVersion, codingSchemeVersion);
			return result;
		}

		public override string ToPlainString()
		{
			return this.ToPlainString(false);
		}

		public string ToPlainString(bool useElementSymbols)
		{
			if (useElementSymbols && !string.IsNullOrEmpty(this.IsotopeSymbol))
				return this.IsotopeSymbol;
			if (!useElementSymbols && !string.IsNullOrEmpty(this.IsotopeName))
				return this.IsotopeName;
			return base.ToPlainString();
		}

		private static string BuildCodeMeaning(string elementName, int nucleonCount, bool metastable)
		{
			return string.Format("^{1}{2}^{0}", elementName, nucleonCount, metastable ? "m" : string.Empty);
		}
	}
}