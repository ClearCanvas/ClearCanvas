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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.ContextGroups
{
	public sealed class BreedRegistryContextGroup : ContextGroupBase<BreedRegistry>
	{
		private BreedRegistryContextGroup() : base(7481, "Breed Registry", true, new DateTime(2006, 8, 22)) { }

		public static readonly BreedRegistry AmericaKennelClub = new BreedRegistry("DCM", "109200", "America Kennel Club");
		public static readonly BreedRegistry AmericasPetRegistryInc = new BreedRegistry("DCM", "109201", "America's Pet Registry Inc.");
		public static readonly BreedRegistry AmericanCanineAssociation = new BreedRegistry("DCM", "109202", "American Canine Association");
		public static readonly BreedRegistry AmericanPurebredRegistry = new BreedRegistry("DCM", "109203", "American Purebred Registry");
		public static readonly BreedRegistry AmericanRareBreedAssociation = new BreedRegistry("DCM", "109204", "American Rare Breed Association");
		public static readonly BreedRegistry AnimalRegistryUnlimited = new BreedRegistry("DCM", "109205", "Animal Registry Unlimited");
		public static readonly BreedRegistry AnimalResearchFoundation = new BreedRegistry("DCM", "109206", "Animal Research Foundation");
		public static readonly BreedRegistry CanadianBorderCollieAssociation = new BreedRegistry("DCM", "109207", "Canadian Border Collie Association");
		public static readonly BreedRegistry CanadianKennelClub = new BreedRegistry("DCM", "109208", "Canadian Kennel Club");
		public static readonly BreedRegistry CanadianLivestockRecordsAssociation = new BreedRegistry("DCM", "109209", "Canadian Livestock Records Association");
		public static readonly BreedRegistry CanineFederationOfCanada = new BreedRegistry("DCM", "109210", "Canine Federation of Canada");
		public static readonly BreedRegistry ContinentalKennelClub = new BreedRegistry("DCM", "109211", "Continental Kennel Club");
		public static readonly BreedRegistry DogRegistryOfAmerica = new BreedRegistry("DCM", "109212", "Dog Registry of America");
		public static readonly BreedRegistry FederationOfInternationalCanines = new BreedRegistry("DCM", "109213", "Federation of International Canines");
		public static readonly BreedRegistry InternationalProgressiveDogBreedersAlliance = new BreedRegistry("DCM", "109214", "International Progressive Dog Breeders' Alliance");
		public static readonly BreedRegistry NationalKennelClub = new BreedRegistry("DCM", "109215", "National Kennel Club");
		public static readonly BreedRegistry NorthAmericanPurebredDogRegistry = new BreedRegistry("DCM", "109216", "North American Purebred Dog Registry");
		public static readonly BreedRegistry UnitedAllBreedRegistry = new BreedRegistry("DCM", "109217", "United All Breed Registry");
		public static readonly BreedRegistry UnitedKennelClub = new BreedRegistry("DCM", "109218", "United Kennel Club");
		public static readonly BreedRegistry UniversalKennelClubInternational = new BreedRegistry("DCM", "109219", "Universal Kennel Club International");
		public static readonly BreedRegistry WorkingCanineAssociationOfCanada = new BreedRegistry("DCM", "109220", "Working Canine Association of Canada");
		public static readonly BreedRegistry WorldKennelClub = new BreedRegistry("DCM", "109221", "World Kennel Club");
		public static readonly BreedRegistry WorldWideKennelClub = new BreedRegistry("DCM", "109222", "World Wide Kennel Club");

		#region Singleton Instancing

		private static readonly BreedRegistryContextGroup _contextGroup = new BreedRegistryContextGroup();

		public static BreedRegistryContextGroup Instance
		{
			get { return _contextGroup; }
		}

		#endregion

		#region Static Enumeration of Values

		public static IEnumerable<BreedRegistry> Values
		{
			get
			{
				yield return AmericaKennelClub;
				yield return AmericasPetRegistryInc;
				yield return AmericanCanineAssociation;
				yield return AmericanPurebredRegistry;
				yield return AmericanRareBreedAssociation;
				yield return AnimalRegistryUnlimited;
				yield return AnimalResearchFoundation;
				yield return CanadianBorderCollieAssociation;
				yield return CanadianKennelClub;
				yield return CanadianLivestockRecordsAssociation;
				yield return CanineFederationOfCanada;
				yield return ContinentalKennelClub;
				yield return DogRegistryOfAmerica;
				yield return FederationOfInternationalCanines;
				yield return InternationalProgressiveDogBreedersAlliance;
				yield return NationalKennelClub;
				yield return NorthAmericanPurebredDogRegistry;
				yield return UnitedAllBreedRegistry;
				yield return UnitedKennelClub;
				yield return UniversalKennelClubInternational;
				yield return WorkingCanineAssociationOfCanada;
				yield return WorldKennelClub;
				yield return WorldWideKennelClub;
			}
		}

		/// <summary>
		/// Gets an enumerator that iterates through the defined titles.
		/// </summary>
		/// <returns>A <see cref="IEnumerator{T}"/> object that can be used to iterate through the defined titles.</returns>
		public override IEnumerator<BreedRegistry> GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		public static BreedRegistry LookupTitle(CodeSequenceMacro codeSequence)
		{
			return Instance.Lookup(codeSequence);
		}

		#endregion
	}

	/// <summary>
	/// Represents a species
	/// </summary>
	public sealed class BreedRegistry : ContextGroupBase<BreedRegistry>.ContextGroupItemBase
	{
		/// <summary>
		/// Constructor for titles defined in DICOM 2009, Part 16, Annex B, CID 7481.
		/// </summary>
		internal BreedRegistry(string codeValue, string codeMeaning) : base("DCM", codeValue, codeMeaning) { }

		/// <summary>
		/// Constructs a new breed registry.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public BreedRegistry(string codingSchemeDesignator, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new breed registry.
		/// </summary>
		/// <param name="codingSchemeDesignator">The designator of the coding scheme in which this code is defined.</param>
		/// <param name="codingSchemeVersion">The version of the coding scheme in which this code is defined, if known. Should be <code>null</code> if not explicitly specified.</param>
		/// <param name="codeValue">The value of this code.</param>
		/// <param name="codeMeaning">The Human-readable meaning of this code.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codingSchemeDesignator"/> or <paramref name="codeValue"/> are <code>null</code> or empty.</exception>
		public BreedRegistry(string codingSchemeDesignator, string codingSchemeVersion, string codeValue, string codeMeaning)
			: base(codingSchemeDesignator, codingSchemeVersion, codeValue, codeMeaning) {}

		/// <summary>
		/// Constructs a new breed registry.
		/// </summary>
		/// <param name="codeSequence">The code sequence attributes macro.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="codeSequence.CodingSchemeDesignator"/> or <paramref name="codeSequence.CodeValue"/> are <code>null</code> or empty.</exception>
		public BreedRegistry(CodeSequenceMacro codeSequence)
			: base(codeSequence.CodingSchemeDesignator, codeSequence.CodingSchemeVersion, codeSequence.CodeValue, codeSequence.CodeMeaning) { }
	}
}