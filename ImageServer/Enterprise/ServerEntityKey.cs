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

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Class representing the primary key of a <see cref="ServerEntity"/> object.
    /// </summary>
    [Serializable]
    public class ServerEntityKey
    {
        #region Private Members

        private readonly object _key;
        private readonly String _name;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the <see cref="ServerEntity"/>.</param>
        /// <param name="entityKey">The primary key object itself.</param>
        public ServerEntityKey(String name, object entityKey)
        {
            _name = name;
            _key = entityKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The primary key itself.
        /// </summary>
        public object Key
        {
            get { return _key; }
        }

        /// <summary>
        /// The name of the <see cref="ServerEntity"/>.
        /// </summary>
        public String EntityName
        {
            get { return _name; }
        }

        #endregion

        #region Public Overrides

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ServerEntityKey key = obj as ServerEntityKey;
            if (key == null) return false;

            return _key.Equals(key.Key);
        }

        public override string ToString()
        {
            return _key.ToString();
        }

        #endregion
    }
}
