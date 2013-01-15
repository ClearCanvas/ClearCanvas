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
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.ContextGroups
{
	public sealed class SpeciesContextGroup : ContextGroupBase<Species>
	{
		private SpeciesContextGroup() : base(7454, "Species", true, new DateTime(2006, 8, 22)) { }

		public static readonly Species HomoSapiens = new Species("SRT", "L-85B00", "homo sapiens");
		public static readonly Species FelineSpecies = new Species("SRT", "L-80A00", "Feline species");
		public static readonly Species EquineSpecies = new Species("SRT", "L-80400", "Equine species");
		public static readonly Species OvineSpecies = new Species("SRT", "L-80300", "Ovine species");
		public static readonly Species PorcineSpecies = new Species("SRT", "L-80500", "Porcine species");
		public static readonly Species CaprineSpecies = new Species("SRT", "L-80200", "Caprine species");
		public static readonly Species CanineSpecies = new Species("SRT", "L-80700", "Canine species");
		public static readonly Species BovineSpecies = new Species("SRT", "L-80100", "Bovine species");

		#region Singleton Instancing

		private static readonly SpeciesContextGroup _contextGroup = new SpeciesContextGroup();

		public static SpeciesContextGroup Instance
		{
			get { return _contextGroup; }
		}

		#endregion

		#region Static Enumeration of Values

		public static IEnumerable<Species> Values
		{
			get
			{
				yield return HomoSapiens;
				yield return FelineSpecies;
				yield return EquineSpecies;
				yield return OvineSpecies;
				yield return PorcineSpecies;
				yield return CaprineSpecies;
				yield return CanineSpecies;
				yield return BovineSpecies;
			}
		}

		/// <summary>
		/// Gets an enumerator that iterates through the defined titles.
		/// </summary>
		/// <returns>A <see cref="IEnumerator{T}"/> object that can be used to iterate through the defined titles.</returns>
		public override IEnumerator<Species> GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		public static Species LookupTitle(CodeSequenceMacro codeSequence)
		{
			return Instance.Lookup(codeSequence);
		}

		#endregion
	}

	/// <summary>
	/// Represents a species
	/// </summary>
	public sealed class Species : ContextGroupBase<Species>.ContextGroupItemBase
	{
		/// <summary>
		/// Constructor for titles defined in DICOM 2009, Part 16, Annex B, CID 7454.
		/// </summary>
		internal Species(string codeValue, string codeMeaning) : base("SRT", codeValue, codeMeaning) { }

		/// <summary>
		/// Constructs a new species.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public Species(string codingSchemeDesignator, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new species.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codingSchemeVersion">The version of the coding scheme in which this code is defined, if known. Should be <code>null</code> if not explicitly specified.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public Species(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codingSchemeVersion, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new species.
		/// </summary>
		/// <param name="codeSequence">The code sequence attributes macro.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codeSequence.CodingSchemeDesignator"/> or <paramref name="codeSequence.CodeValue"/> are <code>null</code> or empty.</exception>
		public Species(CodeSequenceMacro codeSequence)
			: base(codeSequence.CodingSchemeDesignator, codeSequence.CodingSchemeVersion, codeSequence.CodeValue, codeSequence.CodeMeaning) { }
	}
}