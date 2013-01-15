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
	/// Applied to an entity class to specify how validation of that entity should be handled.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ValidationAttribute : Attribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ValidationAttribute()
		{
			this.EnableValidation = true;
		}

		/// <summary>
		/// Gets or sets a value specifying whether validation of this entity is enabled.
		/// </summary>
		/// <remarks>
		/// If set to false, all validation rules defined for this entity, whether defined in code
		/// or in XML, will be ignored.
		/// </remarks>
		public bool EnableValidation { get; set; }

		/// <summary>
		/// Gets the name of the method that supplies high-level validation rules.
		/// </summary>
		/// <remarks>
		/// The method must be static, and have the signature
		/// <code>
		/// IValidationRuleSet MyMethod()
		/// </code>
		/// The rule-set returned by this method will be combined with any rules declared by attributes
		/// on the class or its properties.
		/// </remarks>
		public string HighLevelRulesProviderMethod { get; set; }
	}
}
