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

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Defines an interface to a class that acts as a source of validation rule sets.
	/// </summary>
	public interface IValidationRuleSetSource
	{
		/// <summary>
		/// Gets a value indicating whether the rules provided by this source are static - that is,
		/// they cannot change during the lifetime of the process.
		/// </summary>
		/// <remarks>
		/// If this flag returns true, calls to <see cref="GetRuleSet"/> will cache the return value
		/// for the life of the process, which may provide a slight optimization.  If false, the
		/// return value may still be cached, but only for a relatively short period of time.
		/// </remarks>
		bool IsStatic { get; }

		/// <summary>
		/// Gets the validation rule set for the specified class.
		/// </summary>
		/// <param name="domainClass"> </param>
		/// <returns></returns>
		ValidationRuleSet GetRuleSet(Type domainClass);
	}
}
