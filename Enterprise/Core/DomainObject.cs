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

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Base class for <see cref="Entity"/>, <see cref="ValueObject"/> and <see cref="EnumValue"/>.
	/// </summary>
	/// 
	[Serializable] // TH (Oct 5, 2007): All entity objects should be serializable to use in ASP.NET app
	public abstract class DomainObject
	{
		/// <summary>
		/// Get the unproxied object reference.
		/// </summary>
		/// <remarks>
		/// In the case where this object is a proxy, returns the raw instance underlying the proxy.  This
		/// method must be virtual for correct behaviour, however, it is not intended to be overridden by
		/// subclasses and is not intended for use by application code.
		/// </remarks>
		/// <returns></returns>
		protected virtual DomainObject GetRawInstance()
		{
			// because GetRawInstance is virtual, 'this' refers to the raw instance
			return this;
		}

		/// <summary>
		/// Gets the domain class of this object.
		/// </summary>
		/// <remarks>
		/// The domain class is not necessarily the same as the type of this object, because this object may be a proxy.
		/// Therefore, use this method rather than <see cref="object.GetType"/>.
		/// </remarks>
		/// <returns></returns>
		public virtual Type GetClass()
		{
			// because GetClass is virtual, 'this' refers to the raw instance
			return this.GetType();
		}
	}
}
