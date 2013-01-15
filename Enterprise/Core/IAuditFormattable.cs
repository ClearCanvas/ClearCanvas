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
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Defines an interface to an object that is formattable for auditing purposes.
	/// </summary>
	/// <remarks>
	/// This interface is intended to be implemented by classes that are part of the domain model and
	/// wish to specify custom formatting for auditing purposes. Note that entities should not implement
	/// this interface because entities are not audited as objects.  Rather, a change-set captures a set of changes
	/// to individual properties of entities.  Hence, it is the classes that are used as the properties
	/// of entities (typically but not necessarily subclasses of <see cref="ValueObject"/>) that should 
	/// implement this interface.
	/// </remarks>
	public interface IAuditFormattable
	{
		/// <summary>
		/// Asks the implementor to write itself to the specified object writer.
		/// </summary>
		/// <param name="writer"></param>
		void Write(IObjectWriter writer);
	}
}
