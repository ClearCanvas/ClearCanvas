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
    /// Defines the interface to a specialized broker that can validate a unique constraint for an entity.
    /// </summary>
    public interface IUniqueConstraintValidationBroker : IPersistenceBroker
    {
    	/// <summary>
    	/// Tests whether the specified object satisfies the specified unique constraint.
    	/// </summary>
		/// <param name="domainObject">The object to test.</param>
    	/// <param name="entityClass">The class of entity to which the constraint applies.</param>
    	/// <param name="uniqueConstraintMembers">The properties of the object that form the unique key.
    	/// These may be compound property expressions (e.g. Name.FirstName, Name.LastName).
    	/// </param>
    	/// <returns></returns>
    	bool IsUnique(object domainObject, Type entityClass, string[] uniqueConstraintMembers);
    }
}
