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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    [Serializable] // TH (Oct 5, 2007): All entity objects should be serializable to use in ASP.NET app
    public abstract class ServerEntity : Entity
    {
        #region Private Members
        private readonly object _syncRoot = new object();
        private ServerEntityKey _key;
        private readonly String _name;

        #endregion

        #region Constructors

        public ServerEntity(String name)
        {
            _name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The name of the ServerEntity object.
        /// </summary>
        public String Name
        {
            get { return _name; }
        }

        public ServerEntityKey Key
        {
            get { return _key; }
        }

        protected object SyncRoot
        {
            get { return _syncRoot; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the primary key of the ServerEntity object.
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(ServerEntityKey key)
        {
            _key = key;
        }

        /// <summary>
        /// Get the primary key of the ServerEntity object.
        /// </summary>
        /// <returns>A <see cref="ServerEntityKey"/> object representating the primary key.</returns>
        public ServerEntityKey GetKey()
        {
            if (_key == null)
                throw new InvalidOperationException("Cannot generate entity ref on transient entity");

            return _key;
        }

        /// <summary>
        /// Not supported by ServerEntity objects
        /// </summary>
        /// <returns></returns>
        public override EntityRef GetRef()
        {
            throw new InvalidOperationException("Not supported by ServerEntity");
        }

        #endregion
    }
}
