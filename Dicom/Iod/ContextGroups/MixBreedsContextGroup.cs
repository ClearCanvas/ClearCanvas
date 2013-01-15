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
	public sealed class MixedBreedsContextGroup : ContextGroupBase<MixedBreeds>
	{
		private MixedBreedsContextGroup() : base(7481, "Breed Registry", true, new DateTime(2006, 8, 22)) { }

		public static readonly MixedBreeds MixedBreedCat = new MixedBreeds("SRT", "L-80A74", "Mixed breed cat");
		public static readonly MixedBreeds MixedBreedGoat = new MixedBreeds("SRT", "L-80217", "Mixed breed goat");
		public static readonly MixedBreeds MixedBreedDog = new MixedBreeds("SRT", "L-809DF", "Mixed breed dog");
		public static readonly MixedBreeds MixedBreedHorse = new MixedBreeds("SRT", "L-8A10F", "Mixed breed horse");
		public static readonly MixedBreeds MixedBreedSheep = new MixedBreeds("SRT", "L-8C33A", "Mixed breed sheep");
		public static readonly MixedBreeds MixedBreedChicken = new MixedBreeds("SRT", "L-93791", "Mixed breed chicken");
		public static readonly MixedBreeds MixedBreedCattle = new MixedBreeds("SRT", "L-8B947", "Mixed breed cattle");
		public static readonly MixedBreeds MixedBreedPig = new MixedBreeds("SRT", "L-8B103", "Mixed breed pig");

		#region Singleton Instancing

		private static readonly MixedBreedsContextGroup _contextGroup = new MixedBreedsContextGroup();

		public static MixedBreedsContextGroup Instance
		{
			get { return _contextGroup; }
		}

		#endregion

		#region Static Enumeration of Values

		public static IEnumerable<MixedBreeds> Values
		{
			get
			{
				yield return MixedBreedCat;
				yield return MixedBreedGoat;
				yield return MixedBreedDog;
				yield return MixedBreedHorse;
				yield return MixedBreedSheep;
				yield return MixedBreedChicken;
				yield return MixedBreedCattle;
				yield return MixedBreedPig;
			}
		}

		/// <summary>
		/// Gets an enumerator that iterates through the defined titles.
		/// </summary>
		/// <returns>A <see cref="IEnumerator{T}"/> object that can be used to iterate through the defined titles.</returns>
		public override IEnumerator<MixedBreeds> GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		public static MixedBreeds LookupTitle(CodeSequenceMacro codeSequence)
		{
			return Instance.Lookup(codeSequence);
		}

		#endregion
	}

	/// <summary>
	/// Represents a mixed breeds
	/// </summary>
	public sealed class MixedBreeds : ContextGroupBase<MixedBreeds>.ContextGroupItemBase
	{
		/// <summary>
		/// Constructor for titles defined in DICOM 2009, Part 16, Annex B, CID 7486.
		/// </summary>
		internal MixedBreeds(string codeValue, string codeMeaning) : base("SRT", codeValue, codeMeaning) { }

		/// <summary>
		/// Constructs a new mixed breeds.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public MixedBreeds(string codingSchemeDesignator, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new mixed breeds.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codingSchemeVersion">The version of the coding scheme in which this code is defined, if known. Should be <code>null</code> if not explicitly specified.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public MixedBreeds(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codingSchemeVersion, codeValue, codeMeaning) {}

		/// <summary>
		/// Converts a <see cref="MixedBreeds"/> to a <see cref="Breed"/>.
		/// </summary>
		/// <param name="mixedBreed"></param>
		/// <returns></returns>
		public static implicit operator Breed(MixedBreeds mixedBreed)
		{
			return new Breed(
				mixedBreed.CodingSchemeDesignator,
				mixedBreed.CodingSchemeVersion,
				mixedBreed.CodeValue,
				mixedBreed.CodeMeaning);
		}
	}
}