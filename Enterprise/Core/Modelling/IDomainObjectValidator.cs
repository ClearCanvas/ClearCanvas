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
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	public interface IDomainObjectValidator
	{
		/// <summary>
		/// Validates the specified domain object, applying all known validation rules.
		/// </summary>
		/// <param name="obj"></param>
		/// <exception cref="EntityValidationException">Validation failed.</exception>
		void Validate(object obj);

		/// <summary>
		/// Validates only that the specified object has required fields set.
		/// </summary>
		/// <param name="obj"></param>
		void ValidateRequiredFieldsPresent(object obj);

		/// <summary>
		/// Validates the specified domain object, applying only "low-level" rules, subject to the specified filter.
		/// </summary>
		/// <remarks>
		/// Low-level rules are:
		/// 1. Required fields.
		/// 2. String field lengths.
		/// 3. Unique constraints.
		/// </remarks>
		/// <param name="obj"></param>
		/// <param name="ruleFilter"></param>
		void ValidateLowLevel(object obj, Predicate<ISpecification> ruleFilter);

		/// <summary>
		/// Validates the specified domain object, applying only high-level rules.
		/// </summary>
		/// <remarks>
		/// High-level rules include any rules that are not low-level rules.
		/// </remarks>
		/// <param name="obj"></param>
		void ValidateHighLevel(object obj);
	}

}
