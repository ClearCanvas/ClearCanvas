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
using System.Collections;
using System.Collections.Generic;

using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common;


namespace ClearCanvas.Enterprise.Configuration {


    /// <summary>
    /// Stores a set of settings keys and values for a given settings group.  Used internally by the framework.
    /// </summary>
    [UniqueKey("DocumentKey", new string[]{"DocumentName", "DocumentVersionString", "User", "InstanceKey"})]
	public partial class ConfigurationDocument : Entity
	{
        /// <summary>
        /// Constructs a new configuration document with an empty body.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="versionString"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        public ConfigurationDocument(string name, string versionString, string user, string instanceKey)
        {
            _documentName = name;
            _documentVersionString = versionString;
            _user = user;
            _instanceKey = instanceKey;
        	_creationTime = Platform.Time;
			_body = new ConfigurationDocumentBody(this, null, _creationTime);
        }


		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		#region Object overrides
		
		public override bool Equals(object obj)
		{
            ConfigurationDocument that = obj as ConfigurationDocument;
            if (that == null)
                return false;

            return this._documentName == that._documentName && this._documentVersionString == that._documentVersionString
                && this._instanceKey == that._instanceKey && this._user == that._user;
		}
		
		public override int GetHashCode()
		{
            int hash = _documentName.GetHashCode();

            if (_documentVersionString != null)
                hash ^= _documentVersionString.GetHashCode();
            if (_user != null)
                hash ^= _user.GetHashCode();
            if (_instanceKey != null)
                hash ^= _instanceKey.GetHashCode();
            return hash;
		}
		
		#endregion
    }
}