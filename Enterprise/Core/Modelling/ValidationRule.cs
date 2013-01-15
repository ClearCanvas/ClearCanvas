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
	/// <summary>
	/// Encapsulates a validation rule expressed as a function.
	/// </summary>
	/// <typeparam name="T">The class to which the rule applies.</typeparam>
	public class ValidationRule<T> : ISpecification
	{
		private readonly Converter<T, TestResult> _func;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="func">A function that expresses the validation rule.</param>
		public ValidationRule(Converter<T, TestResult> func)
		{
			_func = func;
		}

		/// <summary>
		/// Tests this rule against the specified instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public TestResult Test(T obj)
		{
			return _func(obj);
		}

		TestResult ISpecification.Test(object obj)
		{
			return Test((T)obj);
		}
	}
}
