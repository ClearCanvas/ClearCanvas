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
using System.Linq;

namespace ClearCanvas.ImageViewer.Common
{
	/// <summary>
	/// Represents a strongly typed list of disposable objects.
	/// </summary>
	/// <remarks>
	/// This class is provided due to the seemingly common pattern of creating a collection of
	/// disposable objects and then needing to dispose of them all at some future time, such as
	/// images, SOPs and data sources.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public class DisposableList<T> : List<T>, IDisposable
		where T : IDisposable
	{
		/// <summary>
		/// Initializes a new <see cref="DisposableList{T}"/>.
		/// </summary>
		public DisposableList() {}

		/// <summary>
		/// Initializes a new <see cref="DisposableList{T}"/>.
		/// </summary>
		/// <param name="collection"></param>
		public DisposableList(IEnumerable<T> collection)
			: base(collection) {}

		public void Dispose()
		{
			foreach (var item in this.Where(item => !ReferenceEquals(item, null)))
				item.Dispose();

			Clear();
		}

		/// <summary>
		/// Adds a new object of type <typeparamref name="T"/> to the end of the <see cref="DisposableList{T}"/>.
		/// </summary>
		/// <typeparam name="TNew"></typeparam>
		/// <returns></returns>
		public TNew AddNew<TNew>()
			where TNew : T, new()
		{
			var newItem = new TNew();
			Add(newItem);
			return newItem;
		}
	}
}