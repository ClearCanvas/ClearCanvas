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

using System.Collections.Generic;
using System.IO;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Represents a configuration document.
	/// </summary>
	public interface IConfigurationDocument
	{
		/// <summary>
		/// Gets the document header.
		/// </summary>
		ConfigurationDocumentHeader Header { get; }

		/// <summary>
		/// Gets the entire content of the document as a string.
		/// </summary>
		/// <returns></returns>
		string ReadAll();

		/// <summary>
		/// Gets a reader that can read the document content.
		/// </summary>
		/// <returns></returns>
		TextReader GetReader();
	}



	/// <summary>
	/// Defines the interface to a mechanism for the storage of configuration data.
	/// </summary>
	/// <remarks>
	/// This interface is more general purpose than <see cref="ISettingsStore"/>, in that it allows storage
	/// of arbitrary configuration "documents" that need not conform to any particular structure.
	/// </remarks>
	public interface IConfigurationStore
	{
		/// <summary>
		/// Lists documents in the configuration that match the specified query.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ConfigurationDocumentHeader> ListDocuments(ConfigurationDocumentQuery query);

		/// <summary>
		/// Retrieves the specified document.
		/// </summary>
		/// <exception cref="ConfigurationDocumentNotFoundException">The requested document does not exist.</exception>
		IConfigurationDocument GetDocument(ConfigurationDocumentKey documentKey);

		/// <summary>
		/// Stores the specified document.
		/// </summary>
		void PutDocument(ConfigurationDocumentKey documentKey, TextReader content);

		/// <summary>
		/// Stores the specified document.
		/// </summary>
		void PutDocument(ConfigurationDocumentKey documentKey, string content);
	}
}
