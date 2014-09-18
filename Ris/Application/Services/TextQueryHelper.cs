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
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class TextQueryHelper
	{
		public static PersonName[] ParsePersonNames(string query)
		{
			// define a person name as anything starting with a letter, and followed by letters or general punctuation
			// (apostrophe and hyphens in particular, but without prejudice to any other punctuation which may be used in native names of other languages)
			// examples:
			// * O'Leary
			// * Wyatt-Jones
			// * Pérez
			const string letter = @"\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{M}";
			var termDefinition = new Regex(@"\b[" + letter + @"][" + letter + @"\p{P}]*\b");
			var terms = ParseTerms(query, termDefinition);

			var names = new List<PersonName>();
			for (var i = 0; i < terms.Length; i++)
			{
				var name = new PersonName {FamilyName = string.Join(" ", terms, 0, i + 1)};
				if (i < terms.Length - 1)
				{
					name.GivenName = string.Join(" ", terms, i + 1, terms.Length - i - 1);
				}
				names.Add(name);
			}
			return names.ToArray();
		}

		public static string[] ParseIdentifiers(string query)
		{
			// define an identifier as anything containing at least 1 digit, and any other alpha-digit chars
			var termDefinition = new Regex(@"\b[A-Za-z\d]*\d[A-Za-z\d]*\b");
			return ParseTerms(query, termDefinition);
		}

		public static string[] ParsePhoneNumbers(string query)
		{
			// define a phone number as anything containing only one or more digits
			var termDefinition = new Regex(@"\b\d+\b");

			var queryWithTelephoneFormattersRemoved = query.Replace("(", "").Replace(")", "").Replace("-", "");

			return ParseTerms(queryWithTelephoneFormattersRemoved, termDefinition);
		}

		public static string[] ParseTerms(string query)
		{
			var termDefinition = new Regex(@"\w+");
			return ParseTerms(query, termDefinition);
		}

		public static string[] ParseTerms(string query, Regex termDefinition)
		{
			var matches = termDefinition.Matches(query);
			return CollectionUtils.Map(matches, (Match m) => m.Value).ToArray();
		}
	}

	public class TextQueryHelper<TDomainItem, TSearchCriteria, TSummary> : TextQueryHelper
		where TSearchCriteria : SearchCriteria
		where TSummary : DataContractBase
	{
		public delegate bool TestCriteriaSpecificityDelegate(TSearchCriteria[] where, int threshold);

		public delegate IList<TDomainItem> DoQueryDelegate(TSearchCriteria[] where, SearchResultPage page);

		private readonly Converter<TextQueryRequest, TSearchCriteria[]> _buildCriteriaCallback;
		private readonly Converter<TDomainItem, TSummary> _summaryAssembler;
		private readonly DoQueryDelegate _queryCallback;
		private readonly TestCriteriaSpecificityDelegate _specificityCallback;

		/// <summary>
		/// Protected constructor for subclasses.
		/// </summary>
		protected TextQueryHelper() {}

		/// <summary>
		/// Public constructor allows direct use of this class without the need to create a subclass.
		/// </summary>
		/// <param name="criteriaBuilder"></param>
		/// <param name="summaryAssembler"></param>
		/// <param name="countCallback"></param>
		/// <param name="queryCallback"></param>
		public TextQueryHelper(
			Converter<TextQueryRequest, TSearchCriteria[]> criteriaBuilder,
			Converter<TDomainItem, TSummary> summaryAssembler,
			TestCriteriaSpecificityDelegate countCallback,
			DoQueryDelegate queryCallback)
		{
			_buildCriteriaCallback = criteriaBuilder;
			_summaryAssembler = summaryAssembler;
			_specificityCallback = countCallback;
			_queryCallback = queryCallback;
		}

		public TextQueryResponse<TSummary> Query(TextQueryRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckArgumentRange(request.SpecificityThreshold, 0, 1000, "SpecificityThreshold");

			if (!ValidateRequest(request))
				return new TextQueryResponse<TSummary>(true, new List<TSummary>());

			var where = BuildCriteria(request);

			// augment criteria to exclude de-activated items if specified
			if (!request.IncludeDeactivated && AttributeUtils.HasAttribute<DeactivationFlagAttribute>(typeof (TDomainItem)))
			{
				var propertyName = AttributeUtils.GetAttribute<DeactivationFlagAttribute>(typeof (TDomainItem)).PropertyName;
				var c = new SearchCondition<bool>(propertyName);
				c.EqualTo(false);
				CollectionUtils.ForEach(where, w => w.SetSubCriteria(c));
			}

			// if a specificity threshold was specified, apply it now
			if (request.SpecificityThreshold > 0)
			{
				// eliminate query that would return too many results
				if (!TestSpecificity(where, request.SpecificityThreshold))
					return new TextQueryResponse<TSummary>(true, new List<TSummary>());
			}

			// execute query
			var matches = DoQuery(where, request.Page);

			return new TextQueryResponse<TSummary>(false,
			                                       CollectionUtils.Map(matches, (TDomainItem entity) => AssembleSummary(entity)));
		}

		protected virtual bool ValidateRequest(TextQueryRequest request)
		{
			// default validation - just ensure the text is not empty
			return request.TextQuery != null && request.TextQuery.Trim().Length > 0;
		}

		protected virtual TSearchCriteria[] BuildCriteria(TextQueryRequest request)
		{
			if (_buildCriteriaCallback == null)
				throw new NotImplementedException("Method must be overridden or a delegate supplied.");
			return _buildCriteriaCallback(request);
		}

		protected virtual bool TestSpecificity(TSearchCriteria[] where, int threshold)
		{
			if (_specificityCallback == null)
				throw new NotImplementedException("Method must be overridden or a delegate supplied.");
			return _specificityCallback(where, threshold);
		}

		protected virtual IList<TDomainItem> DoQuery(TSearchCriteria[] where, SearchResultPage page)
		{
			if (_queryCallback == null)
				throw new NotImplementedException("Method must be overridden or a delegate supplied.");
			return _queryCallback(where, page);
		}

		protected virtual TSummary AssembleSummary(TDomainItem domainItem)
		{
			if (_summaryAssembler == null)
				throw new NotImplementedException("Method must be overridden or a delegate supplied.");
			return _summaryAssembler(domainItem);
		}

		/// <summary>
		/// Applies specified string value to specified condition, if the value is non-empty, using partial matching.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="value"></param>
		protected static void ApplyStringCriteria(ISearchCondition<string> condition, string value)
		{
			ApplyStringCriteria(condition, value, false);
		}

		/// <summary>
		/// Applies specified string value to specified condition, if the value is non-empty.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="value"></param>
		/// <param name="exactMatch"></param>
		protected static void ApplyStringCriteria(ISearchCondition<string> condition, string value, bool exactMatch)
		{
			if (value == null)
				return;

			value = value.Trim();
			if (string.IsNullOrEmpty(value))
				return;

			if (exactMatch)
				condition.EqualTo(value);
			else
				condition.StartsWith(value);
		}
	}
}