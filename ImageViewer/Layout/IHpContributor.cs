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

namespace ClearCanvas.ImageViewer.Layout
{
	/// <summary>
	/// Defines the base interface to an HP "contributor".  A contributor is an object that contributes
	/// to a hanging protocol.
	/// </summary>
	public interface IHpContributor : IHpSerializableElement
	{
		/// <summary>
		/// Gets a GUID identifying this class of contributor (must return a constant value) for serialization purposes.
		/// </summary>
		Guid ContributorId { get; }

        //TODO (CR June 2011): Not sure about this, but I guess it doesn't do any harm.

        /// <summary>
        /// Gets a friendly name for the contributor that could be shown to the user, if needed.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a friendly description for the contributor that could be shown to the user, if needed.
        /// </summary>
        string Description { get; }

		/// <summary>
		/// Called by the user-interface to obtain the set of properties that can be edited by the user.
		/// </summary>
		/// <returns></returns>
		IHpProperty[] GetProperties();

		/// <summary>
		/// Gets a value indicating whether this contributor requires the patient history (prior studies)
		/// to be loaded.
		/// </summary>
		bool RequiresPatientHistory { get; }
	}
}
