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
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.Automation;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	internal static class Utilities
	{
		public static bool IsOpenQuery(this StudyRootStudyIdentifier criteria)
		{
			var attributes = criteria.ToDicomAttributeCollection();

			//Clean out the base identifier attributes, which could have non-empty values.
			attributes[DicomTags.SpecificCharacterSet].SetEmptyValue();
			attributes[DicomTags.RetrieveAeTitle].SetEmptyValue();
			attributes[DicomTags.InstanceAvailability].SetEmptyValue();
			attributes[DicomTags.QueryRetrieveLevel].SetEmptyValue();

			return !attributes.Any(a => !a.IsNull && !a.IsEmpty);
		}

		public static StudyRootStudyIdentifier ToIdentifier(this DicomExplorerSearchCriteria explorerSearchCriteria, bool addWildcards)
		{
			if (addWildcards)
			{
				return new StudyRootStudyIdentifier
				       {
					       PatientsName = ConvertNameToSearchCriteria(explorerSearchCriteria.PatientsName),
					       ReferringPhysiciansName = ConvertNameToSearchCriteria(explorerSearchCriteria.ReferringPhysiciansName),
					       PatientId = ConvertStringToWildcardSearchCriteria(explorerSearchCriteria.PatientId, false, true),
					       AccessionNumber = ConvertStringToWildcardSearchCriteria(explorerSearchCriteria.AccessionNumber, false, true),
					       StudyDescription = ConvertStringToWildcardSearchCriteria(explorerSearchCriteria.StudyDescription, false, true),
					       StudyDate = DateRangeHelper.GetDicomDateRangeQueryString(explorerSearchCriteria.StudyDateFrom, explorerSearchCriteria.StudyDateTo),
					       //At the application level, ClearCanvas defines the 'ModalitiesInStudy' filter as a multi-valued
					       //Key Attribute.  This goes against the Dicom standard for C-FIND SCU behaviour, so the
					       //underlying IStudyFinder(s) must handle this special case, either by ignoring the filter
					       //or by running multiple queries, one per modality specified (for example).
					       ModalitiesInStudy = explorerSearchCriteria.Modalities.ToArray()
				       };
			}

			return new StudyRootStudyIdentifier
			       {
				       PatientsName = explorerSearchCriteria.PatientsName,
				       ReferringPhysiciansName = explorerSearchCriteria.ReferringPhysiciansName,
				       PatientId = explorerSearchCriteria.PatientId,
				       AccessionNumber = explorerSearchCriteria.AccessionNumber,
				       StudyDescription = explorerSearchCriteria.StudyDescription,
				       StudyDate = DateRangeHelper.GetDicomDateRangeQueryString(explorerSearchCriteria.StudyDateFrom, explorerSearchCriteria.StudyDateTo),
				       //At the application level, ClearCanvas defines the 'ModalitiesInStudy' filter as a multi-valued
				       //Key Attribute.  This goes against the Dicom standard for C-FIND SCU behaviour, so the
				       //underlying IStudyFinder(s) must handle this special case, either by ignoring the filter
				       //or by running multiple queries, one per modality specified (for example).
				       ModalitiesInStudy = explorerSearchCriteria.Modalities.ToArray()
			       };
		}

		/// <summary>
		/// Converts the query string into a DICOM search criteria.
		/// Appended with a wildcard character.
		/// </summary>
		public static string ConvertStringToWildcardSearchCriteria(string userQueryString, bool leadingWildcard, bool trailingWildcard)
		{
			var dicomSearchCriteria = "";
			if (String.IsNullOrEmpty(userQueryString))
				return dicomSearchCriteria;

			dicomSearchCriteria = userQueryString;
			if (leadingWildcard)
				dicomSearchCriteria = "*" + dicomSearchCriteria;

			if (trailingWildcard)
				dicomSearchCriteria = dicomSearchCriteria + "*";

			return dicomSearchCriteria;
		}

		/// <summary>
		/// Converts the query string for name into a DICOM search string.
		/// </summary>
		public static string ConvertNameToSearchCriteria(string name)
		{
			var nameComponents = GetNameComponents(name);

			if (nameComponents.Length == 0)
				return "";

			var wildcardPrefix = DicomExplorerConfigurationSettings.Default.AutoLeadingWildcard ? "*" : string.Empty;

			//Open name search
			if (nameComponents.Length == 1)
				return String.Format(wildcardPrefix + "{0}*", nameComponents[0].Trim());

			//Open name search - should never get here
			if (String.IsNullOrEmpty(nameComponents[0]))
				return String.Format(wildcardPrefix + "{0}*", nameComponents[1].Trim());

			//Pure Last Name search
			if (String.IsNullOrEmpty(nameComponents[1]))
				return String.Format("{0}*", nameComponents[0].Trim());

			//Last Name, First Name search
			return String.Format("{0}*{1}*", nameComponents[0].Trim(), nameComponents[1].Trim());
		}

		private static string[] GetNameComponents(string unparsedName)
		{
			unparsedName = unparsedName ?? "";
			var separator = DicomExplorerConfigurationSettings.Default.NameSeparator;
			var name = unparsedName.Trim();
			if (String.IsNullOrEmpty(name))
				return new string[0];

			return name.Split(new[] {separator}, StringSplitOptions.None);
		}

		/// <summary>
		/// Returns distinct elements from a sequence based on comparing a key projected by the specified function.
		/// </summary>
		/// <typeparam name="TElement"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="source"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static IEnumerable<TElement> Distinct<TElement, TKey>(this IEnumerable<TElement> source, Func<TElement, TKey> key)
		{
			return source.Distinct(new DistinctKeyComparer<TElement, TKey>(key));
		}

		/// <summary>
		/// Produces the set union of two sequences based on comparing a key projected by the specified function.
		/// </summary>
		/// <typeparam name="TElement"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static IEnumerable<TElement> Union<TElement, TKey>(this IEnumerable<TElement> first, IEnumerable<TElement> second, Func<TElement, TKey> key)
		{
			return first.Union(second, new DistinctKeyComparer<TElement, TKey>(key));
		}

		private class DistinctKeyComparer<TElement, TKey> : IEqualityComparer<TElement>
		{
			private readonly Func<TElement, TKey> _key;

			public DistinctKeyComparer(Func<TElement, TKey> key)
			{
				_key = key;
			}

			public bool Equals(TElement x, TElement y)
			{
				return Equals(_key.Invoke(x), _key.Invoke(y));
			}

			public int GetHashCode(TElement x)
			{
				return _key.Invoke(x).GetHashCode();
			}
		}
	}
}