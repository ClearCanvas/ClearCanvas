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
	/// Specifies that one or more properties of an entity form a unique key for that entity.
	/// </summary>
	/// <remarks>
	/// Internally, this class makes use of a <see cref="IUniqueConstraintValidationBroker"/> to validate
	/// that the key is unique within the space of entities of a given class.
	/// </remarks>
	internal class UniqueKeySpecification : ISpecification
	{
		private readonly Type _entityClass;
		private readonly string[] _uniqueKeyMembers;
		private readonly string _logicalKeyName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityClass">Class on which the unique key constraint is defined.</param>
		/// <param name="logicalKeyName">The logical name of the key.  This value is used in reporting failures,
		/// and will be used as a key into the string resources.
		/// </param>
		/// <param name="uniqueKeyMembers">
		/// An array of property names that form the unique key for the class.  For example, a Person class
		/// might have a unique key consisting of "FirstName" and "LastName" properties.  Compound
		/// property expressions may be used, e.g. for a Person class with a Name property that itself has First
		/// and Last properties, the unique key members might be "Name.First" and "Name.Last".
		/// </param>
		internal UniqueKeySpecification(Type entityClass, string logicalKeyName, string[] uniqueKeyMembers)
		{
			_entityClass = entityClass;
			_uniqueKeyMembers = uniqueKeyMembers;
			_logicalKeyName = logicalKeyName;
		}

		public TestResult Test(object obj)
		{
			var context = PersistenceScope.CurrentContext;
			if (context == null)
				throw new SpecificationException(SR.ExceptionPersistenceContextRequired);

			var broker = context.GetBroker<IUniqueConstraintValidationBroker>();
			var valid = broker.IsUnique(obj, _entityClass, _uniqueKeyMembers);

			return valid ? new TestResult(true) : new TestResult(false, new TestResultReason(GetMessage(obj)));
		}

		protected virtual string GetMessage(object obj)
		{
			return string.Format(SR.RuleUniqueKey, TerminologyTranslator.Translate(obj.GetClass(), _logicalKeyName),
				TerminologyTranslator.Translate(_entityClass));
		}
	}
}
