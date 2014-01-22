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
    /// Defines the interface to a persistent store, and acts as a factory for obtaining instances of 
    /// <see cref="IReadContext"/> and <see cref="IUpdateContext"/>.
    /// </summary>
    public interface IPersistentStore
    {
        /// <summary>
        /// Called by the framework to initialize the persistent store.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Obtains an <see cref="IReadContext"/> for use by the application to interact
        /// with this persistent store.
        /// </summary>
        /// <returns>a read context</returns>
        IReadContext OpenReadContext();

        /// <summary>
        /// Obtains an <see cref="IUpdateContext"/> for use by the application to interact
        /// with this persistent store.
        /// </summary>
        /// <returns>a update context</returns>
        IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode);

		/// <summary>
		/// The version associated with the persistent store.
		/// </summary>
		Version Version { get; }
    }
}
