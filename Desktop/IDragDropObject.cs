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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Interface for objects representing a generic, multi-format object wrapper.
	/// </summary>
	/// <remarks>
	/// This interface is used in situations where interactions between components
	/// require the runtime selection of a data format suitable for both components
	/// involved, such as a drag and drop or clipboard copy paste to unknown components.
	/// </remarks>
	public interface IDragDropObject
	{
		/// <summary>
		/// Gets a string array of format descriptors available in this wrapper.
		/// </summary>
		/// <returns>A string array of format descriptors.</returns>
		string[] GetFormats();

		/// <summary>
		/// Gets the data encapsulated in this wrapper in the specified format.
		/// </summary>
		/// <param name="type">The type of the data to extract.</param>
		/// <returns>An object of type <paramref name="type"/>, or null if the data is not available in the specified format.</returns>
		object GetData(Type type);

		/// <summary>
		/// Gets the data encapsulated in this wrapper in the specified format.
		/// </summary>
		/// <param name="format">The format descriptor of the data to extract.</param>
		/// <returns>An object matching the specified <paramref name="format"/>, or null if the data is not available in the specified format.</returns>
		object GetData(string format);

		/// <summary>
		/// Gets the data encapsulated in this wrapper in the specified format.
		/// </summary>
		/// <typeparam name="T">The type of the data to extract.</typeparam>
		/// <returns>An object of type <typeparamref name="T"/>, or null if the data is not available in the specified format.</returns>
		T GetData<T>();

		/// <summary>
		/// Checks if the data encapsulated in this wrapper is available in the specified format.
		/// </summary>
		/// <param name="type">The type of the data to check for.</param>
		/// <returns>True if an object of type <paramref name="type"/> is available; False if the data is not available in the specified format.</returns>
		bool HasData(Type type);

		/// <summary>
		/// Checks if the data encapsulated in this wrapper is available in the specified format.
		/// </summary>
		/// <param name="format">The format descriptor of the data to check for.</param>
		/// <returns>True if an object matching the specified <paramref name="format"/> is available; False if the data is not available in the specified format.</returns>
		bool HasData(string format);

		/// <summary>
		/// Checks if the data encapsulated in this wrapper is available in the specified format.
		/// </summary>
		/// <typeparam name="T">The type of the data to check for.</typeparam>
		/// <returns>True if an object of type <typeparamref name="T"/> is available; False if the data is not available in the specified format.</returns>
		bool HasData<T>();

		/// <summary>
		/// Sets the data in the wrapper using the object's type as the format.
		/// </summary>
		/// <param name="data">The data object to encapsulate.</param>
		void SetData(object data);

		/// <summary>
		/// Sets the data in the wrapper using the specified format descriptor.
		/// </summary>
		/// <param name="format">The format descriptor to encapsulate the data as.</param>
		/// <param name="data">The data object to encapsulate.</param>
		void SetData(string format, object data);

		/// <summary>
		/// Sets the data in the wrapper using the specified type as the format.
		/// </summary>
		/// <param name="type">The type to encapsulate the data as.</param>
		/// <param name="data">The data object to encapsulate.</param>
		void SetData(Type type, object data);
	}
}