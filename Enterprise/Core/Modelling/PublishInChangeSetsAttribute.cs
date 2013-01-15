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
	/// When applied to an entity class, specifies whether that class should be published in entity change-sets.
	/// </summary>
	/// <remarks>
	/// Entity classes are published in change sets by default.  Therefore this attribute need only be applied for the purpose
	/// of excluding an entity class from change-sets.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class PublishInChangeSetsAttribute : Attribute
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="isPublishable"></param>
		public PublishInChangeSetsAttribute(bool isPublishable)
		{
			IsPublishable = isPublishable;
		}

		/// <summary>
		/// Gets a value indicating whether the entity is publishable.
		/// </summary>
		public bool IsPublishable { get; private set; }
	}
}
